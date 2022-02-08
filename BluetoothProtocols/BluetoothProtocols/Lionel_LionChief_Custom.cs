using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using enumUtilities;
namespace BluetoothProtocols
{
    public class SpeakMessageConverter : EnumValueConverter<Lionel_LionChief_Custom.SpeakMessage> { }
    public class SoundSourceConverter : EnumValueConverter<Lionel_LionChief_Custom.SoundSource> { }

    public class Lionel_LionChief_Custom : Lionel_LionChief
    {
        /*
         * Write Command	Notify	Notes
01	        00 81 02 00 01 04 0C 0D	
	        00 81 02 00 01 03 0C 0D	
		
	        00 81 02 00 01 03 0C 0D --> SENT AFTER BELL TURNS OFF	
		
	        00 81 02 03 02 03 0C 09	[7] The 09 is lights off / 0D=on 0F=bell
	        00 81 02 0C 02 03 0C 09	[3] is the speed
	        00 81 02 05 01 03 0C 0D	[4] is direction 01=fwd 02=rev
	        00 81 02 05 01 03 0C 0F	
		
	        [0] 00
	        [1] 81
	        [2] 02
	        [3] speed
	        [4] direction 01=fwd 02=reverse
	        [5] 03
	        [6] 0C
	        [7] 0000 1<light><bell>1 
03	        00 83 94 04 00 01	
04 00   	00 84 00 0E FE 02 00 02 01 06 00	
04 01   	00 84 01 0E FE
04 02   	00 84 02 02 00
04 03   	00 84 03 02 01
04 04   	00 84 04 06 00
	
	
	
1F	        00 9F 00 00 13 00	
20 		
20 00	    00 A0 02 2D 30 33 36 38 3B 3D 3F 41 43 45 47 48 4A 4B 4C	        - 0 3 6 8 ; = ? A C E G H J K L
20 01	    00 A0 02 59 5F 65 6A 6F 74 78 7D 81 85 88 8C 8F 92 94 96
20 02	    00 A0 02 00 00 EF BE 03 0E FE 02 00 02 01 06 00 00 80 00
20 03	    00 A0 02 B2 00 00 00 00 1E 12 00 00 00 00 00 00 00 00 00
20 04	    00 A0 02 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
20 05	    00 A0 02 19 04 00 20 05 00 00 00 00 00 00 00 00 00 00 00
20 06	    00 A0 02 13 00 A0 02 19 04 00 20 05 00 00 1F 0A 01 04 00
20 07	    00 A0 02 A0 01 00 00 00 00 00 00 03 00 01 36 00 00 3C E8
20 08	    00 A0 02 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
	
20 0B	    00 A0 02 85 83 BC 04 1B 00 17 00 00 00 00 00 00 00 00 00
20 0C	    00 A0 02 00 00 00 00 00 00 00 FF 00 00 0C 80 FE FF FF FF
20 0D	    00 A0 02 02 2A 80 00 00 00 00 00 00 00 00 00 00 00 00 00
20 0E	    00 A0 02 00 00 00 00 00 00 00 00 00 00 00 09 80 FD 09 0B
20 0F	    00 A0 02 04 00 10 94 17 08 80 17 0A 0C 00 BE 81 09 80 24
20 10	    00 A0 02 00 00 01 09 80 00 00 08 00 10 A9 18 08 80 00 00
20 11	    00 A0 02 0B 00 09 0C 00 11 80 00 00 10 02 00 00 20 00 00
	
	
	
22	        00 A2 01 00 00 00 00 0E 00 11 00 2A 00 2D 00 2F 00 32 00	
22 00 	    00 A2 00 00 00 00 00
22 01	    00 A2 01 00 00 00 00 0E 00 11 00 2A 00 2D 00 2F 00 32 00
	
	
	
23	        00 A3 00 00 00 00	Seems to report the speed
	
	        00 A3 FB 00 FB 00
46	        Train starts :-)	
61	        disconnects	
62	        00 A2 00 00 00 00 00	
63	        00 A3 00 00 00 00	Change speed and notify!
63 00	    00 A3 00 00 FF 00 -- stops
63 01	    00 A3 FE 00 00 00
63 02	    00 A3 FD 00 FE 00 -- reverse slowly
63 03	    00 A3 FC 00 FD 00 -- reverse more quickly
…	
63 FF	    00 A3 FF 00 00 00


         * 
         * 
         * 
         */




