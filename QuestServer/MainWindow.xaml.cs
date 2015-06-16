using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Common.Phone;
using QuestServer.Models;

namespace QuestServer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private readonly PhoneEngine _phone;
        private readonly MainViewModel _model;
        private readonly App _app;

        public MainWindow()
        {
            var sendPort = int.Parse(ConfigurationManager.AppSettings["SendToCommandPort"]);
            var listenPort = int.Parse(ConfigurationManager.AppSettings["ListenCommandPort"]);
            
            _app = App.GetApp();
            _model = new MainViewModel(_app.Clients);
            _phone = new PhoneEngine(-1, false, sendPort, listenPort);
            _phone.OnCallRecieved += PhoneOnOnCallRecieved;
            DataContext = _model;
            InitializeComponent();
        }

        private void PhoneOnOnCallRecieved(object sender, CallRecivedEventHandlerArgs args)
        {
            Dispatcher.Invoke(() =>
            {
                var questId = int.Parse(args.ClientId);
                var clientName = _app.Provider.GetDataSet().Quests.FindById(questId).Desccription;
                var phoneWindow = new PhoneWindow(_phone)
                {
                    CallerName = clientName,
                    Owner = this
                };
                phoneWindow.ShowDialog();
            });
        }

        private void EditQuestsOnClick(object sender, RoutedEventArgs e)
        {
            var questEditor = new QuestEditor(new QuestEditorViewModel())
            {
                Owner = this
            };

            questEditor.ShowDialog();
        }
    }
}
