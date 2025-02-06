using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities
{
    /// <summary>
    /// I love the classic UWP DataReader, but it's not available to Core Framework apps.
    /// This is a handy replacement for simple parsers
    /// </summary>
    public class ByteOnlyDataReader
    {
        byte[] Data;
        public int NextIndex = 0;
        public ByteOnlyDataReader(byte[] data, int nextIndex = 0)
        {
            Data = data;
            NextIndex = nextIndex;
        }

        public int UnconsumedBufferLength
        {
            get { return Data.Length - NextIndex; }
        }

        public byte ReadByte()
        {
            if (UnconsumedBufferLength < 1)
            {
                throw new IndexOutOfRangeException();
            }
            byte b = Data[NextIndex++];
            return b;
        }

        public bool TryReadByte(out byte b)
        {
            if (UnconsumedBufferLength < 1)
            {
                b = 0;
                return false;
            }
            b = Data[NextIndex++];
            return true;
        }

        public static int Test()
        {
            int nerror = 0;
            nerror += TestOneRead(new ByteOnlyDataReader(new byte[] { }, 0), "Empty DR", false, 0);
            nerror += TestOneRead(new ByteOnlyDataReader(new byte[] { }, 99), "Empty DR", false, 0);

            var dr3 = new ByteOnlyDataReader(new byte[] { 10, 20, 30 }, 0);
            nerror += TestOneRead(dr3, "DR3", true, 10);
            nerror += TestOneRead(dr3, "DR3", true, 20);
            nerror += TestOneRead(dr3, "DR3", true, 30);
            nerror += TestOneRead(dr3, "DR3", false, 0);

            return nerror;
        }

        private static int TestOneRead(ByteOnlyDataReader dr, string test, bool expectedResult, byte expectedByte)
        {
            int nerror = 0;
            byte actualByte = 99;
            var actualResult = dr.TryReadByte(out actualByte);
            if (actualResult != expectedResult)
            {
                nerror++;
                Log($"ERROR: Test ByteOnlyDataReader: {test} try expected {expectedResult} actual {actualResult}");
            }
            if (actualByte != expectedByte)
            {
                nerror++;
                Log($"ERROR: Test ByteOnlyDataReader: {test} result expected {expectedByte} actual {actualByte}");
            }

            return nerror;
        }

        public static void Log(string message)
        {
            Console.WriteLine(message);
            System.Diagnostics.Debug.WriteLine(message);
        }

    }

}
