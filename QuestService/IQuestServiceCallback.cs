using System.ServiceModel;
using Common;

namespace QuestService
{
    public interface IQuestServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void SensorTriggered(int sensorId);

        [OperationContract(IsOneWay = true)]
        void StartQuest();

        [OperationContract(IsOneWay = true)]
        void StopQuest();

        [OperationContract]
        QuestState GetQuestState();
    }
}