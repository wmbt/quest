using System;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using Common;
using QuestServer.Models;
using QuestServer.Storage;
using QuestService;

namespace QuestServer.NetworkService
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "QuestService" в коде и файле конфигурации.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class QuestService : IQuestService
    {
        private readonly App _app;
        private readonly Provider _provider;
        public ClientCollection Clients { get; private set; }
        
        public QuestService(ClientCollection clients)
        {
            Clients = clients;
            _app = ((App) Application.Current);
            _provider = _app.Provider;
        }

        public Key[] RegisterQuestClient( int questId)
        {
            var context = OperationContext.Current;
            var quest = _provider.GetQuest(questId);

            _app.Dispatcher.Invoke(() => { Clients.Add(new Client(context, quest)); });

            context.Channel.Closed += ChannelOnClosed;
            context.Channel.Faulted += ChannelOnFaulted;
            
            return quest.Keys;
        }

        public void QuestCompleted(int questId)
        {
            _app.Dispatcher.Invoke(() =>
            {
                var client = Clients.Single(x => x.Quest.Id == questId);
                client.Completed();    
            });
        }

        private void UnregisterClient(IContextChannel channel)
        {
            _app.Dispatcher.Invoke(() =>
            {
                var c = Clients.Single(x => x.SessionId == channel.SessionId);
                Clients.Remove(c);
            });
        }

        private void ChannelOnFaulted(object sender, EventArgs eventArgs)
        {
            UnregisterClient((IContextChannel) sender);
        }

        private void ChannelOnClosed(object sender, EventArgs eventArgs)
        {
            UnregisterClient((IContextChannel) sender);
        }
    }
}
