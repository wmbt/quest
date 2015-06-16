using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using QuestServer.Annotations;
using QuestServer.Storage;

namespace QuestServer.Models
{
    public class QuestEditorViewModel : ViewModelBase
    {

        public List<CbxItem> Quests { get; private set; }
        public ObservableCollection<ListItem> Keys { get; private set; }

        public QuestEditorViewModel()
        {
            Quests = DataSet.Quests.OrderBy(x => x.Id).Select(x => new CbxItem
            {
                QuestRow = x
            }).ToList();

            Keys = new ObservableCollection<ListItem>();
        }

        public void FillKeys(QuestDbDataSet.QuestsRow qr)
        {
            Keys.Clear();
            foreach (var row in qr.GetKeysRows().OrderBy(x => x.OrderNmb))
            {
                Keys.Add(new ListItem(row));
            }
        }
        
        public class CbxItem
        {
            public string Description {
                get { return QuestRow.Desccription; }
            }

            internal QuestDbDataSet.QuestsRow QuestRow;
          
        }
    }

    public class ListItem : INotifyPropertyChanged
    {
        public int Number
        {
            get { return KeyRow.OrderNmb; }
        }
        public string Description
        {
            get { return KeyRow.Description; }
        }
        public double Duration
        {
            get { return KeyRow.TimeOffset.TotalMinutes; }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        internal QuestDbDataSet.KeysRow KeyRow;

        private ListItem() { }

        internal ListItem(QuestDbDataSet.KeysRow row)
        {
            KeyRow = row;
        }


        [NotifyPropertyChangedInvocator]
        internal virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
