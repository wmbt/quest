using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Windows;
using Common;
using QuestServer.Storage;
using QuestService;

namespace QuestServer.NetworkService
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "QuestService" в коде и файле конфигурации.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class QuestService : IQuestService
    {
        private readonly App _app;
        private readonly Provider _provider;


        public QuestService()
        {
            _app = ((App) Application.Current);
            _provider = _app.Provider;
        }

        public IEnumerable<Key> GetQuestKeys(int questId)
        {

            var keys = _provider.GetQuestKeys(questId);
            return keys;
        }

        public void RegisterQuestClient( int questId)
        {
            var context = OperationContext.Current;
            var clients = _app.Clients;
            
            clients.RegisterClient(questId, context);

            context.Channel.Closed += ChannelOnClosed;
            context.Channel.Faulted += ChannelOnFaulted;
        }

        private void ChannelOnFaulted(object sender, EventArgs eventArgs)
        {
            var channel = (IContextChannel) sender;
            _app.Clients.UnregisterClient(channel.SessionId);
        }

        private void ChannelOnClosed(object sender, EventArgs eventArgs)
        {
            var channel = (IContextChannel)sender;
            _app.Clients.UnregisterClient(channel.SessionId);   
        }
    }
}
