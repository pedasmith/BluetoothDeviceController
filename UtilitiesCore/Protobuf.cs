using System;
using System.Collections.Generic;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace Utilities
{
    namespace Protobuf
    {
        public enum MessageType
        {
            VARINT = 0,
            I64 = 1,
            LEN = 2,
            SGROUP_DEPRECATED = 3,
            EGROUP_DEPRECATED = 4,
            I32 = 5,
        }

        public enum MessageSubType
        {
            VarInt_Int32=0, VarInt_Int64, VarInt_UInt32, VarInt_SInt32, VarInt_SInt64, VarInt_Bool, VarInt_Enum,

            I64_I64 = 100,

            Len_String = 200, Len_Bytes, Len_EmbeddedMessages, Len_PackedRepeatedFields,

            I32_Fixed32 = 500, I32_SFixed32, I32_Float,
        }

        public enum ParseResults
        {
            Success = 0,
            NotSet = 1,
            InsufficientBytes = 2,
            IntVarTooLong = 3,
        }

        public class Intvar
        {
            ParseResults Results = ParseResults.NotSet;
            UInt64 Value = 0;

            /// <summary>
            /// IntVar is a set of bytes; all the bytes have the MSB on except for the last (the set might
            /// be just one byte long without the MSB being on)
            /// </summary>
            /// <param name="dr"></param>
            ParseResults Parse(ByteOnlyDataReader dr)
            {
                Results = ParseResults.NotSet;
                Value = 0;

                if (dr.UnconsumedBufferLength < 1)
                {
                    Results = ParseResults.InsufficientBytes;
                    return Results;
                }
                int nbytesRead = 0;
                bool msbSet = false;
                bool error = false;

                do
                {
                    byte b;
                    bool gotByte = dr.TryReadByte(out b);
                    if (!gotByte)
                    {
                        Results = ParseResults.InsufficientBytes;
                    }
                    else
                    {
                        msbSet = (b & 0x80) != 0;
                        UInt64 lowbits = (UInt64)(b & 0x7F);
                        Value = Value | (lowbits << (nbytesRead * 7));
                        nbytesRead++;
                    }

                    if (msbSet && nbytesRead > 10) // it's not the last byte yet but we're out of runway
                    {
                        Results = ParseResults.IntVarTooLong;
                    }
                }
                while (msbSet && Results == ParseResults.NotSet);
                if (!error)
                {
                    Results = ParseResults.Success;
                }
                return Results;
            }

            public static int Test()
            {
                int nerror = 0;
                nerror += TestOne(new byte[] { 0x01 }, "Simple 01", ParseResults.Success, 1);
                nerror += TestOne(new byte[] { 0x96, 0x01 }, "9601=150", ParseResults.Success, 150);
                return nerror;
            }

            private static int TestOne(byte[] input, string test, ParseResults expectedResult, UInt64 expectedValue)
            {
                int nerror = 0;
                var dr = new ByteOnlyDataReader(input);
                var iv = new Intvar();
                var actualResult = iv.Parse(dr);
                var actualValue = iv.Value;

                if (actualResult != expectedResult)
                {
                    nerror++;
                    Log($"ERROR: Protobug: {test}: results expected={expectedResult} actual={actualResult}");
                }
                if (actualValue != expectedValue)
                {
                    nerror++;
                    Log($"ERROR: Protobug: {test}: value expected={expectedValue} actual={actualValue}");
                }
                return nerror;
            }

            private static void Log(string message)
            {
                Console.WriteLine(message);
                System.Diagnostics.Debug.WriteLine(message);    
            }

        }

        public class Message
        {
            byte[] b = new byte[] { 0xC2, 0x6A, 0x00, 0xD6, 0x00, 0x23, 0x2C }; // from ThermPro

        }

        /// <summary>
        /// Status: non-functional. Device this was needed for doesn't use protobuf :-)
        /// </summary>
        public class ProtobufParser
        {

        }
    }
}
