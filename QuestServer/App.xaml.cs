using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
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
        public App()
        {
            this.Startup += OnStartup;
        }

        private void OnStartup(object sender, StartupEventArgs startupEventArgs)
        {
            Provider = new Provider();
            Clients = new Clients();

            InitRooms();
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
