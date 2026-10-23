using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothWinUI3.Beeps
{
    class Speak
    {
        public static void RunComplete()
        {
            if (cts != null)
            {
                cts.Cancel();
                cts = null;
            }
        }
        static CancellationTokenSource cts = null;
        public string Voice { get; set; }
        public string Text { get; set; }

        public Speak(string voice, string text)
        {
            Voice = voice;
            Text = text;
        }
        public Speak(string voice)
        {
            Voice = voice;
            Text = "";
        }

        public MediaPlayerElement MPE = null;
        public List<string> Voices = new();
        public string VoicesStr = "";

        public async Task RunAsync(string speak)
        {
            if (MPE == null || MPE.MediaPlayer == null) return;
            var state = MPE.MediaPlayer.CurrentState;
            if (state == Windows.Media.Playback.MediaPlayerState.Playing)
            {
                return; // Already playing a value
            }

            if (Voice != null) SetVoice(Voice);

            // The object for controlling the speech synthesis engine (voice).
            var synth = new Windows.Media.SpeechSynthesis.SpeechSynthesizer();
            if (preferredVoice != null) synth.Voice = preferredVoice;

            // Generate the audio stream from plain text.
            var stream = await synth.SynthesizeTextToStreamAsync(speak);

            // Send the stream to the media object.
            // uiMedia is a MediaElement
            MPE.MediaPlayer.SetStreamSource(stream);
            MPE.MediaPlayer.Play();
        }

        // An exact match ("Microsoft David" is best.
        // Second best is a partial match like "David" for Microsoft David. 
        Windows.Media.SpeechSynthesis.VoiceInformation preferredVoice = null;
        private void SetVoice(string name)
        {
            Windows.Media.SpeechSynthesis.VoiceInformation secondVoice = null;
            Voices.Clear();
            VoicesStr = "";
            foreach (var voice in Windows.Media.SpeechSynthesis.SpeechSynthesizer.AllVoices)
            {
                Voices.Add(voice.DisplayName);
                if (VoicesStr != "") VoicesStr += ", ";
                VoicesStr += voice.DisplayName;

                if (voice.DisplayName == name)
                {
                    preferredVoice = voice;
                }
                else if (voice.DisplayName.Contains(name))
                {
                    secondVoice = voice;
                }
            }
            if (preferredVoice == null) preferredVoice = secondVoice;
        }
        public override string ToString() { return $"SPEAK {VoicesStr}" ; }
    }
}
