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
    /// The TI 1352 is the 2019 version in the TI range of Sensor Tags. Each battery-driven sensor tag includes a variety of sensors (light, humidity, accelerometer, and more) which are accessible from Bluetooth.
    /// This class was automatically generated 2026-09-09::20:11
    /// </summary>

    public partial class TI_SensorTag_1352 : INotifyPropertyChanged
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

        public string Name { get; } = "Multi-Sensor";
        public string Description { get; } = "The TI 1352 is the 2019 version in the TI range of Sensor Tags. Each battery-driven sensor tag includes a variety of sensors (light, humidity, accelerometer, and more) which are accessible from Bluetooth";

        /* Service and Characteristics summary for the device Multi-Sensor

        Temperature service Guid=f000aa00-0451-4000-b000-000000000000
            Temperature_Data (DataGroup record)
                Temperature Data characteristic has Temperature (Single-->double)  Guid=f000aa01-0451-4000-b000-000000000000
                Temperature Conf. characteristic has TemperatureEnable (Byte-->double)  Guid=f000aa02-0451-4000-b000-000000000000
                Temperature Period characteristic has TemperaturePeriod (Byte-->double)  Guid=f000aa03-0451-4000-b000-000000000000


        Humidity service Guid=f000aa20-0451-4000-b000-000000000000
            Humidity_Data (DataGroup record)
                Humidity Data characteristic has Humidity (Single-->double)  Guid=f000aa21-0451-4000-b000-000000000000
                Humidity Conf. characteristic has HumidityEnable (Byte-->double)  Guid=f000aa22-0451-4000-b000-000000000000
                Humidity Period characteristic has HumidityPeriod (Byte-->double)  Guid=f000aa23-0451-4000-b000-000000000000


        LED service Guid=f0001110-0451-4000-b000-000000000000
            LED_Data (DataGroup record)
                Red characteristic has Red (Byte-->double)  Guid=f0001111-0451-4000-b000-000000000000
                Green characteristic has Green (Byte-->double)  Guid=f0001112-0451-4000-b000-000000000000
                Blue characteristic has Blue (Byte-->double)  Guid=f0001113-0451-4000-b000-000000000000


        Button service Guid=f0001120-0451-4000-b000-000000000000
            Button_Data (DataGroup record)
                Button 0 characteristic has Button0 (Byte-->double)  Guid=f0001121-0451-4000-b000-000000000000
                Button 1 characteristic has Button1 (Byte-->double)  Guid=f0001122-0451-4000-b000-000000000000


        Accelerometer service Guid=f000ffa0-0451-4000-b000-000000000000
            Accelerometer_Data (DataGroup record)
                Accel Enable characteristic has Enable (Bytes-->string)  Guid=f000ffa1-0451-4000-b000-000000000000
                Accel Range characteristic has Accel_Range (UInt16-->double)  Guid=f000ffa2-0451-4000-b000-000000000000
                X characteristic has AccelX (Int16-->double)  Guid=f000ffa3-0451-4000-b000-000000000000
                Y characteristic has AccelY (Int16-->double)  Guid=f000ffa4-0451-4000-b000-000000000000
                Z characteristic has AccelZ (Int16-->double)  Guid=f000ffa5-0451-4000-b000-000000000000


        Optical Service service Guid=f000aa70-0451-4000-b000-000000000000
            Optical Service_Data (DataGroup record)
                Light Data characteristic has Lux (Single-->double)  Guid=f000aa71-0451-4000-b000-000000000000
                Light Conf. characteristic has Enable (Bytes-->string)  Guid=f000aa72-0451-4000-b000-000000000000
                Light Period characteristic has Light_Period (Bytes-->string)  Guid=f000aa73-0451-4000-b000-000000000000


        Battery service Guid=f000180f-0451-4000-b000-000000000000
            Battery_Data (DataGroup record)
                Battery Data characteristic has BatteryLevel (SByte-->double)  Guid=f0002a19-0451-4000-b000-000000000000


        Common Configuration service Guid=1800
            Common Configuration_Data (DataGroup record)
                Device Name characteristic has Device_Name (String-->string)  Guid=2a00
                Appearance characteristic has Appearance (UInt16-->double)  Guid=2a01
                Connection Parameter characteristic has ConnectionParameter (Bytes-->string)  Guid=2a04
                Central Address Resolution characteristic has AddressResolutionSupported (Byte-->double)  Guid=2aa6
                Resolvable Private Address Only characteristic has ResolvablePrivateAddressFlag (Byte-->double)  Guid=2ac9


        Device Info service Guid=180a
            Device Info_Data (DataGroup record)
                System ID characteristic has SystemId (String-->string)  Guid=2a23
                Model Number characteristic has ModelNumber (String-->string)  Guid=2a24
                Serial Number characteristic has SerialNumber (String-->string)  Guid=2a25
                Firmware Revision characteristic has FirmwareRevision (String-->string)  Guid=2a26
                Hardware Revision characteristic has HardwareRevision (String-->string)  Guid=2a27
                Software Revision characteristic has SoftwareRevision (String-->string)  Guid=2a28
                Manufacturer Name characteristic has ManufacturerName (String-->string)  Guid=2a29
                Regulatory List characteristic has BodyType (Byte-->double) BodyStructure (Byte-->double) Data (String-->string)  Guid=2a2a
                PnP ID characteristic has PnPID (String-->string)  Guid=2a50
        */

        public const string Temperature_DataPropertyChangedName = "Temperature_Data";
        public const string Temperature_ConfPropertyChangedName = "Temperature_Conf";
        public const string Temperature_PeriodPropertyChangedName = "Temperature_Period";
        public const string Humidity_DataPropertyChangedName = "Humidity_Data";
        public const string Humidity_ConfPropertyChangedName = "Humidity_Conf";
        public const string Humidity_PeriodPropertyChangedName = "Humidity_Period";
        public const string RedPropertyChangedName = "Red";
        public const string GreenPropertyChangedName = "Green";
        public const string BluePropertyChangedName = "Blue";
        public const string Button_0PropertyChangedName = "Button_0";
        public const string Button_1PropertyChangedName = "Button_1";
        public const string Accel_EnablePropertyChangedName = "Accel_Enable";
        public const string Accel_RangePropertyChangedName = "Accel_Range";
        public const string XPropertyChangedName = "X";
        public const string YPropertyChangedName = "Y";
        public const string ZPropertyChangedName = "Z";
        public const string Light_DataPropertyChangedName = "Light_Data";
        public const string Light_ConfPropertyChangedName = "Light_Conf";
        public const string Light_PeriodPropertyChangedName = "Light_Period";
        public const string Battery_DataPropertyChangedName = "Battery_Data";
        public const string Device_NamePropertyChangedName = "Device_Name";
        public const string AppearancePropertyChangedName = "Appearance";
        public const string Connection_ParameterPropertyChangedName = "Connection_Parameter";
        public const string Central_Address_ResolutionPropertyChangedName = "Central_Address_Resolution";
        public const string Resolvable_Private_Address_OnlyPropertyChangedName = "Resolvable_Private_Address_Only";
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
        /// Data from all of the characteristics in the Temperature Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class Temperature_Data :BTCommonMetaData<Temperature_Data> //, IExportDataSource
        {
            private double _Temperature = 0.0;
            /// <summary>
            /// Temperature (F32 C) from Service=Temperature and Characteristic=Temperature Data
            ///</summary>
            public double Temperature 
            { 
                get { return _Temperature; }
                set { if (value == _Temperature) return; _Temperature = value; OnPropertyChanged();}
            }

            private double _TemperatureEnable = 0;
            /// <summary>
            /// TemperatureEnable (U8 ) from Service=Temperature and Characteristic=Temperature Conf.
            ///</summary>
            public double TemperatureEnable 
            { 
                get { return _TemperatureEnable; }
                set { if (value == _TemperatureEnable) return; _TemperatureEnable = value; OnPropertyChanged();}
            }

            private double _TemperaturePeriod = 0;
            /// <summary>
            /// TemperaturePeriod (U8 10ms) from Service=Temperature and Characteristic=Temperature Period
            ///</summary>
            public double TemperaturePeriod 
            { 
                get { return _TemperaturePeriod; }
                set { if (value == _TemperaturePeriod) return; _TemperaturePeriod = value; OnPropertyChanged();}
            }
            public override Temperature_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as Temperature_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(Temperature_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.Temperature = source.Temperature;
                dest.TemperatureEnable = source.TemperatureEnable;
                dest.TemperaturePeriod = source.TemperaturePeriod;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static Temperature_Data CopyToWithConvertAndCreate(Temperature_Data source, Temperature_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.Temperature = convert(source.Temperature, "C");
                dest.TemperatureEnable = convert(source.TemperatureEnable, "");
                dest.TemperaturePeriod = convert(source.TemperaturePeriod, "10ms");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["Temperature", "TemperatureEnable", "TemperaturePeriod"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(Temperature);
                exporter.CellSet(TemperatureEnable);
                exporter.CellSet(TemperaturePeriod);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {Temperature} {TemperatureEnable} {TemperaturePeriod}");
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
            private double _Humidity = 0.0;
            /// <summary>
            /// Humidity (F32 Percent) from Service=Humidity and Characteristic=Humidity Data
            ///</summary>
            public double Humidity 
            { 
                get { return _Humidity; }
                set { if (value == _Humidity) return; _Humidity = value; OnPropertyChanged();}
            }

            private double _HumidityEnable = 0;
            /// <summary>
            /// HumidityEnable (U8 ) from Service=Humidity and Characteristic=Humidity Conf.
            ///</summary>
            public double HumidityEnable 
            { 
                get { return _HumidityEnable; }
                set { if (value == _HumidityEnable) return; _HumidityEnable = value; OnPropertyChanged();}
            }

            private double _HumidityPeriod = 0;
            /// <summary>
            /// HumidityPeriod (U8 10ms) from Service=Humidity and Characteristic=Humidity Period
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
                dest.Humidity = source.Humidity;
                dest.HumidityEnable = source.HumidityEnable;
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
                dest.Humidity = convert(source.Humidity, "Percent");
                dest.HumidityEnable = convert(source.HumidityEnable, "");
                dest.HumidityPeriod = convert(source.HumidityPeriod, "10ms");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["Humidity", "HumidityEnable", "HumidityPeriod"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(Humidity);
                exporter.CellSet(HumidityEnable);
                exporter.CellSet(HumidityPeriod);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {Humidity} {HumidityEnable} {HumidityPeriod}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the LED Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class LED_Data :BTCommonMetaData<LED_Data> //, IExportDataSource
        {
            private double _Red = 0;
            /// <summary>
            /// Red (U8 ) from Service=LED and Characteristic=Red
            ///</summary>
            public double Red 
            { 
                get { return _Red; }
                set { if (value == _Red) return; _Red = value; OnPropertyChanged();}
            }

            private double _Green = 0;
            /// <summary>
            /// Green (U8 ) from Service=LED and Characteristic=Green
            ///</summary>
            public double Green 
            { 
                get { return _Green; }
                set { if (value == _Green) return; _Green = value; OnPropertyChanged();}
            }

            private double _Blue = 0;
            /// <summary>
            /// Blue (U8 ) from Service=LED and Characteristic=Blue
            ///</summary>
            public double Blue 
            { 
                get { return _Blue; }
                set { if (value == _Blue) return; _Blue = value; OnPropertyChanged();}
            }
            public override LED_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as LED_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(LED_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.Red = source.Red;
                dest.Green = source.Green;
                dest.Blue = source.Blue;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static LED_Data CopyToWithConvertAndCreate(LED_Data source, LED_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.Red = convert(source.Red, "");
                dest.Green = convert(source.Green, "");
                dest.Blue = convert(source.Blue, "");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["Red", "Green", "Blue"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(Red);
                exporter.CellSet(Green);
                exporter.CellSet(Blue);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {Red} {Green} {Blue}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the Button Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class Button_Data :BTCommonMetaData<Button_Data> //, IExportDataSource
        {
            private double _Button0 = 0;
            /// <summary>
            /// Button0 (U8 ) from Service=Button and Characteristic=Button 0
            ///</summary>
            public double Button0 
            { 
                get { return _Button0; }
                set { if (value == _Button0) return; _Button0 = value; OnPropertyChanged();}
            }

            private double _Button1 = 0;
            /// <summary>
            /// Button1 (U8 ) from Service=Button and Characteristic=Button 1
            ///</summary>
            public double Button1 
            { 
                get { return _Button1; }
                set { if (value == _Button1) return; _Button1 = value; OnPropertyChanged();}
            }
            public override Button_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as Button_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(Button_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.Button0 = source.Button0;
                dest.Button1 = source.Button1;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static Button_Data CopyToWithConvertAndCreate(Button_Data source, Button_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.Button0 = convert(source.Button0, "");
                dest.Button1 = convert(source.Button1, "");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["Button0", "Button1"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(Button0);
                exporter.CellSet(Button1);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {Button0} {Button1}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the Accelerometer Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class Accelerometer_Data :BTCommonMetaData<Accelerometer_Data> //, IExportDataSource
        {
            private byte[] _Enable = null;
            /// <summary>
            /// Enable (BYTES ) from Service=Accelerometer and Characteristic=Accel Enable
            ///</summary>
            public byte[] Enable 
            { 
                get { return _Enable; }
                set { if (value == _Enable) return; _Enable = value; OnPropertyChanged();}
            }

            private double _Accel_Range = 0;
            /// <summary>
            /// Accel_Range (U16 ) from Service=Accelerometer and Characteristic=Accel Range
            ///</summary>
            public double Accel_Range 
            { 
                get { return _Accel_Range; }
                set { if (value == _Accel_Range) return; _Accel_Range = value; OnPropertyChanged();}
            }

            private double _AccelX = 0;
            /// <summary>
            /// AccelX (I16 ) from Service=Accelerometer and Characteristic=X
            ///</summary>
            public double AccelX 
            { 
                get { return _AccelX; }
                set { if (value == _AccelX) return; _AccelX = value; OnPropertyChanged();}
            }

            private double _AccelY = 0;
            /// <summary>
            /// AccelY (I16 ) from Service=Accelerometer and Characteristic=Y
            ///</summary>
            public double AccelY 
            { 
                get { return _AccelY; }
                set { if (value == _AccelY) return; _AccelY = value; OnPropertyChanged();}
            }

            private double _AccelZ = 0;
            /// <summary>
            /// AccelZ (I16 ) from Service=Accelerometer and Characteristic=Z
            ///</summary>
            public double AccelZ 
            { 
                get { return _AccelZ; }
                set { if (value == _AccelZ) return; _AccelZ = value; OnPropertyChanged();}
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
                dest.Enable = source.Enable;
                dest.Accel_Range = source.Accel_Range;
                dest.AccelX = source.AccelX;
                dest.AccelY = source.AccelY;
                dest.AccelZ = source.AccelZ;
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
                dest.Enable = source.Enable;
                dest.Accel_Range = convert(source.Accel_Range, "");
                dest.AccelX = convert(source.AccelX, "");
                dest.AccelY = convert(source.AccelY, "");
                dest.AccelZ = convert(source.AccelZ, "");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["Enable", "Accel_Range", "AccelX", "AccelY", "AccelZ"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(Enable);
                exporter.CellSet(Accel_Range);
                exporter.CellSet(AccelX);
                exporter.CellSet(AccelY);
                exporter.CellSet(AccelZ);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {Enable} {Accel_Range} {AccelX} {AccelY} {AccelZ}");
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
            private double _Lux = 0.0;
            /// <summary>
            /// Lux (F32 ) from Service=Optical Service and Characteristic=Light Data
            ///</summary>
            public double Lux 
            { 
                get { return _Lux; }
                set { if (value == _Lux) return; _Lux = value; OnPropertyChanged();}
            }

            private byte[] _Enable = null;
            /// <summary>
            /// Enable (BYTES ) from Service=Optical Service and Characteristic=Light Conf.
            ///</summary>
            public byte[] Enable 
            { 
                get { return _Enable; }
                set { if (value == _Enable) return; _Enable = value; OnPropertyChanged();}
            }

            private byte[] _Light_Period = null;
            /// <summary>
            /// Light_Period (BYTES ) from Service=Optical Service and Characteristic=Light Period
            ///</summary>
            public byte[] Light_Period 
            { 
                get { return _Light_Period; }
                set { if (value == _Light_Period) return; _Light_Period = value; OnPropertyChanged();}
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
                dest.Light_Period = source.Light_Period;
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
                dest.Enable = source.Enable;
                dest.Light_Period = source.Light_Period;
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["Lux", "Enable", "Light_Period"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(Lux);
                exporter.CellSet(Enable);
                exporter.CellSet(Light_Period);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {Lux} {Enable} {Light_Period}");
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
            /// BatteryLevel (I8 %) from Service=Battery and Characteristic=Battery Data
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

            private double _AddressResolutionSupported = 0;
            /// <summary>
            /// AddressResolutionSupported (U8 ) from Service=Common Configuration and Characteristic=Central Address Resolution
            ///</summary>
            public double AddressResolutionSupported 
            { 
                get { return _AddressResolutionSupported; }
                set { if (value == _AddressResolutionSupported) return; _AddressResolutionSupported = value; OnPropertyChanged();}
            }

            private double _ResolvablePrivateAddressFlag = 0;
            /// <summary>
            /// ResolvablePrivateAddressFlag (U8 ) from Service=Common Configuration and Characteristic=Resolvable Private Address Only
            ///</summary>
            public double ResolvablePrivateAddressFlag 
            { 
                get { return _ResolvablePrivateAddressFlag; }
                set { if (value == _ResolvablePrivateAddressFlag) return; _ResolvablePrivateAddressFlag = value; OnPropertyChanged();}
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
                dest.AddressResolutionSupported = source.AddressResolutionSupported;
                dest.ResolvablePrivateAddressFlag = source.ResolvablePrivateAddressFlag;
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
                dest.AddressResolutionSupported = convert(source.AddressResolutionSupported, "");
                dest.ResolvablePrivateAddressFlag = convert(source.ResolvablePrivateAddressFlag, "");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["Device_Name", "Appearance", "ConnectionParameter", "AddressResolutionSupported", "ResolvablePrivateAddressFlag"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(Device_Name);
                exporter.CellSet(Appearance);
                exporter.CellSet(ConnectionParameter);
                exporter.CellSet(AddressResolutionSupported);
                exporter.CellSet(ResolvablePrivateAddressFlag);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {Device_Name} {Appearance} {ConnectionParameter} {AddressResolutionSupported} {ResolvablePrivateAddressFlag}");
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
                dest.SystemId = source.SystemId;
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
                dest.SystemId = source.SystemId;
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
                return ["SystemId", "ModelNumber", "SerialNumber", "FirmwareRevision", "HardwareRevision", "SoftwareRevision", "ManufacturerName", "BodyType", "BodyStructure", "Data", "PnPID"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(SystemId);
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
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {SystemId} {ModelNumber} {SerialNumber} {FirmwareRevision} {HardwareRevision} {SoftwareRevision} {ManufacturerName} {BodyType} {BodyStructure} {Data} {PnPID}");
            }
        }
//


        #endregion


        /// <summary>
        /// Enumeration of all services
        /// </summary>
        enum ServiceIndex
        {
            Temperature_index = 0,
            Humidity_index = 1,
            LED_index = 2,
            Button_index = 3,
            Accelerometer_index = 4,
            Optical_Service_index = 5,
            Battery_index = 6,
            Common_Configuration_index = 7,
            Device_Info_index = 8,
        }

        /// <summary>
        /// Enumeration of all characteristics in all of the services.
        /// </summary>
        enum CharacteristicIndex
        {
            Temperature_Temperature_Data_index = 0,     // GUID f000aa01-0451-4000-b000-000000000000
            Temperature_Temperature_Conf_index = 1,     // GUID f000aa02-0451-4000-b000-000000000000
            Temperature_Temperature_Period_index = 2,     // GUID f000aa03-0451-4000-b000-000000000000
            Humidity_Humidity_Data_index = 3,     // GUID f000aa21-0451-4000-b000-000000000000
            Humidity_Humidity_Conf_index = 4,     // GUID f000aa22-0451-4000-b000-000000000000
            Humidity_Humidity_Period_index = 5,     // GUID f000aa23-0451-4000-b000-000000000000
            LED_Red_index = 6,     // GUID f0001111-0451-4000-b000-000000000000
            LED_Green_index = 7,     // GUID f0001112-0451-4000-b000-000000000000
            LED_Blue_index = 8,     // GUID f0001113-0451-4000-b000-000000000000
            Button_Button_0_index = 9,     // GUID f0001121-0451-4000-b000-000000000000
            Button_Button_1_index = 10,     // GUID f0001122-0451-4000-b000-000000000000
            Accelerometer_Accel_Enable_index = 11,     // GUID f000ffa1-0451-4000-b000-000000000000
            Accelerometer_Accel_Range_index = 12,     // GUID f000ffa2-0451-4000-b000-000000000000
            Accelerometer_X_index = 13,     // GUID f000ffa3-0451-4000-b000-000000000000
            Accelerometer_Y_index = 14,     // GUID f000ffa4-0451-4000-b000-000000000000
            Accelerometer_Z_index = 15,     // GUID f000ffa5-0451-4000-b000-000000000000
            Optical_Service_Light_Data_index = 16,     // GUID f000aa71-0451-4000-b000-000000000000
            Optical_Service_Light_Conf_index = 17,     // GUID f000aa72-0451-4000-b000-000000000000
            Optical_Service_Light_Period_index = 18,     // GUID f000aa73-0451-4000-b000-000000000000
            Battery_Battery_Data_index = 19,     // GUID f0002a19-0451-4000-b000-000000000000
            Common_Configuration_Device_Name_index = 20,     // GUID 00002a00-0000-1000-8000-00805f9b34fb
            Common_Configuration_Appearance_index = 21,     // GUID 00002a01-0000-1000-8000-00805f9b34fb
            Common_Configuration_Connection_Parameter_index = 22,     // GUID 00002a04-0000-1000-8000-00805f9b34fb
            Common_Configuration_Central_Address_Resolution_index = 23,     // GUID 00002aa6-0000-1000-8000-00805f9b34fb
            Common_Configuration_Resolvable_Private_Address_Only_index = 24,     // GUID 00002ac9-0000-1000-8000-00805f9b34fb
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
            Guid.Parse("f000aa00-0451-4000-b000-000000000000"), // #0 is Temperature
            Guid.Parse("f000aa20-0451-4000-b000-000000000000"), // #1 is Humidity
            Guid.Parse("f0001110-0451-4000-b000-000000000000"), // #2 is LED
            Guid.Parse("f0001120-0451-4000-b000-000000000000"), // #3 is Button
            Guid.Parse("f000ffa0-0451-4000-b000-000000000000"), // #4 is Accelerometer
            Guid.Parse("f000aa70-0451-4000-b000-000000000000"), // #5 is Optical Service
            Guid.Parse("f000180f-0451-4000-b000-000000000000"), // #6 is Battery
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
            Guid.Parse("f000aa01-0451-4000-b000-000000000000"), // #0 is Temperature Temperature Data
            Guid.Parse("f000aa02-0451-4000-b000-000000000000"), // #1 is Temperature Temperature Conf.
            Guid.Parse("f000aa03-0451-4000-b000-000000000000"), // #2 is Temperature Temperature Period
            Guid.Parse("f000aa21-0451-4000-b000-000000000000"), // #3 is Humidity Humidity Data
            Guid.Parse("f000aa22-0451-4000-b000-000000000000"), // #4 is Humidity Humidity Conf.
            Guid.Parse("f000aa23-0451-4000-b000-000000000000"), // #5 is Humidity Humidity Period
            Guid.Parse("f0001111-0451-4000-b000-000000000000"), // #6 is LED Red
            Guid.Parse("f0001112-0451-4000-b000-000000000000"), // #7 is LED Green
            Guid.Parse("f0001113-0451-4000-b000-000000000000"), // #8 is LED Blue
            Guid.Parse("f0001121-0451-4000-b000-000000000000"), // #9 is Button Button 0
            Guid.Parse("f0001122-0451-4000-b000-000000000000"), // #10 is Button Button 1
            Guid.Parse("f000ffa1-0451-4000-b000-000000000000"), // #11 is Accelerometer Accel Enable
            Guid.Parse("f000ffa2-0451-4000-b000-000000000000"), // #12 is Accelerometer Accel Range
            Guid.Parse("f000ffa3-0451-4000-b000-000000000000"), // #13 is Accelerometer X
            Guid.Parse("f000ffa4-0451-4000-b000-000000000000"), // #14 is Accelerometer Y
            Guid.Parse("f000ffa5-0451-4000-b000-000000000000"), // #15 is Accelerometer Z
            Guid.Parse("f000aa71-0451-4000-b000-000000000000"), // #16 is Optical Service Light Data
            Guid.Parse("f000aa72-0451-4000-b000-000000000000"), // #17 is Optical Service Light Conf.
            Guid.Parse("f000aa73-0451-4000-b000-000000000000"), // #18 is Optical Service Light Period
            Guid.Parse("f0002a19-0451-4000-b000-000000000000"), // #19 is Battery Battery Data
            Guid.Parse("00002a00-0000-1000-8000-00805f9b34fb"), // #20 is Common Configuration Device Name
            Guid.Parse("00002a01-0000-1000-8000-00805f9b34fb"), // #21 is Common Configuration Appearance
            Guid.Parse("00002a04-0000-1000-8000-00805f9b34fb"), // #22 is Common Configuration Connection Parameter
            Guid.Parse("00002aa6-0000-1000-8000-00805f9b34fb"), // #23 is Common Configuration Central Address Resolution
            Guid.Parse("00002ac9-0000-1000-8000-00805f9b34fb"), // #24 is Common Configuration Resolvable Private Address Only
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


        #region Service_Temperature
        // Service Temperature 

        public Temperature_Data CurrTemperature_Data { get; set; } = new Temperature_Data();

        // Per-characteristics methods for Temperature Temperature_Data
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyTemperature_DataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Temperature_Data", ServiceIndex.Temperature_index, "Temperature", CharacteristicIndex.Temperature_Temperature_Data_index, NotifyTemperature_DataCallback, notifyType);
            return retval;
        }

        private void NotifyTemperature_DataCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Temperature_Temperature_Data_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("F32|FIXED|Temperature|C");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrTemperature_Data.TimestampMostRecent = args.Timestamp;
            CurrTemperature_Data.Temperature = vr.GetNextDouble();
            OnPropertyChanged(Temperature_DataPropertyChangedName); // "Temperature_Data"
        }
        // Per-characteristics methods for Temperature Temperature_Conf
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyTemperature_ConfAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Temperature_Conf", ServiceIndex.Temperature_index, "Temperature", CharacteristicIndex.Temperature_Temperature_Conf_index, NotifyTemperature_ConfCallback, notifyType);
            return retval;
        }

        private void NotifyTemperature_ConfCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Temperature_Temperature_Conf_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX|TemperatureEnable");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrTemperature_Data.TimestampMostRecent = args.Timestamp;
            CurrTemperature_Data.TemperatureEnable = vr.GetNextDouble();
            OnPropertyChanged(Temperature_ConfPropertyChangedName); // "Temperature_Conf"
        }
        // Per-characteristics methods for Temperature Temperature_Period
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyTemperature_PeriodAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Temperature_Period", ServiceIndex.Temperature_index, "Temperature", CharacteristicIndex.Temperature_Temperature_Period_index, NotifyTemperature_PeriodCallback, notifyType);
            return retval;
        }

        private void NotifyTemperature_PeriodCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Temperature_Temperature_Period_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|DEC|TemperaturePeriod|10ms");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrTemperature_Data.TimestampMostRecent = args.Timestamp;
            CurrTemperature_Data.TemperaturePeriod = vr.GetNextDouble();
            OnPropertyChanged(Temperature_PeriodPropertyChangedName); // "Temperature_Period"
        }
        /// <summary>
        /// Reads data from Temperature Data and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Temperature_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Temperature_Data> ReadTemperature_Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Temperature_Temperature_Data_index;
            await Ensure_Characteristic_Async(ServiceIndex.Temperature_index, "Temperature", index, "Temperature Data");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Temperature Data", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("F32|FIXED|Temperature|C");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrTemperature_Data.Temperature = vr.GetNextDouble();
            CurrTemperature_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Temperature_DataPropertyChangedName); // "Temperature_Data"
            return CurrTemperature_Data;
        }
        /// <summary>
        /// Reads data from Temperature Conf. and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Temperature_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Temperature_Data> ReadTemperature_Conf(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Temperature_Temperature_Conf_index;
            await Ensure_Characteristic_Async(ServiceIndex.Temperature_index, "Temperature", index, "Temperature Conf.");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Temperature Conf.", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX|TemperatureEnable");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrTemperature_Data.TemperatureEnable = vr.GetNextDouble();
            CurrTemperature_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Temperature_ConfPropertyChangedName); // "Temperature_Conf"
            return CurrTemperature_Data;
        }
        /// <summary>
        /// Reads data from Temperature Period and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Temperature_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Temperature_Data> ReadTemperature_Period(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Temperature_Temperature_Period_index;
            await Ensure_Characteristic_Async(ServiceIndex.Temperature_index, "Temperature", index, "Temperature Period");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Temperature Period", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|DEC|TemperaturePeriod|10ms");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrTemperature_Data.TemperaturePeriod = vr.GetNextDouble();
            CurrTemperature_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Temperature_PeriodPropertyChangedName); // "Temperature_Period"
            return CurrTemperature_Data;
        }
        /// <summary>
        /// Writes data to Temperature Conf. 
        /// </summary>
        public async Task WriteTemperature_Conf(byte[] data)
        {
            var index = CharacteristicIndex.Temperature_Temperature_Conf_index;
            await Ensure_Characteristic_Async(ServiceIndex.Temperature_index, "Temperature", index, "Temperature Conf.");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteTemperature_Conf", result);
        }
        /// <summary>
        /// Writes data to Temperature Period 
        /// </summary>
        public async Task WriteTemperature_Period(byte[] data)
        {
            var index = CharacteristicIndex.Temperature_Temperature_Period_index;
            await Ensure_Characteristic_Async(ServiceIndex.Temperature_index, "Temperature", index, "Temperature Period");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteTemperature_Period", result);
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
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("F32|FIXED|Humidity|Percent");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrHumidity_Data.TimestampMostRecent = args.Timestamp;
            CurrHumidity_Data.Humidity = vr.GetNextDouble();
            OnPropertyChanged(Humidity_DataPropertyChangedName); // "Humidity_Data"
        }
        // Per-characteristics methods for Humidity Humidity_Conf
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyHumidity_ConfAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Humidity_Conf", ServiceIndex.Humidity_index, "Humidity", CharacteristicIndex.Humidity_Humidity_Conf_index, NotifyHumidity_ConfCallback, notifyType);
            return retval;
        }

        private void NotifyHumidity_ConfCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Humidity_Humidity_Conf_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX|HumidityEnable");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrHumidity_Data.TimestampMostRecent = args.Timestamp;
            CurrHumidity_Data.HumidityEnable = vr.GetNextDouble();
            OnPropertyChanged(Humidity_ConfPropertyChangedName); // "Humidity_Conf"
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
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|DEC|HumidityPeriod|10ms");
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

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("F32|FIXED|Humidity|Percent");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrHumidity_Data.Humidity = vr.GetNextDouble();
            CurrHumidity_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Humidity_DataPropertyChangedName); // "Humidity_Data"
            return CurrHumidity_Data;
        }
        /// <summary>
        /// Reads data from Humidity Conf. and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Humidity_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Humidity_Data> ReadHumidity_Conf(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Humidity_Humidity_Conf_index;
            await Ensure_Characteristic_Async(ServiceIndex.Humidity_index, "Humidity", index, "Humidity Conf.");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Humidity Conf.", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX|HumidityEnable");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrHumidity_Data.HumidityEnable = vr.GetNextDouble();
            CurrHumidity_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Humidity_ConfPropertyChangedName); // "Humidity_Conf"
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

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|DEC|HumidityPeriod|10ms");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrHumidity_Data.HumidityPeriod = vr.GetNextDouble();
            CurrHumidity_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Humidity_PeriodPropertyChangedName); // "Humidity_Period"
            return CurrHumidity_Data;
        }
        /// <summary>
        /// Writes data to Humidity Conf. 
        /// </summary>
        public async Task WriteHumidity_Conf(byte[] data)
        {
            var index = CharacteristicIndex.Humidity_Humidity_Conf_index;
            await Ensure_Characteristic_Async(ServiceIndex.Humidity_index, "Humidity", index, "Humidity Conf.");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteHumidity_Conf", result);
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
        #region Service_LED
        // Service LED 

        public LED_Data CurrLED_Data { get; set; } = new LED_Data();

        // Per-characteristics methods for LED Red
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyRedAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Red", ServiceIndex.LED_index, "LED", CharacteristicIndex.LED_Red_index, NotifyRedCallback, notifyType);
            return retval;
        }

        private void NotifyRedCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.LED_Red_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX|Red");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrLED_Data.TimestampMostRecent = args.Timestamp;
            CurrLED_Data.Red = vr.GetNextDouble();
            OnPropertyChanged(RedPropertyChangedName); // "Red"
        }
        // Per-characteristics methods for LED Green
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyGreenAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Green", ServiceIndex.LED_index, "LED", CharacteristicIndex.LED_Green_index, NotifyGreenCallback, notifyType);
            return retval;
        }

        private void NotifyGreenCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.LED_Green_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX|Green");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrLED_Data.TimestampMostRecent = args.Timestamp;
            CurrLED_Data.Green = vr.GetNextDouble();
            OnPropertyChanged(GreenPropertyChangedName); // "Green"
        }
        // Per-characteristics methods for LED Blue
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyBlueAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Blue", ServiceIndex.LED_index, "LED", CharacteristicIndex.LED_Blue_index, NotifyBlueCallback, notifyType);
            return retval;
        }

        private void NotifyBlueCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.LED_Blue_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX|Blue");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrLED_Data.TimestampMostRecent = args.Timestamp;
            CurrLED_Data.Blue = vr.GetNextDouble();
            OnPropertyChanged(BluePropertyChangedName); // "Blue"
        }
        /// <summary>
        /// Reads data from Red and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>LED_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<LED_Data> ReadRed(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.LED_Red_index;
            await Ensure_Characteristic_Async(ServiceIndex.LED_index, "LED", index, "Red");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Red", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX|Red");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrLED_Data.Red = vr.GetNextDouble();
            CurrLED_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(RedPropertyChangedName); // "Red"
            return CurrLED_Data;
        }
        /// <summary>
        /// Reads data from Green and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>LED_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<LED_Data> ReadGreen(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.LED_Green_index;
            await Ensure_Characteristic_Async(ServiceIndex.LED_index, "LED", index, "Green");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Green", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX|Green");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrLED_Data.Green = vr.GetNextDouble();
            CurrLED_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(GreenPropertyChangedName); // "Green"
            return CurrLED_Data;
        }
        /// <summary>
        /// Reads data from Blue and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>LED_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<LED_Data> ReadBlue(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.LED_Blue_index;
            await Ensure_Characteristic_Async(ServiceIndex.LED_index, "LED", index, "Blue");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Blue", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX|Blue");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrLED_Data.Blue = vr.GetNextDouble();
            CurrLED_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(BluePropertyChangedName); // "Blue"
            return CurrLED_Data;
        }
        /// <summary>
        /// Writes data to Red 
        /// </summary>
        public async Task WriteRed(byte[] data)
        {
            var index = CharacteristicIndex.LED_Red_index;
            await Ensure_Characteristic_Async(ServiceIndex.LED_index, "LED", index, "Red");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteRed", result);
        }
        /// <summary>
        /// Writes data to Green 
        /// </summary>
        public async Task WriteGreen(byte[] data)
        {
            var index = CharacteristicIndex.LED_Green_index;
            await Ensure_Characteristic_Async(ServiceIndex.LED_index, "LED", index, "Green");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteGreen", result);
        }
        /// <summary>
        /// Writes data to Blue 
        /// </summary>
        public async Task WriteBlue(byte[] data)
        {
            var index = CharacteristicIndex.LED_Blue_index;
            await Ensure_Characteristic_Async(ServiceIndex.LED_index, "LED", index, "Blue");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteBlue", result);
        }

        #endregion
