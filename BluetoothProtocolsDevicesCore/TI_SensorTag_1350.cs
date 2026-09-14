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
    /// The TI 1350 and 2650 are the latest in the TI range of Sensor. Each battery-driven sensor tag includes a variety of sensors (light, humidity, accelerometer, and more) which are accessible from Bluetooth.
    /// This class was automatically generated 2026-09-13::20:43
    /// </summary>

    public partial class TI_SensorTag_1350 : INotifyPropertyChanged
    {
        // Useful links for the device and protocol documentation
        // No links for this device

        public BluetoothLEDevice ble { get; set; } = null;
        public BluetoothStatusEvent Status = new BluetoothStatusEvent();

        // For the INotifyPropertyChanged values
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Name { get; } = "CC1350 SensorTag";
        public string Description { get; } = "The TI 1350 and 2650 are the latest in the TI range of Sensor. Each battery-driven sensor tag includes a variety of sensors (light, humidity, accelerometer, and more) which are accessible from Bluetooth";

        /* Service and Characteristics summary for the device CC1350 SensorTag

        Accelerometer service Guid=f000aa80-0451-4000-b000-000000000000
            Accelerometer_Data (DataGroup record)
                Accelerometer Data characteristic has GyroX (Int16-->double) GyroY (Int16-->double) GyroZ (Int16-->double) AccX (Int16-->double) AccY (Int16-->double) AccZ (Int16-->double) MagnetometerX (Int16-->double) MagnetometerY (Int16-->double) MagnetometerZ (Int16-->double)  Guid=f000aa81-0451-4000-b000-000000000000
                Accelerometer Config characteristic has Enable (UInt16-->double)  Guid=f000aa82-0451-4000-b000-000000000000
                Accelerometer Period characteristic has Period (Byte-->double)  Guid=f000aa83-0451-4000-b000-000000000000


        Key Press service Guid=ffe0
            Key Press_Data (DataGroup record)
                Key Press State characteristic has param0 (Byte-->double)  Guid=ffe1


        IR Service service Guid=f000aa00-0451-4000-b000-000000000000
            IR Service_Data (DataGroup record)
                IR Data characteristic has ObjTemp (Int16-->double) AmbTemp (Int16-->double)  Guid=f000aa01-0451-4000-b000-000000000000
                IR Service Config characteristic has Enable (Byte-->double)  Guid=f000aa02-0451-4000-b000-000000000000
                IR Service Period characteristic has Period (Byte-->double)  Guid=f000aa03-0451-4000-b000-000000000000


        Humidity service Guid=f000aa20-0451-4000-b000-000000000000
            Humidity_Data (DataGroup record)
                Humidity Data characteristic has Temp (UInt16-->double) Humidity (UInt16-->double)  Guid=f000aa21-0451-4000-b000-000000000000
                Humidity Config characteristic has Enable (Byte-->double)  Guid=f000aa22-0451-4000-b000-000000000000
                Humidity Period characteristic has Period (Byte-->double)  Guid=f000aa23-0451-4000-b000-000000000000


        Barometer service Guid=f000aa40-0451-4000-b000-000000000000
            Barometer_Data (DataGroup record)
                Barometer Data characteristic has Temp (Int32-->double) Pressure (Int32-->double)  Guid=f000aa41-0451-4000-b000-000000000000
                Barometer Config characteristic has Enable (Byte-->double)  Guid=f000aa42-0451-4000-b000-000000000000
                Barometer Period characteristic has Period (Byte-->double)  Guid=f000aa44-0451-4000-b000-000000000000


        Optical Service service Guid=f000aa70-0451-4000-b000-000000000000
            Optical Service_Data (DataGroup record)
                Optical Service Data characteristic has Lux (UInt16-->double)  Guid=f000aa71-0451-4000-b000-000000000000
                Optical Service Config characteristic has Enable (Byte-->double)  Guid=f000aa72-0451-4000-b000-000000000000
                Optical Service Period characteristic has Period (Byte-->double)  Guid=f000aa73-0451-4000-b000-000000000000


        IO Service service Guid=f000aa64-0451-4000-b000-000000000000
            IO Service_Data (DataGroup record)
                IO Service Data characteristic has IOData (Bytes-->string)  Guid=f000aa65-0451-4000-b000-000000000000
                IO Service Config characteristic has IOConfig (Byte-->double)  Guid=f000aa66-0451-4000-b000-000000000000


        Register service service Guid=f000ac00-0451-4000-b000-000000000000
            Register service_Data (DataGroup record)
                Register Data characteristic has RegisterData (Bytes-->string)  Guid=f000ac01-0451-4000-b000-000000000000
                Register Address characteristic has RegisterAddress (Bytes-->string)  Guid=f000ac02-0451-4000-b000-000000000000
                Register Device ID characteristic has RegisterDeviceID (Bytes-->string)  Guid=f000ac03-0451-4000-b000-000000000000


        Common Configuration service Guid=1800
            Common Configuration_Data (DataGroup record)
                Device Name characteristic has Device_Name (String-->string)  Guid=2a00
                Appearance characteristic has Appearance (UInt16-->double)  Guid=2a01
                Connection Parameter characteristic has ConnectionParameter (Bytes-->string)  Guid=2a04


        Device Info service Guid=180a
            Device Info_Data (DataGroup record)
                System ID characteristic has SystemID (String-->string)  Guid=2a23
                Model Number characteristic has ModelNumber (String-->string)  Guid=2a24
                Serial Number characteristic has SerialNumber (String-->string)  Guid=2a25
                Firmware Revision characteristic has FirmwareRevision (String-->string)  Guid=2a26
                Hardware Revision characteristic has HardwareRevision (String-->string)  Guid=2a27
                Software Revision characteristic has SoftwareRevision (String-->string)  Guid=2a28
                Manufacturer Name characteristic has ManufacturerName (String-->string)  Guid=2a29
                Regulatory List characteristic has BodyType (Byte-->double) BodyStructure (Byte-->double) Data (String-->string)  Guid=2a2a
                PnP ID characteristic has PnPID (String-->string)  Guid=2a50


        Battery service Guid=180f
            Battery_Data (DataGroup record)
                BatteryLevel characteristic has BatteryLevel (SByte-->double)  Guid=2a19
        */

        public const string Accelerometer_DataPropertyChangedName = "Accelerometer_Data";
        public const string Accelerometer_ConfigPropertyChangedName = "Accelerometer_Config";
        public const string Accelerometer_PeriodPropertyChangedName = "Accelerometer_Period";
        public const string Key_Press_StatePropertyChangedName = "Key_Press_State";
        public const string IR_DataPropertyChangedName = "IR_Data";
        public const string IR_Service_ConfigPropertyChangedName = "IR_Service_Config";
        public const string IR_Service_PeriodPropertyChangedName = "IR_Service_Period";
        public const string Humidity_DataPropertyChangedName = "Humidity_Data";
        public const string Humidity_ConfigPropertyChangedName = "Humidity_Config";
        public const string Humidity_PeriodPropertyChangedName = "Humidity_Period";
        public const string Barometer_DataPropertyChangedName = "Barometer_Data";
        public const string Barometer_ConfigPropertyChangedName = "Barometer_Config";
        public const string Barometer_PeriodPropertyChangedName = "Barometer_Period";
        public const string Optical_Service_DataPropertyChangedName = "Optical_Service_Data";
        public const string Optical_Service_ConfigPropertyChangedName = "Optical_Service_Config";
        public const string Optical_Service_PeriodPropertyChangedName = "Optical_Service_Period";
        public const string IO_Service_DataPropertyChangedName = "IO_Service_Data";
        public const string IO_Service_ConfigPropertyChangedName = "IO_Service_Config";
        public const string Register_DataPropertyChangedName = "Register_Data";
        public const string Register_AddressPropertyChangedName = "Register_Address";
        public const string Register_Device_IDPropertyChangedName = "Register_Device_ID";
        public const string Device_NamePropertyChangedName = "Device_Name";
        public const string AppearancePropertyChangedName = "Appearance";
        public const string Connection_ParameterPropertyChangedName = "Connection_Parameter";
        public const string System_IDPropertyChangedName = "System_ID";
        public const string Model_NumberPropertyChangedName = "Model_Number";
        public const string Serial_NumberPropertyChangedName = "Serial_Number";
        public const string Firmware_RevisionPropertyChangedName = "Firmware_Revision";
        public const string Hardware_RevisionPropertyChangedName = "Hardware_Revision";
        public const string Software_RevisionPropertyChangedName = "Software_Revision";
        public const string Manufacturer_NamePropertyChangedName = "Manufacturer_Name";
        public const string Regulatory_ListPropertyChangedName = "Regulatory_List";
        public const string PnP_IDPropertyChangedName = "PnP_ID";
        public const string BatteryLevelPropertyChangedName = "BatteryLevel";



        //
        // All services / characteristics data types 
        //

        #region All_Data_Types
        /// <summary>
        /// Data from all of the characteristics in the Accelerometer Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class Accelerometer_Data :BTCommonMetaData<Accelerometer_Data> //, IExportDataSource
        {
            private double _GyroX = 0;
            /// <summary>
            /// GyroX (I16 dps) from Service=Accelerometer and Characteristic=Accelerometer Data
            ///</summary>
            public double GyroX 
            { 
                get { return _GyroX; }
                set { if (value == _GyroX) return; _GyroX = value; OnPropertyChanged();}
            }
            private double _GyroY = 0;
            /// <summary>
            /// GyroY (I16 dps) from Service=Accelerometer and Characteristic=Accelerometer Data
            ///</summary>
            public double GyroY 
            { 
                get { return _GyroY; }
                set { if (value == _GyroY) return; _GyroY = value; OnPropertyChanged();}
            }
            private double _GyroZ = 0;
            /// <summary>
            /// GyroZ (I16 dps) from Service=Accelerometer and Characteristic=Accelerometer Data
            ///</summary>
            public double GyroZ 
            { 
                get { return _GyroZ; }
                set { if (value == _GyroZ) return; _GyroZ = value; OnPropertyChanged();}
            }
            private double _AccX = 0;
            /// <summary>
            /// AccX (I16 g) from Service=Accelerometer and Characteristic=Accelerometer Data
            ///</summary>
            public double AccX 
            { 
                get { return _AccX; }
                set { if (value == _AccX) return; _AccX = value; OnPropertyChanged();}
            }
            private double _AccY = 0;
            /// <summary>
            /// AccY (I16 g) from Service=Accelerometer and Characteristic=Accelerometer Data
            ///</summary>
            public double AccY 
            { 
                get { return _AccY; }
                set { if (value == _AccY) return; _AccY = value; OnPropertyChanged();}
            }
            private double _AccZ = 0;
            /// <summary>
            /// AccZ (I16 g) from Service=Accelerometer and Characteristic=Accelerometer Data
            ///</summary>
            public double AccZ 
            { 
                get { return _AccZ; }
                set { if (value == _AccZ) return; _AccZ = value; OnPropertyChanged();}
            }
            private double _MagnetometerX = 0;
            /// <summary>
            /// MagnetometerX (I16 microTesla) from Service=Accelerometer and Characteristic=Accelerometer Data
            ///</summary>
            public double MagnetometerX 
            { 
                get { return _MagnetometerX; }
                set { if (value == _MagnetometerX) return; _MagnetometerX = value; OnPropertyChanged();}
            }
            private double _MagnetometerY = 0;
            /// <summary>
            /// MagnetometerY (I16 microTesla) from Service=Accelerometer and Characteristic=Accelerometer Data
            ///</summary>
            public double MagnetometerY 
            { 
                get { return _MagnetometerY; }
                set { if (value == _MagnetometerY) return; _MagnetometerY = value; OnPropertyChanged();}
            }
            private double _MagnetometerZ = 0;
            /// <summary>
            /// MagnetometerZ (I16 microTesla) from Service=Accelerometer and Characteristic=Accelerometer Data
            ///</summary>
            public double MagnetometerZ 
            { 
                get { return _MagnetometerZ; }
                set { if (value == _MagnetometerZ) return; _MagnetometerZ = value; OnPropertyChanged();}
            }

            private double _Enable = 0;
            /// <summary>
            /// Enable (U16 ) from Service=Accelerometer and Characteristic=Accelerometer Config
            ///</summary>
            public double Enable 
            { 
                get { return _Enable; }
                set { if (value == _Enable) return; _Enable = value; OnPropertyChanged();}
            }

            private double _Period = 0;
            /// <summary>
            /// Period (U8 10ms) from Service=Accelerometer and Characteristic=Accelerometer Period
            ///</summary>
            public double Period 
            { 
                get { return _Period; }
                set { if (value == _Period) return; _Period = value; OnPropertyChanged();}
            }
            public override Accelerometer_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as Accelerometer_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(Accelerometer_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.GyroX = source.GyroX;
                dest.GyroY = source.GyroY;
                dest.GyroZ = source.GyroZ;
                dest.AccX = source.AccX;
                dest.AccY = source.AccY;
                dest.AccZ = source.AccZ;
                dest.MagnetometerX = source.MagnetometerX;
                dest.MagnetometerY = source.MagnetometerY;
                dest.MagnetometerZ = source.MagnetometerZ;
                dest.Enable = source.Enable;
                dest.Period = source.Period;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static Accelerometer_Data CopyToWithConvertAndCreate(Accelerometer_Data source, Accelerometer_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.GyroX = convert(source.GyroX, "dps");
                dest.GyroY = convert(source.GyroY, "dps");
                dest.GyroZ = convert(source.GyroZ, "dps");
                dest.AccX = convert(source.AccX, "g");
                dest.AccY = convert(source.AccY, "g");
                dest.AccZ = convert(source.AccZ, "g");
                dest.MagnetometerX = convert(source.MagnetometerX, "microTesla");
                dest.MagnetometerY = convert(source.MagnetometerY, "microTesla");
                dest.MagnetometerZ = convert(source.MagnetometerZ, "microTesla");
                dest.Enable = convert(source.Enable, "");
                dest.Period = convert(source.Period, "10ms");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["GyroX", "GyroY", "GyroZ", "AccX", "AccY", "AccZ", "MagnetometerX", "MagnetometerY", "MagnetometerZ", "Enable", "Period"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(GyroX);
                exporter.CellSet(GyroY);
                exporter.CellSet(GyroZ);
                exporter.CellSet(AccX);
                exporter.CellSet(AccY);
                exporter.CellSet(AccZ);
                exporter.CellSet(MagnetometerX);
                exporter.CellSet(MagnetometerY);
                exporter.CellSet(MagnetometerZ);
                exporter.CellSet(Enable);
                exporter.CellSet(Period);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {GyroX} {GyroY} {GyroZ} {AccX} {AccY} {AccZ} {MagnetometerX} {MagnetometerY} {MagnetometerZ} {Enable} {Period}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the Key Press Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class Key_Press_Data :BTCommonMetaData<Key_Press_Data> //, IExportDataSource
        {
            private double _param0 = 0;
            /// <summary>
            /// param0 (U8 ) from Service=Key Press and Characteristic=Key Press State
            ///</summary>
            public double param0 
            { 
                get { return _param0; }
                set { if (value == _param0) return; _param0 = value; OnPropertyChanged();}
            }
            public override Key_Press_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as Key_Press_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(Key_Press_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.param0 = source.param0;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static Key_Press_Data CopyToWithConvertAndCreate(Key_Press_Data source, Key_Press_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.param0 = convert(source.param0, "");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["param0"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(param0);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {param0}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the IR Service Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class IR_Service_Data :BTCommonMetaData<IR_Service_Data> //, IExportDataSource
        {
            private double _ObjTemp = 0;
            /// <summary>
            /// ObjTemp (I16 C) from Service=IR Service and Characteristic=IR Data
            ///</summary>
            public double ObjTemp 
            { 
                get { return _ObjTemp; }
                set { if (value == _ObjTemp) return; _ObjTemp = value; OnPropertyChanged();}
            }
            private double _AmbTemp = 0;
            /// <summary>
            /// AmbTemp (I16 C) from Service=IR Service and Characteristic=IR Data
            ///</summary>
            public double AmbTemp 
            { 
                get { return _AmbTemp; }
                set { if (value == _AmbTemp) return; _AmbTemp = value; OnPropertyChanged();}
            }

            private double _Enable = 0;
            /// <summary>
            /// Enable (U8 ) from Service=IR Service and Characteristic=IR Service Config
            ///</summary>
            public double Enable 
            { 
                get { return _Enable; }
                set { if (value == _Enable) return; _Enable = value; OnPropertyChanged();}
            }

            private double _Period = 0;
            /// <summary>
            /// Period (U8 10ms) from Service=IR Service and Characteristic=IR Service Period
            ///</summary>
            public double Period 
            { 
                get { return _Period; }
                set { if (value == _Period) return; _Period = value; OnPropertyChanged();}
            }
            public override IR_Service_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as IR_Service_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(IR_Service_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.ObjTemp = source.ObjTemp;
                dest.AmbTemp = source.AmbTemp;
                dest.Enable = source.Enable;
                dest.Period = source.Period;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static IR_Service_Data CopyToWithConvertAndCreate(IR_Service_Data source, IR_Service_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.ObjTemp = convert(source.ObjTemp, "C");
                dest.AmbTemp = convert(source.AmbTemp, "C");
                dest.Enable = convert(source.Enable, "");
                dest.Period = convert(source.Period, "10ms");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["ObjTemp", "AmbTemp", "Enable", "Period"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(ObjTemp);
                exporter.CellSet(AmbTemp);
                exporter.CellSet(Enable);
                exporter.CellSet(Period);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {ObjTemp} {AmbTemp} {Enable} {Period}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the Humidity Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class Humidity_Data :BTCommonMetaData<Humidity_Data> //, IExportDataSource
        {
            private double _Temp = 0;
            /// <summary>
            /// Temp (U16 ) from Service=Humidity and Characteristic=Humidity Data
            ///</summary>
            public double Temp 
            { 
                get { return _Temp; }
                set { if (value == _Temp) return; _Temp = value; OnPropertyChanged();}
            }
            private double _Humidity = 0;
            /// <summary>
            /// Humidity (U16 ) from Service=Humidity and Characteristic=Humidity Data
            ///</summary>
            public double Humidity 
            { 
                get { return _Humidity; }
                set { if (value == _Humidity) return; _Humidity = value; OnPropertyChanged();}
            }

            private double _Enable = 0;
            /// <summary>
            /// Enable (U8 ) from Service=Humidity and Characteristic=Humidity Config
            ///</summary>
            public double Enable 
            { 
                get { return _Enable; }
                set { if (value == _Enable) return; _Enable = value; OnPropertyChanged();}
            }

            private double _Period = 0;
            /// <summary>
            /// Period (U8 10ms) from Service=Humidity and Characteristic=Humidity Period
            ///</summary>
            public double Period 
            { 
                get { return _Period; }
                set { if (value == _Period) return; _Period = value; OnPropertyChanged();}
            }
            public override Humidity_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as Humidity_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(Humidity_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.Temp = source.Temp;
                dest.Humidity = source.Humidity;
                dest.Enable = source.Enable;
                dest.Period = source.Period;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static Humidity_Data CopyToWithConvertAndCreate(Humidity_Data source, Humidity_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.Temp = convert(source.Temp, "");
                dest.Humidity = convert(source.Humidity, "");
                dest.Enable = convert(source.Enable, "");
                dest.Period = convert(source.Period, "10ms");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["Temp", "Humidity", "Enable", "Period"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(Temp);
                exporter.CellSet(Humidity);
                exporter.CellSet(Enable);
                exporter.CellSet(Period);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {Temp} {Humidity} {Enable} {Period}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the Barometer Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class Barometer_Data :BTCommonMetaData<Barometer_Data> //, IExportDataSource
        {
            private double _Temp = 0;
            /// <summary>
            /// Temp (I24 C) from Service=Barometer and Characteristic=Barometer Data
            ///</summary>
            public double Temp 
            { 
                get { return _Temp; }
                set { if (value == _Temp) return; _Temp = value; OnPropertyChanged();}
            }
            private double _Pressure = 0;
            /// <summary>
            /// Pressure (I24 hPa) from Service=Barometer and Characteristic=Barometer Data
            ///</summary>
            public double Pressure 
            { 
                get { return _Pressure; }
                set { if (value == _Pressure) return; _Pressure = value; OnPropertyChanged();}
            }

            private double _Enable = 0;
            /// <summary>
            /// Enable (U8 ) from Service=Barometer and Characteristic=Barometer Config
            ///</summary>
            public double Enable 
            { 
                get { return _Enable; }
                set { if (value == _Enable) return; _Enable = value; OnPropertyChanged();}
            }

            private double _Period = 0;
            /// <summary>
            /// Period (U8 10ms) from Service=Barometer and Characteristic=Barometer Period
            ///</summary>
            public double Period 
            { 
                get { return _Period; }
                set { if (value == _Period) return; _Period = value; OnPropertyChanged();}
            }
            public override Barometer_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as Barometer_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(Barometer_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.Temp = source.Temp;
                dest.Pressure = source.Pressure;
                dest.Enable = source.Enable;
                dest.Period = source.Period;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static Barometer_Data CopyToWithConvertAndCreate(Barometer_Data source, Barometer_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.Temp = convert(source.Temp, "C");
                dest.Pressure = convert(source.Pressure, "hPa");
                dest.Enable = convert(source.Enable, "");
                dest.Period = convert(source.Period, "10ms");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["Temp", "Pressure", "Enable", "Period"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(Temp);
                exporter.CellSet(Pressure);
                exporter.CellSet(Enable);
                exporter.CellSet(Period);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {Temp} {Pressure} {Enable} {Period}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the Optical Service Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class Optical_Service_Data :BTCommonMetaData<Optical_Service_Data> //, IExportDataSource
        {
            private double _Lux = 0;
            /// <summary>
            /// Lux (U16 ) from Service=Optical Service and Characteristic=Optical Service Data
            ///</summary>
            public double Lux 
            { 
                get { return _Lux; }
                set { if (value == _Lux) return; _Lux = value; OnPropertyChanged();}
            }

            private double _Enable = 0;
            /// <summary>
            /// Enable (U8 ) from Service=Optical Service and Characteristic=Optical Service Config
            ///</summary>
            public double Enable 
            { 
                get { return _Enable; }
                set { if (value == _Enable) return; _Enable = value; OnPropertyChanged();}
            }

            private double _Period = 0;
            /// <summary>
            /// Period (U8 10ms) from Service=Optical Service and Characteristic=Optical Service Period
            ///</summary>
            public double Period 
            { 
                get { return _Period; }
                set { if (value == _Period) return; _Period = value; OnPropertyChanged();}
            }
            public override Optical_Service_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as Optical_Service_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(Optical_Service_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.Lux = source.Lux;
                dest.Enable = source.Enable;
                dest.Period = source.Period;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static Optical_Service_Data CopyToWithConvertAndCreate(Optical_Service_Data source, Optical_Service_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.Lux = convert(source.Lux, "");
                dest.Enable = convert(source.Enable, "");
                dest.Period = convert(source.Period, "10ms");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["Lux", "Enable", "Period"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(Lux);
                exporter.CellSet(Enable);
                exporter.CellSet(Period);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {Lux} {Enable} {Period}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the IO Service Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class IO_Service_Data :BTCommonMetaData<IO_Service_Data> //, IExportDataSource
        {
            private byte[] _IOData = null;
            /// <summary>
            /// IOData (BYTES ) from Service=IO Service and Characteristic=IO Service Data
            ///</summary>
            public byte[] IOData 
            { 
                get { return _IOData; }
                set { if (value == _IOData) return; _IOData = value; OnPropertyChanged();}
            }

            private double _IOConfig = 0;
            /// <summary>
            /// IOConfig (U8 ) from Service=IO Service and Characteristic=IO Service Config
            ///</summary>
            public double IOConfig 
            { 
                get { return _IOConfig; }
                set { if (value == _IOConfig) return; _IOConfig = value; OnPropertyChanged();}
            }
            public override IO_Service_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as IO_Service_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(IO_Service_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.IOData = source.IOData;
                dest.IOConfig = source.IOConfig;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static IO_Service_Data CopyToWithConvertAndCreate(IO_Service_Data source, IO_Service_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.IOData = source.IOData;
                dest.IOConfig = convert(source.IOConfig, "");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["IOData", "IOConfig"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(IOData);
                exporter.CellSet(IOConfig);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {IOData} {IOConfig}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the Register service Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class Register_service_Data :BTCommonMetaData<Register_service_Data> //, IExportDataSource
        {
            private byte[] _RegisterData = null;
            /// <summary>
            /// RegisterData (BYTES ) from Service=Register service and Characteristic=Register Data
            ///</summary>
            public byte[] RegisterData 
            { 
                get { return _RegisterData; }
                set { if (value == _RegisterData) return; _RegisterData = value; OnPropertyChanged();}
            }

            private byte[] _RegisterAddress = null;
            /// <summary>
            /// RegisterAddress (BYTES ) from Service=Register service and Characteristic=Register Address
            ///</summary>
            public byte[] RegisterAddress 
            { 
                get { return _RegisterAddress; }
                set { if (value == _RegisterAddress) return; _RegisterAddress = value; OnPropertyChanged();}
            }

            private byte[] _RegisterDeviceID = null;
            /// <summary>
            /// RegisterDeviceID (BYTES ) from Service=Register service and Characteristic=Register Device ID
            ///</summary>
            public byte[] RegisterDeviceID 
            { 
                get { return _RegisterDeviceID; }
                set { if (value == _RegisterDeviceID) return; _RegisterDeviceID = value; OnPropertyChanged();}
            }
            public override Register_service_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as Register_service_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(Register_service_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.RegisterData = source.RegisterData;
                dest.RegisterAddress = source.RegisterAddress;
                dest.RegisterDeviceID = source.RegisterDeviceID;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static Register_service_Data CopyToWithConvertAndCreate(Register_service_Data source, Register_service_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.RegisterData = source.RegisterData;
                dest.RegisterAddress = source.RegisterAddress;
                dest.RegisterDeviceID = source.RegisterDeviceID;
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["RegisterData", "RegisterAddress", "RegisterDeviceID"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(RegisterData);
                exporter.CellSet(RegisterAddress);
                exporter.CellSet(RegisterDeviceID);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {RegisterData} {RegisterAddress} {RegisterDeviceID}");
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

            private byte[] _ConnectionParameter = null;
            /// <summary>
            /// ConnectionParameter (BYTES ) from Service=Common Configuration and Characteristic=Connection Parameter
            ///</summary>
            public byte[] ConnectionParameter 
            { 
                get { return _ConnectionParameter; }
                set { if (value == _ConnectionParameter) return; _ConnectionParameter = value; OnPropertyChanged();}
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
                dest.ConnectionParameter = source.ConnectionParameter;
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
                dest.ConnectionParameter = source.ConnectionParameter;
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["Device_Name", "Appearance", "ConnectionParameter"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(Device_Name);
                exporter.CellSet(Appearance);
                exporter.CellSet(ConnectionParameter);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {Device_Name} {Appearance} {ConnectionParameter}");
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
            private string _SystemID = "";
            /// <summary>
            /// SystemID (STRING ) from Service=Device Info and Characteristic=System ID
            ///</summary>
            public string SystemID 
            { 
                get { return _SystemID; }
                set { if (value == _SystemID) return; _SystemID = value; OnPropertyChanged();}
            }

            private string _ModelNumber = "";
            /// <summary>
            /// ModelNumber (STRING ) from Service=Device Info and Characteristic=Model Number
            ///</summary>
            public string ModelNumber 
            { 
                get { return _ModelNumber; }
                set { if (value == _ModelNumber) return; _ModelNumber = value; OnPropertyChanged();}
            }

            private string _SerialNumber = "";
            /// <summary>
            /// SerialNumber (STRING ) from Service=Device Info and Characteristic=Serial Number
            ///</summary>
            public string SerialNumber 
            { 
                get { return _SerialNumber; }
                set { if (value == _SerialNumber) return; _SerialNumber = value; OnPropertyChanged();}
            }

            private string _FirmwareRevision = "";
            /// <summary>
            /// FirmwareRevision (STRING ) from Service=Device Info and Characteristic=Firmware Revision
            ///</summary>
            public string FirmwareRevision 
            { 
                get { return _FirmwareRevision; }
                set { if (value == _FirmwareRevision) return; _FirmwareRevision = value; OnPropertyChanged();}
            }

            private string _HardwareRevision = "";
            /// <summary>
            /// HardwareRevision (STRING ) from Service=Device Info and Characteristic=Hardware Revision
            ///</summary>
            public string HardwareRevision 
            { 
                get { return _HardwareRevision; }
                set { if (value == _HardwareRevision) return; _HardwareRevision = value; OnPropertyChanged();}
            }

            private string _SoftwareRevision = "";
            /// <summary>
            /// SoftwareRevision (STRING ) from Service=Device Info and Characteristic=Software Revision
            ///</summary>
            public string SoftwareRevision 
            { 
                get { return _SoftwareRevision; }
                set { if (value == _SoftwareRevision) return; _SoftwareRevision = value; OnPropertyChanged();}
            }

            private string _ManufacturerName = "";
            /// <summary>
            /// ManufacturerName (STRING ) from Service=Device Info and Characteristic=Manufacturer Name
            ///</summary>
            public string ManufacturerName 
            { 
                get { return _ManufacturerName; }
                set { if (value == _ManufacturerName) return; _ManufacturerName = value; OnPropertyChanged();}
            }

            private double _BodyType = 0;
            /// <summary>
            /// BodyType (U8 ) from Service=Device Info and Characteristic=Regulatory List
            ///</summary>
            public double BodyType 
            { 
                get { return _BodyType; }
                set { if (value == _BodyType) return; _BodyType = value; OnPropertyChanged();}
            }
            private double _BodyStructure = 0;
            /// <summary>
            /// BodyStructure (U8 ) from Service=Device Info and Characteristic=Regulatory List
            ///</summary>
            public double BodyStructure 
            { 
                get { return _BodyStructure; }
                set { if (value == _BodyStructure) return; _BodyStructure = value; OnPropertyChanged();}
            }
            private string _Data = "";
            /// <summary>
            /// Data (STRING ) from Service=Device Info and Characteristic=Regulatory List
            ///</summary>
            public string Data 
            { 
                get { return _Data; }
                set { if (value == _Data) return; _Data = value; OnPropertyChanged();}
            }

            private string _PnPID = "";
            /// <summary>
            /// PnPID (STRING ) from Service=Device Info and Characteristic=PnP ID
            ///</summary>
            public string PnPID 
            { 
                get { return _PnPID; }
                set { if (value == _PnPID) return; _PnPID = value; OnPropertyChanged();}
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
                dest.SystemID = source.SystemID;
                dest.ModelNumber = source.ModelNumber;
                dest.SerialNumber = source.SerialNumber;
                dest.FirmwareRevision = source.FirmwareRevision;
                dest.HardwareRevision = source.HardwareRevision;
                dest.SoftwareRevision = source.SoftwareRevision;
                dest.ManufacturerName = source.ManufacturerName;
                dest.BodyType = source.BodyType;
                dest.BodyStructure = source.BodyStructure;
                dest.Data = source.Data;
                dest.PnPID = source.PnPID;
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
                dest.SystemID = source.SystemID;
                dest.ModelNumber = source.ModelNumber;
                dest.SerialNumber = source.SerialNumber;
                dest.FirmwareRevision = source.FirmwareRevision;
                dest.HardwareRevision = source.HardwareRevision;
                dest.SoftwareRevision = source.SoftwareRevision;
                dest.ManufacturerName = source.ManufacturerName;
                dest.BodyType = convert(source.BodyType, "");
                dest.BodyStructure = convert(source.BodyStructure, "");
                dest.Data = source.Data;
                dest.PnPID = source.PnPID;
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["SystemID", "ModelNumber", "SerialNumber", "FirmwareRevision", "HardwareRevision", "SoftwareRevision", "ManufacturerName", "BodyType", "BodyStructure", "Data", "PnPID"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(SystemID);
                exporter.CellSet(ModelNumber);
                exporter.CellSet(SerialNumber);
                exporter.CellSet(FirmwareRevision);
                exporter.CellSet(HardwareRevision);
                exporter.CellSet(SoftwareRevision);
                exporter.CellSet(ManufacturerName);
                exporter.CellSet(BodyType);
                exporter.CellSet(BodyStructure);
                exporter.CellSet(Data);
                exporter.CellSet(PnPID);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {SystemID} {ModelNumber} {SerialNumber} {FirmwareRevision} {HardwareRevision} {SoftwareRevision} {ManufacturerName} {BodyType} {BodyStructure} {Data} {PnPID}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the Battery Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class Battery_Data :BTCommonMetaData<Battery_Data> //, IExportDataSource
        {
            private double _BatteryLevel = 0;
            /// <summary>
            /// BatteryLevel (I8 %) from Service=Battery and Characteristic=BatteryLevel
            ///</summary>
            public double BatteryLevel 
            { 
                get { return _BatteryLevel; }
                set { if (value == _BatteryLevel) return; _BatteryLevel = value; OnPropertyChanged();}
            }
            public override Battery_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as Battery_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(Battery_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.BatteryLevel = source.BatteryLevel;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static Battery_Data CopyToWithConvertAndCreate(Battery_Data source, Battery_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.BatteryLevel = convert(source.BatteryLevel, "%");
                return dest;
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


        #endregion


        /// <summary>
        /// Enumeration of all services
        /// </summary>
        enum ServiceIndex
        {
            Accelerometer_index = 0,
            Key_Press_index = 1,
            IR_Service_index = 2,
            Humidity_index = 3,
            Barometer_index = 4,
            Optical_Service_index = 5,
            IO_Service_index = 6,
            Register_service_index = 7,
            Common_Configuration_index = 8,
            Device_Info_index = 9,
            Battery_index = 10,
        }

        /// <summary>
        /// Enumeration of all characteristics in all of the services.
        /// </summary>
        enum CharacteristicIndex
        {
            Accelerometer_Accelerometer_Data_index = 0,     // GUID f000aa81-0451-4000-b000-000000000000
            Accelerometer_Accelerometer_Config_index = 1,     // GUID f000aa82-0451-4000-b000-000000000000
            Accelerometer_Accelerometer_Period_index = 2,     // GUID f000aa83-0451-4000-b000-000000000000
            Key_Press_Key_Press_State_index = 3,     // GUID 0000ffe1-0000-1000-8000-00805f9b34fb
            IR_Service_IR_Data_index = 4,     // GUID f000aa01-0451-4000-b000-000000000000
            IR_Service_IR_Service_Config_index = 5,     // GUID f000aa02-0451-4000-b000-000000000000
            IR_Service_IR_Service_Period_index = 6,     // GUID f000aa03-0451-4000-b000-000000000000
            Humidity_Humidity_Data_index = 7,     // GUID f000aa21-0451-4000-b000-000000000000
            Humidity_Humidity_Config_index = 8,     // GUID f000aa22-0451-4000-b000-000000000000
            Humidity_Humidity_Period_index = 9,     // GUID f000aa23-0451-4000-b000-000000000000
            Barometer_Barometer_Data_index = 10,     // GUID f000aa41-0451-4000-b000-000000000000
            Barometer_Barometer_Config_index = 11,     // GUID f000aa42-0451-4000-b000-000000000000
            Barometer_Barometer_Period_index = 12,     // GUID f000aa44-0451-4000-b000-000000000000
            Optical_Service_Optical_Service_Data_index = 13,     // GUID f000aa71-0451-4000-b000-000000000000
            Optical_Service_Optical_Service_Config_index = 14,     // GUID f000aa72-0451-4000-b000-000000000000
            Optical_Service_Optical_Service_Period_index = 15,     // GUID f000aa73-0451-4000-b000-000000000000
            IO_Service_IO_Service_Data_index = 16,     // GUID f000aa65-0451-4000-b000-000000000000
            IO_Service_IO_Service_Config_index = 17,     // GUID f000aa66-0451-4000-b000-000000000000
            Register_service_Register_Data_index = 18,     // GUID f000ac01-0451-4000-b000-000000000000
            Register_service_Register_Address_index = 19,     // GUID f000ac02-0451-4000-b000-000000000000
            Register_service_Register_Device_ID_index = 20,     // GUID f000ac03-0451-4000-b000-000000000000
            Common_Configuration_Device_Name_index = 21,     // GUID 00002a00-0000-1000-8000-00805f9b34fb
            Common_Configuration_Appearance_index = 22,     // GUID 00002a01-0000-1000-8000-00805f9b34fb
            Common_Configuration_Connection_Parameter_index = 23,     // GUID 00002a04-0000-1000-8000-00805f9b34fb
            Device_Info_System_ID_index = 24,     // GUID 00002a23-0000-1000-8000-00805f9b34fb
            Device_Info_Model_Number_index = 25,     // GUID 00002a24-0000-1000-8000-00805f9b34fb
            Device_Info_Serial_Number_index = 26,     // GUID 00002a25-0000-1000-8000-00805f9b34fb
            Device_Info_Firmware_Revision_index = 27,     // GUID 00002a26-0000-1000-8000-00805f9b34fb
            Device_Info_Hardware_Revision_index = 28,     // GUID 00002a27-0000-1000-8000-00805f9b34fb
            Device_Info_Software_Revision_index = 29,     // GUID 00002a28-0000-1000-8000-00805f9b34fb
            Device_Info_Manufacturer_Name_index = 30,     // GUID 00002a29-0000-1000-8000-00805f9b34fb
            Device_Info_Regulatory_List_index = 31,     // GUID 00002a2a-0000-1000-8000-00805f9b34fb
            Device_Info_PnP_ID_index = 32,     // GUID 00002a50-0000-1000-8000-00805f9b34fb
            Battery_BatteryLevel_index = 33,     // GUID 00002a19-0000-1000-8000-00805f9b34fb
        }

        /// <summary>
        /// List of the guids supported by the device. 
        /// </summary>
        List<Guid> Service_Guids = new List<Guid>()
        {
            Guid.Parse("f000aa80-0451-4000-b000-000000000000"), // #0 is Accelerometer
            Guid.Parse("0000ffe0-0000-1000-8000-00805f9b34fb"), // #1 is Key Press
            Guid.Parse("f000aa00-0451-4000-b000-000000000000"), // #2 is IR Service
            Guid.Parse("f000aa20-0451-4000-b000-000000000000"), // #3 is Humidity
            Guid.Parse("f000aa40-0451-4000-b000-000000000000"), // #4 is Barometer
            Guid.Parse("f000aa70-0451-4000-b000-000000000000"), // #5 is Optical Service
            Guid.Parse("f000aa64-0451-4000-b000-000000000000"), // #6 is IO Service
            Guid.Parse("f000ac00-0451-4000-b000-000000000000"), // #7 is Register service
            Guid.Parse("00001800-0000-1000-8000-00805f9b34fb"), // #8 is Common Configuration
            Guid.Parse("0000180a-0000-1000-8000-00805f9b34fb"), // #9 is Device Info
            Guid.Parse("0000180f-0000-1000-8000-00805f9b34fb"), // #10 is Battery
        };

        /// <summary>
        /// Active list of services. Will be filled in as the services are connected. Starts off as null.
        /// </summary>
        List<GattDeviceService> Services = new List<GattDeviceService>() { null, null, null, null, null, null, null, null, null, null, null, };

        /// <summary>
        /// List of the Characteristic GUIDS for all of the characteristics for all of the services.
        /// Is indexed by the CharacteristicIndex enum. 
        /// </summary>
        List<Guid> Characteristic_Guids = new List<Guid>()
        {
            Guid.Parse("f000aa81-0451-4000-b000-000000000000"), // #0 is Accelerometer Accelerometer Data
            Guid.Parse("f000aa82-0451-4000-b000-000000000000"), // #1 is Accelerometer Accelerometer Config
            Guid.Parse("f000aa83-0451-4000-b000-000000000000"), // #2 is Accelerometer Accelerometer Period
            Guid.Parse("0000ffe1-0000-1000-8000-00805f9b34fb"), // #3 is Key Press Key Press State
            Guid.Parse("f000aa01-0451-4000-b000-000000000000"), // #4 is IR Service IR Data
            Guid.Parse("f000aa02-0451-4000-b000-000000000000"), // #5 is IR Service IR Service Config
            Guid.Parse("f000aa03-0451-4000-b000-000000000000"), // #6 is IR Service IR Service Period
            Guid.Parse("f000aa21-0451-4000-b000-000000000000"), // #7 is Humidity Humidity Data
            Guid.Parse("f000aa22-0451-4000-b000-000000000000"), // #8 is Humidity Humidity Config
            Guid.Parse("f000aa23-0451-4000-b000-000000000000"), // #9 is Humidity Humidity Period
            Guid.Parse("f000aa41-0451-4000-b000-000000000000"), // #10 is Barometer Barometer Data
            Guid.Parse("f000aa42-0451-4000-b000-000000000000"), // #11 is Barometer Barometer Config
            Guid.Parse("f000aa44-0451-4000-b000-000000000000"), // #12 is Barometer Barometer Period
            Guid.Parse("f000aa71-0451-4000-b000-000000000000"), // #13 is Optical Service Optical Service Data
            Guid.Parse("f000aa72-0451-4000-b000-000000000000"), // #14 is Optical Service Optical Service Config
            Guid.Parse("f000aa73-0451-4000-b000-000000000000"), // #15 is Optical Service Optical Service Period
            Guid.Parse("f000aa65-0451-4000-b000-000000000000"), // #16 is IO Service IO Service Data
            Guid.Parse("f000aa66-0451-4000-b000-000000000000"), // #17 is IO Service IO Service Config
            Guid.Parse("f000ac01-0451-4000-b000-000000000000"), // #18 is Register service Register Data
            Guid.Parse("f000ac02-0451-4000-b000-000000000000"), // #19 is Register service Register Address
            Guid.Parse("f000ac03-0451-4000-b000-000000000000"), // #20 is Register service Register Device ID
            Guid.Parse("00002a00-0000-1000-8000-00805f9b34fb"), // #21 is Common Configuration Device Name
            Guid.Parse("00002a01-0000-1000-8000-00805f9b34fb"), // #22 is Common Configuration Appearance
            Guid.Parse("00002a04-0000-1000-8000-00805f9b34fb"), // #23 is Common Configuration Connection Parameter
            Guid.Parse("00002a23-0000-1000-8000-00805f9b34fb"), // #24 is Device Info System ID
            Guid.Parse("00002a24-0000-1000-8000-00805f9b34fb"), // #25 is Device Info Model Number
            Guid.Parse("00002a25-0000-1000-8000-00805f9b34fb"), // #26 is Device Info Serial Number
            Guid.Parse("00002a26-0000-1000-8000-00805f9b34fb"), // #27 is Device Info Firmware Revision
            Guid.Parse("00002a27-0000-1000-8000-00805f9b34fb"), // #28 is Device Info Hardware Revision
            Guid.Parse("00002a28-0000-1000-8000-00805f9b34fb"), // #29 is Device Info Software Revision
            Guid.Parse("00002a29-0000-1000-8000-00805f9b34fb"), // #30 is Device Info Manufacturer Name
            Guid.Parse("00002a2a-0000-1000-8000-00805f9b34fb"), // #31 is Device Info Regulatory List
            Guid.Parse("00002a50-0000-1000-8000-00805f9b34fb"), // #32 is Device Info PnP ID
            Guid.Parse("00002a19-0000-1000-8000-00805f9b34fb"), // #33 is Battery BatteryLevel
        };

        private List<GattCharacteristic> Characteristics = new List<GattCharacteristic>() { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,  };
        private List<bool> NotifyCharacteristic_ValueChanged_set = new List<bool> { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false,  };
        private List<IotNumberFormats.ValueParser> ValueParsers = new List<IotNumberFormats.ValueParser>() {  null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,  };


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


        #region Service_Accelerometer
        // Service Accelerometer 

        public Accelerometer_Data CurrAccelerometer_Data { get; set; } = new Accelerometer_Data();

        // Per-characteristics methods for Accelerometer Accelerometer_Data
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyAccelerometer_DataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Accelerometer_Data", ServiceIndex.Accelerometer_index, "Accelerometer", CharacteristicIndex.Accelerometer_Accelerometer_Data_index, NotifyAccelerometer_DataCallback, notifyType);
            return retval;
        }

        private void NotifyAccelerometer_DataCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Accelerometer_Accelerometer_Data_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("I16^500_*_65536_/|FIXED^N3|GyroX|dps I16^500_*_65536_/|FIXED^N3|GyroY|dps I16^500_*_65536_/|FIXED^N3|GyroZ|dps I16^8_*_32768_/|FIXED^N3|AccX|g I16^8_*_32768_/|FIXED^N3|AccY|g I16^8_*_32768_/|FIXED^N3|AccZ|g I16|DEC|MagnetometerX|microTesla I16|DEC|MagnetometerY|microTesla I16|DEC|MagnetometerZ|microTesla");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrAccelerometer_Data.TimestampMostRecent = args.Timestamp;
            CurrAccelerometer_Data.GyroX = vr.GetNextDouble();
            CurrAccelerometer_Data.GyroY = vr.GetNextDouble();
            CurrAccelerometer_Data.GyroZ = vr.GetNextDouble();
            CurrAccelerometer_Data.AccX = vr.GetNextDouble();
            CurrAccelerometer_Data.AccY = vr.GetNextDouble();
            CurrAccelerometer_Data.AccZ = vr.GetNextDouble();
            CurrAccelerometer_Data.MagnetometerX = vr.GetNextDouble();
            CurrAccelerometer_Data.MagnetometerY = vr.GetNextDouble();
            CurrAccelerometer_Data.MagnetometerZ = vr.GetNextDouble();
            OnPropertyChanged(Accelerometer_DataPropertyChangedName); // "Accelerometer_Data"
        }
        // Per-characteristics methods for Accelerometer Accelerometer_Config
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyAccelerometer_ConfigAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Accelerometer_Config", ServiceIndex.Accelerometer_index, "Accelerometer", CharacteristicIndex.Accelerometer_Accelerometer_Config_index, NotifyAccelerometer_ConfigCallback, notifyType);
            return retval;
        }

        private void NotifyAccelerometer_ConfigCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Accelerometer_Accelerometer_Config_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U16|HEX|Enable");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrAccelerometer_Data.TimestampMostRecent = args.Timestamp;
            CurrAccelerometer_Data.Enable = vr.GetNextDouble();
            OnPropertyChanged(Accelerometer_ConfigPropertyChangedName); // "Accelerometer_Config"
        }
        // Per-characteristics methods for Accelerometer Accelerometer_Period
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyAccelerometer_PeriodAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Accelerometer_Period", ServiceIndex.Accelerometer_index, "Accelerometer", CharacteristicIndex.Accelerometer_Accelerometer_Period_index, NotifyAccelerometer_PeriodCallback, notifyType);
            return retval;
        }

        private void NotifyAccelerometer_PeriodCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Accelerometer_Accelerometer_Period_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|DEC|Period|10ms");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrAccelerometer_Data.TimestampMostRecent = args.Timestamp;
            CurrAccelerometer_Data.Period = vr.GetNextDouble();
            OnPropertyChanged(Accelerometer_PeriodPropertyChangedName); // "Accelerometer_Period"
        }
        /// <summary>
        /// Reads data from Accelerometer Data and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Accelerometer_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Accelerometer_Data> ReadAccelerometer_Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Accelerometer_Accelerometer_Data_index;
            await Ensure_Characteristic_Async(ServiceIndex.Accelerometer_index, "Accelerometer", index, "Accelerometer Data");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Accelerometer Data", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("I16^500_*_65536_/|FIXED^N3|GyroX|dps I16^500_*_65536_/|FIXED^N3|GyroY|dps I16^500_*_65536_/|FIXED^N3|GyroZ|dps I16^8_*_32768_/|FIXED^N3|AccX|g I16^8_*_32768_/|FIXED^N3|AccY|g I16^8_*_32768_/|FIXED^N3|AccZ|g I16|DEC|MagnetometerX|microTesla I16|DEC|MagnetometerY|microTesla I16|DEC|MagnetometerZ|microTesla");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrAccelerometer_Data.GyroX = vr.GetNextDouble();
            CurrAccelerometer_Data.GyroY = vr.GetNextDouble();
            CurrAccelerometer_Data.GyroZ = vr.GetNextDouble();
            CurrAccelerometer_Data.AccX = vr.GetNextDouble();
            CurrAccelerometer_Data.AccY = vr.GetNextDouble();
            CurrAccelerometer_Data.AccZ = vr.GetNextDouble();
            CurrAccelerometer_Data.MagnetometerX = vr.GetNextDouble();
            CurrAccelerometer_Data.MagnetometerY = vr.GetNextDouble();
            CurrAccelerometer_Data.MagnetometerZ = vr.GetNextDouble();
            CurrAccelerometer_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Accelerometer_DataPropertyChangedName); // "Accelerometer_Data"
            return CurrAccelerometer_Data;
        }
        /// <summary>
        /// Reads data from Accelerometer Config and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Accelerometer_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Accelerometer_Data> ReadAccelerometer_Config(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Accelerometer_Accelerometer_Config_index;
            await Ensure_Characteristic_Async(ServiceIndex.Accelerometer_index, "Accelerometer", index, "Accelerometer Config");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Accelerometer Config", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U16|HEX|Enable");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrAccelerometer_Data.Enable = vr.GetNextDouble();
            CurrAccelerometer_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Accelerometer_ConfigPropertyChangedName); // "Accelerometer_Config"
            return CurrAccelerometer_Data;
        }
        /// <summary>
        /// Reads data from Accelerometer Period and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Accelerometer_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Accelerometer_Data> ReadAccelerometer_Period(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Accelerometer_Accelerometer_Period_index;
            await Ensure_Characteristic_Async(ServiceIndex.Accelerometer_index, "Accelerometer", index, "Accelerometer Period");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Accelerometer Period", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|DEC|Period|10ms");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrAccelerometer_Data.Period = vr.GetNextDouble();
            CurrAccelerometer_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Accelerometer_PeriodPropertyChangedName); // "Accelerometer_Period"
            return CurrAccelerometer_Data;
        }
        /// <summary>
        /// Writes data to Accelerometer Config 
        /// </summary>
        public async Task WriteAccelerometer_Config(byte[] data)
        {
            var index = CharacteristicIndex.Accelerometer_Accelerometer_Config_index;
            await Ensure_Characteristic_Async(ServiceIndex.Accelerometer_index, "Accelerometer", index, "Accelerometer Config");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteAccelerometer_Config", result);
        }
        /// <summary>
        /// Writes data to Accelerometer Period 
        /// </summary>
        public async Task WriteAccelerometer_Period(byte[] data)
        {
            var index = CharacteristicIndex.Accelerometer_Accelerometer_Period_index;
            await Ensure_Characteristic_Async(ServiceIndex.Accelerometer_index, "Accelerometer", index, "Accelerometer Period");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteAccelerometer_Period", result);
        }

        #endregion
//
        #region Service_Key_Press
        // Service Key Press 

        public Key_Press_Data CurrKey_Press_Data { get; set; } = new Key_Press_Data();

        // Per-characteristics methods for Key_Press Key_Press_State
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyKey_Press_StateAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Key_Press_State", ServiceIndex.Key_Press_index, "Key Press", CharacteristicIndex.Key_Press_Key_Press_State_index, NotifyKey_Press_StateCallback, notifyType);
            return retval;
        }

        private void NotifyKey_Press_StateCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Key_Press_Key_Press_State_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrKey_Press_Data.TimestampMostRecent = args.Timestamp;
            CurrKey_Press_Data.param0 = vr.GetNextDouble();
            OnPropertyChanged(Key_Press_StatePropertyChangedName); // "Key_Press_State"
        }
        /// <summary>
        /// Reads data from Key Press State and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Key_Press_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Key_Press_Data> ReadKey_Press_State(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Key_Press_Key_Press_State_index;
            await Ensure_Characteristic_Async(ServiceIndex.Key_Press_index, "Key Press", index, "Key Press State");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Key Press State", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrKey_Press_Data.param0 = vr.GetNextDouble();
            CurrKey_Press_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Key_Press_StatePropertyChangedName); // "Key_Press_State"
            return CurrKey_Press_Data;
        }

        #endregion
//
        #region Service_IR_Service
        // Service IR Service 

        public IR_Service_Data CurrIR_Service_Data { get; set; } = new IR_Service_Data();

        // Per-characteristics methods for IR_Service IR_Data
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyIR_DataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("IR_Data", ServiceIndex.IR_Service_index, "IR Service", CharacteristicIndex.IR_Service_IR_Data_index, NotifyIR_DataCallback, notifyType);
            return retval;
        }

        private void NotifyIR_DataCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.IR_Service_IR_Data_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("I16^4_/_0.03125_*|FIXED|ObjTemp|C I16^4_/_0.03125_*|FIXED|AmbTemp|C");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrIR_Service_Data.TimestampMostRecent = args.Timestamp;
            CurrIR_Service_Data.ObjTemp = vr.GetNextDouble();
            CurrIR_Service_Data.AmbTemp = vr.GetNextDouble();
            OnPropertyChanged(IR_DataPropertyChangedName); // "IR_Data"
        }
        // Per-characteristics methods for IR_Service IR_Service_Config
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyIR_Service_ConfigAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("IR_Service_Config", ServiceIndex.IR_Service_index, "IR Service", CharacteristicIndex.IR_Service_IR_Service_Config_index, NotifyIR_Service_ConfigCallback, notifyType);
            return retval;
        }

        private void NotifyIR_Service_ConfigCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.IR_Service_IR_Service_Config_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX|Enable");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrIR_Service_Data.TimestampMostRecent = args.Timestamp;
            CurrIR_Service_Data.Enable = vr.GetNextDouble();
            OnPropertyChanged(IR_Service_ConfigPropertyChangedName); // "IR_Service_Config"
        }
        // Per-characteristics methods for IR_Service IR_Service_Period
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyIR_Service_PeriodAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("IR_Service_Period", ServiceIndex.IR_Service_index, "IR Service", CharacteristicIndex.IR_Service_IR_Service_Period_index, NotifyIR_Service_PeriodCallback, notifyType);
            return retval;
        }

        private void NotifyIR_Service_PeriodCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.IR_Service_IR_Service_Period_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|DEC|Period|10ms");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrIR_Service_Data.TimestampMostRecent = args.Timestamp;
            CurrIR_Service_Data.Period = vr.GetNextDouble();
            OnPropertyChanged(IR_Service_PeriodPropertyChangedName); // "IR_Service_Period"
        }
        /// <summary>
        /// Reads data from IR Data and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>IR_Service_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<IR_Service_Data> ReadIR_Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.IR_Service_IR_Data_index;
            await Ensure_Characteristic_Async(ServiceIndex.IR_Service_index, "IR Service", index, "IR Data");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "IR Data", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("I16^4_/_0.03125_*|FIXED|ObjTemp|C I16^4_/_0.03125_*|FIXED|AmbTemp|C");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrIR_Service_Data.ObjTemp = vr.GetNextDouble();
            CurrIR_Service_Data.AmbTemp = vr.GetNextDouble();
            CurrIR_Service_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(IR_DataPropertyChangedName); // "IR_Data"
            return CurrIR_Service_Data;
        }
        /// <summary>
        /// Reads data from IR Service Config and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>IR_Service_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<IR_Service_Data> ReadIR_Service_Config(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.IR_Service_IR_Service_Config_index;
            await Ensure_Characteristic_Async(ServiceIndex.IR_Service_index, "IR Service", index, "IR Service Config");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "IR Service Config", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX|Enable");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrIR_Service_Data.Enable = vr.GetNextDouble();
            CurrIR_Service_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(IR_Service_ConfigPropertyChangedName); // "IR_Service_Config"
            return CurrIR_Service_Data;
        }
        /// <summary>
        /// Reads data from IR Service Period and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>IR_Service_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<IR_Service_Data> ReadIR_Service_Period(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.IR_Service_IR_Service_Period_index;
            await Ensure_Characteristic_Async(ServiceIndex.IR_Service_index, "IR Service", index, "IR Service Period");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "IR Service Period", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|DEC|Period|10ms");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrIR_Service_Data.Period = vr.GetNextDouble();
            CurrIR_Service_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(IR_Service_PeriodPropertyChangedName); // "IR_Service_Period"
            return CurrIR_Service_Data;
        }
        /// <summary>
        /// Writes data to IR Service Config 
        /// </summary>
        public async Task WriteIR_Service_Config(byte[] data)
        {
            var index = CharacteristicIndex.IR_Service_IR_Service_Config_index;
            await Ensure_Characteristic_Async(ServiceIndex.IR_Service_index, "IR Service", index, "IR Service Config");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteIR_Service_Config", result);
        }
        /// <summary>
        /// Writes data to IR Service Period 
        /// </summary>
        public async Task WriteIR_Service_Period(byte[] data)
        {
            var index = CharacteristicIndex.IR_Service_IR_Service_Period_index;
            await Ensure_Characteristic_Async(ServiceIndex.IR_Service_index, "IR Service", index, "IR Service Period");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteIR_Service_Period", result);
        }

        #endregion
//
        #region Service_Humidity
        // Service Humidity 

        public Humidity_Data CurrHumidity_Data { get; set; } = new Humidity_Data();

        // Per-characteristics methods for Humidity Humidity_Data
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyHumidity_DataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Humidity_Data", ServiceIndex.Humidity_index, "Humidity", CharacteristicIndex.Humidity_Humidity_Data_index, NotifyHumidity_DataCallback, notifyType);
            return retval;
        }

        private void NotifyHumidity_DataCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Humidity_Humidity_Data_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U16^DU_3_ZE_175.72_*_65536_/_46.85_-_SW_2_JZ_PO_NP|FIXED|Temp U16^DU_3_ZE_125.0_*_65536_/_6.0_-_SW_2_JZ_PO_NP|FIXED|Humidity");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrHumidity_Data.TimestampMostRecent = args.Timestamp;
            CurrHumidity_Data.Temp = vr.GetNextDouble();
            CurrHumidity_Data.Humidity = vr.GetNextDouble();
            OnPropertyChanged(Humidity_DataPropertyChangedName); // "Humidity_Data"
        }
        // Per-characteristics methods for Humidity Humidity_Config
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyHumidity_ConfigAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Humidity_Config", ServiceIndex.Humidity_index, "Humidity", CharacteristicIndex.Humidity_Humidity_Config_index, NotifyHumidity_ConfigCallback, notifyType);
            return retval;
        }

        private void NotifyHumidity_ConfigCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Humidity_Humidity_Config_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX|Enable");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrHumidity_Data.TimestampMostRecent = args.Timestamp;
            CurrHumidity_Data.Enable = vr.GetNextDouble();
            OnPropertyChanged(Humidity_ConfigPropertyChangedName); // "Humidity_Config"
        }
        // Per-characteristics methods for Humidity Humidity_Period
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyHumidity_PeriodAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Humidity_Period", ServiceIndex.Humidity_index, "Humidity", CharacteristicIndex.Humidity_Humidity_Period_index, NotifyHumidity_PeriodCallback, notifyType);
            return retval;
        }

        private void NotifyHumidity_PeriodCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Humidity_Humidity_Period_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|DEC|Period|10ms");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrHumidity_Data.TimestampMostRecent = args.Timestamp;
            CurrHumidity_Data.Period = vr.GetNextDouble();
            OnPropertyChanged(Humidity_PeriodPropertyChangedName); // "Humidity_Period"
        }
        /// <summary>
        /// Reads data from Humidity Data and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Humidity_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Humidity_Data> ReadHumidity_Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Humidity_Humidity_Data_index;
            await Ensure_Characteristic_Async(ServiceIndex.Humidity_index, "Humidity", index, "Humidity Data");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Humidity Data", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U16^DU_3_ZE_175.72_*_65536_/_46.85_-_SW_2_JZ_PO_NP|FIXED|Temp U16^DU_3_ZE_125.0_*_65536_/_6.0_-_SW_2_JZ_PO_NP|FIXED|Humidity");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrHumidity_Data.Temp = vr.GetNextDouble();
            CurrHumidity_Data.Humidity = vr.GetNextDouble();
            CurrHumidity_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Humidity_DataPropertyChangedName); // "Humidity_Data"
            return CurrHumidity_Data;
        }
        /// <summary>
        /// Reads data from Humidity Config and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Humidity_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Humidity_Data> ReadHumidity_Config(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Humidity_Humidity_Config_index;
            await Ensure_Characteristic_Async(ServiceIndex.Humidity_index, "Humidity", index, "Humidity Config");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Humidity Config", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX|Enable");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrHumidity_Data.Enable = vr.GetNextDouble();
            CurrHumidity_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Humidity_ConfigPropertyChangedName); // "Humidity_Config"
            return CurrHumidity_Data;
        }
        /// <summary>
        /// Reads data from Humidity Period and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Humidity_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Humidity_Data> ReadHumidity_Period(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Humidity_Humidity_Period_index;
            await Ensure_Characteristic_Async(ServiceIndex.Humidity_index, "Humidity", index, "Humidity Period");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Humidity Period", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|DEC|Period|10ms");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrHumidity_Data.Period = vr.GetNextDouble();
            CurrHumidity_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Humidity_PeriodPropertyChangedName); // "Humidity_Period"
            return CurrHumidity_Data;
        }
        /// <summary>
        /// Writes data to Humidity Config 
        /// </summary>
        public async Task WriteHumidity_Config(byte[] data)
        {
            var index = CharacteristicIndex.Humidity_Humidity_Config_index;
            await Ensure_Characteristic_Async(ServiceIndex.Humidity_index, "Humidity", index, "Humidity Config");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteHumidity_Config", result);
        }
        /// <summary>
        /// Writes data to Humidity Period 
        /// </summary>
        public async Task WriteHumidity_Period(byte[] data)
        {
            var index = CharacteristicIndex.Humidity_Humidity_Period_index;
            await Ensure_Characteristic_Async(ServiceIndex.Humidity_index, "Humidity", index, "Humidity Period");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteHumidity_Period", result);
        }

        #endregion
