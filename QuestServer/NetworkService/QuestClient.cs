using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using Common;
using QuestService;

namespace QuestServer.NetworkService
{
    public class QuestClient
    {
        public int Id { get { return Room.Id; } }
        public Room Room { get; internal set; }
        public string SessionId { get; private set; }

        private readonly IQuestServiceCallback _clientCallback;

        public QuestClient(IQuestServiceCallback clientCallback, Room room, string sessionId)
        {
            _clientCallback = clientCallback;
            SessionId = sessionId;
            Room = room;
            Room.SensorTriggered += RoomOnSensorTriggered;
        }

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

    public class Clients : ObservableCollection<QuestClient>
    {
        private readonly App _app = ((App)Application.Current);

        
        public void UnregisterClient(string sessionId)
        {
            var client = this.Single(x => x.SessionId == sessionId);
            Remove(client);
        }

        public void RegisterClient(int questId, OperationContext operationContext)
        {
            var questRoom = _app.Rooms.GetById(questId);
            var callback = operationContext.GetCallbackChannel<IQuestServiceCallback>();

            Add(new QuestClient(callback, questRoom, operationContext.SessionId));
        }
        
    }
}
