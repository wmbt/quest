using System;
using Common;
using QuestClient;

namespace QuestClient.NetworkService
{
    class QuestServiceCallback : IQuestServiceCallback
    {
        private readonly App _app;
        private readonly Stages _stages;
        public QuestServiceCallback(App app)
        {
            _app = app;
            _stages = app.QusetStages;
        }

        public void SensorTriggered(int sensorId)
        {
            _stages.SensorTriggered(sensorId);
        }

        public void StartQuest()
        {
            _stages.StartWatch();
        }

        public void StopQuest()
        {
            _stages.StopWatch();
        }

        public QuestState GetQuestState()
        {
            throw new NotImplementedException();
        }
    }
}
