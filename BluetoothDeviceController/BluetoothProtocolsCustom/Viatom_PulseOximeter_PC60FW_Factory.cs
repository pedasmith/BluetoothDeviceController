using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

namespace BluetoothDeviceController.BluetoothProtocolsCustom
{
#if MANY_MANY_THANKS
#endif
#if GITHUB_LINKS
2 repos found for pc-60fw

https://github.com/pythag/pc-60fw  Martin Whitaker
Says that the code is based on  
*    The BLE code is largely copied from the ESP32 BLE Arduino/BLE_client
*    example (author unknown).
Comment: has the most basic parser possible; looks for a specific header and then grabs two bytes exactly.

https://github.com/sza2/viatom_pc60fw 
Is a more full explanation of the data. Includes the basic data plus waveform
Waveform is function 2, length 7, with 5 bytes of wave data. wave data includes a peak detection which is the top bit, so the real data is just the bottom seven bits.
Search for viatom pulse oximeter finds one repo
https://github.com/rileyg98/ViatomDataReader
This one gives fully credit to other projects like OSCAR https://gitlab.com/pholy/OSCAR-code.
bu is just for the data files from the viatom

found a repo https://github.com/viatom-dev/iOSPC60F
with a non-useful doc file! https://github.com/viatom-dev/iOSPC60F/blob/main/PC-60F/PC-60F_en/FingerClipOximeter.doc
the doc file jst has a little info on using the library, not on the protocol.
they don't include the source for their actual library (licSpO2SDK.a)

found https://github.com/MackeyStingray/o2r 

it's a fork of https://github.com/shadowmoon-waltz/o2r 
but it's for different devices with a different protocol

#endif

#if EXAMPLE_DATA
Look for AA 55 0F . The next item is length. Commands will be concatenated together in the buffer.
 AA 55 0F 07 02 6E 5F CC 37 22 ED		
 AA 55 0F 07 02 0C 00 00 00 00 09
 AA 55 0F 07 02 00 00 00 00 00 28
 AA 55 0F 07 02 00 00 00 00 00 28
 AA 55 0F 07 02 00 00 00 00 00 28
 
 AA 55 0F 08 01 61 40 00 51 00 C0 5B 	# New data sat=61=97 pr/hr=40=64 PI=51=81=8.1 PI is the Perfusion Index
 AA 55 F0 03 03 03 F6
 
 AA 55 0F 07 02 00 00 00 00 00 28 
 
 AA 55 0F 06 21 02 00 00 0045 
 
 AA 55 0F 07 02 00 00 00 00 00 28
 AA 55 0F 07 02 00 00 00 00 00 28
 AA 55 0F 07 02 00 00 00 00 00 28
 AA 55 0F 07 02 00 00 00 00 00 28
 AA 55 0F 07 02 00 00 00 00 00 28
 AA 55 0F 07 02 00 00 00 00 00 28
 AA 55 0F 07 02 00 00 00 00 00 28
 AA 55 0F 07 02 00 00 00 00 00 28
 AA 55 0F 07 02 00 00 00 00 00 28
 
 AA 55 0F 08 01 62 3F 00 51 00 C0 00	# New data sat=62=98 hr=3F=63


#endif

    /// <summary>
    /// Factory to create pulse oximeter data items. You keep giving it bt notifications, and then you can pull out data from GetNext
    /// </summary>
    public class Viatom_PulseOximeter_PC60FW_Factory
    {
        public Viatom_PulseOximeter_PC60FW_Factory()
        {
        }
        byte[] CurrBuffer = null;
        int NextIndex = 0;
        int LengthRemaining {  get { if (CurrBuffer == null) return 0; return CurrBuffer.Length - NextIndex; } }

        /// <summary>
        /// Adds the next notification onto the current buffer. End result is a longer buffer. You should call GetNext afterwards.
        /// </summary>
        public void AddNotification(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            var b = args.CharacteristicValue;
            if (CurrBuffer == null || NextIndex >= CurrBuffer.Length)
            {
                CurrBuffer = b.ToArray();
                NextIndex = 0;
            }
            else
            {
                // Append old buffer plus new buffer to make one larger buffer.
                // Larger is relative; each 
                var newBuffer = new byte[CurrBuffer.Length + b.Length];
                CurrBuffer.CopyTo(newBuffer, 0);
                var barray = b.ToArray();
                barray.CopyTo(newBuffer, CurrBuffer.Length);
                CurrBuffer = newBuffer;
            }
        }

        /// <summary>
        /// Gets whatever data is ready. Is often null when there's not enough data or it's the wrong type.
        /// </summary>
        /// <returns></returns>
        public Viatom_PulseOximeter_PC60FW GetNext()
        {
            //  AA 55 0F 08 01 61 40 00 51 00 C0 5B 	# New data sat=61=97 pr/hr=40=64 PI=51=81=8.1 PI is the Perfusion Index
            if (CurrBuffer == null) return null;

            bool keepGoing = true;
            while (keepGoing)
            {
                bool foundAA = SkipToAA();
                if (!foundAA) return null;
                if (LengthRemaining < 11) return null;
                if (CurrBuffer[++NextIndex] != 0x55) continue; // Bad format; skip to the next command.
                if (CurrBuffer[++NextIndex] != 0x0F) continue; // other command
                var len = CurrBuffer[++NextIndex];
                if (len > 20) continue; // length is very much out of range; bail and resync.
                var cmd = CurrBuffer[++NextIndex];
                if (len == 0x08 && cmd == 0x01) // There are other command but we don't care about them.
                {
                    var retval = new Viatom_PulseOximeter_PC60FW();
                    retval.OxygenSaturationInPercent = CurrBuffer[NextIndex + 1];
                    retval.PulsePerMinute = CurrBuffer[NextIndex + 2];
                    retval.PerfusionIndexInPercent = (double)CurrBuffer[NextIndex + 4] / 10.0;


                    NextIndex += len;
                    if (NextIndex > CurrBuffer.Length) CurrBuffer = null;
                    return retval;
                }
                else
                {
                    NextIndex += len;
                    if (LengthRemaining < 0)
                    {
                        CurrBuffer = null;
                    }
                    if (CurrBuffer == null)
                    {
                        keepGoing = false;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Skips to the next AA. The NextIndex will be the index of the AA. Often the buffer[NextIndex] is already there.
        /// </summary>
        /// <returns></returns>
        private bool SkipToAA()
        {
            while (LengthRemaining > 0)
            {
                if (CurrBuffer[NextIndex] == 0xAA)
                {
                    // Found it!
                    return true;
                }
                NextIndex++;
            }
            return false;
        }
    }
}