//
        #region Service_Barometer
        // Service Barometer 

        public Barometer_Data CurrBarometer_Data { get; set; } = new Barometer_Data();

        // Per-characteristics methods for Barometer Barometer_Data
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyBarometer_DataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Barometer_Data", ServiceIndex.Barometer_index, "Barometer", CharacteristicIndex.Barometer_Barometer_Data_index, NotifyBarometer_DataCallback, notifyType);
            return retval;
        }

        private void NotifyBarometer_DataCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Barometer_Barometer_Data_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("I24^100_/|FIXED|Temp|C I24^100_/|FIXED|Pressure|hPa");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrBarometer_Data.TimestampMostRecent = args.Timestamp;
            CurrBarometer_Data.Temp = vr.GetNextDouble();
            CurrBarometer_Data.Pressure = vr.GetNextDouble();
            OnPropertyChanged(Barometer_DataPropertyChangedName); // "Barometer_Data"
        }
        // Per-characteristics methods for Barometer Barometer_Config
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyBarometer_ConfigAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Barometer_Config", ServiceIndex.Barometer_index, "Barometer", CharacteristicIndex.Barometer_Barometer_Config_index, NotifyBarometer_ConfigCallback, notifyType);
            return retval;
        }

        private void NotifyBarometer_ConfigCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Barometer_Barometer_Config_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX|Enable");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrBarometer_Data.TimestampMostRecent = args.Timestamp;
            CurrBarometer_Data.Enable = vr.GetNextDouble();
            OnPropertyChanged(Barometer_ConfigPropertyChangedName); // "Barometer_Config"
        }
        // Per-characteristics methods for Barometer Barometer_Period
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyBarometer_PeriodAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Barometer_Period", ServiceIndex.Barometer_index, "Barometer", CharacteristicIndex.Barometer_Barometer_Period_index, NotifyBarometer_PeriodCallback, notifyType);
            return retval;
        }

        private void NotifyBarometer_PeriodCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Barometer_Barometer_Period_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|DEC|Period|10ms");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrBarometer_Data.TimestampMostRecent = args.Timestamp;
            CurrBarometer_Data.Period = vr.GetNextDouble();
            OnPropertyChanged(Barometer_PeriodPropertyChangedName); // "Barometer_Period"
        }
        /// <summary>
        /// Reads data from Barometer Data and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Barometer_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Barometer_Data> ReadBarometer_Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Barometer_Barometer_Data_index;
            await Ensure_Characteristic_Async(ServiceIndex.Barometer_index, "Barometer", index, "Barometer Data");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Barometer Data", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("I24^100_/|FIXED|Temp|C I24^100_/|FIXED|Pressure|hPa");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrBarometer_Data.Temp = vr.GetNextDouble();
            CurrBarometer_Data.Pressure = vr.GetNextDouble();
            CurrBarometer_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Barometer_DataPropertyChangedName); // "Barometer_Data"
            return CurrBarometer_Data;
        }
        /// <summary>
        /// Reads data from Barometer Config and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Barometer_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Barometer_Data> ReadBarometer_Config(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Barometer_Barometer_Config_index;
            await Ensure_Characteristic_Async(ServiceIndex.Barometer_index, "Barometer", index, "Barometer Config");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Barometer Config", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX|Enable");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrBarometer_Data.Enable = vr.GetNextDouble();
            CurrBarometer_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Barometer_ConfigPropertyChangedName); // "Barometer_Config"
            return CurrBarometer_Data;
        }
        /// <summary>
        /// Reads data from Barometer Period and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Barometer_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Barometer_Data> ReadBarometer_Period(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Barometer_Barometer_Period_index;
            await Ensure_Characteristic_Async(ServiceIndex.Barometer_index, "Barometer", index, "Barometer Period");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Barometer Period", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|DEC|Period|10ms");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrBarometer_Data.Period = vr.GetNextDouble();
            CurrBarometer_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Barometer_PeriodPropertyChangedName); // "Barometer_Period"
            return CurrBarometer_Data;
        }
        /// <summary>
        /// Writes data to Barometer Config 
        /// </summary>
        public async Task WriteBarometer_Config(byte[] data)
        {
            var index = CharacteristicIndex.Barometer_Barometer_Config_index;
            await Ensure_Characteristic_Async(ServiceIndex.Barometer_index, "Barometer", index, "Barometer Config");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteBarometer_Config", result);
        }
        /// <summary>
        /// Writes data to Barometer Period 
        /// </summary>
        public async Task WriteBarometer_Period(byte[] data)
        {
            var index = CharacteristicIndex.Barometer_Barometer_Period_index;
            await Ensure_Characteristic_Async(ServiceIndex.Barometer_index, "Barometer", index, "Barometer Period");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteBarometer_Period", result);
        }

        #endregion
