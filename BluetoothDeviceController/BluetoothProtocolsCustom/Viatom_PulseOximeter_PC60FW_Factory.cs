using BluetoothWatcher.BufferList;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Security.Authentication.Identity.Core;
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
but is just for the data files from the viatom

found a repo https://github.com/viatom-dev/iOSPC60F
with a non-useful doc file! https://github.com/viatom-dev/iOSPC60F/blob/main/PC-60F/PC-60F_en/FingerClipOximeter.doc
the doc file jst has a little info on using the library, not on the protocol.
they don't include the source for their actual library (licSpO2SDK.a)

found https://github.com/MackeyStingray/o2r 

it's a fork of https://github.com/shadowmoon-waltz/o2r 
but it's for different devices with a different protocol

#endif

#if EXAMPLE_DATA
Look for AA 55 0F . The next item is length and includes the command opcode. Commands will be concatenated together in the buffer.
The last byte is a checksum.
 AA 55 0F 07 02 6E 5F CC 37 22 ED		# command 2 is waveform data
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

AA 55 0F __ 21 02 00 00 00 __ # Unknown command, but I get it with each pulse. 

#endif

    /// <summary>
    /// Factory to create pulse oximeter data items. You keep giving it bt notifications, and then you can pull out data from GetNext
    /// </summary>
    public class Viatom_PulseOximeter_PC60FW_Factory
    {
        public Viatom_PulseOximeter_PC60FW_Factory()
        {
        }
        BufferList InputBuffers = new BufferList();

        /// <summary>
        /// Adds the next notification onto the current buffer. End result is a longer buffer. You should call GetNext afterwards.
        /// </summary>
        public void AddNotification(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            lock (this)
            {
                InputBuffers.Add(args.CharacteristicValue);
            }
        }
        string CurrWaveform = "!";
        bool WaveformPeak = false;
        List<byte> CurrWaveformData = new List<byte>();

        /// <summary>
        /// Gets whatever data is ready. Is often null when there's not enough data or it's the wrong type.
        /// </summary>
        /// <returns></returns>
        public Viatom_PulseOximeter_PC60FW GetNext()
        {
            //  AA 55 0F 08 01 61 40 00 51 00 C0 5B 	# New data sat=61=97 pr/hr=40=64 PI=51=81=8.1 PI is the Perfusion Index
            if (!InputBuffers.HasData) return null;

            bool keepGoing = true;
            while (keepGoing)
            {
                lock (this)
                {
                    bool foundAA = SkipToAA();
                    if (!foundAA) return null;
                    if (InputBuffers.Length < 11) return null;
                    if (InputBuffers.ReadByte() != 0xAA) continue; // Bad format; skip to the next command.
                    if (InputBuffers.ReadByte() != 0x55) continue; // Bad format; skip to the next command.
                    if (InputBuffers.ReadByte() != 0x0F) continue; // other command
                    var len = InputBuffers.ReadByte();
                    if (len > 20) continue; // length is very much out of range; bail and resync.
                    var cmd = InputBuffers.ReadByte();
                    bool commandIsKnown = true;
                    bool commandIsValid = true;
                    switch (cmd)
                    {
                        case 0x01: // Pulse
                            if (len != 0x08)
                            {
                                commandIsValid = false;
                            }
                            else
                            {
                                var retval = new Viatom_PulseOximeter_PC60FW();

                                retval.OxygenSaturationInPercent = InputBuffers.ReadByte();
                                retval.PulsePerMinute = InputBuffers.ReadByte();
                                byte junk = InputBuffers.ReadByte();
                                retval.PerfusionIndexInPercent = (double)InputBuffers.ReadByte() / 10.0;
                                junk = InputBuffers.ReadByte();
                                junk = InputBuffers.ReadByte();
                                junk = InputBuffers.ReadByte();
                                return retval;
                            }
                            break;
                        case 0x02: // Waveform
                            if (len != 0x07)
                            {
                                commandIsValid = false;
                            }
                            else
                            {
                                int nZero = 0;
                                bool gotZeroStretch = false;
                                bool gotPeak = false;
                                for (int i=0; i<5; i++)
                                {
                                    var wavedata = InputBuffers.ReadByte();
                                    if ((wavedata & 0x80) != 0) WaveformPeak = true;
                                    if ((wavedata & 0x80) != 0) gotPeak = true;
                                    if (WaveformPeak)
                                    {
                                        if (wavedata < 0x20)
                                        {
                                            nZero++;
                                            if (nZero > 2) gotZeroStretch = true;
                                        }
                                        else
                                        {
                                            nZero = 0;
                                        }
                                    }
                                    CurrWaveformData.Add((byte)(wavedata & 0x7F));

                                    var value = (wavedata & 0x7F) / 21; // use Musical Symbol one-line staff .. size-line staff
                                    if (value < 0) value = 0;
                                    if (value > 4) value = 4;
                                    char c1 = '\ud834';
                                    char c2 = (char)('\udd16' + value);
                                    CurrWaveform = CurrWaveform + c1 + c2;
                                }

                                if (gotZeroStretch)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Oximeter: pulse waveform <<{CurrWaveform}>>");
                                    WaveformPeak = false;
                                    CurrWaveform = "*";
                                }
                                if (gotPeak)
                                {
                                    var str = "";
                                    foreach (var b in CurrWaveformData)
                                    {
                                        str += $"{b:X2} ";
                                    }
                                    //System.Diagnostics.Debug.WriteLine($"Oximeter: peak data <<{str}>>");
                                    ;
                                    CurrWaveformData.Clear();
                                }
                                var checksum = InputBuffers.ReadByte();
                            }
                            break;
                        default:
                            commandIsKnown = false;
                            break;
                    }
                    // Read in the remaining bytes
                    if (!commandIsKnown || !commandIsValid)
                    {
                        // Read in the bytes
                        var bytes = "";
                        for (int i = 0; i < len - 2; i++)
                        {
                            var value = InputBuffers.ReadByte();
                            bytes += $"{value:X2} ";
                        }
                        var checksum = InputBuffers.ReadByte();

                        if (!commandIsValid)
                        {
                            System.Diagnostics.Debug.WriteLine($"Oximeter: invalid command {cmd:X2}");
                        }
                        else
                        {
                            bool supress = (cmd == 0x21 && bytes == "02 00 00 00 ");
                            if (!supress)
                            {
                                System.Diagnostics.Debug.WriteLine($"Oximeter: unknown command {cmd:X2} data {bytes}");
                            }
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Skips to the next AA. Often the buffer[NextIndex] is already there.
        /// </summary>
        /// <returns></returns>
        private bool SkipToAA()
        {
            while (InputBuffers.HasData)
            {
                if (InputBuffers.PeekByte() == 0xAA)
                {
                    // Found it!
                    return true;
                }
                var notAA = InputBuffers.ReadByte();
            }
            return false;
        }
    }
}
