//From template: Protocol_Core_Body v2026-04-17 11:43
using System;
using System.Collections.Generic;
using System.ComponentModel; // Needed for INotifyPropertyChanged
using System.Runtime.CompilerServices; // Needed for CallerMemberNameAttribute
using System.Runtime.InteropServices.WindowsRuntime; // Needed for IBuffer.ToArray extension method
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothProtocols
{
    /// <summary>
    /// The Nordic Thingy:52™ is an easy-to-use prototyping platform, designed to help in building prototypes and demos, without the need to build hardware or even write firmware. It is built around the nRF52832 Bluetooth 5 SoC.
    /// This class was automatically generated 2026-06-17::09:35
    /// </summary>

    public  class Nordic_Thingy : INotifyPropertyChanged
    {
        // Useful links for the device and protocol documentation
        // Link: https://nordicsemiconductor.github.io/Nordic-Thingy52-FW/documentation/firmware_architecture.html#fw_arch_ble_services

        public BluetoothLEDevice ble { get; set; } = null;
        public BluetoothStatusEvent Status = new BluetoothStatusEvent();

        // For the INotifyPropertyChanged values
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Name { get; } = "Thingy";
        public string Description { get; } = "The Nordic Thingy:52™ is an easy-to-use prototyping platform, designed to help in building prototypes and demos, without the need to build hardware or even write firmware. It is built around the nRF52832 Bluetooth 5 SoC";

        /* Service and Characteristics summary for the device Thingy

        Environment service Guid=ef680200-9b35-4933-9b10-52ffa9740042
            Environment_Data (DataGroup record)
                Temperature (c) characteristic has Temperature (double-->double)  Guid=ef680201-9b35-4933-9b10-52ffa9740042
                Pressure (hpa) characteristic has Pressure (double-->double)  Guid=ef680202-9b35-4933-9b10-52ffa9740042
                Humidity (%) characteristic has Humidity (Byte-->double)  Guid=ef680203-9b35-4933-9b10-52ffa9740042
                Air Quality eCOS TVOC characteristic has eCOS (UInt16-->double) TVOC (UInt16-->double)  Guid=ef680204-9b35-4933-9b10-52ffa9740042

            EnvironmentColor_Data (DataGroup record)
                Color RGB+Clear characteristic has Red (UInt16-->double) Green (UInt16-->double) Blue (UInt16-->double) Clear (UInt16-->double)  Guid=ef680205-9b35-4933-9b10-52ffa9740042

            EnvironmentConfiguration_Data (DataGroup record)
                Environment Configuration characteristic has TempInterval (UInt16-->double) PressureInterval (UInt16-->double) HumidityInterval (UInt16-->double) ColorInterval (UInt16-->double) GasMode (Byte-->double) RedCalibration (Byte-->double) GreenCalibration (Byte-->double) BlueCalibration (Byte-->double)  Guid=ef680206-9b35-4933-9b10-52ffa9740042


        Common Configuration service Guid=1800
            Common Configuration_Data (DataGroup record)
                Device Name characteristic has Device_Name (String-->string)  Guid=2a00
                Appearance characteristic has Appearance (UInt16-->double)  Guid=2a01
                Connection Parameter characteristic has ConnectionParameter (Bytes-->string)  Guid=2a04
                Central Address Resolution characteristic has AddressResolutionSupported (Byte-->double)  Guid=2aa6


        Generic Service service Guid=1801
            Generic Service_Data (DataGroup record)
                Service Changes characteristic has StartRange (UInt16-->double) EndRange (UInt16-->double)  Guid=2a05


        Battery service Guid=180f
            Battery_Data (DataGroup record)
                BatteryLevel characteristic has BatteryLevel (SByte-->double)  Guid=2a19


        Configuration service Guid=ef680100-9b35-4933-9b10-52ffa9740042
            Configuration_Data (DataGroup record)
                Configuration Device Name characteristic has DeviceName (String-->string)  Guid=ef680101-9b35-4933-9b10-52ffa9740042
                Advertising Parameter characteristic has Interval (UInt16-->double) Timeout (Byte-->double)  Guid=ef680102-9b35-4933-9b10-52ffa9740042
                Connection parameters characteristic has MinInterval (UInt16-->double) MaxInterval (UInt16-->double) Latency (UInt16-->double) SupervisionTimeout (UInt16-->double)  Guid=ef680104-9b35-4933-9b10-52ffa9740042
                Eddystone URL characteristic has Eddystone (String-->string)  Guid=ef680105-9b35-4933-9b10-52ffa9740042
                Cloud Token characteristic has CloudToken (Bytes-->string)  Guid=ef680106-9b35-4933-9b10-52ffa9740042
                Firmware Version characteristic has Major (Byte-->double) Minor (Byte-->double) Patch (Byte-->double)  Guid=ef680107-9b35-4933-9b10-52ffa9740042
                MTU Request characteristic has param0 (Byte-->double) param1 (UInt16-->double)  Guid=ef680108-9b35-4933-9b10-52ffa9740042
                NFC Tag characteristic has NFCTag (String-->string)  Guid=ef680109-9b35-4933-9b10-52ffa9740042
        */

        public const string Temperature_cPropertyChangedName = "Temperature_c";
        public const string Pressure_hpaPropertyChangedName = "Pressure_hpa";
        public const string HumidityPropertyChangedName = "Humidity";
        public const string Air_Quality_eCOS_TVOCPropertyChangedName = "Air_Quality_eCOS_TVOC";
        public const string Color_RGB_ClearPropertyChangedName = "Color_RGB_Clear";
        public const string Environment_ConfigurationPropertyChangedName = "Environment_Configuration";
        public const string Device_NamePropertyChangedName = "Device_Name";
        public const string AppearancePropertyChangedName = "Appearance";
        public const string Connection_ParameterPropertyChangedName = "Connection_Parameter";
        public const string Central_Address_ResolutionPropertyChangedName = "Central_Address_Resolution";
        public const string Service_ChangesPropertyChangedName = "Service_Changes";
        public const string BatteryLevelPropertyChangedName = "BatteryLevel";
        public const string Configuration_Device_NamePropertyChangedName = "Configuration_Device_Name";
        public const string Advertising_ParameterPropertyChangedName = "Advertising_Parameter";
        public const string Connection_parametersPropertyChangedName = "Connection_parameters";
        public const string Eddystone_URLPropertyChangedName = "Eddystone_URL";
        public const string Cloud_TokenPropertyChangedName = "Cloud_Token";
        public const string Firmware_VersionPropertyChangedName = "Firmware_Version";
        public const string MTU_RequestPropertyChangedName = "MTU_Request";
        public const string NFC_TagPropertyChangedName = "NFC_Tag";



        //
        // All services / characteristics data types 
        //

        #region All_Data_Types
        /// <summary>
        /// Data from all of the characteristics in the Environment Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// </summary>
        public class Environment_Data :BTCommonMetaData //, IExportDataSource
        {
            // Template is ServiceDataGroups
            private double _Temperature = 0.0;
            /// <summary>
            /// From Environment and Temperature (c)
            ///</summary>
            public double Temperature 
            { 
                get { return _Temperature; }
                set { if (value == _Temperature) return; _Temperature = value; OnPropertyChanged();}
            }
            private double _Pressure = 0.0;
            /// <summary>
            /// From Environment and Pressure (hpa)
            ///</summary>
            public double Pressure 
            { 
                get { return _Pressure; }
                set { if (value == _Pressure) return; _Pressure = value; OnPropertyChanged();}
            }
            private double _Humidity = 0;
            /// <summary>
            /// From Environment and Humidity (%)
            ///</summary>
            public double Humidity 
            { 
                get { return _Humidity; }
                set { if (value == _Humidity) return; _Humidity = value; OnPropertyChanged();}
            }
            private double _eCOS = 390;
            /// <summary>
            /// From Environment and Air Quality eCOS TVOC
            ///</summary>
            public double eCOS 
            { 
                get { return _eCOS; }
                set { if (value == _eCOS) return; _eCOS = value; OnPropertyChanged();}
            } 
            private double _TVOC = 0;
            /// <summary>
            /// From Environment and Air Quality eCOS TVOC
            ///</summary>
            public double TVOC 
            { 
                get { return _TVOC; }
                set { if (value == _TVOC) return; _TVOC = value; OnPropertyChanged();}
            }
            public Environment_Data Clone()
            {
                return this.MemberwiseClone() as Environment_Data;
            }

            public void CopyFrom(Environment_Data value)
            {
                this.TimestampMostRecent = value.TimestampMostRecent;
                this.Name = value.Name;
                this.Temperature = value.Temperature;
                this.Pressure = value.Pressure;
                this.Humidity = value.Humidity;
                this.eCOS = value.eCOS;
                this.TVOC = value.TVOC;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["Temperature", "Pressure", "Humidity", "eCOS", "TVOC"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(Temperature);
                exporter.CellSet(Pressure);
                exporter.CellSet(Humidity);
                exporter.CellSet(eCOS);
                exporter.CellSet(TVOC);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {Temperature} {Pressure} {Humidity} {eCOS} {TVOC}");
            }
        }
        /// <summary>
        /// Data from all of the characteristics in the Environment Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// </summary>
        public class EnvironmentColor_Data :BTCommonMetaData //, IExportDataSource
        {
            // Template is ServiceDataGroups
            private double _Red = 0;
            /// <summary>
            /// From Environment and Color RGB+Clear
            ///</summary>
            public double Red 
            { 
                get { return _Red; }
                set { if (value == _Red) return; _Red = value; OnPropertyChanged();}
            } 
            private double _Green = 0;
            /// <summary>
            /// From Environment and Color RGB+Clear
            ///</summary>
            public double Green 
            { 
                get { return _Green; }
                set { if (value == _Green) return; _Green = value; OnPropertyChanged();}
            } 
            private double _Blue = 0;
            /// <summary>
            /// From Environment and Color RGB+Clear
            ///</summary>
            public double Blue 
            { 
                get { return _Blue; }
                set { if (value == _Blue) return; _Blue = value; OnPropertyChanged();}
            } 
            private double _Clear = 0;
            /// <summary>
            /// From Environment and Color RGB+Clear
            ///</summary>
            public double Clear 
            { 
                get { return _Clear; }
                set { if (value == _Clear) return; _Clear = value; OnPropertyChanged();}
            }
            public EnvironmentColor_Data Clone()
            {
                return this.MemberwiseClone() as EnvironmentColor_Data;
            }

            public void CopyFrom(EnvironmentColor_Data value)
            {
                this.TimestampMostRecent = value.TimestampMostRecent;
                this.Name = value.Name;
                this.Red = value.Red;
                this.Green = value.Green;
                this.Blue = value.Blue;
                this.Clear = value.Clear;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["Red", "Green", "Blue", "Clear"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(Red);
                exporter.CellSet(Green);
                exporter.CellSet(Blue);
                exporter.CellSet(Clear);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {Red} {Green} {Blue} {Clear}");
            }
        }
        /// <summary>
        /// Data from all of the characteristics in the Environment Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// </summary>
        public class EnvironmentConfiguration_Data :BTCommonMetaData //, IExportDataSource
        {
            // Template is ServiceDataGroups
            private double _TempInterval = 0;
            /// <summary>
            /// From Environment and Environment Configuration
            ///</summary>
            public double TempInterval 
            { 
                get { return _TempInterval; }
                set { if (value == _TempInterval) return; _TempInterval = value; OnPropertyChanged();}
            } 
            private double _PressureInterval = 0;
            /// <summary>
            /// From Environment and Environment Configuration
            ///</summary>
            public double PressureInterval 
            { 
                get { return _PressureInterval; }
                set { if (value == _PressureInterval) return; _PressureInterval = value; OnPropertyChanged();}
            } 
            private double _HumidityInterval = 0;
            /// <summary>
            /// From Environment and Environment Configuration
            ///</summary>
            public double HumidityInterval 
            { 
                get { return _HumidityInterval; }
                set { if (value == _HumidityInterval) return; _HumidityInterval = value; OnPropertyChanged();}
            } 
            private double _ColorInterval = 0;
            /// <summary>
            /// From Environment and Environment Configuration
            ///</summary>
            public double ColorInterval 
            { 
                get { return _ColorInterval; }
                set { if (value == _ColorInterval) return; _ColorInterval = value; OnPropertyChanged();}
            } 
            private double _GasMode = 0;
            /// <summary>
            /// From Environment and Environment Configuration
            ///</summary>
            public double GasMode 
            { 
                get { return _GasMode; }
                set { if (value == _GasMode) return; _GasMode = value; OnPropertyChanged();}
            } 
            private double _RedCalibration = 0;
            /// <summary>
            /// From Environment and Environment Configuration
            ///</summary>
            public double RedCalibration 
            { 
                get { return _RedCalibration; }
                set { if (value == _RedCalibration) return; _RedCalibration = value; OnPropertyChanged();}
            } 
            private double _GreenCalibration = 0;
            /// <summary>
            /// From Environment and Environment Configuration
            ///</summary>
            public double GreenCalibration 
            { 
                get { return _GreenCalibration; }
                set { if (value == _GreenCalibration) return; _GreenCalibration = value; OnPropertyChanged();}
            } 
            private double _BlueCalibration = 0;
            /// <summary>
            /// From Environment and Environment Configuration
            ///</summary>
            public double BlueCalibration 
            { 
                get { return _BlueCalibration; }
                set { if (value == _BlueCalibration) return; _BlueCalibration = value; OnPropertyChanged();}
            }
            public EnvironmentConfiguration_Data Clone()
            {
                return this.MemberwiseClone() as EnvironmentConfiguration_Data;
            }

            public void CopyFrom(EnvironmentConfiguration_Data value)
            {
                this.TimestampMostRecent = value.TimestampMostRecent;
                this.Name = value.Name;
                this.TempInterval = value.TempInterval;
                this.PressureInterval = value.PressureInterval;
                this.HumidityInterval = value.HumidityInterval;
                this.ColorInterval = value.ColorInterval;
                this.GasMode = value.GasMode;
                this.RedCalibration = value.RedCalibration;
                this.GreenCalibration = value.GreenCalibration;
                this.BlueCalibration = value.BlueCalibration;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["TempInterval", "PressureInterval", "HumidityInterval", "ColorInterval", "GasMode", "RedCalibration", "GreenCalibration", "BlueCalibration"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(TempInterval);
                exporter.CellSet(PressureInterval);
                exporter.CellSet(HumidityInterval);
                exporter.CellSet(ColorInterval);
                exporter.CellSet(GasMode);
                exporter.CellSet(RedCalibration);
                exporter.CellSet(GreenCalibration);
                exporter.CellSet(BlueCalibration);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {TempInterval} {PressureInterval} {HumidityInterval} {ColorInterval} {GasMode} {RedCalibration} {GreenCalibration} {BlueCalibration}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the Common Configuration Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// </summary>
        public class Common_Configuration_Data :BTCommonMetaData //, IExportDataSource
        {
            // Template is ServiceDataGroups
            private string _Device_Name = "";
            /// <summary>
            /// From Common Configuration and Device Name
            ///</summary>
            public string Device_Name 
            { 
                get { return _Device_Name; }
                set { if (value == _Device_Name) return; _Device_Name = value; OnPropertyChanged();}
            }
            private double _Appearance = 0;
            /// <summary>
            /// From Common Configuration and Appearance
            ///</summary>
            public double Appearance 
            { 
                get { return _Appearance; }
                set { if (value == _Appearance) return; _Appearance = value; OnPropertyChanged();}
            }
            private byte[] _ConnectionParameter = null;
            /// <summary>
            /// From Common Configuration and Connection Parameter
            ///</summary>
            public byte[] ConnectionParameter 
            { 
                get { return _ConnectionParameter; }
                set { if (value == _ConnectionParameter) return; _ConnectionParameter = value; OnPropertyChanged();}
            }
            private double _AddressResolutionSupported = 0;
            /// <summary>
            /// From Common Configuration and Central Address Resolution
            ///</summary>
            public double AddressResolutionSupported 
            { 
                get { return _AddressResolutionSupported; }
                set { if (value == _AddressResolutionSupported) return; _AddressResolutionSupported = value; OnPropertyChanged();}
            }
            public Common_Configuration_Data Clone()
            {
                return this.MemberwiseClone() as Common_Configuration_Data;
            }

            public void CopyFrom(Common_Configuration_Data value)
            {
                this.TimestampMostRecent = value.TimestampMostRecent;
                this.Name = value.Name;
                this.Device_Name = value.Device_Name;
                this.Appearance = value.Appearance;
                this.ConnectionParameter = value.ConnectionParameter;
                this.AddressResolutionSupported = value.AddressResolutionSupported;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["Device_Name", "Appearance", "ConnectionParameter", "AddressResolutionSupported"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(Device_Name);
                exporter.CellSet(Appearance);
                exporter.CellSet(ConnectionParameter);
                exporter.CellSet(AddressResolutionSupported);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {Device_Name} {Appearance} {ConnectionParameter} {AddressResolutionSupported}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the Generic Service Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// </summary>
        public class Generic_Service_Data :BTCommonMetaData //, IExportDataSource
        {
            // Template is ServiceDataGroups
            private double _StartRange = 0;
            /// <summary>
            /// From Generic Service and Service Changes
            ///</summary>
            public double StartRange 
            { 
                get { return _StartRange; }
                set { if (value == _StartRange) return; _StartRange = value; OnPropertyChanged();}
            } 
            private double _EndRange = 0;
            /// <summary>
            /// From Generic Service and Service Changes
            ///</summary>
            public double EndRange 
            { 
                get { return _EndRange; }
                set { if (value == _EndRange) return; _EndRange = value; OnPropertyChanged();}
            }
            public Generic_Service_Data Clone()
            {
                return this.MemberwiseClone() as Generic_Service_Data;
            }

            public void CopyFrom(Generic_Service_Data value)
            {
                this.TimestampMostRecent = value.TimestampMostRecent;
                this.Name = value.Name;
                this.StartRange = value.StartRange;
                this.EndRange = value.EndRange;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["StartRange", "EndRange"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(StartRange);
                exporter.CellSet(EndRange);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {StartRange} {EndRange}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the Battery Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// </summary>
        public class Battery_Data :BTCommonMetaData //, IExportDataSource
        {
            // Template is ServiceDataGroups
            private double _BatteryLevel = 0;
            /// <summary>
            /// From Battery and BatteryLevel
            ///</summary>
            public double BatteryLevel 
            { 
                get { return _BatteryLevel; }
                set { if (value == _BatteryLevel) return; _BatteryLevel = value; OnPropertyChanged();}
            }
            public Battery_Data Clone()
            {
                return this.MemberwiseClone() as Battery_Data;
            }

            public void CopyFrom(Battery_Data value)
            {
                this.TimestampMostRecent = value.TimestampMostRecent;
                this.Name = value.Name;
                this.BatteryLevel = value.BatteryLevel;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["BatteryLevel"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(BatteryLevel);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {BatteryLevel}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the Configuration Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// </summary>
        public class Configuration_Data :BTCommonMetaData //, IExportDataSource
        {
            // Template is ServiceDataGroups
            private string _DeviceName = "";
            /// <summary>
            /// From Configuration and Configuration Device Name
            ///</summary>
            public string DeviceName 
            { 
                get { return _DeviceName; }
                set { if (value == _DeviceName) return; _DeviceName = value; OnPropertyChanged();}
            }
            private double _Interval = 0;
            /// <summary>
            /// From Configuration and Advertising Parameter
            ///</summary>
            public double Interval 
            { 
                get { return _Interval; }
                set { if (value == _Interval) return; _Interval = value; OnPropertyChanged();}
            } 
            private double _Timeout = 0;
            /// <summary>
            /// From Configuration and Advertising Parameter
            ///</summary>
            public double Timeout 
            { 
                get { return _Timeout; }
                set { if (value == _Timeout) return; _Timeout = value; OnPropertyChanged();}
            }
            private double _MinInterval = 0;
            /// <summary>
            /// From Configuration and Connection parameters
            ///</summary>
            public double MinInterval 
            { 
                get { return _MinInterval; }
                set { if (value == _MinInterval) return; _MinInterval = value; OnPropertyChanged();}
            } 
            private double _MaxInterval = 0;
            /// <summary>
            /// From Configuration and Connection parameters
            ///</summary>
            public double MaxInterval 
            { 
                get { return _MaxInterval; }
                set { if (value == _MaxInterval) return; _MaxInterval = value; OnPropertyChanged();}
            } 
            private double _Latency = 0;
            /// <summary>
            /// From Configuration and Connection parameters
            ///</summary>
            public double Latency 
            { 
                get { return _Latency; }
                set { if (value == _Latency) return; _Latency = value; OnPropertyChanged();}
            } 
            private double _SupervisionTimeout = 0;
            /// <summary>
            /// From Configuration and Connection parameters
            ///</summary>
            public double SupervisionTimeout 
            { 
                get { return _SupervisionTimeout; }
                set { if (value == _SupervisionTimeout) return; _SupervisionTimeout = value; OnPropertyChanged();}
            }
            private string _Eddystone = "";
            /// <summary>
            /// From Configuration and Eddystone URL
            ///</summary>
            public string Eddystone 
            { 
                get { return _Eddystone; }
                set { if (value == _Eddystone) return; _Eddystone = value; OnPropertyChanged();}
            }
            private byte[] _CloudToken = null;
            /// <summary>
            /// From Configuration and Cloud Token
            ///</summary>
            public byte[] CloudToken 
            { 
                get { return _CloudToken; }
                set { if (value == _CloudToken) return; _CloudToken = value; OnPropertyChanged();}
            }
            private double _Major = 0;
            /// <summary>
            /// From Configuration and Firmware Version
            ///</summary>
            public double Major 
            { 
                get { return _Major; }
                set { if (value == _Major) return; _Major = value; OnPropertyChanged();}
            } 
            private double _Minor = 0;
            /// <summary>
            /// From Configuration and Firmware Version
            ///</summary>
            public double Minor 
            { 
                get { return _Minor; }
                set { if (value == _Minor) return; _Minor = value; OnPropertyChanged();}
            } 
            private double _Patch = 0;
            /// <summary>
            /// From Configuration and Firmware Version
            ///</summary>
            public double Patch 
            { 
                get { return _Patch; }
                set { if (value == _Patch) return; _Patch = value; OnPropertyChanged();}
            }
            private double _param0 = 0;
            /// <summary>
            /// From Configuration and MTU Request
            ///</summary>
            public double param0 
            { 
                get { return _param0; }
                set { if (value == _param0) return; _param0 = value; OnPropertyChanged();}
            } 
            private double _param1 = 0;
            /// <summary>
            /// From Configuration and MTU Request
            ///</summary>
            public double param1 
            { 
                get { return _param1; }
                set { if (value == _param1) return; _param1 = value; OnPropertyChanged();}
            }
            private string _NFCTag = "";
            /// <summary>
            /// From Configuration and NFC Tag
            ///</summary>
            public string NFCTag 
            { 
                get { return _NFCTag; }
                set { if (value == _NFCTag) return; _NFCTag = value; OnPropertyChanged();}
            }
            public Configuration_Data Clone()
            {
                return this.MemberwiseClone() as Configuration_Data;
            }

            public void CopyFrom(Configuration_Data value)
            {
                this.TimestampMostRecent = value.TimestampMostRecent;
                this.Name = value.Name;
                this.DeviceName = value.DeviceName;
                this.Interval = value.Interval;
                this.Timeout = value.Timeout;
                this.MinInterval = value.MinInterval;
                this.MaxInterval = value.MaxInterval;
                this.Latency = value.Latency;
                this.SupervisionTimeout = value.SupervisionTimeout;
                this.Eddystone = value.Eddystone;
                this.CloudToken = value.CloudToken;
                this.Major = value.Major;
                this.Minor = value.Minor;
                this.Patch = value.Patch;
                this.param0 = value.param0;
                this.param1 = value.param1;
                this.NFCTag = value.NFCTag;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["DeviceName", "Interval", "Timeout", "MinInterval", "MaxInterval", "Latency", "SupervisionTimeout", "Eddystone", "CloudToken", "Major", "Minor", "Patch", "param0", "param1", "NFCTag"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(DeviceName);
                exporter.CellSet(Interval);
                exporter.CellSet(Timeout);
                exporter.CellSet(MinInterval);
                exporter.CellSet(MaxInterval);
                exporter.CellSet(Latency);
                exporter.CellSet(SupervisionTimeout);
                exporter.CellSet(Eddystone);
                exporter.CellSet(CloudToken);
                exporter.CellSet(Major);
                exporter.CellSet(Minor);
                exporter.CellSet(Patch);
                exporter.CellSet(param0);
                exporter.CellSet(param1);
                exporter.CellSet(NFCTag);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {DeviceName} {Interval} {Timeout} {MinInterval} {MaxInterval} {Latency} {SupervisionTimeout} {Eddystone} {CloudToken} {Major} {Minor} {Patch} {param0} {param1} {NFCTag}");
            }
        }
//


        #endregion


        /// <summary>
        /// Enumeration of all services
        /// </summary>
        enum ServiceIndex
        {
            Environment_index = 0,
            Common_Configuration_index = 1,
            Generic_Service_index = 2,
            Battery_index = 3,
            Configuration_index = 4,
        }

        /// <summary>
        /// Enumeration of all characteristics in all of the services.
        /// </summary>
        enum CharacteristicIndex
        {
            Environment_Temperature_c_index = 0,     // GUID EF680201-9B35-4933-9B10-52FFA9740042
            Environment_Pressure_hpa_index = 1,     // GUID EF680202-9B35-4933-9B10-52FFA9740042
            Environment_Humidity_index = 2,     // GUID EF680203-9B35-4933-9B10-52FFA9740042
            Environment_Air_Quality_eCOS_TVOC_index = 3,     // GUID EF680204-9B35-4933-9B10-52FFA9740042
            Environment_Color_RGB_Clear_index = 4,     // GUID EF680205-9B35-4933-9B10-52FFA9740042
            Environment_Environment_Configuration_index = 5,     // GUID EF680206-9B35-4933-9B10-52FFA9740042
            Common_Configuration_Device_Name_index = 6,     // GUID 00002a00-0000-1000-8000-00805f9b34fb
            Common_Configuration_Appearance_index = 7,     // GUID 00002a01-0000-1000-8000-00805f9b34fb
            Common_Configuration_Connection_Parameter_index = 8,     // GUID 00002a04-0000-1000-8000-00805f9b34fb
            Common_Configuration_Central_Address_Resolution_index = 9,     // GUID 00002aa6-0000-1000-8000-00805f9b34fb
            Generic_Service_Service_Changes_index = 10,     // GUID 00002a05-0000-1000-8000-00805f9b34fb
            Battery_BatteryLevel_index = 11,     // GUID 00002a19-0000-1000-8000-00805f9b34fb
            Configuration_Configuration_Device_Name_index = 12,     // GUID EF680101-9B35-4933-9B10-52FFA9740042
            Configuration_Advertising_Parameter_index = 13,     // GUID EF680102-9B35-4933-9B10-52FFA9740042
            Configuration_Connection_parameters_index = 14,     // GUID EF680104-9B35-4933-9B10-52FFA9740042
            Configuration_Eddystone_URL_index = 15,     // GUID EF680105-9B35-4933-9B10-52FFA9740042
            Configuration_Cloud_Token_index = 16,     // GUID EF680106-9B35-4933-9B10-52FFA9740042
            Configuration_Firmware_Version_index = 17,     // GUID EF680107-9B35-4933-9B10-52FFA9740042
            Configuration_MTU_Request_index = 18,     // GUID EF680108-9B35-4933-9B10-52FFA9740042
            Configuration_NFC_Tag_index = 19,     // GUID EF680109-9B35-4933-9B10-52FFA9740042
        }

        /// <summary>
        /// List of the guids supported by the device. 
        /// </summary>
        List<Guid> Service_Guids = new List<Guid>()
        {
            Guid.Parse("EF680200-9B35-4933-9B10-52FFA9740042"), // #0 is Environment
            Guid.Parse("00001800-0000-1000-8000-00805f9b34fb"), // #1 is Common Configuration
            Guid.Parse("00001801-0000-1000-8000-00805f9b34fb"), // #2 is Generic Service
            Guid.Parse("0000180f-0000-1000-8000-00805f9b34fb"), // #3 is Battery
            Guid.Parse("EF680100-9B35-4933-9B10-52FFA9740042"), // #4 is Configuration
        };

        /// <summary>
        /// Active list of services. Will be filled in as the services are connected. Starts off as null.
        /// </summary>
        List<GattDeviceService> Services = new List<GattDeviceService>() { null, null, null, null, null, };

        /// <summary>
        /// List of the Characteristic GUIDS for all of the characteristics for all of the services.
        /// Is indexed by the CharacteristicIndex enum. 
        /// </summary>
        List<Guid> Characteristic_Guids = new List<Guid>()
        {
            Guid.Parse("EF680201-9B35-4933-9B10-52FFA9740042"), // #0 is Environment Temperature (c)
            Guid.Parse("EF680202-9B35-4933-9B10-52FFA9740042"), // #1 is Environment Pressure (hpa)
            Guid.Parse("EF680203-9B35-4933-9B10-52FFA9740042"), // #2 is Environment Humidity (%)
            Guid.Parse("EF680204-9B35-4933-9B10-52FFA9740042"), // #3 is Environment Air Quality eCOS TVOC
            Guid.Parse("EF680205-9B35-4933-9B10-52FFA9740042"), // #4 is Environment Color RGB+Clear
            Guid.Parse("EF680206-9B35-4933-9B10-52FFA9740042"), // #5 is Environment Environment Configuration
            Guid.Parse("00002a00-0000-1000-8000-00805f9b34fb"), // #6 is Common Configuration Device Name
            Guid.Parse("00002a01-0000-1000-8000-00805f9b34fb"), // #7 is Common Configuration Appearance
            Guid.Parse("00002a04-0000-1000-8000-00805f9b34fb"), // #8 is Common Configuration Connection Parameter
            Guid.Parse("00002aa6-0000-1000-8000-00805f9b34fb"), // #9 is Common Configuration Central Address Resolution
            Guid.Parse("00002a05-0000-1000-8000-00805f9b34fb"), // #10 is Generic Service Service Changes
            Guid.Parse("00002a19-0000-1000-8000-00805f9b34fb"), // #11 is Battery BatteryLevel
            Guid.Parse("EF680101-9B35-4933-9B10-52FFA9740042"), // #12 is Configuration Configuration Device Name
            Guid.Parse("EF680102-9B35-4933-9B10-52FFA9740042"), // #13 is Configuration Advertising Parameter
            Guid.Parse("EF680104-9B35-4933-9B10-52FFA9740042"), // #14 is Configuration Connection parameters
            Guid.Parse("EF680105-9B35-4933-9B10-52FFA9740042"), // #15 is Configuration Eddystone URL
            Guid.Parse("EF680106-9B35-4933-9B10-52FFA9740042"), // #16 is Configuration Cloud Token
            Guid.Parse("EF680107-9B35-4933-9B10-52FFA9740042"), // #17 is Configuration Firmware Version
            Guid.Parse("EF680108-9B35-4933-9B10-52FFA9740042"), // #18 is Configuration MTU Request
            Guid.Parse("EF680109-9B35-4933-9B10-52FFA9740042"), // #19 is Configuration NFC Tag
        };

        List<GattCharacteristic> Characteristics = new List<GattCharacteristic>() { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,  };
        private List<bool> NotifyCharacteristic_ValueChanged_set = new List<bool> { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,  };
        private List<IotNumberFormats.ValueParser> ValueParsers = new List<IotNumberFormats.ValueParser>() {  null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,  };


        /// <summary>
        /// Delegate for all Notify events. this is specific to this device (the indexes are all for this device only)
        /// but otherwise is generic.
        /// </summary>
        /// <param name="data"></param>
        public delegate void BluetoothDataEvent(IotNumberFormats.ValueParserResult data);

        private async Task<bool> Ensure_Characteristic_Async(ServiceIndex serviceIndex, string serviceName, CharacteristicIndex characteristicIndex, string characteristicName)
        {
            if (Characteristics[(int)characteristicIndex] == null)
            {
                if (ble == null) return false;
                if (Services[(int)serviceIndex] == null)
                {
                    var serviceStatus = await ble.GetGattServicesForUuidAsync(Service_Guids[(int)serviceIndex]);
                    if (serviceStatus.Status != GattCommunicationStatus.Success)
                    {
                        Status.ReportStatus($"Unable to get service {serviceName}", serviceStatus);
                        return false;
                    }
                    if (serviceStatus.Services.Count != 1)
                    {
                        Status.ReportStatus($"Unable to get valid service count ({serviceStatus.Services.Count}) for {serviceName}", serviceStatus);
                        return false;
                    }
                    Services[(int)serviceIndex] = serviceStatus.Services[0];
                }
                var service = Services[(int)serviceIndex];
                var characteristicsStatus = await service.GetCharacteristicsForUuidAsync(Characteristic_Guids[(int)characteristicIndex]);
                if (characteristicsStatus.Status != GattCommunicationStatus.Success)
                {
                    Status.ReportStatus($"unable to get characteristic for {characteristicName}", characteristicsStatus);
                    return false;
                }
                if (characteristicsStatus.Characteristics.Count == 0)
                {
                    Status.ReportStatus($"unable to get any characteristics for {characteristicName}", characteristicsStatus);
                    return false;
                }
                else if (characteristicsStatus.Characteristics.Count != 1)
                {
                    Status.ReportStatus($"unable to get correct characteristics count ({characteristicsStatus.Characteristics.Count}) for {characteristicName}", characteristicsStatus);
                    return false;
                }
                Characteristics[(int)characteristicIndex] = characteristicsStatus.Characteristics[0];
            }
            return true;
        }


        /// <summary>
        /// Generic read method; takes in a cache mode which defaults to uncached.
        /// Calls ReportStatus on either sucess or failure
        /// </summary>
        /// <param name="characteristicIndex">Index number of the characteristic</param>
        /// <param name="method" >Name of the actual method; is just used for logging</param>
        /// <param name="cacheMode" >Type of caching</param>
        /// <returns></returns>
        private async Task<IBuffer> ReadAsync(GattCharacteristic ch, string method, BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            GattReadResult readResult;
            IBuffer buffer = null;
            try
            {
                readResult = await ch.ReadValueAsync(cacheMode);
                if (readResult.Status == GattCommunicationStatus.Success)
                {
                    buffer = readResult.Value;
                }
                else
                {
                    // NOTE: reset the characteristics array?
                }
                Status.ReportStatus(method, readResult.Status);
            }
            catch (Exception)
            {
                Status.ReportStatus(method, GattCommunicationStatus.Unreachable);
                // NOTE: reset the characteristics array?
            }
            return buffer;
        }


        private async Task<bool> SetupNotifyAsync(string name, 
            ServiceIndex serviceIndex, string serviceName, CharacteristicIndex index, 
            Windows.Foundation.TypedEventHandler<GattCharacteristic, GattValueChangedEventArgs> callback,
            GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            await Ensure_Characteristic_Async(serviceIndex, serviceName, index, name);
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return false;
            }
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyCharacteristic_ValueChanged_set[(int)index])
                {
                    // Only set the event callback once
                    NotifyCharacteristic_ValueChanged_set[(int)index] = true;
                    ch.ValueChanged += callback;
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"Notify{name}: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"Notify{name}: set notification", result);

            return true;
        }

        //
        //
        // Start of the service + characteristic
        //
        //


        //
        // All services / characteristics methods. 
        //


        #region Service_Environment
        // Service Environment 

        public Environment_Data CurrEnvironment_Data { get; set; } = new Environment_Data();

        // Per-characteristics methods for Environment Temperature_c
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyTemperature_cAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Temperature_c", ServiceIndex.Environment_index, "Environment", CharacteristicIndex.Environment_Temperature_c_index, NotifyTemperature_cCallback, notifyType);
            return retval;
        }

        private void NotifyTemperature_cCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Environment_Temperature_c_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("/I8/P8|FIXED|Temperature|C");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrEnvironment_Data.TimestampMostRecent = args.Timestamp;
            CurrEnvironment_Data.Temperature = vr.GetNextDouble();
            OnPropertyChanged(Temperature_cPropertyChangedName); // "Temperature_c"
        }
        // Per-characteristics methods for Environment Pressure_hpa
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyPressure_hpaAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Pressure_hpa", ServiceIndex.Environment_index, "Environment", CharacteristicIndex.Environment_Pressure_hpa_index, NotifyPressure_hpaCallback, notifyType);
            return retval;
        }

        private void NotifyPressure_hpaCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Environment_Pressure_hpa_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("/I32/P8|FIXED|Pressure|hPA");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrEnvironment_Data.TimestampMostRecent = args.Timestamp;
            CurrEnvironment_Data.Pressure = vr.GetNextDouble();
            OnPropertyChanged(Pressure_hpaPropertyChangedName); // "Pressure_hpa"
        }
        // Per-characteristics methods for Environment Humidity
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyHumidityAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Humidity", ServiceIndex.Environment_index, "Environment", CharacteristicIndex.Environment_Humidity_index, NotifyHumidityCallback, notifyType);
            return retval;
        }

        private void NotifyHumidityCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Environment_Humidity_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|DEC|Humidity|%");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrEnvironment_Data.TimestampMostRecent = args.Timestamp;
            CurrEnvironment_Data.Humidity = vr.GetNextDouble();
            OnPropertyChanged(HumidityPropertyChangedName); // "Humidity"
        }
        // Per-characteristics methods for Environment Air_Quality_eCOS_TVOC
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyAir_Quality_eCOS_TVOCAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Air_Quality_eCOS_TVOC", ServiceIndex.Environment_index, "Environment", CharacteristicIndex.Environment_Air_Quality_eCOS_TVOC_index, NotifyAir_Quality_eCOS_TVOCCallback, notifyType);
            return retval;
        }

        private void NotifyAir_Quality_eCOS_TVOCCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Environment_Air_Quality_eCOS_TVOC_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U16|DEC|eCOS|ppm|390^0 U16|DEC|TVOC|ppb");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrEnvironment_Data.TimestampMostRecent = args.Timestamp;
            CurrEnvironment_Data.eCOS = vr.GetNextDouble();
            CurrEnvironment_Data.TVOC = vr.GetNextDouble();            if (CurrEnvironment_Data.eCOS == 0)
            {
                CurrEnvironment_Data.eCOS = 390;
            }
            OnPropertyChanged(Air_Quality_eCOS_TVOCPropertyChangedName); // "Air_Quality_eCOS_TVOC"
        }
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Environment_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Environment_Data> ReadTemperature_c(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Environment_Temperature_c_index;
            await Ensure_Characteristic_Async(ServiceIndex.Environment_index, "Environment", index, "Temperature (c)");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Temperature (c)", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("/I8/P8|FIXED|Temperature|C");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrEnvironment_Data.Temperature = vr.GetNextDouble();
            OnPropertyChanged(Temperature_cPropertyChangedName); // "Temperature_c"
            return CurrEnvironment_Data;
        }
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Environment_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Environment_Data> ReadPressure_hpa(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Environment_Pressure_hpa_index;
            await Ensure_Characteristic_Async(ServiceIndex.Environment_index, "Environment", index, "Pressure (hpa)");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Pressure (hpa)", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("/I32/P8|FIXED|Pressure|hPA");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrEnvironment_Data.Pressure = vr.GetNextDouble();
            OnPropertyChanged(Pressure_hpaPropertyChangedName); // "Pressure_hpa"
            return CurrEnvironment_Data;
        }
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Environment_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Environment_Data> ReadHumidity(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Environment_Humidity_index;
            await Ensure_Characteristic_Async(ServiceIndex.Environment_index, "Environment", index, "Humidity (%)");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Humidity (%)", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|DEC|Humidity|%");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrEnvironment_Data.Humidity = vr.GetNextDouble();
            OnPropertyChanged(HumidityPropertyChangedName); // "Humidity"
            return CurrEnvironment_Data;
        }
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Environment_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Environment_Data> ReadAir_Quality_eCOS_TVOC(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Environment_Air_Quality_eCOS_TVOC_index;
            await Ensure_Characteristic_Async(ServiceIndex.Environment_index, "Environment", index, "Air Quality eCOS TVOC");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Air Quality eCOS TVOC", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U16|DEC|eCOS|ppm|390^0 U16|DEC|TVOC|ppb");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrEnvironment_Data.eCOS = vr.GetNextDouble();
            CurrEnvironment_Data.TVOC = vr.GetNextDouble();            if (CurrEnvironment_Data.eCOS == 0)
            {
                CurrEnvironment_Data.eCOS = 390;
            }
            OnPropertyChanged(Air_Quality_eCOS_TVOCPropertyChangedName); // "Air_Quality_eCOS_TVOC"
            return CurrEnvironment_Data;
        }
        // Service Environment 

        public EnvironmentColor_Data CurrEnvironmentColor_Data { get; set; } = new EnvironmentColor_Data();

        // Per-characteristics methods for Environment Color_RGB_Clear
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyColor_RGB_ClearAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Color_RGB_Clear", ServiceIndex.Environment_index, "Environment", CharacteristicIndex.Environment_Color_RGB_Clear_index, NotifyColor_RGB_ClearCallback, notifyType);
            return retval;
        }

        private void NotifyColor_RGB_ClearCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Environment_Color_RGB_Clear_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U16|DEC|Red U16|DEC|Green U16|DEC|Blue U16|DEC|Clear");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrEnvironmentColor_Data.TimestampMostRecent = args.Timestamp;
            CurrEnvironmentColor_Data.Red = vr.GetNextDouble();
            CurrEnvironmentColor_Data.Green = vr.GetNextDouble();
            CurrEnvironmentColor_Data.Blue = vr.GetNextDouble();
            CurrEnvironmentColor_Data.Clear = vr.GetNextDouble();
            OnPropertyChanged(Color_RGB_ClearPropertyChangedName); // "Color_RGB_Clear"
        }
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>EnvironmentColor_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<EnvironmentColor_Data> ReadColor_RGB_Clear(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Environment_Color_RGB_Clear_index;
            await Ensure_Characteristic_Async(ServiceIndex.Environment_index, "Environment", index, "Color RGB+Clear");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Color RGB+Clear", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U16|DEC|Red U16|DEC|Green U16|DEC|Blue U16|DEC|Clear");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrEnvironmentColor_Data.Red = vr.GetNextDouble();
            CurrEnvironmentColor_Data.Green = vr.GetNextDouble();
            CurrEnvironmentColor_Data.Blue = vr.GetNextDouble();
            CurrEnvironmentColor_Data.Clear = vr.GetNextDouble();
            OnPropertyChanged(Color_RGB_ClearPropertyChangedName); // "Color_RGB_Clear"
            return CurrEnvironmentColor_Data;
        }
        // Service Environment 

        public EnvironmentConfiguration_Data CurrEnvironmentConfiguration_Data { get; set; } = new EnvironmentConfiguration_Data();

        // Per-characteristics methods for Environment Environment_Configuration
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyEnvironment_ConfigurationAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Environment_Configuration", ServiceIndex.Environment_index, "Environment", CharacteristicIndex.Environment_Environment_Configuration_index, NotifyEnvironment_ConfigurationCallback, notifyType);
            return retval;
        }

        private void NotifyEnvironment_ConfigurationCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Environment_Environment_Configuration_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U16|DEC|TempInterval|ms U16|DEC|PressureInterval|ms U16|DEC|HumidityInterval|ms U16|DEC|ColorInterval|ms U8|DEC|GasMode U8|DEC|RedCalibration U8|DEC|GreenCalibration U8|DEC|BlueCalibration");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrEnvironmentConfiguration_Data.TimestampMostRecent = args.Timestamp;
            CurrEnvironmentConfiguration_Data.TempInterval = vr.GetNextDouble();
            CurrEnvironmentConfiguration_Data.PressureInterval = vr.GetNextDouble();
            CurrEnvironmentConfiguration_Data.HumidityInterval = vr.GetNextDouble();
            CurrEnvironmentConfiguration_Data.ColorInterval = vr.GetNextDouble();
            CurrEnvironmentConfiguration_Data.GasMode = vr.GetNextDouble();
            CurrEnvironmentConfiguration_Data.RedCalibration = vr.GetNextDouble();
            CurrEnvironmentConfiguration_Data.GreenCalibration = vr.GetNextDouble();
            CurrEnvironmentConfiguration_Data.BlueCalibration = vr.GetNextDouble();
            OnPropertyChanged(Environment_ConfigurationPropertyChangedName); // "Environment_Configuration"
        }
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>EnvironmentConfiguration_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<EnvironmentConfiguration_Data> ReadEnvironment_Configuration(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Environment_Environment_Configuration_index;
            await Ensure_Characteristic_Async(ServiceIndex.Environment_index, "Environment", index, "Environment Configuration");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Environment Configuration", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U16|DEC|TempInterval|ms U16|DEC|PressureInterval|ms U16|DEC|HumidityInterval|ms U16|DEC|ColorInterval|ms U8|DEC|GasMode U8|DEC|RedCalibration U8|DEC|GreenCalibration U8|DEC|BlueCalibration");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrEnvironmentConfiguration_Data.TempInterval = vr.GetNextDouble();
            CurrEnvironmentConfiguration_Data.PressureInterval = vr.GetNextDouble();
            CurrEnvironmentConfiguration_Data.HumidityInterval = vr.GetNextDouble();
            CurrEnvironmentConfiguration_Data.ColorInterval = vr.GetNextDouble();
            CurrEnvironmentConfiguration_Data.GasMode = vr.GetNextDouble();
            CurrEnvironmentConfiguration_Data.RedCalibration = vr.GetNextDouble();
            CurrEnvironmentConfiguration_Data.GreenCalibration = vr.GetNextDouble();
            CurrEnvironmentConfiguration_Data.BlueCalibration = vr.GetNextDouble();
            OnPropertyChanged(Environment_ConfigurationPropertyChangedName); // "Environment_Configuration"
            return CurrEnvironmentConfiguration_Data;
        }

        #endregion
