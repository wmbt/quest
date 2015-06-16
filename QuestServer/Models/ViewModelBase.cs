using QuestServer.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QuestServer.Models
{
    public class ViewModelBase
    {
        public App App { get; protected set; }
        public Provider Provider { get; protected set; }
        public QuestDbDataSet DataSet { get; protected set; }

        public ViewModelBase()
        {
            App = (App) Application.Current;
            Provider = App.Provider;
            DataSet = Provider.GetDataSet();
        }
    }
}
