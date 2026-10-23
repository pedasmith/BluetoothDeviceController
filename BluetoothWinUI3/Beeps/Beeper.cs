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

        static Dictionary<string, string> BeepType = new()
        {
            { "connect", "O2 T120 E8" },
            { "continuity", "T120 L4 O0 C c#" },
        };

        /// <summary>
        /// Allowed beep types: connect continuity (or an MML string)
        /// </summary>
        /// <param name="mmlString"></param>
        public static void Beep(string mmlString = "O2 T120 E8 E8 F8 G8 G8 F8")
        {
            if (NotePlayer == null) return;
            if (BeepType.ContainsKey(mmlString))
            {
                mmlString = BeepType[mmlString];
            }
            NotePlayer.AddMmlString(mmlString);
        }

        public static void BeepIfNotBeeping(string mmlString = "O2 T120 E8 E8 F8 G8 G8 F8")
        {
            if (NotePlayer.IsPlaying()) return;

            if (BeepType.ContainsKey(mmlString))
            {
                mmlString = BeepType[mmlString];
            }
            NotePlayer.AddMmlString(mmlString);
        }
    }
}
