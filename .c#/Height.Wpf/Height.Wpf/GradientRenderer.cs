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

        private readonly int _textureLength = 1024;

        public GradientRenderer()
        {
            _calculator = new(_textureLength, new Vector2(-25f, +25f), new Vector2(+25f, -25f), 
            [
                new(+20f, +0.0f), //new(+20f, +0.5f),
                new(-0f, -0.4f),
                new(-2f, -0.6f),
                new(-4f, -0.8f),
                new(-15f, -1.0f),
            ]);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            double renderWidth = Math.Max(ActualWidth, _textureLength);
            double renderHeight = Math.Max(ActualHeight, _textureLength);

            if (renderWidth < 4 || renderHeight < 4)
            {
                return;
            }

            int width = (int)Math.Floor(renderWidth);
            int height = (int)Math.Floor(renderHeight);

            GenerateBitmap(width, height);

            drawingContext.DrawImage(_lastBitmap, new Rect(0, 0, renderWidth, renderHeight));
        }

        // Публичный метод для получения bitmap для сохранения
        public WriteableBitmap CreateBitmap()
        {
            if (_lastBitmap != null)
            {
                return _lastBitmap.Clone();
            }

            return GenerateBitmap(_textureLength, _textureLength);
        }

        private WriteableBitmap GenerateBitmap(int width, int height)
        {
            byte[] pixels = new byte[width * height * 4];
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //byte intensity = (byte)(y * 255 / (height - 1));
                    byte intensity = 0;

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

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            _lastBitmap = null; // Сбрасываем кэш

            InvalidateVisual();
        }
    }
}
