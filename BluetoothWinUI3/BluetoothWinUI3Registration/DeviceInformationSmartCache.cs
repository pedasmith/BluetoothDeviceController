using BluetoothProtocols;
using BluetoothWatcher.AdvertismentWatcher;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothWinUI3.BluetoothWinUI3Registration
{
    internal class DeviceInformationSmartCache
    {
        static DeviceInformationSmartCache Singleton { get; } = new DeviceInformationSmartCache();
        Dictionary<ulong, CachedDeviceInformation> _cache = new Dictionary<ulong, CachedDeviceInformation>();

        /// <summary>
        /// Easiest way to analyze a result. If the return value is non-null, we found something.
        /// </summary>
        public async static Task<SmartCacheAnalysisResult> AnalyzeAsync(WatcherData advertisement)
        {
            var cdi = await Singleton.EnsureServices(advertisement.Addr);
            if (cdi.CurrentState != CachedDeviceInformation.State.Ok) return null;
            var retval = await SmartCacheAnalysis.AnalyzeAsync(advertisement, cdi);
            return retval;
        }

        /// <summary>
        /// Reads the services and caches the information. If services can't be read, saves the failure information
        /// and in the future will short-circuit the read attempt.
        /// Return null or a CachedDeviceInformation where CurrentState is not Ok if the read failed.
        /// </summary>
        public async Task<CachedDeviceInformation> EnsureServices(ulong bluetoothAddress)
        {
            var addrstr = BluetoothAddress.AsString(bluetoothAddress);
            var cachedDeviceInformation = EnsureCacheExists(bluetoothAddress);
            // Quick return on failure
            if (cachedDeviceInformation.CurrentState != CachedDeviceInformation.State.Ok)
            {
                return cachedDeviceInformation;
            }

            var ble = await BluetoothLEDevice.FromBluetoothAddressAsync(bluetoothAddress);
            if (ble == null)
            {
                cachedDeviceInformation.CurrentState = CachedDeviceInformation.State.NoBluetoothLE;
                cachedDeviceInformation.ErrorMessage = $"Cache: Failed to create BluetoothLEDevice for {addrstr}";
                return cachedDeviceInformation;
            }
            cachedDeviceInformation.ble = ble;
            await cachedDeviceInformation.EnsureServicesAsync();
            return cachedDeviceInformation;
        }

        /// <summary>
        /// Can't ever fail; always returns a CachedDeviceInformation object. If the cache doesn't exist, creates it.
        /// </summary>
        private CachedDeviceInformation EnsureCacheExists(ulong bluetoothAddress)
        {
            if (!_cache.ContainsKey(bluetoothAddress))
            {
                _cache[bluetoothAddress] = new CachedDeviceInformation(bluetoothAddress);
            }
            return _cache[bluetoothAddress];
        }
    }
}
