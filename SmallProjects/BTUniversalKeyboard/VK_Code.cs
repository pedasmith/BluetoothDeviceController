using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace BTUniversalKeyboard
{
    class VK_Data
    {
        public string VK_Name { get; set; }
        public int VK_Code { get; set; }
        public string VK_Meaning { get; set; }
        public VK_Data(string vK_Name, int vK_Code, string vK_Meaning)
        {
            VK_Name = vK_Name;
            VK_Code = vK_Code;
            VK_Meaning = vK_Meaning;
        }
    }
    internal class VK_Code
    {
        public static VK_Data GetCode(int code)
        {
            InitializeIfNeeded();
            if (KeyData.ContainsKey(code))
            {
                return KeyData[code];
            }
            return new VK_Data("", code, $"Key {code:X2}");
        }

        private static void InitializeIfNeeded()
        {
            if (KeyData != null) return;
            KeyData = new Dictionary<int, VK_Data>();
            foreach (var item in RawKeyData)
            {
                if (KeyData.ContainsKey(item.VK_Code))
                {
                    System.Diagnostics.Debug.WriteLine($"ERROR: already got 0x{item.VK_Code:X2}");
                }
                else
                {
                    KeyData.Add(item.VK_Code, item);
                }
            }
            for (int i=0; i<10; i++)
            {
                var code = 0x30 + i;
                KeyData.Add(code, new VK_Data("", code, $"{i} key"));
            }
            for (int i = 0; i < 26; i++)
            {
                var code = 0x41 + i;
                char alpha = (char)(i + 'A');
                KeyData.Add(code, new VK_Data("", code, $"{alpha} key"));
            }
            var notacode = new List<int>() { 0x00 };
            var oem = new List<int>() { 0x92, 0x93, 0x94, 0x95, 0x96, 0xE1, 0xE3, 0xE4, 0xE6,
                0xE9, 0xEA, 0xEB, 0xEC, 0xED, 0xEE, 0xEF, 0xF0, 0xF1, 0xF2, 0xF3, 0xF4, 0xF5 };
            var undefined = new List<int>() { 0x07, 0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F, 0x40, 0x0E, 0x0F };
            var unassigned = new List<int>() { 0x88, 0x89, 0x8A, 0x8B, 0x8C, 0x8D, 0x8E, 0x8F,
                0x97, 0x98, 0x99, 0x9A, 0x9B, 0x9C, 0x9D, 0x9E, 0x9F,
                0xD8, 0xD9, 0xDA, 0xE8};
            var reserved = new List<int>() { 0x0A, 0x0B, 0x5E, 0xB8, 0xB9, 0xE0 };
            for (int i=0xC1; i<=0xD7; i++) reserved.Add(i);

            foreach (var code in notacode)
            {
                KeyData.Add(code, new VK_Data("", code, $"Not a code: 0x{code:X2}"));
            }
            foreach (var code in oem)
            {
                KeyData.Add(code, new VK_Data("", code, $"OEM specific 0x{code:X2}"));
            }
            foreach (var code in undefined)
            {
                KeyData.Add(code, new VK_Data("", code, $"Undefined 0x{code:X2}"));
            }
            foreach (var code in unassigned)
            {
                KeyData.Add(code, new VK_Data("", code, $"Unassigned 0x{code:X2}"));
            }
            foreach (var code in reserved)
            {
                KeyData.Add(code, new VK_Data("", code, $"Reserved 0x{code:X2}"));
            }

            int nerror = Verify(KeyData);
            ;
        }

        private static int Verify(Dictionary<int, VK_Data> map)
        {
            int nerror = 0;
            for (int i = 0; i < 0xFF; i++)
            {
                if (!map.ContainsKey(i))
                {
                    nerror++;
                    Log($"ERROR: VK_Code: code {i:X2} does not exist in the map");
                    continue;
                }
                var item = map[i];
                if (item.VK_Code != i)
                {
                    nerror++;
                    Log($"ERROR: VK_CODE: code {i:x2} item's code is {item.VK_Code:X2} but they should be the same.");
                }
            }

            return nerror;
        }

        private static void Log(string str)
        {
            System.Diagnostics.Debug.WriteLine(str);
            Console.WriteLine(str);
        }

        private static Dictionary<int, VK_Data> KeyData = null;
        private static List<VK_Data> RawKeyData = new List<VK_Data>()
        {
            // Data from https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes
            // Extracted 2023-05-07
            new VK_Data("VK_LBUTTON",   0x01,   "Left mouse button"),
            new VK_Data("VK_RBUTTON",   0x02,   "Right mouse button"),
            new VK_Data("VK_CANCEL",    0x03,   "Control-break processing"),
            new VK_Data("VK_MBUTTON",   0x04,   "Middle mouse button (three-button mouse)"),
            new VK_Data("VK_XBUTTON1",  0x05,   "X1 mouse button"),
            new VK_Data("VK_XBUTTON2",  0x06,   "X2 mouse button"),
            new VK_Data("VK_BACK",  0x08,   "BACKSPACE key"),
            new VK_Data("VK_TAB",   0x09,   "TAB key"),
            new VK_Data("VK_CLEAR", 0x0C,   "CLEAR key"),
            new VK_Data("VK_RETURN",    0x0D,   "ENTER key"),
            new VK_Data("VK_SHIFT", 0x10,   "SHIFT key"),
            new VK_Data("VK_CONTROL",   0x11,   "CTRL key"),
            new VK_Data("VK_MENU",  0x12,   "ALT key"),
            new VK_Data("VK_PAUSE", 0x13,   "PAUSE key"),
            new VK_Data("VK_CAPITAL",   0x14,   "CAPS LOCK key"),
            //new VK_Data("VK_KANA",  0x15,   "IME Kana mode"),
            //new VK_Data("VK_HANGUEL",   0x15,   "IME Hanguel mode (maintained for compatibility; use VK_HANGUL)"),
            new VK_Data("VK_HANGUL + VK_KANA",    0x15,   "IME Kana / Hangul mode"),
            new VK_Data("VK_IME_ON",    0x16,   "IME On"),
            new VK_Data("VK_JUNJA", 0x17,   "IME Junja mode"),
            new VK_Data("VK_FINAL", 0x18,   "IME final mode"),
            //new VK_Data("VK_HANJA", 0x19,   "IME Hanja mode"),
            //new VK_Data("VK_KANJI", 0x19,   "IME Kanji mode"),
            new VK_Data("VK_HANJA + VK_KANJI", 0x19,   "IME Hanja + Kanji mode"),
            new VK_Data("VK_IME_OFF",   0x1A,   "IME Off"),
            new VK_Data("VK_ESCAPE",    0x1B,   "ESC key"),
            new VK_Data("VK_CONVERT",   0x1C,   "IME convert"),
            new VK_Data("VK_NONCONVERT",    0x1D,   "IME nonconvert"),
            new VK_Data("VK_ACCEPT",    0x1E,   "IME accept"),
            new VK_Data("VK_MODECHANGE",    0x1F,   "IME mode change request"),
            new VK_Data("VK_SPACE", 0x20,   "SPACEBAR"),
            new VK_Data("VK_PRIOR", 0x21,   "PAGE UP key"),
            new VK_Data("VK_NEXT",  0x22,   "PAGE DOWN key"),
            new VK_Data("VK_END",   0x23,   "END key"),
            new VK_Data("VK_HOME",  0x24,   "HOME key"),
            new VK_Data("VK_LEFT",  0x25,   "LEFT ARROW key"),
            new VK_Data("VK_UP",    0x26,   "UP ARROW key"),
            new VK_Data("VK_RIGHT", 0x27,   "RIGHT ARROW key"),
            new VK_Data("VK_DOWN",  0x28,   "DOWN ARROW key"),
            new VK_Data("VK_SELECT",    0x29,   "SELECT key"),
            new VK_Data("VK_PRINT", 0x2A,   "PRINT key"),
            new VK_Data("VK_EXECUTE",   0x2B,   "EXECUTE key"),
            new VK_Data("VK_SNAPSHOT",  0x2C,   "PRINT SCREEN key"),
            new VK_Data("VK_INSERT",    0x2D,   "INS key"),
            new VK_Data("VK_DELETE",    0x2E,   "DEL key"),
            new VK_Data("VK_HELP",  0x2F,   "HELP key"),
            new VK_Data("VK_LWIN",  0x5B,   "Left Windows key (Natural keyboard)"),
            new VK_Data("VK_RWIN",  0x5C,   "Right Windows key (Natural keyboard)"),
            new VK_Data("VK_APPS",  0x5D,   "Applications key (Natural keyboard)"),
            new VK_Data("VK_SLEEP", 0x5F,   "Computer Sleep key"),
            new VK_Data("VK_NUMPAD0",   0x60,   "Numeric keypad 0 key"),
            new VK_Data("VK_NUMPAD1",   0x61,   "Numeric keypad 1 key"),
            new VK_Data("VK_NUMPAD2",   0x62,   "Numeric keypad 2 key"),
            new VK_Data("VK_NUMPAD3",   0x63,   "Numeric keypad 3 key"),
            new VK_Data("VK_NUMPAD4",   0x64,   "Numeric keypad 4 key"),
            new VK_Data("VK_NUMPAD5",   0x65,   "Numeric keypad 5 key"),
            new VK_Data("VK_NUMPAD6",   0x66,   "Numeric keypad 6 key"),
            new VK_Data("VK_NUMPAD7",   0x67,   "Numeric keypad 7 key"),
            new VK_Data("VK_NUMPAD8",   0x68,   "Numeric keypad 8 key"),
            new VK_Data("VK_NUMPAD9",   0x69,   "Numeric keypad 9 key"),
            new VK_Data("VK_MULTIPLY",  0x6A,   "Multiply key"),
            new VK_Data("VK_ADD",   0x6B,   "Add key"),
            new VK_Data("VK_SEPARATOR", 0x6C,   "Separator key"),
            new VK_Data("VK_SUBTRACT",  0x6D,   "Subtract key"),
            new VK_Data("VK_DECIMAL",   0x6E,   "Decimal key"),
            new VK_Data("VK_DIVIDE",    0x6F,   "Divide key"),
            new VK_Data("VK_F1",    0x70,   "F1 key"),
            new VK_Data("VK_F2",    0x71,   "F2 key"),
            new VK_Data("VK_F3",    0x72,   "F3 key"),
            new VK_Data("VK_F4",    0x73,   "F4 key"),
            new VK_Data("VK_F5",    0x74,   "F5 key"),
            new VK_Data("VK_F6",    0x75,   "F6 key"),
            new VK_Data("VK_F7",    0x76,   "F7 key"),
            new VK_Data("VK_F8",    0x77,   "F8 key"),
            new VK_Data("VK_F9",    0x78,   "F9 key"),
            new VK_Data("VK_F10",   0x79,   "F10 key"),
            new VK_Data("VK_F11",   0x7A,   "F11 key"),
            new VK_Data("VK_F12",   0x7B,   "F12 key"),
            new VK_Data("VK_F13",   0x7C,   "F13 key"),
            new VK_Data("VK_F14",   0x7D,   "F14 key"),
            new VK_Data("VK_F15",   0x7E,   "F15 key"),
            new VK_Data("VK_F16",   0x7F,   "F16 key"),
            new VK_Data("VK_F17",   0x80,   "F17 key"),
            new VK_Data("VK_F18",   0x81,   "F18 key"),
            new VK_Data("VK_F19",   0x82,   "F19 key"),
            new VK_Data("VK_F20",   0x83,   "F20 key"),
            new VK_Data("VK_F21",   0x84,   "F21 key"),
            new VK_Data("VK_F22",   0x85,   "F22 key"),
            new VK_Data("VK_F23",   0x86,   "F23 key"),
            new VK_Data("VK_F24",   0x87,   "F24 key"),
            new VK_Data("VK_NUMLOCK",   0x90,   "NUM LOCK key"),
            new VK_Data("VK_SCROLL",    0x91,   "SCROLL LOCK key"),
            new VK_Data("VK_LSHIFT",    0xA0,   "Left SHIFT key"),
            new VK_Data("VK_RSHIFT",    0xA1,   "Right SHIFT key"),
            new VK_Data("VK_LCONTROL",  0xA2,   "Left CONTROL key"),
            new VK_Data("VK_RCONTROL",  0xA3,   "Right CONTROL key"),
            new VK_Data("VK_LMENU", 0xA4,   "Left ALT key"),
            new VK_Data("VK_RMENU", 0xA5,   "Right ALT key"),
            new VK_Data("VK_BROWSER_BACK",  0xA6,   "Browser Back key"),
            new VK_Data("VK_BROWSER_FORWARD",   0xA7,   "Browser Forward key"),
            new VK_Data("VK_BROWSER_REFRESH",   0xA8,   "Browser Refresh key"),
            new VK_Data("VK_BROWSER_STOP",  0xA9,   "Browser Stop key"),
            new VK_Data("VK_BROWSER_SEARCH",    0xAA,   "Browser Search key"),
            new VK_Data("VK_BROWSER_FAVORITES", 0xAB,   "Browser Favorites key"),
            new VK_Data("VK_BROWSER_HOME",  0xAC,   "Browser Start and Home key"),
            new VK_Data("VK_VOLUME_MUTE",   0xAD,   "Volume Mute key"),
            new VK_Data("VK_VOLUME_DOWN",   0xAE,   "Volume Down key"),
            new VK_Data("VK_VOLUME_UP", 0xAF,   "Volume Up key"),
            new VK_Data("VK_MEDIA_NEXT_TRACK",  0xB0,   "Next Track key"),
            new VK_Data("VK_MEDIA_PREV_TRACK",  0xB1,   "Previous Track key"),
            new VK_Data("VK_MEDIA_STOP",    0xB2,   "Stop Media key"),
            new VK_Data("VK_MEDIA_PLAY_PAUSE",  0xB3,   "Play/Pause Media key"),
            new VK_Data("VK_LAUNCH_MAIL",   0xB4,   "Start Mail key"),
            new VK_Data("VK_LAUNCH_MEDIA_SELECT",   0xB5,   "Select Media key"),
            new VK_Data("VK_LAUNCH_APP1",   0xB6,   "Start Application 1 key"),
            new VK_Data("VK_LAUNCH_APP2",   0xB7,   "Start Application 2 key"),
            new VK_Data("VK_OEM_1", 0xBA,   "Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the ';:' key"),
            new VK_Data("VK_OEM_PLUS",  0xBB,   "For any country/region, the '+' key"),
            new VK_Data("VK_OEM_COMMA", 0xBC,   "For any country/region, the ',' key"),
            new VK_Data("VK_OEM_MINUS", 0xBD,   "For any country/region, the '-' key"),
            new VK_Data("VK_OEM_PERIOD",    0xBE,   "For any country/region, the '.' key"),
            new VK_Data("VK_OEM_2", 0xBF,   "Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '/?' key"),
            new VK_Data("VK_OEM_3", 0xC0,   "Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '`~' key"),
            new VK_Data("VK_OEM_4", 0xDB,   "Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '[{' key"),
            new VK_Data("VK_OEM_5", 0xDC,   "Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '\\|' key"),
            new VK_Data("VK_OEM_6", 0xDD,   "Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the ']}' key"),
            new VK_Data("VK_OEM_7", 0xDE,   "Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the 'single-quote/double-quote' key"),
            new VK_Data("VK_OEM_8", 0xDF,   "Used for miscellaneous characters; it can vary by keyboard."),
            new VK_Data("VK_OEM_102",   0xE2,   "The <> keys on the US standard keyboard, or the \\| key on the non-US 102-key keyboard"),
            new VK_Data("VK_PROCESSKEY",    0xE5,   "IME PROCESS key"),
            new VK_Data("VK_PACKET",    0xE7,   "Used to pass Unicode characters as if they were keystrokes. The VK_PACKET key is the low word of a 32-bit Virtual Key value used for non-keyboard input methods. For more information, see Remark in KEYBDINPUT, SendInput, WM_KEYDOWN, and WM_KEYUP"),
            new VK_Data("VK_ATTN",  0xF6,   "Attn key"),
            new VK_Data("VK_CRSEL", 0xF7,   "CrSel key"),
            new VK_Data("VK_EXSEL", 0xF8,   "ExSel key"),
            new VK_Data("VK_EREOF", 0xF9,   "Erase EOF key"),
            new VK_Data("VK_PLAY",  0xFA,   "Play key"),
            new VK_Data("VK_ZOOM",  0xFB,   "Zoom key"),
            new VK_Data("VK_NONAME",    0xFC,   "Reserved"),
            new VK_Data("VK_PA1",   0xFD,   "PA1 key"),
            new VK_Data("VK_OEM_CLEAR", 0xFE,   "Clear key"),
        };
    }
}
