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
    /// Hand-held multimeter and oscilloscope that uses a phone (or PC) as the display..
    /// This class was automatically generated 2026-10-20::11:38
    /// </summary>

    public partial class PokitProMeter : INotifyPropertyChanged
    {
        // Useful links for the device and protocol documentation
        // Link: https://help.pokitmeter.com/hc/en-us/community/posts/360023523213-Bluetooth-API-Documentation
        // Link: https://www.pokitinnovations.com/wp-content/uploads/D0005250-PokitMeter-Bluetooth-API-Documentation-0_02.pdf
        // Link: https://github.com/pcolby/pokit/blob/6937d011c528b3a5f00c6e867ec91a71eaad5bcb/src/lib/uuids.h
        // Link: https://github.com/pcolby/qtpokit

        public BluetoothLEDevice ble { get; set; } = null;
        public BluetoothStatusEvent Status = new BluetoothStatusEvent();

        // For the INotifyPropertyChanged values
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Name { get; } = "PokitPro";
        public string Description { get; } = "Hand-held multimeter and oscilloscope that uses a phone (or PC) as the display.";

        /* Service and Characteristics summary for the device PokitPro

        Multimeter service Guid=e7481d2f-5781-442e-bb9a-fd4e3441dadc
            Multimeter_Data (DataGroup record)
                MM_Settings characteristic has ModeSetting (Byte-->double) RangeSetting (Byte-->double) IntervalSetting (UInt32-->double)  Guid=53dc9a7a-bc19-4280-b76b-002d0e23b078
                MM_Data characteristic has Status (Byte-->double) Data (Single-->double) OperationMode (Byte-->double) Range (Byte-->double)  Guid=047d3559-8bee-423a-b229-4417fa603b90


        DSO_Oscilloscope service Guid=1569801e-1425-4a7a-b617-a4f4ed719de6
            DSO_Oscilloscope_Data (DataGroup record)
                DSO_Settings characteristic has DsoTriggerType (Byte-->double) DsoTriggerLevel (Single-->double) DsoMode (Byte-->double) DsoRange (Byte-->double) DsoSamplingWindow (UInt32-->double) DsoNSamples (UInt16-->double)  Guid=a81af1b6-b8b3-4244-8859-3da368d2be39
                DSO_Reading characteristic has DsoDataRaw (Int16-->double[])  Guid=98e14f8e-536e-4f24-b4f4-1debfed0a99e
                DSO_Metadata characteristic has DsoStatus (Byte-->double) DsoDataScale (Single-->double) DsoDataMode (Byte-->double) DsoDataRange (Byte-->double) DsoDataSamplingWindow (UInt32-->double) DsoDataNsamples (UInt16-->double) DsoSamplingRate (UInt32-->double)  Guid=970f00ba-f46f-4825-96a8-153a5cd0cda9


        DataLogger_Dlog service Guid=a5ff3566-1fd8-4e10-8362-590a578a4121
            DataLogger_Dlog_Data (DataGroup record)
                DataLogger_Settings characteristic has DlogCommand (Byte-->double) DlogReserved1 (UInt16-->double) DlogModeSetting (Byte-->double) DlogRangeSetting (Byte-->double) DlogUpdateInterval (UInt16-->double) DlogTimestamp (UInt32-->double)  Guid=5f97c62b-a83b-46c6-b9cd-cac59e130a78
                DataLogger_Reading characteristic has DlogData (Bytes-->string)  Guid=3c669dab-fc86-411c-9498-4f9415049cc0
                DataLogger_MetaData characteristic has DlogStatus (Byte-->double) DlogScale (Single-->double) DlogMode (Byte-->double) DlogRange (Byte-->double) DlogCurrLogging (UInt16-->double) DlogCurrNSample (UInt16-->double) DlogCurrTimestamp (UInt32-->double)  Guid=9acada2e-3936-430b-a8f7-da407d97ca6e


        Generic Service service Guid=1801
            Generic Service_Data (DataGroup record)
                Server_Supported_Features characteristic has FeatureBitmap0 (Byte-->double)  Guid=2803
                Service Changes characteristic has StartRange (UInt16-->double) EndRange (UInt16-->double)  Guid=2a05
                Client_Supported_Features characteristic has FeatureBitmapCSF (Byte-->double)  Guid=2b29
                Database Hash characteristic has Hash0 (UInt32-->double) Hash1 (UInt32-->double) Hash2 (UInt32-->double) Hash3 (UInt32-->double)  Guid=2b2a


        Common Configuration service Guid=1800
            Common Configuration_Data (DataGroup record)
                Device Name characteristic has Device_Name (String-->string)  Guid=2a00
                Appearance characteristic has Appearance (UInt16-->double)  Guid=2a01


        Device Info service Guid=180a
            Device Info_Data (DataGroup record)
                Manufacturer Name characteristic has Manufacturer_Name (String-->string)  Guid=2a29
                Model Number characteristic has Model_Number (String-->string)  Guid=2a24
                Firmware Revision characteristic has Firmware_Revision (String-->string)  Guid=2a26
                Software Revision characteristic has Software_Revision (String-->string)  Guid=2a28
                Hardware Revision characteristic has Hardware_Revision (String-->string)  Guid=2a27
                Serial Number characteristic has Serial_Number (String-->string)  Guid=2a25


        Service_Status service Guid=57d3a771-267c-4394-8872-78223e92aec5
            Service_Status_Data (DataGroup record)
                Status_Device characteristic has FirmwareMajor (Byte-->double) FirmwareMinor (Byte-->double) MaxInputVoltage (UInt16-->double) MaxInputCurrent (UInt16-->double) MaxInputResistance (UInt16-->double) MaxSamplingRate (UInt16-->double) DeviceBufferSize (UInt16-->double) Reserved01 (UInt16-->double) MacAddress (Bytes-->string)  Guid=6974f5e5-0e54-45c3-97dd-29e4b5fb0849
                Status_Status characteristic has DeviceStatus (Byte-->double) BatteryLevel (Single-->double)  Guid=3dba36e1-6120-4706-8dfd-ed9c16e569b6
                Status_Device_Name characteristic has Device_Name (String-->string)  Guid=7f0375de-077e-4555-8f78-800494509cc3
                Status_Flash_LED characteristic has Red (Byte-->double) Green (Byte-->double) Blue (Byte-->double) Beep (Byte-->double)  Guid=ec9bb1f3-05a9-4277-8dd0-60a7896f0d6e
                Status_Light_LED characteristic has Light (Byte-->double)  Guid=aaf3f6d5-43d4-4a83-9510-dff3d858d4cc
                UnknownStatusValues characteristic has StatusUnknown5 (Bytes-->string)  Guid=8fe5b5a9-b5b4-4a7b-8ff2-87224b970f89


        CalibrationService service Guid=6f53be2f-780b-49b8-a7c3-e8a052b3ae2c
            CalibrationService_Data (DataGroup record)
                CalbrateTemperature characteristic has CalibrateUnknown0 (Bytes-->string)  Guid=0cd0f713-f5aa-4572-9e23-f8049f6bcaaa
                CalibrateUnknown1 characteristic has CalibrateUnknown1 (Bytes-->string)  Guid=b6728f91-409c-4d6c-864e-272a6a7a0204
                CalibrrateUnknown2 characteristic has CalibrrateUnknown2 (Bytes-->string)  Guid=5588e47b-cb81-4f7b-acc4-6029a3f39f72


        Silabs_Service_OTA service Guid=1d14d6ee-fd63-4fa1-bfa4-8f47b42119f0
            Silabs_Service_OTA_Data (DataGroup record)
                OTA_Control characteristic has Unknown0 (Bytes-->string)  Guid=f7bf3564-fb6d-4e53-88a4-5e37e0326063
                OTA_Data characteristic has Unknown1 (Bytes-->string)  Guid=984227f3-34fc-4045-a5d0-2c581f81a153
        */

        public const string MM_SettingsPropertyChangedName = "MM_Settings";
        public const string MM_DataPropertyChangedName = "MM_Data";
        public const string DSO_SettingsPropertyChangedName = "DSO_Settings";
        public const string DSO_ReadingPropertyChangedName = "DSO_Reading";
        public const string DSO_MetadataPropertyChangedName = "DSO_Metadata";
        public const string DataLogger_SettingsPropertyChangedName = "DataLogger_Settings";
        public const string DataLogger_ReadingPropertyChangedName = "DataLogger_Reading";
        public const string DataLogger_MetaDataPropertyChangedName = "DataLogger_MetaData";
        public const string Server_Supported_FeaturesPropertyChangedName = "Server_Supported_Features";
        public const string Service_ChangesPropertyChangedName = "Service_Changes";
        public const string Client_Supported_FeaturesPropertyChangedName = "Client_Supported_Features";
        public const string Database_HashPropertyChangedName = "Database_Hash";
        public const string Device_NamePropertyChangedName = "Device_Name";
        public const string AppearancePropertyChangedName = "Appearance";
        public const string Manufacturer_NamePropertyChangedName = "Manufacturer_Name";
        public const string Model_NumberPropertyChangedName = "Model_Number";
        public const string Firmware_RevisionPropertyChangedName = "Firmware_Revision";
        public const string Software_RevisionPropertyChangedName = "Software_Revision";
        public const string Hardware_RevisionPropertyChangedName = "Hardware_Revision";
        public const string Serial_NumberPropertyChangedName = "Serial_Number";
        public const string Status_DevicePropertyChangedName = "Status_Device";
        public const string Status_StatusPropertyChangedName = "Status_Status";
        public const string Status_Device_NamePropertyChangedName = "Status_Device_Name";
        public const string Status_Flash_LEDPropertyChangedName = "Status_Flash_LED";
        public const string Status_Light_LEDPropertyChangedName = "Status_Light_LED";
        public const string UnknownStatusValuesPropertyChangedName = "UnknownStatusValues";
        public const string CalbrateTemperaturePropertyChangedName = "CalbrateTemperature";
        public const string CalibrateUnknown1PropertyChangedName = "CalibrateUnknown1";
        public const string CalibrrateUnknown2PropertyChangedName = "CalibrrateUnknown2";
        public const string OTA_ControlPropertyChangedName = "OTA_Control";
        public const string OTA_DataPropertyChangedName = "OTA_Data";



        //
        // All services / characteristics data types 
        //

        #region All_Data_Types
        /// <summary>
        /// Data from all of the characteristics in the Multimeter Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class Multimeter_Data :BTCommonMetaData<Multimeter_Data> //, IExportDataSource
        {
            private double _ModeSetting = 0;
            /// <summary>
            /// ModeSetting (U8 ) from Service=Multimeter and Characteristic=MM_Settings
            ///</summary>
            public double ModeSetting 
            { 
                get { return _ModeSetting; }
                set { if (value == _ModeSetting) return; _ModeSetting = value; OnPropertyChanged();}
            }
            private double _RangeSetting = 0;
            /// <summary>
            /// RangeSetting (U8 ) from Service=Multimeter and Characteristic=MM_Settings
            ///</summary>
            public double RangeSetting 
            { 
                get { return _RangeSetting; }
                set { if (value == _RangeSetting) return; _RangeSetting = value; OnPropertyChanged();}
            }
            private double _IntervalSetting = 0;
            /// <summary>
            /// IntervalSetting (U32 ms) from Service=Multimeter and Characteristic=MM_Settings
            ///</summary>
            public double IntervalSetting 
            { 
                get { return _IntervalSetting; }
                set { if (value == _IntervalSetting) return; _IntervalSetting = value; OnPropertyChanged();}
            }

            private double _Status = 0;
            /// <summary>
            /// Status (U8 ) from Service=Multimeter and Characteristic=MM_Data
            ///</summary>
            public double Status 
            { 
                get { return _Status; }
                set { if (value == _Status) return; _Status = value; OnPropertyChanged();}
            }
            private double _Data = 0.0;
            /// <summary>
            /// Data (F32 ) from Service=Multimeter and Characteristic=MM_Data
            ///</summary>
            public double Data 
            { 
                get { return _Data; }
                set { if (value == _Data) return; _Data = value; OnPropertyChanged();}
            }
            private double _OperationMode = 0;
            /// <summary>
            /// OperationMode (U8 ) from Service=Multimeter and Characteristic=MM_Data
            ///</summary>
            public double OperationMode 
            { 
                get { return _OperationMode; }
                set { if (value == _OperationMode) return; _OperationMode = value; OnPropertyChanged();}
            }
            private double _Range = 0;
            /// <summary>
            /// Range (U8 ) from Service=Multimeter and Characteristic=MM_Data
            ///</summary>
            public double Range 
            { 
                get { return _Range; }
                set { if (value == _Range) return; _Range = value; OnPropertyChanged();}
            }
            public override Multimeter_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as Multimeter_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(Multimeter_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.ModeSetting = source.ModeSetting;
                dest.RangeSetting = source.RangeSetting;
                dest.IntervalSetting = source.IntervalSetting;
                dest.Status = source.Status;
                dest.Data = source.Data;
                dest.OperationMode = source.OperationMode;
                dest.Range = source.Range;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static Multimeter_Data CopyToWithConvertAndCreate(Multimeter_Data source, Multimeter_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.ModeSetting = convert(source.ModeSetting, "");
                dest.RangeSetting = convert(source.RangeSetting, "");
                dest.IntervalSetting = convert(source.IntervalSetting, "ms");
                dest.Status = convert(source.Status, "");
                dest.Data = convert(source.Data, "");
                dest.OperationMode = convert(source.OperationMode, "");
                dest.Range = convert(source.Range, "");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["ModeSetting", "RangeSetting", "IntervalSetting", "Status", "Data", "OperationMode", "Range"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(ModeSetting);
                exporter.CellSet(RangeSetting);
                exporter.CellSet(IntervalSetting);
                exporter.CellSet(Status);
                exporter.CellSet(Data);
                exporter.CellSet(OperationMode);
                exporter.CellSet(Range);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {ModeSetting} {RangeSetting} {IntervalSetting} {Status} {Data} {OperationMode} {Range}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the DSO_Oscilloscope Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class DSO_Oscilloscope_Data :BTCommonMetaData<DSO_Oscilloscope_Data> //, IExportDataSource
        {
            private double _DsoTriggerType = 0;
            /// <summary>
            /// DsoTriggerType (U8 ) from Service=DSO_Oscilloscope and Characteristic=DSO_Settings
            ///</summary>
            public double DsoTriggerType 
            { 
                get { return _DsoTriggerType; }
                set { if (value == _DsoTriggerType) return; _DsoTriggerType = value; OnPropertyChanged();}
            }
            private double _DsoTriggerLevel = 0.0;
            /// <summary>
            /// DsoTriggerLevel (F32 ) from Service=DSO_Oscilloscope and Characteristic=DSO_Settings
            ///</summary>
            public double DsoTriggerLevel 
            { 
                get { return _DsoTriggerLevel; }
                set { if (value == _DsoTriggerLevel) return; _DsoTriggerLevel = value; OnPropertyChanged();}
            }
            private double _DsoMode = 0;
            /// <summary>
            /// DsoMode (U8 ) from Service=DSO_Oscilloscope and Characteristic=DSO_Settings
            ///</summary>
            public double DsoMode 
            { 
                get { return _DsoMode; }
                set { if (value == _DsoMode) return; _DsoMode = value; OnPropertyChanged();}
            }
            private double _DsoRange = 0;
            /// <summary>
            /// DsoRange (U8 ) from Service=DSO_Oscilloscope and Characteristic=DSO_Settings
            ///</summary>
            public double DsoRange 
            { 
                get { return _DsoRange; }
                set { if (value == _DsoRange) return; _DsoRange = value; OnPropertyChanged();}
            }
            private double _DsoSamplingWindow = 0;
            /// <summary>
            /// DsoSamplingWindow (U32 ms) from Service=DSO_Oscilloscope and Characteristic=DSO_Settings
            ///</summary>
            public double DsoSamplingWindow 
            { 
                get { return _DsoSamplingWindow; }
                set { if (value == _DsoSamplingWindow) return; _DsoSamplingWindow = value; OnPropertyChanged();}
            }
            private double _DsoNSamples = 0;
            /// <summary>
            /// DsoNSamples (U16 ) from Service=DSO_Oscilloscope and Characteristic=DSO_Settings
            ///</summary>
            public double DsoNSamples 
            { 
                get { return _DsoNSamples; }
                set { if (value == _DsoNSamples) return; _DsoNSamples = value; OnPropertyChanged();}
            }

            private List<double> _DsoDataRaw = new();
            /// <summary>
            /// DsoDataRaw (I16S ) from Service=DSO_Oscilloscope and Characteristic=DSO_Reading
            ///</summary>
            public List<double> DsoDataRaw 
            { 
                get { return _DsoDataRaw; }
                set { if (value == _DsoDataRaw) return; _DsoDataRaw = value; OnPropertyChanged();}
            }

            private double _DsoStatus = 0;
            /// <summary>
            /// DsoStatus (U8 ) from Service=DSO_Oscilloscope and Characteristic=DSO_Metadata
            ///</summary>
            public double DsoStatus 
            { 
                get { return _DsoStatus; }
                set { if (value == _DsoStatus) return; _DsoStatus = value; OnPropertyChanged();}
            }
            private double _DsoDataScale = 0.0;
            /// <summary>
            /// DsoDataScale (F32 ) from Service=DSO_Oscilloscope and Characteristic=DSO_Metadata
            ///</summary>
            public double DsoDataScale 
            { 
                get { return _DsoDataScale; }
                set { if (value == _DsoDataScale) return; _DsoDataScale = value; OnPropertyChanged();}
            }
            private double _DsoDataMode = 0;
            /// <summary>
            /// DsoDataMode (U8 ) from Service=DSO_Oscilloscope and Characteristic=DSO_Metadata
            ///</summary>
            public double DsoDataMode 
            { 
                get { return _DsoDataMode; }
                set { if (value == _DsoDataMode) return; _DsoDataMode = value; OnPropertyChanged();}
            }
            private double _DsoDataRange = 0;
            /// <summary>
            /// DsoDataRange (U8 ) from Service=DSO_Oscilloscope and Characteristic=DSO_Metadata
            ///</summary>
            public double DsoDataRange 
            { 
                get { return _DsoDataRange; }
                set { if (value == _DsoDataRange) return; _DsoDataRange = value; OnPropertyChanged();}
            }
            private double _DsoDataSamplingWindow = 0;
            /// <summary>
            /// DsoDataSamplingWindow (U32 ) from Service=DSO_Oscilloscope and Characteristic=DSO_Metadata
            ///</summary>
            public double DsoDataSamplingWindow 
            { 
                get { return _DsoDataSamplingWindow; }
                set { if (value == _DsoDataSamplingWindow) return; _DsoDataSamplingWindow = value; OnPropertyChanged();}
            }
            private double _DsoDataNsamples = 0;
            /// <summary>
            /// DsoDataNsamples (U16 ) from Service=DSO_Oscilloscope and Characteristic=DSO_Metadata
            ///</summary>
            public double DsoDataNsamples 
            { 
                get { return _DsoDataNsamples; }
                set { if (value == _DsoDataNsamples) return; _DsoDataNsamples = value; OnPropertyChanged();}
            }
            private double _DsoSamplingRate = 0;
            /// <summary>
            /// DsoSamplingRate (U32 hz) from Service=DSO_Oscilloscope and Characteristic=DSO_Metadata
            ///</summary>
            public double DsoSamplingRate 
            { 
                get { return _DsoSamplingRate; }
                set { if (value == _DsoSamplingRate) return; _DsoSamplingRate = value; OnPropertyChanged();}
            }
            public override DSO_Oscilloscope_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as DSO_Oscilloscope_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(DSO_Oscilloscope_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.DsoTriggerType = source.DsoTriggerType;
                dest.DsoTriggerLevel = source.DsoTriggerLevel;
                dest.DsoMode = source.DsoMode;
                dest.DsoRange = source.DsoRange;
                dest.DsoSamplingWindow = source.DsoSamplingWindow;
                dest.DsoNSamples = source.DsoNSamples;
                dest.DsoDataRaw = new List<double>(source.DsoDataRaw);
                dest.DsoStatus = source.DsoStatus;
                dest.DsoDataScale = source.DsoDataScale;
                dest.DsoDataMode = source.DsoDataMode;
                dest.DsoDataRange = source.DsoDataRange;
                dest.DsoDataSamplingWindow = source.DsoDataSamplingWindow;
                dest.DsoDataNsamples = source.DsoDataNsamples;
                dest.DsoSamplingRate = source.DsoSamplingRate;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static DSO_Oscilloscope_Data CopyToWithConvertAndCreate(DSO_Oscilloscope_Data source, DSO_Oscilloscope_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.DsoTriggerType = convert(source.DsoTriggerType, "");
                dest.DsoTriggerLevel = convert(source.DsoTriggerLevel, "");
                dest.DsoMode = convert(source.DsoMode, "");
                dest.DsoRange = convert(source.DsoRange, "");
                dest.DsoSamplingWindow = convert(source.DsoSamplingWindow, "ms");
                dest.DsoNSamples = convert(source.DsoNSamples, "");
                if (source.DsoDataRaw != null)
                {
                    dest.DsoDataRaw = new List<double>();
                    foreach (var value in source.DsoDataRaw)
                    {
                        double newvalue = convert(value, "");
                        dest.DsoDataRaw.Add(newvalue);
                    }
                }
                dest.DsoStatus = convert(source.DsoStatus, "");
                dest.DsoDataScale = convert(source.DsoDataScale, "");
                dest.DsoDataMode = convert(source.DsoDataMode, "");
                dest.DsoDataRange = convert(source.DsoDataRange, "");
                dest.DsoDataSamplingWindow = convert(source.DsoDataSamplingWindow, "");
                dest.DsoDataNsamples = convert(source.DsoDataNsamples, "");
                dest.DsoSamplingRate = convert(source.DsoSamplingRate, "hz");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["DsoTriggerType", "DsoTriggerLevel", "DsoMode", "DsoRange", "DsoSamplingWindow", "DsoNSamples", "DsoDataRaw", "DsoStatus", "DsoDataScale", "DsoDataMode", "DsoDataRange", "DsoDataSamplingWindow", "DsoDataNsamples", "DsoSamplingRate"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(DsoTriggerType);
                exporter.CellSet(DsoTriggerLevel);
                exporter.CellSet(DsoMode);
                exporter.CellSet(DsoRange);
                exporter.CellSet(DsoSamplingWindow);
                exporter.CellSet(DsoNSamples);
                exporter.CellSet(DsoDataRaw);
                exporter.CellSet(DsoStatus);
                exporter.CellSet(DsoDataScale);
                exporter.CellSet(DsoDataMode);
                exporter.CellSet(DsoDataRange);
                exporter.CellSet(DsoDataSamplingWindow);
                exporter.CellSet(DsoDataNsamples);
                exporter.CellSet(DsoSamplingRate);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {DsoTriggerType} {DsoTriggerLevel} {DsoMode} {DsoRange} {DsoSamplingWindow} {DsoNSamples} {DsoDataRaw} {DsoStatus} {DsoDataScale} {DsoDataMode} {DsoDataRange} {DsoDataSamplingWindow} {DsoDataNsamples} {DsoSamplingRate}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the DataLogger_Dlog Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class DataLogger_Dlog_Data :BTCommonMetaData<DataLogger_Dlog_Data> //, IExportDataSource
        {
            private double _DlogCommand = 0;
            /// <summary>
            /// DlogCommand (U8 ) from Service=DataLogger_Dlog and Characteristic=DataLogger_Settings
            ///</summary>
            public double DlogCommand 
            { 
                get { return _DlogCommand; }
                set { if (value == _DlogCommand) return; _DlogCommand = value; OnPropertyChanged();}
            }
            private double _DlogReserved1 = 0;
            /// <summary>
            /// DlogReserved1 (U16 ) from Service=DataLogger_Dlog and Characteristic=DataLogger_Settings
            ///</summary>
            public double DlogReserved1 
            { 
                get { return _DlogReserved1; }
                set { if (value == _DlogReserved1) return; _DlogReserved1 = value; OnPropertyChanged();}
            }
            private double _DlogModeSetting = 0;
            /// <summary>
            /// DlogModeSetting (U8 ) from Service=DataLogger_Dlog and Characteristic=DataLogger_Settings
            ///</summary>
            public double DlogModeSetting 
            { 
                get { return _DlogModeSetting; }
                set { if (value == _DlogModeSetting) return; _DlogModeSetting = value; OnPropertyChanged();}
            }
            private double _DlogRangeSetting = 0;
            /// <summary>
            /// DlogRangeSetting (U8 ) from Service=DataLogger_Dlog and Characteristic=DataLogger_Settings
            ///</summary>
            public double DlogRangeSetting 
            { 
                get { return _DlogRangeSetting; }
                set { if (value == _DlogRangeSetting) return; _DlogRangeSetting = value; OnPropertyChanged();}
            }
            private double _DlogUpdateInterval = 0;
            /// <summary>
            /// DlogUpdateInterval (U16 s) from Service=DataLogger_Dlog and Characteristic=DataLogger_Settings
            ///</summary>
            public double DlogUpdateInterval 
            { 
                get { return _DlogUpdateInterval; }
                set { if (value == _DlogUpdateInterval) return; _DlogUpdateInterval = value; OnPropertyChanged();}
            }
            private double _DlogTimestamp = 0;
            /// <summary>
            /// DlogTimestamp (U32 ) from Service=DataLogger_Dlog and Characteristic=DataLogger_Settings
            ///</summary>
            public double DlogTimestamp 
            { 
                get { return _DlogTimestamp; }
                set { if (value == _DlogTimestamp) return; _DlogTimestamp = value; OnPropertyChanged();}
            }

            private byte[] _DlogData = null;
            /// <summary>
            /// DlogData (BYTES ) from Service=DataLogger_Dlog and Characteristic=DataLogger_Reading
            ///</summary>
            public byte[] DlogData 
            { 
                get { return _DlogData; }
                set { if (value == _DlogData) return; _DlogData = value; OnPropertyChanged();}
            }

            private double _DlogStatus = 0;
            /// <summary>
            /// DlogStatus (U8 ) from Service=DataLogger_Dlog and Characteristic=DataLogger_MetaData
            ///</summary>
            public double DlogStatus 
            { 
                get { return _DlogStatus; }
                set { if (value == _DlogStatus) return; _DlogStatus = value; OnPropertyChanged();}
            }
            private double _DlogScale = 0.0;
            /// <summary>
            /// DlogScale (F32 ) from Service=DataLogger_Dlog and Characteristic=DataLogger_MetaData
            ///</summary>
            public double DlogScale 
            { 
                get { return _DlogScale; }
                set { if (value == _DlogScale) return; _DlogScale = value; OnPropertyChanged();}
            }
            private double _DlogMode = 0;
            /// <summary>
            /// DlogMode (U8 ) from Service=DataLogger_Dlog and Characteristic=DataLogger_MetaData
            ///</summary>
            public double DlogMode 
            { 
                get { return _DlogMode; }
                set { if (value == _DlogMode) return; _DlogMode = value; OnPropertyChanged();}
            }
            private double _DlogRange = 0;
            /// <summary>
            /// DlogRange (U8 ) from Service=DataLogger_Dlog and Characteristic=DataLogger_MetaData
            ///</summary>
            public double DlogRange 
            { 
                get { return _DlogRange; }
                set { if (value == _DlogRange) return; _DlogRange = value; OnPropertyChanged();}
            }
            private double _DlogCurrLogging = 0;
            /// <summary>
            /// DlogCurrLogging (U16 us) from Service=DataLogger_Dlog and Characteristic=DataLogger_MetaData
            ///</summary>
            public double DlogCurrLogging 
            { 
                get { return _DlogCurrLogging; }
                set { if (value == _DlogCurrLogging) return; _DlogCurrLogging = value; OnPropertyChanged();}
            }
            private double _DlogCurrNSample = 0;
            /// <summary>
            /// DlogCurrNSample (U16 ) from Service=DataLogger_Dlog and Characteristic=DataLogger_MetaData
            ///</summary>
            public double DlogCurrNSample 
            { 
                get { return _DlogCurrNSample; }
                set { if (value == _DlogCurrNSample) return; _DlogCurrNSample = value; OnPropertyChanged();}
            }
            private double _DlogCurrTimestamp = 0;
            /// <summary>
            /// DlogCurrTimestamp (U32 ) from Service=DataLogger_Dlog and Characteristic=DataLogger_MetaData
            ///</summary>
            public double DlogCurrTimestamp 
            { 
                get { return _DlogCurrTimestamp; }
                set { if (value == _DlogCurrTimestamp) return; _DlogCurrTimestamp = value; OnPropertyChanged();}
            }
            public override DataLogger_Dlog_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as DataLogger_Dlog_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(DataLogger_Dlog_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.DlogCommand = source.DlogCommand;
                dest.DlogReserved1 = source.DlogReserved1;
                dest.DlogModeSetting = source.DlogModeSetting;
                dest.DlogRangeSetting = source.DlogRangeSetting;
                dest.DlogUpdateInterval = source.DlogUpdateInterval;
                dest.DlogTimestamp = source.DlogTimestamp;
                dest.DlogData = source.DlogData;
                dest.DlogStatus = source.DlogStatus;
                dest.DlogScale = source.DlogScale;
                dest.DlogMode = source.DlogMode;
                dest.DlogRange = source.DlogRange;
                dest.DlogCurrLogging = source.DlogCurrLogging;
                dest.DlogCurrNSample = source.DlogCurrNSample;
                dest.DlogCurrTimestamp = source.DlogCurrTimestamp;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static DataLogger_Dlog_Data CopyToWithConvertAndCreate(DataLogger_Dlog_Data source, DataLogger_Dlog_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.DlogCommand = convert(source.DlogCommand, "");
                dest.DlogReserved1 = convert(source.DlogReserved1, "");
                dest.DlogModeSetting = convert(source.DlogModeSetting, "");
                dest.DlogRangeSetting = convert(source.DlogRangeSetting, "");
                dest.DlogUpdateInterval = convert(source.DlogUpdateInterval, "s");
                dest.DlogTimestamp = convert(source.DlogTimestamp, "");
                dest.DlogData = source.DlogData;
                dest.DlogStatus = convert(source.DlogStatus, "");
                dest.DlogScale = convert(source.DlogScale, "");
                dest.DlogMode = convert(source.DlogMode, "");
                dest.DlogRange = convert(source.DlogRange, "");
                dest.DlogCurrLogging = convert(source.DlogCurrLogging, "us");
                dest.DlogCurrNSample = convert(source.DlogCurrNSample, "");
                dest.DlogCurrTimestamp = convert(source.DlogCurrTimestamp, "");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["DlogCommand", "DlogReserved1", "DlogModeSetting", "DlogRangeSetting", "DlogUpdateInterval", "DlogTimestamp", "DlogData", "DlogStatus", "DlogScale", "DlogMode", "DlogRange", "DlogCurrLogging", "DlogCurrNSample", "DlogCurrTimestamp"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(DlogCommand);
                exporter.CellSet(DlogReserved1);
                exporter.CellSet(DlogModeSetting);
                exporter.CellSet(DlogRangeSetting);
                exporter.CellSet(DlogUpdateInterval);
                exporter.CellSet(DlogTimestamp);
                exporter.CellSet(DlogData);
                exporter.CellSet(DlogStatus);
                exporter.CellSet(DlogScale);
                exporter.CellSet(DlogMode);
                exporter.CellSet(DlogRange);
                exporter.CellSet(DlogCurrLogging);
                exporter.CellSet(DlogCurrNSample);
                exporter.CellSet(DlogCurrTimestamp);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {DlogCommand} {DlogReserved1} {DlogModeSetting} {DlogRangeSetting} {DlogUpdateInterval} {DlogTimestamp} {DlogData} {DlogStatus} {DlogScale} {DlogMode} {DlogRange} {DlogCurrLogging} {DlogCurrNSample} {DlogCurrTimestamp}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the Generic Service Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class Generic_Service_Data :BTCommonMetaData<Generic_Service_Data> //, IExportDataSource
        {
            private double _FeatureBitmap0 = 0;
            /// <summary>
            /// FeatureBitmap0 (U8 ) from Service=Generic Service and Characteristic=Server_Supported_Features
            ///</summary>
            public double FeatureBitmap0 
            { 
                get { return _FeatureBitmap0; }
                set { if (value == _FeatureBitmap0) return; _FeatureBitmap0 = value; OnPropertyChanged();}
            }

            private double _StartRange = 0;
            /// <summary>
            /// StartRange (U16 ) from Service=Generic Service and Characteristic=Service Changes
            ///</summary>
            public double StartRange 
            { 
                get { return _StartRange; }
                set { if (value == _StartRange) return; _StartRange = value; OnPropertyChanged();}
            }
            private double _EndRange = 0;
            /// <summary>
            /// EndRange (U16 ) from Service=Generic Service and Characteristic=Service Changes
            ///</summary>
            public double EndRange 
            { 
                get { return _EndRange; }
                set { if (value == _EndRange) return; _EndRange = value; OnPropertyChanged();}
            }

            private double _FeatureBitmapCSF = 0;
            /// <summary>
            /// FeatureBitmapCSF (U8 ) from Service=Generic Service and Characteristic=Client_Supported_Features
            ///</summary>
            public double FeatureBitmapCSF 
            { 
                get { return _FeatureBitmapCSF; }
                set { if (value == _FeatureBitmapCSF) return; _FeatureBitmapCSF = value; OnPropertyChanged();}
            }

            private double _Hash0 = 0;
            /// <summary>
            /// Hash0 (U32 ) from Service=Generic Service and Characteristic=Database Hash
            ///</summary>
            public double Hash0 
            { 
                get { return _Hash0; }
                set { if (value == _Hash0) return; _Hash0 = value; OnPropertyChanged();}
            }
            private double _Hash1 = 0;
            /// <summary>
            /// Hash1 (U32 ) from Service=Generic Service and Characteristic=Database Hash
            ///</summary>
            public double Hash1 
            { 
                get { return _Hash1; }
                set { if (value == _Hash1) return; _Hash1 = value; OnPropertyChanged();}
            }
            private double _Hash2 = 0;
            /// <summary>
            /// Hash2 (U32 ) from Service=Generic Service and Characteristic=Database Hash
            ///</summary>
            public double Hash2 
            { 
                get { return _Hash2; }
                set { if (value == _Hash2) return; _Hash2 = value; OnPropertyChanged();}
            }
            private double _Hash3 = 0;
            /// <summary>
            /// Hash3 (U32 ) from Service=Generic Service and Characteristic=Database Hash
            ///</summary>
            public double Hash3 
            { 
                get { return _Hash3; }
                set { if (value == _Hash3) return; _Hash3 = value; OnPropertyChanged();}
            }
            public override Generic_Service_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as Generic_Service_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(Generic_Service_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.FeatureBitmap0 = source.FeatureBitmap0;
                dest.StartRange = source.StartRange;
                dest.EndRange = source.EndRange;
                dest.FeatureBitmapCSF = source.FeatureBitmapCSF;
                dest.Hash0 = source.Hash0;
                dest.Hash1 = source.Hash1;
                dest.Hash2 = source.Hash2;
                dest.Hash3 = source.Hash3;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static Generic_Service_Data CopyToWithConvertAndCreate(Generic_Service_Data source, Generic_Service_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.FeatureBitmap0 = convert(source.FeatureBitmap0, "");
                dest.StartRange = convert(source.StartRange, "");
                dest.EndRange = convert(source.EndRange, "");
                dest.FeatureBitmapCSF = convert(source.FeatureBitmapCSF, "");
                dest.Hash0 = convert(source.Hash0, "");
                dest.Hash1 = convert(source.Hash1, "");
                dest.Hash2 = convert(source.Hash2, "");
                dest.Hash3 = convert(source.Hash3, "");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["FeatureBitmap0", "StartRange", "EndRange", "FeatureBitmapCSF", "Hash0", "Hash1", "Hash2", "Hash3"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(FeatureBitmap0);
                exporter.CellSet(StartRange);
                exporter.CellSet(EndRange);
                exporter.CellSet(FeatureBitmapCSF);
                exporter.CellSet(Hash0);
                exporter.CellSet(Hash1);
                exporter.CellSet(Hash2);
                exporter.CellSet(Hash3);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {FeatureBitmap0} {StartRange} {EndRange} {FeatureBitmapCSF} {Hash0} {Hash1} {Hash2} {Hash3}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the Common Configuration Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class Common_Configuration_Data :BTCommonMetaData<Common_Configuration_Data> //, IExportDataSource
        {
            private string _Device_Name = "";
            /// <summary>
            /// Device_Name (STRING ) from Service=Common Configuration and Characteristic=Device Name
            ///</summary>
            public string Device_Name 
            { 
                get { return _Device_Name; }
                set { if (value == _Device_Name) return; _Device_Name = value; OnPropertyChanged();}
            }

            private double _Appearance = 0;
            /// <summary>
            /// Appearance (U16 ) from Service=Common Configuration and Characteristic=Appearance
            ///</summary>
            public double Appearance 
            { 
                get { return _Appearance; }
                set { if (value == _Appearance) return; _Appearance = value; OnPropertyChanged();}
            }
            public override Common_Configuration_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as Common_Configuration_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(Common_Configuration_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.Device_Name = source.Device_Name;
                dest.Appearance = source.Appearance;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static Common_Configuration_Data CopyToWithConvertAndCreate(Common_Configuration_Data source, Common_Configuration_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.Device_Name = source.Device_Name;
                dest.Appearance = convert(source.Appearance, "");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["Device_Name", "Appearance"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(Device_Name);
                exporter.CellSet(Appearance);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {Device_Name} {Appearance}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the Device Info Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class Device_Info_Data :BTCommonMetaData<Device_Info_Data> //, IExportDataSource
        {
            private string _Manufacturer_Name = "";
            /// <summary>
            /// Manufacturer_Name (STRING ) from Service=Device Info and Characteristic=Manufacturer Name
            ///</summary>
            public string Manufacturer_Name 
            { 
                get { return _Manufacturer_Name; }
                set { if (value == _Manufacturer_Name) return; _Manufacturer_Name = value; OnPropertyChanged();}
            }

            private string _Model_Number = "";
            /// <summary>
            /// Model_Number (STRING ) from Service=Device Info and Characteristic=Model Number
            ///</summary>
            public string Model_Number 
            { 
                get { return _Model_Number; }
                set { if (value == _Model_Number) return; _Model_Number = value; OnPropertyChanged();}
            }

            private string _Firmware_Revision = "";
            /// <summary>
            /// Firmware_Revision (STRING ) from Service=Device Info and Characteristic=Firmware Revision
            ///</summary>
            public string Firmware_Revision 
            { 
                get { return _Firmware_Revision; }
                set { if (value == _Firmware_Revision) return; _Firmware_Revision = value; OnPropertyChanged();}
            }

            private string _Software_Revision = "";
            /// <summary>
            /// Software_Revision (STRING ) from Service=Device Info and Characteristic=Software Revision
            ///</summary>
            public string Software_Revision 
            { 
                get { return _Software_Revision; }
                set { if (value == _Software_Revision) return; _Software_Revision = value; OnPropertyChanged();}
            }

            private string _Hardware_Revision = "";
            /// <summary>
            /// Hardware_Revision (STRING ) from Service=Device Info and Characteristic=Hardware Revision
            ///</summary>
            public string Hardware_Revision 
            { 
                get { return _Hardware_Revision; }
                set { if (value == _Hardware_Revision) return; _Hardware_Revision = value; OnPropertyChanged();}
            }

            private string _Serial_Number = "";
            /// <summary>
            /// Serial_Number (STRING ) from Service=Device Info and Characteristic=Serial Number
            ///</summary>
            public string Serial_Number 
            { 
                get { return _Serial_Number; }
                set { if (value == _Serial_Number) return; _Serial_Number = value; OnPropertyChanged();}
            }
            public override Device_Info_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as Device_Info_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(Device_Info_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.Manufacturer_Name = source.Manufacturer_Name;
                dest.Model_Number = source.Model_Number;
                dest.Firmware_Revision = source.Firmware_Revision;
                dest.Software_Revision = source.Software_Revision;
                dest.Hardware_Revision = source.Hardware_Revision;
                dest.Serial_Number = source.Serial_Number;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static Device_Info_Data CopyToWithConvertAndCreate(Device_Info_Data source, Device_Info_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.Manufacturer_Name = source.Manufacturer_Name;
                dest.Model_Number = source.Model_Number;
                dest.Firmware_Revision = source.Firmware_Revision;
                dest.Software_Revision = source.Software_Revision;
                dest.Hardware_Revision = source.Hardware_Revision;
                dest.Serial_Number = source.Serial_Number;
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["Manufacturer_Name", "Model_Number", "Firmware_Revision", "Software_Revision", "Hardware_Revision", "Serial_Number"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(Manufacturer_Name);
                exporter.CellSet(Model_Number);
                exporter.CellSet(Firmware_Revision);
                exporter.CellSet(Software_Revision);
                exporter.CellSet(Hardware_Revision);
                exporter.CellSet(Serial_Number);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {Manufacturer_Name} {Model_Number} {Firmware_Revision} {Software_Revision} {Hardware_Revision} {Serial_Number}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the Service_Status Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class Service_Status_Data :BTCommonMetaData<Service_Status_Data> //, IExportDataSource
        {
            private double _FirmwareMajor = 0;
            /// <summary>
            /// FirmwareMajor (U8 ) from Service=Service_Status and Characteristic=Status_Device
            ///</summary>
            public double FirmwareMajor 
            { 
                get { return _FirmwareMajor; }
                set { if (value == _FirmwareMajor) return; _FirmwareMajor = value; OnPropertyChanged();}
            }
            private double _FirmwareMinor = 0;
            /// <summary>
            /// FirmwareMinor (U8 ) from Service=Service_Status and Characteristic=Status_Device
            ///</summary>
            public double FirmwareMinor 
            { 
                get { return _FirmwareMinor; }
                set { if (value == _FirmwareMinor) return; _FirmwareMinor = value; OnPropertyChanged();}
            }
            private double _MaxInputVoltage = 0;
            /// <summary>
            /// MaxInputVoltage (U16 ) from Service=Service_Status and Characteristic=Status_Device
            ///</summary>
            public double MaxInputVoltage 
            { 
                get { return _MaxInputVoltage; }
                set { if (value == _MaxInputVoltage) return; _MaxInputVoltage = value; OnPropertyChanged();}
            }
            private double _MaxInputCurrent = 0;
            /// <summary>
            /// MaxInputCurrent (U16 ) from Service=Service_Status and Characteristic=Status_Device
            ///</summary>
            public double MaxInputCurrent 
            { 
                get { return _MaxInputCurrent; }
                set { if (value == _MaxInputCurrent) return; _MaxInputCurrent = value; OnPropertyChanged();}
            }
            private double _MaxInputResistance = 0;
            /// <summary>
            /// MaxInputResistance (U16 ) from Service=Service_Status and Characteristic=Status_Device
            ///</summary>
            public double MaxInputResistance 
            { 
                get { return _MaxInputResistance; }
                set { if (value == _MaxInputResistance) return; _MaxInputResistance = value; OnPropertyChanged();}
            }
            private double _MaxSamplingRate = 0;
            /// <summary>
            /// MaxSamplingRate (U16 ) from Service=Service_Status and Characteristic=Status_Device
            ///</summary>
            public double MaxSamplingRate 
            { 
                get { return _MaxSamplingRate; }
                set { if (value == _MaxSamplingRate) return; _MaxSamplingRate = value; OnPropertyChanged();}
            }
            private double _DeviceBufferSize = 0;
            /// <summary>
            /// DeviceBufferSize (U16 ) from Service=Service_Status and Characteristic=Status_Device
            ///</summary>
            public double DeviceBufferSize 
            { 
                get { return _DeviceBufferSize; }
                set { if (value == _DeviceBufferSize) return; _DeviceBufferSize = value; OnPropertyChanged();}
            }
            private double _Reserved01 = 0;
            /// <summary>
            /// Reserved01 (U16 ) from Service=Service_Status and Characteristic=Status_Device
            ///</summary>
            public double Reserved01 
            { 
                get { return _Reserved01; }
                set { if (value == _Reserved01) return; _Reserved01 = value; OnPropertyChanged();}
            }
            private byte[] _MacAddress = null;
            /// <summary>
            /// MacAddress (BYTES ) from Service=Service_Status and Characteristic=Status_Device
            ///</summary>
            public byte[] MacAddress 
            { 
                get { return _MacAddress; }
                set { if (value == _MacAddress) return; _MacAddress = value; OnPropertyChanged();}
            }

            private double _DeviceStatus = 0;
            /// <summary>
            /// DeviceStatus (U8 ) from Service=Service_Status and Characteristic=Status_Status
            ///</summary>
            public double DeviceStatus 
            { 
                get { return _DeviceStatus; }
                set { if (value == _DeviceStatus) return; _DeviceStatus = value; OnPropertyChanged();}
            }
            private double _BatteryLevel = 0.0;
            /// <summary>
            /// BatteryLevel (F32 ) from Service=Service_Status and Characteristic=Status_Status
            ///</summary>
            public double BatteryLevel 
            { 
                get { return _BatteryLevel; }
                set { if (value == _BatteryLevel) return; _BatteryLevel = value; OnPropertyChanged();}
            }

            private string _Device_Name = "";
            /// <summary>
            /// Device_Name (STRING ) from Service=Service_Status and Characteristic=Status_Device_Name
            ///</summary>
            public string Device_Name 
            { 
                get { return _Device_Name; }
                set { if (value == _Device_Name) return; _Device_Name = value; OnPropertyChanged();}
            }

            private double _Red = 0;
            /// <summary>
            /// Red (U8 ) from Service=Service_Status and Characteristic=Status_Flash_LED
            ///</summary>
            public double Red 
            { 
                get { return _Red; }
                set { if (value == _Red) return; _Red = value; OnPropertyChanged();}
            }
            private double _Green = 0;
            /// <summary>
            /// Green (U8 ) from Service=Service_Status and Characteristic=Status_Flash_LED
            ///</summary>
            public double Green 
            { 
                get { return _Green; }
                set { if (value == _Green) return; _Green = value; OnPropertyChanged();}
            }
            private double _Blue = 0;
            /// <summary>
            /// Blue (U8 ) from Service=Service_Status and Characteristic=Status_Flash_LED
            ///</summary>
            public double Blue 
            { 
                get { return _Blue; }
                set { if (value == _Blue) return; _Blue = value; OnPropertyChanged();}
            }
            private double _Beep = 0;
            /// <summary>
            /// Beep (U8 ) from Service=Service_Status and Characteristic=Status_Flash_LED
            ///</summary>
            public double Beep 
            { 
                get { return _Beep; }
                set { if (value == _Beep) return; _Beep = value; OnPropertyChanged();}
            }

            private double _Light = 0;
            /// <summary>
            /// Light (U8 ) from Service=Service_Status and Characteristic=Status_Light_LED
            ///</summary>
            public double Light 
            { 
                get { return _Light; }
                set { if (value == _Light) return; _Light = value; OnPropertyChanged();}
            }

            private byte[] _StatusUnknown5 = null;
            /// <summary>
            /// StatusUnknown5 (BYTES ) from Service=Service_Status and Characteristic=UnknownStatusValues
            ///</summary>
            public byte[] StatusUnknown5 
            { 
                get { return _StatusUnknown5; }
                set { if (value == _StatusUnknown5) return; _StatusUnknown5 = value; OnPropertyChanged();}
            }
            public override Service_Status_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as Service_Status_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(Service_Status_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.FirmwareMajor = source.FirmwareMajor;
                dest.FirmwareMinor = source.FirmwareMinor;
                dest.MaxInputVoltage = source.MaxInputVoltage;
                dest.MaxInputCurrent = source.MaxInputCurrent;
                dest.MaxInputResistance = source.MaxInputResistance;
                dest.MaxSamplingRate = source.MaxSamplingRate;
                dest.DeviceBufferSize = source.DeviceBufferSize;
                dest.Reserved01 = source.Reserved01;
                dest.MacAddress = source.MacAddress;
                dest.DeviceStatus = source.DeviceStatus;
                dest.BatteryLevel = source.BatteryLevel;
                dest.Device_Name = source.Device_Name;
                dest.Red = source.Red;
                dest.Green = source.Green;
                dest.Blue = source.Blue;
                dest.Beep = source.Beep;
                dest.Light = source.Light;
                dest.StatusUnknown5 = source.StatusUnknown5;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static Service_Status_Data CopyToWithConvertAndCreate(Service_Status_Data source, Service_Status_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.FirmwareMajor = convert(source.FirmwareMajor, "");
                dest.FirmwareMinor = convert(source.FirmwareMinor, "");
                dest.MaxInputVoltage = convert(source.MaxInputVoltage, "");
                dest.MaxInputCurrent = convert(source.MaxInputCurrent, "");
                dest.MaxInputResistance = convert(source.MaxInputResistance, "");
                dest.MaxSamplingRate = convert(source.MaxSamplingRate, "");
                dest.DeviceBufferSize = convert(source.DeviceBufferSize, "");
                dest.Reserved01 = convert(source.Reserved01, "");
                dest.MacAddress = source.MacAddress;
                dest.DeviceStatus = convert(source.DeviceStatus, "");
                dest.BatteryLevel = convert(source.BatteryLevel, "");
                dest.Device_Name = source.Device_Name;
                dest.Red = convert(source.Red, "");
                dest.Green = convert(source.Green, "");
                dest.Blue = convert(source.Blue, "");
                dest.Beep = convert(source.Beep, "");
                dest.Light = convert(source.Light, "");
                dest.StatusUnknown5 = source.StatusUnknown5;
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["FirmwareMajor", "FirmwareMinor", "MaxInputVoltage", "MaxInputCurrent", "MaxInputResistance", "MaxSamplingRate", "DeviceBufferSize", "Reserved01", "MacAddress", "DeviceStatus", "BatteryLevel", "Device_Name", "Red", "Green", "Blue", "Beep", "Light", "StatusUnknown5"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(FirmwareMajor);
                exporter.CellSet(FirmwareMinor);
                exporter.CellSet(MaxInputVoltage);
                exporter.CellSet(MaxInputCurrent);
                exporter.CellSet(MaxInputResistance);
                exporter.CellSet(MaxSamplingRate);
                exporter.CellSet(DeviceBufferSize);
                exporter.CellSet(Reserved01);
                exporter.CellSet(MacAddress);
                exporter.CellSet(DeviceStatus);
                exporter.CellSet(BatteryLevel);
                exporter.CellSet(Device_Name);
                exporter.CellSet(Red);
                exporter.CellSet(Green);
                exporter.CellSet(Blue);
                exporter.CellSet(Beep);
                exporter.CellSet(Light);
                exporter.CellSet(StatusUnknown5);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {FirmwareMajor} {FirmwareMinor} {MaxInputVoltage} {MaxInputCurrent} {MaxInputResistance} {MaxSamplingRate} {DeviceBufferSize} {Reserved01} {MacAddress} {DeviceStatus} {BatteryLevel} {Device_Name} {Red} {Green} {Blue} {Beep} {Light} {StatusUnknown5}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the CalibrationService Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class CalibrationService_Data :BTCommonMetaData<CalibrationService_Data> //, IExportDataSource
        {
            private byte[] _CalibrateUnknown0 = null;
            /// <summary>
            /// CalibrateUnknown0 (BYTES ) from Service=CalibrationService and Characteristic=CalbrateTemperature
            ///</summary>
            public byte[] CalibrateUnknown0 
            { 
                get { return _CalibrateUnknown0; }
                set { if (value == _CalibrateUnknown0) return; _CalibrateUnknown0 = value; OnPropertyChanged();}
            }

            private byte[] _CalibrateUnknown1 = null;
            /// <summary>
            /// CalibrateUnknown1 (BYTES ) from Service=CalibrationService and Characteristic=CalibrateUnknown1
            ///</summary>
            public byte[] CalibrateUnknown1 
            { 
                get { return _CalibrateUnknown1; }
                set { if (value == _CalibrateUnknown1) return; _CalibrateUnknown1 = value; OnPropertyChanged();}
            }

            private byte[] _CalibrrateUnknown2 = null;
            /// <summary>
            /// CalibrrateUnknown2 (BYTES ) from Service=CalibrationService and Characteristic=CalibrrateUnknown2
            ///</summary>
            public byte[] CalibrrateUnknown2 
            { 
                get { return _CalibrrateUnknown2; }
                set { if (value == _CalibrrateUnknown2) return; _CalibrrateUnknown2 = value; OnPropertyChanged();}
            }
            public override CalibrationService_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as CalibrationService_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(CalibrationService_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.CalibrateUnknown0 = source.CalibrateUnknown0;
                dest.CalibrateUnknown1 = source.CalibrateUnknown1;
                dest.CalibrrateUnknown2 = source.CalibrrateUnknown2;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static CalibrationService_Data CopyToWithConvertAndCreate(CalibrationService_Data source, CalibrationService_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.CalibrateUnknown0 = source.CalibrateUnknown0;
                dest.CalibrateUnknown1 = source.CalibrateUnknown1;
                dest.CalibrrateUnknown2 = source.CalibrrateUnknown2;
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["CalibrateUnknown0", "CalibrateUnknown1", "CalibrrateUnknown2"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(CalibrateUnknown0);
                exporter.CellSet(CalibrateUnknown1);
                exporter.CellSet(CalibrrateUnknown2);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {CalibrateUnknown0} {CalibrateUnknown1} {CalibrrateUnknown2}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the Silabs_Service_OTA Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class Silabs_Service_OTA_Data :BTCommonMetaData<Silabs_Service_OTA_Data> //, IExportDataSource
        {
            private byte[] _Unknown0 = null;
            /// <summary>
            /// Unknown0 (BYTES ) from Service=Silabs_Service_OTA and Characteristic=OTA_Control
            ///</summary>
            public byte[] Unknown0 
            { 
                get { return _Unknown0; }
                set { if (value == _Unknown0) return; _Unknown0 = value; OnPropertyChanged();}
            }

            private byte[] _Unknown1 = null;
            /// <summary>
            /// Unknown1 (BYTES ) from Service=Silabs_Service_OTA and Characteristic=OTA_Data
            ///</summary>
            public byte[] Unknown1 
            { 
                get { return _Unknown1; }
                set { if (value == _Unknown1) return; _Unknown1 = value; OnPropertyChanged();}
            }
            public override Silabs_Service_OTA_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as Silabs_Service_OTA_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(Silabs_Service_OTA_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.Unknown0 = source.Unknown0;
                dest.Unknown1 = source.Unknown1;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static Silabs_Service_OTA_Data CopyToWithConvertAndCreate(Silabs_Service_OTA_Data source, Silabs_Service_OTA_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.Unknown0 = source.Unknown0;
                dest.Unknown1 = source.Unknown1;
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["Unknown0", "Unknown1"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(Unknown0);
                exporter.CellSet(Unknown1);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {Unknown0} {Unknown1}");
            }
        }
//


        #endregion


        /// <summary>
        /// Enumeration of all services
        /// </summary>
        enum ServiceIndex
        {
            Multimeter_index = 0,
            DSO_Oscilloscope_index = 1,
            DataLogger_Dlog_index = 2,
            Generic_Service_index = 3,
            Common_Configuration_index = 4,
            Device_Info_index = 5,
            Service_Status_index = 6,
            CalibrationService_index = 7,
            Silabs_Service_OTA_index = 8,
        }

        /// <summary>
        /// Enumeration of all characteristics in all of the services.
        /// </summary>
        enum CharacteristicIndex
        {
            Multimeter_MM_Settings_index = 0,     // GUID 53dc9a7a-bc19-4280-b76b-002d0e23b078
            Multimeter_MM_Data_index = 1,     // GUID 047d3559-8bee-423a-b229-4417fa603b90
            DSO_Oscilloscope_DSO_Settings_index = 2,     // GUID a81af1b6-b8b3-4244-8859-3da368d2be39
            DSO_Oscilloscope_DSO_Reading_index = 3,     // GUID 98e14f8e-536e-4f24-b4f4-1debfed0a99e
            DSO_Oscilloscope_DSO_Metadata_index = 4,     // GUID 970f00ba-f46f-4825-96a8-153a5cd0cda9
            DataLogger_Dlog_DataLogger_Settings_index = 5,     // GUID 5f97c62b-a83b-46c6-b9cd-cac59e130a78
            DataLogger_Dlog_DataLogger_Reading_index = 6,     // GUID 3c669dab-fc86-411c-9498-4f9415049cc0
            DataLogger_Dlog_DataLogger_MetaData_index = 7,     // GUID 9acada2e-3936-430b-a8f7-da407d97ca6e
            Generic_Service_Server_Supported_Features_index = 8,     // GUID 00002803-0000-1000-8000-00805f9b34fb
            Generic_Service_Service_Changes_index = 9,     // GUID 00002a05-0000-1000-8000-00805f9b34fb
            Generic_Service_Client_Supported_Features_index = 10,     // GUID 00002b29-0000-1000-8000-00805f9b34fb
            Generic_Service_Database_Hash_index = 11,     // GUID 00002b2a-0000-1000-8000-00805f9b34fb
            Common_Configuration_Device_Name_index = 12,     // GUID 00002a00-0000-1000-8000-00805f9b34fb
            Common_Configuration_Appearance_index = 13,     // GUID 00002a01-0000-1000-8000-00805f9b34fb
            Device_Info_Manufacturer_Name_index = 14,     // GUID 00002a29-0000-1000-8000-00805f9b34fb
            Device_Info_Model_Number_index = 15,     // GUID 00002a24-0000-1000-8000-00805f9b34fb
            Device_Info_Firmware_Revision_index = 16,     // GUID 00002a26-0000-1000-8000-00805f9b34fb
            Device_Info_Software_Revision_index = 17,     // GUID 00002a28-0000-1000-8000-00805f9b34fb
            Device_Info_Hardware_Revision_index = 18,     // GUID 00002a27-0000-1000-8000-00805f9b34fb
            Device_Info_Serial_Number_index = 19,     // GUID 00002a25-0000-1000-8000-00805f9b34fb
            Service_Status_Status_Device_index = 20,     // GUID 6974f5e5-0e54-45c3-97dd-29e4b5fb0849
            Service_Status_Status_Status_index = 21,     // GUID 3dba36e1-6120-4706-8dfd-ed9c16e569b6
            Service_Status_Status_Device_Name_index = 22,     // GUID 7f0375de-077e-4555-8f78-800494509cc3
            Service_Status_Status_Flash_LED_index = 23,     // GUID ec9bb1f3-05a9-4277-8dd0-60a7896f0d6e
            Service_Status_Status_Light_LED_index = 24,     // GUID aaf3f6d5-43d4-4a83-9510-dff3d858d4cc
            Service_Status_UnknownStatusValues_index = 25,     // GUID 8fe5b5a9-b5b4-4a7b-8ff2-87224b970f89
            CalibrationService_CalbrateTemperature_index = 26,     // GUID 0cd0f713-f5aa-4572-9e23-f8049f6bcaaa
            CalibrationService_CalibrateUnknown1_index = 27,     // GUID b6728f91-409c-4d6c-864e-272a6a7a0204
            CalibrationService_CalibrrateUnknown2_index = 28,     // GUID 5588e47b-cb81-4f7b-acc4-6029a3f39f72
            Silabs_Service_OTA_OTA_Control_index = 29,     // GUID f7bf3564-fb6d-4e53-88a4-5e37e0326063
            Silabs_Service_OTA_OTA_Data_index = 30,     // GUID 984227f3-34fc-4045-a5d0-2c581f81a153
        }

        // All of the services that this device supports
        /// <summary>
        /// Convenience GUID for Multimeter service. 
        /// </summary>
        public static readonly Guid ServiceGuid_Multimeter = Guid.Parse("e7481d2f-5781-442e-bb9a-fd4e3441dadc"); // #0 is Multimeter
        /// <summary>
        /// Convenience GUID for DSO_Oscilloscope service. 
        /// </summary>
        public static readonly Guid ServiceGuid_DSO_Oscilloscope = Guid.Parse("1569801e-1425-4a7a-b617-a4f4ed719de6"); // #1 is DSO_Oscilloscope
        /// <summary>
        /// Convenience GUID for DataLogger_Dlog service. 
        /// </summary>
        public static readonly Guid ServiceGuid_DataLogger_Dlog = Guid.Parse("a5ff3566-1fd8-4e10-8362-590a578a4121"); // #2 is DataLogger_Dlog
        /// <summary>
        /// Convenience GUID for Generic Service service. 
        /// </summary>
        public static readonly Guid ServiceGuid_Generic_Service = Guid.Parse("00001801-0000-1000-8000-00805f9b34fb"); // #3 is Generic Service
        /// <summary>
        /// Convenience GUID for Common Configuration service. 
        /// </summary>
        public static readonly Guid ServiceGuid_Common_Configuration = Guid.Parse("00001800-0000-1000-8000-00805f9b34fb"); // #4 is Common Configuration
        /// <summary>
        /// Convenience GUID for Device Info service. 
        /// </summary>
        public static readonly Guid ServiceGuid_Device_Info = Guid.Parse("0000180a-0000-1000-8000-00805f9b34fb"); // #5 is Device Info
        /// <summary>
        /// Convenience GUID for Service_Status service. 
        /// </summary>
        public static readonly Guid ServiceGuid_Service_Status = Guid.Parse("57d3a771-267c-4394-8872-78223e92aec5"); // #6 is Service_Status
        /// <summary>
        /// Convenience GUID for CalibrationService service. 
        /// </summary>
        public static readonly Guid ServiceGuid_CalibrationService = Guid.Parse("6f53be2f-780b-49b8-a7c3-e8a052b3ae2c"); // #7 is CalibrationService
        /// <summary>
        /// Convenience GUID for Silabs_Service_OTA service. 
        /// </summary>
        public static readonly Guid ServiceGuid_Silabs_Service_OTA = Guid.Parse("1d14d6ee-fd63-4fa1-bfa4-8f47b42119f0"); // #8 is Silabs_Service_OTA        

        /// <summary>
        /// List of the guids supported by the device. 
        /// </summary>
        List<Guid> Service_Guids = new List<Guid>()
        {
            Guid.Parse("e7481d2f-5781-442e-bb9a-fd4e3441dadc"), // #0 is Multimeter
            Guid.Parse("1569801e-1425-4a7a-b617-a4f4ed719de6"), // #1 is DSO_Oscilloscope
            Guid.Parse("a5ff3566-1fd8-4e10-8362-590a578a4121"), // #2 is DataLogger_Dlog
            Guid.Parse("00001801-0000-1000-8000-00805f9b34fb"), // #3 is Generic Service
            Guid.Parse("00001800-0000-1000-8000-00805f9b34fb"), // #4 is Common Configuration
            Guid.Parse("0000180a-0000-1000-8000-00805f9b34fb"), // #5 is Device Info
            Guid.Parse("57d3a771-267c-4394-8872-78223e92aec5"), // #6 is Service_Status
            Guid.Parse("6f53be2f-780b-49b8-a7c3-e8a052b3ae2c"), // #7 is CalibrationService
            Guid.Parse("1d14d6ee-fd63-4fa1-bfa4-8f47b42119f0"), // #8 is Silabs_Service_OTA
        };

        /// <summary>
        /// Active list of services. Will be filled in as the services are connected. Starts off as null.
        /// </summary>
        List<GattDeviceService> Services = new List<GattDeviceService>() { null, null, null, null, null, null, null, null, null, };

        /// <summary>
        /// List of the Characteristic GUIDS for all of the characteristics for all of the services.
        /// Is indexed by the CharacteristicIndex enum. 
        /// </summary>
        List<Guid> Characteristic_Guids = new List<Guid>()
        {
            Guid.Parse("53dc9a7a-bc19-4280-b76b-002d0e23b078"), // #0 is Multimeter MM_Settings
            Guid.Parse("047d3559-8bee-423a-b229-4417fa603b90"), // #1 is Multimeter MM_Data
            Guid.Parse("a81af1b6-b8b3-4244-8859-3da368d2be39"), // #2 is DSO_Oscilloscope DSO_Settings
            Guid.Parse("98e14f8e-536e-4f24-b4f4-1debfed0a99e"), // #3 is DSO_Oscilloscope DSO_Reading
            Guid.Parse("970f00ba-f46f-4825-96a8-153a5cd0cda9"), // #4 is DSO_Oscilloscope DSO_Metadata
            Guid.Parse("5f97c62b-a83b-46c6-b9cd-cac59e130a78"), // #5 is DataLogger_Dlog DataLogger_Settings
            Guid.Parse("3c669dab-fc86-411c-9498-4f9415049cc0"), // #6 is DataLogger_Dlog DataLogger_Reading
            Guid.Parse("9acada2e-3936-430b-a8f7-da407d97ca6e"), // #7 is DataLogger_Dlog DataLogger_MetaData
            Guid.Parse("00002803-0000-1000-8000-00805f9b34fb"), // #8 is Generic Service Server_Supported_Features
            Guid.Parse("00002a05-0000-1000-8000-00805f9b34fb"), // #9 is Generic Service Service Changes
            Guid.Parse("00002b29-0000-1000-8000-00805f9b34fb"), // #10 is Generic Service Client_Supported_Features
            Guid.Parse("00002b2a-0000-1000-8000-00805f9b34fb"), // #11 is Generic Service Database Hash
            Guid.Parse("00002a00-0000-1000-8000-00805f9b34fb"), // #12 is Common Configuration Device Name
            Guid.Parse("00002a01-0000-1000-8000-00805f9b34fb"), // #13 is Common Configuration Appearance
            Guid.Parse("00002a29-0000-1000-8000-00805f9b34fb"), // #14 is Device Info Manufacturer Name
            Guid.Parse("00002a24-0000-1000-8000-00805f9b34fb"), // #15 is Device Info Model Number
            Guid.Parse("00002a26-0000-1000-8000-00805f9b34fb"), // #16 is Device Info Firmware Revision
            Guid.Parse("00002a28-0000-1000-8000-00805f9b34fb"), // #17 is Device Info Software Revision
            Guid.Parse("00002a27-0000-1000-8000-00805f9b34fb"), // #18 is Device Info Hardware Revision
            Guid.Parse("00002a25-0000-1000-8000-00805f9b34fb"), // #19 is Device Info Serial Number
            Guid.Parse("6974f5e5-0e54-45c3-97dd-29e4b5fb0849"), // #20 is Service_Status Status_Device
            Guid.Parse("3dba36e1-6120-4706-8dfd-ed9c16e569b6"), // #21 is Service_Status Status_Status
            Guid.Parse("7f0375de-077e-4555-8f78-800494509cc3"), // #22 is Service_Status Status_Device_Name
            Guid.Parse("ec9bb1f3-05a9-4277-8dd0-60a7896f0d6e"), // #23 is Service_Status Status_Flash_LED
            Guid.Parse("aaf3f6d5-43d4-4a83-9510-dff3d858d4cc"), // #24 is Service_Status Status_Light_LED
            Guid.Parse("8fe5b5a9-b5b4-4a7b-8ff2-87224b970f89"), // #25 is Service_Status UnknownStatusValues
            Guid.Parse("0cd0f713-f5aa-4572-9e23-f8049f6bcaaa"), // #26 is CalibrationService CalbrateTemperature
            Guid.Parse("b6728f91-409c-4d6c-864e-272a6a7a0204"), // #27 is CalibrationService CalibrateUnknown1
            Guid.Parse("5588e47b-cb81-4f7b-acc4-6029a3f39f72"), // #28 is CalibrationService CalibrrateUnknown2
            Guid.Parse("f7bf3564-fb6d-4e53-88a4-5e37e0326063"), // #29 is Silabs_Service_OTA OTA_Control
            Guid.Parse("984227f3-34fc-4045-a5d0-2c581f81a153"), // #30 is Silabs_Service_OTA OTA_Data
        };

        private List<GattCharacteristic> Characteristics = new List<GattCharacteristic>() { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,  };
        private List<bool> NotifyCharacteristic_ValueChanged_set = new List<bool> { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,  };
        private List<IotNumberFormats.ValueParser> ValueParsers = new List<IotNumberFormats.ValueParser>() {  null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,  };


        /// <summary>
        /// Delegate for all Notify events. this is specific to this device (the indexes are all for this device only)
        /// but otherwise is generic.
        /// </summary>
        /// <param name="data"></param>
        public delegate void BluetoothDataEvent(IotNumberFormats.ValueParserResult data);

        async Task<bool> Ensure_Characteristic_Async(ServiceIndex serviceIndex, string serviceName, CharacteristicIndex characteristicIndex, string characteristicName)
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


        #region Service_Multimeter
        // Service Multimeter 

        public Multimeter_Data CurrMultimeter_Data { get; set; } = new Multimeter_Data();

        // Per-characteristics methods for Multimeter MM_Settings
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyMM_SettingsAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("MM_Settings", ServiceIndex.Multimeter_index, "Multimeter", CharacteristicIndex.Multimeter_MM_Settings_index, NotifyMM_SettingsCallback, notifyType);
            return retval;
        }

        private void NotifyMM_SettingsCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Multimeter_MM_Settings_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|DEC|ModeSetting U8|DEC|RangeSetting U32|DEC|IntervalSetting|ms");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrMultimeter_Data.TimestampMostRecent = args.Timestamp;
            CurrMultimeter_Data.ModeSetting = vr.GetNextDouble();
            CurrMultimeter_Data.RangeSetting = vr.GetNextDouble();
            CurrMultimeter_Data.IntervalSetting = vr.GetNextDouble();
            OnPropertyChanged(MM_SettingsPropertyChangedName); // "MM_Settings"
        }
        // Per-characteristics methods for Multimeter MM_Data
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyMM_DataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("MM_Data", ServiceIndex.Multimeter_index, "Multimeter", CharacteristicIndex.Multimeter_MM_Data_index, NotifyMM_DataCallback, notifyType);
            return retval;
        }

        private void NotifyMM_DataCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Multimeter_MM_Data_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|DEC|Status F32|FIXED|Data U8|DEC|OperationMode U8|HEX|Range");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrMultimeter_Data.TimestampMostRecent = args.Timestamp;
            CurrMultimeter_Data.Status = vr.GetNextDouble();
            CurrMultimeter_Data.Data = vr.GetNextDouble();
            CurrMultimeter_Data.OperationMode = vr.GetNextDouble();
            CurrMultimeter_Data.Range = vr.GetNextDouble();
            OnPropertyChanged(MM_DataPropertyChangedName); // "MM_Data"
        }
        /// <summary>
        /// Reads data from MM_Settings and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Multimeter_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Multimeter_Data> ReadMM_Settings(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Multimeter_MM_Settings_index;
            await Ensure_Characteristic_Async(ServiceIndex.Multimeter_index, "Multimeter", index, "MM_Settings");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "MM_Settings", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|DEC|ModeSetting U8|DEC|RangeSetting U32|DEC|IntervalSetting|ms");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrMultimeter_Data.ModeSetting = vr.GetNextDouble();
            CurrMultimeter_Data.RangeSetting = vr.GetNextDouble();
            CurrMultimeter_Data.IntervalSetting = vr.GetNextDouble();
            CurrMultimeter_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(MM_SettingsPropertyChangedName); // "MM_Settings"
            return CurrMultimeter_Data;
        }
        /// <summary>
        /// Reads data from MM_Data and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Multimeter_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Multimeter_Data> ReadMM_Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Multimeter_MM_Data_index;
            await Ensure_Characteristic_Async(ServiceIndex.Multimeter_index, "Multimeter", index, "MM_Data");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "MM_Data", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|DEC|Status F32|FIXED|Data U8|DEC|OperationMode U8|HEX|Range");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrMultimeter_Data.Status = vr.GetNextDouble();
            CurrMultimeter_Data.Data = vr.GetNextDouble();
            CurrMultimeter_Data.OperationMode = vr.GetNextDouble();
            CurrMultimeter_Data.Range = vr.GetNextDouble();
            CurrMultimeter_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(MM_DataPropertyChangedName); // "MM_Data"
            return CurrMultimeter_Data;
        }
        /// <summary>
        /// Writes data to MM_Settings 
        /// </summary>
        public async Task<GattCommunicationStatus?> WriteMM_Settings(byte[] data)
        {
            var index = CharacteristicIndex.Multimeter_MM_Settings_index;
            await Ensure_Characteristic_Async(ServiceIndex.Multimeter_index, "Multimeter", index, "MM_Settings");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteMM_Settings", result);
            return result;
        }

        #endregion
//
        #region Service_DSO_Oscilloscope
        // Service DSO_Oscilloscope 

        public DSO_Oscilloscope_Data CurrDSO_Oscilloscope_Data { get; set; } = new DSO_Oscilloscope_Data();

        // Per-characteristics methods for DSO_Oscilloscope DSO_Settings
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyDSO_SettingsAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("DSO_Settings", ServiceIndex.DSO_Oscilloscope_index, "DSO_Oscilloscope", CharacteristicIndex.DSO_Oscilloscope_DSO_Settings_index, NotifyDSO_SettingsCallback, notifyType);
            return retval;
        }

        private void NotifyDSO_SettingsCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.DSO_Oscilloscope_DSO_Settings_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX|DsoTriggerType F32|FIXED|DsoTriggerLevel U8|HEX|DsoMode U8|HEX|DsoRange U32|DEC|DsoSamplingWindow|ms U16|DEC|DsoNSamples");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDSO_Oscilloscope_Data.TimestampMostRecent = args.Timestamp;
            CurrDSO_Oscilloscope_Data.DsoTriggerType = vr.GetNextDouble();
            CurrDSO_Oscilloscope_Data.DsoTriggerLevel = vr.GetNextDouble();
            CurrDSO_Oscilloscope_Data.DsoMode = vr.GetNextDouble();
            CurrDSO_Oscilloscope_Data.DsoRange = vr.GetNextDouble();
            CurrDSO_Oscilloscope_Data.DsoSamplingWindow = vr.GetNextDouble();
            CurrDSO_Oscilloscope_Data.DsoNSamples = vr.GetNextDouble();
            OnPropertyChanged(DSO_SettingsPropertyChangedName); // "DSO_Settings"
        }
        // Per-characteristics methods for DSO_Oscilloscope DSO_Reading
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyDSO_ReadingAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("DSO_Reading", ServiceIndex.DSO_Oscilloscope_index, "DSO_Oscilloscope", CharacteristicIndex.DSO_Oscilloscope_DSO_Reading_index, NotifyDSO_ReadingCallback, notifyType);
            return retval;
        }

        private void NotifyDSO_ReadingCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.DSO_Oscilloscope_DSO_Reading_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("I16S|DEC|DsoDataRaw");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDSO_Oscilloscope_Data.TimestampMostRecent = args.Timestamp;
            CurrDSO_Oscilloscope_Data.DsoDataRaw = vr.GetNextDoubleArray();
            OnPropertyChanged(DSO_ReadingPropertyChangedName); // "DSO_Reading"
        }
        // Per-characteristics methods for DSO_Oscilloscope DSO_Metadata
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyDSO_MetadataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("DSO_Metadata", ServiceIndex.DSO_Oscilloscope_index, "DSO_Oscilloscope", CharacteristicIndex.DSO_Oscilloscope_DSO_Metadata_index, NotifyDSO_MetadataCallback, notifyType);
            return retval;
        }

        private void NotifyDSO_MetadataCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.DSO_Oscilloscope_DSO_Metadata_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX|DsoStatus F32|FIXED|DsoDataScale U8|HEX|DsoDataMode U8|HEX|DsoDataRange U32|DEC|DsoDataSamplingWindow U16|DEC|DsoDataNsamples U32|DEC|DsoSamplingRate|hz");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDSO_Oscilloscope_Data.TimestampMostRecent = args.Timestamp;
            CurrDSO_Oscilloscope_Data.DsoStatus = vr.GetNextDouble();
            CurrDSO_Oscilloscope_Data.DsoDataScale = vr.GetNextDouble();
            CurrDSO_Oscilloscope_Data.DsoDataMode = vr.GetNextDouble();
            CurrDSO_Oscilloscope_Data.DsoDataRange = vr.GetNextDouble();
            CurrDSO_Oscilloscope_Data.DsoDataSamplingWindow = vr.GetNextDouble();
            CurrDSO_Oscilloscope_Data.DsoDataNsamples = vr.GetNextDouble();
            CurrDSO_Oscilloscope_Data.DsoSamplingRate = vr.GetNextDouble();
            OnPropertyChanged(DSO_MetadataPropertyChangedName); // "DSO_Metadata"
        }
        /// <summary>
        /// Reads data from DSO_Settings and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>DSO_Oscilloscope_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<DSO_Oscilloscope_Data> ReadDSO_Settings(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.DSO_Oscilloscope_DSO_Settings_index;
            await Ensure_Characteristic_Async(ServiceIndex.DSO_Oscilloscope_index, "DSO_Oscilloscope", index, "DSO_Settings");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "DSO_Settings", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX|DsoTriggerType F32|FIXED|DsoTriggerLevel U8|HEX|DsoMode U8|HEX|DsoRange U32|DEC|DsoSamplingWindow|ms U16|DEC|DsoNSamples");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDSO_Oscilloscope_Data.DsoTriggerType = vr.GetNextDouble();
            CurrDSO_Oscilloscope_Data.DsoTriggerLevel = vr.GetNextDouble();
            CurrDSO_Oscilloscope_Data.DsoMode = vr.GetNextDouble();
            CurrDSO_Oscilloscope_Data.DsoRange = vr.GetNextDouble();
            CurrDSO_Oscilloscope_Data.DsoSamplingWindow = vr.GetNextDouble();
            CurrDSO_Oscilloscope_Data.DsoNSamples = vr.GetNextDouble();
            CurrDSO_Oscilloscope_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(DSO_SettingsPropertyChangedName); // "DSO_Settings"
            return CurrDSO_Oscilloscope_Data;
        }
        /// <summary>
        /// Reads data from DSO_Reading and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>DSO_Oscilloscope_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<DSO_Oscilloscope_Data> ReadDSO_Reading(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.DSO_Oscilloscope_DSO_Reading_index;
            await Ensure_Characteristic_Async(ServiceIndex.DSO_Oscilloscope_index, "DSO_Oscilloscope", index, "DSO_Reading");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "DSO_Reading", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("I16S|DEC|DsoDataRaw");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDSO_Oscilloscope_Data.DsoDataRaw = vr.GetNextDoubleArray();
            CurrDSO_Oscilloscope_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(DSO_ReadingPropertyChangedName); // "DSO_Reading"
            return CurrDSO_Oscilloscope_Data;
        }
        /// <summary>
        /// Reads data from DSO_Metadata and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>DSO_Oscilloscope_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<DSO_Oscilloscope_Data> ReadDSO_Metadata(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.DSO_Oscilloscope_DSO_Metadata_index;
            await Ensure_Characteristic_Async(ServiceIndex.DSO_Oscilloscope_index, "DSO_Oscilloscope", index, "DSO_Metadata");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "DSO_Metadata", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX|DsoStatus F32|FIXED|DsoDataScale U8|HEX|DsoDataMode U8|HEX|DsoDataRange U32|DEC|DsoDataSamplingWindow U16|DEC|DsoDataNsamples U32|DEC|DsoSamplingRate|hz");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDSO_Oscilloscope_Data.DsoStatus = vr.GetNextDouble();
            CurrDSO_Oscilloscope_Data.DsoDataScale = vr.GetNextDouble();
            CurrDSO_Oscilloscope_Data.DsoDataMode = vr.GetNextDouble();
            CurrDSO_Oscilloscope_Data.DsoDataRange = vr.GetNextDouble();
            CurrDSO_Oscilloscope_Data.DsoDataSamplingWindow = vr.GetNextDouble();
            CurrDSO_Oscilloscope_Data.DsoDataNsamples = vr.GetNextDouble();
            CurrDSO_Oscilloscope_Data.DsoSamplingRate = vr.GetNextDouble();
            CurrDSO_Oscilloscope_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(DSO_MetadataPropertyChangedName); // "DSO_Metadata"
            return CurrDSO_Oscilloscope_Data;
        }
        /// <summary>
        /// Writes data to DSO_Settings 
        /// </summary>
        public async Task<GattCommunicationStatus?> WriteDSO_Settings(byte[] data)
        {
            var index = CharacteristicIndex.DSO_Oscilloscope_DSO_Settings_index;
            await Ensure_Characteristic_Async(ServiceIndex.DSO_Oscilloscope_index, "DSO_Oscilloscope", index, "DSO_Settings");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteDSO_Settings", result);
            return result;
        }

        #endregion
//
        #region Service_DataLogger_Dlog
        // Service DataLogger_Dlog 

        public DataLogger_Dlog_Data CurrDataLogger_Dlog_Data { get; set; } = new DataLogger_Dlog_Data();

        // Per-characteristics methods for DataLogger_Dlog DataLogger_Settings
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyDataLogger_SettingsAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("DataLogger_Settings", ServiceIndex.DataLogger_Dlog_index, "DataLogger_Dlog", CharacteristicIndex.DataLogger_Dlog_DataLogger_Settings_index, NotifyDataLogger_SettingsCallback, notifyType);
            return retval;
        }

        private void NotifyDataLogger_SettingsCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.DataLogger_Dlog_DataLogger_Settings_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX|DlogCommand U16|HEX|DlogReserved1 U8|HEX|DlogModeSetting U8|HEX|DlogRangeSetting U16|DEC|DlogUpdateInterval|s U32|DEC|DlogTimestamp");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDataLogger_Dlog_Data.TimestampMostRecent = args.Timestamp;
            CurrDataLogger_Dlog_Data.DlogCommand = vr.GetNextDouble();
            CurrDataLogger_Dlog_Data.DlogReserved1 = vr.GetNextDouble();
            CurrDataLogger_Dlog_Data.DlogModeSetting = vr.GetNextDouble();
            CurrDataLogger_Dlog_Data.DlogRangeSetting = vr.GetNextDouble();
            CurrDataLogger_Dlog_Data.DlogUpdateInterval = vr.GetNextDouble();
            CurrDataLogger_Dlog_Data.DlogTimestamp = vr.GetNextDouble();
            OnPropertyChanged(DataLogger_SettingsPropertyChangedName); // "DataLogger_Settings"
        }
        // Per-characteristics methods for DataLogger_Dlog DataLogger_Reading
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyDataLogger_ReadingAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("DataLogger_Reading", ServiceIndex.DataLogger_Dlog_index, "DataLogger_Dlog", CharacteristicIndex.DataLogger_Dlog_DataLogger_Reading_index, NotifyDataLogger_ReadingCallback, notifyType);
            return retval;
        }

        private void NotifyDataLogger_ReadingCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.DataLogger_Dlog_DataLogger_Reading_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|DlogData");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDataLogger_Dlog_Data.TimestampMostRecent = args.Timestamp;
            CurrDataLogger_Dlog_Data.DlogData = vr.GetNextByteArray();
            OnPropertyChanged(DataLogger_ReadingPropertyChangedName); // "DataLogger_Reading"
        }
        // Per-characteristics methods for DataLogger_Dlog DataLogger_MetaData
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyDataLogger_MetaDataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("DataLogger_MetaData", ServiceIndex.DataLogger_Dlog_index, "DataLogger_Dlog", CharacteristicIndex.DataLogger_Dlog_DataLogger_MetaData_index, NotifyDataLogger_MetaDataCallback, notifyType);
            return retval;
        }

        private void NotifyDataLogger_MetaDataCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.DataLogger_Dlog_DataLogger_MetaData_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX|DlogStatus F32|FIXED|DlogScale U8|HEx|DlogMode U8|HEX|DlogRange U16|DEC|DlogCurrLogging|us U16|DEC|DlogCurrNSample U32|DEC|DlogCurrTimestamp");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDataLogger_Dlog_Data.TimestampMostRecent = args.Timestamp;
            CurrDataLogger_Dlog_Data.DlogStatus = vr.GetNextDouble();
            CurrDataLogger_Dlog_Data.DlogScale = vr.GetNextDouble();
            CurrDataLogger_Dlog_Data.DlogMode = vr.GetNextDouble();
            CurrDataLogger_Dlog_Data.DlogRange = vr.GetNextDouble();
            CurrDataLogger_Dlog_Data.DlogCurrLogging = vr.GetNextDouble();
            CurrDataLogger_Dlog_Data.DlogCurrNSample = vr.GetNextDouble();
            CurrDataLogger_Dlog_Data.DlogCurrTimestamp = vr.GetNextDouble();
            OnPropertyChanged(DataLogger_MetaDataPropertyChangedName); // "DataLogger_MetaData"
        }
        /// <summary>
        /// Reads data from DataLogger_Settings and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>DataLogger_Dlog_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<DataLogger_Dlog_Data> ReadDataLogger_Settings(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.DataLogger_Dlog_DataLogger_Settings_index;
            await Ensure_Characteristic_Async(ServiceIndex.DataLogger_Dlog_index, "DataLogger_Dlog", index, "DataLogger_Settings");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "DataLogger_Settings", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX|DlogCommand U16|HEX|DlogReserved1 U8|HEX|DlogModeSetting U8|HEX|DlogRangeSetting U16|DEC|DlogUpdateInterval|s U32|DEC|DlogTimestamp");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDataLogger_Dlog_Data.DlogCommand = vr.GetNextDouble();
            CurrDataLogger_Dlog_Data.DlogReserved1 = vr.GetNextDouble();
            CurrDataLogger_Dlog_Data.DlogModeSetting = vr.GetNextDouble();
            CurrDataLogger_Dlog_Data.DlogRangeSetting = vr.GetNextDouble();
            CurrDataLogger_Dlog_Data.DlogUpdateInterval = vr.GetNextDouble();
            CurrDataLogger_Dlog_Data.DlogTimestamp = vr.GetNextDouble();
            CurrDataLogger_Dlog_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(DataLogger_SettingsPropertyChangedName); // "DataLogger_Settings"
            return CurrDataLogger_Dlog_Data;
        }
        /// <summary>
        /// Reads data from DataLogger_Reading and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>DataLogger_Dlog_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<DataLogger_Dlog_Data> ReadDataLogger_Reading(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.DataLogger_Dlog_DataLogger_Reading_index;
            await Ensure_Characteristic_Async(ServiceIndex.DataLogger_Dlog_index, "DataLogger_Dlog", index, "DataLogger_Reading");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "DataLogger_Reading", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|DlogData");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDataLogger_Dlog_Data.DlogData = vr.GetNextByteArray();
            CurrDataLogger_Dlog_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(DataLogger_ReadingPropertyChangedName); // "DataLogger_Reading"
            return CurrDataLogger_Dlog_Data;
        }
        /// <summary>
        /// Reads data from DataLogger_MetaData and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>DataLogger_Dlog_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<DataLogger_Dlog_Data> ReadDataLogger_MetaData(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.DataLogger_Dlog_DataLogger_MetaData_index;
            await Ensure_Characteristic_Async(ServiceIndex.DataLogger_Dlog_index, "DataLogger_Dlog", index, "DataLogger_MetaData");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "DataLogger_MetaData", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX|DlogStatus F32|FIXED|DlogScale U8|HEx|DlogMode U8|HEX|DlogRange U16|DEC|DlogCurrLogging|us U16|DEC|DlogCurrNSample U32|DEC|DlogCurrTimestamp");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDataLogger_Dlog_Data.DlogStatus = vr.GetNextDouble();
            CurrDataLogger_Dlog_Data.DlogScale = vr.GetNextDouble();
            CurrDataLogger_Dlog_Data.DlogMode = vr.GetNextDouble();
            CurrDataLogger_Dlog_Data.DlogRange = vr.GetNextDouble();
            CurrDataLogger_Dlog_Data.DlogCurrLogging = vr.GetNextDouble();
            CurrDataLogger_Dlog_Data.DlogCurrNSample = vr.GetNextDouble();
            CurrDataLogger_Dlog_Data.DlogCurrTimestamp = vr.GetNextDouble();
            CurrDataLogger_Dlog_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(DataLogger_MetaDataPropertyChangedName); // "DataLogger_MetaData"
            return CurrDataLogger_Dlog_Data;
        }
        /// <summary>
        /// Writes data to DataLogger_Settings 
        /// </summary>
        public async Task<GattCommunicationStatus?> WriteDataLogger_Settings(byte[] data)
        {
            var index = CharacteristicIndex.DataLogger_Dlog_DataLogger_Settings_index;
            await Ensure_Characteristic_Async(ServiceIndex.DataLogger_Dlog_index, "DataLogger_Dlog", index, "DataLogger_Settings");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteDataLogger_Settings", result);
            return result;
        }

        #endregion
//
        #region Service_Generic_Service
        // Service Generic Service 

        public Generic_Service_Data CurrGeneric_Service_Data { get; set; } = new Generic_Service_Data();

        // Per-characteristics methods for Generic_Service Server_Supported_Features
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyServer_Supported_FeaturesAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Server_Supported_Features", ServiceIndex.Generic_Service_index, "Generic Service", CharacteristicIndex.Generic_Service_Server_Supported_Features_index, NotifyServer_Supported_FeaturesCallback, notifyType);
            return retval;
        }

        private void NotifyServer_Supported_FeaturesCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Generic_Service_Server_Supported_Features_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX|FeatureBitmap0");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrGeneric_Service_Data.TimestampMostRecent = args.Timestamp;
            CurrGeneric_Service_Data.FeatureBitmap0 = vr.GetNextDouble();
            OnPropertyChanged(Server_Supported_FeaturesPropertyChangedName); // "Server_Supported_Features"
        }
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
        // Per-characteristics methods for Generic_Service Client_Supported_Features
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyClient_Supported_FeaturesAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Client_Supported_Features", ServiceIndex.Generic_Service_index, "Generic Service", CharacteristicIndex.Generic_Service_Client_Supported_Features_index, NotifyClient_Supported_FeaturesCallback, notifyType);
            return retval;
        }

        private void NotifyClient_Supported_FeaturesCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Generic_Service_Client_Supported_Features_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX|FeatureBitmapCSF");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrGeneric_Service_Data.TimestampMostRecent = args.Timestamp;
            CurrGeneric_Service_Data.FeatureBitmapCSF = vr.GetNextDouble();
            OnPropertyChanged(Client_Supported_FeaturesPropertyChangedName); // "Client_Supported_Features"
        }
        // Per-characteristics methods for Generic_Service Database_Hash
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyDatabase_HashAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Database_Hash", ServiceIndex.Generic_Service_index, "Generic Service", CharacteristicIndex.Generic_Service_Database_Hash_index, NotifyDatabase_HashCallback, notifyType);
            return retval;
        }

        private void NotifyDatabase_HashCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Generic_Service_Database_Hash_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U32|HEX|Hash0 U32|HEX|Hash1 U32|HEX|Hash2 U32|HEX|Hash3");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrGeneric_Service_Data.TimestampMostRecent = args.Timestamp;
            CurrGeneric_Service_Data.Hash0 = vr.GetNextDouble();
            CurrGeneric_Service_Data.Hash1 = vr.GetNextDouble();
            CurrGeneric_Service_Data.Hash2 = vr.GetNextDouble();
            CurrGeneric_Service_Data.Hash3 = vr.GetNextDouble();
            OnPropertyChanged(Database_HashPropertyChangedName); // "Database_Hash"
        }
        /// <summary>
        /// Reads data from Server_Supported_Features and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Generic_Service_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Generic_Service_Data> ReadServer_Supported_Features(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Generic_Service_Server_Supported_Features_index;
            await Ensure_Characteristic_Async(ServiceIndex.Generic_Service_index, "Generic Service", index, "Server_Supported_Features");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Server_Supported_Features", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX|FeatureBitmap0");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrGeneric_Service_Data.FeatureBitmap0 = vr.GetNextDouble();
            CurrGeneric_Service_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Server_Supported_FeaturesPropertyChangedName); // "Server_Supported_Features"
            return CurrGeneric_Service_Data;
        }
        /// <summary>
        /// Reads data from Service Changes and triggers an OnPropertyChanged
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
            CurrGeneric_Service_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Service_ChangesPropertyChangedName); // "Service_Changes"
            return CurrGeneric_Service_Data;
        }
        /// <summary>
        /// Reads data from Client_Supported_Features and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Generic_Service_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Generic_Service_Data> ReadClient_Supported_Features(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Generic_Service_Client_Supported_Features_index;
            await Ensure_Characteristic_Async(ServiceIndex.Generic_Service_index, "Generic Service", index, "Client_Supported_Features");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Client_Supported_Features", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX|FeatureBitmapCSF");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrGeneric_Service_Data.FeatureBitmapCSF = vr.GetNextDouble();
            CurrGeneric_Service_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Client_Supported_FeaturesPropertyChangedName); // "Client_Supported_Features"
            return CurrGeneric_Service_Data;
        }
        /// <summary>
        /// Reads data from Database Hash and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Generic_Service_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Generic_Service_Data> ReadDatabase_Hash(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Generic_Service_Database_Hash_index;
            await Ensure_Characteristic_Async(ServiceIndex.Generic_Service_index, "Generic Service", index, "Database Hash");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Database Hash", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U32|HEX|Hash0 U32|HEX|Hash1 U32|HEX|Hash2 U32|HEX|Hash3");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrGeneric_Service_Data.Hash0 = vr.GetNextDouble();
            CurrGeneric_Service_Data.Hash1 = vr.GetNextDouble();
            CurrGeneric_Service_Data.Hash2 = vr.GetNextDouble();
            CurrGeneric_Service_Data.Hash3 = vr.GetNextDouble();
            CurrGeneric_Service_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Database_HashPropertyChangedName); // "Database_Hash"
            return CurrGeneric_Service_Data;
        }
        /// <summary>
        /// Writes data to Server_Supported_Features 
        /// </summary>
        public async Task<GattCommunicationStatus?> WriteServer_Supported_Features(byte[] data)
        {
            var index = CharacteristicIndex.Generic_Service_Server_Supported_Features_index;
            await Ensure_Characteristic_Async(ServiceIndex.Generic_Service_index, "Generic Service", index, "Server_Supported_Features");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteServer_Supported_Features", result);
            return result;
        }
        /// <summary>
        /// Writes data to Client_Supported_Features 
        /// </summary>
        public async Task<GattCommunicationStatus?> WriteClient_Supported_Features(byte[] data)
        {
            var index = CharacteristicIndex.Generic_Service_Client_Supported_Features_index;
            await Ensure_Characteristic_Async(ServiceIndex.Generic_Service_index, "Generic Service", index, "Client_Supported_Features");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteClient_Supported_Features", result);
            return result;
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
        /// <summary>
        /// Reads data from Device Name and triggers an OnPropertyChanged
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
            CurrCommon_Configuration_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Device_NamePropertyChangedName); // "Device_Name"
            return CurrCommon_Configuration_Data;
        }
        /// <summary>
        /// Reads data from Appearance and triggers an OnPropertyChanged
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
            CurrCommon_Configuration_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(AppearancePropertyChangedName); // "Appearance"
            return CurrCommon_Configuration_Data;
        }
        /// <summary>
        /// Writes data to Device Name 
        /// </summary>
        public async Task<GattCommunicationStatus?> WriteDevice_Name(byte[] data)
        {
            var index = CharacteristicIndex.Common_Configuration_Device_Name_index;
            await Ensure_Characteristic_Async(ServiceIndex.Common_Configuration_index, "Common Configuration", index, "Device Name");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteDevice_Name", result);
            return result;
        }

        #endregion
