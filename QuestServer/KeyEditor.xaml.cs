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
using QuestServer.Models;
using QuestServer.Storage;

namespace QuestServer
{
    /// <summary>
    /// Логика взаимодействия для KeyEditor.xaml
    /// </summary>
    public partial class KeyEditor : Window
    {
        private readonly KeyEditorViewModel _model;
        public KeyEditor(KeyEditorViewModel model)
        {
            InitializeComponent();
            _model = model;
            DataContext = model;
            
            DescriptionTbx.Text = model.Description;
            DurationTbx.Text = model.Duration;


            var image = model.GetBitmapImage();
            if (image != null)
            {
                KeyImage.Source = image;
            }
        }

        private void SelImageOnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog {Filter = "Image files (*.jpg;*.jpeg)|*.jpg;*.jpeg"};

            if (openFileDialog.ShowDialog() != true) 
                return;
            
            var f = new FileInfo(openFileDialog.FileName);
            if (f.Length > 10485760 /* 10MB */)
            //if (f.Length > 1048576 /* 1MB */)
            {
                MessageBox.Show("Размер файла должен быть меньше 10 Мб", "Ошибка изменения изображения",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                KeyImage.Source = KeyEditorViewModel.GetBitmapImage(openFileDialog.FileName);
            }
        }

        

        private void ButtonSaveOnClick(object sender, RoutedEventArgs e)
        {
            double minutes = 0;
            var isValid = true;
            var errorMsg = new List<string>();
            var minutesParsed = double.TryParse(DurationTbx.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out minutes);

            if (!minutesParsed)
            {
                isValid = false;
                errorMsg.Add("Ошибка значения времени");
            }
            if (string.IsNullOrEmpty(DescriptionTbx.Text.Trim()))
            {
                isValid = false;
                errorMsg.Add("Укажите описание");
            }

            if (!isValid)
            {
                var message = string.Join(Environment.NewLine, errorMsg.ToArray());
                MessageBox.Show(message, "Ошибка в данных",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            _model.KeyRow.BeginEdit();
            _model.KeyRow.Description = DescriptionTbx.Text;
            _model.KeyRow.TimeOffset = new TimeSpan(0, 0, (int)(60 * minutes));

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


                _model.KeyRow.Image = buffer;
            }
            else
            {
                _model.KeyRow.SetImageNull();
            }

            _model.KeyRow.EndEdit();
            _model.Key.OnPropertyChanged("Description");
            _model.Key.OnPropertyChanged("Duration");
            Close();
        }

        private void ButtonCancelOnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
    
}