//
        #region Service_Optical_Service
        // Service Optical Service 

        public Optical_Service_Data CurrOptical_Service_Data { get; set; } = new Optical_Service_Data();

        // Per-characteristics methods for Optical_Service Optical_Service_Data
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyOptical_Service_DataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Optical_Service_Data", ServiceIndex.Optical_Service_index, "Optical Service", CharacteristicIndex.Optical_Service_Optical_Service_Data_index, NotifyOptical_Service_DataCallback, notifyType);
            return retval;
        }

        private void NotifyOptical_Service_DataCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Optical_Service_Optical_Service_Data_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U16^DU_12_RS_15_AN_2_XY_0.01_*_SW_4095_AN_*|FIXED^N1|Lux");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrOptical_Service_Data.TimestampMostRecent = args.Timestamp;
            CurrOptical_Service_Data.Lux = vr.GetNextDouble();
            OnPropertyChanged(Optical_Service_DataPropertyChangedName); // "Optical_Service_Data"
        }
        // Per-characteristics methods for Optical_Service Optical_Service_Config
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyOptical_Service_ConfigAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Optical_Service_Config", ServiceIndex.Optical_Service_index, "Optical Service", CharacteristicIndex.Optical_Service_Optical_Service_Config_index, NotifyOptical_Service_ConfigCallback, notifyType);
            return retval;
        }

        private void NotifyOptical_Service_ConfigCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Optical_Service_Optical_Service_Config_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX|Enable");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrOptical_Service_Data.TimestampMostRecent = args.Timestamp;
            CurrOptical_Service_Data.Enable = vr.GetNextDouble();
            OnPropertyChanged(Optical_Service_ConfigPropertyChangedName); // "Optical_Service_Config"
        }
        // Per-characteristics methods for Optical_Service Optical_Service_Period
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyOptical_Service_PeriodAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Optical_Service_Period", ServiceIndex.Optical_Service_index, "Optical Service", CharacteristicIndex.Optical_Service_Optical_Service_Period_index, NotifyOptical_Service_PeriodCallback, notifyType);
            return retval;
        }

        private void NotifyOptical_Service_PeriodCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Optical_Service_Optical_Service_Period_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|DEC|Period|10ms");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrOptical_Service_Data.TimestampMostRecent = args.Timestamp;
            CurrOptical_Service_Data.Period = vr.GetNextDouble();
            OnPropertyChanged(Optical_Service_PeriodPropertyChangedName); // "Optical_Service_Period"
        }
        /// <summary>
        /// Reads data from Optical Service Data and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Optical_Service_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Optical_Service_Data> ReadOptical_Service_Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Optical_Service_Optical_Service_Data_index;
            await Ensure_Characteristic_Async(ServiceIndex.Optical_Service_index, "Optical Service", index, "Optical Service Data");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Optical Service Data", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U16^DU_12_RS_15_AN_2_XY_0.01_*_SW_4095_AN_*|FIXED^N1|Lux");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrOptical_Service_Data.Lux = vr.GetNextDouble();
            CurrOptical_Service_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Optical_Service_DataPropertyChangedName); // "Optical_Service_Data"
            return CurrOptical_Service_Data;
        }
        /// <summary>
        /// Reads data from Optical Service Config and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Optical_Service_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Optical_Service_Data> ReadOptical_Service_Config(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Optical_Service_Optical_Service_Config_index;
            await Ensure_Characteristic_Async(ServiceIndex.Optical_Service_index, "Optical Service", index, "Optical Service Config");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Optical Service Config", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX|Enable");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrOptical_Service_Data.Enable = vr.GetNextDouble();
            CurrOptical_Service_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Optical_Service_ConfigPropertyChangedName); // "Optical_Service_Config"
            return CurrOptical_Service_Data;
        }
        /// <summary>
        /// Reads data from Optical Service Period and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Optical_Service_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Optical_Service_Data> ReadOptical_Service_Period(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Optical_Service_Optical_Service_Period_index;
            await Ensure_Characteristic_Async(ServiceIndex.Optical_Service_index, "Optical Service", index, "Optical Service Period");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Optical Service Period", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|DEC|Period|10ms");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrOptical_Service_Data.Period = vr.GetNextDouble();
            CurrOptical_Service_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Optical_Service_PeriodPropertyChangedName); // "Optical_Service_Period"
            return CurrOptical_Service_Data;
        }
        /// <summary>
        /// Writes data to Optical Service Config 
        /// </summary>
        public async Task WriteOptical_Service_Config(byte[] data)
        {
            var index = CharacteristicIndex.Optical_Service_Optical_Service_Config_index;
            await Ensure_Characteristic_Async(ServiceIndex.Optical_Service_index, "Optical Service", index, "Optical Service Config");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteOptical_Service_Config", result);
        }
        /// <summary>
        /// Writes data to Optical Service Period 
        /// </summary>
        public async Task WriteOptical_Service_Period(byte[] data)
        {
            var index = CharacteristicIndex.Optical_Service_Optical_Service_Period_index;
            await Ensure_Characteristic_Async(ServiceIndex.Optical_Service_index, "Optical Service", index, "Optical Service Period");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteOptical_Service_Period", result);
        }

        #endregion
