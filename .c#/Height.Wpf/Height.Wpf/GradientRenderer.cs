using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Height.Wpf
{

    public class GradientRenderer : FrameworkElement
    {
        private WriteableBitmap? _lastBitmap;

        protected override void OnRender(DrawingContext drawingContext)
        {
            double renderWidth = Math.Max(ActualWidth, 1024);
            double renderHeight = Math.Max(ActualHeight, 1024);

            if (renderWidth < 4 || renderHeight < 4)
            {
                return;
            }

            int width = (int)Math.Floor(renderWidth);
            int height = (int)Math.Floor(renderHeight);

            byte[] pixels = new byte[width * height * 4];

            for (int y = 0; y < height; y++)
            {
                byte intensity = (byte)(y * 255 / Math.Max(height - 1, 1));

                for (int x = 0; x < width; x++)
                {
                    int pixelIndex = (y * width + x) * 4;
                    pixels[pixelIndex + 0] = intensity; // B
                    pixels[pixelIndex + 1] = intensity; // G
                    pixels[pixelIndex + 2] = intensity; // R
                    pixels[pixelIndex + 3] = 255;       // A
                }
            }

            _lastBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Pbgra32, null);
            _lastBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, width * 4, 0);

            drawingContext.DrawImage(_lastBitmap, new Rect(0, 0, renderWidth, renderHeight));
        }

        // Публичный метод для получения bitmap для сохранения
        public WriteableBitmap CreateBitmap()
        {
            if (_lastBitmap != null)
                return _lastBitmap.Clone(); // Возвращаем копию для безопасности

            // Fallback - пересоздаём если нужно
            return GenerateBitmap(1024, 1024);
        }

        private WriteableBitmap GenerateBitmap(int width, int height)
        {
            byte[] pixels = new byte[width * height * 4];
            for (int y = 0; y < height; y++)
            {
                byte intensity = (byte)(y * 255 / (height - 1));
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
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            _lastBitmap = null; // Сбрасываем кэш

            InvalidateVisual();
        }
    }
}
