using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
using Windows.UI.Composition;
using Windows.UI.Input.Preview.Injection;

namespace BTUniversalKeyboard
{
    internal class BtUniversalKeyboardCommand
    {
        public enum Opcode
        {
            String = 1,
            Mouse = 2,
            VKey = 3,
            String_Start = 0x09,
        }

        private static void ReadToList(DataReader source, uint len, List<byte> destination)
        {
            var buffer = source.ReadBuffer(len);
            byte[] newByteArray;
            CryptographicBuffer.CopyToByteArray(buffer, out newByteArray);
            foreach (var b in newByteArray)
            {
                destination.Add(b);
            }
        }
        public static List<Object> Parse(byte[] value, List<Object> retval)
        {
            if (retval == null)
            {
                retval = new List<Object>();
            }
            var string_buffer = new List<byte>();
            var dr = DataReader.FromBuffer(value.AsBuffer());
            while (dr.UnconsumedBufferLength > 0)
            {
                var cmd = dr.ReadByte();
                var len = (uint)(cmd & 0x0f);
                var opcode = (Opcode)((cmd & 0xf0) >> 4);
                switch (opcode)
                {
                    case Opcode.Mouse:
                        {
                            // LINK: https://learn.microsoft.com/en-us/uwp/api/windows.ui.input.preview.injection.injectedinputmouseinfo
                            var click = dr.ReadByte();
                            for (int i = 1; i < len; i++) dr.ReadByte(); // Throw out the unused bytes.
                            InjectedInputMouseOptions optionDown = InjectedInputMouseOptions.None;
                            if ((click & 0x01) != 0) optionDown |= InjectedInputMouseOptions.LeftDown;
                            if ((click & 0x02) != 0) optionDown |= InjectedInputMouseOptions.MiddleDown;
                            if ((click & 0x04) != 0) optionDown |= InjectedInputMouseOptions.RightDown;
                            InjectedInputMouseOptions optionUp = InjectedInputMouseOptions.None;
                            if ((click & 0x01) != 0) optionUp |= InjectedInputMouseOptions.LeftUp;
                            if ((click & 0x02) != 0) optionUp |= InjectedInputMouseOptions.MiddleUp;
                            if ((click & 0x04) != 0) optionUp |= InjectedInputMouseOptions.RightUp;
                            retval.Add(new InjectedInputMouseInfo()
                            {
                                DeltaX = 0,
                                DeltaY = 0,
                                MouseOptions = optionDown
                            });
                            retval.Add(new InjectedInputMouseInfo()
                            {
                                DeltaX = 0,
                                DeltaY = 0,
                                MouseOptions = optionUp
                            });
                        }
                        break;
                    case Opcode.String_Start:
                        ReadToList(dr, len, string_buffer);
                        break;
                    case Opcode.String:
                        ReadToList(dr, len, string_buffer);
                        var str = CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, string_buffer.ToArray().AsBuffer());
                        string_buffer.Clear();
                        // LINK: https://learn.microsoft.com/en-us/uwp/api/windows.ui.input.preview.injection.injectedinputkeyboardinfo
                        foreach (var uchar in str)
                        {
                            switch (uchar)
                            {
                                case '\n':
                                    var ch = (char)0x0d; // VK_ENTER
                                    retval.Add(new InjectedInputKeyboardInfo()
                                    {
                                        KeyOptions = InjectedInputKeyOptions.None, // = Virtual Key
                                        VirtualKey = ch,
                                    });
                                    retval.Add(new InjectedInputKeyboardInfo()
                                    {
                                        KeyOptions = InjectedInputKeyOptions.None | InjectedInputKeyOptions.KeyUp,
                                        VirtualKey = ch,
                                    });
                                    break;
                                default:
                                    retval.Add(new InjectedInputKeyboardInfo()
                                    {
                                        KeyOptions = InjectedInputKeyOptions.Unicode,
                                        ScanCode = uchar,
                                    });
                                    retval.Add(new InjectedInputKeyboardInfo()
                                    {
                                        KeyOptions = InjectedInputKeyOptions.Unicode | InjectedInputKeyOptions.KeyUp,
                                        ScanCode = uchar,
                                    });
                                    break;
                            }
                        }
                        break;
                    case Opcode.VKey:
                        {
                            var ch = dr.ReadByte();
                            for (int i = 1; i < len; i++) dr.ReadByte(); // Throw out the unused bytes.
                            retval.Add(new InjectedInputKeyboardInfo()
                            {
                                KeyOptions = InjectedInputKeyOptions.None, // = Virtual Key
                                VirtualKey = ch,
                            });
                            retval.Add(new InjectedInputKeyboardInfo()
                            {
                                KeyOptions = InjectedInputKeyOptions.None | InjectedInputKeyOptions.KeyUp,
                                VirtualKey = ch,
                            });
                        }
                        break;
                }
            }
            return retval;
        }
    }
}
