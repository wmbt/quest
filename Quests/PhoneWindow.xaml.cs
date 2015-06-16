using System.Configuration;
using System.Windows;
using Common.Phone;

namespace QuestClient
{
    /// <summary>
    /// Логика взаимодействия для PhoneWindow.xaml
    /// </summary>
    public partial class PhoneWindow : Window
    {
        private readonly App _app;
        private readonly PhoneEngine _phone;
        public PhoneWindow()
        {
            var serverIp = ConfigurationManager.AppSettings["ServerIP"];
            var sendPort = int.Parse(ConfigurationManager.AppSettings["SendToCommandPort"]);
            var listenPort = int.Parse(ConfigurationManager.AppSettings["ListenCommandPort"]);
            
            InitializeComponent();
            _app = (App)Application.Current;
            _phone = new PhoneEngine(_app.QuestId, false, sendPort, listenPort);
            
            _phone.OnCallDropped += PhoneOnOnCallDropped;
            _phone.OnCallBegin += PhoneOnOnCallBegin;
            _phone.BeginCall(serverIp);

        }

        private void PhoneOnOnCallBegin(object sender, CallBeginEventHandlerArgs args)
        {
            _app.Dispatcher.Invoke(() =>
            {
                CallButton.Content = "Завершить вызов";
            });
        }

        private void PhoneOnOnCallDropped(object sender, CallDroppedEventHandlerArgs args)
        {
            _app.Dispatcher.Invoke(() => {
                                             if (args.Busy)
                                             {
                                                 var box = MessageBox.Show("Оператор занят", "Вызов",
                                                     MessageBoxButton.OK, MessageBoxImage.Warning);
                                                 if (box == MessageBoxResult.Cancel || box == MessageBoxResult.OK)
                                                     Close();
                                             }
                                             else
                                             {
                                                 Close();
                                             }
                                        });
            
        }

        private void CallButtonOnClick(object sender, RoutedEventArgs e)
        {
            _phone.DropCall();
        }
    }
}