//
        #region Service_Device_Info
        // Service Device Info 

        public Device_Info_Data CurrDevice_Info_Data { get; set; } = new Device_Info_Data();

        // Per-characteristics methods for Device_Info Manufacturer_Name
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyManufacturer_NameAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Manufacturer_Name", ServiceIndex.Device_Info_index, "Device Info", CharacteristicIndex.Device_Info_Manufacturer_Name_index, NotifyManufacturer_NameCallback, notifyType);
            return retval;
        }

        private void NotifyManufacturer_NameCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Device_Info_Manufacturer_Name_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|ASCII|Manufacturer_Name");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDevice_Info_Data.TimestampMostRecent = args.Timestamp;
            CurrDevice_Info_Data.Manufacturer_Name = vr.GetNextString();
            OnPropertyChanged(Manufacturer_NamePropertyChangedName); // "Manufacturer_Name"
        }
        // Per-characteristics methods for Device_Info Model_Number
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyModel_NumberAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Model_Number", ServiceIndex.Device_Info_index, "Device Info", CharacteristicIndex.Device_Info_Model_Number_index, NotifyModel_NumberCallback, notifyType);
            return retval;
        }

        private void NotifyModel_NumberCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Device_Info_Model_Number_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|ASCII|Model_Number");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDevice_Info_Data.TimestampMostRecent = args.Timestamp;
            CurrDevice_Info_Data.Model_Number = vr.GetNextString();
            OnPropertyChanged(Model_NumberPropertyChangedName); // "Model_Number"
        }
        // Per-characteristics methods for Device_Info Firmware_Revision
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyFirmware_RevisionAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Firmware_Revision", ServiceIndex.Device_Info_index, "Device Info", CharacteristicIndex.Device_Info_Firmware_Revision_index, NotifyFirmware_RevisionCallback, notifyType);
            return retval;
        }

        private void NotifyFirmware_RevisionCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Device_Info_Firmware_Revision_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|ASCII|Firmware_Revision");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDevice_Info_Data.TimestampMostRecent = args.Timestamp;
            CurrDevice_Info_Data.Firmware_Revision = vr.GetNextString();
            OnPropertyChanged(Firmware_RevisionPropertyChangedName); // "Firmware_Revision"
        }
        // Per-characteristics methods for Device_Info Software_Revision
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifySoftware_RevisionAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Software_Revision", ServiceIndex.Device_Info_index, "Device Info", CharacteristicIndex.Device_Info_Software_Revision_index, NotifySoftware_RevisionCallback, notifyType);
            return retval;
        }

        private void NotifySoftware_RevisionCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Device_Info_Software_Revision_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|ASCII|Software_Revision");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDevice_Info_Data.TimestampMostRecent = args.Timestamp;
            CurrDevice_Info_Data.Software_Revision = vr.GetNextString();
            OnPropertyChanged(Software_RevisionPropertyChangedName); // "Software_Revision"
        }
        // Per-characteristics methods for Device_Info Hardware_Revision
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyHardware_RevisionAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Hardware_Revision", ServiceIndex.Device_Info_index, "Device Info", CharacteristicIndex.Device_Info_Hardware_Revision_index, NotifyHardware_RevisionCallback, notifyType);
            return retval;
        }

        private void NotifyHardware_RevisionCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Device_Info_Hardware_Revision_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|ASCII|Hardware_Revision");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDevice_Info_Data.TimestampMostRecent = args.Timestamp;
            CurrDevice_Info_Data.Hardware_Revision = vr.GetNextString();
            OnPropertyChanged(Hardware_RevisionPropertyChangedName); // "Hardware_Revision"
        }
        // Per-characteristics methods for Device_Info Serial_Number
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifySerial_NumberAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Serial_Number", ServiceIndex.Device_Info_index, "Device Info", CharacteristicIndex.Device_Info_Serial_Number_index, NotifySerial_NumberCallback, notifyType);
            return retval;
        }

        private void NotifySerial_NumberCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Device_Info_Serial_Number_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|ASCII|Serial_Number");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDevice_Info_Data.TimestampMostRecent = args.Timestamp;
            CurrDevice_Info_Data.Serial_Number = vr.GetNextString();
            OnPropertyChanged(Serial_NumberPropertyChangedName); // "Serial_Number"
        }
        /// <summary>
        /// Reads data from Manufacturer Name and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Device_Info_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Device_Info_Data> ReadManufacturer_Name(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Device_Info_Manufacturer_Name_index;
            await Ensure_Characteristic_Async(ServiceIndex.Device_Info_index, "Device Info", index, "Manufacturer Name");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Manufacturer Name", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("STRING|ASCII|Manufacturer_Name");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDevice_Info_Data.Manufacturer_Name = vr.GetNextString();
            CurrDevice_Info_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Manufacturer_NamePropertyChangedName); // "Manufacturer_Name"
            return CurrDevice_Info_Data;
        }
        /// <summary>
        /// Reads data from Model Number and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Device_Info_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Device_Info_Data> ReadModel_Number(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Device_Info_Model_Number_index;
            await Ensure_Characteristic_Async(ServiceIndex.Device_Info_index, "Device Info", index, "Model Number");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Model Number", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("STRING|ASCII|Model_Number");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDevice_Info_Data.Model_Number = vr.GetNextString();
            CurrDevice_Info_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Model_NumberPropertyChangedName); // "Model_Number"
            return CurrDevice_Info_Data;
        }
        /// <summary>
        /// Reads data from Firmware Revision and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Device_Info_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Device_Info_Data> ReadFirmware_Revision(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Device_Info_Firmware_Revision_index;
            await Ensure_Characteristic_Async(ServiceIndex.Device_Info_index, "Device Info", index, "Firmware Revision");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Firmware Revision", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("STRING|ASCII|Firmware_Revision");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDevice_Info_Data.Firmware_Revision = vr.GetNextString();
            CurrDevice_Info_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Firmware_RevisionPropertyChangedName); // "Firmware_Revision"
            return CurrDevice_Info_Data;
        }
        /// <summary>
        /// Reads data from Software Revision and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Device_Info_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Device_Info_Data> ReadSoftware_Revision(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Device_Info_Software_Revision_index;
            await Ensure_Characteristic_Async(ServiceIndex.Device_Info_index, "Device Info", index, "Software Revision");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Software Revision", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("STRING|ASCII|Software_Revision");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDevice_Info_Data.Software_Revision = vr.GetNextString();
            CurrDevice_Info_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Software_RevisionPropertyChangedName); // "Software_Revision"
            return CurrDevice_Info_Data;
        }
        /// <summary>
        /// Reads data from Hardware Revision and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Device_Info_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Device_Info_Data> ReadHardware_Revision(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Device_Info_Hardware_Revision_index;
            await Ensure_Characteristic_Async(ServiceIndex.Device_Info_index, "Device Info", index, "Hardware Revision");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Hardware Revision", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("STRING|ASCII|Hardware_Revision");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDevice_Info_Data.Hardware_Revision = vr.GetNextString();
            CurrDevice_Info_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Hardware_RevisionPropertyChangedName); // "Hardware_Revision"
            return CurrDevice_Info_Data;
        }
        /// <summary>
        /// Reads data from Serial Number and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Device_Info_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Device_Info_Data> ReadSerial_Number(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Device_Info_Serial_Number_index;
            await Ensure_Characteristic_Async(ServiceIndex.Device_Info_index, "Device Info", index, "Serial Number");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Serial Number", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("STRING|ASCII|Serial_Number");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDevice_Info_Data.Serial_Number = vr.GetNextString();
            CurrDevice_Info_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Serial_NumberPropertyChangedName); // "Serial_Number"
            return CurrDevice_Info_Data;
        }

        #endregion
