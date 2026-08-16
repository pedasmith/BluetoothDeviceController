//From template: Protocol_Core_Body v2026-04-17 11:43
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel; // Needed for INotifyPropertyChanged
using System.Linq;
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
    /// .
    /// This class was automatically generated 2026-08-15::20:33
    /// </summary>

    public  class ChoiceMMed_PulseOximeter : INotifyPropertyChanged
    {
        // Useful links for the device and protocol documentation
        // Link: https://github.com/crcctcpr/choicemmed-md300cn358r

        public BluetoothLEDevice ble { get; set; } = null;
        public BluetoothStatusEvent Status = new BluetoothStatusEvent();

        // For the INotifyPropertyChanged values
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Name { get; } = "500E-B";
        public string Description { get; } = "";

        /* Service and Characteristics summary for the device 500E-B

        TransmitNordic service Guid=6e400001-b5a3-f393-e0a9-e50e24dcca9e
            TransmitNordic (DataGroup record)
                EnablePulseOximeterStream characteristic has TransmitData (Bytes-->string)  Guid=fff0
                OximeterDataStream characteristic has Opcode (Byte-->double) OxygenSaturationInPercent (UInt16-->double) PulseRate (UInt16-->double) RespirationRate (Byte-->double) Unknown10 (Byte-->double) Unknown11 (Byte-->double) Unknown12 (Byte-->double) Unknown13 (Byte-->double) Unknown14 (Byte-->double) PerfusionIndexInPercent (Byte-->double) PulseData (Byte-->double) RestOfData (Bytes-->string)  Guid=fff1
                Uknown_FFF2 characteristic has ReceiveDataRead (Bytes-->string)  Guid=fff2


        GAP service Guid=1800
            GAP_Data (DataGroup record)
                Device Name characteristic has Device_Name (String-->string)  Guid=2a00
                Appearance characteristic has Appearance (UInt16-->double)  Guid=2a01
                Peripheral Privacy Flag characteristic has Flag (Byte-->double)  Guid=2a02
                Reconnection Address characteristic has ReconnectAddress (Bytes-->string)  Guid=2a03
                Peripheral Preferred Connection Parameters characteristic has Interval_Min (UInt16-->double) Interval_Max (UInt16-->double) Latency (UInt16-->double) Timeout (UInt16-->double)  Guid=2a04


        Device Information service Guid=180a
            Device Information_Data (DataGroup record)
                Model Number String characteristic has ModelNumber (String-->string)  Guid=2a24
                Serial Number String characteristic has SerialNumber (String-->string)  Guid=2a25
                Software Revision String characteristic has SoftwareRevision (String-->string)  Guid=2a28
                Manufacturer Name String characteristic has ManufacturerName (String-->string)  Guid=2a29


        Battery service Guid=180f
            Battery_Data (DataGroup record)
                Battery Level characteristic has BatteryLevel (SByte-->double)  Guid=2a19


        ServiceControl0001 service Guid=00000001-0000-6465-6d6d-65636c6f6843
            ServiceControl0001 (DataGroup record)
                ReadC0002 characteristic has ReadC0002 (Bytes-->string)  Guid=00000002-0000-6465-6d6d-65636c6f6843
                NotifyC0003 characteristic has NotifyC0003 (Bytes-->string)  Guid=00000003-0000-6465-6d6d-65636c6f6843
                WriteC0004 characteristic has WriteC0004 (Bytes-->string)  Guid=00000004-0000-6465-6d6d-65636c6f6843
                ReadC0005 characteristic has ReadC0005 (Bytes-->string)  Guid=00000005-0000-6465-6d6d-65636c6f6843


        ServiceControlFF00 service Guid=ff00
            ServiceControlFF00 (DataGroup record)
                FF01 characteristic has FF01 (Bytes-->string)  Guid=ff01
                FF02 characteristic has FF02 (Bytes-->string)  Guid=ff02
                FF03 characteristic has FF03 (Bytes-->string)  Guid=ff03
        */

        public const string EnablePulseOximeterStreamPropertyChangedName = "EnablePulseOximeterStream";
        public const string OximeterDataStreamPropertyChangedName = "OximeterDataStream";
        public const string Uknown_FFF2PropertyChangedName = "Uknown_FFF2";
        public const string Device_NamePropertyChangedName = "Device_Name";
        public const string AppearancePropertyChangedName = "Appearance";
        public const string Peripheral_Privacy_FlagPropertyChangedName = "Peripheral_Privacy_Flag";
        public const string Reconnection_AddressPropertyChangedName = "Reconnection_Address";
        public const string Peripheral_Preferred_Connection_ParametersPropertyChangedName = "Peripheral_Preferred_Connection_Parameters";
        public const string Model_Number_StringPropertyChangedName = "Model_Number_String";
        public const string Serial_Number_StringPropertyChangedName = "Serial_Number_String";
        public const string Software_Revision_StringPropertyChangedName = "Software_Revision_String";
        public const string Manufacturer_Name_StringPropertyChangedName = "Manufacturer_Name_String";
        public const string Battery_LevelPropertyChangedName = "Battery_Level";
        public const string ReadC0002PropertyChangedName = "ReadC0002";
        public const string NotifyC0003PropertyChangedName = "NotifyC0003";
        public const string WriteC0004PropertyChangedName = "WriteC0004";
        public const string ReadC0005PropertyChangedName = "ReadC0005";
        public const string FF01PropertyChangedName = "FF01";
        public const string FF02PropertyChangedName = "FF02";
        public const string FF03PropertyChangedName = "FF03";



        //
        // All services / characteristics data types 
        //

        #region All_Data_Types
        /// <summary>
        /// Data from all of the characteristics in the TransmitNordic Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class TransmitNordic :BTCommonMetaData<TransmitNordic> //, IExportDataSource
        {
            private byte[] _TransmitData = null;
            /// <summary>
            /// TransmitData (BYTES ) from Service=TransmitNordic and Characteristic=EnablePulseOximeterStream
            ///</summary>
            public byte[] TransmitData 
            { 
                get { return _TransmitData; }
                set { if (value == _TransmitData) return; _TransmitData = value; OnPropertyChanged();}
            }

            private double _Opcode = 0;
            /// <summary>
            /// Opcode (U8 ) from Service=TransmitNordic and Characteristic=OximeterDataStream
            ///</summary>
            public double Opcode 
            { 
                get { return _Opcode; }
                set { if (value == _Opcode) return; _Opcode = value; OnPropertyChanged();}
            }
            private double _OxygenSaturationInPercent = 0;
            /// <summary>
            /// OxygenSaturationInPercent (U16 ) from Service=TransmitNordic and Characteristic=OximeterDataStream
            ///</summary>
            public double OxygenSaturationInPercent 
            { 
                get { return _OxygenSaturationInPercent; }
                set { if (value == _OxygenSaturationInPercent) return; _OxygenSaturationInPercent = value; OnPropertyChanged();}
            }
            private double _PulseRate = 0;
            /// <summary>
            /// PulseRate (U16 bpm) from Service=TransmitNordic and Characteristic=OximeterDataStream
            ///</summary>
            public double PulseRate 
            { 
                get { return _PulseRate; }
                set { if (value == _PulseRate) return; _PulseRate = value; OnPropertyChanged();}
            }
            private double _RespirationRate = 0;
            /// <summary>
            /// RespirationRate (U8 ) from Service=TransmitNordic and Characteristic=OximeterDataStream
            ///</summary>
            public double RespirationRate 
            { 
                get { return _RespirationRate; }
                set { if (value == _RespirationRate) return; _RespirationRate = value; OnPropertyChanged();}
            }
            private double _Unknown10 = 0;
            /// <summary>
            /// Unknown10 (U8 ) from Service=TransmitNordic and Characteristic=OximeterDataStream
            ///</summary>
            public double Unknown10 
            { 
                get { return _Unknown10; }
                set { if (value == _Unknown10) return; _Unknown10 = value; OnPropertyChanged();}
            }
            private double _Unknown11 = 0;
            /// <summary>
            /// Unknown11 (U8 ) from Service=TransmitNordic and Characteristic=OximeterDataStream
            ///</summary>
            public double Unknown11 
            { 
                get { return _Unknown11; }
                set { if (value == _Unknown11) return; _Unknown11 = value; OnPropertyChanged();}
            }
            private double _Unknown12 = 0;
            /// <summary>
            /// Unknown12 (U8 ) from Service=TransmitNordic and Characteristic=OximeterDataStream
            ///</summary>
            public double Unknown12 
            { 
                get { return _Unknown12; }
                set { if (value == _Unknown12) return; _Unknown12 = value; OnPropertyChanged();}
            }
            private double _Unknown13 = 0;
            /// <summary>
            /// Unknown13 (U8 ) from Service=TransmitNordic and Characteristic=OximeterDataStream
            ///</summary>
            public double Unknown13 
            { 
                get { return _Unknown13; }
                set { if (value == _Unknown13) return; _Unknown13 = value; OnPropertyChanged();}
            }
            private double _Unknown14 = 0;
            /// <summary>
            /// Unknown14 (U8 ) from Service=TransmitNordic and Characteristic=OximeterDataStream
            ///</summary>
            public double Unknown14 
            { 
                get { return _Unknown14; }
                set { if (value == _Unknown14) return; _Unknown14 = value; OnPropertyChanged();}
            }
            private double _PerfusionIndexInPercent = 0;
            /// <summary>
            /// PerfusionIndexInPercent (U8 ) from Service=TransmitNordic and Characteristic=OximeterDataStream
            ///</summary>
            public double PerfusionIndexInPercent 
            { 
                get { return _PerfusionIndexInPercent; }
                set { if (value == _PerfusionIndexInPercent) return; _PerfusionIndexInPercent = value; OnPropertyChanged();}
            }
            private double _PulseData = 0;
            /// <summary>
            /// PulseData (U8 ) from Service=TransmitNordic and Characteristic=OximeterDataStream
            ///</summary>
            public double PulseData 
            { 
                get { return _PulseData; }
                set { if (value == _PulseData) return; _PulseData = value; OnPropertyChanged();}
            }
            private byte[] _RestOfData = null;
            /// <summary>
            /// RestOfData (BYTES ) from Service=TransmitNordic and Characteristic=OximeterDataStream
            ///</summary>
            public byte[] RestOfData 
            { 
                get { return _RestOfData; }
                set { if (value == _RestOfData) return; _RestOfData = value; OnPropertyChanged();}
            }

            private byte[] _ReceiveDataRead = null;
            /// <summary>
            /// ReceiveDataRead (BYTES ) from Service=TransmitNordic and Characteristic=Uknown_FFF2
            ///</summary>
            public byte[] ReceiveDataRead 
            { 
                get { return _ReceiveDataRead; }
                set { if (value == _ReceiveDataRead) return; _ReceiveDataRead = value; OnPropertyChanged();}
            }
            public override TransmitNordic Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as TransmitNordic;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(TransmitNordic source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.TransmitData = source.TransmitData;
                dest.Opcode = source.Opcode;
                dest.OxygenSaturationInPercent = source.OxygenSaturationInPercent;
                dest.PulseRate = source.PulseRate;
                dest.RespirationRate = source.RespirationRate;
                dest.Unknown10 = source.Unknown10;
                dest.Unknown11 = source.Unknown11;
                dest.Unknown12 = source.Unknown12;
                dest.Unknown13 = source.Unknown13;
                dest.Unknown14 = source.Unknown14;
                dest.PerfusionIndexInPercent = source.PerfusionIndexInPercent;
                dest.PulseData = source.PulseData;
                dest.RestOfData = source.RestOfData;
                dest.ReceiveDataRead = source.ReceiveDataRead;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static TransmitNordic CopyToWithConvertAndCreate(TransmitNordic source, TransmitNordic dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.TransmitData = source.TransmitData;
                dest.Opcode = convert(source.Opcode, "");
                dest.OxygenSaturationInPercent = convert(source.OxygenSaturationInPercent, "");
                dest.PulseRate = convert(source.PulseRate, "bpm");
                dest.RespirationRate = convert(source.RespirationRate, "");
                dest.Unknown10 = convert(source.Unknown10, "");
                dest.Unknown11 = convert(source.Unknown11, "");
                dest.Unknown12 = convert(source.Unknown12, "");
                dest.Unknown13 = convert(source.Unknown13, "");
                dest.Unknown14 = convert(source.Unknown14, "");
                dest.PerfusionIndexInPercent = convert(source.PerfusionIndexInPercent, "");
                dest.PulseData = convert(source.PulseData, "");
                dest.RestOfData = source.RestOfData;
                dest.ReceiveDataRead = source.ReceiveDataRead;
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["TransmitData", "Opcode", "OxygenSaturationInPercent", "PulseRate", "RespirationRate", "Unknown10", "Unknown11", "Unknown12", "Unknown13", "Unknown14", "PerfusionIndexInPercent", "PulseData", "RestOfData", "ReceiveDataRead"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(TransmitData);
                exporter.CellSet(Opcode);
                exporter.CellSet(OxygenSaturationInPercent);
                exporter.CellSet(PulseRate);
                exporter.CellSet(RespirationRate);
                exporter.CellSet(Unknown10);
                exporter.CellSet(Unknown11);
                exporter.CellSet(Unknown12);
                exporter.CellSet(Unknown13);
                exporter.CellSet(Unknown14);
                exporter.CellSet(PerfusionIndexInPercent);
                exporter.CellSet(PulseData);
                exporter.CellSet(RestOfData);
                exporter.CellSet(ReceiveDataRead);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {TransmitData} {Opcode} {OxygenSaturationInPercent} {PulseRate} {RespirationRate} {Unknown10} {Unknown11} {Unknown12} {Unknown13} {Unknown14} {PerfusionIndexInPercent} {PulseData} {RestOfData} {ReceiveDataRead}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the GAP Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class GAP_Data :BTCommonMetaData<GAP_Data> //, IExportDataSource
        {
            private string _Device_Name = "";
            /// <summary>
            /// Device_Name (STRING ) from Service=GAP and Characteristic=Device Name
            ///</summary>
            public string Device_Name 
            { 
                get { return _Device_Name; }
                set { if (value == _Device_Name) return; _Device_Name = value; OnPropertyChanged();}
            }

            private double _Appearance = 0;
            /// <summary>
            /// Appearance (U16 ) from Service=GAP and Characteristic=Appearance
            ///</summary>
            public double Appearance 
            { 
                get { return _Appearance; }
                set { if (value == _Appearance) return; _Appearance = value; OnPropertyChanged();}
            }

            private double _Flag = 0;
            /// <summary>
            /// Flag (U8 ) from Service=GAP and Characteristic=Peripheral Privacy Flag
            ///</summary>
            public double Flag 
            { 
                get { return _Flag; }
                set { if (value == _Flag) return; _Flag = value; OnPropertyChanged();}
            }

            private byte[] _ReconnectAddress = null;
            /// <summary>
            /// ReconnectAddress (BYTES ) from Service=GAP and Characteristic=Reconnection Address
            ///</summary>
            public byte[] ReconnectAddress 
            { 
                get { return _ReconnectAddress; }
                set { if (value == _ReconnectAddress) return; _ReconnectAddress = value; OnPropertyChanged();}
            }

            private double _Interval_Min = 0;
            /// <summary>
            /// Interval_Min (U16 ms) from Service=GAP and Characteristic=Peripheral Preferred Connection Parameters
            ///</summary>
            public double Interval_Min 
            { 
                get { return _Interval_Min; }
                set { if (value == _Interval_Min) return; _Interval_Min = value; OnPropertyChanged();}
            }
            private double _Interval_Max = 0;
            /// <summary>
            /// Interval_Max (U16 ms) from Service=GAP and Characteristic=Peripheral Preferred Connection Parameters
            ///</summary>
            public double Interval_Max 
            { 
                get { return _Interval_Max; }
                set { if (value == _Interval_Max) return; _Interval_Max = value; OnPropertyChanged();}
            }
            private double _Latency = 0;
            /// <summary>
            /// Latency (U16 ms) from Service=GAP and Characteristic=Peripheral Preferred Connection Parameters
            ///</summary>
            public double Latency 
            { 
                get { return _Latency; }
                set { if (value == _Latency) return; _Latency = value; OnPropertyChanged();}
            }
            private double _Timeout = 0;
            /// <summary>
            /// Timeout (U16 ms) from Service=GAP and Characteristic=Peripheral Preferred Connection Parameters
            ///</summary>
            public double Timeout 
            { 
                get { return _Timeout; }
                set { if (value == _Timeout) return; _Timeout = value; OnPropertyChanged();}
            }
            public override GAP_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as GAP_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(GAP_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.Device_Name = source.Device_Name;
                dest.Appearance = source.Appearance;
                dest.Flag = source.Flag;
                dest.ReconnectAddress = source.ReconnectAddress;
                dest.Interval_Min = source.Interval_Min;
                dest.Interval_Max = source.Interval_Max;
                dest.Latency = source.Latency;
                dest.Timeout = source.Timeout;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static GAP_Data CopyToWithConvertAndCreate(GAP_Data source, GAP_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.Device_Name = source.Device_Name;
                dest.Appearance = convert(source.Appearance, "");
                dest.Flag = convert(source.Flag, "");
                dest.ReconnectAddress = source.ReconnectAddress;
                dest.Interval_Min = convert(source.Interval_Min, "ms");
                dest.Interval_Max = convert(source.Interval_Max, "ms");
                dest.Latency = convert(source.Latency, "ms");
                dest.Timeout = convert(source.Timeout, "ms");
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["Device_Name", "Appearance", "Flag", "ReconnectAddress", "Interval_Min", "Interval_Max", "Latency", "Timeout"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(Device_Name);
                exporter.CellSet(Appearance);
                exporter.CellSet(Flag);
                exporter.CellSet(ReconnectAddress);
                exporter.CellSet(Interval_Min);
                exporter.CellSet(Interval_Max);
                exporter.CellSet(Latency);
                exporter.CellSet(Timeout);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {Device_Name} {Appearance} {Flag} {ReconnectAddress} {Interval_Min} {Interval_Max} {Latency} {Timeout}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the Device Information Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class Device_Information_Data :BTCommonMetaData<Device_Information_Data> //, IExportDataSource
        {
            private string _ModelNumber = "";
            /// <summary>
            /// ModelNumber (STRING ) from Service=Device Information and Characteristic=Model Number String
            ///</summary>
            public string ModelNumber 
            { 
                get { return _ModelNumber; }
                set { if (value == _ModelNumber) return; _ModelNumber = value; OnPropertyChanged();}
            }

            private string _SerialNumber = "";
            /// <summary>
            /// SerialNumber (STRING ) from Service=Device Information and Characteristic=Serial Number String
            ///</summary>
            public string SerialNumber 
            { 
                get { return _SerialNumber; }
                set { if (value == _SerialNumber) return; _SerialNumber = value; OnPropertyChanged();}
            }

            private string _SoftwareRevision = "";
            /// <summary>
            /// SoftwareRevision (STRING ) from Service=Device Information and Characteristic=Software Revision String
            ///</summary>
            public string SoftwareRevision 
            { 
                get { return _SoftwareRevision; }
                set { if (value == _SoftwareRevision) return; _SoftwareRevision = value; OnPropertyChanged();}
            }

            private string _ManufacturerName = "";
            /// <summary>
            /// ManufacturerName (STRING ) from Service=Device Information and Characteristic=Manufacturer Name String
            ///</summary>
            public string ManufacturerName 
            { 
                get { return _ManufacturerName; }
                set { if (value == _ManufacturerName) return; _ManufacturerName = value; OnPropertyChanged();}
            }
            public override Device_Information_Data Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as Device_Information_Data;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(Device_Information_Data source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.ModelNumber = source.ModelNumber;
                dest.SerialNumber = source.SerialNumber;
                dest.SoftwareRevision = source.SoftwareRevision;
                dest.ManufacturerName = source.ManufacturerName;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static Device_Information_Data CopyToWithConvertAndCreate(Device_Information_Data source, Device_Information_Data dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.ModelNumber = source.ModelNumber;
                dest.SerialNumber = source.SerialNumber;
                dest.SoftwareRevision = source.SoftwareRevision;
                dest.ManufacturerName = source.ManufacturerName;
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["ModelNumber", "SerialNumber", "SoftwareRevision", "ManufacturerName"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(ModelNumber);
                exporter.CellSet(SerialNumber);
                exporter.CellSet(SoftwareRevision);
                exporter.CellSet(ManufacturerName);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {ModelNumber} {SerialNumber} {SoftwareRevision} {ManufacturerName}");
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
            /// BatteryLevel (I8 %) from Service=Battery and Characteristic=Battery Level
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
        /// Data from all of the characteristics in the ServiceControl0001 Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class ServiceControl0001 :BTCommonMetaData<ServiceControl0001> //, IExportDataSource
        {
            private byte[] _ReadC0002 = null;
            /// <summary>
            /// ReadC0002 (BYTES ) from Service=ServiceControl0001 and Characteristic=ReadC0002
            ///</summary>
            public byte[] ReadC0002 
            { 
                get { return _ReadC0002; }
                set { if (value == _ReadC0002) return; _ReadC0002 = value; OnPropertyChanged();}
            }

            private byte[] _NotifyC0003 = null;
            /// <summary>
            /// NotifyC0003 (BYTES ) from Service=ServiceControl0001 and Characteristic=NotifyC0003
            ///</summary>
            public byte[] NotifyC0003 
            { 
                get { return _NotifyC0003; }
                set { if (value == _NotifyC0003) return; _NotifyC0003 = value; OnPropertyChanged();}
            }

            private byte[] _WriteC0004 = null;
            /// <summary>
            /// WriteC0004 (BYTES ) from Service=ServiceControl0001 and Characteristic=WriteC0004
            ///</summary>
            public byte[] WriteC0004 
            { 
                get { return _WriteC0004; }
                set { if (value == _WriteC0004) return; _WriteC0004 = value; OnPropertyChanged();}
            }

            private byte[] _ReadC0005 = null;
            /// <summary>
            /// ReadC0005 (BYTES ) from Service=ServiceControl0001 and Characteristic=ReadC0005
            ///</summary>
            public byte[] ReadC0005 
            { 
                get { return _ReadC0005; }
                set { if (value == _ReadC0005) return; _ReadC0005 = value; OnPropertyChanged();}
            }
            public override ServiceControl0001 Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as ServiceControl0001;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(ServiceControl0001 source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.ReadC0002 = source.ReadC0002;
                dest.NotifyC0003 = source.NotifyC0003;
                dest.WriteC0004 = source.WriteC0004;
                dest.ReadC0005 = source.ReadC0005;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static ServiceControl0001 CopyToWithConvertAndCreate(ServiceControl0001 source, ServiceControl0001 dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.ReadC0002 = source.ReadC0002;
                dest.NotifyC0003 = source.NotifyC0003;
                dest.WriteC0004 = source.WriteC0004;
                dest.ReadC0005 = source.ReadC0005;
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["ReadC0002", "NotifyC0003", "WriteC0004", "ReadC0005"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(ReadC0002);
                exporter.CellSet(NotifyC0003);
                exporter.CellSet(WriteC0004);
                exporter.CellSet(ReadC0005);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {ReadC0002} {NotifyC0003} {WriteC0004} {ReadC0005}");
            }
        }
//
        /// <summary>
        /// Data from all of the characteristics in the ServiceControlFF00 Service. Dervices from
        /// BTCommonMetaData which includes DateTimeOffset, DateTimeOffsetDT, Name
        /// and implements INotifyPropertyChanged.
        /// Code generation template is the ServiceDataGroups template in CSharp_Core_BT_template.md
        /// Note the use of the Curiously Recurring Template Pattern (CRTP)
        /// </summary>
        public class ServiceControlFF00 :BTCommonMetaData<ServiceControlFF00> //, IExportDataSource
        {
            private byte[] _FF01 = null;
            /// <summary>
            /// FF01 (BYTES ) from Service=ServiceControlFF00 and Characteristic=FF01
            ///</summary>
            public byte[] FF01 
            { 
                get { return _FF01; }
                set { if (value == _FF01) return; _FF01 = value; OnPropertyChanged();}
            }

            private byte[] _FF02 = null;
            /// <summary>
            /// FF02 (BYTES ) from Service=ServiceControlFF00 and Characteristic=FF02
            ///</summary>
            public byte[] FF02 
            { 
                get { return _FF02; }
                set { if (value == _FF02) return; _FF02 = value; OnPropertyChanged();}
            }

            private byte[] _FF03 = null;
            /// <summary>
            /// FF03 (BYTES ) from Service=ServiceControlFF00 and Characteristic=FF03
            ///</summary>
            public byte[] FF03 
            { 
                get { return _FF03; }
                set { if (value == _FF03) return; _FF03 = value; OnPropertyChanged();}
            }
            public override ServiceControlFF00 Clone(string name = null)
            {
                var retval = this.MemberwiseClone() as ServiceControlFF00;
                if (name != null)
                {
                    retval.Name = name;
                }
                return retval;
            }

            /// <summary>
            /// Copies all of the source fields to the 'this' destination
            /// </summary>
            public override void CopyFrom(ServiceControlFF00 source)
            {
                var dest = this; // so that the code here and in CopyToWithConvertAndCreate are more similar
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = source.Name;
                dest.FF01 = source.FF01;
                dest.FF02 = source.FF02;
                dest.FF03 = source.FF03;
            }

            // Like CopyFrom, but convert the doubles as appropriate + sets name
            /// <summary>
            /// Similar to CopyFrom, but will create the destination if needed (using Clone), will convert the units,
            /// and will set the name to the given name if it's not null or empty.
            /// </summary>

            public static ServiceControlFF00 CopyToWithConvertAndCreate(ServiceControlFF00 source, ServiceControlFF00 dest, string name, BluetoothProtocols.UnitConverterDelegate.ConvertMethod convert)
            {
                if (dest == null)
                {
                    dest = source.Clone(name);
                }
                dest.TimestampMostRecent = source.TimestampMostRecent;
                dest.Name = String.IsNullOrEmpty(name) ? source.Name : name;
                dest.FF01 = source.FF01;
                dest.FF02 = source.FF02;
                dest.FF03 = source.FF03;
                return dest;
            }

            public override string[] ExportGetHeaders(IExportData _)
            {
                return ["FF01", "FF02", "FF03"];
            }

            public override void ExportRow(IExportData exporter)
            {
                // Note: the code in ExportDeviceData.cs in ExportData will do the RowStart
                // RowEnd and add in the timestamps
                exporter.CellSet(FF01);
                exporter.CellSet(FF02);
                exporter.CellSet(FF03);                
            }

            public override string ToString()
            {
                return String.Format($"{TimestampMostRecentDT.ToString("HH:mm.ss")} {FF01} {FF02} {FF03}");
            }
        }
//


        #endregion


        /// <summary>
        /// Enumeration of all services
        /// </summary>
        enum ServiceIndex
        {
            TransmitNordic_index = 0,
            GAP_index = 1,
            Device_Information_index = 2,
            Battery_index = 3,
            ServiceControl0001_index = 4,
            ServiceControlFF00_index = 5,
        }

        /// <summary>
        /// Enumeration of all characteristics in all of the services.
        /// </summary>
        enum CharacteristicIndex
        {
            TransmitNordic_EnablePulseOximeterStream_index = 0,     // GUID 0000fff0-0000-1000-8000-00805f9b34fb
            TransmitNordic_OximeterDataStream_index = 1,     // GUID 0000fff1-0000-1000-8000-00805f9b34fb
            TransmitNordic_Uknown_FFF2_index = 2,     // GUID 0000fff2-0000-1000-8000-00805f9b34fb
            GAP_Device_Name_index = 3,     // GUID 00002a00-0000-1000-8000-00805f9b34fb
            GAP_Appearance_index = 4,     // GUID 00002a01-0000-1000-8000-00805f9b34fb
            GAP_Peripheral_Privacy_Flag_index = 5,     // GUID 00002a02-0000-1000-8000-00805f9b34fb
            GAP_Reconnection_Address_index = 6,     // GUID 00002a03-0000-1000-8000-00805f9b34fb
            GAP_Peripheral_Preferred_Connection_Parameters_index = 7,     // GUID 00002a04-0000-1000-8000-00805f9b34fb
            Device_Information_Model_Number_String_index = 8,     // GUID 00002a24-0000-1000-8000-00805f9b34fb
            Device_Information_Serial_Number_String_index = 9,     // GUID 00002a25-0000-1000-8000-00805f9b34fb
            Device_Information_Software_Revision_String_index = 10,     // GUID 00002a28-0000-1000-8000-00805f9b34fb
            Device_Information_Manufacturer_Name_String_index = 11,     // GUID 00002a29-0000-1000-8000-00805f9b34fb
            Battery_Battery_Level_index = 12,     // GUID 00002a19-0000-1000-8000-00805f9b34fb
            ServiceControl0001_ReadC0002_index = 13,     // GUID 00000002-0000-6465-6d6d-65636c6f6843
            ServiceControl0001_NotifyC0003_index = 14,     // GUID 00000003-0000-6465-6d6d-65636c6f6843
            ServiceControl0001_WriteC0004_index = 15,     // GUID 00000004-0000-6465-6d6d-65636c6f6843
            ServiceControl0001_ReadC0005_index = 16,     // GUID 00000005-0000-6465-6d6d-65636c6f6843
            ServiceControlFF00_FF01_index = 17,     // GUID 0000ff01-0000-1000-8000-00805f9b34fb
            ServiceControlFF00_FF02_index = 18,     // GUID 0000ff02-0000-1000-8000-00805f9b34fb
            ServiceControlFF00_FF03_index = 19,     // GUID 0000ff03-0000-1000-8000-00805f9b34fb
        }

        /// <summary>
        /// List of the guids supported by the device. 
        /// </summary>
        List<Guid> Service_Guids = new List<Guid>()
        {
            Guid.Parse("6e400001-b5a3-f393-e0a9-e50e24dcca9e"), // #0 is TransmitNordic
            Guid.Parse("00001800-0000-1000-8000-00805f9b34fb"), // #1 is GAP
            Guid.Parse("0000180a-0000-1000-8000-00805f9b34fb"), // #2 is Device Information
            Guid.Parse("0000180f-0000-1000-8000-00805f9b34fb"), // #3 is Battery
            Guid.Parse("00000001-0000-6465-6d6d-65636c6f6843"), // #4 is ServiceControl0001
            Guid.Parse("0000ff00-0000-1000-8000-00805f9b34fb"), // #5 is ServiceControlFF00
        };

        /// <summary>
        /// Active list of services. Will be filled in as the services are connected. Starts off as null.
        /// </summary>
        List<GattDeviceService> Services = new List<GattDeviceService>() { null, null, null, null, null, null, };

        /// <summary>
        /// List of the Characteristic GUIDS for all of the characteristics for all of the services.
        /// Is indexed by the CharacteristicIndex enum. 
        /// </summary>
        List<Guid> Characteristic_Guids = new List<Guid>()
        {
            Guid.Parse("0000fff0-0000-1000-8000-00805f9b34fb"), // #0 is TransmitNordic EnablePulseOximeterStream
            Guid.Parse("0000fff1-0000-1000-8000-00805f9b34fb"), // #1 is TransmitNordic OximeterDataStream
            Guid.Parse("0000fff2-0000-1000-8000-00805f9b34fb"), // #2 is TransmitNordic Uknown_FFF2
            Guid.Parse("00002a00-0000-1000-8000-00805f9b34fb"), // #3 is GAP Device Name
            Guid.Parse("00002a01-0000-1000-8000-00805f9b34fb"), // #4 is GAP Appearance
            Guid.Parse("00002a02-0000-1000-8000-00805f9b34fb"), // #5 is GAP Peripheral Privacy Flag
            Guid.Parse("00002a03-0000-1000-8000-00805f9b34fb"), // #6 is GAP Reconnection Address
            Guid.Parse("00002a04-0000-1000-8000-00805f9b34fb"), // #7 is GAP Peripheral Preferred Connection Parameters
            Guid.Parse("00002a24-0000-1000-8000-00805f9b34fb"), // #8 is Device Information Model Number String
            Guid.Parse("00002a25-0000-1000-8000-00805f9b34fb"), // #9 is Device Information Serial Number String
            Guid.Parse("00002a28-0000-1000-8000-00805f9b34fb"), // #10 is Device Information Software Revision String
            Guid.Parse("00002a29-0000-1000-8000-00805f9b34fb"), // #11 is Device Information Manufacturer Name String
            Guid.Parse("00002a19-0000-1000-8000-00805f9b34fb"), // #12 is Battery Battery Level
            Guid.Parse("00000002-0000-6465-6d6d-65636c6f6843"), // #13 is ServiceControl0001 ReadC0002
            Guid.Parse("00000003-0000-6465-6d6d-65636c6f6843"), // #14 is ServiceControl0001 NotifyC0003
            Guid.Parse("00000004-0000-6465-6d6d-65636c6f6843"), // #15 is ServiceControl0001 WriteC0004
            Guid.Parse("00000005-0000-6465-6d6d-65636c6f6843"), // #16 is ServiceControl0001 ReadC0005
            Guid.Parse("0000ff01-0000-1000-8000-00805f9b34fb"), // #17 is ServiceControlFF00 FF01
            Guid.Parse("0000ff02-0000-1000-8000-00805f9b34fb"), // #18 is ServiceControlFF00 FF02
            Guid.Parse("0000ff03-0000-1000-8000-00805f9b34fb"), // #19 is ServiceControlFF00 FF03
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
                    // TODO: switch all protocols to this new pattern!
                    // What's going on: the code used to use ble.GetGattServiceForUuidAsync(). However,
                    // that code will throw when the guid isn't an advertised guid. 
                    // According to Copilot, it's known in the developer community, but isn't
                    // documented. So instead I have to use the more robust pattern of calling GetGattServicesAsync
                    // and then filtering.
                    var guid = Service_Guids[(int)serviceIndex];

                    var serviceStatus = await ble.GetGattServicesAsync();


                    //var serviceStatus = await ble.GetGattServicesForUuidAsync(Service_Guids[(int)serviceIndex]);
                    if (serviceStatus.Status != GattCommunicationStatus.Success)
                    {
                        Status.ReportStatus($"Unable to get service {serviceName}", serviceStatus);
                        return false;
                    }
                    var s = serviceStatus.Services.FirstOrDefault(s => s.Uuid == guid);
                    if (s == null) // TODO: serviceStatus.Services.Count != 1)
                    {
                        Status.ReportStatus($"Unable to get valid service count ({serviceStatus.Services.Count}) for {serviceName}", serviceStatus);
                        return false;
                    }
                    Services[(int)serviceIndex] = s;
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


        #region Service_TransmitNordic
        // Service TransmitNordic 

        public TransmitNordic CurrTransmitNordic { get; set; } = new TransmitNordic();

        // Per-characteristics methods for TransmitNordic EnablePulseOximeterStream
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyEnablePulseOximeterStreamAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("EnablePulseOximeterStream", ServiceIndex.TransmitNordic_index, "TransmitNordic", CharacteristicIndex.TransmitNordic_EnablePulseOximeterStream_index, NotifyEnablePulseOximeterStreamCallback, notifyType);
            return retval;
        }

        private void NotifyEnablePulseOximeterStreamCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.TransmitNordic_EnablePulseOximeterStream_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|TransmitData");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrTransmitNordic.TimestampMostRecent = args.Timestamp;
            CurrTransmitNordic.TransmitData = vr.GetNextByteArray();
            OnPropertyChanged(EnablePulseOximeterStreamPropertyChangedName); // "EnablePulseOximeterStream"
        }
        // Per-characteristics methods for TransmitNordic OximeterDataStream
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyOximeterDataStreamAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("OximeterDataStream", ServiceIndex.TransmitNordic_index, "TransmitNordic", CharacteristicIndex.TransmitNordic_OximeterDataStream_index, NotifyOximeterDataStreamCallback, notifyType);
            return retval;
        }

        private void NotifyOximeterDataStreamCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.TransmitNordic_OximeterDataStream_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|HEX|Opcode OSKIP^9^$Opcode_GN_62_EQ_NT U16|DEC|OxygenSaturationInPercent U16|DEC|PulseRate|bpm U8|DEC|RespirationRate U8|DEC|Unknown10 U8|DEC|Unknown11 U8|DEC|Unknown12 U8|DEC|Unknown13 U8|DEC|Unknown14 U8^10_/|FIXED|PerfusionIndexInPercent OSKIP^1^$Opcode_GN_01_EQ_NT U8|DEC|PulseData OOPT BYTES|HEX|RestOfData");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrTransmitNordic.TimestampMostRecent = args.Timestamp;
            CurrTransmitNordic.Opcode = vr.GetNextDouble();
            CurrTransmitNordic.OxygenSaturationInPercent = vr.GetNextDouble();
            CurrTransmitNordic.PulseRate = vr.GetNextDouble();
            CurrTransmitNordic.RespirationRate = vr.GetNextDouble();
            CurrTransmitNordic.Unknown10 = vr.GetNextDouble();
            CurrTransmitNordic.Unknown11 = vr.GetNextDouble();
            CurrTransmitNordic.Unknown12 = vr.GetNextDouble();
            CurrTransmitNordic.Unknown13 = vr.GetNextDouble();
            CurrTransmitNordic.Unknown14 = vr.GetNextDouble();
            CurrTransmitNordic.PerfusionIndexInPercent = vr.GetNextDouble();
            CurrTransmitNordic.PulseData = vr.GetNextDouble();
            CurrTransmitNordic.RestOfData = vr.GetNextByteArray();
            OnPropertyChanged(OximeterDataStreamPropertyChangedName); // "OximeterDataStream"
        }
        // Per-characteristics methods for TransmitNordic Uknown_FFF2
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyUknown_FFF2Async(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Uknown_FFF2", ServiceIndex.TransmitNordic_index, "TransmitNordic", CharacteristicIndex.TransmitNordic_Uknown_FFF2_index, NotifyUknown_FFF2Callback, notifyType);
            return retval;
        }

        private void NotifyUknown_FFF2Callback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.TransmitNordic_Uknown_FFF2_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|ReceiveDataRead");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrTransmitNordic.TimestampMostRecent = args.Timestamp;
            CurrTransmitNordic.ReceiveDataRead = vr.GetNextByteArray();
            OnPropertyChanged(Uknown_FFF2PropertyChangedName); // "Uknown_FFF2"
        }
        /// <summary>
        /// Reads data from EnablePulseOximeterStream and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>TransmitNordic of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<TransmitNordic> ReadEnablePulseOximeterStream(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.TransmitNordic_EnablePulseOximeterStream_index;
            await Ensure_Characteristic_Async(ServiceIndex.TransmitNordic_index, "TransmitNordic", index, "EnablePulseOximeterStream");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "EnablePulseOximeterStream", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|TransmitData");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrTransmitNordic.TransmitData = vr.GetNextByteArray();
            CurrTransmitNordic.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(EnablePulseOximeterStreamPropertyChangedName); // "EnablePulseOximeterStream"
            return CurrTransmitNordic;
        }
        /// <summary>
        /// Reads data from OximeterDataStream and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>TransmitNordic of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<TransmitNordic> ReadOximeterDataStream(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.TransmitNordic_OximeterDataStream_index;
            await Ensure_Characteristic_Async(ServiceIndex.TransmitNordic_index, "TransmitNordic", index, "OximeterDataStream");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "OximeterDataStream", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|HEX|Opcode OSKIP^9^$Opcode_GN_62_EQ_NT U16|DEC|OxygenSaturationInPercent U16|DEC|PulseRate|bpm U8|DEC|RespirationRate U8|DEC|Unknown10 U8|DEC|Unknown11 U8|DEC|Unknown12 U8|DEC|Unknown13 U8|DEC|Unknown14 U8^10_/|FIXED|PerfusionIndexInPercent OSKIP^1^$Opcode_GN_01_EQ_NT U8|DEC|PulseData OOPT BYTES|HEX|RestOfData");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrTransmitNordic.Opcode = vr.GetNextDouble();
            CurrTransmitNordic.OxygenSaturationInPercent = vr.GetNextDouble();
            CurrTransmitNordic.PulseRate = vr.GetNextDouble();
            CurrTransmitNordic.RespirationRate = vr.GetNextDouble();
            CurrTransmitNordic.Unknown10 = vr.GetNextDouble();
            CurrTransmitNordic.Unknown11 = vr.GetNextDouble();
            CurrTransmitNordic.Unknown12 = vr.GetNextDouble();
            CurrTransmitNordic.Unknown13 = vr.GetNextDouble();
            CurrTransmitNordic.Unknown14 = vr.GetNextDouble();
            CurrTransmitNordic.PerfusionIndexInPercent = vr.GetNextDouble();
            CurrTransmitNordic.PulseData = vr.GetNextDouble();
            CurrTransmitNordic.RestOfData = vr.GetNextByteArray();
            CurrTransmitNordic.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(OximeterDataStreamPropertyChangedName); // "OximeterDataStream"
            return CurrTransmitNordic;
        }
        /// <summary>
        /// Reads data from Uknown_FFF2 and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>TransmitNordic of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<TransmitNordic> ReadUknown_FFF2(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.TransmitNordic_Uknown_FFF2_index;
            await Ensure_Characteristic_Async(ServiceIndex.TransmitNordic_index, "TransmitNordic", index, "Uknown_FFF2");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Uknown_FFF2", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|ReceiveDataRead");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrTransmitNordic.ReceiveDataRead = vr.GetNextByteArray();
            CurrTransmitNordic.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Uknown_FFF2PropertyChangedName); // "Uknown_FFF2"
            return CurrTransmitNordic;
        }

        #endregion
//
        #region Service_GAP
        // Service GAP 

        public GAP_Data CurrGAP_Data { get; set; } = new GAP_Data();

        // Per-characteristics methods for GAP Device_Name
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyDevice_NameAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Device_Name", ServiceIndex.GAP_index, "GAP", CharacteristicIndex.GAP_Device_Name_index, NotifyDevice_NameCallback, notifyType);
            return retval;
        }

        private void NotifyDevice_NameCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.GAP_Device_Name_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|ASCII|Device_Name");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrGAP_Data.TimestampMostRecent = args.Timestamp;
            CurrGAP_Data.Device_Name = vr.GetNextString();
            OnPropertyChanged(Device_NamePropertyChangedName); // "Device_Name"
        }
        // Per-characteristics methods for GAP Appearance
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyAppearanceAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Appearance", ServiceIndex.GAP_index, "GAP", CharacteristicIndex.GAP_Appearance_index, NotifyAppearanceCallback, notifyType);
            return retval;
        }

        private void NotifyAppearanceCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.GAP_Appearance_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U16|Speciality^Appearance|Appearance");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrGAP_Data.TimestampMostRecent = args.Timestamp;
            CurrGAP_Data.Appearance = vr.GetNextDouble();
            OnPropertyChanged(AppearancePropertyChangedName); // "Appearance"
        }
        // Per-characteristics methods for GAP Peripheral_Privacy_Flag
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyPeripheral_Privacy_FlagAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Peripheral_Privacy_Flag", ServiceIndex.GAP_index, "GAP", CharacteristicIndex.GAP_Peripheral_Privacy_Flag_index, NotifyPeripheral_Privacy_FlagCallback, notifyType);
            return retval;
        }

        private void NotifyPeripheral_Privacy_FlagCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.GAP_Peripheral_Privacy_Flag_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U8|DEC|Flag");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrGAP_Data.TimestampMostRecent = args.Timestamp;
            CurrGAP_Data.Flag = vr.GetNextDouble();
            OnPropertyChanged(Peripheral_Privacy_FlagPropertyChangedName); // "Peripheral_Privacy_Flag"
        }
        // Per-characteristics methods for GAP Reconnection_Address
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyReconnection_AddressAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Reconnection_Address", ServiceIndex.GAP_index, "GAP", CharacteristicIndex.GAP_Reconnection_Address_index, NotifyReconnection_AddressCallback, notifyType);
            return retval;
        }

        private void NotifyReconnection_AddressCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.GAP_Reconnection_Address_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|ReconnectAddress");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrGAP_Data.TimestampMostRecent = args.Timestamp;
            CurrGAP_Data.ReconnectAddress = vr.GetNextByteArray();
            OnPropertyChanged(Reconnection_AddressPropertyChangedName); // "Reconnection_Address"
        }
        // Per-characteristics methods for GAP Peripheral_Preferred_Connection_Parameters
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyPeripheral_Preferred_Connection_ParametersAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Peripheral_Preferred_Connection_Parameters", ServiceIndex.GAP_index, "GAP", CharacteristicIndex.GAP_Peripheral_Preferred_Connection_Parameters_index, NotifyPeripheral_Preferred_Connection_ParametersCallback, notifyType);
            return retval;
        }

        private void NotifyPeripheral_Preferred_Connection_ParametersCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.GAP_Peripheral_Preferred_Connection_Parameters_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("U16^1.25_*|DEC|Interval_Min|ms U16^1.15_*|DEC|Interval_Max|ms U16|DEC|Latency|ms U16^10_*|DEC|Timeout|ms");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrGAP_Data.TimestampMostRecent = args.Timestamp;
            CurrGAP_Data.Interval_Min = vr.GetNextDouble();
            CurrGAP_Data.Interval_Max = vr.GetNextDouble();
            CurrGAP_Data.Latency = vr.GetNextDouble();
            CurrGAP_Data.Timeout = vr.GetNextDouble();
            OnPropertyChanged(Peripheral_Preferred_Connection_ParametersPropertyChangedName); // "Peripheral_Preferred_Connection_Parameters"
        }
        /// <summary>
        /// Reads data from Device Name and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>GAP_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<GAP_Data> ReadDevice_Name(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.GAP_Device_Name_index;
            await Ensure_Characteristic_Async(ServiceIndex.GAP_index, "GAP", index, "Device Name");
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
            CurrGAP_Data.Device_Name = vr.GetNextString();
            CurrGAP_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Device_NamePropertyChangedName); // "Device_Name"
            return CurrGAP_Data;
        }
        /// <summary>
        /// Reads data from Appearance and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>GAP_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<GAP_Data> ReadAppearance(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.GAP_Appearance_index;
            await Ensure_Characteristic_Async(ServiceIndex.GAP_index, "GAP", index, "Appearance");
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
            CurrGAP_Data.Appearance = vr.GetNextDouble();
            CurrGAP_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(AppearancePropertyChangedName); // "Appearance"
            return CurrGAP_Data;
        }
        /// <summary>
        /// Reads data from Peripheral Privacy Flag and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>GAP_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<GAP_Data> ReadPeripheral_Privacy_Flag(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.GAP_Peripheral_Privacy_Flag_index;
            await Ensure_Characteristic_Async(ServiceIndex.GAP_index, "GAP", index, "Peripheral Privacy Flag");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Peripheral Privacy Flag", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U8|DEC|Flag");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrGAP_Data.Flag = vr.GetNextDouble();
            CurrGAP_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Peripheral_Privacy_FlagPropertyChangedName); // "Peripheral_Privacy_Flag"
            return CurrGAP_Data;
        }
        /// <summary>
        /// Reads data from Reconnection Address and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>GAP_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<GAP_Data> ReadReconnection_Address(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.GAP_Reconnection_Address_index;
            await Ensure_Characteristic_Async(ServiceIndex.GAP_index, "GAP", index, "Reconnection Address");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Reconnection Address", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|ReconnectAddress");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrGAP_Data.ReconnectAddress = vr.GetNextByteArray();
            CurrGAP_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Reconnection_AddressPropertyChangedName); // "Reconnection_Address"
            return CurrGAP_Data;
        }
        /// <summary>
        /// Reads data from Peripheral Preferred Connection Parameters and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>GAP_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<GAP_Data> ReadPeripheral_Preferred_Connection_Parameters(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.GAP_Peripheral_Preferred_Connection_Parameters_index;
            await Ensure_Characteristic_Async(ServiceIndex.GAP_index, "GAP", index, "Peripheral Preferred Connection Parameters");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Peripheral Preferred Connection Parameters", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("U16^1.25_*|DEC|Interval_Min|ms U16^1.15_*|DEC|Interval_Max|ms U16|DEC|Latency|ms U16^10_*|DEC|Timeout|ms");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrGAP_Data.Interval_Min = vr.GetNextDouble();
            CurrGAP_Data.Interval_Max = vr.GetNextDouble();
            CurrGAP_Data.Latency = vr.GetNextDouble();
            CurrGAP_Data.Timeout = vr.GetNextDouble();
            CurrGAP_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Peripheral_Preferred_Connection_ParametersPropertyChangedName); // "Peripheral_Preferred_Connection_Parameters"
            return CurrGAP_Data;
        }

        #endregion