//
        #region Service_Common_Configuration
        // Service Common Configuration 

        public Common_Configuration_Data CurrCommon_Configuration_Data { get; set; } = new Common_Configuration_Data();

        // Per-characteristics methods for Common_Configuration Device_Name
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyDevice_NameAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Device_Name", ServiceIndex.Common_Configuration_index, "Common Configuration", CharacteristicIndex.Common_Configuration_Device_Name_index, NotifyDevice_NameCallback, notifyType);
            return retval;
        }

        private void NotifyDevice_NameCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Common_Configuration_Device_Name_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|ASCII|Device_Name");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrCommon_Configuration_Data.TimestampMostRecent = args.Timestamp;
            CurrCommon_Configuration_Data.Device_Name = vr.GetNextString();
            OnPropertyChanged(Device_NamePropertyChangedName); // "Device_Name"
        }
        // Per-characteristics methods for Common_Configuration Appearance
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyAppearanceAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Appearance", ServiceIndex.Common_Configuration_index, "Common Configuration", CharacteristicIndex.Common_Configuration_Appearance_index, NotifyAppearanceCallback, notifyType);
            return retval;
        }

        private void NotifyAppearanceCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Common_Configuration_Appearance_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U16|Speciality^Appearance|Appearance");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrCommon_Configuration_Data.TimestampMostRecent = args.Timestamp;
            CurrCommon_Configuration_Data.Appearance = vr.GetNextDouble();
            OnPropertyChanged(AppearancePropertyChangedName); // "Appearance"
        }
        // Per-characteristics methods for Common_Configuration Connection_Parameter
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyConnection_ParameterAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Connection_Parameter", ServiceIndex.Common_Configuration_index, "Common Configuration", CharacteristicIndex.Common_Configuration_Connection_Parameter_index, NotifyConnection_ParameterCallback, notifyType);
            return retval;
        }

        private void NotifyConnection_ParameterCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Common_Configuration_Connection_Parameter_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|ConnectionParameter");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrCommon_Configuration_Data.TimestampMostRecent = args.Timestamp;
            CurrCommon_Configuration_Data.ConnectionParameter = vr.GetNextByteArray();
            OnPropertyChanged(Connection_ParameterPropertyChangedName); // "Connection_Parameter"
        }
        // Per-characteristics methods for Common_Configuration Central_Address_Resolution
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyCentral_Address_ResolutionAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Central_Address_Resolution", ServiceIndex.Common_Configuration_index, "Common Configuration", CharacteristicIndex.Common_Configuration_Central_Address_Resolution_index, NotifyCentral_Address_ResolutionCallback, notifyType);
            return retval;
        }

        private void NotifyCentral_Address_ResolutionCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Common_Configuration_Central_Address_Resolution_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|DEC|AddressResolutionSupported");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrCommon_Configuration_Data.TimestampMostRecent = args.Timestamp;
            CurrCommon_Configuration_Data.AddressResolutionSupported = vr.GetNextDouble();
            OnPropertyChanged(Central_Address_ResolutionPropertyChangedName); // "Central_Address_Resolution"
        }
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Common_Configuration_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Common_Configuration_Data> ReadDevice_Name(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Common_Configuration_Device_Name_index;
            await Ensure_Characteristic_Async(ServiceIndex.Common_Configuration_index, "Common Configuration", index, "Device Name");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Device Name", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("STRING|ASCII|Device_Name");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrCommon_Configuration_Data.Device_Name = vr.GetNextString();
            OnPropertyChanged(Device_NamePropertyChangedName); // "Device_Name"
            return CurrCommon_Configuration_Data;
        }
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Common_Configuration_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Common_Configuration_Data> ReadAppearance(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Common_Configuration_Appearance_index;
            await Ensure_Characteristic_Async(ServiceIndex.Common_Configuration_index, "Common Configuration", index, "Appearance");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Appearance", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U16|Speciality^Appearance|Appearance");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrCommon_Configuration_Data.Appearance = vr.GetNextDouble();
            OnPropertyChanged(AppearancePropertyChangedName); // "Appearance"
            return CurrCommon_Configuration_Data;
        }
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Common_Configuration_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Common_Configuration_Data> ReadConnection_Parameter(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Common_Configuration_Connection_Parameter_index;
            await Ensure_Characteristic_Async(ServiceIndex.Common_Configuration_index, "Common Configuration", index, "Connection Parameter");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Connection Parameter", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|ConnectionParameter");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrCommon_Configuration_Data.ConnectionParameter = vr.GetNextByteArray();
            OnPropertyChanged(Connection_ParameterPropertyChangedName); // "Connection_Parameter"
            return CurrCommon_Configuration_Data;
        }
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Common_Configuration_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Common_Configuration_Data> ReadCentral_Address_Resolution(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Common_Configuration_Central_Address_Resolution_index;
            await Ensure_Characteristic_Async(ServiceIndex.Common_Configuration_index, "Common Configuration", index, "Central Address Resolution");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Central Address Resolution", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|DEC|AddressResolutionSupported");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrCommon_Configuration_Data.AddressResolutionSupported = vr.GetNextDouble();
            OnPropertyChanged(Central_Address_ResolutionPropertyChangedName); // "Central_Address_Resolution"
            return CurrCommon_Configuration_Data;
        }

        #endregion
