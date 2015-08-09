using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using QuestServer.Models;

namespace QuestServer
{
    /// <summary>
    /// Логика взаимодействия для Thumbnails.xaml
    /// </summary>
    ///  
    public partial class Thumbnails : UserControl
    {
        public static readonly DependencyProperty QuestProperty = DependencyProperty.Register("Client", typeof(Client), typeof(Thumbnails),
        new PropertyMetadata(null, PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            /*var oldClient = dependencyPropertyChangedEventArgs.OldValue as Client;
            if (oldClient != null)
                oldClient.PropertyChanged -= ClientOnPropertyChanged;*/
            
            var client = (Client)dependencyPropertyChangedEventArgs.NewValue;
            var c = (Thumbnails) dependencyObject;
            c.AssignClient(client);
           
        }

        private void AssignClient(Client client)
        {
            Thumbs.Children.Clear();

            foreach (var key in client.Quest.Keys)
            {
                var th = new Thumbnail(key, client);
                Thumbs.Children.Add(th);
            }

            client.PropertyChanged += ClientOnPropertyChanged;
        }

        private void ClientOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName != "InProcess") 
                return;
            
            var client = (Client)sender;
            var thumbs = Thumbs.Children.OfType<Thumbnail>().ToArray();
            if (client.InProcess)
            {
                thumbs.First().RadioButton.IsChecked = true;
                foreach (var t in thumbs)
                {
                    t.RadioButton.IsEnabled = true;
                }
            }
            else
            {
                foreach (var t in thumbs)
                {
                    t.RadioButton.IsEnabled = false;
                    t.RadioButton.IsChecked = false;
                }
            }
        }

        public Client Client
        {
            get { return (Client)GetValue(QuestProperty); }
            set { SetValue(QuestProperty, value); }
        }
        
        public Thumbnails()
        {
            InitializeComponent();
        }
    }
}
