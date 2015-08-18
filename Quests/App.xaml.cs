using System;
using System.Configuration;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;
using System.Timers;
using System.Windows;
using QuestClient.NetworkService;

namespace QuestClient
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App
    {
        private QuestServiceCallback _callback;
        public QuestServiceClient QuestServiceClient { get; private set; }
        public Stages QusetStages { get; private set; }
        public int QuestId { get; private set; }
        public bool Connected { get; private set; }
        public int KeysCount { get; set; }
        public System.Timers.Timer HeartBeatTimer;
        public DateTime ServerPing = DateTime.Now;
        
        public App()

        {
            int MaxThreadsCount = Environment.ProcessorCount * 4;
            // Установим максимальное количество рабочих потоков
            ThreadPool.SetMaxThreads(MaxThreadsCount, MaxThreadsCount);
            // Установим минимальное количество рабочих потоков
            ThreadPool.SetMinThreads(2, 2);
            Startup += OnStartup;
            Exit += OnExit;

            HeartBeatTimer = new System.Timers.Timer {Interval = 5000};
            //HeartBeatTimer.Elapsed += HeartBeatTimerOnElapsed;
            
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
        }

        private void HeartBeatTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            HeartBeatTimer.Stop();
           
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
            QusetStages = new Stages();
            _callback.Stages = QusetStages;
            KeysCount = ConnectToServer();
        }

        public int ConnectToServer()
        {
            try
            {
                var instanceContext = new InstanceContext(_callback);

                if (QuestServiceClient != null)
                    QuestServiceClient.Abort();

                QuestServiceClient = new QuestServiceClient(instanceContext);                
                QuestServiceClient.Open();
                var keysCount = QuestServiceClient.RegisterQuestClient(QuestId);
                Connected = keysCount > 0;
                return keysCount;
            }
            catch (Exception ex)
            {

                if (QuestServiceClient != null)
                    QuestServiceClient.Abort();
                Connected = false;
                var msg = ex.Message + "\r\n" + ex.StackTrace;
                EventLog.WriteEntry("QuestClient", msg);
                return 0;
            }
            
        }

        /*public bool PingServer()
        {
            try
            {
                EventLog.WriteEntry("QuestClient", "Пинг отправлен", EventLogEntryType.Warning);
                var ping =  QuestServiceClient.Ping(QuestId);
                EventLog.WriteEntry("QuestClient", "Пинг получен", EventLogEntryType.Warning);
                return ping;
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("QuestClient", ex.Message + Environment.NewLine + ex.StackTrace, EventLogEntryType.Error);
                QuestServiceClient.Abort();
                return false;
            }
        }*/
    }
}