        private byte CalculateChecksum (byte command, byte[] param)
        {
            byte retval = (byte)((byte)0xFF - command);
            foreach (var b in param)
            {
                retval = (byte)(retval - b);
            }
            return retval;
        }

        /// <summary>
        /// The generated version of this include a "zero" byte which is always zero and a checksum which is calculated with the above method.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public Task WriteLionelCommand(byte cmd, byte[] param = null)
        {
            if (param == null) param = new byte[] { };
            return WriteLionelCommand(0, cmd, param, CalculateChecksum(cmd, param));
        }
        public Task WriteLionelCommand(byte cmd, byte paramByte)
        {
            var param = new byte[] { paramByte };
            return WriteLionelCommand(0, cmd, param, CalculateChecksum(cmd, param));
        }

        //
        //
        // All of the known Lionel commands, ordered by opcode
        //
        //

        public enum SoundSource {  Horn = 0x01, Bell=0x02, Speach=0x03, Engine=0x04 };

        public Task WriteLionelVolumePitch(SoundSource source, byte volume, sbyte pitch)
        {
            if (pitch < -2) pitch = -2;
            if (pitch > 2) pitch = 2;
            return WriteLionelCommand(0x44, new byte[] { (byte)source, volume, (byte)pitch });
        }
        public Task WriteLionelVolume(SoundSource source, sbyte volume) // see also the overall volume 4c
        {
            if (volume < 0) volume = 0;
            //if (volume > 7) volume = 7;
            return WriteLionelCommand(0x44, new byte[] { (byte)source, (byte)volume });
        }


        public Task WriteLionelSpeed(byte speed)
        {
            if (speed > 0x1f) speed = 0x1f;
            return WriteLionelCommand(0x45, speed);
        }
        public Task WriteLionelDirection(bool isForward) // true=forward false=reverse
        {
            return WriteLionelCommand(0x46, isForward ? (byte)0x01 : (byte)0x02);
        }
        public Task WriteLionelBell(bool turnOn)
        {
            return WriteLionelCommand(0x47, turnOn ? (byte)1 : (byte)0);
        }
        public Task WriteLionelHorn(bool turnOn)
        {
            return WriteLionelCommand(0x48, turnOn ? (byte)1 : (byte)0);
        }
        public Task WriteLionelDisconnect()
        {
            return WriteLionelCommand(0x4b);
        }

        public Task WriteLionelOverallVolume(byte volume) // see also command 44
        {
            if (volume < 0) volume = 0;
            if (volume > 7) volume = 7;
            return WriteLionelCommand(0x4c, volume);
        }

        // 1225 Speech codes (Polar Express)
        // 1 - this is the poloar express
        // 2 - all aboard
        // 3 - well you coming
        // 4 - tickets. #tickets please
        // 5 - The first gift of christmas
        // 6 - I am the king of the north pole
        public enum SpeakMessage
        {
            [enumUtilities.Display("Random message")]
            Random_Message = 0,
            [enumUtilities.Display("Operator: Hold position")]
            Operator_Hold_Location = 1,
            [enumUtilities.Display("Operator: Clear to proceed")]
            Operator_Clear_To_Proceed = 2,
            [enumUtilities.Display("Clear to depart on signal")]
            Clear_To_Depart_On_Signal = 3,
            [enumUtilities.Display("Clear to depart")]
            Clear_To_Depart = 4,
            [enumUtilities.Display("Train is moving")]
            Train_Is_Moving = 5,
            [enumUtilities.Display("Are we clear through diamond?")]
            Are_We_Clear_Through_Diamond = 6,
            [enumUtilities.Display("We're on time")]
            Dispatcher_Were_On_Time = 7,
            [enumUtilities.Display("Please hold at location")]
            Please_Hold_At_Location = 8,
        }

        public Task WriteLionelSpeak(SpeakMessage message)
        {
            return WriteLionelCommand(0x4d, (byte)message);
        }

        public Task WriteLionelLights(bool turnOn)
        {
            return WriteLionelCommand(0x51, turnOn ? (byte)1 : (byte)0);
        }
    }
}
