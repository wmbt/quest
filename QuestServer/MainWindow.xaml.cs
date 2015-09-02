using System.Configuration;
using System.Linq;
using System.Timers;
using System.Windows;
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
        private readonly Timer _networkTimer;
        //private readonly int _hbInterval;

        public MainWindow()
        {
            var sendPort = int.Parse(ConfigurationManager.AppSettings["SendToCommandPort"]);
            var listenPort = int.Parse(ConfigurationManager.AppSettings["ListenCommandPort"]);
            var hbInterval = int.Parse(ConfigurationManager.AppSettings["HeartBeatSec"]);
            
            _app = App.GetApp();
            _model = new MainViewModel(_app.Clients);
            _phone = new PhoneEngine(-1, false, sendPort, listenPort);
            _phone.OnCallRecieved += PhoneOnOnCallRecieved;
            _networkTimer = new Timer(hbInterval * 1000);
            _networkTimer.Elapsed += NetworkTimerOnElapsed;
            DataContext = _model;
            Closing += (sender, args) =>
            {
                _phone.Dispose();
                _networkTimer.Stop();
            };
            InitializeComponent();
            _networkTimer.Start();
        }

        private void NetworkTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _networkTimer.Stop();
            //var now = DateTime.Now;

            var clients = _model.Clients.ToArray();

            foreach (var c in clients)
            {
                /*var elapsed = (now - c.Quest.LastPing).TotalSeconds;
                if (elapsed > _hbInterval)
                {
                    c.Cannel.Abort();
                }
                c.RefreshState();*/
                if (c.SendServerPing())
                    c.RefreshState();
                /*else
                {
                    
                }*/
            }
            _networkTimer.Start();
        }

        private void PhoneOnOnCallRecieved(object sender, CallRecivedEventHandlerArgs args)
        {
            Dispatcher.InvokeAsync(() =>
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
