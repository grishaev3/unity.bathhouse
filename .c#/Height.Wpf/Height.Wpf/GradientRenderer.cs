using System.Numerics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Height.Wpf
{
    public class GradientRenderer : FrameworkElement
    {
        private WriteableBitmap? _lastBitmap;

        private readonly Calculator _calculator;

        private readonly int _perlinSeed = 42;

        private readonly int _textureLength = 1025;

        private readonly Vector2 _terrainTopLeft = new(-25f, +25f);
        private readonly Vector2 _terrainBottomRight = new(+25f, -25f);

        public GradientRenderer()
        {
            HeightData[] heightData = new HeightData[] {
                new(_terrainTopLeft.X, -1.50f),
                new(-23.5f,  -1.35f),
                new(-18.5f,  -1.25f),
                new(-15.50f, -1.20f),    // низ к 3уч
                new(-11.25f, -1.00f),    // низ к 1уч

                new(-4.00f, -0.8f),
                new(-2.00f, -0.6f),
                new(-0.00f, -0.4f),
                new(+2.00f, -0.2f),
                new(+4.00f, -0.0f),

                new(11.25f, +0.10f),
                new(15.50f, +0.20f),
                new(+18.5f, +0.25f),    // верх к 1уч
                new(+23.5f, +0.35f),    // верх к 3уч
                new(_terrainBottomRight.X, +0.50f)
            };

            _calculator = new(_textureLength, _terrainTopLeft, 
                _terrainBottomRight, heightData, _perlinSeed);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            _lastBitmap ??= GenerateBitmap(_textureLength, _textureLength);

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
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    byte intensity = _calculator.GetColor(x, y);

                    int pixelIndex = (y * width + x) * 4;
                    pixels[pixelIndex + 0] = intensity;
                    pixels[pixelIndex + 1] = intensity;
                    pixels[pixelIndex + 2] = intensity;
                    pixels[pixelIndex + 3] = byte.MaxValue;
                }
            }

            var bitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Pbgra32, null);
            bitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, width * 4, 0);
            return bitmap;
        }
    }
}
