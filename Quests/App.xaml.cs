using System;
using System.Configuration;
using System.ServiceModel;
using System.Windows;
using QuestClient.NetworkService;
using Quests;

namespace QuestClient
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        private QuestServiceCallback _callback;
        public QuestServiceClient QuestServiceClient { get; private set; }
        public Stages QusetStages { get; private set; }
        public int QuestId { get; private set; }
        public bool Connected { get; private set; }
        
        public App()
        {
            Startup += OnStartup;
            Exit += OnExit;
            
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            QuestServiceClient.Close();
        }

        private void OnExit(object sender, ExitEventArgs exitEventArgs)
        {
            QuestServiceClient.Close();
        }

        private void OnStartup(object sender, StartupEventArgs startupEventArgs)
        {
            _callback = new QuestServiceCallback(this);
            QuestId = int.Parse(ConfigurationManager.AppSettings["QuestId"]);
            ConnectToServer();
        }

        public bool ConnectToServer()
        {
            try
            {
                var instanceContext = new InstanceContext(_callback);
                QuestServiceClient = new QuestServiceClient(instanceContext);                
                QuestServiceClient.Open();
                var keys = QuestServiceClient.RegisterQuestClient(QuestId);
                QusetStages = new Stages(keys);
                _callback.Stages = QusetStages;
                Connected = true;
                return true;
            }
            catch (Exception ex)
            {

                QuestServiceClient.Abort();
                Connected = false;
                return false;
            }
            
        }

        public bool PingServer()
        {
            try
            {
               return QuestServiceClient.Ping();
            }
            catch (Exception ex)
            {
                QuestServiceClient.Abort();
                return false;
            }
        }
    }
}
