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
                BmpBitmapEncoder encoder = new();
                encoder.Frames.Add(BitmapFrame.Create(bitmap));
                encoder.Save(fileStream);
            }

            Title = $"Сохранено: {fileName}";
        }

    }
}