//
        #region Service_Service_Status
        // Service Service_Status 

        public Service_Status_Data CurrService_Status_Data { get; set; } = new Service_Status_Data();

        // Per-characteristics methods for Service_Status Status_Device
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyStatus_DeviceAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Status_Device", ServiceIndex.Service_Status_index, "Service_Status", CharacteristicIndex.Service_Status_Status_Device_index, NotifyStatus_DeviceCallback, notifyType);
            return retval;
        }

        private void NotifyStatus_DeviceCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Service_Status_Status_Device_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX|FirmwareMajor U8|HEX|FirmwareMinor U16|HEX|MaxInputVoltage U16|HEX|MaxInputCurrent U16|HEX|MaxInputResistance U16|HEX|MaxSamplingRate U16|HEX|DeviceBufferSize U16|HEX|Reserved01 BYTES|HEX|MacAddress");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrService_Status_Data.TimestampMostRecent = args.Timestamp;
            CurrService_Status_Data.FirmwareMajor = vr.GetNextDouble();
            CurrService_Status_Data.FirmwareMinor = vr.GetNextDouble();
            CurrService_Status_Data.MaxInputVoltage = vr.GetNextDouble();
            CurrService_Status_Data.MaxInputCurrent = vr.GetNextDouble();
            CurrService_Status_Data.MaxInputResistance = vr.GetNextDouble();
            CurrService_Status_Data.MaxSamplingRate = vr.GetNextDouble();
            CurrService_Status_Data.DeviceBufferSize = vr.GetNextDouble();
            CurrService_Status_Data.Reserved01 = vr.GetNextDouble();
            CurrService_Status_Data.MacAddress = vr.GetNextByteArray();
            OnPropertyChanged(Status_DevicePropertyChangedName); // "Status_Device"
        }
        // Per-characteristics methods for Service_Status Status_Status
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyStatus_StatusAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Status_Status", ServiceIndex.Service_Status_index, "Service_Status", CharacteristicIndex.Service_Status_Status_Status_index, NotifyStatus_StatusCallback, notifyType);
            return retval;
        }

        private void NotifyStatus_StatusCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Service_Status_Status_Status_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|DEC|DeviceStatus F32|FIXED|BatteryLevel");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrService_Status_Data.TimestampMostRecent = args.Timestamp;
            CurrService_Status_Data.DeviceStatus = vr.GetNextDouble();
            CurrService_Status_Data.BatteryLevel = vr.GetNextDouble();
            OnPropertyChanged(Status_StatusPropertyChangedName); // "Status_Status"
        }
        // Per-characteristics methods for Service_Status Status_Device_Name
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyStatus_Device_NameAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Status_Device_Name", ServiceIndex.Service_Status_index, "Service_Status", CharacteristicIndex.Service_Status_Status_Device_Name_index, NotifyStatus_Device_NameCallback, notifyType);
            return retval;
        }

        private void NotifyStatus_Device_NameCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Service_Status_Status_Device_Name_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|ASCII|Device_Name");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrService_Status_Data.TimestampMostRecent = args.Timestamp;
            CurrService_Status_Data.Device_Name = vr.GetNextString();
            OnPropertyChanged(Status_Device_NamePropertyChangedName); // "Status_Device_Name"
        }
        // Per-characteristics methods for Service_Status Status_Flash_LED
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyStatus_Flash_LEDAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Status_Flash_LED", ServiceIndex.Service_Status_index, "Service_Status", CharacteristicIndex.Service_Status_Status_Flash_LED_index, NotifyStatus_Flash_LEDCallback, notifyType);
            return retval;
        }

        private void NotifyStatus_Flash_LEDCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Service_Status_Status_Flash_LED_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX|Red U8|HEX|Green U8|HEX|Blue U8|HEX|Beep");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrService_Status_Data.TimestampMostRecent = args.Timestamp;
            CurrService_Status_Data.Red = vr.GetNextDouble();
            CurrService_Status_Data.Green = vr.GetNextDouble();
            CurrService_Status_Data.Blue = vr.GetNextDouble();
            CurrService_Status_Data.Beep = vr.GetNextDouble();
            OnPropertyChanged(Status_Flash_LEDPropertyChangedName); // "Status_Flash_LED"
        }
        // Per-characteristics methods for Service_Status Status_Light_LED
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyStatus_Light_LEDAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Status_Light_LED", ServiceIndex.Service_Status_index, "Service_Status", CharacteristicIndex.Service_Status_Status_Light_LED_index, NotifyStatus_Light_LEDCallback, notifyType);
            return retval;
        }

        private void NotifyStatus_Light_LEDCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Service_Status_Status_Light_LED_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX|Light");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrService_Status_Data.TimestampMostRecent = args.Timestamp;
            CurrService_Status_Data.Light = vr.GetNextDouble();
            OnPropertyChanged(Status_Light_LEDPropertyChangedName); // "Status_Light_LED"
        }
        // Per-characteristics methods for Service_Status UnknownStatusValues
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyUnknownStatusValuesAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("UnknownStatusValues", ServiceIndex.Service_Status_index, "Service_Status", CharacteristicIndex.Service_Status_UnknownStatusValues_index, NotifyUnknownStatusValuesCallback, notifyType);
            return retval;
        }

        private void NotifyUnknownStatusValuesCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Service_Status_UnknownStatusValues_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|StatusUnknown5");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrService_Status_Data.TimestampMostRecent = args.Timestamp;
            CurrService_Status_Data.StatusUnknown5 = vr.GetNextByteArray();
            OnPropertyChanged(UnknownStatusValuesPropertyChangedName); // "UnknownStatusValues"
        }
        /// <summary>
        /// Reads data from Status_Device and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Service_Status_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Service_Status_Data> ReadStatus_Device(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Service_Status_Status_Device_index;
            await Ensure_Characteristic_Async(ServiceIndex.Service_Status_index, "Service_Status", index, "Status_Device");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Status_Device", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX|FirmwareMajor U8|HEX|FirmwareMinor U16|HEX|MaxInputVoltage U16|HEX|MaxInputCurrent U16|HEX|MaxInputResistance U16|HEX|MaxSamplingRate U16|HEX|DeviceBufferSize U16|HEX|Reserved01 BYTES|HEX|MacAddress");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrService_Status_Data.FirmwareMajor = vr.GetNextDouble();
            CurrService_Status_Data.FirmwareMinor = vr.GetNextDouble();
            CurrService_Status_Data.MaxInputVoltage = vr.GetNextDouble();
            CurrService_Status_Data.MaxInputCurrent = vr.GetNextDouble();
            CurrService_Status_Data.MaxInputResistance = vr.GetNextDouble();
            CurrService_Status_Data.MaxSamplingRate = vr.GetNextDouble();
            CurrService_Status_Data.DeviceBufferSize = vr.GetNextDouble();
            CurrService_Status_Data.Reserved01 = vr.GetNextDouble();
            CurrService_Status_Data.MacAddress = vr.GetNextByteArray();
            CurrService_Status_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Status_DevicePropertyChangedName); // "Status_Device"
            return CurrService_Status_Data;
        }
        /// <summary>
        /// Reads data from Status_Status and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Service_Status_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Service_Status_Data> ReadStatus_Status(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Service_Status_Status_Status_index;
            await Ensure_Characteristic_Async(ServiceIndex.Service_Status_index, "Service_Status", index, "Status_Status");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Status_Status", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|DEC|DeviceStatus F32|FIXED|BatteryLevel");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrService_Status_Data.DeviceStatus = vr.GetNextDouble();
            CurrService_Status_Data.BatteryLevel = vr.GetNextDouble();
            CurrService_Status_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Status_StatusPropertyChangedName); // "Status_Status"
            return CurrService_Status_Data;
        }
        /// <summary>
        /// Reads data from Status_Device_Name and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Service_Status_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Service_Status_Data> ReadStatus_Device_Name(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Service_Status_Status_Device_Name_index;
            await Ensure_Characteristic_Async(ServiceIndex.Service_Status_index, "Service_Status", index, "Status_Device_Name");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Status_Device_Name", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("STRING|ASCII|Device_Name");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrService_Status_Data.Device_Name = vr.GetNextString();
            CurrService_Status_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Status_Device_NamePropertyChangedName); // "Status_Device_Name"
            return CurrService_Status_Data;
        }
        /// <summary>
        /// Reads data from Status_Flash_LED and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Service_Status_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Service_Status_Data> ReadStatus_Flash_LED(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Service_Status_Status_Flash_LED_index;
            await Ensure_Characteristic_Async(ServiceIndex.Service_Status_index, "Service_Status", index, "Status_Flash_LED");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Status_Flash_LED", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX|Red U8|HEX|Green U8|HEX|Blue U8|HEX|Beep");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrService_Status_Data.Red = vr.GetNextDouble();
            CurrService_Status_Data.Green = vr.GetNextDouble();
            CurrService_Status_Data.Blue = vr.GetNextDouble();
            CurrService_Status_Data.Beep = vr.GetNextDouble();
            CurrService_Status_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Status_Flash_LEDPropertyChangedName); // "Status_Flash_LED"
            return CurrService_Status_Data;
        }
        /// <summary>
        /// Reads data from Status_Light_LED and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Service_Status_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Service_Status_Data> ReadStatus_Light_LED(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Service_Status_Status_Light_LED_index;
            await Ensure_Characteristic_Async(ServiceIndex.Service_Status_index, "Service_Status", index, "Status_Light_LED");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Status_Light_LED", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX|Light");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrService_Status_Data.Light = vr.GetNextDouble();
            CurrService_Status_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Status_Light_LEDPropertyChangedName); // "Status_Light_LED"
            return CurrService_Status_Data;
        }
        /// <summary>
        /// Reads data from UnknownStatusValues and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Service_Status_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Service_Status_Data> ReadUnknownStatusValues(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Service_Status_UnknownStatusValues_index;
            await Ensure_Characteristic_Async(ServiceIndex.Service_Status_index, "Service_Status", index, "UnknownStatusValues");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "UnknownStatusValues", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|StatusUnknown5");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrService_Status_Data.StatusUnknown5 = vr.GetNextByteArray();
            CurrService_Status_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(UnknownStatusValuesPropertyChangedName); // "UnknownStatusValues"
            return CurrService_Status_Data;
        }
        /// <summary>
        /// Writes data to Status_Device_Name 
        /// </summary>
        public async Task<GattCommunicationStatus?> WriteStatus_Device_Name(byte[] data)
        {
            var index = CharacteristicIndex.Service_Status_Status_Device_Name_index;
            await Ensure_Characteristic_Async(ServiceIndex.Service_Status_index, "Service_Status", index, "Status_Device_Name");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteStatus_Device_Name", result);
            return result;
        }
        /// <summary>
        /// Writes data to Status_Flash_LED 
        /// </summary>
        public async Task<GattCommunicationStatus?> WriteStatus_Flash_LED(byte[] data)
        {
            var index = CharacteristicIndex.Service_Status_Status_Flash_LED_index;
            await Ensure_Characteristic_Async(ServiceIndex.Service_Status_index, "Service_Status", index, "Status_Flash_LED");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteStatus_Flash_LED", result);
            return result;
        }
        /// <summary>
        /// Writes data to Status_Light_LED 
        /// </summary>
        public async Task<GattCommunicationStatus?> WriteStatus_Light_LED(byte[] data)
        {
            var index = CharacteristicIndex.Service_Status_Status_Light_LED_index;
            await Ensure_Characteristic_Async(ServiceIndex.Service_Status_index, "Service_Status", index, "Status_Light_LED");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteStatus_Light_LED", result);
            return result;
        }
        /// <summary>
        /// Writes data to UnknownStatusValues 
        /// </summary>
        public async Task<GattCommunicationStatus?> WriteUnknownStatusValues(byte[] data)
        {
            var index = CharacteristicIndex.Service_Status_UnknownStatusValues_index;
            await Ensure_Characteristic_Async(ServiceIndex.Service_Status_index, "Service_Status", index, "UnknownStatusValues");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteUnknownStatusValues", result);
            return result;
        }

        #endregion
