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

            if (openFileDialog.ShowDialog() == true)
            {
                KeyImage.Source = KeyEditorViewModel.GetBitmapImage(openFileDialog.FileName);
            }

        }

        

        private void ButtonSaveOnClick(object sender, RoutedEventArgs e)
        {
            var minutes = int.Parse(DurationTbx.Text);
            
            _model.KeyRow.BeginEdit();
            _model.KeyRow.Description = DescriptionTbx.Text;
            _model.KeyRow.TimeOffset = new TimeSpan(0, minutes, 0);

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
