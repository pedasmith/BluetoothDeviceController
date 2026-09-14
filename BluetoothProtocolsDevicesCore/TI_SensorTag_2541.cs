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
    /// The original smaller (upright) TI SensorTag. As of 2017, it's obsolete. The CC2541 SensorTag is the first Bluetooth Smart development kit focused on wireless sensor applications and it is the only development kit targeted for smart phone app developers. The SensorTag can be used as reference design and development platform for a variety of smart phone accessories..
    /// This class was automatically generated 2026-09-13::17:20
    /// </summary>

    public partial class TI_SensorTag_2541 : INotifyPropertyChanged
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

        public string Name { get; } = "SensorTag";
        public string Description { get; } = "The original smaller (upright) TI SensorTag. As of 2017, it's obsolete. The CC2541 SensorTag is the first Bluetooth Smart development kit focused on wireless sensor applications and it is the only development kit targeted for smart phone app developers. The SensorTag can be used as reference design and development platform for a variety of smart phone accessories.";

        /* Service and Characteristics summary for the device SensorTag

        Accelerometer service Guid=f000aa10-0451-4000-b000-000000000000
            Accelerometer_Data (DataGroup record)
                Accelerometer Data characteristic has AccelX (SByte-->double) AccelY (SByte-->double) AccelZ (SByte-->double)  Guid=f000aa11-0451-4000-b000-000000000000
                Accelerometer Configure characteristic has AccelerometerConfigure (Byte-->double)  Guid=f000aa12-0451-4000-b000-000000000000
                Accelerometer Period characteristic has AccelerometerPeriod (Byte-->double)  Guid=f000aa13-0451-4000-b000-000000000000


        Key Press service Guid=ffe0
            Key Press_Data (DataGroup record)
                Key Press State characteristic has KeyPressState (Byte-->double)  Guid=ffe1


        IR Service service Guid=f000aa00-0451-4000-b000-000000000000
            IR Service_Data (DataGroup record)
                IR Data characteristic has ObjTemp (Int16-->double) AmbientTemp (Int16-->double)  Guid=f000aa01-0451-4000-b000-000000000000
                IR Service Configure characteristic has IRConfigure (Byte-->double)  Guid=f000aa02-0451-4000-b000-000000000000
                IR Service Period characteristic has IRPeriod (Byte-->double)  Guid=f000aa03-0451-4000-b000-000000000000


        Humidity service Guid=f000aa20-0451-4000-b000-000000000000
            Humidity_Data (DataGroup record)
                Humidity Data characteristic has Temp (UInt16-->double) Humidity (UInt16-->double)  Guid=f000aa21-0451-4000-b000-000000000000
                Humidity Configure characteristic has HumidityConfigure (Byte-->double)  Guid=f000aa22-0451-4000-b000-000000000000
                Humidity Period characteristic has HumidityPeriod (Byte-->double)  Guid=f000aa23-0451-4000-b000-000000000000


        Magnetometer service Guid=f000aa30-0451-4000-b000-000000000000
            Magnetometer_Data (DataGroup record)
                Magnetometer Data characteristic has X (Int16-->double) Y (Int16-->double) Z (Int16-->double)  Guid=f000aa31-0451-4000-b000-000000000000
                Magnetometer Configure characteristic has MagnetometerConfigure (Byte-->double)  Guid=f000aa32-0451-4000-b000-000000000000
                Magnetometer Period characteristic has MagnetometerPeriod (Byte-->double)  Guid=f000aa33-0451-4000-b000-000000000000


        Barometer service Guid=f000aa40-0451-4000-b000-000000000000
            Barometer_Data (DataGroup record)
                Barometer Data characteristic has TempRaw (UInt16-->double) PressureRaw (UInt16-->double)  Guid=f000aa41-0451-4000-b000-000000000000
                Barometer Configure characteristic has BarometerConfigure (Byte-->double)  Guid=f000aa42-0451-4000-b000-000000000000
                Barometer Calibration characteristic has BarometerCalibration (Bytes-->string)  Guid=f000aa43-0451-4000-b000-000000000000
                Barometer Period characteristic has BarometerPeriod (Byte-->double)  Guid=f000aa44-0451-4000-b000-000000000000


        Gyroscope service Guid=f000aa50-0451-4000-b000-000000000000
            Gyroscope_Data (DataGroup record)
                Gyroscope Data characteristic has X (Int16-->double) Y (Int16-->double) Z (Int16-->double)  Guid=f000aa51-0451-4000-b000-000000000000
                Gyroscope Configure characteristic has GyroscopeConfigure (Byte-->double)  Guid=f000aa52-0451-4000-b000-000000000000
                Gyroscope Period characteristic has GyroscopePeriod (Byte-->double)  Guid=f000aa53-0451-4000-b000-000000000000


        Common Configuration service Guid=1800
            Common Configuration_Data (DataGroup record)
                Device Name characteristic has Device_Name (String-->string)  Guid=2a00
                Appearance characteristic has Appearance (UInt16-->double)  Guid=2a01
                Privacy characteristic has Privacy (Bytes-->string)  Guid=2a02
                Reconnect Address characteristic has ReconnectAddress (Bytes-->string)  Guid=2a03
                Connection Parameter characteristic has ConnectionParameter (Bytes-->string)  Guid=2a04


        Device Info service Guid=180a
            Device Info_Data (DataGroup record)
                System ID characteristic has SystemId (String-->string)  Guid=2a23
                Model Number characteristic has ModelNumber (String-->string)  Guid=2a24
                Serial Number characteristic has SerialNumber (String-->string)  Guid=2a25
                Firmware Revision characteristic has FormwareRevision (String-->string)  Guid=2a26
                Hardware Revision characteristic has HardwareRevision (String-->string)  Guid=2a27
                Software Revision characteristic has SoftwareRevision (String-->string)  Guid=2a28
                Manufacturer Name characteristic has ManufacturerName (String-->string)  Guid=2a29
                Regulatory List characteristic has BodyType (Byte-->double) BodyStructure (Byte-->double) Data (String-->string)  Guid=2a2a
                PnP ID characteristic has PnpID (String-->string)  Guid=2a50
        */

        public const string Accelerometer_DataPropertyChangedName = "Accelerometer_Data";
        public const string Accelerometer_ConfigurePropertyChangedName = "Accelerometer_Configure";
        public const string Accelerometer_PeriodPropertyChangedName = "Accelerometer_Period";
        public const string Key_Press_StatePropertyChangedName = "Key_Press_State";
        public const string IR_DataPropertyChangedName = "IR_Data";
        public const string IR_Service_ConfigurePropertyChangedName = "IR_Service_Configure";
        public const string IR_Service_PeriodPropertyChangedName = "IR_Service_Period";
        public const string Humidity_DataPropertyChangedName = "Humidity_Data";
        public const string Humidity_ConfigurePropertyChangedName = "Humidity_Configure";
        public const string Humidity_PeriodPropertyChangedName = "Humidity_Period";
        public const string Magnetometer_DataPropertyChangedName = "Magnetometer_Data";
        public const string Magnetometer_ConfigurePropertyChangedName = "Magnetometer_Configure";
        public const string Magnetometer_PeriodPropertyChangedName = "Magnetometer_Period";
        public const string Barometer_DataPropertyChangedName = "Barometer_Data";
        public const string Barometer_ConfigurePropertyChangedName = "Barometer_Configure";
        public const string Barometer_CalibrationPropertyChangedName = "Barometer_Calibration";
        public const string Barometer_PeriodPropertyChangedName = "Barometer_Period";
        public const string Gyroscope_DataPropertyChangedName = "Gyroscope_Data";
        public const string Gyroscope_ConfigurePropertyChangedName = "Gyroscope_Configure";
        public const string Gyroscope_PeriodPropertyChangedName = "Gyroscope_Period";
        public const string Device_NamePropertyChangedName = "Device_Name";
        public const string AppearancePropertyChangedName = "Appearance";
        public const string PrivacyPropertyChangedName = "Privacy";
        public const string Reconnect_AddressPropertyChangedName = "Reconnect_Address";
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
            private double _AccelX = 0;
            /// <summary>
            /// AccelX (I8 g) from Service=Accelerometer and Characteristic=Accelerometer Data
            ///</summary>
            public double AccelX 
            { 
                get { return _AccelX; }
                set { if (value == _AccelX) return; _AccelX = value; OnPropertyChanged();}
            }
            private double _AccelY = 0;
            /// <summary>
            /// AccelY (I8 g) from Service=Accelerometer and Characteristic=Accelerometer Data
            ///</summary>
            public double AccelY 
            { 
                get { return _AccelY; }
                set { if (value == _AccelY) return; _AccelY = value; OnPropertyChanged();}
            }
            private double _AccelZ = 0;
            /// <summary>
            /// AccelZ (I8 g) from Service=Accelerometer and Characteristic=Accelerometer Data
            ///</summary>
            public double AccelZ 
            { 
                get { return _AccelZ; }
                set { if (value == _AccelZ) return; _AccelZ = value; OnPropertyChanged();}
            }

            private double _AccelerometerConfigure = 0;
            /// <summary>
            /// AccelerometerConfigure (U8 ) from Service=Accelerometer and Characteristic=Accelerometer Configure
            ///</summary>
            public double AccelerometerConfigure 
            { 
                get { return _AccelerometerConfigure; }
                set { if (value == _AccelerometerConfigure) return; _AccelerometerConfigure = value; OnPropertyChanged();}
            }

            private double _AccelerometerPeriod = 0;
            /// <summary>
            /// AccelerometerPeriod (U8 ) from Service=Accelerometer and Characteristic=Accelerometer Period
            ///</summary>
            public double AccelerometerPeriod 
            { 
                get { return _AccelerometerPeriod; }
                set { if (value == _AccelerometerPeriod) return; _AccelerometerPeriod = value; OnPropertyChanged();}
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
                dest.AccelX = source.AccelX;
                dest.AccelY = source.AccelY;
                dest.AccelZ = source.AccelZ;
                dest.AccelerometerConfigure = source.AccelerometerConfigure;
                dest.AccelerometerPeriod = source.AccelerometerPeriod;
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
                dest.AccelX = convert(source.AccelX, "g");
                dest.AccelY = convert(source.AccelY, "g");
                dest.AccelZ = convert(source.AccelZ, "g");
                dest.AccelerometerConfigure = convert(source.AccelerometerConfigure, "");
                dest.AccelerometerPeriod = convert(source.AccelerometerPeriod, "");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["AccelX", "AccelY", "AccelZ", "AccelerometerConfigure", "AccelerometerPeriod"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(AccelX);
                exporter.CellSet(AccelY);
                exporter.CellSet(AccelZ);
                exporter.CellSet(AccelerometerConfigure);
                exporter.CellSet(AccelerometerPeriod);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {AccelX} {AccelY} {AccelZ} {AccelerometerConfigure} {AccelerometerPeriod}");
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
            private double _KeyPressState = 0;
            /// <summary>
            /// KeyPressState (U8 ) from Service=Key Press and Characteristic=Key Press State
            ///</summary>
            public double KeyPressState 
            { 
                get { return _KeyPressState; }
                set { if (value == _KeyPressState) return; _KeyPressState = value; OnPropertyChanged();}
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
                dest.KeyPressState = source.KeyPressState;
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
                dest.KeyPressState = convert(source.KeyPressState, "");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["KeyPressState"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(KeyPressState);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {KeyPressState}");
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
            private double _AmbientTemp = 0;
            /// <summary>
            /// AmbientTemp (I16 C) from Service=IR Service and Characteristic=IR Data
            ///</summary>
            public double AmbientTemp 
            { 
                get { return _AmbientTemp; }
                set { if (value == _AmbientTemp) return; _AmbientTemp = value; OnPropertyChanged();}
            }

            private double _IRConfigure = 0;
            /// <summary>
            /// IRConfigure (U8 ) from Service=IR Service and Characteristic=IR Service Configure
            ///</summary>
            public double IRConfigure 
            { 
                get { return _IRConfigure; }
                set { if (value == _IRConfigure) return; _IRConfigure = value; OnPropertyChanged();}
            }

            private double _IRPeriod = 0;
            /// <summary>
            /// IRPeriod (U8 ) from Service=IR Service and Characteristic=IR Service Period
            ///</summary>
            public double IRPeriod 
            { 
                get { return _IRPeriod; }
                set { if (value == _IRPeriod) return; _IRPeriod = value; OnPropertyChanged();}
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
                dest.AmbientTemp = source.AmbientTemp;
                dest.IRConfigure = source.IRConfigure;
                dest.IRPeriod = source.IRPeriod;
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
                dest.AmbientTemp = convert(source.AmbientTemp, "C");
                dest.IRConfigure = convert(source.IRConfigure, "");
                dest.IRPeriod = convert(source.IRPeriod, "");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["ObjTemp", "AmbientTemp", "IRConfigure", "IRPeriod"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(ObjTemp);
                exporter.CellSet(AmbientTemp);
                exporter.CellSet(IRConfigure);
                exporter.CellSet(IRPeriod);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {ObjTemp} {AmbientTemp} {IRConfigure} {IRPeriod}");
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

            private double _HumidityConfigure = 0;
            /// <summary>
            /// HumidityConfigure (U8 ) from Service=Humidity and Characteristic=Humidity Configure
            ///</summary>
            public double HumidityConfigure 
            { 
                get { return _HumidityConfigure; }
                set { if (value == _HumidityConfigure) return; _HumidityConfigure = value; OnPropertyChanged();}
            }

            private double _HumidityPeriod = 0;
            /// <summary>
            /// HumidityPeriod (U8 ) from Service=Humidity and Characteristic=Humidity Period
            ///</summary>
            public double HumidityPeriod 
            { 
                get { return _HumidityPeriod; }
                set { if (value == _HumidityPeriod) return; _HumidityPeriod = value; OnPropertyChanged();}
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
                dest.HumidityConfigure = source.HumidityConfigure;
                dest.HumidityPeriod = source.HumidityPeriod;
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
                dest.HumidityConfigure = convert(source.HumidityConfigure, "");
                dest.HumidityPeriod = convert(source.HumidityPeriod, "");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["Temp", "Humidity", "HumidityConfigure", "HumidityPeriod"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(Temp);
                exporter.CellSet(Humidity);
                exporter.CellSet(HumidityConfigure);
                exporter.CellSet(HumidityPeriod);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {Temp} {Humidity} {HumidityConfigure} {HumidityPeriod}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the Magnetometer Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class Magnetometer_Data :BTCommonMetaData<Magnetometer_Data> //, IExportDataSource
        {
            private double _X = 0;
            /// <summary>
            /// X (I16 ) from Service=Magnetometer and Characteristic=Magnetometer Data
            ///</summary>
            public double X 
            { 
                get { return _X; }
                set { if (value == _X) return; _X = value; OnPropertyChanged();}
            }
            private double _Y = 0;
            /// <summary>
            /// Y (I16 ) from Service=Magnetometer and Characteristic=Magnetometer Data
            ///</summary>
            public double Y 
            { 
                get { return _Y; }
                set { if (value == _Y) return; _Y = value; OnPropertyChanged();}
            }
            private double _Z = 0;
            /// <summary>
            /// Z (I16 ) from Service=Magnetometer and Characteristic=Magnetometer Data
            ///</summary>
            public double Z 
            { 
                get { return _Z; }
                set { if (value == _Z) return; _Z = value; OnPropertyChanged();}
            }

            private double _MagnetometerConfigure = 0;
            /// <summary>
            /// MagnetometerConfigure (U8 ) from Service=Magnetometer and Characteristic=Magnetometer Configure
            ///</summary>
            public double MagnetometerConfigure 
            { 
                get { return _MagnetometerConfigure; }
                set { if (value == _MagnetometerConfigure) return; _MagnetometerConfigure = value; OnPropertyChanged();}
            }

            private double _MagnetometerPeriod = 0;
            /// <summary>
            /// MagnetometerPeriod (U8 ) from Service=Magnetometer and Characteristic=Magnetometer Period
            ///</summary>
            public double MagnetometerPeriod 
            { 
                get { return _MagnetometerPeriod; }
                set { if (value == _MagnetometerPeriod) return; _MagnetometerPeriod = value; OnPropertyChanged();}
            }
            public override Magnetometer_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as Magnetometer_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(Magnetometer_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.X = source.X;
                dest.Y = source.Y;
                dest.Z = source.Z;
                dest.MagnetometerConfigure = source.MagnetometerConfigure;
                dest.MagnetometerPeriod = source.MagnetometerPeriod;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static Magnetometer_Data CopyToWithConvertAndCreate(Magnetometer_Data source, Magnetometer_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.X = convert(source.X, "");
                dest.Y = convert(source.Y, "");
                dest.Z = convert(source.Z, "");
                dest.MagnetometerConfigure = convert(source.MagnetometerConfigure, "");
                dest.MagnetometerPeriod = convert(source.MagnetometerPeriod, "");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["X", "Y", "Z", "MagnetometerConfigure", "MagnetometerPeriod"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(X);
                exporter.CellSet(Y);
                exporter.CellSet(Z);
                exporter.CellSet(MagnetometerConfigure);
                exporter.CellSet(MagnetometerPeriod);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {X} {Y} {Z} {MagnetometerConfigure} {MagnetometerPeriod}");
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
            private double _TempRaw = 0;
            /// <summary>
            /// TempRaw (U16 ) from Service=Barometer and Characteristic=Barometer Data
            ///</summary>
            public double TempRaw 
            { 
                get { return _TempRaw; }
                set { if (value == _TempRaw) return; _TempRaw = value; OnPropertyChanged();}
            }
            private double _PressureRaw = 0;
            /// <summary>
            /// PressureRaw (U16 ) from Service=Barometer and Characteristic=Barometer Data
            ///</summary>
            public double PressureRaw 
            { 
                get { return _PressureRaw; }
                set { if (value == _PressureRaw) return; _PressureRaw = value; OnPropertyChanged();}
            }

            private double _BarometerConfigure = 0;
            /// <summary>
            /// BarometerConfigure (U8 ) from Service=Barometer and Characteristic=Barometer Configure
            ///</summary>
            public double BarometerConfigure 
            { 
                get { return _BarometerConfigure; }
                set { if (value == _BarometerConfigure) return; _BarometerConfigure = value; OnPropertyChanged();}
            }

            private byte[] _BarometerCalibration = null;
            /// <summary>
            /// BarometerCalibration (BYTES ) from Service=Barometer and Characteristic=Barometer Calibration
            ///</summary>
            public byte[] BarometerCalibration 
            { 
                get { return _BarometerCalibration; }
                set { if (value == _BarometerCalibration) return; _BarometerCalibration = value; OnPropertyChanged();}
            }

            private double _BarometerPeriod = 0;
            /// <summary>
            /// BarometerPeriod (U8 ) from Service=Barometer and Characteristic=Barometer Period
            ///</summary>
            public double BarometerPeriod 
            { 
                get { return _BarometerPeriod; }
                set { if (value == _BarometerPeriod) return; _BarometerPeriod = value; OnPropertyChanged();}
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
                dest.TempRaw = source.TempRaw;
                dest.PressureRaw = source.PressureRaw;
                dest.BarometerConfigure = source.BarometerConfigure;
                dest.BarometerCalibration = source.BarometerCalibration;
                dest.BarometerPeriod = source.BarometerPeriod;
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
                dest.TempRaw = convert(source.TempRaw, "");
                dest.PressureRaw = convert(source.PressureRaw, "");
                dest.BarometerConfigure = convert(source.BarometerConfigure, "");
                dest.BarometerCalibration = source.BarometerCalibration;
                dest.BarometerPeriod = convert(source.BarometerPeriod, "");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["TempRaw", "PressureRaw", "BarometerConfigure", "BarometerCalibration", "BarometerPeriod"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(TempRaw);
                exporter.CellSet(PressureRaw);
                exporter.CellSet(BarometerConfigure);
                exporter.CellSet(BarometerCalibration);
                exporter.CellSet(BarometerPeriod);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {TempRaw} {PressureRaw} {BarometerConfigure} {BarometerCalibration} {BarometerPeriod}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the Gyroscope Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class Gyroscope_Data :BTCommonMetaData<Gyroscope_Data> //, IExportDataSource
        {
            private double _X = 0;
            /// <summary>
            /// X (I16 ) from Service=Gyroscope and Characteristic=Gyroscope Data
            ///</summary>
            public double X 
            { 
                get { return _X; }
                set { if (value == _X) return; _X = value; OnPropertyChanged();}
            }
            private double _Y = 0;
            /// <summary>
            /// Y (I16 ) from Service=Gyroscope and Characteristic=Gyroscope Data
            ///</summary>
            public double Y 
            { 
                get { return _Y; }
                set { if (value == _Y) return; _Y = value; OnPropertyChanged();}
            }
            private double _Z = 0;
            /// <summary>
            /// Z (I16 ) from Service=Gyroscope and Characteristic=Gyroscope Data
            ///</summary>
            public double Z 
            { 
                get { return _Z; }
                set { if (value == _Z) return; _Z = value; OnPropertyChanged();}
            }

            private double _GyroscopeConfigure = 0;
            /// <summary>
            /// GyroscopeConfigure (U8 ) from Service=Gyroscope and Characteristic=Gyroscope Configure
            ///</summary>
            public double GyroscopeConfigure 
            { 
                get { return _GyroscopeConfigure; }
                set { if (value == _GyroscopeConfigure) return; _GyroscopeConfigure = value; OnPropertyChanged();}
            }

            private double _GyroscopePeriod = 0;
            /// <summary>
            /// GyroscopePeriod (U8 ) from Service=Gyroscope and Characteristic=Gyroscope Period
            ///</summary>
            public double GyroscopePeriod 
            { 
                get { return _GyroscopePeriod; }
                set { if (value == _GyroscopePeriod) return; _GyroscopePeriod = value; OnPropertyChanged();}
            }
            public override Gyroscope_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as Gyroscope_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(Gyroscope_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.X = source.X;
                dest.Y = source.Y;
                dest.Z = source.Z;
                dest.GyroscopeConfigure = source.GyroscopeConfigure;
                dest.GyroscopePeriod = source.GyroscopePeriod;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static Gyroscope_Data CopyToWithConvertAndCreate(Gyroscope_Data source, Gyroscope_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.X = convert(source.X, "");
                dest.Y = convert(source.Y, "");
                dest.Z = convert(source.Z, "");
                dest.GyroscopeConfigure = convert(source.GyroscopeConfigure, "");
                dest.GyroscopePeriod = convert(source.GyroscopePeriod, "");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["X", "Y", "Z", "GyroscopeConfigure", "GyroscopePeriod"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(X);
                exporter.CellSet(Y);
                exporter.CellSet(Z);
                exporter.CellSet(GyroscopeConfigure);
                exporter.CellSet(GyroscopePeriod);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {X} {Y} {Z} {GyroscopeConfigure} {GyroscopePeriod}");
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

            private byte[] _Privacy = null;
            /// <summary>
            /// Privacy (BYTES ) from Service=Common Configuration and Characteristic=Privacy
            ///</summary>
            public byte[] Privacy 
            { 
                get { return _Privacy; }
                set { if (value == _Privacy) return; _Privacy = value; OnPropertyChanged();}
            }

            private byte[] _ReconnectAddress = null;
            /// <summary>
            /// ReconnectAddress (BYTES ) from Service=Common Configuration and Characteristic=Reconnect Address
            ///</summary>
            public byte[] ReconnectAddress 
            { 
                get { return _ReconnectAddress; }
                set { if (value == _ReconnectAddress) return; _ReconnectAddress = value; OnPropertyChanged();}
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
                dest.Privacy = source.Privacy;
                dest.ReconnectAddress = source.ReconnectAddress;
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
                dest.Privacy = source.Privacy;
                dest.ReconnectAddress = source.ReconnectAddress;
                dest.ConnectionParameter = source.ConnectionParameter;
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["Device_Name", "Appearance", "Privacy", "ReconnectAddress", "ConnectionParameter"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(Device_Name);
                exporter.CellSet(Appearance);
                exporter.CellSet(Privacy);
                exporter.CellSet(ReconnectAddress);
                exporter.CellSet(ConnectionParameter);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {Device_Name} {Appearance} {Privacy} {ReconnectAddress} {ConnectionParameter}");
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
            private string _SystemId = "";
            /// <summary>
            /// SystemId (STRING ) from Service=Device Info and Characteristic=System ID
            ///</summary>
            public string SystemId 
            { 
                get { return _SystemId; }
                set { if (value == _SystemId) return; _SystemId = value; OnPropertyChanged();}
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

            private string _FormwareRevision = "";
            /// <summary>
            /// FormwareRevision (STRING ) from Service=Device Info and Characteristic=Firmware Revision
            ///</summary>
            public string FormwareRevision 
            { 
                get { return _FormwareRevision; }
                set { if (value == _FormwareRevision) return; _FormwareRevision = value; OnPropertyChanged();}
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

            private string _PnpID = "";
            /// <summary>
            /// PnpID (STRING ) from Service=Device Info and Characteristic=PnP ID
            ///</summary>
            public string PnpID 
            { 
                get { return _PnpID; }
                set { if (value == _PnpID) return; _PnpID = value; OnPropertyChanged();}
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
                dest.SystemId = source.SystemId;
                dest.ModelNumber = source.ModelNumber;
                dest.SerialNumber = source.SerialNumber;
                dest.FormwareRevision = source.FormwareRevision;
                dest.HardwareRevision = source.HardwareRevision;
                dest.SoftwareRevision = source.SoftwareRevision;
                dest.ManufacturerName = source.ManufacturerName;
                dest.BodyType = source.BodyType;
                dest.BodyStructure = source.BodyStructure;
                dest.Data = source.Data;
                dest.PnpID = source.PnpID;
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
                dest.SystemId = source.SystemId;
                dest.ModelNumber = source.ModelNumber;
                dest.SerialNumber = source.SerialNumber;
                dest.FormwareRevision = source.FormwareRevision;
                dest.HardwareRevision = source.HardwareRevision;
                dest.SoftwareRevision = source.SoftwareRevision;
                dest.ManufacturerName = source.ManufacturerName;
                dest.BodyType = convert(source.BodyType, "");
                dest.BodyStructure = convert(source.BodyStructure, "");
                dest.Data = source.Data;
                dest.PnpID = source.PnpID;
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["SystemId", "ModelNumber", "SerialNumber", "FormwareRevision", "HardwareRevision", "SoftwareRevision", "ManufacturerName", "BodyType", "BodyStructure", "Data", "PnpID"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(SystemId);
                exporter.CellSet(ModelNumber);
                exporter.CellSet(SerialNumber);
                exporter.CellSet(FormwareRevision);
                exporter.CellSet(HardwareRevision);
                exporter.CellSet(SoftwareRevision);
                exporter.CellSet(ManufacturerName);
                exporter.CellSet(BodyType);
                exporter.CellSet(BodyStructure);
                exporter.CellSet(Data);
                exporter.CellSet(PnpID);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {SystemId} {ModelNumber} {SerialNumber} {FormwareRevision} {HardwareRevision} {SoftwareRevision} {ManufacturerName} {BodyType} {BodyStructure} {Data} {PnpID}");
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
            Magnetometer_index = 4,
            Barometer_index = 5,
            Gyroscope_index = 6,
            Common_Configuration_index = 7,
            Device_Info_index = 8,
        }

        /// <summary>
        /// Enumeration of all characteristics in all of the services.
        /// </summary>
        enum CharacteristicIndex
        {
            Accelerometer_Accelerometer_Data_index = 0,     // GUID f000aa11-0451-4000-b000-000000000000
            Accelerometer_Accelerometer_Configure_index = 1,     // GUID f000aa12-0451-4000-b000-000000000000
            Accelerometer_Accelerometer_Period_index = 2,     // GUID f000aa13-0451-4000-b000-000000000000
            Key_Press_Key_Press_State_index = 3,     // GUID 0000ffe1-0000-1000-8000-00805f9b34fb
            IR_Service_IR_Data_index = 4,     // GUID f000aa01-0451-4000-b000-000000000000
            IR_Service_IR_Service_Configure_index = 5,     // GUID f000aa02-0451-4000-b000-000000000000
            IR_Service_IR_Service_Period_index = 6,     // GUID f000aa03-0451-4000-b000-000000000000
            Humidity_Humidity_Data_index = 7,     // GUID f000aa21-0451-4000-b000-000000000000
            Humidity_Humidity_Configure_index = 8,     // GUID f000aa22-0451-4000-b000-000000000000
            Humidity_Humidity_Period_index = 9,     // GUID f000aa23-0451-4000-b000-000000000000
            Magnetometer_Magnetometer_Data_index = 10,     // GUID f000aa31-0451-4000-b000-000000000000
            Magnetometer_Magnetometer_Configure_index = 11,     // GUID f000aa32-0451-4000-b000-000000000000
            Magnetometer_Magnetometer_Period_index = 12,     // GUID f000aa33-0451-4000-b000-000000000000
            Barometer_Barometer_Data_index = 13,     // GUID f000aa41-0451-4000-b000-000000000000
            Barometer_Barometer_Configure_index = 14,     // GUID f000aa42-0451-4000-b000-000000000000
            Barometer_Barometer_Calibration_index = 15,     // GUID f000aa43-0451-4000-b000-000000000000
            Barometer_Barometer_Period_index = 16,     // GUID f000aa44-0451-4000-b000-000000000000
            Gyroscope_Gyroscope_Data_index = 17,     // GUID f000aa51-0451-4000-b000-000000000000
            Gyroscope_Gyroscope_Configure_index = 18,     // GUID f000aa52-0451-4000-b000-000000000000
            Gyroscope_Gyroscope_Period_index = 19,     // GUID f000aa53-0451-4000-b000-000000000000
            Common_Configuration_Device_Name_index = 20,     // GUID 00002a00-0000-1000-8000-00805f9b34fb
            Common_Configuration_Appearance_index = 21,     // GUID 00002a01-0000-1000-8000-00805f9b34fb
            Common_Configuration_Privacy_index = 22,     // GUID 00002a02-0000-1000-8000-00805f9b34fb
            Common_Configuration_Reconnect_Address_index = 23,     // GUID 00002a03-0000-1000-8000-00805f9b34fb
            Common_Configuration_Connection_Parameter_index = 24,     // GUID 00002a04-0000-1000-8000-00805f9b34fb
            Device_Info_System_ID_index = 25,     // GUID 00002a23-0000-1000-8000-00805f9b34fb
            Device_Info_Model_Number_index = 26,     // GUID 00002a24-0000-1000-8000-00805f9b34fb
            Device_Info_Serial_Number_index = 27,     // GUID 00002a25-0000-1000-8000-00805f9b34fb
            Device_Info_Firmware_Revision_index = 28,     // GUID 00002a26-0000-1000-8000-00805f9b34fb
            Device_Info_Hardware_Revision_index = 29,     // GUID 00002a27-0000-1000-8000-00805f9b34fb
            Device_Info_Software_Revision_index = 30,     // GUID 00002a28-0000-1000-8000-00805f9b34fb
            Device_Info_Manufacturer_Name_index = 31,     // GUID 00002a29-0000-1000-8000-00805f9b34fb
            Device_Info_Regulatory_List_index = 32,     // GUID 00002a2a-0000-1000-8000-00805f9b34fb
            Device_Info_PnP_ID_index = 33,     // GUID 00002a50-0000-1000-8000-00805f9b34fb
        }

        /// <summary>
        /// List of the guids supported by the device. 
        /// </summary>
        List<Guid> Service_Guids = new List<Guid>()
        {
            Guid.Parse("f000aa10-0451-4000-b000-000000000000"), // #0 is Accelerometer
            Guid.Parse("0000ffe0-0000-1000-8000-00805f9b34fb"), // #1 is Key Press
            Guid.Parse("f000aa00-0451-4000-b000-000000000000"), // #2 is IR Service
            Guid.Parse("f000aa20-0451-4000-b000-000000000000"), // #3 is Humidity
            Guid.Parse("f000aa30-0451-4000-b000-000000000000"), // #4 is Magnetometer
            Guid.Parse("f000aa40-0451-4000-b000-000000000000"), // #5 is Barometer
            Guid.Parse("f000aa50-0451-4000-b000-000000000000"), // #6 is Gyroscope
            Guid.Parse("00001800-0000-1000-8000-00805f9b34fb"), // #7 is Common Configuration
            Guid.Parse("0000180a-0000-1000-8000-00805f9b34fb"), // #8 is Device Info
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
            Guid.Parse("f000aa11-0451-4000-b000-000000000000"), // #0 is Accelerometer Accelerometer Data
            Guid.Parse("f000aa12-0451-4000-b000-000000000000"), // #1 is Accelerometer Accelerometer Configure
            Guid.Parse("f000aa13-0451-4000-b000-000000000000"), // #2 is Accelerometer Accelerometer Period
            Guid.Parse("0000ffe1-0000-1000-8000-00805f9b34fb"), // #3 is Key Press Key Press State
            Guid.Parse("f000aa01-0451-4000-b000-000000000000"), // #4 is IR Service IR Data
            Guid.Parse("f000aa02-0451-4000-b000-000000000000"), // #5 is IR Service IR Service Configure
            Guid.Parse("f000aa03-0451-4000-b000-000000000000"), // #6 is IR Service IR Service Period
            Guid.Parse("f000aa21-0451-4000-b000-000000000000"), // #7 is Humidity Humidity Data
            Guid.Parse("f000aa22-0451-4000-b000-000000000000"), // #8 is Humidity Humidity Configure
            Guid.Parse("f000aa23-0451-4000-b000-000000000000"), // #9 is Humidity Humidity Period
            Guid.Parse("f000aa31-0451-4000-b000-000000000000"), // #10 is Magnetometer Magnetometer Data
            Guid.Parse("f000aa32-0451-4000-b000-000000000000"), // #11 is Magnetometer Magnetometer Configure
            Guid.Parse("f000aa33-0451-4000-b000-000000000000"), // #12 is Magnetometer Magnetometer Period
            Guid.Parse("f000aa41-0451-4000-b000-000000000000"), // #13 is Barometer Barometer Data
            Guid.Parse("f000aa42-0451-4000-b000-000000000000"), // #14 is Barometer Barometer Configure
            Guid.Parse("f000aa43-0451-4000-b000-000000000000"), // #15 is Barometer Barometer Calibration
            Guid.Parse("f000aa44-0451-4000-b000-000000000000"), // #16 is Barometer Barometer Period
            Guid.Parse("f000aa51-0451-4000-b000-000000000000"), // #17 is Gyroscope Gyroscope Data
            Guid.Parse("f000aa52-0451-4000-b000-000000000000"), // #18 is Gyroscope Gyroscope Configure
            Guid.Parse("f000aa53-0451-4000-b000-000000000000"), // #19 is Gyroscope Gyroscope Period
            Guid.Parse("00002a00-0000-1000-8000-00805f9b34fb"), // #20 is Common Configuration Device Name
            Guid.Parse("00002a01-0000-1000-8000-00805f9b34fb"), // #21 is Common Configuration Appearance
            Guid.Parse("00002a02-0000-1000-8000-00805f9b34fb"), // #22 is Common Configuration Privacy
            Guid.Parse("00002a03-0000-1000-8000-00805f9b34fb"), // #23 is Common Configuration Reconnect Address
            Guid.Parse("00002a04-0000-1000-8000-00805f9b34fb"), // #24 is Common Configuration Connection Parameter
            Guid.Parse("00002a23-0000-1000-8000-00805f9b34fb"), // #25 is Device Info System ID
            Guid.Parse("00002a24-0000-1000-8000-00805f9b34fb"), // #26 is Device Info Model Number
            Guid.Parse("00002a25-0000-1000-8000-00805f9b34fb"), // #27 is Device Info Serial Number
            Guid.Parse("00002a26-0000-1000-8000-00805f9b34fb"), // #28 is Device Info Firmware Revision
            Guid.Parse("00002a27-0000-1000-8000-00805f9b34fb"), // #29 is Device Info Hardware Revision
            Guid.Parse("00002a28-0000-1000-8000-00805f9b34fb"), // #30 is Device Info Software Revision
            Guid.Parse("00002a29-0000-1000-8000-00805f9b34fb"), // #31 is Device Info Manufacturer Name
            Guid.Parse("00002a2a-0000-1000-8000-00805f9b34fb"), // #32 is Device Info Regulatory List
            Guid.Parse("00002a50-0000-1000-8000-00805f9b34fb"), // #33 is Device Info PnP ID
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
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("I8^64_/|FIXED|AccelX|g I8^64_/|FIXED|AccelY|g I8^64_/_IV|FIXED|AccelZ|g");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrAccelerometer_Data.TimestampMostRecent = args.Timestamp;
            CurrAccelerometer_Data.AccelX = vr.GetNextDouble();
            CurrAccelerometer_Data.AccelY = vr.GetNextDouble();
            CurrAccelerometer_Data.AccelZ = vr.GetNextDouble();
            OnPropertyChanged(Accelerometer_DataPropertyChangedName); // "Accelerometer_Data"
        }
        // Per-characteristics methods for Accelerometer Accelerometer_Configure
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyAccelerometer_ConfigureAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Accelerometer_Configure", ServiceIndex.Accelerometer_index, "Accelerometer", CharacteristicIndex.Accelerometer_Accelerometer_Configure_index, NotifyAccelerometer_ConfigureCallback, notifyType);
            return retval;
        }

        private void NotifyAccelerometer_ConfigureCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Accelerometer_Accelerometer_Configure_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX|AccelerometerConfigure");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrAccelerometer_Data.TimestampMostRecent = args.Timestamp;
            CurrAccelerometer_Data.AccelerometerConfigure = vr.GetNextDouble();
            OnPropertyChanged(Accelerometer_ConfigurePropertyChangedName); // "Accelerometer_Configure"
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
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|DEC|AccelerometerPeriod");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrAccelerometer_Data.TimestampMostRecent = args.Timestamp;
            CurrAccelerometer_Data.AccelerometerPeriod = vr.GetNextDouble();
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

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("I8^64_/|FIXED|AccelX|g I8^64_/|FIXED|AccelY|g I8^64_/_IV|FIXED|AccelZ|g");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrAccelerometer_Data.AccelX = vr.GetNextDouble();
            CurrAccelerometer_Data.AccelY = vr.GetNextDouble();
            CurrAccelerometer_Data.AccelZ = vr.GetNextDouble();
            CurrAccelerometer_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Accelerometer_DataPropertyChangedName); // "Accelerometer_Data"
            return CurrAccelerometer_Data;
        }
        /// <summary>
        /// Reads data from Accelerometer Configure and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Accelerometer_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Accelerometer_Data> ReadAccelerometer_Configure(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Accelerometer_Accelerometer_Configure_index;
            await Ensure_Characteristic_Async(ServiceIndex.Accelerometer_index, "Accelerometer", index, "Accelerometer Configure");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Accelerometer Configure", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX|AccelerometerConfigure");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrAccelerometer_Data.AccelerometerConfigure = vr.GetNextDouble();
            CurrAccelerometer_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Accelerometer_ConfigurePropertyChangedName); // "Accelerometer_Configure"
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

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|DEC|AccelerometerPeriod");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrAccelerometer_Data.AccelerometerPeriod = vr.GetNextDouble();
            CurrAccelerometer_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Accelerometer_PeriodPropertyChangedName); // "Accelerometer_Period"
            return CurrAccelerometer_Data;
        }
        /// <summary>
        /// Writes data to Accelerometer Configure 
        /// </summary>
        public async Task WriteAccelerometer_Configure(byte[] data)
        {
            var index = CharacteristicIndex.Accelerometer_Accelerometer_Configure_index;
            await Ensure_Characteristic_Async(ServiceIndex.Accelerometer_index, "Accelerometer", index, "Accelerometer Configure");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteAccelerometer_Configure", result);
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
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX|KeyPressState");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrKey_Press_Data.TimestampMostRecent = args.Timestamp;
            CurrKey_Press_Data.KeyPressState = vr.GetNextDouble();
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

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX|KeyPressState");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrKey_Press_Data.KeyPressState = vr.GetNextDouble();
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
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("I16|HEX|ObjTemp|C I16|HEX|AmbientTemp|C");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrIR_Service_Data.TimestampMostRecent = args.Timestamp;
            CurrIR_Service_Data.ObjTemp = vr.GetNextDouble();
            CurrIR_Service_Data.AmbientTemp = vr.GetNextDouble();
            OnPropertyChanged(IR_DataPropertyChangedName); // "IR_Data"
        }
        // Per-characteristics methods for IR_Service IR_Service_Configure
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyIR_Service_ConfigureAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("IR_Service_Configure", ServiceIndex.IR_Service_index, "IR Service", CharacteristicIndex.IR_Service_IR_Service_Configure_index, NotifyIR_Service_ConfigureCallback, notifyType);
            return retval;
        }

        private void NotifyIR_Service_ConfigureCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.IR_Service_IR_Service_Configure_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX|IRConfigure");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrIR_Service_Data.TimestampMostRecent = args.Timestamp;
            CurrIR_Service_Data.IRConfigure = vr.GetNextDouble();
            OnPropertyChanged(IR_Service_ConfigurePropertyChangedName); // "IR_Service_Configure"
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
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|DEC|IRPeriod");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrIR_Service_Data.TimestampMostRecent = args.Timestamp;
            CurrIR_Service_Data.IRPeriod = vr.GetNextDouble();
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

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("I16|HEX|ObjTemp|C I16|HEX|AmbientTemp|C");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrIR_Service_Data.ObjTemp = vr.GetNextDouble();
            CurrIR_Service_Data.AmbientTemp = vr.GetNextDouble();
            CurrIR_Service_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(IR_DataPropertyChangedName); // "IR_Data"
            return CurrIR_Service_Data;
        }
        /// <summary>
        /// Reads data from IR Service Configure and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>IR_Service_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<IR_Service_Data> ReadIR_Service_Configure(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.IR_Service_IR_Service_Configure_index;
            await Ensure_Characteristic_Async(ServiceIndex.IR_Service_index, "IR Service", index, "IR Service Configure");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "IR Service Configure", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX|IRConfigure");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrIR_Service_Data.IRConfigure = vr.GetNextDouble();
            CurrIR_Service_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(IR_Service_ConfigurePropertyChangedName); // "IR_Service_Configure"
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

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|DEC|IRPeriod");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrIR_Service_Data.IRPeriod = vr.GetNextDouble();
            CurrIR_Service_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(IR_Service_PeriodPropertyChangedName); // "IR_Service_Period"
            return CurrIR_Service_Data;
        }
        /// <summary>
        /// Writes data to IR Service Configure 
        /// </summary>
        public async Task WriteIR_Service_Configure(byte[] data)
        {
            var index = CharacteristicIndex.IR_Service_IR_Service_Configure_index;
            await Ensure_Characteristic_Async(ServiceIndex.IR_Service_index, "IR Service", index, "IR Service Configure");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteIR_Service_Configure", result);
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
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U16^175.72_*_65536_/_46.86_-|FIXED|Temp U16^125.0_*_65536_/_6.0_-|FIXED|Humidity");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrHumidity_Data.TimestampMostRecent = args.Timestamp;
            CurrHumidity_Data.Temp = vr.GetNextDouble();
            CurrHumidity_Data.Humidity = vr.GetNextDouble();
            OnPropertyChanged(Humidity_DataPropertyChangedName); // "Humidity_Data"
        }
        // Per-characteristics methods for Humidity Humidity_Configure
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyHumidity_ConfigureAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Humidity_Configure", ServiceIndex.Humidity_index, "Humidity", CharacteristicIndex.Humidity_Humidity_Configure_index, NotifyHumidity_ConfigureCallback, notifyType);
            return retval;
        }

        private void NotifyHumidity_ConfigureCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Humidity_Humidity_Configure_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX|HumidityConfigure");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrHumidity_Data.TimestampMostRecent = args.Timestamp;
            CurrHumidity_Data.HumidityConfigure = vr.GetNextDouble();
            OnPropertyChanged(Humidity_ConfigurePropertyChangedName); // "Humidity_Configure"
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
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|DEC|HumidityPeriod");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrHumidity_Data.TimestampMostRecent = args.Timestamp;
            CurrHumidity_Data.HumidityPeriod = vr.GetNextDouble();
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

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U16^175.72_*_65536_/_46.86_-|FIXED|Temp U16^125.0_*_65536_/_6.0_-|FIXED|Humidity");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrHumidity_Data.Temp = vr.GetNextDouble();
            CurrHumidity_Data.Humidity = vr.GetNextDouble();
            CurrHumidity_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Humidity_DataPropertyChangedName); // "Humidity_Data"
            return CurrHumidity_Data;
        }
        /// <summary>
        /// Reads data from Humidity Configure and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Humidity_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Humidity_Data> ReadHumidity_Configure(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Humidity_Humidity_Configure_index;
            await Ensure_Characteristic_Async(ServiceIndex.Humidity_index, "Humidity", index, "Humidity Configure");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Humidity Configure", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX|HumidityConfigure");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrHumidity_Data.HumidityConfigure = vr.GetNextDouble();
            CurrHumidity_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Humidity_ConfigurePropertyChangedName); // "Humidity_Configure"
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

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|DEC|HumidityPeriod");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrHumidity_Data.HumidityPeriod = vr.GetNextDouble();
            CurrHumidity_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Humidity_PeriodPropertyChangedName); // "Humidity_Period"
            return CurrHumidity_Data;
        }
        /// <summary>
        /// Writes data to Humidity Configure 
        /// </summary>
        public async Task WriteHumidity_Configure(byte[] data)
        {
            var index = CharacteristicIndex.Humidity_Humidity_Configure_index;
            await Ensure_Characteristic_Async(ServiceIndex.Humidity_index, "Humidity", index, "Humidity Configure");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteHumidity_Configure", result);
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
        #region Service_Magnetometer
        // Service Magnetometer 

        public Magnetometer_Data CurrMagnetometer_Data { get; set; } = new Magnetometer_Data();

        // Per-characteristics methods for Magnetometer Magnetometer_Data
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyMagnetometer_DataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Magnetometer_Data", ServiceIndex.Magnetometer_index, "Magnetometer", CharacteristicIndex.Magnetometer_Magnetometer_Data_index, NotifyMagnetometer_DataCallback, notifyType);
            return retval;
        }

        private void NotifyMagnetometer_DataCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Magnetometer_Magnetometer_Data_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("I16^2000_*_65536_/_IV|FIXED|X I16^2000_*_65536_/_IV|FIXED|Y I16^2000_*_65536_/|FIXED|Z");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrMagnetometer_Data.TimestampMostRecent = args.Timestamp;
            CurrMagnetometer_Data.X = vr.GetNextDouble();
            CurrMagnetometer_Data.Y = vr.GetNextDouble();
            CurrMagnetometer_Data.Z = vr.GetNextDouble();
            OnPropertyChanged(Magnetometer_DataPropertyChangedName); // "Magnetometer_Data"
        }
        // Per-characteristics methods for Magnetometer Magnetometer_Configure
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyMagnetometer_ConfigureAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Magnetometer_Configure", ServiceIndex.Magnetometer_index, "Magnetometer", CharacteristicIndex.Magnetometer_Magnetometer_Configure_index, NotifyMagnetometer_ConfigureCallback, notifyType);
            return retval;
        }

        private void NotifyMagnetometer_ConfigureCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Magnetometer_Magnetometer_Configure_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX|MagnetometerConfigure");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrMagnetometer_Data.TimestampMostRecent = args.Timestamp;
            CurrMagnetometer_Data.MagnetometerConfigure = vr.GetNextDouble();
            OnPropertyChanged(Magnetometer_ConfigurePropertyChangedName); // "Magnetometer_Configure"
        }
        // Per-characteristics methods for Magnetometer Magnetometer_Period
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyMagnetometer_PeriodAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Magnetometer_Period", ServiceIndex.Magnetometer_index, "Magnetometer", CharacteristicIndex.Magnetometer_Magnetometer_Period_index, NotifyMagnetometer_PeriodCallback, notifyType);
            return retval;
        }

        private void NotifyMagnetometer_PeriodCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Magnetometer_Magnetometer_Period_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|DEC|MagnetometerPeriod");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrMagnetometer_Data.TimestampMostRecent = args.Timestamp;
            CurrMagnetometer_Data.MagnetometerPeriod = vr.GetNextDouble();
            OnPropertyChanged(Magnetometer_PeriodPropertyChangedName); // "Magnetometer_Period"
        }
        /// <summary>
        /// Reads data from Magnetometer Data and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Magnetometer_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Magnetometer_Data> ReadMagnetometer_Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Magnetometer_Magnetometer_Data_index;
            await Ensure_Characteristic_Async(ServiceIndex.Magnetometer_index, "Magnetometer", index, "Magnetometer Data");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Magnetometer Data", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("I16^2000_*_65536_/_IV|FIXED|X I16^2000_*_65536_/_IV|FIXED|Y I16^2000_*_65536_/|FIXED|Z");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrMagnetometer_Data.X = vr.GetNextDouble();
            CurrMagnetometer_Data.Y = vr.GetNextDouble();
            CurrMagnetometer_Data.Z = vr.GetNextDouble();
            CurrMagnetometer_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Magnetometer_DataPropertyChangedName); // "Magnetometer_Data"
            return CurrMagnetometer_Data;
        }
        /// <summary>
        /// Reads data from Magnetometer Configure and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Magnetometer_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Magnetometer_Data> ReadMagnetometer_Configure(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Magnetometer_Magnetometer_Configure_index;
            await Ensure_Characteristic_Async(ServiceIndex.Magnetometer_index, "Magnetometer", index, "Magnetometer Configure");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Magnetometer Configure", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX|MagnetometerConfigure");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrMagnetometer_Data.MagnetometerConfigure = vr.GetNextDouble();
            CurrMagnetometer_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Magnetometer_ConfigurePropertyChangedName); // "Magnetometer_Configure"
            return CurrMagnetometer_Data;
        }
        /// <summary>
        /// Reads data from Magnetometer Period and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Magnetometer_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Magnetometer_Data> ReadMagnetometer_Period(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Magnetometer_Magnetometer_Period_index;
            await Ensure_Characteristic_Async(ServiceIndex.Magnetometer_index, "Magnetometer", index, "Magnetometer Period");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Magnetometer Period", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|DEC|MagnetometerPeriod");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrMagnetometer_Data.MagnetometerPeriod = vr.GetNextDouble();
            CurrMagnetometer_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Magnetometer_PeriodPropertyChangedName); // "Magnetometer_Period"
            return CurrMagnetometer_Data;
        }
        /// <summary>
        /// Writes data to Magnetometer Configure 
        /// </summary>
        public async Task WriteMagnetometer_Configure(byte[] data)
        {
            var index = CharacteristicIndex.Magnetometer_Magnetometer_Configure_index;
            await Ensure_Characteristic_Async(ServiceIndex.Magnetometer_index, "Magnetometer", index, "Magnetometer Configure");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteMagnetometer_Configure", result);
        }
        /// <summary>
        /// Writes data to Magnetometer Period 
        /// </summary>
        public async Task WriteMagnetometer_Period(byte[] data)
        {
            var index = CharacteristicIndex.Magnetometer_Magnetometer_Period_index;
            await Ensure_Characteristic_Async(ServiceIndex.Magnetometer_index, "Magnetometer", index, "Magnetometer Period");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteMagnetometer_Period", result);
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
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U16|HEX|TempRaw U16|HEX|PressureRaw");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrBarometer_Data.TimestampMostRecent = args.Timestamp;
            CurrBarometer_Data.TempRaw = vr.GetNextDouble();
            CurrBarometer_Data.PressureRaw = vr.GetNextDouble();
            OnPropertyChanged(Barometer_DataPropertyChangedName); // "Barometer_Data"
        }
        // Per-characteristics methods for Barometer Barometer_Configure
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyBarometer_ConfigureAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Barometer_Configure", ServiceIndex.Barometer_index, "Barometer", CharacteristicIndex.Barometer_Barometer_Configure_index, NotifyBarometer_ConfigureCallback, notifyType);
            return retval;
        }

        private void NotifyBarometer_ConfigureCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Barometer_Barometer_Configure_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX|BarometerConfigure");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrBarometer_Data.TimestampMostRecent = args.Timestamp;
            CurrBarometer_Data.BarometerConfigure = vr.GetNextDouble();
            OnPropertyChanged(Barometer_ConfigurePropertyChangedName); // "Barometer_Configure"
        }
        // Per-characteristics methods for Barometer Barometer_Calibration
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyBarometer_CalibrationAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Barometer_Calibration", ServiceIndex.Barometer_index, "Barometer", CharacteristicIndex.Barometer_Barometer_Calibration_index, NotifyBarometer_CalibrationCallback, notifyType);
            return retval;
        }

        private void NotifyBarometer_CalibrationCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Barometer_Barometer_Calibration_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|BarometerCalibration");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrBarometer_Data.TimestampMostRecent = args.Timestamp;
            CurrBarometer_Data.BarometerCalibration = vr.GetNextByteArray();
            OnPropertyChanged(Barometer_CalibrationPropertyChangedName); // "Barometer_Calibration"
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
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|DEC|BarometerPeriod");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrBarometer_Data.TimestampMostRecent = args.Timestamp;
            CurrBarometer_Data.BarometerPeriod = vr.GetNextDouble();
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

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U16|HEX|TempRaw U16|HEX|PressureRaw");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrBarometer_Data.TempRaw = vr.GetNextDouble();
            CurrBarometer_Data.PressureRaw = vr.GetNextDouble();
            CurrBarometer_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Barometer_DataPropertyChangedName); // "Barometer_Data"
            return CurrBarometer_Data;
        }
        /// <summary>
        /// Reads data from Barometer Configure and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Barometer_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Barometer_Data> ReadBarometer_Configure(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Barometer_Barometer_Configure_index;
            await Ensure_Characteristic_Async(ServiceIndex.Barometer_index, "Barometer", index, "Barometer Configure");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Barometer Configure", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX|BarometerConfigure");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrBarometer_Data.BarometerConfigure = vr.GetNextDouble();
            CurrBarometer_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Barometer_ConfigurePropertyChangedName); // "Barometer_Configure"
            return CurrBarometer_Data;
        }
        /// <summary>
        /// Reads data from Barometer Calibration and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Barometer_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Barometer_Data> ReadBarometer_Calibration(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Barometer_Barometer_Calibration_index;
            await Ensure_Characteristic_Async(ServiceIndex.Barometer_index, "Barometer", index, "Barometer Calibration");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Barometer Calibration", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|BarometerCalibration");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrBarometer_Data.BarometerCalibration = vr.GetNextByteArray();
            CurrBarometer_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Barometer_CalibrationPropertyChangedName); // "Barometer_Calibration"
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

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|DEC|BarometerPeriod");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrBarometer_Data.BarometerPeriod = vr.GetNextDouble();
            CurrBarometer_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Barometer_PeriodPropertyChangedName); // "Barometer_Period"
            return CurrBarometer_Data;
        }
        /// <summary>
        /// Writes data to Barometer Configure 
        /// </summary>
        public async Task WriteBarometer_Configure(byte[] data)
        {
            var index = CharacteristicIndex.Barometer_Barometer_Configure_index;
            await Ensure_Characteristic_Async(ServiceIndex.Barometer_index, "Barometer", index, "Barometer Configure");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteBarometer_Configure", result);
        }

        #endregion
