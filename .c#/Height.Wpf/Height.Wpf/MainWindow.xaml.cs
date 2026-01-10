using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Height.Wpf
{
    public partial class MainWindow : Window
    {
        private readonly string _path = "e:\\Projects\\.unity\\unity.bathhouse\\Assets\\Textures\\";

        private readonly string _fileName = $"terrain.raw";

        public MainWindow()
        {
            InitializeComponent();

            FocusManager.SetFocusedElement(this, this);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && gradientRenderer != null)
            {
                SaveToRawPlanar();
                e.Handled = true;
            }
        }

        private void SaveToBmp()
        {
            var bitmap = gradientRenderer.CreateBitmap();

            string fileName = $"gradient_manual.bmp";
            string filePath = Path.Combine(_path, fileName);

            using (var fileStream = File.Create(filePath))
            {
                BmpBitmapEncoder encoder = new();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                encoder.Save(fileStream);
            }

            Title = $"Сохранено: {fileName}";
        }

        private void SaveToRawPlanar()
        {
            WriteableBitmap bitmap = gradientRenderer.CreateBitmap();

            string filePath = Path.Combine(_path, _fileName);

            int width = bitmap.PixelWidth;
            int height = bitmap.PixelHeight;
            int stride = width * 4;
            int pixelCount = width * height;

            byte[] pixels = new byte[height * stride];
            bitmap.CopyPixels(pixels, stride, 0);

            // 4 отдельных массива по каналам (planar format)
            byte[] rChannel = new byte[pixelCount];
            byte[] gChannel = new byte[pixelCount];
            byte[] bChannel = new byte[pixelCount];
            byte[] aChannel = new byte[pixelCount];

            // Разделяем interleaved BGRA → planar RGBA
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int srcIndex = y * stride + x * 4; // BGRA порядок
                    int pixelIndex = y * width + x;

                    rChannel[pixelIndex] = pixels[srcIndex + 2]; // R из BGRA[2]
                    gChannel[pixelIndex] = pixels[srcIndex + 1]; // G из BGRA[1]
                    bChannel[pixelIndex] = pixels[srcIndex + 0]; // B из BGRA[0]
                    aChannel[pixelIndex] = pixels[srcIndex + 3]; // A из BGRA[3]
                }
            }

            // Записываем подряд: RRR...GGG...BBB...AAA... (4 * width * height байт)
            using var fileStream = File.Create(filePath);
            fileStream.Write(rChannel, 0, pixelCount);
            fileStream.Write(gChannel, 0, pixelCount);
            fileStream.Write(bChannel, 0, pixelCount);
            fileStream.Write(aChannel, 0, pixelCount);

            Title = $"Planar RAW: {_fileName} ({width}x{height}, {pixelCount * 4} байт)";
        }

        //private void SaveTo(string name)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    for (var f = _terrainTopLeft.X; f <= _terrainBottomRight.X; f += 1.0f)
        //    {
        //        var abs = _calculator.GetAbsoluteHeight(f);
        //        var norm = _calculator.GetNormalizeHeight(f);
        //        sb.AppendLine($"{f}: {abs.ToString()}");
        //    }
        //    File.WriteAllText(name, sb.ToString());
        //}
    }
}