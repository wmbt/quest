using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using Common;

namespace QuestClient.NetworkService
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple/*, UseSynchronizationContext = false*/)]
    class QuestServiceCallback : IQuestServiceCallback
    {
        private readonly App _app;
        public Stages Stages;
        public QuestServiceCallback(App app)
        {
            _app = app;
            Stages = app.QusetStages;
        }

        public void SensorTriggered(int sensorId)
        {
            //Stages.SensorTriggered(sensorId);
        }

        
        public int StartQuest(Key[] keys)
        {
            try
            {
                Stages.AssignKeys(keys);
                _app.Dispatcher.InvokeAsync(() =>
                {
                    Stages.StartWatch();
                });
                var questDuration = int.Parse(ConfigurationManager.AppSettings["QuestDurationMin"]);
                _app.ServerPing = DateTime.Now;
                return questDuration;

            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("QuestClient", ex.Message, EventLogEntryType.Error);
                throw;
            }
        }

        public void SetCurrentKey(int keyId)
        {
            Stages.SetCurrentKey(keyId);
        }

        public void StopQuest()
        {
            _app.ServerPing = DateTime.Now;
            _app.Dispatcher.InvokeAsync(() =>
            {
                Stages.StopWatch();    
            });
        }

        public QuestState GetQuestState()
        {
            throw new NotImplementedException();
        }

        public void ServerPing()
        {
            _app.ServerPing = DateTime.Now;
        }


        public void SendMessage(string msg)
        {
            var messageWindow = Application.Current.Windows.OfType<MessageWindow>().SingleOrDefault();
            if (messageWindow != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    messageWindow.Message = msg;
                });
            }
            else
            {
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    messageWindow = new MessageWindow(msg)
                    {
                        Owner = Application.Current.MainWindow
                    };

                    messageWindow.ShowDialog();
                });
            }

        }
    }
}