//
        #region Service_Button
        // Service Button 

        public Button_Data CurrButton_Data { get; set; } = new Button_Data();

        // Per-characteristics methods for Button Button_0
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyButton_0Async(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Button_0", ServiceIndex.Button_index, "Button", CharacteristicIndex.Button_Button_0_index, NotifyButton_0Callback, notifyType);
            return retval;
        }

        private void NotifyButton_0Callback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Button_Button_0_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX|Button0");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrButton_Data.TimestampMostRecent = args.Timestamp;
            CurrButton_Data.Button0 = vr.GetNextDouble();
            OnPropertyChanged(Button_0PropertyChangedName); // "Button_0"
        }
        // Per-characteristics methods for Button Button_1
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyButton_1Async(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Button_1", ServiceIndex.Button_index, "Button", CharacteristicIndex.Button_Button_1_index, NotifyButton_1Callback, notifyType);
            return retval;
        }

        private void NotifyButton_1Callback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Button_Button_1_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX|Button1");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrButton_Data.TimestampMostRecent = args.Timestamp;
            CurrButton_Data.Button1 = vr.GetNextDouble();
            OnPropertyChanged(Button_1PropertyChangedName); // "Button_1"
        }
        /// <summary>
        /// Reads data from Button 0 and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Button_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Button_Data> ReadButton_0(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Button_Button_0_index;
            await Ensure_Characteristic_Async(ServiceIndex.Button_index, "Button", index, "Button 0");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Button 0", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX|Button0");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrButton_Data.Button0 = vr.GetNextDouble();
            CurrButton_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Button_0PropertyChangedName); // "Button_0"
            return CurrButton_Data;
        }
        /// <summary>
        /// Reads data from Button 1 and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Button_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Button_Data> ReadButton_1(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Button_Button_1_index;
            await Ensure_Characteristic_Async(ServiceIndex.Button_index, "Button", index, "Button 1");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Button 1", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX|Button1");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrButton_Data.Button1 = vr.GetNextDouble();
            CurrButton_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Button_1PropertyChangedName); // "Button_1"
            return CurrButton_Data;
        }

        #endregion