//
        #region Service_CalibrationService
        // Service CalibrationService 

        public CalibrationService_Data CurrCalibrationService_Data { get; set; } = new CalibrationService_Data();

        // Per-characteristics methods for CalibrationService CalbrateTemperature
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyCalbrateTemperatureAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("CalbrateTemperature", ServiceIndex.CalibrationService_index, "CalibrationService", CharacteristicIndex.CalibrationService_CalbrateTemperature_index, NotifyCalbrateTemperatureCallback, notifyType);
            return retval;
        }

        private void NotifyCalbrateTemperatureCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.CalibrationService_CalbrateTemperature_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|CalibrateUnknown0");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrCalibrationService_Data.TimestampMostRecent = args.Timestamp;
            CurrCalibrationService_Data.CalibrateUnknown0 = vr.GetNextByteArray();
            OnPropertyChanged(CalbrateTemperaturePropertyChangedName); // "CalbrateTemperature"
        }
        // Per-characteristics methods for CalibrationService CalibrateUnknown1
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyCalibrateUnknown1Async(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("CalibrateUnknown1", ServiceIndex.CalibrationService_index, "CalibrationService", CharacteristicIndex.CalibrationService_CalibrateUnknown1_index, NotifyCalibrateUnknown1Callback, notifyType);
            return retval;
        }

        private void NotifyCalibrateUnknown1Callback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.CalibrationService_CalibrateUnknown1_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|CalibrateUnknown1");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrCalibrationService_Data.TimestampMostRecent = args.Timestamp;
            CurrCalibrationService_Data.CalibrateUnknown1 = vr.GetNextByteArray();
            OnPropertyChanged(CalibrateUnknown1PropertyChangedName); // "CalibrateUnknown1"
        }
        // Per-characteristics methods for CalibrationService CalibrrateUnknown2
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyCalibrrateUnknown2Async(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("CalibrrateUnknown2", ServiceIndex.CalibrationService_index, "CalibrationService", CharacteristicIndex.CalibrationService_CalibrrateUnknown2_index, NotifyCalibrrateUnknown2Callback, notifyType);
            return retval;
        }

        private void NotifyCalibrrateUnknown2Callback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.CalibrationService_CalibrrateUnknown2_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|CalibrrateUnknown2");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrCalibrationService_Data.TimestampMostRecent = args.Timestamp;
            CurrCalibrationService_Data.CalibrrateUnknown2 = vr.GetNextByteArray();
            OnPropertyChanged(CalibrrateUnknown2PropertyChangedName); // "CalibrrateUnknown2"
        }
        /// <summary>
        /// Reads data from CalbrateTemperature and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>CalibrationService_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<CalibrationService_Data> ReadCalbrateTemperature(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.CalibrationService_CalbrateTemperature_index;
            await Ensure_Characteristic_Async(ServiceIndex.CalibrationService_index, "CalibrationService", index, "CalbrateTemperature");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "CalbrateTemperature", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|CalibrateUnknown0");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrCalibrationService_Data.CalibrateUnknown0 = vr.GetNextByteArray();
            CurrCalibrationService_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(CalbrateTemperaturePropertyChangedName); // "CalbrateTemperature"
            return CurrCalibrationService_Data;
        }
        /// <summary>
        /// Reads data from CalibrateUnknown1 and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>CalibrationService_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<CalibrationService_Data> ReadCalibrateUnknown1(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.CalibrationService_CalibrateUnknown1_index;
            await Ensure_Characteristic_Async(ServiceIndex.CalibrationService_index, "CalibrationService", index, "CalibrateUnknown1");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "CalibrateUnknown1", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|CalibrateUnknown1");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrCalibrationService_Data.CalibrateUnknown1 = vr.GetNextByteArray();
            CurrCalibrationService_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(CalibrateUnknown1PropertyChangedName); // "CalibrateUnknown1"
            return CurrCalibrationService_Data;
        }
        /// <summary>
        /// Reads data from CalibrrateUnknown2 and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>CalibrationService_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<CalibrationService_Data> ReadCalibrrateUnknown2(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.CalibrationService_CalibrrateUnknown2_index;
            await Ensure_Characteristic_Async(ServiceIndex.CalibrationService_index, "CalibrationService", index, "CalibrrateUnknown2");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "CalibrrateUnknown2", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|CalibrrateUnknown2");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrCalibrationService_Data.CalibrrateUnknown2 = vr.GetNextByteArray();
            CurrCalibrationService_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(CalibrrateUnknown2PropertyChangedName); // "CalibrrateUnknown2"
            return CurrCalibrationService_Data;
        }
        /// <summary>
        /// Writes data to CalbrateTemperature 
        /// </summary>
        public async Task<GattCommunicationStatus?> WriteCalbrateTemperature(byte[] data)
        {
            var index = CharacteristicIndex.CalibrationService_CalbrateTemperature_index;
            await Ensure_Characteristic_Async(ServiceIndex.CalibrationService_index, "CalibrationService", index, "CalbrateTemperature");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteCalbrateTemperature", result);
            return result;
        }
        /// <summary>
        /// Writes data to CalibrateUnknown1 
        /// </summary>
        public async Task<GattCommunicationStatus?> WriteCalibrateUnknown1(byte[] data)
        {
            var index = CharacteristicIndex.CalibrationService_CalibrateUnknown1_index;
            await Ensure_Characteristic_Async(ServiceIndex.CalibrationService_index, "CalibrationService", index, "CalibrateUnknown1");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteCalibrateUnknown1", result);
            return result;
        }
        /// <summary>
        /// Writes data to CalibrrateUnknown2 
        /// </summary>
        public async Task<GattCommunicationStatus?> WriteCalibrrateUnknown2(byte[] data)
        {
            var index = CharacteristicIndex.CalibrationService_CalibrrateUnknown2_index;
            await Ensure_Characteristic_Async(ServiceIndex.CalibrationService_index, "CalibrationService", index, "CalibrrateUnknown2");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteCalibrrateUnknown2", result);
            return result;
        }

        #endregion
