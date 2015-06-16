using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using QuestServer.Storage;

namespace QuestServer.Models
{
    public class KeyEditorViewModel : ViewModelBase
    {
        public ListItem Key { get; private set; }
        public QuestDbDataSet.KeysRow KeyRow { get; private set; }
        public string Description {
            get { return KeyRow.Description; }
        }
        public string Duration {
            get { return KeyRow.TimeOffset.TotalMinutes.ToString(CultureInfo.InvariantCulture); }
        }
        
        public KeyEditorViewModel(ListItem key)
        {
            Key = key;
            KeyRow = Key.KeyRow;
        }

        public static BitmapImage GetBitmapImage(string path)
        {
            var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);

            var image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = fileStream;
            image.EndInit();

            return image;
        }

        public BitmapImage GetBitmapImage()
        {
            if (KeyRow.IsImageNull())
                return null;

            var stream = new MemoryStream(KeyRow.Image);

            var image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = stream;
            image.EndInit();

            return image;
        }
    }
}
