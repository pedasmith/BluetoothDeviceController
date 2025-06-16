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
        public NmeaMessageSummary(string name, string explanation) { Name = name; Explanation = explanation; }
        /// <summary>
        /// Message name e.g. $GPGGA
        /// </summary>
        public string Name { get { return name; }  internal set { name = value; OnPropertyChanged(); } }
        private string name = "";

        public string LastMessageDecoded { get { return lastMessageDecoded; } internal set {  lastMessageDecoded = value; OnPropertyChanged(); } }
        private string lastMessageDecoded = "";

        public string Explanation { get { return explanation; } internal set { explanation = value; OnPropertyChanged(); } }
        private string explanation = "";
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
            LastMessageDecoded = data.ToString(); // All Nmea_Data derived classes have usable ToString overrides.

            OnPropertyChanged(isOk ? "NOk" : "NError");
            OnPropertyChanged("NTotal");
            if (setFirst) OnPropertyChanged("FirstMessage");
            OnPropertyChanged("LastMessage");
            OnPropertyChanged("LastMessageDecoded");
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
