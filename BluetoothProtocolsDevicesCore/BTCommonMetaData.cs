using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using static BluetoothProtocols.BTStandard_HeartRate;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothProtocols
{
    public interface IBTCommonMetaData
    {
        string[] ExportGetHeaders(IExportData exporter);
        void ExportRow(IExportData exporter);
        DateTimeOffset TimestampMostRecent { get; set; }
        DateTime TimestampMostRecentDT { get; }
        string Name { get; set; }
    }

    public abstract class BTCommonMetaData<Tself> : IBTCommonMetaData, IExportDataSource, INotifyPropertyChanged
    {
        // For the INPC INotifyPropertyChanged values
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public abstract Tself Clone(string name = null);
        public abstract void CopyFrom(Tself value);

        public abstract string[] ExportGetHeaders(IExportData exporter);

        public abstract void ExportRow(IExportData exporter);

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
