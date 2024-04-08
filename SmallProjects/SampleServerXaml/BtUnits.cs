using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Windows.Storage.Streams;

namespace SampleServerXaml
{
    public class BtUnits
    {
        /// <summary>
        /// Picked 8020 for the UserUnitPreferences
        /// </summary>
        public static Guid UserUnitPreferenceGuid = Guid.Parse("00008020-0000-1000-8000-00805f9b34fb");

        public enum Length { NoPreference=0,
            foot = 0x27A3,
            inch = 0x27A2,
            metre = 0x2701,
            mile = 0x27A4,
            nautical_mile = 0x2783,
            parsec = 0x27A1,
            yard = 0x27A0,
            angstrom = 0x278,
        }

        public enum Temperature
        {
            NoPreference = 0,
            celsius=  0x272F,
            fahrenheit = 0x27AC,
            kelvin =  0x2705,
        }

        public enum Time
        {
            NoPreference = 0,
            hour24 = 0x8024, // Not part of the BT standard
            hour12ampm = 0x8025, // Not part of the BT standard
        }

        public Time TimePref = Time.NoPreference;
        public Temperature TemperaturePref = Temperature.NoPreference;
        
        public int ZZZNUnitsSet {  get
            {
                int retval = 0;
                if (TimePref != Time.NoPreference) retval++;
                if (TemperaturePref != Temperature.NoPreference) retval++;
                return retval;
            } 
        }

        public int NItemsToWrite {  get { return 8; } } // up to 8 ushorts
        /// <summary>
        /// Will write the data to the datawriter as little-endian; will preserve old endianness
        /// </summary>
        /// <param name="dw"></param>
        public void Write(DataWriter dw)
        {
            var oldbo = dw.ByteOrder;
            dw.ByteOrder = ByteOrder.LittleEndian;
            int nwrite = NItemsToWrite;
            if (TimePref != Time.NoPreference) { nwrite--; dw.WriteUInt16((ushort)TimePref); }
            if (TemperaturePref != Temperature.NoPreference) { nwrite--; dw.WriteUInt16((ushort)TemperaturePref); }
            for (int i = 0; i < nwrite; i++)
            {
                dw.WriteUInt16((ushort)0);
            }

            dw.ByteOrder = oldbo;
        }

        public IBuffer WriteToBuffer()
        {
            byte[] buffer = new byte[NItemsToWrite * 2]; // everything is 2 bytes long
            var ms = new MemoryStream(buffer);
            var writer = new DataWriter(ms.AsOutputStream());
            writer.ByteOrder = ByteOrder.LittleEndian; // Not really needed
            Write(writer);
            return writer.DetachBuffer();
        }

        /*
    length (foot) 0x27A3
    length (inch) 0x27A2
    length (metre) 0x2701
    length (mile) 0x27A4
    length (nautical mile) 0x2783
    length (parsec) 0x27A1
    length (yard) 0x27A0
    length (ångström) 0x278
        */
    }
}