//
        #region Service_IO_Service
        // Service IO Service 

        public IO_Service_Data CurrIO_Service_Data { get; set; } = new IO_Service_Data();

        // Per-characteristics methods for IO_Service IO_Service_Data
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyIO_Service_DataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("IO_Service_Data", ServiceIndex.IO_Service_index, "IO Service", CharacteristicIndex.IO_Service_IO_Service_Data_index, NotifyIO_Service_DataCallback, notifyType);
            return retval;
        }

        private void NotifyIO_Service_DataCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.IO_Service_IO_Service_Data_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|IOData");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrIO_Service_Data.TimestampMostRecent = args.Timestamp;
            CurrIO_Service_Data.IOData = vr.GetNextByteArray();
            OnPropertyChanged(IO_Service_DataPropertyChangedName); // "IO_Service_Data"
        }
        // Per-characteristics methods for IO_Service IO_Service_Config
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyIO_Service_ConfigAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("IO_Service_Config", ServiceIndex.IO_Service_index, "IO Service", CharacteristicIndex.IO_Service_IO_Service_Config_index, NotifyIO_Service_ConfigCallback, notifyType);
            return retval;
        }

        private void NotifyIO_Service_ConfigCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.IO_Service_IO_Service_Config_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX|IOConfig");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrIO_Service_Data.TimestampMostRecent = args.Timestamp;
            CurrIO_Service_Data.IOConfig = vr.GetNextDouble();
            OnPropertyChanged(IO_Service_ConfigPropertyChangedName); // "IO_Service_Config"
        }
        /// <summary>
        /// Reads data from IO Service Data and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>IO_Service_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<IO_Service_Data> ReadIO_Service_Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.IO_Service_IO_Service_Data_index;
            await Ensure_Characteristic_Async(ServiceIndex.IO_Service_index, "IO Service", index, "IO Service Data");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "IO Service Data", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|IOData");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrIO_Service_Data.IOData = vr.GetNextByteArray();
            CurrIO_Service_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(IO_Service_DataPropertyChangedName); // "IO_Service_Data"
            return CurrIO_Service_Data;
        }
        /// <summary>
        /// Reads data from IO Service Config and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>IO_Service_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<IO_Service_Data> ReadIO_Service_Config(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.IO_Service_IO_Service_Config_index;
            await Ensure_Characteristic_Async(ServiceIndex.IO_Service_index, "IO Service", index, "IO Service Config");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "IO Service Config", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX|IOConfig");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrIO_Service_Data.IOConfig = vr.GetNextDouble();
            CurrIO_Service_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(IO_Service_ConfigPropertyChangedName); // "IO_Service_Config"
            return CurrIO_Service_Data;
        }
        /// <summary>
        /// Writes data to IO Service Data 
        /// </summary>
        public async Task WriteIO_Service_Data(byte[] data)
        {
            var index = CharacteristicIndex.IO_Service_IO_Service_Data_index;
            await Ensure_Characteristic_Async(ServiceIndex.IO_Service_index, "IO Service", index, "IO Service Data");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteIO_Service_Data", result);
        }
        /// <summary>
        /// Writes data to IO Service Config 
        /// </summary>
        public async Task WriteIO_Service_Config(byte[] data)
        {
            var index = CharacteristicIndex.IO_Service_IO_Service_Config_index;
            await Ensure_Characteristic_Async(ServiceIndex.IO_Service_index, "IO Service", index, "IO Service Config");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteIO_Service_Config", result);
        }

        #endregion
