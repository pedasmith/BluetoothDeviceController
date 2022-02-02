using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using enumUtilities;
namespace BluetoothProtocols
{
    public class SpeakMessageConverter : EnumValueConverter<Lionel_LionChief_Custom.SpeakMessage> { }
    public class Lionel_LionChief_Custom : Lionel_LionChief
    {
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

        public Task WriteLionelBellPitch(sbyte pitch)
        {
            if (pitch < -2) pitch = -2;
            if (pitch > 2) pitch = 2;
            return WriteLionelCommand(0x44, new byte[] { 0x02, 0x0e, (byte)pitch });
        }


        public Task WriteLionelSpeed(byte speed)
        {
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

        public Task WriteLionelSteamVolume(byte volume)
        {
            if (volume < 0) volume = 0;
            if (volume > 7) volume = 7;
            return WriteLionelCommand(0x4c, volume);
        }
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
