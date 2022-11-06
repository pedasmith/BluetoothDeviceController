using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothDeviceController.Names
{
    public static class GuidGetCommon
    {
        /// <summary>
        /// Given a string like BluetoothLE#BluetoothLEbc:83:85:22:5a:70-5c:31:3e:89:ad:5c return 5c:31:3e:89:ad:5c
        /// which is the Bluetooth address (the stuff before the - is the Windows ID. The GUID 83:85:22... will
        /// change at some interval, so it cannot be relied on.
        /// </summary>
        /// <param name="id">String like BluetoothLE#BluetoothLEbc:83:85:22:5a:70-5c:31:3e:89:ad:5c</param>
        /// <returns></returns>
        public static string NiceId(string id, string prefixIfReplaced="") // was DeviceInformation args)
        {
            var retval = id;
            var idx = retval.IndexOf('-');
            if (retval.StartsWith("BluetoothLE#BluetoothLE") && idx >= 0)
            {
                retval = prefixIfReplaced + retval.Substring(idx + 1);
            }
            return retval;
        }
        /// <summary>
        /// Given two bluetooth GUIDs, return a short version of string B such that it contains
        /// the characters that are different from A such that the differences are "bluetooth-like".
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static string GetCommon (this string A, string B)
        {
            //    0..7   9..12 14..17 19..22 24..35
            // A: EF680400-9B35-4933-9B10-52FFA9740042
            // B: EF680404-9B35-4933-9B10-52FFA9740042
            // Return: 0404

            // A: 9dc84838-7619-4f09-a1ce-ddcf63225b10
            // B: 9dc84838-7619-4f09-a1ce-ddcf63225b11
            // Return: 5b10

            var m1 = A.SubstringEqual(B, 0, 4);
            var m2 = A.SubstringEqual(B, 4, 4);
            var m3 = A.SubstringEqual(B, 9, 4);
            var m4 = A.SubstringEqual(B, 14, 4);
            var m5 = A.SubstringEqual(B, 19, 4);
            var m6 = A.SubstringEqual(B, 24, 8);
            var m7 = A.SubstringEqual(B, 32, 4);
            var commonMatch = m1 && !m2 && m3 && m4 && m5 && m6 && m7;
            if (commonMatch) return B.Substring(4, 4);

            commonMatch = m1 && m2 && m3 && m4 && m5 && m6 && !m7;
            if (commonMatch) return B.Substring(32, 4);

            return B;
        }

        public static int Test()
        {
            int NError = 0;
            NError += TestGetCommon();
            return NError;
        }

        private static int TestGetCommon()
        {
            int NError = 0;
            // Not a match in various ways
            NError += TestGetCommonOne("EF680400-9B35-4933-9B10-52FFA9740042", "EF680400-9B35-4933-9B10-52FFA9740042", "EF680400-9B35-4933-9B10-52FFA9740042");
            NError += TestGetCommonOne("EF680400-9B35-4933-9B10-52FFA9740042", "EF680404-9B35-4933-9B10-52FFA9740044", "EF680404-9B35-4933-9B10-52FFA9740044");

            // Proper match
            NError += TestGetCommonOne("EF680400-9B35-4933-9B10-52FFA9740042", "EF680404-9B35-4933-9B10-52FFA9740042", "0404");
            NError += TestGetCommonOne("9dc84838-7619-4f09-a1ce-ddcf63225b10", "9dc84838-7619-4f09-a1ce-ddcf63225b11", "5b11");

            return NError;
        }
        private static int TestGetCommonOne(string A, string B, string expected)
        {
            int NError = 0;
            try
            {
                var actual = A.GetCommon(B);
                if (actual != expected)
                {
                    NError++;
                    System.Diagnostics.Debug.WriteLine($"ERROR: TestGetCommon ({A}, {B}) expected {expected} actually got {actual}");
                }
            }
            catch (Exception e)
            {
                NError++;
                System.Diagnostics.Debug.WriteLine($"ERROR: TestGetCommon ({A}, {B}) threw exception {e.Message}");
            }


            return NError;
        }

        public static bool SubstringEqual (this string A, string B, int startIndex, int length)
        {
            var partA = A.Substring(startIndex, length);
            var partB = B.Substring(startIndex, length);
            return partA == partB;
        }
    }
}
