using BluetoothDeviceController.Names;
using BluetoothProtocols;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

namespace BluetoothDeviceController.BluetoothProtocolsCustom
{
    public class Nordic_Uart
    {
        public const string SpecializationName = "NORDIC-UART";

        const string Transmit_Service_Name = "6e400001-b5a3-f393-e0a9-e50e24dcca9e";
        const string Tx_Characteristic_Name = "6e400002-b5a3-f393-e0a9-e50e24dcca9e";
        const string Rv_Characteristic_Name = "6e400003-b5a3-f393-e0a9-e50e24dcca9e";
        public Nordic_Uart(BluetoothLEDevice device)
        {
            Device = device;
        }
        public Nordic_Uart(DeviceInformation di)
        {
            DI = di;
        }
        public BluetoothLEDevice Device { get; set; }
        DeviceInformation DI = null;

        GattCharacteristic Tx = null;
        GattCharacteristic Rv = null;
        public bool IsUart {  get { return Tx != null && Rv != null; } }
        private bool DidInit = false;
        public async Task<bool> EnsureCharacteristicAsync()
        {
            if (DidInit) return IsUart;

            if (Device == null && DI != null)
            {
                // Get the device information
                Device = await BluetoothLEDevice.FromIdAsync(DI.Id);
            }

            DidInit = true;
            var TransmitGuid = Guid.Parse(Transmit_Service_Name);
            var TxGuid = Guid.Parse(Tx_Characteristic_Name);
            var RvGuid = Guid.Parse(Rv_Characteristic_Name);

            var services = await Device.GetGattServicesForUuidAsync(TransmitGuid);
            if (services.Status != GattCommunicationStatus.Success) return IsUart;
            foreach (var service in services.Services) // Should be exactly 1!
            {
                var chs = await service.GetCharacteristicsForUuidAsync(TxGuid);
                if (chs.Status != GattCommunicationStatus.Success) return IsUart;
                foreach (var characteristic in chs.Characteristics) // should be exactly 1!
                {
                    Tx = characteristic;
                }

                chs = await service.GetCharacteristicsForUuidAsync(RvGuid);
                if (chs.Status != GattCommunicationStatus.Success) return IsUart;
                foreach (var characteristic in chs.Characteristics) // should be exactly 1!
                {
                    Rv = characteristic;
                }
            }
            return IsUart;
        }

        public GattCharacteristic GetTx()
        {
            if (!IsUart) return null;
            return Tx;
        }

        public GattCharacteristic GetRv()
        {
            if (!IsUart) return null;
            return Rv;
        }

        // Mostly copied from CraftyRobot_Smartibot code!
        // For the INotifyPropertyChanged values
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public BluetoothStatusEvent Status = new BluetoothStatusEvent();

        /// <summary>
        /// Delegate for all Notify events
        /// </summary>
        /// <param name="data"></param>
        public delegate void BluetoothDataEvent(BluetoothDeviceController.BleEditor.ValueParserResult data);

        private string _Receive = "";
        private bool _Receive_set = false;
        public string Receive
        {
            get { return _Receive; }
            internal set { if (_Receive_set && value == _Receive) return; _Receive = value; _Receive_set = true; OnPropertyChanged(); }
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; ReceiveEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent ReceiveEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyReceive_ValueChanged_Set = false;
        public async Task<bool> NotifyReceiveAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Rv;
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyReceive_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyReceive_ValueChanged_Set = true;
                    ch.ValueChanged += (sender, args) =>
                    {
                        var datameaning = "STRING|ASCII^LONG|Data";
                        var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

                        Receive = parseResult.ValueList.GetValue("Data").AsString;

                        ReceiveEvent?.Invoke(parseResult);
                    };
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyReceive: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyReceive: set notification", result);

            return true;
        }


        /// <summary>
        /// Writes data for Transmit
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteTransmit(String Data)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteString(Data);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            for (int i = 0; i < command.Length; i += MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(Tx, "Transmit", subcommand, GattWriteOption.WriteWithoutResponse);
            }
        }
        private async Task WriteCommandAsync(GattCharacteristic characteristic, string method, byte[] command, GattWriteOption writeOption)
        {
            GattCommunicationStatus result = GattCommunicationStatus.Unreachable;
            try
            {
                result = await characteristic.WriteValueAsync(command.AsBuffer(), writeOption);
            }
            catch (Exception)
            {
                result = GattCommunicationStatus.Unreachable;
            }
            Status.ReportStatus(method, result);
            if (result != GattCommunicationStatus.Success)
            {
                // NOTE: should add a way to reset
            }
        }
    }
}
