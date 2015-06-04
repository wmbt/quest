using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Key
    {
        public int QuestId { get; set; }
        public TimeSpan TimeOffset { get; set; }
        public string Description { get; set; }
        public byte[] Image { get; set; }
        public int SensorId { get; set; }
    }

    
}
