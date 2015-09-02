using System.Windows;

namespace QuestClient
{
    /// <summary>
    /// Логика взаимодействия для KeyWindow.xaml
    /// </summary>
    public partial class MessageWindow : Window
    {
        public string Message {
            get { return MessageText.Text; }
            set { MessageText.Text = value; }
        }
        
        public MessageWindow(string message)
        {
            InitializeComponent();
            Message = message;
        }

        private void CloseOnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
