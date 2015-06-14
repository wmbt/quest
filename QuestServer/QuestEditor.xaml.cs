using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using QuestServer.Storage;

namespace QuestServer
{
    /// <summary>
    /// Логика взаимодействия для QuestEditor.xaml
    /// </summary>
    public partial class QuestEditor : Window
    {
        private readonly App _app = App.GetApp();
        private QuestDbDataSet _ds;
        public QuestEditor()
        {

           InitializeComponent();

            _ds = _app.Provider.GetDataSet();

            var quests = _ds.Quests.OrderBy(x => x.Id);
            QuestsCbx.DataContext = quests;

            QuestsCbx.SelectedIndex = 0;

        }

        private void QuestsCbxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = (QuestDbDataSet.QuestsRow)e.AddedItems[0];
            var keys = selected.GetKeysRows();


            KeysGrid.DataContext = keys;
        }

        private void OnRowDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var row = (DataGridRow) sender;
            var keyEditor = new KeyEditor((QuestDbDataSet.KeysRow) row.Item)
            {
                Owner = this
            };

            keyEditor.ShowDialog();
        }

        private void ButtonCancelOnClick(object sender, RoutedEventArgs e)
        {
            
            _ds.RejectChanges();
            Close();
        }

        private void ButtonOkOnClick(object sender, RoutedEventArgs e)
        {
            
            var i = _app.Provider.UpdateKeys();
            var j = _app.Provider.UpdateQuests();
            //_ds.AcceptChanges();
            Close();
        }
    }
}
