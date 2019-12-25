using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothDeviceController.Charts
{
    /// <summary>
    /// Says what do do when there's too much data. Choices are to keep the most recent (RemoveFirst) or
    /// to remove data at random.
    /// </summary>
    public enum RemoveRecordAlgorithm { RemoveFirst, RemoveRandom };
    /// <summary>
    /// Says what happened when more data was added. AddSimple means it was added at the end; this can often
    /// make the graph update much much simpler. AddReplace means that the array had to shift. NotAdded means
    /// that the data didn't have to be added (and therefore the graph update is trivial :-) )
    /// </summary>
    public enum AddResult { AddSimple, AddReplace, NotAdded };
    public interface ISummarizeValue
    {
        string GetSummary(double ratio);
    }

    /// <summary>
    /// Always use AddRecord, not Add
    /// DataCollection is an ObservableCollection that resizes based on a user-selected algorithm (random or newest-wins)
    /// so that the size is always bounded. It's also an ISummarizeValue which is used by the charting functions.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataCollection<T> : ObservableCollection<T>, ISummarizeValue
    {
        public RemoveRecordAlgorithm RemoveAlgorithm = RemoveRecordAlgorithm.RemoveRandom;

        private int _MaxLength = 10000;// about 1 reading every 10 seconds for an entire day
        public int MaxLength { get { return _MaxLength; } set { if (value == _MaxLength) return; ResetSizeForNewMaxLength(value); _MaxLength = value; } }

        /// <summary>
        /// Gets a summary of the data at a 'ratio' (0..1) that corresponds to some point of actual data.
        /// </summary>
        /// <param name="ratio"></param>
        /// <returns></returns>
        public string GetSummary(double ratio)
        {
            int index = (int)Math.Floor (ratio * this.Count);
            if (index < 0) index = 0;
            if (index >= this.Count) return "";
            var item = this[index];

            string retval = "";
            foreach (var property in typeof(T).GetProperties())
            {
                try
                {
                    string str = "";
                    var value = property.GetValue(item);
                    if (value is DateTime dt)
                    {
                        str = dt.ToString("HH:mm:ss.f");
                    }
                    else if (value is UInt16 ui16)
                    {
                        str = ui16.ToString();
                    }
                    else if (value is Byte ui8)
                    {
                        str = ui8.ToString();
                    }
                    else
                    {
                        var dvalue = Convert.ToDouble(value);
                        str = dvalue.ToString("N3");
                    }
                    if (property.Name == "EventTime")
                    {
                        retval += $"At {str}";
                    }
                    else
                    {
                        retval += $"\n{property.Name}:\t {str}";
                    }
                }
                catch (Exception ex)
                {
                    retval += $"{property.Name}: Exception {ex.Message}\n";
                }
            }
            return retval;
        }
        private void ResetSizeForNewMaxLength(int newvalue)
        {
            // Must remove old values
            switch (RemoveAlgorithm)
            {
                case RemoveRecordAlgorithm.RemoveFirst:
                    while (newvalue < this.Count)
                    {
                        // There isn't a bulk remove capability (otehrwise I'd use it)
                        base.RemoveAt(0);
                    }
                    break;
                case RemoveRecordAlgorithm.RemoveRandom:
                    while (newvalue < this.Count)
                    {
                        var removeIndex = r.Next(0, this.Count);
                        base.RemoveAt(removeIndex);
                    }
                    break;
            }
        }
        private int NAdded = 0;
        private Random r = new Random();
        /// <summary>
        /// Adds the record into the ObservableCollection being mindful of the
        /// limitations (the array will only grow to MaxLength and the removal
        /// algorithm is picked by the user)
        /// </summary>
        /// <param name="item"></param>
        public AddResult AddRecord(T item)
        {
            if (this.Count < MaxLength)
            {
                // Just add to the end
                base.Add(item);
                NAdded++;
                return AddResult.AddSimple;
            }
            else
            {
                switch (RemoveAlgorithm)
                {
                    case RemoveRecordAlgorithm.RemoveFirst:
                        base.RemoveAt(0);
                        base.Add(item);
                        NAdded++;
                        return AddResult.AddReplace;
                    case RemoveRecordAlgorithm.RemoveRandom:
                        var removeIndex = r.Next(0, NAdded);
                        NAdded++;
                        if (removeIndex < base.Count)
                        {
                            // Remove random item from the middle and add
                            // the new item to the end.
                            base.RemoveAt(removeIndex);
                            base.Add(item);
                            return AddResult.AddReplace;
                        }
                        return AddResult.NotAdded;
                }
            }
            return AddResult.NotAdded;
        }

        public static int Test()
        {
            int NError = 0;
            NError += TestLargeToSmall();
            return NError;
        }

        private static int TestLargeToSmall()
        {
            int NError = 0;
            var data = new DataCollection<int>();
            data.MaxLength = 20;
            data.RemoveAlgorithm = RemoveRecordAlgorithm.RemoveFirst;
            for (int i=0; i<30; i++)
            {
                data.AddRecord(i);
            }

            // Data should be 10..29
            if (data.Count != 20)
            {
                NError++;
                System.Diagnostics.Debug.WriteLine($"ERROR: DataCollection simple.Count={data.Count} expected {20}");
            }
            for (int i = 0; i < data.Count; i++)
            {
                var expected = i + 10;
                var actual = data[i];
                if (expected != actual)
                {
                    NError++;
                    System.Diagnostics.Debug.WriteLine($"ERROR: DataCollection simple [{i}] expected {expected} actual {actual}");
                }
            }

            // Now set the size to 10. Data should be 19..29
            data.MaxLength = 10;
            if (data.Count != 10)
            {
                NError++;
                System.Diagnostics.Debug.WriteLine($"ERROR: DataCollection simple.Count={data.Count} expected {10}");
            }
            for (int i = 0; i < data.Count; i++)
            {
                var expected = i + 20;
                var actual = data[i];
                if (expected != actual)
                {
                    NError++;
                    System.Diagnostics.Debug.WriteLine($"ERROR: DataCollection simple [{i}] expected {expected} actual {actual}");
                }
            }

            return NError;
        }
    }
}
