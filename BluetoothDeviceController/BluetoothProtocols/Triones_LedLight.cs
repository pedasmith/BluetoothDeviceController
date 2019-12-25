using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

namespace BluetoothProtocols
{
    /// <summary>
    /// .
    /// This class was automatically generated 10/12/2019 8:08 PM
    /// </summary>

    public class Triones_LedLight : INotifyPropertyChanged
    {
        // Useful links for the device and protocol documentation
        // [[LINKS]]TODO: create LINKS

        public BluetoothLEDevice ble { get; set; } = null;
        public BluetoothStatusEvent Status = new BluetoothStatusEvent();

        // For the INotifyPropertyChanged values
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        Guid[] ServiceGuids = new Guid[] {
            Guid.Parse("0000fff0-0000-1000-8000-00805f9b34fb"),
            Guid.Parse("0000ffe0-0000-1000-8000-00805f9b34fb"),
            Guid.Parse("0000ffd5-0000-1000-8000-00805f9b34fb"),
            Guid.Parse("0000ffe5-0000-1000-8000-00805f9b34fb"),

        };
        String[] ServiceNames = new string[] {
            "Unknown-fff0",
            "Unknown-ffe0",
            "Light Control",
            "Light Control",

        };
        GattDeviceService[] Services = new GattDeviceService[] {
            null,
            null,
            null,
            null,

        };
        Guid[] CharacteristicGuids = new Guid[] {
            Guid.Parse("0000fff1-0000-1000-8000-00805f9b34fb"), // #0 is Unknown fff1
            Guid.Parse("0000fff2-0000-1000-8000-00805f9b34fb"), // #1 is Unknown fff2
            Guid.Parse("0000fff3-0000-1000-8000-00805f9b34fb"), // #2 is Unknown fff3
            Guid.Parse("0000fff4-0000-1000-8000-00805f9b34fb"), // #3 is Unknown fff4
            Guid.Parse("0000fff5-0000-1000-8000-00805f9b34fb"), // #4 is Unknown fff5
            Guid.Parse("0000ffe1-0000-1000-8000-00805f9b34fb"), // #0 is Unknown ffe1
            Guid.Parse("0000ffe2-0000-1000-8000-00805f9b34fb"), // #1 is Unknown ffe2
            Guid.Parse("0000ffe3-0000-1000-8000-00805f9b34fb"), // #2 is Unknown ffe3
            Guid.Parse("0000ffe4-0000-1000-8000-00805f9b34fb"), // #3 is Status Reply
            Guid.Parse("0000ffe5-0000-1000-8000-00805f9b34fb"), // #4 is Unknown ffe5
            Guid.Parse("0000ffd6-0000-1000-8000-00805f9b34fb"), // #0 is Unknown ffd6
            Guid.Parse("0000ffd7-0000-1000-8000-00805f9b34fb"), // #1 is Unknown ffd7
            Guid.Parse("0000ffd8-0000-1000-8000-00805f9b34fb"), // #2 is Unknown ffd8
            Guid.Parse("0000ffd9-0000-1000-8000-00805f9b34fb"), // #3 is Triones Command
            Guid.Parse("0000ffe6-0000-1000-8000-00805f9b34fb"), // #0 is Unknown ffe6
            Guid.Parse("0000ffe7-0000-1000-8000-00805f9b34fb"), // #1 is Unknown ffe7
            Guid.Parse("0000ffe8-0000-1000-8000-00805f9b34fb"), // #2 is Unknown ffe8
            Guid.Parse("0000ffe9-0000-1000-8000-00805f9b34fb"), // #3 is Triones Command

        };
        String[] CharacteristicNames = new string[] {
            "Unknown fff1", // #0 is 0000fff1-0000-1000-8000-00805f9b34fb
            "Unknown fff2", // #1 is 0000fff2-0000-1000-8000-00805f9b34fb
            "Unknown fff3", // #2 is 0000fff3-0000-1000-8000-00805f9b34fb
            "Unknown fff4", // #3 is 0000fff4-0000-1000-8000-00805f9b34fb
            "Unknown fff5", // #4 is 0000fff5-0000-1000-8000-00805f9b34fb
            "Unknown ffe1", // #0 is 0000ffe1-0000-1000-8000-00805f9b34fb
            "Unknown ffe2", // #1 is 0000ffe2-0000-1000-8000-00805f9b34fb
            "Unknown ffe3", // #2 is 0000ffe3-0000-1000-8000-00805f9b34fb
            "Status Reply", // #3 is 0000ffe4-0000-1000-8000-00805f9b34fb
            "Unknown ffe5", // #4 is 0000ffe5-0000-1000-8000-00805f9b34fb
            "Unknown ffd6", // #0 is 0000ffd6-0000-1000-8000-00805f9b34fb
            "Unknown ffd7", // #1 is 0000ffd7-0000-1000-8000-00805f9b34fb
            "Unknown ffd8", // #2 is 0000ffd8-0000-1000-8000-00805f9b34fb
            "Triones Command", // #3 is 0000ffd9-0000-1000-8000-00805f9b34fb
            "Unknown ffe6", // #0 is 0000ffe6-0000-1000-8000-00805f9b34fb
            "Unknown ffe7", // #1 is 0000ffe7-0000-1000-8000-00805f9b34fb
            "Unknown ffe8", // #2 is 0000ffe8-0000-1000-8000-00805f9b34fb
            "Triones Command", // #3 is 0000ffe9-0000-1000-8000-00805f9b34fb

        };
        GattCharacteristic[] Characteristics = new GattCharacteristic[] {
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,

        };
        List<HashSet<int>> MapServiceToCharacteristic = new List<HashSet<int>>() {
            new HashSet<int>(){ 0, 1, 2, 3, 4,  },
            new HashSet<int>(){ 5, 6, 7, 8, 9,  },
            new HashSet<int>(){ 10, 11, 12, 13,  },
            new HashSet<int>(){ 14, 15, 16, 17,  },

        };


