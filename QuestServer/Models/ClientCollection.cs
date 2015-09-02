using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace QuestServer.Models
{
    public class ClientCollection : ObservableCollection<Client>
    {
        protected override void RemoveItem(int index)
        {
            var client = this[index];
            var messageWindow = Application.Current.Windows.OfType<SendMessage>().SingleOrDefault();
            
            if (messageWindow != null)
            {
                if (messageWindow.QuestId == client.Quest.Id)
                    messageWindow.Close();
            }

            base.RemoveItem(index);
        }
    }
}
