using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Common;

namespace QuestService
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени интерфейса "IQuestService" в коде и файле конфигурации.
    [ServiceContract(CallbackContract = typeof(IQuestServiceCallback), SessionMode = SessionMode.Required)]
    public interface IQuestService
    {
        [OperationContract]
        void RegisterQuestClient(int questId);

        [OperationContract(IsOneWay = true)]
        void QuestCompleted(int questId);

        [OperationContract]
        bool Ping(int questId);

    }
}
