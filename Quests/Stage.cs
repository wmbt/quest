using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Common;

namespace Quests
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
     
        private readonly List<Stage> _stages = new List<Stage>();
        private readonly Timer _timer = new Timer();
        private Stage _currentStage;


        public Stages(IEnumerable<Key> keys)
        {
            foreach (var key in keys)
                _stages.Add(new Stage(key));

            _timer.Elapsed += TimerOnElapsed;
            _timer.AutoReset = false;
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _currentStage.TimerFired = true;
            
            OnKeyPublished(new KeyPublishedHandlerArgs(_currentStage.Key));
        }

        public IEnumerator<Stage> GetEnumerator()
        {
            return _stages.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void SensorTriggered(int sensorId)
        {
            var completedStage = _stages.Single(x => x.Key.SensorId == sensorId);
            
            completedStage.SensorTriggered = true;
            _timer.Stop();

            var completedIndex = _stages.IndexOf(completedStage);

            if (completedIndex < _stages.Count - 1)
            {
                OnStageCompleted(new StageCompletedHandlerArgs(completedStage));
                _currentStage = _stages[completedIndex + 1];
                _timer.Interval = _currentStage.Key.TimeOffset.Milliseconds;
                _timer.Start();

            }
            else
            {
                OnQuestCompleted(new QuestCompletedHandlerArgs());
            }

            
        }
        public void StartWatch() 
        {
            ResetWatch();
            _currentStage = _stages.First();
            _timer.Interval = _currentStage.Key.TimeOffset.Milliseconds;
            _timer.Start();
        }

        public void StopWatch()
        {
            ResetWatch();
        }

        private void ResetWatch()
        {
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
        public Key Key { get; private set; }
        
        public KeyPublishedHandlerArgs(Key k)
        {
            Key = k;
        }
    }
}