//
        #region Service_Gyroscope
        // Service Gyroscope 

        public Gyroscope_Data CurrGyroscope_Data { get; set; } = new Gyroscope_Data();

        // Per-characteristics methods for Gyroscope Gyroscope_Data
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyGyroscope_DataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Gyroscope_Data", ServiceIndex.Gyroscope_index, "Gyroscope", CharacteristicIndex.Gyroscope_Gyroscope_Data_index, NotifyGyroscope_DataCallback, notifyType);
            return retval;
        }

        private void NotifyGyroscope_DataCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Gyroscope_Gyroscope_Data_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("I16^500_*_65536_/_IV|FIXED|X I16^500_*_65536_/|FIXED|Y I16^500_*_65536_/|FIXED|Z");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrGyroscope_Data.TimestampMostRecent = args.Timestamp;
            CurrGyroscope_Data.X = vr.GetNextDouble();
            CurrGyroscope_Data.Y = vr.GetNextDouble();
            CurrGyroscope_Data.Z = vr.GetNextDouble();
            OnPropertyChanged(Gyroscope_DataPropertyChangedName); // "Gyroscope_Data"
        }
        // Per-characteristics methods for Gyroscope Gyroscope_Configure
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyGyroscope_ConfigureAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Gyroscope_Configure", ServiceIndex.Gyroscope_index, "Gyroscope", CharacteristicIndex.Gyroscope_Gyroscope_Configure_index, NotifyGyroscope_ConfigureCallback, notifyType);
            return retval;
        }

        private void NotifyGyroscope_ConfigureCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Gyroscope_Gyroscope_Configure_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX|GyroscopeConfigure");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrGyroscope_Data.TimestampMostRecent = args.Timestamp;
            CurrGyroscope_Data.GyroscopeConfigure = vr.GetNextDouble();
            OnPropertyChanged(Gyroscope_ConfigurePropertyChangedName); // "Gyroscope_Configure"
        }
        // Per-characteristics methods for Gyroscope Gyroscope_Period
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyGyroscope_PeriodAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Gyroscope_Period", ServiceIndex.Gyroscope_index, "Gyroscope", CharacteristicIndex.Gyroscope_Gyroscope_Period_index, NotifyGyroscope_PeriodCallback, notifyType);
            return retval;
        }

        private void NotifyGyroscope_PeriodCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Gyroscope_Gyroscope_Period_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|DEC|GyroscopePeriod");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrGyroscope_Data.TimestampMostRecent = args.Timestamp;
            CurrGyroscope_Data.GyroscopePeriod = vr.GetNextDouble();
            OnPropertyChanged(Gyroscope_PeriodPropertyChangedName); // "Gyroscope_Period"
        }
        /// <summary>
        /// Reads data from Gyroscope Data and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Gyroscope_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Gyroscope_Data> ReadGyroscope_Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Gyroscope_Gyroscope_Data_index;
            await Ensure_Characteristic_Async(ServiceIndex.Gyroscope_index, "Gyroscope", index, "Gyroscope Data");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Gyroscope Data", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("I16^500_*_65536_/_IV|FIXED|X I16^500_*_65536_/|FIXED|Y I16^500_*_65536_/|FIXED|Z");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrGyroscope_Data.X = vr.GetNextDouble();
            CurrGyroscope_Data.Y = vr.GetNextDouble();
            CurrGyroscope_Data.Z = vr.GetNextDouble();
            CurrGyroscope_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Gyroscope_DataPropertyChangedName); // "Gyroscope_Data"
            return CurrGyroscope_Data;
        }
        /// <summary>
        /// Reads data from Gyroscope Configure and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Gyroscope_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Gyroscope_Data> ReadGyroscope_Configure(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Gyroscope_Gyroscope_Configure_index;
            await Ensure_Characteristic_Async(ServiceIndex.Gyroscope_index, "Gyroscope", index, "Gyroscope Configure");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Gyroscope Configure", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX|GyroscopeConfigure");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrGyroscope_Data.GyroscopeConfigure = vr.GetNextDouble();
            CurrGyroscope_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Gyroscope_ConfigurePropertyChangedName); // "Gyroscope_Configure"
            return CurrGyroscope_Data;
        }
        /// <summary>
        /// Reads data from Gyroscope Period and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Gyroscope_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Gyroscope_Data> ReadGyroscope_Period(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Gyroscope_Gyroscope_Period_index;
            await Ensure_Characteristic_Async(ServiceIndex.Gyroscope_index, "Gyroscope", index, "Gyroscope Period");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Gyroscope Period", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|DEC|GyroscopePeriod");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrGyroscope_Data.GyroscopePeriod = vr.GetNextDouble();
            CurrGyroscope_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Gyroscope_PeriodPropertyChangedName); // "Gyroscope_Period"
            return CurrGyroscope_Data;
        }
        /// <summary>
        /// Writes data to Gyroscope Configure 
        /// </summary>
        public async Task WriteGyroscope_Configure(byte[] data)
        {
            var index = CharacteristicIndex.Gyroscope_Gyroscope_Configure_index;
            await Ensure_Characteristic_Async(ServiceIndex.Gyroscope_index, "Gyroscope", index, "Gyroscope Configure");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteGyroscope_Configure", result);
        }
        /// <summary>
        /// Writes data to Gyroscope Period 
        /// </summary>
        public async Task WriteGyroscope_Period(byte[] data)
        {
            var index = CharacteristicIndex.Gyroscope_Gyroscope_Period_index;
            await Ensure_Characteristic_Async(ServiceIndex.Gyroscope_index, "Gyroscope", index, "Gyroscope Period");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteGyroscope_Period", result);
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
        // Per-characteristics methods for Common_Configuration Privacy
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyPrivacyAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Privacy", ServiceIndex.Common_Configuration_index, "Common Configuration", CharacteristicIndex.Common_Configuration_Privacy_index, NotifyPrivacyCallback, notifyType);
            return retval;
        }

        private void NotifyPrivacyCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Common_Configuration_Privacy_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|Privacy");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrCommon_Configuration_Data.TimestampMostRecent = args.Timestamp;
            CurrCommon_Configuration_Data.Privacy = vr.GetNextByteArray();
            OnPropertyChanged(PrivacyPropertyChangedName); // "Privacy"
        }
        // Per-characteristics methods for Common_Configuration Reconnect_Address
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyReconnect_AddressAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Reconnect_Address", ServiceIndex.Common_Configuration_index, "Common Configuration", CharacteristicIndex.Common_Configuration_Reconnect_Address_index, NotifyReconnect_AddressCallback, notifyType);
            return retval;
        }

        private void NotifyReconnect_AddressCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Common_Configuration_Reconnect_Address_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|ReconnectAddress");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrCommon_Configuration_Data.TimestampMostRecent = args.Timestamp;
            CurrCommon_Configuration_Data.ReconnectAddress = vr.GetNextByteArray();
            OnPropertyChanged(Reconnect_AddressPropertyChangedName); // "Reconnect_Address"
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
        /// Reads data from Privacy and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Common_Configuration_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Common_Configuration_Data> ReadPrivacy(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Common_Configuration_Privacy_index;
            await Ensure_Characteristic_Async(ServiceIndex.Common_Configuration_index, "Common Configuration", index, "Privacy");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Privacy", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|Privacy");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrCommon_Configuration_Data.Privacy = vr.GetNextByteArray();
            CurrCommon_Configuration_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(PrivacyPropertyChangedName); // "Privacy"
            return CurrCommon_Configuration_Data;
        }
        /// <summary>
        /// Reads data from Reconnect Address and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Common_Configuration_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Common_Configuration_Data> ReadReconnect_Address(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Common_Configuration_Reconnect_Address_index;
            await Ensure_Characteristic_Async(ServiceIndex.Common_Configuration_index, "Common Configuration", index, "Reconnect Address");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Reconnect Address", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|ReconnectAddress");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrCommon_Configuration_Data.ReconnectAddress = vr.GetNextByteArray();
            CurrCommon_Configuration_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Reconnect_AddressPropertyChangedName); // "Reconnect_Address"
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
        /// <summary>
        /// Writes data to Reconnect Address 
        /// </summary>
        public async Task WriteReconnect_Address(byte[] data)
        {
            var index = CharacteristicIndex.Common_Configuration_Reconnect_Address_index;
            await Ensure_Characteristic_Async(ServiceIndex.Common_Configuration_index, "Common Configuration", index, "Reconnect Address");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteReconnect_Address", result);
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
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|ASCII|SystemId");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDevice_Info_Data.TimestampMostRecent = args.Timestamp;
            CurrDevice_Info_Data.SystemId = vr.GetNextString();
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
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|ASCII|FormwareRevision");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDevice_Info_Data.TimestampMostRecent = args.Timestamp;
            CurrDevice_Info_Data.FormwareRevision = vr.GetNextString();
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
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|ASCII|PnpID");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDevice_Info_Data.TimestampMostRecent = args.Timestamp;
            CurrDevice_Info_Data.PnpID = vr.GetNextString();
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

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("STRING|ASCII|SystemId");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDevice_Info_Data.SystemId = vr.GetNextString();
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

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("STRING|ASCII|FormwareRevision");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDevice_Info_Data.FormwareRevision = vr.GetNextString();
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

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("STRING|ASCII|PnpID");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDevice_Info_Data.PnpID = vr.GetNextString();
            CurrDevice_Info_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(PnP_IDPropertyChangedName); // "PnP_ID"
            return CurrDevice_Info_Data;
        }

        #endregion
//


// Long obsolete! [[zzMETHOD+LIST]]
    }
}