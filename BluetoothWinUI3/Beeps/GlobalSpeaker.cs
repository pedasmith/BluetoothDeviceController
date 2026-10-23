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
            CurrSpeak = new Speak("David");
            CurrSpeak.MPE = App.CurrMediaPlayerElement;

            if (CurrSpeak != null && CurrSpeak.MPE != null) return true; // already initialized
            return false;
        }
        static Task? SpeakTask;

        public static void Speak(string speak)
        {
            if (!Initialize()) return;
            SpeakTask = CurrSpeak?.RunAsync(speak);
        }
    }
}
