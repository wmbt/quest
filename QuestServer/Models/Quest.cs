using System;
using System.Runtime.InteropServices.ComTypes;
using Common;

namespace QuestServer.Models
{
    public class Quest
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public Key[] Keys { get; set; }
        public Sensor[] Sensors { get; set; }
        public DateTime LastPing { get; set; }
    }
}