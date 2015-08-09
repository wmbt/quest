using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Common;
using QuestServer.Models;

namespace QuestServer
{
    /// <summary>
    /// Логика взаимодействия для Thumbnails.xaml
    /// </summary>
    public partial class Thumbnail : UserControl
    {
        private readonly Key _key;
        private readonly App _app;
        private readonly Client _client;
        public Thumbnail(Key key, Client client)
        {
            _client = client;
            _key = key;
            _app = (App) Application.Current;
            InitializeComponent();
            RadioButton.GroupName = "group" + _client.Quest.Id;
            RadioButton.Content = key.Description;
            RadioButton.IsEnabled = false;
            KeyImage.Source = GetImageSource(key.Image);

            key.PropertyChanged += KeyOnPropertyChanged;
        }

        private void KeyOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName != "Viewed")
                return;
            
            if (_key.Viewed)
            {
                ThStack.Background = new SolidColorBrush(Colors.LightSalmon);
            }
            else
            {
                ThStack.Background = new SolidColorBrush(Colors.White);
            }
        }

        public BitmapImage GetImageSource(byte[] imageBytes)
        {
            var ms = new MemoryStream(imageBytes);
            var image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }

        private void RadioButtonOnChecked(object sender, RoutedEventArgs e)
        {
            if (!RadioButton.IsEnabled)
                return;
            Dispatcher.InvokeAsync(() =>
            {
                _client.SetCurrentKey(_key.KeyId);
            });
        }
    }
}
