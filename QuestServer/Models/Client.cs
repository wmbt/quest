using System.ServiceModel;
using System.Windows;
using System.Windows.Input;
using QuestService;

namespace QuestServer.Models
{
    public class Client
    {
        public string SessionId { get; private set; }
        public Quest Quest { get; private set; }
        public Command StartCommand { get; set; }
        public Command StopCommand { get; set; }

        private readonly IQuestServiceCallback _clientCallback;
        public IContextChannel Cannel { get; private set; }
        
        public Client(OperationContext context, Quest quest)
        {
            _clientCallback = context.GetCallbackChannel<IQuestServiceCallback>();
            SessionId = context.SessionId;
            Cannel = context.Channel;
            Quest = quest;

            StartCommand = new Command(StartQuest);
            StopCommand = new Command(StopQuest);
            StartCommand.Enabled = true;
        }

        public void StartQuest()
        {
            StartCommand.Enabled = false;
            StopCommand.Enabled = true;
            var app = (App) Application.Current;
            var q = app.Provider.GetQuest(Quest.Id);
            Quest.Keys = q.Keys;
            _clientCallback.StartQuest(Quest.Keys);
            


            /*Room.AllowListening = true;
            Room.EnableSensors();*/
        }

        private void RoomOnSensorTriggered(object sender, SensorTriggeredArgs args)
        {
            var sensor = (Sensor) sender;
            _clientCallback.SensorTriggered(sensor.Id);
        }

        public void StopQuest()
        {
            /*Room.AllowListening = true;
            Room.ResetSensors();*/
            StartCommand.Enabled = true;
            StopCommand.Enabled = false;
            _clientCallback.StopQuest();
        }

        public void Completed()
        {
            StartCommand.Enabled = true;
            StopCommand.Enabled = false;
        }
    }

   /* public class Clients : ObservableCollection<Client>
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

            Add(new Client(callback, questRoom, operationContext.SessionId));
        }
        
    }*/
}
