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

        public bool CheckCaptcha(CharInfo[] old, CharInfo[] subData, int imageSize)
        {
            if (old == null || subData == null || old.Length != subData.Length)
                return false;

            for (var i = 0; i < old.Length; i++)
            {
                if (Math.Abs(old[i].X - subData[i].X) > imageSize / 2 || Math.Abs(old[i].Y - subData[i].Y) > imageSize / 2)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 创建验证码图片
        /// </summary>
        /// <param name="backgroundPath">背景图JPG格式</param>
        /// <param name="skimage">png格式小图片</param>
        /// <returns></returns>
        public (string, CharInfo[]) CreateBase64Captcha(string backgroundPath, SKImage[] skimage,int imageSize)
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
        /// 创建验证码图片
        /// </summary>
        /// <param name="backgroundPath">背景图</param>
        /// <param name="count">验证文字个数。</param>
        /// <param name="fontSize">字体大小</param>
        /// <returns></returns>
        public (string, CharInfo[]) CreateBase64Captcha(string backgroundPath,int count,int fontSize)
        {
            // 创建一个背景图
            SKBitmap background = LoadRandomBackgroundImage(backgroundPath);
            try
            {
                using (var surface = SKSurface.Create(new SKImageInfo(background.Width, background.Height)))
                {
                    var canvas = surface.Canvas;
                    canvas.DrawBitmap(background, new SKPoint(0, 0));

                    var points = GenerateOrderedPoints(count, background.Width, background.Height);
                    var chars = GenerateRandomChinese(count);
                    var rotationAngle = random.Next(0, 360);
                    CharInfo[] keyInfos = new CharInfo[count];
                    for (var i = 0; i < count; i++)
                    {
                        SKPoint point = points[i];

                        string text = chars[i]; 
                        keyInfos[i] = new CharInfo() { Index = text, X = point.X + fontSize / 2, Y = point.Y + fontSize / 2 };

                        SKPaint paint = new SKPaint
                        {
                            Typeface = SKTypeface.FromFamilyName("黑体"),
                            TextSize = fontSize,
                            IsAntialias = true,
                            Color = SKColors.Red,

                        };
                        canvas.RotateDegrees(rotationAngle, point.X + fontSize / 2, point.Y + fontSize / 2);
                        canvas.DrawText(text, point, paint);
                        canvas.ResetMatrix();
                    }

                    var base64String= EncodeSurfaceToBase64(surface);

                    return (base64String, keyInfos);
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


        /// <summary>
        /// 获取背景图
        /// </summary>
        /// <returns></returns>
        private SKBitmap LoadRandomBackgroundImage(string backgroundPath)
        {
            var images = Directory.GetFiles(backgroundPath, "*.jpg");

            if (images.Length == 0)
                throw new Exception("验证码初始化失败！缺少|*.jpg|格式的背景图片。");

            int num = random.Next(1, images.Length + 1);
            string path = images[num - 1];
            return SKBitmap.Decode(path);
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

        private string EncodeSurfaceToBase64(SKSurface surface)
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

        private List<SKPoint> GenerateOrderedPoints(int count, int maxWidth, int maxHeight)
        {
            var points = new List<SKPoint>();
            var spanSum = 0;
            var maxSpan = maxWidth / count;

            for (var i = 0; i < count; i++)
            {
                var span = random.Next(1, maxSpan);
                spanSum += span;
                float x = spanSum;

                x = Math.Min(x, maxWidth - 1);

                if (i > 0)
                {
                    var minDistance = 20; // 调整这个值以确保坐标之间的最小距离
                    var lastX = points[i - 1].X;
                    if (x - lastX < minDistance)
                    {
                        x = lastX + minDistance;
                    }
                }

                var y = random.Next(1, maxHeight);
                y = Math.Min(y, maxHeight - 1);

                points.Add(new SKPoint(x, y));
            }

            return points;
        }
    }
    public class CharInfo
    {
        public string Index { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
    }

}
