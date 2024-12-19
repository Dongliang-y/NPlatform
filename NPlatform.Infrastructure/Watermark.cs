using SkiaSharp;
namespace NPlatform.Infrastructure
{
    public class ImageWatermark
    {
        public string InputImagePath { get; set; }
        public string OutputImagePath { get; set; }
        public int BottomSpace { get; set; } = 10;
        public int RightSpace { get; set; } = 10;
        public int WaterWidth { get; set; } = 200;
        public int WaterHeight { get; set; } = 80;
        public int WaterTextSize { get; set; } = 24;

        private int _transparency = 70;
        public int Transparency
        {
            get => _transparency;
            set
            {
                if (value < 0 || value > 100)
                    throw new ArgumentOutOfRangeException(nameof(Transparency), "Transparency must be between 0 and 100.");
                _transparency = value;
            }
        }

        private static string DefaultFontPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "msyh.ttc");

        private byte CalculateAlpha() => (byte)(255 * Transparency / 100);

        public async Task AddWatermarkAsync(string watermarkText)
        {
            if (string.IsNullOrWhiteSpace(InputImagePath) || !File.Exists(InputImagePath))
                throw new FileNotFoundException($"Input image not found: {InputImagePath}");

            if (string.IsNullOrWhiteSpace(OutputImagePath))
                throw new ArgumentException("Output image path must be specified.", nameof(OutputImagePath));

            if (!Directory.Exists(Path.GetDirectoryName(OutputImagePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(OutputImagePath));

            using var originalBitmap = SKBitmap.Decode(InputImagePath);
            if (originalBitmap == null)
                throw new Exception("Failed to decode input image.");

            using var watermarkBitmap = await CreateWatermarkAsync(watermarkText);
            using var canvas = new SKCanvas(originalBitmap);

            int x = originalBitmap.Width - watermarkBitmap.Width - RightSpace;
            int y = originalBitmap.Height - watermarkBitmap.Height - BottomSpace;

            using var paint = new SKPaint { Color = SKColors.White.WithAlpha(CalculateAlpha()), IsAntialias = true };
            canvas.DrawBitmap(watermarkBitmap, new SKPoint(x, y), paint);

            using var image = SKImage.FromBitmap(originalBitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            using var stream = File.OpenWrite(OutputImagePath);

            data.SaveTo(stream);
        }

        private async Task<SKBitmap> CreateWatermarkAsync(string text)
        {
            var bitmap = new SKBitmap(WaterWidth, WaterHeight); // 不放入 using，返回未释放的对象
            using (var canvas = new SKCanvas(bitmap))
            using (var paint = new SKPaint
            {
                TextSize = WaterTextSize,
                IsAntialias = true,
                Color = new SKColor(17, 17, 17),
                Typeface = SKTypeface.FromFile(DefaultFontPath) ?? SKTypeface.Default
            })
            {
                canvas.Clear(SKColors.Transparent);
                canvas.RotateDegrees(-30, WaterWidth / 2, WaterHeight / 2);
                canvas.DrawText(text, WaterWidth / 4, WaterHeight / 2, paint);
            }

            return bitmap;
        }
    }

}