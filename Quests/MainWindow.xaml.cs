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
using System.Windows.Navigation;
using System.Windows.Shapes;
using QuestClient;

namespace Quests
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Stages Stages { get; set; }
        public MainWindow()
        {
            var app = (App) Application.Current;
            InitializeComponent();
            //Stages = app.QusetStages;

            /*ShowKeyButton.IsEnabled = false;*/

            //Stages.KeyPublished += StagesOnKeyPublished;
            //Stages.StageCompleted += StagesOnStageCompleted;
        }

        private void StagesOnStageCompleted(object sender, StageCompletedHandlerArgs args)
        {
                    
        }

        private void StagesOnKeyPublished(object sender, KeyPublishedHandlerArgs args)
        {
                
        }
    }
}
