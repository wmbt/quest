using System;

namespace QuestServer.Models
{
    public class Sensor
    {
        public int QuestId { get; set; }
        public int Id { get; set; }
        public bool Triggered { get; private set; }
        public bool Active { get; private set; }
        public bool Enabled { get; private set; }
        public event SensorTriggeredHandler SensorTriggered;
        
        public Sensor()
        {
            Triggered = false;
            Active = false;
            Enabled = false;
        }

        internal void SetState(bool active)
        {
            if (!Enabled)
                return;
            
            Active = active;
            
            if (!Active || Triggered) 
                return;
            Triggered = true;
            OnSensorTriggered(new SensorTriggeredArgs());
        }

        internal void Reset()
        {
            Triggered = Active = Enabled = false;
        }

        private void OnSensorTriggered(SensorTriggeredArgs args)
        {
            var handler = SensorTriggered;
            if (handler != null) handler(this, args);
        }
    }

    public delegate void SensorTriggeredHandler(object sender, SensorTriggeredArgs args);

    public class SensorTriggeredArgs : EventArgs
    {
    }

    //public class SensorCollection : IEnumerable<Sensor>
    //{
    //    readonly List<Sensor> _sensors = new List<Sensor>();
    //    private Room _room;

    //    internal SensorCollection(Room room)
    //    {
    //        _room = room;
    //    }

    //    public IEnumerator<Sensor> GetEnumerator()
    //    {
    //        return _sensors.GetEnumerator();
    //    }

    //    IEnumerator IEnumerable.GetEnumerator()
    //    {
    //        return GetEnumerator();
    //    }

    //    internal void Add(Sensor sensor)
    //    {
    //        _sensors.Add(sensor);
    //    }
    //}
}
