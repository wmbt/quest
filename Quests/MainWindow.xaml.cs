using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Common.Phone;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using QuestClient;

namespace Quests
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly WaveOut _backgroundPlayer = new WaveOut();
        private readonly MediaFoundationReader _mp3Track;
        private PhoneEngine _phone;
        private readonly DispatcherTimer _dispatcherTimer;
        private Stages Stages { get; set; }
        private readonly Dictionary<Stage, KeyButton> _currentKeyButtons = new Dictionary<Stage, KeyButton>();
        private readonly Timer _networkWatcher;
        private readonly App _app;
        private readonly KeyButton[] _allKeyButtons;
        private VolumeSampleProvider _mp3Volume;

        public MainWindow()
        {
            InitializeComponent();
            TotalElapsed.Visibility = Visibility.Hidden;
            Closing += OnClosing;
            var hbInterval = int.Parse(ConfigurationManager.AppSettings["HeartBeatSec"]);
            _app = (App) Application.Current;
            _dispatcherTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 1) };
            _dispatcherTimer.Tick += DispatcherTimerOnTick;
            _networkWatcher = new Timer { Interval = 1000 * hbInterval };
            _networkWatcher.Elapsed += NetworkWatcherOnElapsed;
            _allKeyButtons = TopGrid.Children.OfType<KeyButton>().ToArray();

            foreach (var keyButton in _allKeyButtons)
            {
                keyButton.ButtonControl.Click += LockOnClick;
            }

            Stages = _app.QusetStages;
            Stages.KeyPublished += StagesOnKeyPublished;
            Stages.QuestStarted += StagesOnQuestStarted;
            Stages.QuestStopped += StagesOnQuestStopped;
            Stages.QuestCompleted += StagesOnQuestCompleted;

            if (File.Exists(@"Media\background.mp3"))
            {
                _mp3Track = new MediaFoundationReader(@"Media\background.mp3");
                _mp3Volume = new VolumeSampleProvider(_mp3Track.ToSampleProvider());
                _backgroundPlayer.Init(_mp3Volume);
            }

            _networkWatcher.Start();

            if (!_app.Connected)
                return;
            PrepareWindow(_app.KeysCount);
            
        }

        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
                _backgroundPlayer.Dispose();
        }

        private void PrepareWindow(int keysCount)
        {
            
            _app.Dispatcher.Invoke(() =>
            {
                foreach (var keyButton in _allKeyButtons)
                {
                    keyButton.ButtonControl.IsEnabled = false;
                    keyButton.Label = string.Empty;
                    keyButton.ImageControl.Source = (ImageSource)FindResource("LockImage");
                    keyButton.Visibility =
                        Array.IndexOf(_allKeyButtons, keyButton) < keysCount
                        ? Visibility.Visible : Visibility.Hidden;
                }
                TotalElapsed.Visibility = Visibility.Hidden;
            });
            
        }

        private void StagesOnQuestCompleted(object sender, QuestCompletedHandlerArgs args)
        {
            _app.Dispatcher.Invoke(() =>
            {
                _dispatcherTimer.Stop();
                if (_mp3Track != null)
                {
                    _backgroundPlayer.Stop();
                }

                TotalElapsed.Content = "Игровое время вышло";
                /*CurrentElapsed.Visibility = Visibility.Hidden;*/
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
                int keysCount = app.ConnectToServer();
                if (keysCount > 0)
                {
                    PrepareWindow(keysCount);
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
                        /*CurrentElapsed.Visibility = Visibility.Hidden;*/

                        foreach (var v in _currentKeyButtons.Values)
                        {
                            v.ButtonControl.IsEnabled = false;
                            v.Label = string.Empty;
                            v.ImageControl.Source = (ImageSource)FindResource("LockImage");
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
                var currentButton = _currentKeyButtons.Single(x => x.Key.Key == Stages.CurrentStage.Key).Value;
                
                currentButton.Label = currentRemaining.ToString("mm\\:ss");

                if (!TotalElapsed.IsVisible)
                    TotalElapsed.Visibility = Visibility.Visible;
                
                /*if(!CurrentElapsed.IsVisible)
                    CurrentElapsed.Visibility = Visibility.Visible;*/
            }
        }

        private void StagesOnQuestStopped(object sender, QuestStoppedHandlerArgs args)
        {
            _dispatcherTimer.Stop();
            if (_mp3Track != null)
            {
                _backgroundPlayer.Stop();
            }

            _app.Dispatcher.Invoke(() =>
            {
                TotalElapsed.Visibility = Visibility.Hidden;
                /*CurrentElapsed.Visibility = Visibility.Hidden;*/
                CallButton.IsEnabled = false;

                foreach (var v in _allKeyButtons)
                {
                    v.ButtonControl.IsEnabled = false;
                    v.Label = String.Empty;
                    v.ImageControl.Source = (ImageSource)FindResource("LockImage");
                    
                }    
            });
            
        }

        private void StagesOnQuestStarted(object sender, QuestStartedHandlerArgs args)
        {
            _app.Dispatcher.InvokeAsync(() =>
            {
                _currentKeyButtons.Clear();
                var stagesCount = Stages.Count();
                //var locks = TopGrid.Children.OfType<Button>().Where(x => x.Name.Contains("Key")).ToArray();
                for (var i = 0; i < stagesCount; i++)
                {
                    _currentKeyButtons.Add(Stages[i], _allKeyButtons[i]);
                    _allKeyButtons[i].ButtonControl.IsEnabled = false;
                    _allKeyButtons[i].Label = String.Empty;
                    _allKeyButtons[i].ImageControl.Source = (ImageSource) FindResource("LockImage");
                }

                TotalElapsed.Visibility = Visibility.Hidden;
                /*foreach (var v in _currentKeyButtons.Values)
            {
               v.IsEnabled = false;
            }*/
                CallButton.IsEnabled = true;


                if (_mp3Track != null)
                {
                    _mp3Track.CurrentTime = TimeSpan.Zero;
                    _backgroundPlayer.Play();
                }
            });
            _dispatcherTimer.Start();
        }

        //private void StagesOnStageCompleted(object sender, StageCompletedHandlerArgs args)
        //{
           
        //}

        private void StagesOnKeyPublished(object sender, KeyPublishedHandlerArgs args)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _currentKeyButtons[args.Stage].ButtonControl.IsEnabled = true;
                _currentKeyButtons[args.Stage].ImageControl.Source = (ImageSource)FindResource("KeyImage");
                _currentKeyButtons[args.Stage].Label = String.Empty;
            });
        }

        private void LockOnClick(object sender, RoutedEventArgs e)
        {
            var stage = _currentKeyButtons.SingleOrDefault(x => Equals(x.Value.ButtonControl, (Button) sender)).Key;
            
            var keyViewer = new KeyWindow(stage.Key){ Owner = this};
            keyViewer.ShowDialog();
        }

        private void CallButtonOnClick(object sender, RoutedEventArgs e)
        {
            var phoneWindow = new PhoneWindow(_mp3Volume) { Owner = this };
            phoneWindow.ShowDialog();
        }
    }
}
