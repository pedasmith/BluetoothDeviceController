

byte[] data1 = { 0x01, 0x01, 0x0E, 0x16, 0x8D, 0x59 };
byte[] data = { 0x01, 0x01, 0x0D, 0xBA, 0xE8, 0x69 };
string ZZct01hex = "AA 0D A1 00 59 02 29 64 14 15 09 17 01 36 16 BB";
string ct01hex = "00 00 00 00 66 02 00 00 00 00";
byte[] ct01 = FindBitPattern.ConvertFromHex(ct01hex);

//var nfound = FindBitPattern.FindDecimalPattern(data, 233.0, 250.0);
//FindBitPattern.ExplorePattern(data, 233, 250);

Console.WriteLine($"Pattern: {ct01hex}");
int nfound = 0;
nfound = FindBitPattern.FindDecimalPattern(ct01, 19.0, 25.0);
Console.WriteLine($"Result: nfound={nfound}");

FindBitPattern.Test();

//var nfound = FindBitPattern.FindDecimalPattern(data, 233, 250);
//FindBitPattern.ExplorePattern(data, 233, 250);
//Console.WriteLine($"Result: nfound={nfound}");

var nerror = 0;
nerror += Utilities.Protobuf.Intvar.Test();
nerror += Utilities.ByteOnlyDataReader.Test();

Console.WriteLine($"Unit tests: nerror={nerror}");

class FindBitPattern
{
    public static int ExplorePattern(byte[] data, int patternMin, int patternMax)
    {
        int nfound = 0;
        for (int nbytes = 4; nbytes <= 4; nbytes++)
        {
            var lastIndex = data.Length - nbytes;
            for (int startIndex = 0; startIndex <= lastIndex; startIndex++)
            {
                var calc = ExtractBytesAsIntBigEndian(data, nbytes, startIndex);
                Console.WriteLine($"Value: BE={calc}");
                calc = ExtractBytesAsIntLittleEndian(data, nbytes, startIndex);
                Console.WriteLine($"Value: LE={calc}");
            }
        }
        return nfound;
    }

    public static int FindDecimalPattern(byte[] data, double patternMin, double patternMax)
    {
        double[] divide = new double[] { 10.0, 16.0, 100.0, 256.0, 1000.0, 10_000.0, 100_000.0 };

        int nfound = 0;
        for (int nbytes = 1; nbytes <= 4; nbytes++)
        {
            var lastIndex = data.Length - nbytes;
            for (int startIndex = 0; startIndex <= lastIndex; startIndex++)
            {
                var calc = (double)ExtractBytesAsIntBigEndian(data, nbytes, startIndex);
                nfound += Check("big endian straight", startIndex, nbytes, patternMin, patternMax, calc);
                foreach (var divideBy in divide)
                {
                    nfound += CheckDivide("big endian", startIndex, nbytes, patternMin, patternMax, calc, divideBy);
                }

                if (nbytes > 1)
                {
                    nfound += Check("big endian % 1000", startIndex, nbytes, patternMin, patternMax, calc % 1000);
                    nfound += Check("big endian % 10000", startIndex, nbytes, patternMin, patternMax, calc % 10000);
                    nfound += Check("big endian % 100000", startIndex, nbytes, patternMin, patternMax, calc % 100000);
                }

                if (nbytes > 1)
                {
                    calc = (double)ExtractBytesAsIntLittleEndian(data, nbytes, startIndex);
                    nfound += Check("little endian straight", startIndex, nbytes, patternMin, patternMax, calc);
                    foreach (var divideBy in divide)
                    {
                        nfound += CheckDivide("little endian", startIndex, nbytes, patternMin, patternMax, calc, divideBy);
                    }
                    nfound += Check("little endian % 1000", startIndex, nbytes, patternMin, patternMax, calc % 1000);
                    nfound += Check("little endian % 10000", startIndex, nbytes, patternMin, patternMax, calc % 10000);
                    nfound += Check("little endian % 100000", startIndex, nbytes, patternMin, patternMax, calc % 100000);
                }
            }
        }
        return nfound;
    }

    private static int CheckDivide(string testType, int startIndex, int nbytes, double patternMin, double patternMax, double binaryValue, double divideBy)
    {
        int retval = 0;
        double test = binaryValue / divideBy;
        if (test >= patternMin && test <= patternMax)
        {
            int original = (int)binaryValue;
            retval = 1;
            Console.WriteLine($"Found:{testType} / {divideBy} at={startIndex} length={nbytes} value={test} {original:X4}");
        }
        return retval;
    }

