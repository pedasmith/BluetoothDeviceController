using System;
using System.Collections.Generic;
using System.Text;

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
    internal class SmartCacheAnalysisResult
    {
        public enum DeviceType
        {
            Unknown,
            ChoiceMMed,
        }
        public DeviceType AnalysisResult { get; set; } = DeviceType.Unknown;
        public string Analysis { get; set; } = "";
    }
}