//
        #region Service_Generic_Service
        // Service Generic Service 

        public Generic_Service_Data CurrGeneric_Service_Data { get; set; } = new Generic_Service_Data();

        // Per-characteristics methods for Generic_Service Service_Changes
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyService_ChangesAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Service_Changes", ServiceIndex.Generic_Service_index, "Generic Service", CharacteristicIndex.Generic_Service_Service_Changes_index, NotifyService_ChangesCallback, notifyType);
            return retval;
        }

        private void NotifyService_ChangesCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Generic_Service_Service_Changes_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U16|DEC|StartRange U16|DEC|EndRange");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrGeneric_Service_Data.TimestampMostRecent = args.Timestamp;
            CurrGeneric_Service_Data.StartRange = vr.GetNextDouble();
            CurrGeneric_Service_Data.EndRange = vr.GetNextDouble();
            OnPropertyChanged(Service_ChangesPropertyChangedName); // "Service_Changes"
        }
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Generic_Service_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Generic_Service_Data> ReadService_Changes(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Generic_Service_Service_Changes_index;
            await Ensure_Characteristic_Async(ServiceIndex.Generic_Service_index, "Generic Service", index, "Service Changes");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Service Changes", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U16|DEC|StartRange U16|DEC|EndRange");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrGeneric_Service_Data.StartRange = vr.GetNextDouble();
            CurrGeneric_Service_Data.EndRange = vr.GetNextDouble();
            OnPropertyChanged(Service_ChangesPropertyChangedName); // "Service_Changes"
            return CurrGeneric_Service_Data;
        }

        #endregion