//
        #region Service_Accelerometer
        // Service Accelerometer 

        public Accelerometer_Data CurrAccelerometer_Data { get; set; } = new Accelerometer_Data();

        // Per-characteristics methods for Accelerometer Accel_Enable
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyAccel_EnableAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Accel_Enable", ServiceIndex.Accelerometer_index, "Accelerometer", CharacteristicIndex.Accelerometer_Accel_Enable_index, NotifyAccel_EnableCallback, notifyType);
            return retval;
        }

        private void NotifyAccel_EnableCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Accelerometer_Accel_Enable_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|Enable");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrAccelerometer_Data.TimestampMostRecent = args.Timestamp;
            CurrAccelerometer_Data.Enable = vr.GetNextByteArray();
            OnPropertyChanged(Accel_EnablePropertyChangedName); // "Accel_Enable"
        }
        // Per-characteristics methods for Accelerometer Accel_Range
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyAccel_RangeAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Accel_Range", ServiceIndex.Accelerometer_index, "Accelerometer", CharacteristicIndex.Accelerometer_Accel_Range_index, NotifyAccel_RangeCallback, notifyType);
            return retval;
        }

        private void NotifyAccel_RangeCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Accelerometer_Accel_Range_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U16|HEX|Accel_Range");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrAccelerometer_Data.TimestampMostRecent = args.Timestamp;
            CurrAccelerometer_Data.Accel_Range = vr.GetNextDouble();
            OnPropertyChanged(Accel_RangePropertyChangedName); // "Accel_Range"
        }
        // Per-characteristics methods for Accelerometer X
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyXAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("X", ServiceIndex.Accelerometer_index, "Accelerometer", CharacteristicIndex.Accelerometer_X_index, NotifyXCallback, notifyType);
            return retval;
        }

        private void NotifyXCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Accelerometer_X_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("I16^1000_/|FIXED|AccelX");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrAccelerometer_Data.TimestampMostRecent = args.Timestamp;
            CurrAccelerometer_Data.AccelX = vr.GetNextDouble();
            OnPropertyChanged(XPropertyChangedName); // "X"
        }
        // Per-characteristics methods for Accelerometer Y
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyYAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Y", ServiceIndex.Accelerometer_index, "Accelerometer", CharacteristicIndex.Accelerometer_Y_index, NotifyYCallback, notifyType);
            return retval;
        }

        private void NotifyYCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Accelerometer_Y_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("I16^1000_/|FIXED|AccelY");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrAccelerometer_Data.TimestampMostRecent = args.Timestamp;
            CurrAccelerometer_Data.AccelY = vr.GetNextDouble();
            OnPropertyChanged(YPropertyChangedName); // "Y"
        }
        // Per-characteristics methods for Accelerometer Z
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyZAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Z", ServiceIndex.Accelerometer_index, "Accelerometer", CharacteristicIndex.Accelerometer_Z_index, NotifyZCallback, notifyType);
            return retval;
        }

        private void NotifyZCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Accelerometer_Z_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("I16^1000_/|FIXED|AccelZ");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrAccelerometer_Data.TimestampMostRecent = args.Timestamp;
            CurrAccelerometer_Data.AccelZ = vr.GetNextDouble();
            OnPropertyChanged(ZPropertyChangedName); // "Z"
        }
        /// <summary>
        /// Reads data from Accel Enable and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Accelerometer_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Accelerometer_Data> ReadAccel_Enable(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Accelerometer_Accel_Enable_index;
            await Ensure_Characteristic_Async(ServiceIndex.Accelerometer_index, "Accelerometer", index, "Accel Enable");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Accel Enable", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|Enable");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrAccelerometer_Data.Enable = vr.GetNextByteArray();
            CurrAccelerometer_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Accel_EnablePropertyChangedName); // "Accel_Enable"
            return CurrAccelerometer_Data;
        }
        /// <summary>
        /// Reads data from Accel Range and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Accelerometer_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Accelerometer_Data> ReadAccel_Range(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Accelerometer_Accel_Range_index;
            await Ensure_Characteristic_Async(ServiceIndex.Accelerometer_index, "Accelerometer", index, "Accel Range");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Accel Range", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U16|HEX|Accel_Range");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrAccelerometer_Data.Accel_Range = vr.GetNextDouble();
            CurrAccelerometer_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Accel_RangePropertyChangedName); // "Accel_Range"
            return CurrAccelerometer_Data;
        }
        /// <summary>
        /// Reads data from X and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Accelerometer_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Accelerometer_Data> ReadX(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Accelerometer_X_index;
            await Ensure_Characteristic_Async(ServiceIndex.Accelerometer_index, "Accelerometer", index, "X");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "X", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("I16^1000_/|FIXED|AccelX");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrAccelerometer_Data.AccelX = vr.GetNextDouble();
            CurrAccelerometer_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(XPropertyChangedName); // "X"
            return CurrAccelerometer_Data;
        }
        /// <summary>
        /// Reads data from Y and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Accelerometer_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Accelerometer_Data> ReadY(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Accelerometer_Y_index;
            await Ensure_Characteristic_Async(ServiceIndex.Accelerometer_index, "Accelerometer", index, "Y");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Y", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("I16^1000_/|FIXED|AccelY");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrAccelerometer_Data.AccelY = vr.GetNextDouble();
            CurrAccelerometer_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(YPropertyChangedName); // "Y"
            return CurrAccelerometer_Data;
        }
        /// <summary>
        /// Reads data from Z and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Accelerometer_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Accelerometer_Data> ReadZ(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Accelerometer_Z_index;
            await Ensure_Characteristic_Async(ServiceIndex.Accelerometer_index, "Accelerometer", index, "Z");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Z", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("I16^1000_/|FIXED|AccelZ");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrAccelerometer_Data.AccelZ = vr.GetNextDouble();
            CurrAccelerometer_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(ZPropertyChangedName); // "Z"
            return CurrAccelerometer_Data;
        }
        /// <summary>
        /// Writes data to Accel Enable 
        /// </summary>
        public async Task WriteAccel_Enable(byte[] data)
        {
            var index = CharacteristicIndex.Accelerometer_Accel_Enable_index;
            await Ensure_Characteristic_Async(ServiceIndex.Accelerometer_index, "Accelerometer", index, "Accel Enable");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteAccel_Enable", result);
        }

        #endregion
