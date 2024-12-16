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

        /// <summary>
        /// ��ʼ����ˮӡ
        /// </summary>
        /// <example>
        /// <code>
        ///     Watermark wm = new Watermark();
        ///     wm.DrawedImagePath= Server.MapPath("") + "/upfile/" + "backlogo.gif";
        ///     wm.ModifyImagePath=path; 
        ///     wm.RightSpace=184;
        ///     wm.BottoamSpace=81;
        ///     wm.LucencyPercent=50;
        ///     wm.OutPath=Server.MapPath("") + "/upfile/" + fileName + "_new" + extension;
        ///     wm.DrawImage();
        ///     
        ///     //�����ˮӡ�����ͼƬ,ɾ��ԭʼͼƬ 
        ///     mFileName=fileName + "_new" + extension;
        ///     if(File.Exists(path)) {  File.Delete(path); } 
        /// </code>
        /// </example>
        public void DrawImage()
        {
            Image modifyImage = null;
            Image drawedImage = null;
            Graphics g = null;
            try
            {
                modifyImage = Image.FromFile(this.ModifyImagePath); //����ͼ�ζ���
                drawedImage = Image.FromFile(this.DrawedImagePath);
                g = Graphics.FromImage(modifyImage);

                int x = modifyImage.Width - this.rightSpace; //��ȡҪ����ͼ������
                int y = modifyImage.Height - this.BottoamSpace;

                float[][] matrixItems =
                    {
                        //������ɫ����
                        new float[] { 1, 0, 0, 0, 0 }, new float[] { 0, 1, 0, 0, 0 }, new float[] { 0, 0, 1, 0, 0 },
                        new float[] { 0, 0, 0, (float)this.LucencyPercent / 100f, 0 }, new float[] { 0, 0, 0, 0, 1 }
                    };

                ColorMatrix colorMatrix = new ColorMatrix(matrixItems);
                ImageAttributes imgAttr = new ImageAttributes();
                imgAttr.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                g.DrawImage( //������Ӱͼ��
                    drawedImage,
                    new Rectangle(x, y, drawedImage.Width, drawedImage.Height),
                    0,
                    0,
                    drawedImage.Width,
                    drawedImage.Height,
                    GraphicsUnit.Pixel,
                    imgAttr);

                string[] allowImageType = { ".jpg", ".gif", ".png", ".bmp", ".tiff", ".wmf", ".ico" }; //�����ļ�
                FileInfo file = new FileInfo(this.ModifyImagePath);
                ImageFormat imageType = ImageFormat.Gif;
                switch (file.Extension.ToLower())
                {
                    case ".jpg":
                        imageType = ImageFormat.Jpeg;
                        break;
                    case ".gif":
                        imageType = ImageFormat.Gif;
                        break;
                    case ".png":
                        imageType = ImageFormat.Png;
                        break;
                    case ".bmp":
                        imageType = ImageFormat.Bmp;
                        break;
                    case ".tif":
                        imageType = ImageFormat.Tiff;
                        break;
                    case ".wmf":
                        imageType = ImageFormat.Wmf;
                        break;
                    case ".ico":
                        imageType = ImageFormat.Icon;
                        break;
                    default: break;
                }

                MemoryStream ms = new MemoryStream();
                modifyImage.Save(ms, imageType);
                byte[] imgData = ms.ToArray();
                modifyImage.Dispose();
                drawedImage.Dispose();
                g.Dispose();
                FileStream fs = null;
                if (this.OutPath == null || this.OutPath == "")
                {
                    File.Delete(this.ModifyImagePath);
                    fs = new FileStream(this.ModifyImagePath, FileMode.Create, FileAccess.Write);
                }
                else
                {
                    fs = new FileStream(this.OutPath, FileMode.Create, FileAccess.Write);
                }

                if (fs != null)
                {
                    fs.Write(imgData, 0, imgData.Length);
                    fs.Close();
                }
            }
            finally
            {
                try
                {
                    drawedImage.Dispose();
                    modifyImage.Dispose();
                    g.Dispose();
                }
                catch
                {
                }
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
    }
}