//
        #region Service_Device_Information
        // Service Device Information 

        public Device_Information_Data CurrDevice_Information_Data { get; set; } = new Device_Information_Data();

        // Per-characteristics methods for Device_Information Model_Number_String
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyModel_Number_StringAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Model_Number_String", ServiceIndex.Device_Information_index, "Device Information", CharacteristicIndex.Device_Information_Model_Number_String_index, NotifyModel_Number_StringCallback, notifyType);
            return retval;
        }

        private void NotifyModel_Number_StringCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Device_Information_Model_Number_String_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|ASCII|ModelNumber");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDevice_Information_Data.TimestampMostRecent = args.Timestamp;
            CurrDevice_Information_Data.ModelNumber = vr.GetNextString();
            OnPropertyChanged(Model_Number_StringPropertyChangedName); // "Model_Number_String"
        }
        // Per-characteristics methods for Device_Information Serial_Number_String
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifySerial_Number_StringAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Serial_Number_String", ServiceIndex.Device_Information_index, "Device Information", CharacteristicIndex.Device_Information_Serial_Number_String_index, NotifySerial_Number_StringCallback, notifyType);
            return retval;
        }

        private void NotifySerial_Number_StringCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Device_Information_Serial_Number_String_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|ASCII|SerialNumber");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDevice_Information_Data.TimestampMostRecent = args.Timestamp;
            CurrDevice_Information_Data.SerialNumber = vr.GetNextString();
            OnPropertyChanged(Serial_Number_StringPropertyChangedName); // "Serial_Number_String"
        }
        // Per-characteristics methods for Device_Information Software_Revision_String
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifySoftware_Revision_StringAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Software_Revision_String", ServiceIndex.Device_Information_index, "Device Information", CharacteristicIndex.Device_Information_Software_Revision_String_index, NotifySoftware_Revision_StringCallback, notifyType);
            return retval;
        }

        private void NotifySoftware_Revision_StringCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Device_Information_Software_Revision_String_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|ASCII|SoftwareRevision");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDevice_Information_Data.TimestampMostRecent = args.Timestamp;
            CurrDevice_Information_Data.SoftwareRevision = vr.GetNextString();
            OnPropertyChanged(Software_Revision_StringPropertyChangedName); // "Software_Revision_String"
        }
        // Per-characteristics methods for Device_Information Manufacturer_Name_String
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyManufacturer_Name_StringAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Manufacturer_Name_String", ServiceIndex.Device_Information_index, "Device Information", CharacteristicIndex.Device_Information_Manufacturer_Name_String_index, NotifyManufacturer_Name_StringCallback, notifyType);
            return retval;
        }

        private void NotifyManufacturer_Name_StringCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Device_Information_Manufacturer_Name_String_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("STRING|ASCII|ManufacturerName");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrDevice_Information_Data.TimestampMostRecent = args.Timestamp;
            CurrDevice_Information_Data.ManufacturerName = vr.GetNextString();
            OnPropertyChanged(Manufacturer_Name_StringPropertyChangedName); // "Manufacturer_Name_String"
        }
        /// <summary>
        /// Reads data from Model Number String and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Device_Information_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Device_Information_Data> ReadModel_Number_String(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Device_Information_Model_Number_String_index;
            await Ensure_Characteristic_Async(ServiceIndex.Device_Information_index, "Device Information", index, "Model Number String");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Model Number String", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("STRING|ASCII|ModelNumber");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDevice_Information_Data.ModelNumber = vr.GetNextString();
            CurrDevice_Information_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Model_Number_StringPropertyChangedName); // "Model_Number_String"
            return CurrDevice_Information_Data;
        }
        /// <summary>
        /// Reads data from Serial Number String and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Device_Information_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Device_Information_Data> ReadSerial_Number_String(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Device_Information_Serial_Number_String_index;
            await Ensure_Characteristic_Async(ServiceIndex.Device_Information_index, "Device Information", index, "Serial Number String");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Serial Number String", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("STRING|ASCII|SerialNumber");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDevice_Information_Data.SerialNumber = vr.GetNextString();
            CurrDevice_Information_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Serial_Number_StringPropertyChangedName); // "Serial_Number_String"
            return CurrDevice_Information_Data;
        }
        /// <summary>
        /// Reads data from Software Revision String and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Device_Information_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Device_Information_Data> ReadSoftware_Revision_String(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Device_Information_Software_Revision_String_index;
            await Ensure_Characteristic_Async(ServiceIndex.Device_Information_index, "Device Information", index, "Software Revision String");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Software Revision String", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("STRING|ASCII|SoftwareRevision");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDevice_Information_Data.SoftwareRevision = vr.GetNextString();
            CurrDevice_Information_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Software_Revision_StringPropertyChangedName); // "Software_Revision_String"
            return CurrDevice_Information_Data;
        }
        /// <summary>
        /// Reads data from Manufacturer Name String and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Device_Information_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Device_Information_Data> ReadManufacturer_Name_String(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Device_Information_Manufacturer_Name_String_index;
            await Ensure_Characteristic_Async(ServiceIndex.Device_Information_index, "Device Information", index, "Manufacturer Name String");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Manufacturer Name String", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("STRING|ASCII|ManufacturerName");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrDevice_Information_Data.ManufacturerName = vr.GetNextString();
            CurrDevice_Information_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Manufacturer_Name_StringPropertyChangedName); // "Manufacturer_Name_String"
            return CurrDevice_Information_Data;
        }

        #endregion
