using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Media.Imaging;
using Common;

namespace QuestClient
{
    /// <summary>
    /// Логика взаимодействия для KeyWindow.xaml
    /// </summary>
    public partial class KeyWindow : Window
    {
        private readonly Timer _closeTimer;
        public KeyWindow(Key key)
        {
            InitializeComponent();

            Description.Content = key.Description;

            if (key.Image != null && key.Image.Length > 0)
            {
                KeyImage.Source = GetBitmapImage(key.Image);
            }
            var interval = int.Parse(ConfigurationManager.AppSettings["KeyWindowInterval"]);

            _closeTimer = new Timer(1000*interval);
            _closeTimer.Elapsed += CloseTimerOnElapsed;
            Closing += OnClosing;
            _closeTimer.Start();
        }

        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            _closeTimer.Dispose();
        }

        private void CloseTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _closeTimer.Stop();
            Dispatcher.Invoke(Close);
        }

        public BitmapImage GetBitmapImage(byte[] imageBytes)
        {
            var stream = new MemoryStream(imageBytes);

            var image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = stream;
            image.EndInit();

            return image;
        }

        private void CloseOnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
