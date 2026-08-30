using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    public static class CrcCalculations
    {
        /// <summary>
        /// Given a byte array, calcuate the XOR CRC of each byte except for the last byte. Then
        /// set the last byte to the calculated value.
        /// </summary>
        public static void UpdateXorAtEnd(byte[] command)
        {
            byte value = 0;
            for (int i = 0; i < command.Length - 1; i++)
            {
                value ^= command[i];
            }
            command[command.Length - 1] = value;
        }

        /// <summary>
        /// Uses the iHealtChecksum calculation to set the checksum CRC byte at the end
        /// of an iHealth command
        /// </summary>
        public static void UpdateiHealthChecksumAtEnd(byte[] command)
        {
            // default values for start and end are OK
            byte checksum = iHealthChecksum.Calculate(command);
            command[command.Length - 1] = checksum;
        }


        public static void UpdateModbusCrc16AtEnd(byte[] command)
        {
            ushort crc = ModbusCrc.Calculate(command, 2); // ignore the last two bytes
            byte msb = (byte)((crc >> 8) & 0xFF);
            byte lsb = (byte)(crc & 0xFF);
            var idx = command.Length - 2;
            command[idx] = lsb; // Always the same endian
            command[idx + 1] = msb;
        }
    }

    public static class iHealthChecksum
    {
        /// <summary>
        /// Calculates an iHealth-compatible CRC checksum. According to the iHealth protocol, the CRC is calculated by adding all the 
        /// bytes together and taking the bottom byte of the result (alt: by adding everything together as bytes with wrap-around).
        /// The checksum is not done with the first two bytes.
        /// EndIndex of -1 means "go to the end of the array". Since the CRC is placed into the last byte, the default is to set endIndex to -2 which skips
        /// that byte. Some code will include the last byte, but that only works because the buffer is zerod out first.
        /// 
        /// This code just does the calculation; it does not place the CRC at the end.
        /// </summary>
        public static byte Calculate(byte[] message, int startIndex = 2, int endIndex = -2)
        {
            if (endIndex < 0) endIndex = message.Length + endIndex; // adding a negative number
            byte retval = 0;
            for (int i = startIndex; i <= endIndex; i++)
            {
                retval = (byte)(retval + message[i]);
            }
            return retval;
        }

        private static int TestOne(string messagestr, byte expected)
        {
            var message = HexUtilities.HexStringToByteArray(messagestr);
            int nerror = 0;
            var actual = Calculate(message);
            if (actual != expected)
            {
                nerror++;
                Log($"CRC iHealth: Error: [{message[0]:X2}] actual={actual:X2} expected={expected:X2}");
            }
            return nerror;
        }


        public static int Test()
        {
            int nerror = 0;
            nerror += TestOne("A0 03 A0 02 AC 4E", 0x4E);
            nerror += TestOne("b0 04 00 01 f0 d1 C2", 0xC2);
            nerror += TestOne("B0 03 A0 02 AC 50", 0x50);
            return nerror;
        }

        private static void Log(string str)
        {
            Console.WriteLine(str);
            System.Diagnostics.Debug.WriteLine(str);
        }
    }

    public static class ModbusCrc
    {
        // See: https://docklight.de/manual/generatingchecksums.htm
        // See: https://docklight.de/manual/dl_calcchecksum.htm
        // See: https://crccalc.com/?crc=123456789&method=&datatype=ascii&outtype=hex

        public static int Test()
        {
            int nerror = 0;
            nerror += TestOne(new byte[] { 0x01, 0x03, 0x00, 0x00, 0x00, 0x0A }, 0xCDc5, "AI generated sample"); // don't actually know the answer
            nerror += TestOne(new byte[] { 0x01, 0x04, 0x02, 0xFF, 0xFF, }, 0x80B8, "ST sample"); // https://community.st.com/t5/stm32-mcus-security/are-there-any-working-examples-using-the-crc-for-modbus-rtu-i/td-p/58913
            nerror += TestOne(new byte[] { 0xde, 0x00, 0x00, 0x00, 0x00 }, 0x138C, "Medium.com"); // https://medium.com/@wide4head/modbus-crc-in-c-how-to-dea119940256
            nerror += TestOne(new byte[] { 0x02, 0x07 }, 0x1241, "Modbus serial line sample p41");
            nerror += TestOne(new byte[] { 0xA0, 0x11, 0x04, 0x01 }, 0x21B1, "Daybetter reddit");
            return nerror;
        }
        public static int TestOne(byte[] message, ushort expected, string errorPrefix)
        {
            int nerror = 0;
            ushort actual = Calculate(message);
            if (actual != expected)
            {
                Console.WriteLine($"Error: ModbusCrc: {errorPrefix}: actual {actual:X4} but expected {expected:X4}");
                nerror += 1;
            }

            return nerror;
        }
        public static ushort Calculate(byte[] message, int nignore = 0)
        {
            ushort crc = 0xFFFF;
            for (int i = 0; i < message.Length - nignore; i++)
            {
                crc ^= (message[i]);
                for (int j = 0; j < 8; j++)
                {
                    // The Modbus-IDA.org "Modbus over serial line specification and implementation V1.01"
                    // says this about the LSB test:
                    // 'each 8-bit character is exclusive-ORed with the register contents. Then the result is shift in the
                    // direct of the least significant bit (LSB), with a zero filled into the most significant bit (MS) position. The LSB is extracted and
                    // examined. If the LSB was a 1, the register is then exclusive ORed with a preset. If the LSB was 0, no exclusive OR takes
                    // place.
                    // This is confusing, to say the least -- it says to shift first and then check the LSB. But they really mean to check 
                    // the LSB first. The Appendix B on page 40 which has a detailed flow chart; it calls the LSB the "carry over".
                    // AFAICT, this is a result of the code being described as if it was for a microcontroller, not for a high-level language.
                    // For example, the Motorola (NXP) 6805 chip says this: https://www.nxp.com/docs/en/supporting-information/MCUPDFNOTESTUT.pdf
                    // 
                    //  > Arithmetic shift right and logical shift right are two different operations. In both
                    //  > cases, though, the operand is shifted one bit to the right, with the LSB moving
                    //  > into the carry bit
                    // 
                    // Note how easy this makes handling the LSB: you just have to do the shift and then jump based on the carry bit!
                    //
                    // The 6502 is similar (http://www.6502.org/users/obelisk/6502/reference.html)
                    // ARM can also do this (https://developer.arm.com/documentation/ddi0406/c/Application-Level-Architecture/Instruction-Details/Alphabetical-list-of-instructions/LSR--immediate-?lang=en)
                    //    although it looks like setting the flags is optional (LSRS in assembler as opposed to LSR).
                    // Intel 8051 does this with a RRC A opcode (https://www.silabs.com/documents/public/presentations/8051_Instruction_Set.pdf) but you have to set the carry flag to zero
                    //     before doing the instruction (if it's 1, then a 1 gets put into the MSB)
                    ushort addXor = ((crc & 0x01) == 0x01) ? (ushort)0xA001 : (ushort)0x0000;
                    crc = (ushort)(crc >> (ushort)1);
                    crc ^= addXor;
                }
            }
            return crc;
        }
    }
}