    private static int Check(string testType, int startIndex, int nbytes, double patternMin, double patternMax, double test)
    {
        int retval = 0;
        if (test >= patternMin && test <= patternMax)
        {
            int testi = (int)test;
            retval = 1;
            Console.WriteLine($"Found:{testType} at={startIndex} length={nbytes} value={test} {testi:X4}");
        }
        return retval;
    }


    private static int ExtractBytesAsIntBigEndian(byte[] data, int nbytes, int startIndex)
    {
        int retval = 0;
        for (int i = 0; i < nbytes; i++)
        {
            retval = (retval << 8) + data[startIndex + i];
        }
        return retval;
    }
    private static int ExtractBytesAsIntLittleEndian(byte[] data, int nbytes, int startIndex)
    {
        int retval = 0;
        for (int i = 0; i < nbytes; i++)
        {
            retval = (retval << 8) + data[startIndex + nbytes - i -1];
        }
        return retval;
    }

    public static int Test()
    {
        int nerror = 0;
        byte[] data = { 0x01, 0x01, 0x0E, 0x16, 0x8D, 0x59 };
        byte NA = 0xFF;
        nerror += TestExtractBitsOne(data, 0, 4, 1, 0x01, NA);
        nerror += TestExtractBitsOne(data, 32, 4, 1, 0x0D, NA);
        nerror += TestExtractBitsOne(data, 36, 4, 1, 0x08, NA);
        nerror += TestExtractBitsOne(data, 36, 8, 1, 0x98, NA);
        nerror += TestExtractBitsOne(data, 36, 12, 2, 0x98, 0x05);
        nerror += TestExtractBitsOne(data, 0, 8, 1, 0x01, NA);
        nerror += TestExtractBitsOne(data, 0, 7, 1, 0x01, NA);
        nerror += TestExtractBitsOne(data, 0, 16, 2, 0x01, 0x01);
        nerror += TestExtractBitsOne(data, 8, 16, 2, 0x01, 0x0E);

        return nerror;
    }

    public static byte[] ExtractBits(byte[] data, int startBit, int length)
    {
        var retlength = ((length + 7) / 8);
        var retval = new byte[retlength];
        for (int i = 0; i < length; i++)
        {
            var (fromByte, fromBit) = GetIndex(i + startBit);
            var (toByte, toBit) = GetIndex(i);
            var bit = (data[fromByte] & (1 << fromBit)) == 0 ? 0 : 1;
            retval[toByte] |= (byte)(bit << toBit);
        }
        return retval;
    }

    private static int TestExtractBitsOne(byte[] data, int startBit, int length, int expectedlength, byte expected0, byte expected1)
    {
        int nerror = 0;
        var actual = ExtractBits(data, startBit, length);
        if (actual.Length != expectedlength)
        {
            nerror++;
            Console.WriteLine($"Error: startBit={startBit} length={length} expected length={expectedlength} actual={actual.Length}");
        }
        if (actual.Length > 0 && actual[0] != expected0)
        {
            nerror++;
            Console.WriteLine($"Error: startBit={startBit} length={length} expected[0]={expected0:X} actual={actual[0]:X}");
        }
        if (actual.Length > 1 && actual[1] != expected1)
        {
            nerror++;
            Console.WriteLine($"Error: startBit={startBit} length={length} expected[1]={expected1:X} actual={actual[1]:X}");
        }
        return nerror;
    }
    public static int ExtractBitsAsInt(byte[] data, int startBit, int length)
    {
        var bits = ExtractBits(data, startBit, length);
        var padded = new byte[4] { 0, 0, 0, 0 };
        for (int i = 0; i < bits.Length; i++) padded[i] = bits[i];
        return 0; // TODO: blah?
    }

    private static (int byteIndex, int bitIndex) GetIndex(int startBit)
    {
        var byteIndex = (startBit) / 8;
        var bitIndex = startBit - (byteIndex * 8);
        return (byteIndex, bitIndex);
    }

    // Example: AA 0D A1 00 59 02 29 64 14 15 09 17 01 36 16 BB
    public static byte[] ConvertFromHex(string hex)
    {
        var list = hex.Split(new char[] { ' ' });
        var retval = new byte[list.Length];
        for (int i=0; i<list.Length; i++)
        {
            byte b = 0;
            var result = byte.TryParse(list[i], System.Globalization.NumberStyles.HexNumber, null, out b);
            if (!result)
            {
                Console.WriteLine($"Error: hex[{i}]={list[i]} is not a HEX byte");
            }
            retval[i] = b;
        }
        return retval;
    }
}