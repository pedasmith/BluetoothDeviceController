
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
using Windows.UI.WebUI;
using Buffer = Windows.Storage.Streams.Buffer;

namespace BluetoothWatcher.BufferList
{
    public class BufferList
    {
        Queue<byte[]> DataList = new Queue<byte[]>();
        /// <summary>
        /// Is always 0..DataList[0].Length-1 in size. As soon as it's longer than DataLength, nuke it.
        /// </summary>
        int NextIndex = 0;

        public void Add(IBuffer additionalData)
        {
            lock (this)
            {
                if (additionalData.Length < 1) return;
                DataList.Enqueue(additionalData.ToArray());
            }
        }
        public void Add(byte[] additionalData)
        {
            lock (this)
            {
                if (additionalData.Length < 1) return;
                DataList.Enqueue(additionalData);
            }
        }



        public int GetNBuffer()
        {
            lock (this)
            {
                return DataList.Count();
            }
        }

        public int Length
        {
            get
            {
                int retval = 0;
                lock (this)
                {
                    foreach (var array in DataList)
                    {
                        retval += array.Length;
                    }
                    retval -= NextIndex;
                }
                return retval;
            }
        }

        public bool HasData { get { return DataList.Count > 0; } }

        public byte ReadByte()
        {
            lock (this)
            {
                if (DataList.Count < 1)
                {
                    throw new IndexOutOfRangeException($"BufferList: Asked for a byte, but there aren't any");
                }
                var array = DataList.Peek();
                var retval = array[NextIndex++];
                if (NextIndex >= array.Length)
                {
                    DataList.Dequeue();
                    NextIndex = 0;
                }
                return retval;
            }
        }

        public byte PeekByte()
        {
            lock (this)
            {
                if (DataList.Count < 1)
                {
                    throw new IndexOutOfRangeException($"BufferList: Asked for a byte, but there aren't any");
                }
                var array = DataList.Peek();
                var retval = array[NextIndex];
                return retval;
            }
        }

        public byte[] ToArray(int maxBytes = int.MaxValue)
        {
            lock (this)
            {
                var length = Length;
                if (length > maxBytes) length = maxBytes;
                var retval = new byte[length];
                for (int i=0; i<length; i++)
                {
                    retval[i] = ReadByte();
                }
                return retval;
            }
        }

        public IBuffer ToBuffer(int maxBytes = int.MaxValue)
        {
            var bytes = ToArray(maxBytes);
            return CryptographicBuffer.CreateFromByteArray(bytes);
        }



        private static int TestSimple()
        {
            int nerror = 0;
            var b = new BufferList();
            b.Add(new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04 });
            b.Add(new byte[] { 0x10, 0x11, 0x12, 0x13, 0x14 });
            b.Add(new byte[] { 0x20, 0x21, 0x22, 0x23, 0x24 });
            var b0 = b.ReadByte();
            var dr = DataReader.FromBuffer(b.ToBuffer(8)); // Should skip the first byte
            var b1 = dr.ReadByte();
            var i2340 = dr.ReadInt32();
            var s12 = dr.ReadInt16();
            var b3 = dr.ReadByte();
            var nbuff2 = b.GetNBuffer(); // The first two buffers should be gone at this point!
            var b4 = b.ReadByte();
            var nbuff1 = b.GetNBuffer(); // The first two buffers should be gone at this point!

            if (b0 != 0x00) { Log($"BufferList: Error: b0 was {b0} expected 0x00"); nerror++; }
            if (b1 != 0x01) { Log($"BufferList: Error: b1 was {b1} expected 0x01"); nerror++; }
            if (i2340 != 0x02030410) { Log($"BufferList: Error: i2340 was {i2340:X8} expected 0x02030410"); nerror++; }
            if (s12!= 0x1112) { Log($"BufferList: Error: s12 was {s12:X4} expected 0x1112"); nerror++; }
            if (b3 != 0x13) { Log($"BufferList: Error: b3 was {b3} expected 0x13"); nerror++; }
            if (nbuff2 != 2) { Log($"BufferList: Error: nbuff2 was {nbuff2} expected 2"); nerror++; }
            if (b4 != 0x14) { Log($"BufferList: Error: b4 was {b4} expected 0x14"); nerror++; }
            if (nbuff1 != 1) { Log($"BufferList: Error: nbuff1 was {nbuff1} expected 1"); nerror++; }

            return nerror;
        }
        private static void Log(string str)
        {
            System.Diagnostics.Debug.WriteLine(str);
        }
        public static int Test()
        {
            int nerror = 0;
            nerror += TestSimple();
            return nerror;
        }

        class BufferListResults : IAsyncOperationWithProgress<IBuffer, uint>
        {
            Buffer retval = null;
            public BufferListResults(Buffer b)
            {
                retval = b;
            }
            public IBuffer GetResults()
            {
                return retval;
            }

            public AsyncOperationProgressHandler<IBuffer, uint> Progress { get; set; }
            public AsyncOperationWithProgressCompletedHandler<IBuffer, uint> Completed { get; set; }

            public void Cancel()
            {
            }

            public void Close()
            {
            }

            public System.Exception ErrorCode => null;

            public uint Id => 0;

            public AsyncStatus Status => AsyncStatus.Completed;
        }
    }
}
