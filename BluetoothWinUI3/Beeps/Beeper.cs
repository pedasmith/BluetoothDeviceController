using AdvancedCalculator.BCBasic.RunTimeLibrary;
using System;
using System.Collections.Generic;
using System.Text;
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
        public const string Continuity = "continuity";
        static Dictionary<string, string> BeepType = new()
        {
            // FYI: I13 is the Marimba
            { Connect, "I13 T180 O3 L8 C E G" }, // was "O2 T120 E8 E8"
            { "zzConnect", "O2 T120 E8 E8" },
            { Continuity, "I13 T180 O4 L4 C" }, // was "T120 L4 O0 C c#"
        };

        /// <summary>
        /// Allowed beep types: connect continuity (or an MML string)
        /// </summary>
        /// <param name="mmlString"></param>
        public static void Beep(UserPreferences.SoundLevelType beepLevel, string mmlString = "O2 T120 E8 E8 F8")
        {
            if (MainWindow.IsMuteAll) return;
            if (NotePlayer == null) return;
            if (BeepType.ContainsKey(mmlString))
            {
                mmlString = BeepType[mmlString];
            }
            var velocity = GetVelocity(beepLevel);
            if (velocity == 0) return;
            NotePlayer.AddMmlString(mmlString, velocity);
        }

        public static void BeepIfNotBeeping(UserPreferences.SoundLevelType beepLevel, string mmlString = "O2 T120 E8 E8 F8")
        {
            if (MainWindow.IsMuteAll) return;
            if (NotePlayer == null) return;
            if (NotePlayer.IsPlaying()) return;

            if (BeepType.ContainsKey(mmlString))
            {
                mmlString = BeepType[mmlString];
            }
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
