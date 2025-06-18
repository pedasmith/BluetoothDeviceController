using Parsers.Nmea;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace TestNmeaGpsParserWinUI
{
    public class NmeaMessageSummary: INotifyPropertyChanged
    {
        public NmeaMessageSummary(string name) { Name = name; }
        /// <summary>
        /// Message name e.g. $GPGGA
        /// </summary>
        public string Name { get { return name; }  internal set { name = value; OnPropertyChanged(); } }
        private string name = "";



        public string Explanation { get { return Nmea_Data.OpcodeExplanation(MostRecentData?.GetFirstPart()); } }
        /// <summary>
        /// Number of OK messages
        /// </summary>
        public int NOk { get; internal set; } = 0;
        /// <summary>
        /// Number of incorrect messages
        /// </summary>
        public int NError { get; internal set; } = 0;
        public int NTotal {  get { return NOk + NError; } }
        public DateTimeOffset FirstMessage { get; set; } = DateTimeOffset.MaxValue;
        public DateTimeOffset LastMessage { get; set;} = DateTimeOffset.MinValue;

        public Nmea_Data MostRecentData { get; internal set; } = null;

        public void Add (Nmea_Data data)
        {
            bool isOk = data.ParseStatus == Nmea_Gps_Parser.ParseResult.Ok;
            if (isOk) NOk++; else NError++;
            var now = DateTimeOffset.UtcNow;
            bool setFirst = false;
            if (FirstMessage == DateTimeOffset.MaxValue)
            {
                setFirst = true;
                FirstMessage = now;
            }
            LastMessage = now;
            MostRecentData = data;

            OnPropertyChanged(isOk ? "NOk" : "NError");
            OnPropertyChanged("NTotal");
            if (setFirst) OnPropertyChanged("FirstMessage");
            OnPropertyChanged("LastMessage");
            OnPropertyChanged("LastMessageSummary");
            OnPropertyChanged("LastMessageDetails");
            OnPropertyChanged("LastMessageHeader");
            OnPropertyChanged("LastMessageExplanation");
        }


        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