//
        #region Service_Register_service
        // Service Register service 

        public Register_service_Data CurrRegister_service_Data { get; set; } = new Register_service_Data();

        // Per-characteristics methods for Register_service Register_Data
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyRegister_DataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Register_Data", ServiceIndex.Register_service_index, "Register service", CharacteristicIndex.Register_service_Register_Data_index, NotifyRegister_DataCallback, notifyType);
            return retval;
        }

        private void NotifyRegister_DataCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Register_service_Register_Data_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|RegisterData");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrRegister_service_Data.TimestampMostRecent = args.Timestamp;
            CurrRegister_service_Data.RegisterData = vr.GetNextByteArray();
            OnPropertyChanged(Register_DataPropertyChangedName); // "Register_Data"
        }
        // Per-characteristics methods for Register_service Register_Address
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyRegister_AddressAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Register_Address", ServiceIndex.Register_service_index, "Register service", CharacteristicIndex.Register_service_Register_Address_index, NotifyRegister_AddressCallback, notifyType);
            return retval;
        }

        private void NotifyRegister_AddressCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Register_service_Register_Address_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|RegisterAddress");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrRegister_service_Data.TimestampMostRecent = args.Timestamp;
            CurrRegister_service_Data.RegisterAddress = vr.GetNextByteArray();
            OnPropertyChanged(Register_AddressPropertyChangedName); // "Register_Address"
        }
        // Per-characteristics methods for Register_service Register_Device_ID
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyRegister_Device_IDAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Register_Device_ID", ServiceIndex.Register_service_index, "Register service", CharacteristicIndex.Register_service_Register_Device_ID_index, NotifyRegister_Device_IDCallback, notifyType);
            return retval;
        }

        private void NotifyRegister_Device_IDCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Register_service_Register_Device_ID_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|RegisterDeviceID");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrRegister_service_Data.TimestampMostRecent = args.Timestamp;
            CurrRegister_service_Data.RegisterDeviceID = vr.GetNextByteArray();
            OnPropertyChanged(Register_Device_IDPropertyChangedName); // "Register_Device_ID"
        }
        /// <summary>
        /// Reads data from Register Data and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Register_service_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Register_service_Data> ReadRegister_Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Register_service_Register_Data_index;
            await Ensure_Characteristic_Async(ServiceIndex.Register_service_index, "Register service", index, "Register Data");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Register Data", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|RegisterData");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrRegister_service_Data.RegisterData = vr.GetNextByteArray();
            CurrRegister_service_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Register_DataPropertyChangedName); // "Register_Data"
            return CurrRegister_service_Data;
        }
        /// <summary>
        /// Reads data from Register Address and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Register_service_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Register_service_Data> ReadRegister_Address(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Register_service_Register_Address_index;
            await Ensure_Characteristic_Async(ServiceIndex.Register_service_index, "Register service", index, "Register Address");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Register Address", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|RegisterAddress");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrRegister_service_Data.RegisterAddress = vr.GetNextByteArray();
            CurrRegister_service_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Register_AddressPropertyChangedName); // "Register_Address"
            return CurrRegister_service_Data;
        }
        /// <summary>
        /// Reads data from Register Device ID and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Register_service_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Register_service_Data> ReadRegister_Device_ID(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Register_service_Register_Device_ID_index;
            await Ensure_Characteristic_Async(ServiceIndex.Register_service_index, "Register service", index, "Register Device ID");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Register Device ID", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|RegisterDeviceID");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrRegister_service_Data.RegisterDeviceID = vr.GetNextByteArray();
            CurrRegister_service_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Register_Device_IDPropertyChangedName); // "Register_Device_ID"
            return CurrRegister_service_Data;
        }
        /// <summary>
        /// Writes data to Register Data 
        /// </summary>
        public async Task WriteRegister_Data(byte[] data)
        {
            var index = CharacteristicIndex.Register_service_Register_Data_index;
            await Ensure_Characteristic_Async(ServiceIndex.Register_service_index, "Register service", index, "Register Data");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteRegister_Data", result);
        }
        /// <summary>
        /// Writes data to Register Address 
        /// </summary>
        public async Task WriteRegister_Address(byte[] data)
        {
            var index = CharacteristicIndex.Register_service_Register_Address_index;
            await Ensure_Characteristic_Async(ServiceIndex.Register_service_index, "Register service", index, "Register Address");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteRegister_Address", result);
        }
        /// <summary>
        /// Writes data to Register Device ID 
        /// </summary>
        public async Task WriteRegister_Device_ID(byte[] data)
        {
            var index = CharacteristicIndex.Register_service_Register_Device_ID_index;
            await Ensure_Characteristic_Async(ServiceIndex.Register_service_index, "Register service", index, "Register Device ID");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteRegister_Device_ID", result);
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
        /// Reads data from Connection Parameter and triggers an OnPropertyChanged
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
            CurrCommon_Configuration_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Connection_ParameterPropertyChangedName); // "Connection_Parameter"
            return CurrCommon_Configuration_Data;
        }

        #endregion
