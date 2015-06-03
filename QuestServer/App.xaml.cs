using System;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using Common;
using QuestServer.NetworkService;
using QuestServer.Storage;

namespace QuestServer
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public Provider Provider;
        public RoomCollection Rooms;
        public Clients Clients;
        public ServiceHost ServiceHost;
        
        public App()
        {
            Startup += OnStartup;
            Exit += OnExit;
        }

        private void OnExit(object sender, ExitEventArgs exitEventArgs)
        {
            if (ServiceHost == null) 
                return;
            
            ServiceHost.Close();
            ServiceHost = null;
        }

        private void OnStartup(object sender, StartupEventArgs startupEventArgs)
        {
            Provider = new Provider();
            Clients = new Clients();

            InitRooms();
            ServiceHost = new ServiceHost(typeof (NetworkService.QuestService));
            ServiceHost.Open();
        }

        private void InitRooms()
        {
            Rooms = new RoomCollection();

            var quests = Provider.GetAllQuests();
            
            foreach (var room in 
                from quest in quests 
                    let sensorsIds = Provider.GetSensorsIds(quest.Id) 
                    select new Room(quest.Id, sensorsIds))
                Rooms.Add(room);
        }
    }
}
