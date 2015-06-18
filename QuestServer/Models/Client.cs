using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Windows;
using System.Windows.Input;
using QuestServer.Annotations;
using QuestService;

namespace QuestServer.Models
{
    public class Client : INotifyPropertyChanged
    {
        public string SessionId { get; private set; }
        public Quest Quest { get; private set; }
        public Command StartCommand { get; set; }
        public Command StopCommand { get; set; }
        public string State { get; set; }
        private DateTime _startTime { get; set; }
        private DateTime _endTime { get; set; }
        //private TimeSpan _duration { get; set; }
        private bool _inProcess { get; set; }

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
            var duration = new TimeSpan();
            duration = q.Keys.Aggregate(duration, (current, key) => current + key.TimeOffset);
            
            _startTime = DateTime.Now;
            _endTime = _startTime + duration;
            Quest.Keys = q.Keys;
            _clientCallback.StartQuest(Quest.Keys);
            _inProcess = true;


            /*Room.AllowListening = true;
            Room.EnableSensors();*/
        }

        public void RefreshState()
        {
            if (!_inProcess)
                State = "";
            else
            {
                State = "До завершения осталось " + ((_endTime - _startTime).ToString("hh\\:mm"));
            }
            OnPropertyChanged("State");
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
            _inProcess = false;
            State = "";
            OnPropertyChanged("State");
            StartCommand.Enabled = true;
            StopCommand.Enabled = false;
            _clientCallback.StopQuest();
        }

        public void Completed()
        {
            _inProcess = false;
            State = "";
            OnPropertyChanged("State");
            StartCommand.Enabled = true;
            StopCommand.Enabled = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
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
