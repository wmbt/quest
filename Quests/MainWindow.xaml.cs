using System;
using System.ComponentModel;
using System.Configuration;
using System.Timers;
using System.Windows;
using System.Windows.Threading;

namespace QuestClient
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly Player _player = new Player();
        private readonly DispatcherTimer _dispatcherTimer;
        private Stages Stages { get; set; }
        //private readonly Timer _networkWatcher;
        private readonly App _app;
        
        public MainWindow()
        {
            InitializeComponent();
            TotalElapsed.Visibility = Visibility.Hidden;
            Closing += OnClosing;
            //var hbInterval = int.Parse(ConfigurationManager.AppSettings["HeartBeatSec"]);
            _app = (App) Application.Current;
            _dispatcherTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 1) };
            _dispatcherTimer.Tick += DispatcherTimerOnTick;
            //_networkWatcher = new Timer { Interval = 1000 * hbInterval };
            //_networkWatcher.Elapsed += NetworkWatcherOnElapsed;
            
            _app.HeartBeatTimer.Elapsed += NetworkWatcherOnElapsed;
            
            Stages = _app.QusetStages;
            Stages.QuestStarted += StagesOnQuestStarted;
            Stages.QuestStopped += StagesOnQuestStopped;
            Stages.QuestCompleted += StagesOnQuestCompleted;

            


            _app.HeartBeatTimer.Start();
            if (!_app.Connected)
                return;
            PrepareWindow();
            
        }

        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
                _player.Dispose();
        }

        private void PrepareWindow()
        {
            
            _app.Dispatcher.Invoke(() =>
            {
                CallButton.IsEnabled = false;
                KeyButton.IsEnabled = false;
                TotalElapsed.Visibility = Visibility.Hidden;
            });
            
        }

        private void StagesOnQuestCompleted(object sender, QuestCompletedHandlerArgs args)
        {
            _app.Dispatcher.Invoke(() =>
            {
                _dispatcherTimer.Stop();
                _player.Stop();

                TotalElapsed.Content = "GAME OVER";
                _app.QuestServiceClient.QuestCompleted(_app.QuestId);
                CallButton.IsEnabled = false;
                KeyButton.IsEnabled = false;
            });
        }

        private void NetworkWatcherOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _app.HeartBeatTimer.Stop();

            var maxInterval = new TimeSpan(0, 0, 0, int.Parse(ConfigurationManager.AppSettings["HeartBeatSec"]));
            var interval = DateTime.Now - _app.ServerPing;
            
            if (interval > maxInterval)
            {
                var keysCount = _app.ConnectToServer();
                if (keysCount > 0)
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
                    _app.Dispatcher.Invoke(() =>
                    {
                        TotalElapsed.Visibility = Visibility.Hidden;
                        CallButton.IsEnabled = false;
                        KeyButton.IsEnabled = false;
                    });
                }
            }
            
            
            //var app = (App)Application.Current;
            /*if (!app.PingServer())
            {
                
            }*/
            _app.HeartBeatTimer.Start();
        }

        private void DispatcherTimerOnTick(object sender, EventArgs eventArgs)
        {
            if (!Stages.QuestRunning) 
                return;
            
            var totalRemaining = Stages.QuestDuration - Stages.TotalTime.Elapsed;
            
            TotalElapsed.Content = totalRemaining.ToString("mm\\:ss");

            if (!TotalElapsed.IsVisible)
                TotalElapsed.Visibility = Visibility.Visible;
        }

        private void StagesOnQuestStopped(object sender, QuestStoppedHandlerArgs args)
        {
            _dispatcherTimer.Stop();
            
            _app.Dispatcher.Invoke(() =>
            {
                _player.Stop();
                TotalElapsed.Visibility = Visibility.Hidden;
                CallButton.IsEnabled = false;
                KeyButton.IsEnabled = false;
            });
            
        }

        private void StagesOnQuestStarted(object sender, QuestStartedHandlerArgs args)
        {
            _app.Dispatcher.Invoke(() =>
            {
                TotalElapsed.Visibility = Visibility.Hidden;
                CallButton.IsEnabled = true;
                KeyButton.IsEnabled = true;
                _player.Play();
                _dispatcherTimer.Start();
            });
        }

        private void CallButtonOnClick(object sender, RoutedEventArgs e)
        {
            var phoneWindow = new PhoneWindow(/*_mp3Volume*/) { Owner = this };
            _player.Mute = true;
            phoneWindow.Closed += (o, args) => { _player.Mute = false; };
            phoneWindow.ShowDialog();
        }

        private void KeyButtonOnClick(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                var keyViewer = new KeyWindow(Stages.CurrentStage.Key) { Owner = this };
                keyViewer.ShowDialog();
            });

            Dispatcher.InvokeAsync(() =>
            {
               _app.QuestServiceClient.KeyViewed(Stages.CurrentStage.Key.QuestId, Stages.CurrentStage.Key.KeyId);
            });
        }
    }
}