//
        #region Service_Optical_Service
        // Service Optical Service 

        public Optical_Service_Data CurrOptical_Service_Data { get; set; } = new Optical_Service_Data();

        // Per-characteristics methods for Optical_Service Light_Data
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyLight_DataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Light_Data", ServiceIndex.Optical_Service_index, "Optical Service", CharacteristicIndex.Optical_Service_Light_Data_index, NotifyLight_DataCallback, notifyType);
            return retval;
        }

        private void NotifyLight_DataCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Optical_Service_Light_Data_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("F32|FIXED^N0|Lux");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrOptical_Service_Data.TimestampMostRecent = args.Timestamp;
            CurrOptical_Service_Data.Lux = vr.GetNextDouble();
            OnPropertyChanged(Light_DataPropertyChangedName); // "Light_Data"
        }
        // Per-characteristics methods for Optical_Service Light_Conf
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyLight_ConfAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Light_Conf", ServiceIndex.Optical_Service_index, "Optical Service", CharacteristicIndex.Optical_Service_Light_Conf_index, NotifyLight_ConfCallback, notifyType);
            return retval;
        }

        private void NotifyLight_ConfCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Optical_Service_Light_Conf_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|Enable");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrOptical_Service_Data.TimestampMostRecent = args.Timestamp;
            CurrOptical_Service_Data.Enable = vr.GetNextByteArray();
            OnPropertyChanged(Light_ConfPropertyChangedName); // "Light_Conf"
        }
        // Per-characteristics methods for Optical_Service Light_Period
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyLight_PeriodAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Light_Period", ServiceIndex.Optical_Service_index, "Optical Service", CharacteristicIndex.Optical_Service_Light_Period_index, NotifyLight_PeriodCallback, notifyType);
            return retval;
        }

        private void NotifyLight_PeriodCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Optical_Service_Light_Period_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|Light_Period");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrOptical_Service_Data.TimestampMostRecent = args.Timestamp;
            CurrOptical_Service_Data.Light_Period = vr.GetNextByteArray();
            OnPropertyChanged(Light_PeriodPropertyChangedName); // "Light_Period"
        }
        /// <summary>
        /// Reads data from Light Data and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Optical_Service_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Optical_Service_Data> ReadLight_Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Optical_Service_Light_Data_index;
            await Ensure_Characteristic_Async(ServiceIndex.Optical_Service_index, "Optical Service", index, "Light Data");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Light Data", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("F32|FIXED^N0|Lux");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrOptical_Service_Data.Lux = vr.GetNextDouble();
            CurrOptical_Service_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Light_DataPropertyChangedName); // "Light_Data"
            return CurrOptical_Service_Data;
        }
        /// <summary>
        /// Reads data from Light Conf. and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Optical_Service_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Optical_Service_Data> ReadLight_Conf(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Optical_Service_Light_Conf_index;
            await Ensure_Characteristic_Async(ServiceIndex.Optical_Service_index, "Optical Service", index, "Light Conf.");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Light Conf.", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|Enable");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrOptical_Service_Data.Enable = vr.GetNextByteArray();
            CurrOptical_Service_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Light_ConfPropertyChangedName); // "Light_Conf"
            return CurrOptical_Service_Data;
        }
        /// <summary>
        /// Reads data from Light Period and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Optical_Service_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Optical_Service_Data> ReadLight_Period(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Optical_Service_Light_Period_index;
            await Ensure_Characteristic_Async(ServiceIndex.Optical_Service_index, "Optical Service", index, "Light Period");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Light Period", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|Light_Period");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrOptical_Service_Data.Light_Period = vr.GetNextByteArray();
            CurrOptical_Service_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Light_PeriodPropertyChangedName); // "Light_Period"
            return CurrOptical_Service_Data;
        }
        /// <summary>
        /// Writes data to Light Conf. 
        /// </summary>
        public async Task WriteLight_Conf(byte[] data)
        {
            var index = CharacteristicIndex.Optical_Service_Light_Conf_index;
            await Ensure_Characteristic_Async(ServiceIndex.Optical_Service_index, "Optical Service", index, "Light Conf.");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteLight_Conf", result);
        }
        /// <summary>
        /// Writes data to Light Period 
        /// </summary>
        public async Task WriteLight_Period(byte[] data)
        {
            var index = CharacteristicIndex.Optical_Service_Light_Period_index;
            await Ensure_Characteristic_Async(ServiceIndex.Optical_Service_index, "Optical Service", index, "Light Period");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var result = await ch.WriteValueAsync(data.AsBuffer());
            Status.ReportStatus("WriteLight_Period", result);
        }

        #endregion
