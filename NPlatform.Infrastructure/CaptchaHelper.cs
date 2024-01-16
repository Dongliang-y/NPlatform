using System.Net;
using System.IO;
using System.Net.Http;
using SkiaSharp;
using Org.BouncyCastle.Utilities;
using System.Drawing;

namespace NPlatform.Domains.Services.Captchas
{
    /// <summary>
    /// 图形验证码，生成一张随机的背景图， 以及按顺序返回小图片的坐标信息。
    /// 客户端也必须按顺序提交用户选择的坐标。
    /// </summary>
    public class CaptchaHelper {

        private static readonly Random random = new Random();
        /// <summary>
        /// 验证码校验
        /// </summary>
        /// <param name="old">服务端存储的验证信息</param>
        /// <param name="subData">客户端提交的验证信息</param>
        /// <param name="tolerance">容忍差值范围。</param>
        /// <returns></returns>
        public static bool CheckCaptcha(CharInfo[] old, CharInfo[] subData, int tolerance = 12)
        {
            if (old == null || subData == null || old.Length != subData.Length || subData.Length == 0)
                return false;

            for (var i = 0; i < old.Length; i++)
            {
                if (Math.Abs(old[i].X - subData[i].X) > tolerance || Math.Abs(old[i].Y - subData[i].Y) > tolerance)
                    return false;
            }
            return true;
        }
        /// <summary>
        /// 根据提供的随机文字图片，创建验证码图片
        /// </summary>
        /// <param name="backgroundPath">背景图JPG格式</param>
        /// <param name="skimage">png格式小图片</param>
        /// <returns></returns>
        public static (string, CharInfo[]) CreateBase64Captcha(string backgroundPath, SKImage[] skimage,int imageSize)
        {
            if (skimage is null)
            {
                throw new ArgumentNullException(nameof(skimage));
            }

            // 创建一个背景图
            SKBitmap background = LoadRandomBackgroundImage(backgroundPath);
            try
            {
                var width = background.Width;
                var height = background.Height;
                using (var surface = SKSurface.Create(new SKImageInfo(background.Width, background.Height)))
                {
                    var canvas = surface.Canvas;
                    canvas.DrawBitmap(background, new SKPoint(0, 0));
                    var count = skimage.Length;
                    var points = GenerateOrderedPoints(skimage.Length,background.Width,background.Height);
                    var rotationAngle = random.Next(0, 360);
                    CharInfo[] keyInfos = new CharInfo[count];
                    for (var i = 0; i < count; i++)
                    {
                        SKPoint point = points[i];
                        var img = skimage[i];
                        keyInfos[i] = new CharInfo() { Index = EncodeImageToBase64(img), X = point.X + skimage[i].Width / 2, Y = point.Y + skimage[i].Height / 2 };
                        //旋转。
                        canvas.RotateDegrees(rotationAngle, point.X + skimage[i].Width / 2, point.Y + skimage[i].Height / 2);

                        canvas.DrawImage(skimage[i], point);
                        canvas.ResetMatrix();
                    }

                    var base64String = EncodeSurfaceToBase64(surface);
                    // 输出 Base64 字符串
                    return (base64String, keyInfos);
                }
            }
            catch(Exception ex )
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
            finally
            {
                background.Dispose();
            }
        }