//
        #region Service_Battery
        // Service Battery 

        public Battery_Data CurrBattery_Data { get; set; } = new Battery_Data();

        // Per-characteristics methods for Battery Battery_Level
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyBattery_LevelAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("Battery_Level", ServiceIndex.Battery_index, "Battery", CharacteristicIndex.Battery_Battery_Level_index, NotifyBattery_LevelCallback, notifyType);
            return retval;
        }

        private void NotifyBattery_LevelCallback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.Battery_Battery_Level_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("I8|DEC|BatteryLevel|%");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrBattery_Data.TimestampMostRecent = args.Timestamp;
            CurrBattery_Data.BatteryLevel = vr.GetNextDouble();
            OnPropertyChanged(Battery_LevelPropertyChangedName); // "Battery_Level"
        }
        /// <summary>
        /// Reads data from Battery Level and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>Battery_Data of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<Battery_Data> ReadBattery_Level(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.Battery_Battery_Level_index;
            await Ensure_Characteristic_Async(ServiceIndex.Battery_index, "Battery", index, "Battery Level");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "Battery Level", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("I8|DEC|BatteryLevel|%");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrBattery_Data.BatteryLevel = vr.GetNextDouble();
            CurrBattery_Data.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(Battery_LevelPropertyChangedName); // "Battery_Level"
            return CurrBattery_Data;
        }

        #endregion
