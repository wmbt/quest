using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public class Room
    {
        internal RoomCollection RoomCollection;
        public int Id { get; internal set; }
        public SensorsCollection Sensors { get; private set; }
        public bool AllowListening { get; set; }
        public event SensorTriggeredHandler SensorTriggered;
        
        public Room(int id, IEnumerable<int> sensorIds)
        {
            Id = id;
            Sensors = new SensorsCollection(this);

            foreach (var s in sensorIds.Select(i => new Sensor(i)))
            {
                s.SensorTriggered += OnSensorTriggered;
                Sensors.Add(s);
                s.Room = this;
            }
        }
        private void OnSensorTriggered(object sender, SensorTriggeredArgs args)
        {
            if (!AllowListening)
                return;
            
            var handler = SensorTriggered;
            if (handler != null) handler(this, args);   
        }
        public void ResetSensors()
        {
            foreach (var sensor in Sensors)
                sensor.Reset();
        }
        public void EnableSensors()
        {
            foreach (var sensor in Sensors)
                sensor.Enabled = true;
        }

    }

    public class RoomCollection : IEnumerable<Room>
    {
        private readonly List<Room> _rooms = new List<Room>();

        public void Add(Room room)
        {
            room.RoomCollection = this;
            _rooms.Add(room);
        }

        public IEnumerator<Room> GetEnumerator()
        {
            return _rooms.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Dictionary<int, Sensor> GetAllSensors()
        {
            return _rooms.SelectMany(room => room.Sensors).ToDictionary(s => s.Id);
        }

        public Room GetById(int roomId)
        {
            return _rooms.Single(x => x.Id == roomId);
        }
    }
}
