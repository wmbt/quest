using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.Windows;
using System.Windows.Input;
using Common;
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
        //private DateTime _startTime { get; set; }
        private DateTime _endTime { get; set; }
        //private TimeSpan _duration { get; set; }
        public bool InProcess { get; set; }
        //public QuestState State { get; set; }

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
            //var duration = new TimeSpan();
            //duration = q.Keys.Aggregate(duration, (current, key) => current + key.TimeOffset);
            
            //_startTime = DateTime.Now;
            var duration = _clientCallback.StartQuest(q.Keys);
            _endTime = DateTime.Now + new TimeSpan(0, duration, 0);
            //Quest.Keys = q.Keys;
            
            InProcess = true;
            OnPropertyChanged("InProcess");
            RefreshState();


            /*Room.AllowListening = true;
            Room.EnableSensors();*/
        }

        public void SetCurrentKey(int keyId)
        {
            _clientCallback.SetCurrentKey(keyId);
        }

        public void RefreshState()
        {
            if (!InProcess)
                State = "";
            else
            {
                State = /*"До завершения осталось " + */((_endTime - DateTime.Now).ToString("hh\\:mm"));
            }
            OnPropertyChanged("State");
        }

       /* private void RoomOnSensorTriggered(object sender, SensorTriggeredArgs args)
        {
            var sensor = (Sensor) sender;
            _clientCallback.SensorTriggered(sensor.Id);
        }*/

        public void StopQuest()
        {
            /*Room.AllowListening = true;
            Room.ResetSensors();*/
            InProcess = false;
            OnPropertyChanged("InProcess");
            State = "";
            OnPropertyChanged("State");
            StartCommand.Enabled = true;
            StopCommand.Enabled = false;

            foreach (var key in Quest.Keys)
            {
                key.Viewed = false;
            }
            _clientCallback.StopQuest();
        }

        public void Completed()
        {
            InProcess = false;
            OnPropertyChanged("InProcess");
            State = "";
            OnPropertyChanged("State");
            StartCommand.Enabled = true;
            StopCommand.Enabled = false;

            foreach (var key in Quest.Keys)
            {
                key.Viewed = false;
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetKeyViewed(int keyId)
        {
            var key = Quest.Keys.Single(x => x.KeyId == keyId);
            key.Viewed = true;

        }

        public bool SendServerPing()
        {
            try
            {
                _clientCallback.ServerPing();
                return true;
            }
            catch (Exception ex)
            {
                Cannel.Abort();
                return false;
            }
        }

        //public void CloseClient()
        //{
        //    _clientCallback
        //}
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
