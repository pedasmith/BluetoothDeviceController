using BluetoothConversions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Geolocation;
using Windows.Storage.Streams;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothWinUI3.BluetoothWinUI3Registration
{
    internal class CachedDeviceInformation
    {
        /// <summary>
        /// Every state that isn't Ok is an error state.
        /// </summary>
        public enum State { Ok, NoBluetoothLE };
        public State CurrentState { get; set; } = State.Ok;
        public string ErrorMessage { get; internal set; } = "";
        private void AddError(string message)
        {
            if (string.IsNullOrEmpty(ErrorMessage))
            {
                ErrorMessage = message;
            }
            else
            {
                ErrorMessage += Environment.NewLine + message;
            }
        }
        public CachedDeviceInformation(ulong bluetoothAddress)
        {
            BluetoothAddress = bluetoothAddress;
        }
        public ulong BluetoothAddress { get; internal set; }
        public string addrstr {  get {  return BluetoothProtocols.BluetoothAddress.AsString(BluetoothAddress); } }
        public BluetoothLEDevice ble { get; set; } = null;
        public IReadOnlyList<Windows.Devices.Bluetooth.GenericAttributeProfile.GattDeviceService> Services { get; set; } = null;
        public HashSet<Guid> FailedCharacteristicGuids { get; } = new HashSet<Guid>();
        public Dictionary<Guid, byte[]> CachedCharacteristicValues { get; } = new Dictionary<Guid, byte[]>();
        public Dictionary<Guid, string> CachedCharacteristicString { get; } = new Dictionary<Guid, string>();
        public Dictionary<Guid, IReadOnlyList<GattCharacteristic>> CachedServiceCharacteristics { get; } = new Dictionary<Guid, IReadOnlyList<GattCharacteristic>>();
        public enum StandardCharacteristic
        {
            ManufacturerNameString = 0x2A29,  // checked against BluetoothCharacteristics.cs
        }

        public enum StandardService
        {
            DeviceInformation = 0x180A, 
        }

        public async Task<bool> EnsureServicesAsync()
        {
            var serviceStatus = await ble.GetGattServicesAsync();
            if (serviceStatus == null)
            {
                CurrentState = CachedDeviceInformation.State.NoBluetoothLE;
                ErrorMessage = $"Cache: Failed to get GATT services for {addrstr}";
                return false;
            }
            if (serviceStatus.Status != Windows.Devices.Bluetooth.GenericAttributeProfile.GattCommunicationStatus.Success)
            {
                CurrentState = CachedDeviceInformation.State.NoBluetoothLE;
                ErrorMessage = $"Cache: Failed to get GATT services for {addrstr} with status {serviceStatus.Status}";
                return false;
            }
            if (serviceStatus.Services == null)
            {
                CurrentState = CachedDeviceInformation.State.NoBluetoothLE;
                ErrorMessage = $"Cache: No GATT services found for {addrstr}";
                return false;
            }
            Services = serviceStatus.Services;
            return true;
        }

        /// <summary>
        /// Reads the given characteristic using the cache system. Will return null if then characteristic
        /// can't be read. The service is assumed to be the standard service for the characteristic. For example, 
        /// ManufacturerNameString is in the DeviceInformation service.
        /// </summary>
        public async Task<string> ReadStandardCharacteristicAsync(StandardCharacteristic characteristic)
        {
            switch (characteristic)
            {
                case StandardCharacteristic.ManufacturerNameString:
                    {
                        var serviceGuid = BluetoothUuidHelper.FromShortId((ushort)StandardService.DeviceInformation);
                        var characteristicGuid = BluetoothUuidHelper.FromShortId((ushort)characteristic);
                        bool success = await ReadAndCacheCharacteristicAsync(serviceGuid, characteristicGuid);
                        if (success)
                        {
                            return CachedCharacteristicString[characteristicGuid];
                        }
                        else
                        {
                            return null;
                        }
                    }
            }
            return null;
        }

        public bool HasCharacteristic(Guid serviceGuid, Guid characteristicGuid)
        {
            if (CurrentState != State.Ok) return false;
            if (!CachedServiceCharacteristics.ContainsKey(serviceGuid)) return false;
            var retval = CachedServiceCharacteristics[serviceGuid].Any(c => c.Uuid == characteristicGuid);
            return retval;
        }

        /// <summary>
        /// Populates the CachedServiceCharacteristics dictionary for the given serviceGuid. Returns true if successful, false otherwise.
        /// Caches the results, of course and never retries.
        /// </summary>
        public async Task<bool> CacheCharacteristicsForServiceAsync(Guid serviceGuid)
        {
            // Quick return if the data is already cached.
            if (CachedServiceCharacteristics.ContainsKey(serviceGuid))
            {
                return true;
            }

            if (CurrentState != State.Ok || Services == null)
            {
                AddError($"Cache: can't read characteristic for address {BluetoothAddress} guid={serviceGuid} because status={CurrentState}");
                return false;
            }
            // Assumes services have already been read and cached. If not, this will fail.
            var service = Services.FirstOrDefault(s => s.Uuid == serviceGuid);
            if (service == null)
            {
                AddError($"Cache: can't read characteristic for address {BluetoothAddress} guid={serviceGuid} because service not found");
                return false;
            }
            var characteristicsStatus = await service.GetCharacteristicsAsync();
            if (characteristicsStatus.Status != Windows.Devices.Bluetooth.GenericAttributeProfile.GattCommunicationStatus.Success)
            {
                AddError($"Cache: can't discover characteristic for address {BluetoothAddress} guid={serviceGuid} because status={characteristicsStatus.Status}");
                return false;
            }
            if (characteristicsStatus.Characteristics == null || characteristicsStatus.Characteristics.Count == 0)
            {
                AddError($"Cache: can't read characteristic for address {BluetoothAddress} guid={serviceGuid} because no characteristics found");
                return false;
            }

            // Now I have the list of characteristics. 
            CachedServiceCharacteristics[serviceGuid] = characteristicsStatus.Characteristics;

            return true;

        }


        /// <summary>
        /// Returns true if the characteristic was read successfully, false otherwise.
        /// Uses caching so the characteristic is only read once. And if that read fails,
        /// it won't be retried. Assumes the services have already been read and cached. If not, this will fail.
        /// </summary>
        public async Task<bool> ReadAndCacheCharacteristicAsync(Guid serviceGuid, Guid characteristicGuid)
        {
            if (CurrentState != State.Ok || Services == null)
            {
                AddError($"Cache: can't read characteristic for address {BluetoothAddress} guid={characteristicGuid} because status={CurrentState}");
                return false;
            }
            // Assumes services have already been read and cached. If not, this will fail.
            var service = Services.FirstOrDefault(s => s.Uuid == serviceGuid);
            if (service == null) 
            {
                AddError($"Cache: can't read characteristic for address {BluetoothAddress} guid={characteristicGuid} because service not found");
                return false;
            }
            var characteristicsStatus = await service.GetCharacteristicsForUuidAsync(characteristicGuid);
            if (characteristicsStatus.Status != Windows.Devices.Bluetooth.GenericAttributeProfile.GattCommunicationStatus.Success)
            {
                // Never try to get this guid again!
                FailedCharacteristicGuids.Add(characteristicGuid);
                AddError($"Cache: can't read characteristic for address {BluetoothAddress} guid={characteristicGuid} because status={characteristicsStatus.Status}");
                return false;
            }
            if (characteristicsStatus.Characteristics == null || characteristicsStatus.Characteristics.Count == 0)
            {
                // Never try to get this guid again!
                FailedCharacteristicGuids.Add(characteristicGuid);
                AddError($"Cache: can't read characteristic for address {BluetoothAddress} guid={characteristicGuid} because no characteristics found");
                return false;
            }

            var readStatus = await characteristicsStatus.Characteristics[0].ReadValueAsync();
            if (readStatus == null || readStatus.Status != Windows.Devices.Bluetooth.GenericAttributeProfile.GattCommunicationStatus.Success)
            {
                // Never try to get this guid again!
                FailedCharacteristicGuids.Add(characteristicGuid);
                AddError($"Cache: can't read characteristic for address {BluetoothAddress} guid={characteristicGuid} because read failed with status={readStatus.Status}");
                return false;
            }

            byte[] data = readStatus.Value.ToByteArray();
            CachedCharacteristicValues[characteristicGuid] = data;

            var (str,status) = DataReaderReadStringRobust.ReadStringEntire(data);
            CachedCharacteristicString[characteristicGuid] = str;
            return true;
        }
    }
}
