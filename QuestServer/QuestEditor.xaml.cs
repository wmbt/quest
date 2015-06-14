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
using QuestServer.Models;
using QuestServer.Storage;

namespace QuestServer
{
    /// <summary>
    /// Логика взаимодействия для QuestEditor.xaml
    /// </summary>
    public partial class QuestEditor : Window
    {
        private readonly QuestEditorViewModel _model;
        public QuestEditor(QuestEditorViewModel model)
        {

           InitializeComponent();
            _model = model;
            DataContext = model;
            QuestsCbx.SelectedIndex = 0;
        }

        private void QuestsCbxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = (QuestEditorViewModel.CbxItem)e.AddedItems[0];
            _model.FillKeys(selected.QuestRow);
        }

        /*private void OnRowDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var row = (DataGridRow) sender;
            var keyEditor = new KeyEditor((QuestDbDataSet.KeysRow) row.Item)
            {
                Owner = this
            };

            keyEditor.ShowDialog();
        }*/

        private void ButtonCancelOnClick(object sender, RoutedEventArgs e)
        {
            
            _model.DataSet.RejectChanges();
            Close();
        }

        private void ButtonOkOnClick(object sender, RoutedEventArgs e)
        {

            _model.Provider.UpdateKeys();
            _model.Provider.UpdateQuests();
            Close();
        }

        private void Control_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var row = (ListView)sender;
            var keyEditor = new KeyEditor(new KeyEditorViewModel((QuestServer.Models.ListItem)row.SelectedItem))
            {
                Owner = this
            };

            keyEditor.ShowDialog();
        }
    }
}
