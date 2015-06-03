using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Common;
using QuestService;

namespace QuestServer.NetworkService
{
    public class QuestClient
    {
        public QuestClient(IQuestServiceCallback clientCallback, Room room)
        {
            _clientCallback = clientCallback;
            Room = room;
            Room.SensorTriggered += RoomOnSensorTriggered;
        }

        public int Id { get { return Room.Id; } }
        public Room Room { get; internal set; }
        private readonly IQuestServiceCallback _clientCallback;

        public void StartQuest()
        {
            _clientCallback.StartQuest();
            Room.AllowListening = true;
            Room.EnableSensors();
        }

        private void RoomOnSensorTriggered(object sender, SensorTriggeredArgs args)
        {
            var sensor = (Sensor) sender;
            _clientCallback.SensorTriggered(sensor.Id);
        }

        public void StopQuest()
        {
            Room.AllowListening = true;
            Room.ResetSensors();
            _clientCallback.StopQuest();
        }
    }

    public class Clients : IEnumerable<QuestClient>
    {
        private readonly List<QuestClient> _clients = new List<QuestClient>();
        private readonly App _app = ((App)Application.Current);

        public void UnregisterClient(int questId)
        {
            var client = _clients.Single(x => x.Room.Id == questId);
            _clients.Remove(client);
        }

        public void RegisterClient(int questId, IQuestServiceCallback callback)
        {
            var questRoom = _app.Rooms.GetById(questId);
            
            _clients.Add(new QuestClient(callback, questRoom));
        }

        public IEnumerator<QuestClient> GetEnumerator()
        {
            return _clients.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        
    }
}
