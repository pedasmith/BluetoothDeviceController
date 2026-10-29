using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothWinUI3.Beeps
{
    internal static class GlobalSpeaker
    {
        private static Speak? CurrSpeak;

        private static bool Initialize()
        {
            if (CurrSpeak != null && CurrSpeak.MPE != null) return true; // already initialized
            CurrSpeak = new Speak();
            CurrSpeak.MPE = App.CurrMediaPlayerElement;

            if (CurrSpeak != null && CurrSpeak.MPE != null) return true; // already initialized
            return false;
        }
        static Task? SpeakTask;

        public static void Speak(UserPreferences.SoundLevelType voiceLevel, string speak)
        {
            if (!Initialize()) return;
            if (MainWindow.IsMuteAll) return;
            double volume = 1.0; // default for 'On' (and also 'NotSet', but that should never be passed in)
            if (voiceLevel == UserPreferences.SoundLevelType.Off) return;
            if (voiceLevel == UserPreferences.SoundLevelType.Quiet) volume = 0.5;
            SpeakTask = CurrSpeak?.RunAsync(volume, speak);
        }
    }
}
