using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Common.Phone;
using QuestServer.Models;

namespace QuestServer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        //private PhoneEngine _phone;
        private readonly MainViewModel _model;
        public MainWindow()
        {
            var app = App.GetApp();
            _model = new MainViewModel(app.Clients);
            DataContext = _model;
            InitializeComponent();
        }

        private void EditQuestsOnClick(object sender, RoutedEventArgs e)
        {
            var questEditor = new QuestEditor(new QuestEditorViewModel())
            {
                Owner = this
            };

            questEditor.ShowDialog();
        }
    }
}
