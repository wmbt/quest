using System.ServiceModel;
using Common;

namespace QuestService
{
    public interface IQuestServiceCallback
    {
        /*[OperationContract(IsOneWay = true)]
        void SensorTriggered(int sensorId);*/

        [OperationContract(IsOneWay = true)]
        void SetCurrentKey(int keyId);
        
        [OperationContract]
        int StartQuest(Key[] keys);

        [OperationContract(IsOneWay = true)]
        void StopQuest();

        [OperationContract]
        QuestState GetQuestState();

        [OperationContract(IsOneWay = true)]
        void ServerPing();
    }
}