using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Key = Common.Key;

namespace QuestClient
{
    /// <summary>
    /// Логика взаимодействия для KeyWindow.xaml
    /// </summary>
    public partial class KeyWindow : Window
    {
        public KeyWindow(Key key)
        {
            InitializeComponent();

            Description.Content = key.Description;

            if (key.Image != null && key.Image.Length > 0)
            {
                KeyImage.Source = GetBitmapImage(key.Image);
            }

            
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
