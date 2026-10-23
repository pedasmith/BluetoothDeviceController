using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Midi;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace MusicFidget
{
    public class MidiSynth
    {
        static MidiSynthesizer MS = null;
        public static async Task<MidiSynthesizer> GetSynthesizerAsync(MidiSynthesizer ms = null)
        {
            if (ms != null)
            {
                MS = ms;
                return ms;
            }
            if (MS == null) MS = await MidiSynthesizer.CreateAsync();
            return MS;
        }

        private static async Task DoMessageAsync(IMidiMessage midiMessage, MidiSynthesizer ms = null)
        {
            ms = await GetSynthesizerAsync(ms);
            if (ms == null) return;
            try
            {
                ms?.SendMessage(midiMessage);
            }
            catch (Exception)
            {
                //nexception++;
            }
        }

        // Turn off all notes
        public static async Task DoStopAll(byte channel, MidiSynthesizer ms = null)
        {
            var midiMessage = new MidiControlChangeMessage(channel, 120, 0); // Control Change c=120 v=0 is All Sound Off
            await DoMessageAsync(midiMessage, ms);
        }

        // e.g. program byte 14 is program #15 which is tubular bells.
        static public async Task DoProgramChangeForChannel(byte channel, byte program, MidiSynthesizer ms = null)
        {
            var midiMessage = new MidiProgramChangeMessage(channel, program);
            await DoMessageAsync(midiMessage, ms);
        }

    }
}
