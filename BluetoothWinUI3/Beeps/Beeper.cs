using AdvancedCalculator.BCBasic.RunTimeLibrary;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Midi;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothWinUI3.Beeps
{
    internal static class Beeper
    {
        public static MidiSynthesizer MS;
        public static MmlNotePlayer NotePlayer;
        static CancellationTokenSource CTS = new();

        public static async Task InitializeAsync()
        {
            MS = await MidiSynthesizer.CreateAsync();
            NotePlayer = new (CTS.Token);
        }
        public const string Connect = "connect";
        public const string GeneralFailure = "generalfailure";
        public const string ConnectionFailed = "connectionfailed";
        public const string Timeout = "timeout";

        // Special cases of connect based on tagToResourceKeyMap in AppDictionaryFromTag.cs
        public const string ConnectCycling = "connect #cycling";
        public const string ConnectHeartRate = "connect #heartrate";
        public const string ConnectPulseOximeter = "connect #pulseoximeter";
        public const string ConnectHealth = "connect #health";

        public const string ConnectTestEquipment = "connect #testequipment";
        public const string ConnectMultimeter = "connect #multimeter";
        public const string ConnectOscilloscope = "connect #oscilloscope";

        public const string ConnectEnvironment = "connect #environment";
        public const string ConnectSecurity = "connect #security";
        public const string ConnectEntertainment = "connect #entertainment";
        public const string ConnectProductivity = "connect #productivity";
        public const string ConnectReport = "connect #report";
        public const string ConnectUtility = "connect #utility";
        public const string ConnectOther = "connect #other";

        // Synonyms
        public const string ConnectExercise = "connect #exercise";
        public const string ConnectCooking = "connect #cooking";
        public const string ConnectAgriculture = "connect #agriculture";
        public const string ConnectLight = "connect #light";

        // Specialty string
        public const string Continuity = "continuity";



        public static Dictionary<string, string> BeepType = new()
        {
            // FYI: I11 is Vibraphone I13 is the Marimba I14 is Xylophone
            { Connect, "I13 T180 O3 L8 C E L4 G" }, // was "O2 T120 E8 E8"

            { ConnectCycling, "I14 T200 O5 L16 C E G" },
            { ConnectHeartRate, "I13 T160 O3 L16 C L16 E" },
            { ConnectPulseOximeter, "I13 T160 O3 L16 C L16 E" },
            { ConnectHealth, "I13 T160 O3 L16 C L16 E" },

            { ConnectTestEquipment, "I13 T180 O5 L16 C C# E" },
            { ConnectMultimeter, "I13 T180 O5 L16 C C# E" },
            { ConnectOscilloscope, "I13 T180 O5 L16 C C# E" },

            { ConnectEnvironment, "I13 T150 O4 L8 C E L8 D" },
            { ConnectSecurity, "I13 T160 O3 L8 C F L4 G" },
            { ConnectEntertainment, "I61 T170 O4 L4 C G" },
            { ConnectProductivity, "I11 T170 O4 L8 C E L4 D" },
            { ConnectReport, "I11 T170 O4 L8 C E L4 G" },
            { ConnectUtility, "I13 T180 O4 L8 C D L4 E" },
            { ConnectOther, "I11 T180 O4 L8 D G L4 C" },
            { ConnectExercise, "I14 T200 O5 L16 C F A" },
            { ConnectCooking, "I13 T150 O4 L16 C L16 D L8 G" },
            { ConnectAgriculture, "I13 T140 O3 L16 C L16 C L8 G" },
            { ConnectLight, "I73 T180 O5 L16 C L16 E L8 G" },

            // Failures
            { GeneralFailure, "I13 T160 O2 L8 E C L4 C" },
            { ConnectionFailed, "I11 T180 O2 L8 G E L4 C" },
            { Timeout, "I14 T200 O2 L16 G F D" },

            // Specialty tones
            { Continuity, "I13 T180 O4 L4 C" }, // was "T120 L4 O0 C c#"
        };

        /// <summary>
        /// Takes a string like "connect #oscilloscope #testequipment and converts it to an MML string.
        /// Returns the origina string if it's not in the right format or is the right format with tags
        /// that are unknown.
        /// </summary>
        public static string ConnectTagsToMml(string verbAndTags)
        {
            var list = verbAndTags.Split(" ");
            if (list.Length < 2) return verbAndTags;

            var start = list[0]; // e.g., "connect"
            for (int i = 1; i < list.Length; i++)
            {
                var trial = $"{start} {list[i]}"; // e.g., "connect #oscilloscope"
                if (BeepType.ContainsKey(trial))
                {
                    return BeepType[trial];
                }
            }
            if (BeepType.ContainsKey(start))
            {
                return BeepType[start];
            }

            return verbAndTags;
        }


        /// <summary>
        /// Allowed beep types: connect continuity (or an MML string)
        /// </summary>
        /// <param name="mmlString"></param>
        public static void Beep(UserPreferences.SoundLevelType beepLevel, string mmlString = "O2 T120 E8 E8 F8")
        {
            if (MainWindow.IsMuteAll) return;
            if (NotePlayer == null) return;
            mmlString = ConnectTagsToMml(mmlString);
            var velocity = GetVelocity(beepLevel);
            if (velocity == 0) return;
            NotePlayer.AddMmlString(mmlString, velocity);
        }


        public static void BeepIfNotBeeping(UserPreferences.SoundLevelType beepLevel, string mmlString = "O2 T120 E8 E8 F8")
        {
            if (MainWindow.IsMuteAll) return;
            if (NotePlayer == null) return;
            if (NotePlayer.IsPlaying()) return;

            mmlString = ConnectTagsToMml(mmlString);
            var velocity = GetVelocity(beepLevel);
            if (velocity == 0) return;
            NotePlayer.AddMmlString(mmlString, velocity);
        }

        private static byte GetVelocity(UserPreferences.SoundLevelType beepLevel)
        {
            switch (beepLevel)
            {
                case UserPreferences.SoundLevelType.Off: return 0;
                case UserPreferences.SoundLevelType.On: return 75;
                case UserPreferences.SoundLevelType.Quiet: return 25;
            }
            return 75;
        }
    }
}
