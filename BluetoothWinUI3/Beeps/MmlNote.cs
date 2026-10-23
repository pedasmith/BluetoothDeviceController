using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Devices.Midi;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace MusicMacroLanguage
{
    public class MmlNote
    {
        public MmlNote(string image, char pitch, int noteModifier, int octave, int duration, double silencePercent, int instrument)
        {
            Image = image;
            Pitch = pitch; // Pitch might be 'R' for Rest
            NoteModifier = noteModifier;
            Octave = octave;
            DurationInMilliseconds = duration;
            SilencePercent = silencePercent;
            Instrument = instrument;
        }
        public override string ToString()
        {
            var Retval = $"{Image}{SharpFlat} {Octave} {Pitch}/{NoteModifier} {DurationInMilliseconds}milliSeconds";
            return Retval;
        }
        string Image;
        string SharpFlat { get { if (NoteModifier == 1) return "♯"; if (NoteModifier == -1) return "♭"; return ""; } }
        public char Pitch; // A-G, R
        int NoteModifier = 0;
        int Octave;
        public int Instrument = -1; // default instrument is "not set"
        public int DurationInMilliseconds;
        public byte Velocity = 75;
        public double SilencePercent = 7.0 / 8.0; // MN=music normal.  ML=1.0 (no spaces) MS=.75 (longer space)
        private static byte PitchToMidiNote(char pitch, int NoteModifier, int octave)
        {
            int n = 60;
            switch (pitch)
            {
                case 'A': n = 69; break;
                case 'B': n = 71; break;
                case 'C': n = 60; break;
                case 'D': n = 62; break;
                case 'E': n = 64; break;
                case 'F': n = 65; break;
                case 'G': n = 67; break;
            }
            n += NoteModifier;

            int octaveDelta = (octave - 4);
            n += 12 * octaveDelta;
            return (byte)n;
        }
        public byte MidiNote { get { return PitchToMidiNote(Pitch, NoteModifier, Octave); } }

        public enum MMLType { Classical, Modern }
        static public IEnumerable<MmlNote> Parse(string MML, MMLType type)
        {
            List<MmlNote> Retval = new List<MmlNote>();
            // Nice defaults
            int octave = 4;
            int tempo = 150;
            int instrument = -1; // default value

            var musicRegex = "(?<music>[mM][lnsLNS])";
            var octaveRegex = "(?<octave>[<>])";
            var optionRegex = "((?<option>[iloprtILOPRT])(?<value>[0-9]*))";
            // The underscore is weird; it will match a < sign, too?
            //var noteRegex = "((?<octavePrefix>[+-￣_]?)(?<note>[a-gA-G])(?<sharp>[+#-]?)(?<duration>[0-9]*))";
            var noteRegex = "((?<octavePrefix>[+\\-]?)(?<note>[a-gA-G])(?<sharp>[+#♯\\-♭]?)(?<duration>[0-9]*))";
            var itemRegex = $"{optionRegex}|{noteRegex}|{octaveRegex}|{musicRegex}|([\\s]+)";
            var matches = Regex.Matches(MML, itemRegex);
            var defaultDuration = "4"; // 4=quarter note
            var silencePercent = 7.0 / 8.0; // MN=.875, ML (legato)=1.0 MS (statico) = .75
            foreach (Match match in matches)
            {
                var note = match.Groups["note"].Value.ToUpper();
                var duration = match.Groups["duration"].Value;
                var option = match.Groups["option"].Value.ToUpper();
                var value = match.Groups["value"].Value;
                int valueInt = 0;
                Int32.TryParse(value, out valueInt);
                var octaveMatch = match.Groups["octave"].Value;
                var octavePrefix = match.Groups["octavePrefix"].Value;
                var sharp = match.Groups["sharp"].Value; // + and # and ♯ for sharp, - or ♭ for flat
                var musicMatch = match.Groups["music"].Value;
                int noteModifier = 0;

                var noteImage = "¼";
                switch (sharp)
                {
                    case "+": noteModifier++; break;
                    case "#": noteModifier++; break;
                    case "♯": noteModifier++; break;
                    case "-": noteModifier--; break;
                    case "♭": noteModifier--; break;
                }

                switch (musicMatch.ToUpper())
                {
                    case "ML": silencePercent = 1.0; break; // Legato
                    case "MN": silencePercent = .875; break; // Normal
                    case "MS": silencePercent = .75; break; // Statico
                }
                switch (octaveMatch)
                {
                    case ">":
                        octave++;
                        break;
                    case "<":
                        octave--;
                        break;
                }

                switch (option)
                {
                    case "I":
                        instrument = valueInt;//a Midi value
                        var n = new MmlNote(noteImage, 'I', noteModifier, octave, 0, silencePercent, instrument-1); // Just the instrument is really used here.  Input is 15 based but internally 0 based
                        Retval.Add(n);
                        break;
                    case "L":
                        defaultDuration = value;//4=quarter note
                        break;
                    case "O":
                        octave = valueInt;
                        break;
                    case "P":
                        note = "R"; // turn it into a REST
                        noteImage = "‖";
                        duration = value;
                        break;
                    case "R":
                        note = "R"; // turn it into a REST
                        noteImage = "‖";
                        duration = value;
                        break;
                    case "T":
                        tempo = valueInt;
                        break;
                }
                if (note != "")
                {
                    int noteOctave = octave;
                    double quarterNoteDurationInMilliseconds = (int)(60.0 * 1000.0 / (double)tempo);
                    int noteDurationInMilliseconds = (int)quarterNoteDurationInMilliseconds;
                    switch (octavePrefix)
                    {
                        case "-": noteOctave--; break;
                        //case "_": noteOctave--; break;
                        case "+": noteOctave--; break;
                            //case "￣": noteOctave--; break;
                    }
                    switch (type)
                    {
                        // 0=32nd, 1=16th, 2=dotted 16th, 3=eighth, 
                        case MMLType.Classical:
                            if (duration == "") duration = "5"; // quarter note
                            var minim = quarterNoteDurationInMilliseconds / 8.0; // a 32nd note is a quarter note divided by eight.
                            switch (duration)
                            {
                                case "0": noteImage = "㉜"; noteDurationInMilliseconds = (int)(minim * 1); break; // 1/32
                                case "1": noteImage = "⑯"; noteDurationInMilliseconds = (int)(minim * 2); break; // 1/16
                                case "2": noteImage = "⑯·"; noteDurationInMilliseconds = (int)(minim * 3); break;
                                case "3": noteImage = "♪"; noteDurationInMilliseconds = (int)(minim * 4); break; // eighth
                                case "4": noteImage = "♪·"; noteDurationInMilliseconds = (int)(minim * 6); break;
                                case "5": noteImage = "♩"; noteDurationInMilliseconds = (int)(minim * 8); break; // quarter
                                case "6": noteImage = "♩·"; noteDurationInMilliseconds = (int)(minim * 12); break;
                                case "7": noteImage = "½"; noteDurationInMilliseconds = (int)(minim * 16); break; // half
                                case "8": noteImage = "½·"; noteDurationInMilliseconds = (int)(minim * 24); break;
                                case "9": noteImage = "o"; noteDurationInMilliseconds = (int)(minim * 32); break; // whole
                            }
                            break;
                        case MMLType.Modern:
                            if (duration == "") duration = defaultDuration; // default is a quarter note.
                            double noteDuration = 4;
                            Double.TryParse(duration, out noteDuration);
                            noteDurationInMilliseconds = (int)(quarterNoteDurationInMilliseconds * 4 / noteDuration);
                            switch (duration)
                            {
                                case "0": noteImage = "o"; break;
                                case "1": noteImage = "o"; break;
                                case "2": noteImage = "½"; break;
                                case "4": noteImage = "♩"; break;
                                case "8": noteImage = "♪"; break;
                                case "16": noteImage = "⑯"; break;
                                case "32": noteImage = "㉜"; break;
                            }
                            break;
                    }

                    var n = new MmlNote(noteImage, note[0], noteModifier, noteOctave, noteDurationInMilliseconds, silencePercent, instrument);
                    Retval.Add(n);
                }
            }
            return Retval;
        }

        public static int Test()
        {
            var NError = 0;
            var notes = Parse("A4 G2", MMLType.Modern);
            return NError;
        }

        static int nexception = 0;

        static public async Task DoProgramChange(int program, MidiSynthesizer ms = null)
        {
            if (program < 0) return; // not a valid program
            ms = await MusicFidget.MidiSynth.GetSynthesizerAsync(ms);
            try
            {
                byte channel = 0; // always zero!
                var midiMessage = new MidiProgramChangeMessage(channel, (byte)program);
                ms?.SendMessage(midiMessage);
            }
            catch (Exception)
            {
                nexception++;
            }
        }

        public static byte channel = 0;
        static public async Task NoteOnAsync(MmlNote note, MidiSynthesizer ms = null)
        {
            ms = await MusicFidget.MidiSynth.GetSynthesizerAsync(ms);
            try
            {
                var midiMessage = new MidiNoteOnMessage(channel, note.MidiNote, note.Velocity);
                ms?.SendMessage(midiMessage);
            }
            catch (Exception)
            {
                nexception++;
            }
        }

        static public async Task NoteOffAsync(MmlNote note, MidiSynthesizer ms = null)
        {
            ms = await MusicFidget.MidiSynth.GetSynthesizerAsync(ms);
            try
            {
                var midiMessage = new MidiNoteOffMessage(channel, note.MidiNote, note.Velocity);
                ms?.SendMessage(midiMessage);
            }
            catch (Exception)
            {
                nexception++;
            }
        }

        static public async Task PlayNotesAsync(IEnumerable<MmlNote> notes, MidiSynthesizer ms = null)
        {
            ms = await MusicFidget.MidiSynth.GetSynthesizerAsync(ms);
            foreach (var note in notes)
            {
                if (note.Pitch == 'I') // Instrument
                {
                    await DoProgramChange(note.Instrument, ms);
                }
                else if (note.Pitch == 'R')
                {
                    await Task.Delay(note.DurationInMilliseconds);
                }
                else
                {
                    try
                    {
                        int playDuration = (int)Math.Ceiling(note.DurationInMilliseconds * note.SilencePercent);
                        int silenceDuration = note.DurationInMilliseconds - playDuration;
                        await NoteOnAsync(note, ms);
                        await Task.Delay(playDuration);
                        await NoteOffAsync(note, ms);
                        if (silenceDuration > 0) await Task.Delay(silenceDuration);
                    }
                    catch (Exception)
                    {
                        nexception++;
                    }
                }
            }
        }

        static public async Task PlayNotesAsync(IEnumerable<IEnumerable<MmlNote>> notes, MidiSynthesizer ms = null)
        {
            ms = await MusicFidget.MidiSynth.GetSynthesizerAsync(ms);
            List<Task> tasks = new List<Task>();
            foreach (var list in notes)
            {
                var t = PlayNotesAsync(list, ms);
                tasks.Add(t);
            }
            await Task.WhenAll(tasks);
        }

        // Samples from http://www.vgmpf.com/Wiki/index.php?title=Music_Macro_Language
#if ORIGINAL_MUSIC_SAMPLES
Examples
Microsoft BASIC
This will play Ode to Joy on the PC Speaker.

10 PLAY "O2 T120 E8 E8 F8 G8 G8 F8 E8 D8 C8 C8 E8 E8 E8 D12 D4"
20 PLAY "E8 E8 F8 C8 G8 F8 E8 D8 C8 C8 D8 E8 D8 C12 C4"
30 PLAY "D8 D8 E8 C8 D8 E12 F12 E8 C8 D8 E12 F12 E8 D8 C8 D8 P8"
40 PLAY "E8 E8 F8 G8 G8 F8 E8 D8 C8 C8 D8 E8 D8 C12 C4"
Sharp S-BASIC
This plays the Japanese song "Toryanse" on the PC Speaker in Sharp MZ-731.

10 TEMPO 4
20 A$="E5R1E3R0D3R0E3R0E1R0D1R0-G4R1"
30 B$="F3R0F1R0F1R0A3R0F1R0E1R0D1R0D1R0E5R0"
40 C$="C3R0C1R0C1R0E3R0C1R0-B1R0C1R0-B1R0-A1R0-A1-B5R0"
50 D$="E1R0E1R0E1R0E1R0E1R0E1R0D1R0E1R0E1R0E1R0D1R0-A1R0-A1R0B3R1"
60 E$="-A1R0-B1R0C1R0D1R0E1R0F1R0E1R0F3R1A3R1B1R0A1R0F3R0E3R0E1R0E4R0"
70 MUSIC A$+B$+B$
80 MUSIC C$+C$+B$
90 MUSIC C$+D$+E$
Tandy 1000 BASIC
This program will play the first couple bars of J.S. Bach's Fantasia and Fugue In C Minor on the Tandy 3 Voice.

10 DIM A$(3)
20 DIM B$(3)
30 DIM C$(3)
40 SOUND ON
50 PLAY "MLv15T80", "MLv14T80", "MNv15T80"
60 A$(0)="o2l6p6p6go3el12p12dl6ecl3fl12fl24edl12eagal6dl3gl12gl24fel12fco2bo3f"
70 B$(0)="o2l6p6p6p6p6p6cal12p12gl6ago3l3cl12cl24o2bal12bo3edel6o2al3o3d"
80 C$(0)="o1l1ccc"
90 A$(1)="o3l12fel3al12al24gfl12edegl6bl12agl6fl2o4c"
100 B$(1)="o3l12dl6co2l12bo3cdo2l3bl6bo3l12dcl3cl12cdefge"
110 C$(1)="o1l1cc"
120 A$(2)="o4l6cl12p12co3bo4co3l6bl12o4co3bagfl3gl6fp6e"
130 B$(2)="o3l12fcl3fl12fl24edl3el12el24dcbcbcl6dl12dl24cbl6cl12db"
140 C$(2)="o1l1cl2cl3cl6o2c"
150 FOR N = 0 TO 2
160 PLAY A$(N), B$(N), C$(N)
170 NEXT
180 END
NES
Here is an example of a song written for the NES in a custom MML.

.#TITLE Song Title
.#COMPOSER Composer
.#PROGRAMER Copyright
@v0 = { 10 9 8 7 6 5 4 3 2 }
@v1 = { 15 15 14 14 13 13 12 12 11 11 10 10 9 9 8 8 7 7 6 6 }
@v2 = { 15 12 10 8 6 3 2 1 0 }
@v3 = { 15 14 13 12 11 10 9 8 7 6 5 4 3 2 1 0 }
@DPCM0 = { "samples\kick.dmc",15 }
@DPCM2 = { "samples\snare.dmc",15 }
ABCDE t150
A l8 o4 @01 @v0
A [c d e f @v1 g4 @v0 a16 b16 >c c d e f @v1 g4 @v0 a16 b16 >c<<]2
C l4 o3 q6
C [c e g8 g8 a16 b16 >c8 c e g8 g8 a16 b16 >c8<<]4
D o0
D [@v2 b @v3 e @v2 b @v3 e @v2 b @v3 e @v2 b @v3 e8 @v2 b8]4

#endif

        // Sounds very mediocre!
        static public IEnumerable<MmlNote> OdeToJoy()
        {
            var str10 = "O2 T120 E8 E8 F8 G8 G8 F8 E8 D8 C8 C8 E8 E8 E8 D12 D4";
            var str20 = "E8 E8 F8 C8 G8 F8 E8 D8 C8 C8 D8 E8 D8 C12 C4";
            var str30 = "D8 D8 E8 C8 D8 E12 F12 E8 C8 D8 E12 F12 E8 D8 C8 D8 P8";
            var str40 = "E8 E8 F8 G8 G8 F8 E8 D8 C8 C8 D8 E8 D8 C12 C4";
            var str = str10 + str20 + str30 + str40;
            return Parse(str, MMLType.Modern);
        }

        // Also pretty terrible!
        static public IEnumerable<MmlNote> Bach()
        {
            var str = "t220o4l4cdefedcrefgagfe4crcrcrcrl8ccddeeffl4edcr";
            return Parse(str, MMLType.Modern);
        }


        static public IEnumerable<IEnumerable<MmlNote>> Fugue()
        {
            var Retval = new List<IEnumerable<MmlNote>>();
            var a0 = "o2l6p6p6go3el12p12dl6ecl3fl12fl24edl12eagal6dl3gl12gl24fel12fco2bo3f   o3l12fel3al12al24gfl12edegl6bl12agl6fl2o4c  o4l6cl12p12co3bo4co3l6bl12o4co3bagfl3gl6fp6e";
            a0 = a0.Replace("o4", "o6").Replace("o3", "o5").Replace("o3", "o5").Replace("o2", "o4");
            //var oldb0 = "o2l6p6p6p6p6p6cal12p12gl6ago3l3cl12cl24o2bal12bo3edel6o2al3o3d  o3l12dl6co2l12bo3cdo2l3bl6bo3l12dcl3cl12cdefge  o3l12fcl3fl12fl24edl3el12el24dcbcbcl6dl12dl24cbl6cl12db";
            var b0 = "    o3 f12 c f3 f12 e24 d24 el3 e12 l24 dcbcbc d6 d12 l24 cb c6 l12 db";
            //var c0 = "o1l1ccc  o1l1cc  o1l1cl2cl3cl6o2c";
            //Retval.Add(Parse(a0, MMLType.Modern));
            Retval.Add(Parse(b0, MMLType.Modern));
            //Retval.Add(Parse(c0, MMLType.Modern));
            return Retval;
        }
        static public IEnumerable<MmlNote> Kimigayo()
        {
            var str = "T250 O4 D2C2D2E2 G2E2D1 E2G2A2G4A4 O5D2O4B2A2G2 E2G2A1 O5D2C2D1 O4E2G2A2G2 E2R4G4D1 A2O5C2D1 C2D2O4A2G2 A2G4E4D";
            return Parse(str, MMLType.Modern);
        }

        static public IEnumerable<MmlNote> Toryanse()
        {
            var a = "E5R1E3R0D3R0E3R0E1R0D1R0-G4R1";
            var b = "F3R0F1R0F1R0A3R0F1R0E1R0D1R0D1R0E5R0";
            var c = "C3R0C1R0C1R0E3R0C1R0-B1R0C1R0-B1R0-A1R0-A1-B5R0";
            var d = "E1R0E1R0E1R0E1R0E1R0E1R0D1R0E1R0E1R0E1R0D1R0-A1R0-A1R0B3R1";
            var e = "-A1R0-B1R0C1R0D1R0E1R0F1R0E1R0F3R1A3R1B1R0A1R0F3R0E3R0E1R0E4R0";
            var str = a + b + b + c + c + b + c + d + e;
            return Parse(str, MMLType.Classical);
        }

        static public IEnumerable<MmlNote> Scale()
        {
            var notes = "CDEFGAB";
            var str = $"T120 L4 O0 {notes} O1 {notes} L5 O2 {notes} O3 {notes} L6 O4 {notes} O5 {notes} L7 O6 {notes} O7 {notes} L8 O8 {notes} O9 {notes}";
            return Parse(str, MMLType.Modern);
        }

        static public IEnumerable<MmlNote> NullSleep()
        {
            var notes = "c d e f g4 a16 b16 >c c d e f g4 a16 b16 >c<<";
            //var notes = "c >c g4 >c<<";
            var str = $"T120 L4 O4 {notes} {notes}";
            return Parse(str, MMLType.Modern);
        }
    }
}
