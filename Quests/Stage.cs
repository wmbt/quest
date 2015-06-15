using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using Common;

namespace QuestClient
{
    public class Stage
    {
        public Key Key { get; private set; }
        public bool SensorTriggered { get; internal set; }
        /*public bool KeyViewed { get; internal set; }*/
        public bool TimerFired { get; internal set; }
        public Stage(Key key)
        {
            Key = key;
        }
    }

    public class Stages : IEnumerable<Stage>
    {
        public event KeyPublishedHandler KeyPublished;
        public event StageCompletedHandler StageCompleted;
        public event QuestCompletedHandler QuestCompleted;
        public event QuestStartedHandler QuestStarted;
        public event QuestStoppedHandler QuestStopped;
        public bool QuestRunning { get; private set; }
     
        private readonly List<Stage> _stages = new List<Stage>();
        private readonly Timer _timer = new Timer();
        public Stopwatch TotalTime { get; private set; }
        public Stopwatch CurrentTime { get; private set; }
        public Stage CurrentStage { get; private set;}
        public TimeSpan QuestDuration { get; private set; }


        public Stage this[int i]
        {
            get { return _stages[i]; }
        }

        public Stages(IEnumerable<Key> keys)
        {
            foreach (var key in keys)
                _stages.Add(new Stage(key));

            _timer.Elapsed += TimerOnElapsed;
            _timer.AutoReset = false;
            QuestRunning = false;
            TotalTime = new Stopwatch();
            CurrentTime = new Stopwatch();
            QuestDuration = new TimeSpan();

            foreach (var stage in _stages)
            {
                QuestDuration += stage.Key.TimeOffset;
            }
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            CurrentStage.TimerFired = true;
            OnKeyPublished(new KeyPublishedHandlerArgs(CurrentStage));

            CurrentTime.Reset();
            _timer.Stop();
            var completedIndex = _stages.IndexOf(CurrentStage);

            if (completedIndex < _stages.Count - 1)
            {
                CurrentStage = _stages[completedIndex + 1];
                _timer.Interval = CurrentStage.Key.TimeOffset.TotalMilliseconds;
                CurrentTime.Start();
                _timer.Start();
            }
            else
            {
                ResetWatch();
                OnQuestCompleted(new QuestCompletedHandlerArgs());
            }
        }


        public IEnumerator<Stage> GetEnumerator()
        {
            return _stages.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /*public void SensorTriggered(int sensorId)
        {
            var completedStage = _stages.Single(x => x.Key.SensorId == sensorId);
            
            completedStage.SensorTriggered = true;
            _timer.Stop();

            var completedIndex = _stages.IndexOf(completedStage);

            if (completedIndex < _stages.Count - 1)
            {
                OnStageCompleted(new StageCompletedHandlerArgs(completedStage));
                _currentStage = _stages[completedIndex + 1];
                _timer.Interval = _currentStage.Key.TimeOffset.TotalMilliseconds;
                _timer.Start();

            }
            else
            {
                OnQuestCompleted(new QuestCompletedHandlerArgs());
            }

            
        }*/
        public void StartWatch() 
        {
            ResetWatch();
            CurrentStage = _stages.First();
            _timer.Interval = CurrentStage.Key.TimeOffset.TotalMilliseconds;
            OnQuestStarted(new QuestStartedHandlerArgs());
            QuestRunning = true;
            _timer.Start();
            TotalTime.Start();
            CurrentTime.Start();
        }

        public void StopWatch()
        {
            ResetWatch();
            OnQuestStopped(new QuestStoppedHandlerArgs());
            QuestRunning = false;
        }

        private void ResetWatch()
        {
            TotalTime.Reset();
            CurrentTime.Reset();
            _timer.Stop();
            _stages.ForEach(x => { x.TimerFired = x.SensorTriggered = false; });
        }
        
        protected virtual void OnKeyPublished(KeyPublishedHandlerArgs args)
        {
            var handler = KeyPublished;
            if (handler != null) handler(this, args);
        }

        protected virtual void OnStageCompleted(StageCompletedHandlerArgs args)
        {
            var handler = StageCompleted;
            if (handler != null) handler(this, args);
        }

        protected virtual void OnQuestCompleted(QuestCompletedHandlerArgs args)
        {
            var handler = QuestCompleted;
            if (handler != null) handler(this, args);
        }

        protected virtual void OnQuestStarted(QuestStartedHandlerArgs args)
        {
            var handler = QuestStarted;
            if (handler != null) handler(this, args);   
        }

        protected virtual void OnQuestStopped(QuestStoppedHandlerArgs args)
        {
            var handler = QuestStopped;
            if (handler != null) handler(this, args);
        }
    }

    public delegate void QuestStoppedHandler(object sender, QuestStoppedHandlerArgs args);

    public class QuestStoppedHandlerArgs : EventArgs
    {
    }

    public delegate void QuestStartedHandler(object sender, QuestStartedHandlerArgs args);

    public class QuestStartedHandlerArgs : EventArgs   
    {
    }

    public delegate void QuestCompletedHandler(object sender, QuestCompletedHandlerArgs args);

    public class QuestCompletedHandlerArgs :EventArgs
    {
    }

    public delegate void StageCompletedHandler(object sender, StageCompletedHandlerArgs args);

    public class StageCompletedHandlerArgs : EventArgs
    {
        public Stage Stage { get; private set; }

        public StageCompletedHandlerArgs(Stage stage)
        {
            Stage = stage;
        }
    }

    public delegate void KeyPublishedHandler(object sender, KeyPublishedHandlerArgs args);

    public class KeyPublishedHandlerArgs : EventArgs
    {
        public Stage Stage { get; private set; }

        public KeyPublishedHandlerArgs(Stage s)
        {
            Stage = s;
        }
    }
}
