namespace NPlatform.Infrastructure
{
    using SkiaSharp;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Security.Claims;

    /// <summary>
    /// 添加水印类 只支持添加图片水印
    /// </summary>
    public class Watermark
    {
        private int bottoamSpace;

        private int lucencyPercent = 70;

        private string modifyImagePath = null;

        private string outPath = null;

        private int rightSpace;

        /// <summary>
        /// 构造函数
        /// </summary>
        public Watermark()
        {
        }

        /// <summary>
        /// 获取或设置水印在修改图片中距底部的高度
        /// </summary>
        public int BottoamSpace
        {
            get
            {
                return this.bottoamSpace;
            }
            set
            {
                this.bottoamSpace = value;
            }
        }

        /// <summary>
        /// 获取或设置要绘制水印的透明度,注意是原来图片透明度的百分比
        /// </summary>
        public int LucencyPercent
        {
            get
            {
                return this.lucencyPercent;
            }
            set
            {
                if (value >= 0 && value <= 100) this.lucencyPercent = value;
            }
        }

        /// <summary>
        /// 获取或设置要修改的图像路径
        /// </summary>
        public string ModifyImagePath
        {
            get
            {
                return this.modifyImagePath;
            }
            set
            {
                this.modifyImagePath = value;
            }
        }

        /// <summary>
        /// 获取或设置要输出图像的路径
        /// </summary>
        public string OutPath
        {
            get
            {
                return this.outPath;
            }
            set
            {
                this.outPath = value;
            }
        }

        /// <summary>
        /// 获取或设置水印在修改图片中的右边距
        /// </summary>
        public int RightSpace
        {
            get
            {
                return this.rightSpace;
            }
            set
            {
                this.rightSpace = value;
            }
        }
        private string ConvertToBase64String(SKBitmap image, SKEncodedImageFormat format)
        {
            using (SKData data = image.Encode(format, 100))
            {
                byte[] imageBytes = data.ToArray();
                string base64String = Convert.ToBase64String(imageBytes);

                return "data:image/png;base64," + base64String;
            }
        }

        private async Task<SKBitmap> CreateImage(string waterStr,int width=150,int height=80,int TextSize=24)
        {
            // 创建一个 SKBitmap 对象
            using (SKBitmap bitmap = new SKBitmap(width, height))
            {
                // 创建 SKCanvas 对象，用于在 SKBitmap 上进行绘制
                using (SKCanvas canvas = new SKCanvas(bitmap))
                {
                    // 在 SKCanvas 上绘制文字并旋转30度
                    using (SKPaint paint = new SKPaint())
                    {
                        paint.TextSize = 24;
                        paint.IsAntialias = true;
                        paint.Color = new SKColor(17, 17, 17);

                        // 指定字体文件路径为 msyh.ttc

                        var chineseFontPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "msyh.ttc");

                        if (File.Exists(chineseFontPath))
                        {
                            var typeface = SKTypeface.FromFile(chineseFontPath, 0); // 选择第一个字体（对于 TTC 文件）
                            paint.Typeface = typeface;
                        }

                        // 计算旋转后文字的边界框
                        SKRect textBounds = new SKRect();
                        paint.MeasureText(waterStr, ref textBounds);

                        // 旋转变换
                        canvas.Translate(bitmap.Width / 2, bitmap.Height / 2);
                        canvas.RotateDegrees(-30);

                        // 计算旋转后的文字位置
                        SKPoint textLocation = new SKPoint(-textBounds.MidX, -textBounds.MidY);

                        // 绘制文字
                        canvas.DrawText(waterStr, textLocation, paint);

                        // 重置变换
                        canvas.ResetMatrix();
                    }
                }
                return bitmap;
            }
        }

        /// <summary>
        /// 给图片生产水印
        /// </summary>
        public async Task AddWatermarkToImageAsync(string watermarkText)
        {
            if (string.IsNullOrEmpty(this.modifyImagePath) || !File.Exists(this.modifyImagePath))
            {
                throw new FileNotFoundException("ModifyImagePath is invalid.");
            }

            if (string.IsNullOrEmpty(this.outPath))
            {
                throw new ArgumentException("OutPath is not set.");
            }

            // Load the original image
            using (var originalBitmap = SKBitmap.Decode(this.modifyImagePath))
            {
                if (originalBitmap == null)
                {
                    throw new Exception("Failed to load the original image.");
                }

                // Create the watermark image
                using (var watermarkBitmap = await CreateImage(watermarkText))
                {
                    using (var canvas = new SKCanvas(originalBitmap))
                    {
                        int x = originalBitmap.Width - watermarkBitmap.Width - this.RightSpace;
                        int y = originalBitmap.Height - watermarkBitmap.Height - this.BottoamSpace;

                        // Set transparency
                        var paint = new SKPaint
                        {
                            Color = SKColors.White.WithAlpha((byte)(255 * this.LucencyPercent / 100)),
                            IsAntialias = true,
                        };

                        // Draw the watermark onto the original image
                        canvas.DrawBitmap(watermarkBitmap, new SKPoint(x, y), paint);
                    }

                    // Save the modified image
                    using (var image = SKImage.FromBitmap(originalBitmap))
                    using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                    using (var stream = File.OpenWrite(this.outPath))
                    {
                        data.SaveTo(stream);
                    }
                }
            }
        }

    }
}