using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Windows;
using Common;
using QuestServer.Storage;
using QuestService;

namespace QuestServer
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "QuestService" в коде и файле конфигурации.
    public class QuestService : IQuestService
    {
        private readonly App _app;
        private readonly Provider _provider;


        public QuestService()
        {
            _app = ((App)Application.Current);
            _provider = _app.Provider;
        }

        public IEnumerable<Key> GetQuestKeys(int questId)
        {

            var keys = _provider.GetQuestKeys(questId);
            return keys;
        }

        public void RegisterQuestClient( int questId)
        {
            var clients = _app.Clients;
            var callback = OperationContext.Current.GetCallbackChannel<IQuestServiceCallback>();
            clients.RegisterClient(questId, callback);
        }

        
    }
}
