using System;
using System.Configuration;
using System.Linq;
using Common;

namespace QuestClient.NetworkService
{
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
            Stages.AssignKeys(keys);
            _app.Dispatcher.InvokeAsync(() =>
            {
                Stages.StartWatch();
            });
            var questDuration = int.Parse(ConfigurationManager.AppSettings["QuestDurationMin"]);
            return questDuration;

        }

        public void SetCurrentKey(int keyId)
        {
            Stages.SetCurrentKey(keyId);
        }

        public void StopQuest()
        {
            _app.Dispatcher.InvokeAsync(() =>
            {
                Stages.StopWatch();    
            });
        }

        public QuestState GetQuestState()
        {
            throw new NotImplementedException();
        }
    }
}