        /// <summary>
        /// 随机创建指定数量文字的，创建验证码图片
        /// </summary>
        /// <param name="backgroundPath">背景图</param>
        /// <param name="count">验证文字个数。</param>
        /// <param name="fontSize">字体大小</param>
        /// <returns></returns>
        public static (string,string, CharInfo[]) CreateBase64Captcha(string backgroundPath,int count,int fontSize=32)
        {
            // 创建一个背景图
            SKBitmap background = LoadRandomBackgroundImage(backgroundPath);
            try
            {
                string tipsText= "请按顺序依次点击：";

                using (var surface = SKSurface.Create(new SKImageInfo(background.Width, background.Height)))
                {
                    var canvas = surface.Canvas;
                    canvas.DrawBitmap(background, new SKPoint(0, 0));

                    var points = GenerateOrderedPoints(count, background.Width-50, background.Height-50);
                    var chars = GenerateRandomChinese(count);

                    CharInfo[] keyInfos = new CharInfo[count];
                    for (var i = 0; i < count; i++)
                    {
                        var rotationAngle = random.Next(0, 180);
                        SKPoint point = points[i];
                        string text = chars[i];
                        tipsText += $"“{text}”，";
                        keyInfos[i] = new CharInfo() { Index = text, X = point.X + fontSize / 2, Y = point.Y + fontSize / 2 };

                        SKPaint paint = new SKPaint
                        {
                            Typeface = SKTypeface.FromFamilyName("黑体"),
                            TextSize = fontSize,
                            FakeBoldText=true,
                            IsAntialias = true,
                            Color = new SKColor((byte)random.Next(256), (byte)random.Next(256), (byte)random.Next(256)),
                        };
                        canvas.RotateDegrees(rotationAngle, point.X + fontSize / 2, point.Y + fontSize / 2);
                        canvas.Save();  // 保存画布状态
                        canvas.DrawText(text, point, paint);
                        canvas.Restore();  // 还原画布状态
                        canvas.ResetMatrix();
                    }

                    var base64String= EncodeSurfaceToBase64(surface);
                    var tips=CreateTips(tipsText);
                    return (base64String, tips, keyInfos);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw;
            }
            finally
            {
                background.Dispose();
            }
        }


        private static string CreateTips(string text)
        {
            // 图片大小
            int width = 480;
            int height = 30;

            // 创建 SKBitmap
            using (var surface = SKSurface.Create(new SKImageInfo(width, height)))
            {
                var canvas = surface.Canvas;

                // 绘制提示文字
                SKPaint paint = new SKPaint
                {
                    Typeface = SKTypeface.FromFamilyName("黑体"),
                    TextSize = 15,
                    IsAntialias = true,
                    Color = SKColors.Black,
                };

                canvas.DrawText(text, 0, 15, paint);

                // 将 SKSurface 转换为 SKImage
                var image = surface.Snapshot();
                //var fs=System.IO.File.OpenWrite(System.IO.Directory.GetCurrentDirectory() + "/tips.png");
                //image.Encode(SKEncodedImageFormat.Png, 100).SaveTo(fs);
                // 将 SKImage 编码为字节数组
                using (var stream = new MemoryStream())
                {
                    image.Encode(SKEncodedImageFormat.Png, 100).SaveTo(stream);
                    byte[] imageBytes = stream.ToArray();
                    MemoryStream ms = new MemoryStream(imageBytes);
                    // 将字节数组转换为 Base64 字符串
                    var base64String = $"data:image/png;base64,{Convert.ToBase64String(imageBytes)}";
                    return base64String;
                }
            }
        }

        /// <summary>
        /// 获取背景图
        /// </summary>
        /// <returns></returns>
        private static SKBitmap LoadRandomBackgroundImage(string backgroundPath)
        {
            var images = Directory.GetFiles(backgroundPath, "*.jpg");

            if (images.Length == 0)
                throw new Exception("验证码初始化失败！缺少|*.jpg|格式的背景图片。");

            int num = random.Next(1, images.Length + 1);
            string path = images[num - 1];
            return SKBitmap.Decode(path);
        }
        private static string EncodeImageToBase64(SKImage image)
        {
            using (var data = image.Encode())
            {
                byte[] imageBytes = data.ToArray();
                var mime = GetImageMimeType(imageBytes);
                return $"data:{mime};base64,{Convert.ToBase64String(imageBytes)}";
            }
        }

        private static string EncodeSurfaceToBase64(SKSurface surface)
        {
            using (var imageSurface = surface.Snapshot())
            {
                using (var imageData = imageSurface.Encode())
                {
                    var imageBytes= imageData.ToArray();
                    var mime = GetImageMimeType(imageBytes);
                    return $"data:{mime};base64,{Convert.ToBase64String(imageBytes)}";
                }
            }
        }

        private static string GetImageMimeType(byte[] imageBytes)
        {
            using (var codec = SKCodec.Create(new SKMemoryStream(imageBytes)))
            {
                byte[] signature = new byte[4];
                codec.GetPixels(new SKImageInfo(1, 1), signature);

                if (signature.Length >= 2)
                {
                    if (signature[0] == 0xFF && signature[1] == 0xD8)
                    {
                        return "image/jpeg";
                    }

                    if (signature[0] == 0x89 && signature[1] == 0x50)
                    {
                        return "image/png";
                    }
                }

                return "image/png"; // 默认为 PNG 类型
            }
        }

        private static string[] GenerateRandomChinese(int count)
        {
            HashSet<string> uniqueCharacters = new HashSet<string>();
            // 获取GB2312编码页（表）
            Encoding gb = Encoding.GetEncoding("gb2312");

            object[] bytes = CreateRegionCode(count);

            for (int i = 0; i < count; i++)
            {
                string temp = gb.GetString((byte[])Convert.ChangeType(bytes[i], typeof(byte[])));
                uniqueCharacters.Add(temp);
            }

            return uniqueCharacters.ToArray();
        }
        /**
        此函数在汉字编码范围内随机创建含两个元素的十六进制字节数组，每个字节数组代表一个汉字，并将
        四个字节数组存储在object数组中。
        参数：strlength，代表需要产生的汉字个数
        **/
        private static object[] CreateRegionCode(int strlength)
        {
            //定义一个字符串数组储存汉字编码的组成元素
            string[] rBase = new String[16] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f" };

            Random rnd = new Random();

            //定义一个object数组用来
            object[] bytes = new object[strlength];

            /**
             每循环一次产生一个含两个元素的十六进制字节数组，并将其放入bytes数组中
             每个汉字有四个区位码组成
             区位码第1位和区位码第2位作为字节数组第一个元素
             区位码第3位和区位码第4位作为字节数组第二个元素
            **/
            for (int i = 0; i < strlength; i++)
            {
                //区位码第1位
                int r1 = rnd.Next(11, 14);
                string str_r1 = rBase[r1].Trim();

                //区位码第2位
                rnd = new Random(r1 * unchecked((int)DateTime.Now.Ticks) + i); // 更换随机数发生器的 种子避免产生重复值
                int r2;
                if (r1 == 13)
                {
                    r2 = rnd.Next(0, 7);
                }
                else
                {
                    r2 = rnd.Next(0, 16);
                }
                string str_r2 = rBase[r2].Trim();

                //区位码第3位
                rnd = new Random(r2 * unchecked((int)DateTime.Now.Ticks) + i);
                int r3 = rnd.Next(10, 16);
                string str_r3 = rBase[r3].Trim();

                //区位码第4位
                rnd = new Random(r3 * unchecked((int)DateTime.Now.Ticks) + i);
                int r4;
                if (r3 == 10)
                {
                    r4 = rnd.Next(1, 16);
                }
                else if (r3 == 15)
                {
                    r4 = rnd.Next(0, 15);
                }
                else
                {
                    r4 = rnd.Next(0, 16);
                }
                string str_r4 = rBase[r4].Trim();

                // 定义两个字节变量存储产生的随机汉字区位码
                byte byte1 = Convert.ToByte(str_r1 + str_r2, 16);
                byte byte2 = Convert.ToByte(str_r3 + str_r4, 16);
                // 将两个字节变量存储在字节数组中
                byte[] str_r = new byte[] { byte1, byte2 };

                // 将产生的一个汉字的字节数组放入object数组中
                bytes.SetValue(str_r, i);
            }

            return bytes;
        }
        private static List<SKPoint> GenerateOrderedPoints(int count, int maxWidth, int maxHeight)
        {
            var points = new List<SKPoint>();
            Random random = new Random();

            // 生成四个不重叠的随机坐标
            for (int i = 0; i < count; i++)
            {
                double x, y;
                do
                {
                    x = random.NextDouble() * maxWidth; // 在 0 到 100 之间生成随机 X 坐标
                    y = random.NextDouble() * maxHeight; // 在 0 到 100 之间生成随机 Y 坐标
                } while (HasOverlap(points, x, y));
                var pint = new SKPoint((float)x, (float)y);
                points.Add(pint);
                Console.WriteLine($"坐标 {i + 1}: ({x}, {y})");
            }
            return points;
        }

        static bool HasOverlap(List<SKPoint> coordinates, double x, double y)
        {
            foreach (var coord in coordinates)
            {
                double distance = Math.Sqrt(Math.Pow((coord.X - x), 2) + Math.Pow((coord.Y - y), 2));
                if (distance < 5.0) // 设定一个阈值，表示两个坐标之间的最小距离
                {
                    return true; // 有重叠
                }
            }
            return false; // 没有重叠
        }
    }
    public class CharInfo
    {
        public string Index { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
    }

}
