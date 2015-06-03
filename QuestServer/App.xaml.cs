using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Common;
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
        public App()
        {
            this.Startup += OnStartup;
        }

        private void OnStartup(object sender, StartupEventArgs startupEventArgs)
        {
             Provider = new Provider();
            InitRooms();
        }

        private void InitRooms()
        {
            Rooms = new RoomCollection();
            var roomsCount = int.Parse(ConfigurationManager.AppSettings["RoomsCount"]);
            for (var i = 0; i < roomsCount; i++)
            {
                var roomsSensors = ConfigurationManager.AppSettings["Room" + i + "Sensor"];
                var sensorsIds = roomsSensors.Split(',').Select(int.Parse);
                var room = new Room(sensorsIds);
                Rooms.Add(room);
            }
        }
    }
}
