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
            public MMMode Mode;
            //TODO: implement these! public float RangeMin;
            //public float RangeMax;
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
                retval.Value = (float)source.MM_Data_Data;
                return retval;
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