//
        #region Service_Battery
        // Service Battery 

        public Battery_Data CurrBattery_Data { get; set; } = new Battery_Data();

        // Per-characteristics methods for Battery BatteryLevel
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyBatteryLevelAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("BatteryLevel", ServiceIndex.Battery_index, "Battery", CharacteristicIndex.Battery_BatteryLevel_index, NotifyBatteryLevelCallback, notifyType);
            return retval;
        }

        private void NotifyBatteryLevelCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Battery_BatteryLevel_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("I8|DEC|BatteryLevel|%");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrBattery_Data.TimestampMostRecent = args.Timestamp;
            CurrBattery_Data.BatteryLevel = vr.GetNextDouble();
            OnPropertyChanged(BatteryLevelPropertyChangedName); // "BatteryLevel"
        }
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Battery_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Battery_Data> ReadBatteryLevel(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Battery_BatteryLevel_index;
            await Ensure_Characteristic_Async(ServiceIndex.Battery_index, "Battery", index, "BatteryLevel");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "BatteryLevel", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("I8|DEC|BatteryLevel|%");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrBattery_Data.BatteryLevel = vr.GetNextDouble();
            OnPropertyChanged(BatteryLevelPropertyChangedName); // "BatteryLevel"
            return CurrBattery_Data;
        }

        #endregion