//
        #region Service_ServiceControl0001
        // Service ServiceControl0001 

        public ServiceControl0001 CurrServiceControl0001 { get; set; } = new ServiceControl0001();

        // Per-characteristics methods for ServiceControl0001 ReadC0002
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyReadC0002Async(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("ReadC0002", ServiceIndex.ServiceControl0001_index, "ServiceControl0001", CharacteristicIndex.ServiceControl0001_ReadC0002_index, NotifyReadC0002Callback, notifyType);
            return retval;
        }

        private void NotifyReadC0002Callback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.ServiceControl0001_ReadC0002_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|ReadC0002");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrServiceControl0001.TimestampMostRecent = args.Timestamp;
            CurrServiceControl0001.ReadC0002 = vr.GetNextByteArray();
            OnPropertyChanged(ReadC0002PropertyChangedName); // "ReadC0002"
        }
        // Per-characteristics methods for ServiceControl0001 NotifyC0003
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyNotifyC0003Async(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("NotifyC0003", ServiceIndex.ServiceControl0001_index, "ServiceControl0001", CharacteristicIndex.ServiceControl0001_NotifyC0003_index, NotifyNotifyC0003Callback, notifyType);
            return retval;
        }

        private void NotifyNotifyC0003Callback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.ServiceControl0001_NotifyC0003_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|NotifyC0003");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrServiceControl0001.TimestampMostRecent = args.Timestamp;
            CurrServiceControl0001.NotifyC0003 = vr.GetNextByteArray();
            OnPropertyChanged(NotifyC0003PropertyChangedName); // "NotifyC0003"
        }
        // Per-characteristics methods for ServiceControl0001 WriteC0004
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyWriteC0004Async(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("WriteC0004", ServiceIndex.ServiceControl0001_index, "ServiceControl0001", CharacteristicIndex.ServiceControl0001_WriteC0004_index, NotifyWriteC0004Callback, notifyType);
            return retval;
        }

        private void NotifyWriteC0004Callback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.ServiceControl0001_WriteC0004_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|WriteC0004");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrServiceControl0001.TimestampMostRecent = args.Timestamp;
            CurrServiceControl0001.WriteC0004 = vr.GetNextByteArray();
            OnPropertyChanged(WriteC0004PropertyChangedName); // "WriteC0004"
        }
        // Per-characteristics methods for ServiceControl0001 ReadC0005
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyReadC0005Async(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("ReadC0005", ServiceIndex.ServiceControl0001_index, "ServiceControl0001", CharacteristicIndex.ServiceControl0001_ReadC0005_index, NotifyReadC0005Callback, notifyType);
            return retval;
        }

        private void NotifyReadC0005Callback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.ServiceControl0001_ReadC0005_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|ReadC0005");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrServiceControl0001.TimestampMostRecent = args.Timestamp;
            CurrServiceControl0001.ReadC0005 = vr.GetNextByteArray();
            OnPropertyChanged(ReadC0005PropertyChangedName); // "ReadC0005"
        }
        /// <summary>
        /// Reads data from ReadC0002 and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>ServiceControl0001 of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<ServiceControl0001> ReadReadC0002(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.ServiceControl0001_ReadC0002_index;
            await Ensure_Characteristic_Async(ServiceIndex.ServiceControl0001_index, "ServiceControl0001", index, "ReadC0002");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "ReadC0002", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|ReadC0002");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrServiceControl0001.ReadC0002 = vr.GetNextByteArray();
            CurrServiceControl0001.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(ReadC0002PropertyChangedName); // "ReadC0002"
            return CurrServiceControl0001;
        }
        /// <summary>
        /// Reads data from NotifyC0003 and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>ServiceControl0001 of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<ServiceControl0001> ReadNotifyC0003(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.ServiceControl0001_NotifyC0003_index;
            await Ensure_Characteristic_Async(ServiceIndex.ServiceControl0001_index, "ServiceControl0001", index, "NotifyC0003");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "NotifyC0003", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|NotifyC0003");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrServiceControl0001.NotifyC0003 = vr.GetNextByteArray();
            CurrServiceControl0001.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(NotifyC0003PropertyChangedName); // "NotifyC0003"
            return CurrServiceControl0001;
        }
        /// <summary>
        /// Reads data from WriteC0004 and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>ServiceControl0001 of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<ServiceControl0001> ReadWriteC0004(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.ServiceControl0001_WriteC0004_index;
            await Ensure_Characteristic_Async(ServiceIndex.ServiceControl0001_index, "ServiceControl0001", index, "WriteC0004");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "WriteC0004", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|WriteC0004");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrServiceControl0001.WriteC0004 = vr.GetNextByteArray();
            CurrServiceControl0001.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(WriteC0004PropertyChangedName); // "WriteC0004"
            return CurrServiceControl0001;
        }
        /// <summary>
        /// Reads data from ReadC0005 and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>ServiceControl0001 of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<ServiceControl0001> ReadReadC0005(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.ServiceControl0001_ReadC0005_index;
            await Ensure_Characteristic_Async(ServiceIndex.ServiceControl0001_index, "ServiceControl0001", index, "ReadC0005");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "ReadC0005", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|ReadC0005");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrServiceControl0001.ReadC0005 = vr.GetNextByteArray();
            CurrServiceControl0001.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(ReadC0005PropertyChangedName); // "ReadC0005"
            return CurrServiceControl0001;
        }

        #endregion
