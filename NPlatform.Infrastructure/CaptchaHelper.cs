using System.Net;
using System.IO;
using System.Net.Http;
using SkiaSharp;

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
        /// <param name="imageSize">图片大小</param>
        /// <returns></returns>
        public bool CheackCaptch(CharInfo[] old, CharInfo[] subData, int imageSize)
        {
            if(old == null || subData == null) return false;
            if(old.Length != subData.Length) return false;
            for(var i=0;i<old.Length; i++)
            {
                if (Math.Abs(old[i].X - subData[i].X) > imageSize / 2) return false;
                if (Math.Abs(old[i].Y - subData[i].Y) > imageSize / 2) return false;
            }
            return true;
        }

        /// <summary>
        /// 创建验证码图片
        /// </summary>
        /// <param name="backgroundPath">背景图JPG格式</param>
        /// <param name="skimage">png格式小图片</param>
        /// <returns></returns>
        public (string, CharInfo[]) CreateBase64Captch(string backgroundPath, SKImage[] skimage,int imageSize)
        {
            if (skimage is null)
            {
                throw new ArgumentNullException(nameof(skimage));
            }
            // 创建一个背景图
            SKBitmap background = BgImage(backgroundPath);
            try
            {
                var width = background.Width;
                var height = background.Height;
                // 在背景图上创建SKCanvas对象
                using (var surface = SKSurface.Create(new SKImageInfo(background.Width, background.Height)))
                {
                    var canvas = surface.Canvas;
                    canvas.DrawBitmap(background, new SKPoint(0, 0));
                    var count = skimage.Length;
                    var widthArrays = GenerateRandomX(count, width);
                    var heightArrays = GenerateRandomArray(count, width);
                    //随机旋转。
                    var rotationAngle = random.Next(0, 360);
                    CharInfo[] keyInfos = new CharInfo[count];
                    for (var i = 0; i < count; i++)
                    {
                        SKPoint point = new SKPoint(widthArrays[i], heightArrays[i]);

                        var img = skimage[i];
                        keyInfos[i] = new CharInfo() { Index = EncodeImageToBase64(img), X = point.X + skimage[i].Width / 2, Y = point.Y + skimage[i].Height / 2 };
                        //旋转。
                        canvas.RotateDegrees(rotationAngle, point.X + skimage[i].Width / 2, point.Y + skimage[i].Height / 2);

                        canvas.DrawImage(skimage[i], point);
                        canvas.ResetMatrix();
                    }

                    var base64String = EncodeSurfaceToBytes(surface);
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
        /// 创建验证码图片
        /// </summary>
        /// <param name="backgroundPath">背景图</param>
        /// <param name="count">验证文字个数。</param>
        /// <param name="fontSize">字体大小</param>
        /// <returns></returns>
        public (string, CharInfo[]) CreateBase64Captch(string backgroundPath,int count,int fontSize)
        {
            // 创建一个背景图
            SKBitmap background = BgImage(backgroundPath);
            var width = background.Width;
            var height = background.Height;
            try
            {
                // 在背景图上创建SKCanvas对象
                using (var surface = SKSurface.Create(new SKImageInfo(background.Width, background.Height)))
                {
                    var canvas = surface.Canvas;
                    canvas.DrawBitmap(background, new SKPoint(0, 0));

                    var widthArrays = GenerateRandomX(count, width);
                    var heightArrays = GenerateRandomArray(count, width);

                    // 生成随机汉字
                    var chars = GenerateRandomChinese(count);


                    var rotationAngle = random.Next(0, 360);
                    CharInfo[] points = new CharInfo[count];
                    for (var i = 0; i < chars.Length; i++)
                    {
                        SKPoint point = new SKPoint(widthArrays[i], heightArrays[i]);

                        string text = chars[i]; // 需要绘制的汉字
                        points[i] = new CharInfo() { Index = text, X = point.X + fontSize / 2, Y = point.Y + fontSize / 2 };

                        SKPaint paint = new SKPaint
                        {
                            Typeface = SKTypeface.FromFamilyName("黑体"),
                            TextSize = fontSize,
                            IsAntialias = true,
                            Color = SKColors.Red,

                        };
                        canvas.RotateDegrees(rotationAngle, point.X + fontSize / 2, point.Y + fontSize / 2);
                        // 在背景图上绘制汉字
                        canvas.DrawText(text, point, paint);
                        canvas.ResetMatrix();
                    }

                    var base64String= EncodeSurfaceToBytes(surface);

                    // 输出 Base64 字符串
                    return (base64String, points);
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
        private string EncodeImageToBase64(SKImage image)
        {
            using (var data = image.Encode())
            {
                byte[] imageBytes = data.ToArray();
                var mime = GetImageMimeType(imageBytes);
                return $"data:{mime};base64,{Convert.ToBase64String(imageBytes)}";
            }
        }

        private string EncodeSurfaceToBytes(SKSurface surface)
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

        private string GetImageMimeType(byte[] imageBytes)
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

        private string[] GenerateRandomChinese(int count)
        {
            HashSet<string> uniqueCharacters = new HashSet<string>();

            while (uniqueCharacters.Count < count)
            {
                int unicodeValue = random.Next(0x4e00, 0x9fff + 1);
                string character = char.ConvertFromUtf32(unicodeValue);

                if (" "!= character )
                {
                    uniqueCharacters.Add(character);
                }
            }

            return uniqueCharacters.ToArray();
        }
        private int[] GenerateRandomX(int count, int max)
        {
            HashSet<int> points = new HashSet<int>();
            int x = 1;
            int span = max / count;

            while (points.Count < count)
            {
                points.Add(random.Next(x, (points.Count+1)*span));
                x = points.Count*span;
            }
            return points.ToArray();
        }
        private int[] GenerateRandomArray(int count,int max)
        {
            HashSet<int> points = new HashSet<int>();
            while (points.Count < count)
            {
                points.Add(random.Next(1,max));
            }
            return points.ToArray();
        }

        /// <summary>
        /// 获取背景图
        /// </summary>
        /// <returns></returns>
        private SKBitmap BgImage(string backgroundPath)
        {
            var images = System.IO.Directory.GetFiles(backgroundPath,"*.jpg");

            if (images.Length == 0)
            {
                throw new Exception("验证码初始化失败！缺少|*.jpg|格式的背景图片。");
            }

            int num = random.Next(1, (images?.Length) ?? 2);

            string path = images[num - 1];
            return SKBitmap.Decode(path);
        }


    }

}
