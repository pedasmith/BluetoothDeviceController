using System;
using System.Collections.Generic;
using System.Text;

namespace BluetoothProtocols
{
    public partial class PokitProMeter
    {
        public enum MMMode { Idle=0, VoltDC=1, VoltAC=2,  CurrentDC=3, CurrentAC=4, Resistance=5, Diode=6, Continuity=7, Temperature=8};

        public enum MMStatus { AutoRangeOn, AutoRangeOff, NoContinuity, Continuity, Error, DiodeOk, TemperatureOk, IncorrectProtocolError };


        public event EventHandler<MMData> OnMMVoltDC = null;
        public event EventHandler<MMData> OnMMVoltAC = null;
        public event EventHandler<MMData> OnMMCurrentDC = null;
        public event EventHandler<MMData> OnMMCurrentAC = null;
        public event EventHandler<MMData> OnMMResistance = null;
        public event EventHandler<MMData> OnMMDiode = null;
        public event EventHandler<MMData> OnMMContinuity = null;
        public event EventHandler<MMData> OnMMTemperature = null;
        public event EventHandler<MMData> OnMMOther = null;

        /// <summary>
        /// Call this from the *Page BleDevice_MM_DataEvent callback.
        /// </summary>
        public void HandleMMMessageCustom()
        {
            var mmdata = MMData.Create(this);
            switch (mmdata.Mode)
            {
                case MMMode.VoltDC: OnMMVoltDC?.Invoke(this, mmdata); break;
                case MMMode.VoltAC: OnMMVoltAC?.Invoke(this, mmdata); break;
                case MMMode.CurrentDC: OnMMCurrentDC?.Invoke(this, mmdata); break;
                case MMMode.CurrentAC: OnMMCurrentAC?.Invoke(this, mmdata); break;
                case MMMode.Resistance: OnMMResistance?.Invoke(this, mmdata); break;
                case MMMode.Diode: OnMMDiode?.Invoke(this, mmdata); break;
                case MMMode.Continuity: OnMMContinuity?.Invoke(this, mmdata); break;
                case MMMode.Temperature: OnMMTemperature?.Invoke(this, mmdata); break;
                default: OnMMOther?.Invoke(this, mmdata); break;
            }
        }

        public class MMData
        {
            public float Value;
            public float RawValue;
            public MMMode Mode;
            public float RangeMin;
            public float RangeMax;
            public float Range {  get { return RangeMax - RangeMin; } }
            public MMStatus Status;


            /// <summary>
            /// The source will have e.g. MMData_Range filled in after a call to ReadMM_Data
            /// </summary>
            /// <param name="source"></param>
            /// <returns></returns>
            public static MMData Create(PokitProMeter source)
            {
                var retval = new MMData();
                retval.Mode = (MMMode)source.MM_Data_OperationMode;
                retval.Status = CalculateStatus(retval.Mode, source.MM_Data_Status);
                retval.RawValue = (float)source.MM_Data_Data;
                SetRange(source, retval); // Will also set Value
                Log($"Pokit: got data {retval.ToString()}");
                return retval;
            }

            public override string ToString()
            {
                return $"MM: {Value} Mode={Mode} Range={RangeMin}::{RangeMax} ";
            }

            private static void Log(string text)
            {
                System.Diagnostics.Debug.WriteLine(text);
            }

            private static MMData SetRange(PokitProMeter source, MMData dest)
            {
                bool canSetValue = true;
                switch (dest.Mode)
                {
                    case MMMode.VoltAC:
                    case MMMode.VoltDC:
                        switch ((int)source.MM_Data_Range)
                        {
                            case 0: dest.RangeMin = 0; dest.RangeMax = 0.3f; break;
                            case 1: dest.RangeMin = 0.3f; dest.RangeMax = 2.0f; break;
                            case 2: dest.RangeMin = 2.0f; dest.RangeMax = 6.0f; break;
                            case 3: dest.RangeMin = 6.0f; dest.RangeMax = 12.0f; break;
                            case 4: dest.RangeMin = 12.0f; dest.RangeMax = 30.0f; break;
                            case 5: dest.RangeMin = 30.0f; dest.RangeMax = 60.0f; break;
                            default: dest.RangeMin = 1000; dest.RangeMax = 1010; break; // set to out-of-range
                        }
                        break;

                    case MMMode.Resistance:
                        switch ((int)source.MM_Data_Range)
                        {
                            case 0: dest.RangeMin = 0; dest.RangeMax = 160; break;
                            case 1: dest.RangeMin = 160; dest.RangeMax = 330; break;
                            case 2: dest.RangeMin = 330; dest.RangeMax = 890; break;
                            case 3: dest.RangeMin = 890; dest.RangeMax = 1_500; break;
                            case 4: dest.RangeMin = 1_500; dest.RangeMax = 10_000; break;
                            case 5: dest.RangeMin = 10_000; dest.RangeMax = 100_000; break;
                            case 6: dest.RangeMin = 100_000; dest.RangeMax = 470_000; break;
                            case 7: dest.RangeMin = 470_000; dest.RangeMax = 1_000_000; break;
                            default: dest.RangeMin = 0; dest.RangeMax = 1099; break;
                        }
                        break;
                    default:
                        dest.RangeMin = 1020; dest.RangeMax = 1030; break; // Set to out-of-range

                }
                if (canSetValue)
                {
                    dest.Value = dest.RawValue; // NOPE! Wrong entirely! I thought this only because there was a busted fuse!  (dest.RawValue * dest.Range) + dest.RangeMin;
                }
                else
                {
                    dest.Value = dest.RawValue; // Provide something?
                }
                return dest;
            }


            private static MMStatus CalculateStatus(MMMode mode, double mm_data_status)
            {
                MMStatus retval = MMStatus.IncorrectProtocolError;
                switch (mode)
                {
                    case MMMode.VoltDC:
                    case MMMode.VoltAC:
                    case MMMode.CurrentDC:
                    case MMMode.CurrentAC:
                    case MMMode.Resistance:
                        switch (mm_data_status)
                        {
                            case 0: retval = MMStatus.AutoRangeOff; break;
                            case 1: retval = MMStatus.AutoRangeOn; break;
                            case 255: retval = MMStatus.Error; break;
                            default: retval = MMStatus.IncorrectProtocolError; break;
                        }
                        break;
                    case MMMode.Continuity:
                        switch (mm_data_status)
                        {
                            case 0: retval = MMStatus.NoContinuity; break;
                            case 1: retval = MMStatus.Continuity; break;
                            case 255: retval = MMStatus.Error; break;
                            default: retval = MMStatus.IncorrectProtocolError; break;
                        }
                        break;
                    case MMMode.Diode:
                        switch (mm_data_status)
                        {
                            case 0: retval = MMStatus.DiodeOk; break;
                            case 255: retval = MMStatus.Error; break;
                            default: retval = MMStatus.IncorrectProtocolError; break;
                        }
                        break;
                    case MMMode.Temperature:
                        switch (mm_data_status)
                        {
                            case 0: retval = MMStatus.TemperatureOk; break;
                            case 255: retval = MMStatus.Error; break;
                            default: retval = MMStatus.IncorrectProtocolError; break;
                        }
                        break;
                    default:
                        retval = MMStatus.IncorrectProtocolError;
                        break;
                }

                return retval;
            }
        }
    }
}
