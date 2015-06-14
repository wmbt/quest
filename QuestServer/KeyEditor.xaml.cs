using System;
using System.Collections.Generic;
using System.Globalization;
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
using Microsoft.Win32;
using QuestServer.Storage;

namespace QuestServer
{
    /// <summary>
    /// Логика взаимодействия для KeyEditor.xaml
    /// </summary>
    public partial class KeyEditor : Window
    {
        private readonly App _app = App.GetApp();
        private QuestDbDataSet _ds;
        private readonly QuestDbDataSet.KeysRow _currentRow;
        public KeyEditor(QuestDbDataSet.KeysRow keysRow)
        {
            InitializeComponent();
            _currentRow = keysRow;
            _ds = _app.Provider.GetDataSet();

            DescriptionTbx.Text = keysRow.Description;
            DurationTbx.Text = keysRow.TimeOffset.TotalMinutes.ToString(CultureInfo.InvariantCulture);

            if (!keysRow.IsImageNull() && keysRow.Image.Length > 0)
            {
                KeyImage.Source = GetBitmapImage(keysRow.Image);
            }


        }

        private void SelImageOnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog {Filter = "Image files (*.jpg;*.jpeg)|*.jpg;*.jpeg"};

            if (openFileDialog.ShowDialog() == true)
            {
                KeyImage.Source = GetBitmapImage(openFileDialog.FileName);
            }

        }

        private BitmapImage GetBitmapImage(string path)
        {
            var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
            
            var image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = fileStream;
            image.EndInit();

            return image;
        }

        private BitmapImage GetBitmapImage(byte[] imageBytes)
        {
            var stream = new MemoryStream(imageBytes);

            var image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = stream;
            image.EndInit();

            return image;
        }

        private void ButtonSaveOnClick(object sender, RoutedEventArgs e)
        {
            var minutes = int.Parse(DurationTbx.Text);
            
            _currentRow.BeginEdit();
            _currentRow.Description = DescriptionTbx.Text;
            _currentRow.TimeOffset = new TimeSpan(0, minutes, 0);

            var image = (BitmapImage) KeyImage.Source;
            if (image != null)
            {
                var stream = image.StreamSource;
                
                byte[] buffer = null;
                if (stream != null && stream.Length > 0)
                {
                    using (var br = new BinaryReader(stream))
                    {
                        stream.Position = 0;
                        buffer = br.ReadBytes((int) stream.Length);
                    }
                }

                
                _currentRow.Image = buffer;
            }
            else
            {
                _currentRow.SetImageNull();
            }
            
            _currentRow.EndEdit();
            //_currentRow.SetModified();
            Close();
        }

        private void ButtonCancelOnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
