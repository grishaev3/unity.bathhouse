using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Height.Wpf
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Calculator calculator = new(
                new System.Numerics.Vector2(-23.5f, 30f),
                new System.Numerics.Vector2(11.5f, -20f),
                1024);


            var a = calculator.Calc(-23.5f, 30f);
            var b = calculator.Calc(11.5f, -20f);
            var c = calculator.Calc(0f, 0f);

            FocusManager.SetFocusedElement(this, this);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SaveGradientToBmp();
                e.Handled = true;
            }
        }
        private void SaveGradientToBmp()
        {
            var bitmap = gradientRenderer.CreateBitmap();

            string fileName = $"gradient_{System.DateTime.Now:yyyyMMdd_HHmmss}.bmp";
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), fileName);

            using (var fileStream = File.Create(filePath))
            {
                BmpBitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                encoder.Save(fileStream);
            }

            Title = $"Сохранено: {fileName}";
        }

    }
}