        bool readCharacteristics = false;
        public async Task<bool> EnsureCharacteristicAsync(bool forceReread = false)
        {
            if (Characteristics.Length == 0) return false;

            GattCharacteristicsResult lastResult = null;
            if (forceReread)
            {
                readCharacteristics = false;
            }
            if (readCharacteristics == false)
            {
                for (int serviceIndex = 0; serviceIndex < MapServiceToCharacteristic.Count; serviceIndex++)
                {
                    var serviceStatus = await ble.GetGattServicesForUuidAsync(ServiceGuids[serviceIndex]);
                    if (serviceStatus.Status != GattCommunicationStatus.Success)
                    {
                        Status.ReportStatus($"Unable to get service {ServiceNames[serviceIndex]}", serviceStatus);
                        return false;
                    }
                    if (serviceStatus.Services.Count != 1)
                    {
                        Status.ReportStatus($"Unable to get valid service count ({serviceStatus.Services.Count}) for {ServiceNames[serviceIndex]}", serviceStatus);
                        continue; //return false;
                    }
                    var service = serviceStatus.Services[0];
                    var characteristicIndexSet = MapServiceToCharacteristic[serviceIndex];
                    foreach (var characteristicIndex in characteristicIndexSet)
                    {
                        var characteristicsStatus = await service.GetCharacteristicsForUuidAsync(CharacteristicGuids[characteristicIndex]);
                        if (characteristicsStatus.Status != GattCommunicationStatus.Success)
                        {
                            Status.ReportStatus($"unable to get characteristic for {CharacteristicNames[characteristicIndex]}", characteristicsStatus);
                            return false;
                        }
                        if (characteristicsStatus.Characteristics.Count == 0)
                        {
                            Status.ReportStatus($"unable to get any characteristics for {CharacteristicNames[characteristicIndex]}", characteristicsStatus);
                            Characteristics[characteristicIndex] = null;
                        }
                        else if (characteristicsStatus.Characteristics.Count != 1)
                        {
                            Status.ReportStatus($"unable to get correct characteristics count ({characteristicsStatus.Characteristics.Count}) for {CharacteristicNames[characteristicIndex]}", characteristicsStatus);
                            Characteristics[characteristicIndex] = null;
                        }
                        else
                        {
                            Characteristics[characteristicIndex] = characteristicsStatus.Characteristics[0];
                            lastResult = characteristicsStatus;
                        }
                        lastResult = characteristicsStatus;
                    }
                }
                // Do not call ReportStatus on OK -- the actual read/write/etc. call will
                // call ReportStatus for them. It's important that for any one actual call
                // (public method) that there's only one ReportStatus.
                //Status.ReportStatus("OK: Connected to device", lastResult);
                readCharacteristics = true;
            }
            return readCharacteristics;
        }

        /// <summary>
        /// Primary method used to for any bluetooth characteristic WriteValueAsync() calls.
        /// There's only one characteristic we use, so just use the one global.
        /// </summary>
        /// <param name="method" ></param>
        /// <param name="command" ></param>
        /// <returns></returns>
        private async Task WriteCommandAsync(int characteristicIndex, string method, byte[] command, GattWriteOption writeOption)
        {
            GattCommunicationStatus result = GattCommunicationStatus.Unreachable;
            try
            {
                result = await Characteristics[characteristicIndex].WriteValueAsync(command.AsBuffer(), writeOption);
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
        /// <summary>
        /// Generic read method; takes in a cache mode which defaults to uncached.
        /// Calls ReportStatus on either sucess or failure
        /// </summary>
        /// <param name="characteristicIndex">Index number of the characteristic</param>
        /// <param name="method" >Name of the actual method; is just used for logging</param>
        /// <param name="cacheMode" >Type of caching</param>
        /// <returns></returns>
        private async Task<IBuffer> ReadAsync(int characteristicIndex, string method, BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            GattReadResult readResult;
            IBuffer buffer = null;
            try
            {
                readResult = await Characteristics[characteristicIndex].ReadValueAsync(cacheMode);
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

        /// <summary>
        /// Delegate for all Notify events
        /// </summary>
        /// <param name="data"></param>
        public delegate void BluetoothDataEvent(BluetoothDeviceController.BleEditor.ValueParserResult data);




    }
}