//
        #region Service_Silabs_Service_OTA
        // Service Silabs_Service_OTA 

        public Silabs_Service_OTA_Data CurrSilabs_Service_OTA_Data { get; set; } = new Silabs_Service_OTA_Data();

        // Per-characteristics methods for Silabs_Service_OTA OTA_Control
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyOTA_ControlAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("OTA_Control", ServiceIndex.Silabs_Service_OTA_index, "Silabs_Service_OTA", CharacteristicIndex.Silabs_Service_OTA_OTA_Control_index, NotifyOTA_ControlCallback, notifyType);
            return retval;
        }

        private void NotifyOTA_ControlCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Silabs_Service_OTA_OTA_Control_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|Unknown0");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrSilabs_Service_OTA_Data.TimestampMostRecent = args.Timestamp;
            CurrSilabs_Service_OTA_Data.Unknown0 = vr.GetNextByteArray();
            OnPropertyChanged(OTA_ControlPropertyChangedName); // "OTA_Control"
        }
        // Per-characteristics methods for Silabs_Service_OTA OTA_Data
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyOTA_DataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("OTA_Data", ServiceIndex.Silabs_Service_OTA_index, "Silabs_Service_OTA", CharacteristicIndex.Silabs_Service_OTA_OTA_Data_index, NotifyOTA_DataCallback, notifyType);
            return retval;
        }

        private void NotifyOTA_DataCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Silabs_Service_OTA_OTA_Data_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|Unknown1");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrSilabs_Service_OTA_Data.TimestampMostRecent = args.Timestamp;
            CurrSilabs_Service_OTA_Data.Unknown1 = vr.GetNextByteArray();
            OnPropertyChanged(OTA_DataPropertyChangedName); // "OTA_Data"
        }
        /// <summary>
        /// Reads data from OTA_Control and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Silabs_Service_OTA_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Silabs_Service_OTA_Data> ReadOTA_Control(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Silabs_Service_OTA_OTA_Control_index;
            await Ensure_Characteristic_Async(ServiceIndex.Silabs_Service_OTA_index, "Silabs_Service_OTA", index, "OTA_Control");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "OTA_Control", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|Unknown0");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrSilabs_Service_OTA_Data.Unknown0 = vr.GetNextByteArray();
            CurrSilabs_Service_OTA_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(OTA_ControlPropertyChangedName); // "OTA_Control"
            return CurrSilabs_Service_OTA_Data;
        }
        /// <summary>
        /// Reads data from OTA_Data and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Silabs_Service_OTA_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Silabs_Service_OTA_Data> ReadOTA_Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Silabs_Service_OTA_OTA_Data_index;
            await Ensure_Characteristic_Async(ServiceIndex.Silabs_Service_OTA_index, "Silabs_Service_OTA", index, "OTA_Data");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "OTA_Data", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|Unknown1");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrSilabs_Service_OTA_Data.Unknown1 = vr.GetNextByteArray();
            CurrSilabs_Service_OTA_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(OTA_DataPropertyChangedName); // "OTA_Data"
            return CurrSilabs_Service_OTA_Data;
        }
        /// <summary>
        /// Writes data to OTA_Control 
        /// </summary>
        public async Task<GattCommunicationStatus?> WriteOTA_Control(byte[] data)
        {
            var index = CharacteristicIndex.Silabs_Service_OTA_OTA_Control_index;
            await Ensure_Characteristic_Async(ServiceIndex.Silabs_Service_OTA_index, "Silabs_Service_OTA", index, "OTA_Control");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteOTA_Control", result);
            return result;
        }
        /// <summary>
        /// Writes data to OTA_Data 
        /// </summary>
        public async Task<GattCommunicationStatus?> WriteOTA_Data(byte[] data)
        {
            var index = CharacteristicIndex.Silabs_Service_OTA_OTA_Data_index;
            await Ensure_Characteristic_Async(ServiceIndex.Silabs_Service_OTA_index, "Silabs_Service_OTA", index, "OTA_Data");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteOTA_Data", result);
            return result;
        }

        #endregion
//


// Long obsolete! [[zzMETHOD+LIST]]
    }
}