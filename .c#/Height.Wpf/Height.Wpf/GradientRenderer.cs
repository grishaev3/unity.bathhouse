using System.IO;
using System.Numerics;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Height.Wpf
{
    public class GradientRenderer : FrameworkElement
    {
        private WriteableBitmap? _lastBitmap;

        private readonly Calculator _calculator;

        private readonly int _textureLength = 1025;

        public GradientRenderer()
        {
            HeightData[] heightData = [
                new(-25.00f, -1.5f),       
                new(-15.50f, -1.2f),   // низ к 3уч
                new(-4.00f, -0.8f),
                new(-2.00f, -0.6f),
                new(-0.00f, -0.4f),
                new(+5.00f, -0.0f),
                new(+18.5f, -0.25f),     // верх к 2уч
                new(+23.1f, +0.50f),     // верх к 3уч
                new(+25.0f, +0.50f),
            ];

            _calculator = new(_textureLength, 
                new Vector2(-25f, +25f), 
                new Vector2(+25f, -25f), 
                heightData);

            //SaveTo("C:\\TEMP\\1.txt");
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            _lastBitmap = GenerateBitmap(_textureLength, _textureLength);

            drawingContext.DrawImage(_lastBitmap, new Rect(0, 0, _textureLength, _textureLength));
        }

        public WriteableBitmap CreateBitmap()
        {
            if (_lastBitmap != null)
            {
                return _lastBitmap.Clone();
            }

            return GenerateBitmap(_textureLength, _textureLength);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            _lastBitmap = null; // Сбрасываем кэш

            InvalidateVisual();
        }

        private WriteableBitmap GenerateBitmap(int width, int height)
        {
            byte[] pixels = new byte[width * height * 4];
            //var y = 1024;
            for (int y = 0; y < height; y++)
            {
                byte intensity = _calculator.GetColor(default, y);

                for (int x = 0; x < width; x++)
                {
                    int pixelIndex = (y * width + x) * 4;
                    pixels[pixelIndex + 0] = intensity;
                    pixels[pixelIndex + 1] = intensity;
                    pixels[pixelIndex + 2] = intensity;
                    pixels[pixelIndex + 3] = 255;
                }
            }

            var bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Pbgra32, null);
            bitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, width * 4, 0);
            return bitmap;
        }

        private void SaveTo(string name)
        {
            StringBuilder sb = new StringBuilder();
            for (var f = -25f; f <= 25f; f += 1.0f)
            {
                var abs = _calculator.GatAbsoluteHeight(f);
                var norm = _calculator.GetNormalizeHeight(f);
                sb.AppendLine($"{f}: {abs.ToString()}");
            }
            File.WriteAllText(name, sb.ToString());
        }
    }
}
