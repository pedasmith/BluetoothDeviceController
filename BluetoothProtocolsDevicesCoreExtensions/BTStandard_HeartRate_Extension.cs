using BluetoothProtocols;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using static BluetoothProtocols.BTStandard_HeartRate;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothProtocols
{
    internal static class BTStandard_HeartRate_Extension 
    {
        /// <summary>
        /// The Bluetooth standard here says that the message can contain either a one-byte (low range)
        /// or two-byte (high range) PulseRate depending on the value of the 'flags'. When a pulse rate
        /// fits into a byte (0..255), it should be the one-byte version; otherwise the two-byte version.
        /// 
        /// My BT automatic generation system doesn't have any way to combine these two together.
        /// </summary>
        public static double GetPulseRate(this Heart_Rate_Data value)
        {
            int flag = (int)(value.Flags);
            var pulseRate = ((flag & 0x01) != 0) ? value.PulseRateHighRange : value.PulseRateLowRange;
            return pulseRate;
        }
    }

    public class Heart_Rate_Data_Extension : Heart_Rate_Data
    {
        /// <summary>
        /// Decoded per specs at https://www.bluetooth.com/specifications/specs/html/?src=HRS_v1.0/out/en/index-en.html#UUID-5a90afed-6e38-b34e-199b-1eae1c2a4b95
        /// section 3.1.1.1
        /// </summary>
        [Flags]
        public enum FlagsDecoded { 
            /// <summary>
            /// 0=pulse uses the 1-byte format and is in the range 0..255
            /// 1=pulse uses 2-bytes format and is 256 or higher
            /// </summary>
            HeartRateValueFormatHighRange = 0x01, 
            /// <summary>
            /// 1=device supports detecting skin contact
            /// </summary>
            SensorContactSupported = 0x02, 
            /// <summary>
            /// 1=skin is detected
            /// </summary>
            SensorContactDetected = 0x04, 
            /// <summary>
            /// 1=the EnergyExpended field is filled in
            /// </summary>
            EnergyExpendedStatus = 0x08,
            /// <summary>
            /// 1=there are RRInterval status values present
            /// </summary>
            RRIntervalStatus = 0x10,
        }
        public FlagsDecoded CurrFlagsDecoded {  get { return (FlagsDecoded)Flags; } }
        public bool PulseRateIsHighRange {  get { return CurrFlagsDecoded.HasFlag(FlagsDecoded.HeartRateValueFormatHighRange); } }
        public double PulseRate {  get {  return PulseRateIsHighRange ? PulseRateHighRange : PulseRateLowRange; } }


        public int TestingExtraFields { get; set; }
    }
}
