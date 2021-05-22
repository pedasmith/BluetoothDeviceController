using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Storage.Streams;

namespace BluetoothDeviceController.Beacons
{
    public static class Ruuvi_Tag_v1_Helper
    {
        public class Ruuvi_Communication
        {
            public Ruuvi_Communication(bool success, double temperature, double pressure, double humidity, string ruuviUri = null)
            {
                Success = success;
                Data = new Ruuvi_DataRecord(temperature, pressure, humidity);
                RuuviUri = ruuviUri;
            }
            public static Ruuvi_Communication MakeFailure()
            {
                return new Ruuvi_Communication(false, double.NaN, double.NaN, double.NaN);
            }
            public bool Success { get; set; }
            public Ruuvi_DataRecord Data { get; set; } = null;
            public string RuuviUri { get; set; } = null;
            public override string ToString()
            {
                if (!Success) return "Ruuvi: Unable to communicate";
                return Data.ToString();
            }
        }
        public class Ruuvi_DataRecord : INotifyPropertyChanged
        {
            public Ruuvi_DataRecord()
            {
                Temperature = double.NaN;
                Pressure = double.NaN;
                Humidity = double.NaN;
            }
            public Ruuvi_DataRecord(double temperature, double pressure, double humidity)
            {
                Temperature = temperature;
                Pressure = pressure;
                Humidity = humidity;
            }


            // For the INPC INotifyPropertyChanged values
            public event PropertyChangedEventHandler PropertyChanged;
            protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            private DateTime _EventTime;
            public DateTime EventTime { get { return _EventTime; } set { if (value == _EventTime) return; _EventTime = value; OnPropertyChanged(); } }

            private double _Temperature;
            public double Temperature { get { return _Temperature; } set { if (value == _Temperature) return; _Temperature = value; OnPropertyChanged(); } }

            /// <summary>
            /// Pressure in hPA
            /// </summary>
            private double _Pressure;
            public double Pressure { get { return _Pressure; } set { if (value == _Pressure) return; _Pressure = value; OnPropertyChanged(); } }

            private double _Humidity;
            public double Humidity { get { return _Humidity; } set { if (value == _Humidity) return; _Humidity = value; OnPropertyChanged(); } }

            private String _Note;
            public String Note { get { return _Note; } set { if (value == _Note) return; _Note = value; OnPropertyChanged(); } }

            public override string ToString()
            {
                var retval = $"Ruuvi {Temperature}℃ {Pressure}mb {Humidity}%";
                return retval;
            }
        }

        /// <summary>
        /// Parse the URL that RuuviTags produce
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static Ruuvi_Communication ParseRuuviTag(string url)
        {
            bool success = false;
            double temperature = double.NaN;
            double pressure = double.NaN;
            double humidity = double.NaN;
            var hi = url.IndexOf('#');
            if (hi < 0) return Ruuvi_Communication.MakeFailure();
            var str = url.Substring(hi + 1);
            var ruuviData = System.Convert.FromBase64String(str);

            // https://github.com/ruuvi/ruuvi-sensor-protocols
            var dr = DataReader.FromBuffer(ruuviData.AsBuffer());
            dr.ByteOrder = ByteOrder.BigEndian;
            var format = dr.ReadByte();
            switch (format)
            {
                case 2:
                case 4:
                    humidity = dr.ReadByte() / 2;
                    var temp = dr.ReadByte();
                    var tempFraction = dr.ReadByte();
                    var tsigned = (temp & 0x80) != 0;
                    if (tsigned)
                    {
                        temp = (byte)(temp & 0x7F);
                        temperature = -1.0 * (temp & 0x7F) + (double)tempFraction / 100.0;
                    }
                    else
                    {
                        temperature = (double)temp + (double)tempFraction / 100.0;
                    }

                    pressure = (double)(dr.ReadUInt16() + 50000) / 100.0;  // Convert to hPA

                    success = true;

                    byte tag = 0;
                    //bool gotTag = false;
                    if (dr.UnconsumedBufferLength >= 1)
                    {
                        tag = dr.ReadByte();
                        //gotTag = true; // NOTE: actually use the Tag and gotTag values?
                    }
                    break;
            }
            return new Ruuvi_Communication(success, temperature, pressure, humidity, url);
        }

        public static Ruuvi_Communication GetRuuviUrl(BluetoothLEAdvertisementReceivedEventArgs ble)
        {
            // Lets's see if it's an Eddystone beacon...
            // https://github.com/google/eddystone
            // https://github.com/google/eddystone/blob/master/protocol-specification.md

            foreach (var section in ble.Advertisement.DataSections)
            {
                switch (section.DataType)
                {
                    case 0x16: // 22=service data
                        var dr = DataReader.FromBuffer(section.Data);
                        dr.ByteOrder = ByteOrder.LittleEndian;
                        var Service = dr.ReadUInt16();
                        // https://github.com/google/eddystone
                        if (Service == 0xFEAA) // An Eddystone type
                        {
                            //EddystoneFrameType = (byte)(0x0F & (dr.ReadByte() >> 4));
                            var EddystoneFrameType = dr.ReadByte();
                            const int TypeURL = 0x10;
                            switch (EddystoneFrameType)
                            {
                                case TypeURL: // 0x10: An Eddystone-URL
                                    // https://github.com/google/eddystone/tree/master/eddystone-url
                                    var result = BluetoothDeviceController.Beacons.Eddystone.ParseEddystoneUrlArgs(section.Data);
                                    if (result.Success && result.Url.StartsWith("https://ruu.vi/#"))
                                    {
                                        //foundValues.Add(AdvertisementType.RuuviTag);
                                        var ruuvi = ParseRuuviTag(result.Url);
                                        ruuvi.Data.EventTime = DateTime.Now;
                                        return ruuvi;
                                    }
                                    break;
                            }
                        }
                        break;
                }
            }
            return Ruuvi_Communication.MakeFailure();
        }
    }
}