//
        #region Service_Configuration
        // Service Configuration 

        public Configuration_Data CurrConfiguration_Data { get; set; } = new Configuration_Data();

        // Per-characteristics methods for Configuration Configuration_Device_Name
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyConfiguration_Device_NameAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Configuration_Device_Name", ServiceIndex.Configuration_index, "Configuration", CharacteristicIndex.Configuration_Configuration_Device_Name_index, NotifyConfiguration_Device_NameCallback, notifyType);
            return retval;
        }

        private void NotifyConfiguration_Device_NameCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Configuration_Configuration_Device_Name_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|ASCII|DeviceName");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrConfiguration_Data.TimestampMostRecent = args.Timestamp;
            CurrConfiguration_Data.DeviceName = vr.GetNextString();
            OnPropertyChanged(Configuration_Device_NamePropertyChangedName); // "Configuration_Device_Name"
        }
        // Per-characteristics methods for Configuration Advertising_Parameter
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyAdvertising_ParameterAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Advertising_Parameter", ServiceIndex.Configuration_index, "Configuration", CharacteristicIndex.Configuration_Advertising_Parameter_index, NotifyAdvertising_ParameterCallback, notifyType);
            return retval;
        }

        private void NotifyAdvertising_ParameterCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Configuration_Advertising_Parameter_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U16|DEC|Interval|ms U8|DEC|Timeout|s");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrConfiguration_Data.TimestampMostRecent = args.Timestamp;
            CurrConfiguration_Data.Interval = vr.GetNextDouble();
            CurrConfiguration_Data.Timeout = vr.GetNextDouble();
            OnPropertyChanged(Advertising_ParameterPropertyChangedName); // "Advertising_Parameter"
        }
        // Per-characteristics methods for Configuration Connection_parameters
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyConnection_parametersAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Connection_parameters", ServiceIndex.Configuration_index, "Configuration", CharacteristicIndex.Configuration_Connection_parameters_index, NotifyConnection_parametersCallback, notifyType);
            return retval;
        }

        private void NotifyConnection_parametersCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Configuration_Connection_parameters_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U16|DEC|MinInterval U16|DEC|MaxInterval U16|DEC|Latency U16|DEC|SupervisionTimeout|10ms");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrConfiguration_Data.TimestampMostRecent = args.Timestamp;
            CurrConfiguration_Data.MinInterval = vr.GetNextDouble();
            CurrConfiguration_Data.MaxInterval = vr.GetNextDouble();
            CurrConfiguration_Data.Latency = vr.GetNextDouble();
            CurrConfiguration_Data.SupervisionTimeout = vr.GetNextDouble();
            OnPropertyChanged(Connection_parametersPropertyChangedName); // "Connection_parameters"
        }
        // Per-characteristics methods for Configuration Eddystone_URL
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyEddystone_URLAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Eddystone_URL", ServiceIndex.Configuration_index, "Configuration", CharacteristicIndex.Configuration_Eddystone_URL_index, NotifyEddystone_URLCallback, notifyType);
            return retval;
        }

        private void NotifyEddystone_URLCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Configuration_Eddystone_URL_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|Eddystone|Eddystone");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrConfiguration_Data.TimestampMostRecent = args.Timestamp;
            CurrConfiguration_Data.Eddystone = vr.GetNextString();
            OnPropertyChanged(Eddystone_URLPropertyChangedName); // "Eddystone_URL"
        }
        // Per-characteristics methods for Configuration Cloud_Token
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyCloud_TokenAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Cloud_Token", ServiceIndex.Configuration_index, "Configuration", CharacteristicIndex.Configuration_Cloud_Token_index, NotifyCloud_TokenCallback, notifyType);
            return retval;
        }

        private void NotifyCloud_TokenCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Configuration_Cloud_Token_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|CloudToken");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrConfiguration_Data.TimestampMostRecent = args.Timestamp;
            CurrConfiguration_Data.CloudToken = vr.GetNextByteArray();
            OnPropertyChanged(Cloud_TokenPropertyChangedName); // "Cloud_Token"
        }
        // Per-characteristics methods for Configuration Firmware_Version
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyFirmware_VersionAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Firmware_Version", ServiceIndex.Configuration_index, "Configuration", CharacteristicIndex.Configuration_Firmware_Version_index, NotifyFirmware_VersionCallback, notifyType);
            return retval;
        }

        private void NotifyFirmware_VersionCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Configuration_Firmware_Version_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|DEC|Major U8|DEC|Minor U8|DEC|Patch");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrConfiguration_Data.TimestampMostRecent = args.Timestamp;
            CurrConfiguration_Data.Major = vr.GetNextDouble();
            CurrConfiguration_Data.Minor = vr.GetNextDouble();
            CurrConfiguration_Data.Patch = vr.GetNextDouble();
            OnPropertyChanged(Firmware_VersionPropertyChangedName); // "Firmware_Version"
        }
        // Per-characteristics methods for Configuration MTU_Request
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyMTU_RequestAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("MTU_Request", ServiceIndex.Configuration_index, "Configuration", CharacteristicIndex.Configuration_MTU_Request_index, NotifyMTU_RequestCallback, notifyType);
            return retval;
        }

        private void NotifyMTU_RequestCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Configuration_MTU_Request_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8 U16");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrConfiguration_Data.TimestampMostRecent = args.Timestamp;
            CurrConfiguration_Data.param0 = vr.GetNextDouble();
            CurrConfiguration_Data.param1 = vr.GetNextDouble();
            OnPropertyChanged(MTU_RequestPropertyChangedName); // "MTU_Request"
        }
        // Per-characteristics methods for Configuration NFC_Tag
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyNFC_TagAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("NFC_Tag", ServiceIndex.Configuration_index, "Configuration", CharacteristicIndex.Configuration_NFC_Tag_index, NotifyNFC_TagCallback, notifyType);
            return retval;
        }

        private void NotifyNFC_TagCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Configuration_NFC_Tag_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|ASCII|NFCTag");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrConfiguration_Data.TimestampMostRecent = args.Timestamp;
            CurrConfiguration_Data.NFCTag = vr.GetNextString();
            OnPropertyChanged(NFC_TagPropertyChangedName); // "NFC_Tag"
        }
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Configuration_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Configuration_Data> ReadConfiguration_Device_Name(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Configuration_Configuration_Device_Name_index;
            await Ensure_Characteristic_Async(ServiceIndex.Configuration_index, "Configuration", index, "Configuration Device Name");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Configuration Device Name", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("STRING|ASCII|DeviceName");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrConfiguration_Data.DeviceName = vr.GetNextString();
            OnPropertyChanged(Configuration_Device_NamePropertyChangedName); // "Configuration_Device_Name"
            return CurrConfiguration_Data;
        }
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Configuration_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Configuration_Data> ReadAdvertising_Parameter(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Configuration_Advertising_Parameter_index;
            await Ensure_Characteristic_Async(ServiceIndex.Configuration_index, "Configuration", index, "Advertising Parameter");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Advertising Parameter", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U16|DEC|Interval|ms U8|DEC|Timeout|s");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrConfiguration_Data.Interval = vr.GetNextDouble();
            CurrConfiguration_Data.Timeout = vr.GetNextDouble();
            OnPropertyChanged(Advertising_ParameterPropertyChangedName); // "Advertising_Parameter"
            return CurrConfiguration_Data;
        }
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Configuration_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Configuration_Data> ReadConnection_parameters(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Configuration_Connection_parameters_index;
            await Ensure_Characteristic_Async(ServiceIndex.Configuration_index, "Configuration", index, "Connection parameters");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Connection parameters", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U16|DEC|MinInterval U16|DEC|MaxInterval U16|DEC|Latency U16|DEC|SupervisionTimeout|10ms");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrConfiguration_Data.MinInterval = vr.GetNextDouble();
            CurrConfiguration_Data.MaxInterval = vr.GetNextDouble();
            CurrConfiguration_Data.Latency = vr.GetNextDouble();
            CurrConfiguration_Data.SupervisionTimeout = vr.GetNextDouble();
            OnPropertyChanged(Connection_parametersPropertyChangedName); // "Connection_parameters"
            return CurrConfiguration_Data;
        }
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Configuration_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Configuration_Data> ReadEddystone_URL(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Configuration_Eddystone_URL_index;
            await Ensure_Characteristic_Async(ServiceIndex.Configuration_index, "Configuration", index, "Eddystone URL");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Eddystone URL", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("STRING|Eddystone|Eddystone");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrConfiguration_Data.Eddystone = vr.GetNextString();
            OnPropertyChanged(Eddystone_URLPropertyChangedName); // "Eddystone_URL"
            return CurrConfiguration_Data;
        }
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Configuration_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Configuration_Data> ReadCloud_Token(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Configuration_Cloud_Token_index;
            await Ensure_Characteristic_Async(ServiceIndex.Configuration_index, "Configuration", index, "Cloud Token");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Cloud Token", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|CloudToken");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrConfiguration_Data.CloudToken = vr.GetNextByteArray();
            OnPropertyChanged(Cloud_TokenPropertyChangedName); // "Cloud_Token"
            return CurrConfiguration_Data;
        }
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Configuration_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Configuration_Data> ReadFirmware_Version(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Configuration_Firmware_Version_index;
            await Ensure_Characteristic_Async(ServiceIndex.Configuration_index, "Configuration", index, "Firmware Version");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Firmware Version", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|DEC|Major U8|DEC|Minor U8|DEC|Patch");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrConfiguration_Data.Major = vr.GetNextDouble();
            CurrConfiguration_Data.Minor = vr.GetNextDouble();
            CurrConfiguration_Data.Patch = vr.GetNextDouble();
            OnPropertyChanged(Firmware_VersionPropertyChangedName); // "Firmware_Version"
            return CurrConfiguration_Data;
        }
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Configuration_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Configuration_Data> ReadMTU_Request(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Configuration_MTU_Request_index;
            await Ensure_Characteristic_Async(ServiceIndex.Configuration_index, "Configuration", index, "MTU Request");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "MTU Request", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8 U16");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrConfiguration_Data.param0 = vr.GetNextDouble();
            CurrConfiguration_Data.param1 = vr.GetNextDouble();
            OnPropertyChanged(MTU_RequestPropertyChangedName); // "MTU_Request"
            return CurrConfiguration_Data;
        }
        /// Reads data
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Configuration_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Configuration_Data> ReadNFC_Tag(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Configuration_NFC_Tag_index;
            await Ensure_Characteristic_Async(ServiceIndex.Configuration_index, "Configuration", index, "NFC Tag");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "NFC Tag", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("STRING|ASCII|NFCTag");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrConfiguration_Data.NFCTag = vr.GetNextString();
            OnPropertyChanged(NFC_TagPropertyChangedName); // "NFC_Tag"
            return CurrConfiguration_Data;
        }

        #endregion
//


// Long obsolete! [[zzMETHOD+LIST]]
    }
}