using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Quests.NetworkService;

namespace Quests
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
        }

        private void OnStartup(object sender, StartupEventArgs startupEventArgs)
        {
            var instanceContext = new InstanceContext(new QuestServiceCallback(this));
            QuestId = int.Parse(ConfigurationManager.AppSettings["QuestId"]);
            QuestServiceClient = new QuestServiceClient(instanceContext);
            QuestServiceClient.RegisterQuestClient(QuestId);

            var keys = QuestServiceClient.GetQuestKeys(QuestId);
            QusetStages = new Stages(keys);

            
        }
        
    }
}
