namespace NPlatform.Infrastructure
{
    using SkiaSharp;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Security.Claims;

    /// <summary>
    /// ���ˮӡ�� ֻ֧�����ͼƬˮӡ
    /// </summary>
    public class Watermark
    {
        private int bottoamSpace;

        private int lucencyPercent = 70;

        private string modifyImagePath = null;

        private string outPath = null;

        private int rightSpace;

        /// <summary>
        /// ���캯��
        /// </summary>
        public Watermark()
        {
        }

        /// <summary>
        /// ��ȡ������ˮӡ���޸�ͼƬ�о�ײ��ĸ߶�
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
        /// ��ȡ������Ҫ����ˮӡ��͸����,ע����ԭ��ͼƬ͸���ȵİٷֱ�
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
        /// ��ȡ������Ҫ�޸ĵ�ͼ��·��
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
        /// ��ȡ������Ҫ���ͼ���·��
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
        /// ��ȡ������ˮӡ���޸�ͼƬ�е��ұ߾�
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
            // ����һ�� SKBitmap ����
            using (SKBitmap bitmap = new SKBitmap(width, height))
            {
                // ���� SKCanvas ���������� SKBitmap �Ͻ��л���
                using (SKCanvas canvas = new SKCanvas(bitmap))
                {
                    // �� SKCanvas �ϻ������ֲ���ת30��
                    using (SKPaint paint = new SKPaint())
                    {
                        paint.TextSize = 24;
                        paint.IsAntialias = true;
                        paint.Color = new SKColor(17, 17, 17);

                        // ָ�������ļ�·��Ϊ msyh.ttc

                        var chineseFontPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "msyh.ttc");

                        if (File.Exists(chineseFontPath))
                        {
                            var typeface = SKTypeface.FromFile(chineseFontPath, 0); // ѡ���һ�����壨���� TTC �ļ���
                            paint.Typeface = typeface;
                        }

                        // ������ת�����ֵı߽��
                        SKRect textBounds = new SKRect();
                        paint.MeasureText(waterStr, ref textBounds);

                        // ��ת�任
                        canvas.Translate(bitmap.Width / 2, bitmap.Height / 2);
                        canvas.RotateDegrees(-30);

                        // ������ת�������λ��
                        SKPoint textLocation = new SKPoint(-textBounds.MidX, -textBounds.MidY);

                        // ��������
                        canvas.DrawText(waterStr, textLocation, paint);

                        // ���ñ任
                        canvas.ResetMatrix();
                    }
                }
                return bitmap;
            }
        }

        /// <summary>
        /// ��ͼƬ����ˮӡ
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