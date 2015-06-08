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
        public QuestServiceClient QuestServiceClient { get; private set; }
        public Stages QusetStages { get; private set; }
        public int QuestId { get; private set; }
        
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
            var callback = new QuestServiceCallback(this);
            var instanceContext = new InstanceContext(callback);
            QuestId = int.Parse(ConfigurationManager.AppSettings["QuestId"]);
            QuestServiceClient = new QuestServiceClient(instanceContext);
            var keys = QuestServiceClient.RegisterQuestClient(QuestId);
            QusetStages = new Stages(keys);
            callback.Stages = QusetStages;
        }
        
    }
}
