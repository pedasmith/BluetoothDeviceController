using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothProtocols
{
    public class BTCommonMetaData : INotifyPropertyChanged
    {
        // For the INPC INotifyPropertyChanged values
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private DateTimeOffset _TimestampMostRecent = DateTimeOffset.MinValue;
        public DateTimeOffset TimestampMostRecent
        {
            get { return _TimestampMostRecent; }
            set
            {
                if (value == _TimestampMostRecent) return;
                _TimestampMostRecent = value;
                OnPropertyChanged();
                OnPropertyChanged("TimestamptMostRecentDT");
            }
        }
        public DateTime TimestampMostRecentDT { get { return TimestampMostRecent.DateTime; } }

        private string _Name = "";
        public string Name
        {
            get { return _Name; }
            set
            {
                if (value == _Name) return;
                _Name = value;
                OnPropertyChanged();
            }
        } 

    }
}
