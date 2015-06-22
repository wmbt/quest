using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace QuestClient
{
    /// <summary>
    /// Логика взаимодействия для KeyButton.xaml
    /// </summary>
    public partial class KeyButton : UserControl
    {
        public string Label {
            get { return (string) Time.Content; }
            set { Time.Content = value; }
        }
        public BitmapImage ImageSource {
            get { return (BitmapImage) ButtonImage.Source; }
            set { ButtonImage.Source = value; }
        }
        public Image ImageControl {
            get { return ButtonImage; }
            set { ButtonImage = value; }
        }

        public Button ButtonControl
        {
            get { return KeyBtn; }
            set { KeyBtn = value; }
        }

        public KeyButton()
        {
            InitializeComponent();
            KeyBtn.IsEnabled = false;
        }
    }
}
