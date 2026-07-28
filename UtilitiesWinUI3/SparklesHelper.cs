using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace UtilitiesWinUI3
{
    internal class SparklesHelper
    {
        Dictionary<string, int> NPropertyChanges { get; } = [];

        readonly List<string> Sparkles = ["──", "╾─", "╼─", "─╾", "─╼", "─╾", "╾─",]; // 2026-07-28: Box drawing "─ ╼ ╾"

        public void InitializeSparkles(List<(string, Microsoft.UI.Xaml.Documents.Run)> controlsWithSparkles)
        {
            foreach ((string potentialMatchName, Microsoft.UI.Xaml.Documents.Run run) in controlsWithSparkles)
            {
                run.Text = Sparkles[0];
            }
        }

        /// <summary>
        /// Updates the sparkles based on the changed property. Called from UpdateDeviceDataUX which is
        /// called by Device_PropertyChanged when a device property changes.
        /// </summary>
        public void UpdateSparkles(List<(string, Microsoft.UI.Xaml.Documents.Run)> controlsWithSparkles, string name)
        {
            // In practice, name is never "*". The code is set up this way to match the Govee code.
            if (name == "") return;
            NPropertyChanges[name] = NPropertyChanges.GetValueOrDefault(name, 0) + 1;
            int sparkleIndex = SparkleIndex(NPropertyChanges[name], Sparkles.Count);

            foreach ((string potentialMatchName, Microsoft.UI.Xaml.Documents.Run run) in controlsWithSparkles)
            {
                if (potentialMatchName == name || name == "*")
                {
                    run.Text = Sparkles[sparkleIndex];
                }
            }
        }

        /// <summary>
        /// Given a number 0...very large return a number 0..n. This is like
        /// a normal mod (%) operator except that the only time we return zero
        /// is when the nchange is zero
        /// </summary>
        private static int SparkleIndex(int nchange, int maxIndex)
        {
            // Example:
            // NCHANGE: 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9
            // OUTPUT : 0 1 2 3 1 2 3 1 2 3 1 2 3 1 2 3
            // 
            // mod    : X X X X 0 1 2 0 1 2 0 1 2 0 1 2
            // retval : 
            int retval = 0;
            if (nchange < maxIndex)
            {
                retval = nchange;
            }
            else
            {
                var mod = (nchange-maxIndex) % (maxIndex - 1);
                retval = mod + 1;
            }

            return retval;
        }

        private static void Log(string str)
        {
            System.Diagnostics.Debug.WriteLine(str);
        }

        private static int TestOne(int nchange, int maxIndex, int expected)
        {
            int nerror = 0;
            var actual = SparkleIndex(nchange, maxIndex);
            if (actual != expected)
            {
                nerror++;
                Log($"Error: Sparkle: ({nchange}, {maxIndex}) returned {actual} but expected={expected}");
            }
            return nerror;
        }

        public static int Test()
        {
            int nerror = 0;
            nerror += TestOne( 0, 4, 0);
            nerror += TestOne( 1, 4, 1);
            nerror += TestOne( 2, 4, 2);
            nerror += TestOne( 3, 4, 3);
            nerror += TestOne( 4, 4, 1);
            nerror += TestOne( 5, 4, 2);
            nerror += TestOne( 6, 4, 3);
            nerror += TestOne( 7, 4, 1);
            nerror += TestOne( 8, 4, 2);
            nerror += TestOne( 9, 4, 3);
            nerror += TestOne(10, 4, 1);
            nerror += TestOne(11, 4, 2);
            return nerror;
        }
    }
}
