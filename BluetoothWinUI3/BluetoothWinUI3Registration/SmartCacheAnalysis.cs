using BluetoothWatcher.AdvertismentWatcher;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothWinUI3.BluetoothWinUI3Registration
{
    /// <summary>
    /// Contains the conclusions of a smart cache analysis. For example, one of the many pulse oximeters
    /// might be categorized as a "ChoiceMMed". The device can then be treated as a ChoiceMMed device
    /// (in the SupportedDevices list) and will be sent to the BTCommon_HealthControl which can handle it.
    /// 
    /// Note that there's coordination needed: the different controls have to be updated to use the 
    /// SmartCacheAnalysis,
    /// </summary>
    internal static class SmartCacheAnalysis
    {
        // The 2026 version is newer than the older verison.
        private static Guid Nordic_TransmitService2026 = Guid.Parse("6e400001-b5a3-f393-e0a9-e50e24dcca9e"); // Copilot inserted "6E400001-B5A3-4933-9BAA-9B5D09812C2F"!
        private static Guid Nordic_ChoiceMMed_FFF0_Indicate = Guid.Parse("0000fff0-0000-1000-8000-00805f9b34fb");
        private static Guid Nordic_ChoiceMMed_FFF1_Read = Guid.Parse("0000fff1-0000-1000-8000-00805f9b34fb");
        private static Guid Nordic_ChoiceMMed_FFF2_Read = Guid.Parse("0000fff2-0000-1000-8000-00805f9b34fb");
        private static async Task<SmartCacheAnalysisResult> AnalyzeChoiceMMedPulseOximeterAsync (WatcherData advertisement, CachedDeviceInformation cdi)
        {
            bool status = await cdi.CacheCharacteristicsForServiceAsync(Nordic_TransmitService2026);
            if (status == false) return null;
            var name = await cdi.ReadStandardCharacteristicAsync(CachedDeviceInformation.StandardCharacteristic.ManufacturerNameString);
            if (name != "ChoiceMMed") return null;

            if (cdi.HasCharacteristic(Nordic_TransmitService2026, Nordic_ChoiceMMed_FFF0_Indicate) &&
                cdi.HasCharacteristic(Nordic_TransmitService2026, Nordic_ChoiceMMed_FFF1_Read) &&
                cdi.HasCharacteristic(Nordic_TransmitService2026, Nordic_ChoiceMMed_FFF2_Read))
            {
                var result = new SmartCacheAnalysisResult();
                result.AnalysisResult = SmartCacheAnalysisResult.DeviceType.ChoiceMMed;
                result.Analysis = $"Device ManufacturerName={name} (must be ChoiceMMed) and has the Nordic Transmit Service and FFF0 FFF1 FFF2 characteristics";
                return result;
            }
            return null;
        }

        /// <summary>
        /// Main analysis function. Will return null if there are no known devices. Mostly ignores the
        /// advertisement
        /// </summary>
        public static async Task<SmartCacheAnalysisResult> AnalyzeAsync(WatcherData advertisement, CachedDeviceInformation cdi)
        {
            var retval = await AnalyzeChoiceMMedPulseOximeterAsync(advertisement, cdi);
            return retval;
        }
    }
}
