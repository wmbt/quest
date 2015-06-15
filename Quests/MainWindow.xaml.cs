using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Threading;
using System.Xml.Schema;
using Common.Phone;
using QuestClient;
using QuestClient.NetworkService;

namespace Quests
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PhoneEngine _phone;
        private readonly DispatcherTimer _dispatcherTimer;
        private Stages Stages { get; set; }
        private readonly Dictionary<Stage, Button> _keyButtons = new Dictionary<Stage, Button>();
        private readonly Timer _networkWatcher;
        private readonly App _app;

        public MainWindow()
        {
            InitializeComponent();
            
            _app = (App) Application.Current;
            _dispatcherTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 1) };
            _dispatcherTimer.Tick += DispatcherTimerOnTick;
            _networkWatcher = new Timer { Interval = 10000 };
            _networkWatcher.Elapsed += NetworkWatcherOnElapsed;
            _phone = new PhoneEngine(_app.QuestId);
            _phone.InitializeCall();


            
            _networkWatcher.Start();

            if (!_app.Connected)
                return;
            PrepareWindow();
            
        }

        private void PrepareWindow()
        {
            Stages = _app.QusetStages;

            Stages.KeyPublished += StagesOnKeyPublished;
            Stages.QuestStarted += StagesOnQuestStarted;
            Stages.QuestStopped += StagesOnQuestStopped;
            Stages.QuestCompleted += StagesOnQuestCompleted;

            _app.Dispatcher.Invoke(() =>
            {
                var locks = Grid.Children.OfType<Button>().Where(x => x.Name.Contains("Key")).ToArray();
                for (var i = 0; i < locks.Length; i++)
                {
                    _keyButtons.Add(Stages[i], locks[i]);
                }

                TotalElapsed.Visibility = Visibility.Hidden;
                CurrentElapsed.Visibility = Visibility.Hidden;
            });
            
        }

        private void StagesOnQuestCompleted(object sender, QuestCompletedHandlerArgs args)
        {
            _app.Dispatcher.Invoke(() =>
            {
                _dispatcherTimer.Stop();
                foreach (var v in _keyButtons.Values)
                {
                    v.IsEnabled = false;
                }
                TotalElapsed.Visibility = Visibility.Hidden;
                CurrentElapsed.Visibility = Visibility.Hidden;
                _app.QuestServiceClient.QuestCompleted(_app.QuestId);
                CallButton.IsEnabled = false;
            });
        }

        private void NetworkWatcherOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _networkWatcher.Stop();
            var app = (App)Application.Current;
            if (!app.PingServer())
            {
                if (app.ConnectToServer())
                {
                    PrepareWindow();
                }
                else
                {
                    if (Stages != null)
                    {
                        Stages.StopWatch();
                    }
                    _dispatcherTimer.Stop();
                    app.Dispatcher.Invoke(() =>
                    {
                        TotalElapsed.Visibility = Visibility.Hidden;
                        CurrentElapsed.Visibility = Visibility.Hidden;

                        foreach (var v in _keyButtons.Values)
                        {
                            v.IsEnabled = false;
                        }
                        CallButton.IsEnabled = false;
                    });
                }
            }
            _networkWatcher.Start();
        }

        private void DispatcherTimerOnTick(object sender, EventArgs eventArgs)
        {
            if (Stages.QuestRunning)
            {
                var totalRemaining = Stages.QuestDuration - Stages.TotalTime.Elapsed;
                TotalElapsed.Content = string.Format("Время до окончания квеста {0}", totalRemaining.ToString("mm\\:ss"));

                var currentRemaining = Stages.CurrentStage.Key.TimeOffset - Stages.CurrentTime.Elapsed;
                CurrentElapsed.Content = string.Format("Время  до следующей подсказки {0}",
                    currentRemaining.ToString("mm\\:ss"));

                if (!TotalElapsed.IsVisible)
                    TotalElapsed.Visibility = Visibility.Visible;
                
                if(!CurrentElapsed.IsVisible)
                    CurrentElapsed.Visibility = Visibility.Visible;
            }
        }

        private void StagesOnQuestStopped(object sender, QuestStoppedHandlerArgs args)
        {
            _dispatcherTimer.Stop();

            _app.Dispatcher.Invoke(() =>
            {
                TotalElapsed.Visibility = Visibility.Hidden;
                CurrentElapsed.Visibility = Visibility.Hidden;
                CallButton.IsEnabled = false;

                foreach (var v in _keyButtons.Values)
                {
                    v.IsEnabled = false;
                }    
            });
            
        }

        private void StagesOnQuestStarted(object sender, QuestStartedHandlerArgs args)
        {
            CallButton.IsEnabled = true;
            _dispatcherTimer.Start();
        }

        //private void StagesOnStageCompleted(object sender, StageCompletedHandlerArgs args)
        //{
           
        //}

        private void StagesOnKeyPublished(object sender, KeyPublishedHandlerArgs args)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _keyButtons[args.Stage].IsEnabled = true;    
            });
        }

        private void LockOnClick(object sender, RoutedEventArgs e)
        {
            var stage = _keyButtons.SingleOrDefault(x => Equals(x.Value, (Button) sender)).Key;
            
            var keyViewer = new KeyWindow(stage.Key){ Owner = this};
            keyViewer.ShowDialog();
        }
    }
}
