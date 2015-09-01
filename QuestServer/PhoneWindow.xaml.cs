using System;
using System.Configuration;
using System.Threading;
using System.Windows;
using Common.Phone;

namespace QuestServer
{
    /// <summary>
    /// Логика взаимодействия для PhoneWindow.xaml
    /// </summary>
    public partial class PhoneWindow : Window
    {
        private readonly App _app;
        private readonly PhoneEngine _phone;

        public string CallerName {
            set { CallerId.Content = value; }
        }
        public PhoneWindow(PhoneEngine phone)
        {
            InitializeComponent();
            _app = (App)Application.Current;
            _phone = phone;
            _phone.OnCallDropped += PhoneOnOnCallDropped;
        }

        private void PhoneOnOnCallDropped(object sender, CallDroppedEventHandlerArgs args)
        {
            Dispatcher.Invoke(Close);
        }

        /*private void PhoneOnOnCallBegin(object sender, CallBeginEventHandlerArgs args)
        {
            _app.Dispatcher.Invoke(() =>
            {
                CallButton.Content = "Завершить вызов";
            });
        }*/

        private void AcceptButtonOnClick(object sender, RoutedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                _phone.AcceptCall();
            });
            AcceptButton.IsEnabled = false;
            DropButton.Content = "Завершить";
        }

        private void RejectButtonOnClick(object sender, RoutedEventArgs e)
        {
            _phone.DropCall();
        }
    }
}
