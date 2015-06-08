using System;
using Common;
using QuestClient;

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
            Stages.SensorTriggered(sensorId);
        }

        public void StartQuest()
        {
            Stages.StartWatch();
        }

        public void StopQuest()
        {
            Stages.StopWatch();
        }

        public QuestState GetQuestState()
        {
            throw new NotImplementedException();
        }
    }
}
