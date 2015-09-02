using System.Windows;
using QuestServer.Models;

namespace QuestServer
{
    /// <summary>
    /// Логика взаимодействия для SendMessage.xaml
    /// </summary>
    public partial class SendMessage : Window
    {
        public int QuestId {
            get { return _client.Quest.Id; }
        }
        private readonly Client _client;

        public SendMessage(Client client)
        {
            _client = client;
            InitializeComponent();
            Title = Title + " - " + client.Quest.Description;
        }

        private void OnCloseClick(object sender, RoutedEventArgs e)
        {
            Close();    
        }

        private void OnSendMessageClick(object sender, RoutedEventArgs e)
        {
            var message = Msg.Text.Trim();

            if (string.IsNullOrEmpty(message))
            {
                MessageBox.Show("Введите текст для отправки", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            
            _client.SendMessage(message);
            Close();
        }
    }
}