//
        #region Service_ServiceControlFF00
        // Service ServiceControlFF00 

        public ServiceControlFF00 CurrServiceControlFF00 { get; set; } = new ServiceControlFF00();

        // Per-characteristics methods for ServiceControlFF00 FF01
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyFF01Async(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("FF01", ServiceIndex.ServiceControlFF00_index, "ServiceControlFF00", CharacteristicIndex.ServiceControlFF00_FF01_index, NotifyFF01Callback, notifyType);
            return retval;
        }

        private void NotifyFF01Callback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.ServiceControlFF00_FF01_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|FF01");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrServiceControlFF00.TimestampMostRecent = args.Timestamp;
            CurrServiceControlFF00.FF01 = vr.GetNextByteArray();
            OnPropertyChanged(FF01PropertyChangedName); // "FF01"
        }
        // Per-characteristics methods for ServiceControlFF00 FF02
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyFF02Async(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("FF02", ServiceIndex.ServiceControlFF00_index, "ServiceControlFF00", CharacteristicIndex.ServiceControlFF00_FF02_index, NotifyFF02Callback, notifyType);
            return retval;
        }

        private void NotifyFF02Callback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.ServiceControlFF00_FF02_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|FF02");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrServiceControlFF00.TimestampMostRecent = args.Timestamp;
            CurrServiceControlFF00.FF02 = vr.GetNextByteArray();
            OnPropertyChanged(FF02PropertyChangedName); // "FF02"
        }
        // Per-characteristics methods for ServiceControlFF00 FF03
        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>
        /// 
        public async Task<bool> NotifyFF03Async(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            var retval = await SetupNotifyAsync("FF03", ServiceIndex.ServiceControlFF00_index, "ServiceControlFF00", CharacteristicIndex.ServiceControlFF00_FF03_index, NotifyFF03Callback, notifyType);
            return retval;
        }

        private void NotifyFF03Callback(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var index = (int)CharacteristicIndex.ServiceControlFF00_FF03_index;
            if (ValueParsers[index] == null) ValueParsers[index] = new IotNumberFormats.ValueParser("BYTES|HEX|FF03");
            var vr = ValueParsers[index];

            vr.Initialize(args.CharacteristicValue.ToArray());
            CurrServiceControlFF00.TimestampMostRecent = args.Timestamp;
            CurrServiceControlFF00.FF03 = vr.GetNextByteArray();
            OnPropertyChanged(FF03PropertyChangedName); // "FF03"
        }
        /// <summary>
        /// Reads data from FF01 and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>ServiceControlFF00 of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<ServiceControlFF00> ReadFF01(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.ServiceControlFF00_FF01_index;
            await Ensure_Characteristic_Async(ServiceIndex.ServiceControlFF00_index, "ServiceControlFF00", index, "FF01");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "FF01", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|FF01");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrServiceControlFF00.FF01 = vr.GetNextByteArray();
            CurrServiceControlFF00.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(FF01PropertyChangedName); // "FF01"
            return CurrServiceControlFF00;
        }
        /// <summary>
        /// Reads data from FF02 and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>ServiceControlFF00 of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<ServiceControlFF00> ReadFF02(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.ServiceControlFF00_FF02_index;
            await Ensure_Characteristic_Async(ServiceIndex.ServiceControlFF00_index, "ServiceControlFF00", index, "FF02");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "FF02", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|FF02");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrServiceControlFF00.FF02 = vr.GetNextByteArray();
            CurrServiceControlFF00.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(FF02PropertyChangedName); // "FF02"
            return CurrServiceControlFF00;
        }
        /// <summary>
        /// Reads data from FF03 and triggers an OnPropertyChanged
        /// </summary>
        /// <param name="cacheMode">Caching mode. Often for data we want uncached data.</param>
        /// <returns>ServiceControlFF00 of results; each result is named based on the name in the characteristic string. E.G. U8|Hex|Red will be named Red</returns>
        public async Task<ServiceControlFF00> ReadFF03(BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            var index = CharacteristicIndex.ServiceControlFF00_FF03_index;
            await Ensure_Characteristic_Async(ServiceIndex.ServiceControlFF00_index, "ServiceControlFF00", index, "FF03");
            var ch = Characteristics[(int)index];
            if (ch == null)
            {
                return null;
            }

            IBuffer result = await ReadAsync(ch, "FF03", cacheMode);
            if (result == null) return null;

            if (ValueParsers[(int)index] == null) ValueParsers[(int)index] = new IotNumberFormats.ValueParser("BYTES|HEX|FF03");
            var vr = ValueParsers[(int)index];

            vr.Initialize(result.ToArray());
            CurrServiceControlFF00.FF03 = vr.GetNextByteArray();
            CurrServiceControlFF00.TimestampMostRecent = DateTimeOffset.Now;
            OnPropertyChanged(FF03PropertyChangedName); // "FF03"
            return CurrServiceControlFF00;
        }

        #endregion
//


// Long obsolete! [[zzMETHOD+LIST]]
    }
}