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
        // public string Voice { get; set; }
        public string Text { get; set; } = "";

#if NEVER_EVER_DEFINED
        public Speak(string text)
        {
            //Voice = voice;
            Text = text;
        }
        public Speak() // string voice)
        {
            //Voice = voice;
            Text = "";
        }
#endif

        public MediaPlayerElement MPE = null;
        //public List<string> Voices = new();
        //public string VoicesStr = "";

        public async Task RunAsync(double volume, string speak)
        {
            if (MPE == null || MPE.MediaPlayer == null) return;
            var state = MPE.MediaPlayer.CurrentState;
            if (state == Windows.Media.Playback.MediaPlayerState.Playing)
            {
                return; // Already playing a value
            }

            //if (Voice != null) SetVoice(Voice);

            // The object for controlling the speech synthesis engine (voice).
            var synth = new Windows.Media.SpeechSynthesis.SpeechSynthesizer();
            // if (PreferredVoice != null) synth.Voice = PreferredVoice;

            // Generate the audio stream from plain text.
            var stream = await synth.SynthesizeTextToStreamAsync(speak);

            // Send the stream to the media object.
            // uiMedia is a MediaElement
            MPE.MediaPlayer.SetStreamSource(stream);
            MPE.MediaPlayer.Volume = volume;
            MPE.MediaPlayer.Play();
        }

        // An exact match ("Microsoft David" is best.
        // Second best is a partial match like "David" for Microsoft David. 
        // 2026-10-27 Removed in favor of the OneCore (Modern) voices
#if NEVER_EVER_DEFINED
        //Windows.Media.SpeechSynthesis.VoiceInformation PreferredVoice = null;
        /// <summary>
        /// Sets PreferredVoice to match what's returned by SpeechSynthesizer.AllVoices.
        /// According to Copilot 2026-10-27 AllVoices just returns the obsolete SAPI
        /// voices like "Microsoft David". The new voices from Accessability are e.g. Aria, Jenny 
        /// and more and are called the OneCore (Modern) voices.
        /// </summary>
        private void ZZZSetVoice(string name)
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
                    PreferredVoice = voice;
                }
                else if (voice.DisplayName.Contains(name))
                {
                    secondVoice = voice;
                }
            }
            if (PreferredVoice == null) PreferredVoice = secondVoice;
        }
#endif
        public override string ToString() { return $"SPEAK " ; }
    }
}