//
        #region Service_Battery
        // Service Battery 

        public Battery_Data CurrBattery_Data { get; set; } = new Battery_Data();

        // Per-characteristics methods for Battery Battery_Data
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyBattery_DataAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Battery_Data", ServiceIndex.Battery_index, "Battery", CharacteristicIndex.Battery_Battery_Data_index, NotifyBattery_DataCallback, notifyType);
            return retval;
        }

        private void NotifyBattery_DataCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Battery_Battery_Data_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("I8|DEC|BatteryLevel|%");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrBattery_Data.TimestampMostRecent = args.Timestamp;
            CurrBattery_Data.BatteryLevel = vr.GetNextDouble();
            OnPropertyChanged(Battery_DataPropertyChangedName); // "Battery_Data"
        }
        /// <summary>
        /// Reads data from Battery Data and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Battery_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Battery_Data> ReadBattery_Data(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Battery_Battery_Data_index;
            await Ensure_Characteristic_Async(ServiceIndex.Battery_index, "Battery", index, "Battery Data");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Battery Data", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("I8|DEC|BatteryLevel|%");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrBattery_Data.BatteryLevel = vr.GetNextDouble();
            CurrBattery_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Battery_DataPropertyChangedName); // "Battery_Data"
            return CurrBattery_Data;
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
        // Per-characteristics methods for Common_Configuration Resolvable_Private_Address_Only
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyResolvable_Private_Address_OnlyAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Resolvable_Private_Address_Only", ServiceIndex.Common_Configuration_index, "Common Configuration", CharacteristicIndex.Common_Configuration_Resolvable_Private_Address_Only_index, NotifyResolvable_Private_Address_OnlyCallback, notifyType);
            return retval;
        }

        private void NotifyResolvable_Private_Address_OnlyCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Common_Configuration_Resolvable_Private_Address_Only_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX|ResolvablePrivateAddressFlag");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrCommon_Configuration_Data.TimestampMostRecent = args.Timestamp;
            CurrCommon_Configuration_Data.ResolvablePrivateAddressFlag = vr.GetNextDouble();
            OnPropertyChanged(Resolvable_Private_Address_OnlyPropertyChangedName); // "Resolvable_Private_Address_Only"
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
        /// <summary>
        /// Reads data from Central Address Resolution and triggers an OnPropertyChanged
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
            CurrCommon_Configuration_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Central_Address_ResolutionPropertyChangedName); // "Central_Address_Resolution"
            return CurrCommon_Configuration_Data;
        }
        /// <summary>
        /// Reads data from Resolvable Private Address Only and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Common_Configuration_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Common_Configuration_Data> ReadResolvable_Private_Address_Only(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Common_Configuration_Resolvable_Private_Address_Only_index;
            await Ensure_Characteristic_Async(ServiceIndex.Common_Configuration_index, "Common Configuration", index, "Resolvable Private Address Only");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Resolvable Private Address Only", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX|ResolvablePrivateAddressFlag");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrCommon_Configuration_Data.ResolvablePrivateAddressFlag = vr.GetNextDouble();
            CurrCommon_Configuration_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Resolvable_Private_Address_OnlyPropertyChangedName); // "Resolvable_Private_Address_Only"
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


// Long obsolete! [[zzMETHOD+LIST]]
    }
}