//
        #region Service_Device_Info
        // Service Device Info 

        public Device_Info_Data CurrDevice_Info_Data { get; set; } = new Device_Info_Data();

        // Per-characteristics methods for Device_Info System_ID
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifySystem_IDAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("System_ID", ServiceIndex.Device_Info_index, "Device Info", CharacteristicIndex.Device_Info_System_ID_index, NotifySystem_IDCallback, notifyType);
            return retval;
        }

        private void NotifySystem_IDCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Device_Info_System_ID_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|ASCII|SystemID");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDevice_Info_Data.TimestampMostRecent = args.Timestamp;
            CurrDevice_Info_Data.SystemID = vr.GetNextString();
            OnPropertyChanged(System_IDPropertyChangedName); // "System_ID"
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
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|ASCII|ModelNumber");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDevice_Info_Data.TimestampMostRecent = args.Timestamp;
            CurrDevice_Info_Data.ModelNumber = vr.GetNextString();
            OnPropertyChanged(Model_NumberPropertyChangedName); // "Model_Number"
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
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|ASCII|SerialNumber");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDevice_Info_Data.TimestampMostRecent = args.Timestamp;
            CurrDevice_Info_Data.SerialNumber = vr.GetNextString();
            OnPropertyChanged(Serial_NumberPropertyChangedName); // "Serial_Number"
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
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|ASCII|FirmwareRevision");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDevice_Info_Data.TimestampMostRecent = args.Timestamp;
            CurrDevice_Info_Data.FirmwareRevision = vr.GetNextString();
            OnPropertyChanged(Firmware_RevisionPropertyChangedName); // "Firmware_Revision"
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
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|ASCII|HardwareRevision");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDevice_Info_Data.TimestampMostRecent = args.Timestamp;
            CurrDevice_Info_Data.HardwareRevision = vr.GetNextString();
            OnPropertyChanged(Hardware_RevisionPropertyChangedName); // "Hardware_Revision"
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
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|ASCII|SoftwareRevision");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDevice_Info_Data.TimestampMostRecent = args.Timestamp;
            CurrDevice_Info_Data.SoftwareRevision = vr.GetNextString();
            OnPropertyChanged(Software_RevisionPropertyChangedName); // "Software_Revision"
        }
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
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|ASCII|ManufacturerName");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDevice_Info_Data.TimestampMostRecent = args.Timestamp;
            CurrDevice_Info_Data.ManufacturerName = vr.GetNextString();
            OnPropertyChanged(Manufacturer_NamePropertyChangedName); // "Manufacturer_Name"
        }
        // Per-characteristics methods for Device_Info Regulatory_List
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyRegulatory_ListAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Regulatory_List", ServiceIndex.Device_Info_index, "Device Info", CharacteristicIndex.Device_Info_Regulatory_List_index, NotifyRegulatory_ListCallback, notifyType);
            return retval;
        }

        private void NotifyRegulatory_ListCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Device_Info_Regulatory_List_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX|BodyType U8|HEX|BodyStructure STRING|ASCII|Data");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDevice_Info_Data.TimestampMostRecent = args.Timestamp;
            CurrDevice_Info_Data.BodyType = vr.GetNextDouble();
            CurrDevice_Info_Data.BodyStructure = vr.GetNextDouble();
            CurrDevice_Info_Data.Data = vr.GetNextString();
            OnPropertyChanged(Regulatory_ListPropertyChangedName); // "Regulatory_List"
        }
        // Per-characteristics methods for Device_Info PnP_ID
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyPnP_IDAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("PnP_ID", ServiceIndex.Device_Info_index, "Device Info", CharacteristicIndex.Device_Info_PnP_ID_index, NotifyPnP_IDCallback, notifyType);
            return retval;
        }

        private void NotifyPnP_IDCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Device_Info_PnP_ID_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|ASCII|PnPID");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDevice_Info_Data.TimestampMostRecent = args.Timestamp;
            CurrDevice_Info_Data.PnPID = vr.GetNextString();
            OnPropertyChanged(PnP_IDPropertyChangedName); // "PnP_ID"
        }
        /// <summary>
        /// Reads data from System ID and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Device_Info_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Device_Info_Data> ReadSystem_ID(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Device_Info_System_ID_index;
            await Ensure_Characteristic_Async(ServiceIndex.Device_Info_index, "Device Info", index, "System ID");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "System ID", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("STRING|ASCII|SystemID");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDevice_Info_Data.SystemID = vr.GetNextString();
            CurrDevice_Info_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(System_IDPropertyChangedName); // "System_ID"
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

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("STRING|ASCII|ModelNumber");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDevice_Info_Data.ModelNumber = vr.GetNextString();
            CurrDevice_Info_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Model_NumberPropertyChangedName); // "Model_Number"
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

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("STRING|ASCII|SerialNumber");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDevice_Info_Data.SerialNumber = vr.GetNextString();
            CurrDevice_Info_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Serial_NumberPropertyChangedName); // "Serial_Number"
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

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("STRING|ASCII|FirmwareRevision");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDevice_Info_Data.FirmwareRevision = vr.GetNextString();
            CurrDevice_Info_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Firmware_RevisionPropertyChangedName); // "Firmware_Revision"
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

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("STRING|ASCII|HardwareRevision");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDevice_Info_Data.HardwareRevision = vr.GetNextString();
            CurrDevice_Info_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Hardware_RevisionPropertyChangedName); // "Hardware_Revision"
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

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("STRING|ASCII|SoftwareRevision");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDevice_Info_Data.SoftwareRevision = vr.GetNextString();
            CurrDevice_Info_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Software_RevisionPropertyChangedName); // "Software_Revision"
            return CurrDevice_Info_Data;
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

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("STRING|ASCII|ManufacturerName");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDevice_Info_Data.ManufacturerName = vr.GetNextString();
            CurrDevice_Info_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Manufacturer_NamePropertyChangedName); // "Manufacturer_Name"
            return CurrDevice_Info_Data;
        }
        /// <summary>
        /// Reads data from Regulatory List and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Device_Info_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Device_Info_Data> ReadRegulatory_List(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Device_Info_Regulatory_List_index;
            await Ensure_Characteristic_Async(ServiceIndex.Device_Info_index, "Device Info", index, "Regulatory List");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Regulatory List", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX|BodyType U8|HEX|BodyStructure STRING|ASCII|Data");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDevice_Info_Data.BodyType = vr.GetNextDouble();
            CurrDevice_Info_Data.BodyStructure = vr.GetNextDouble();
            CurrDevice_Info_Data.Data = vr.GetNextString();
            CurrDevice_Info_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Regulatory_ListPropertyChangedName); // "Regulatory_List"
            return CurrDevice_Info_Data;
        }
        /// <summary>
        /// Reads data from PnP ID and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Device_Info_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Device_Info_Data> ReadPnP_ID(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Device_Info_PnP_ID_index;
            await Ensure_Characteristic_Async(ServiceIndex.Device_Info_index, "Device Info", index, "PnP ID");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "PnP ID", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("STRING|ASCII|PnPID");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDevice_Info_Data.PnPID = vr.GetNextString();
            CurrDevice_Info_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(PnP_IDPropertyChangedName); // "PnP_ID"
            return CurrDevice_Info_Data;
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
        /// <summary>
        /// Reads data from BatteryLevel and triggers an OnPropertyChanged
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
            CurrBattery_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(BatteryLevelPropertyChangedName); // "BatteryLevel"
            return CurrBattery_Data;
        }

        #endregion
//


// Long obsolete! [[zzMETHOD+LIST]]
    }
}