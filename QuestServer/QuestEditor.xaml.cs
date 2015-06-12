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
        private App _app = App.GetApp();
        private QuestDbDataSet _ds;
        public QuestEditor()
        {
            InitializeComponent();
            _ds = _app.Provider.
        }

        private void QuestsCbxOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
                
        }
    }
}
