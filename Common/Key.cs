using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Key : INotifyPropertyChanged
    {
        private bool _viewed;
        
        public int KeyId { get; set; }
        public int QuestId { get; set; }
        public TimeSpan TimeOffset { get; set; }
        public string Description { get; set; }
        public byte[] Image { get; set; }
        public int SensorId { get; set; }
        public bool Viewed
        {
            get { return _viewed; }
            set
            {
                _viewed = value;
                OnPropertyChanged("Viewed");
            } 
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    
}
