using BCBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCBasic
{
    public class BCValueList : IObjectValue
    {
        // Fields to handle the 
        private int MaxCount = int.MaxValue;
        private enum RemoveAlgorithm {  First, Last, Random };
        private RemoveAlgorithm CurrRemoveAlgorithm = RemoveAlgorithm.Random;
        private int TotalAdds = 0;
        private Random r = new Random();

        // NOTE: should I combine MaxIndex and MaxCount? They are almost
        // the same (MaxIndex also sets the array to that size, but MaxCount
        // does not)
        public int MaxIndex = -1;
        public bool ReadOnly = true;
        private ObservableCollection<BCValue> _data = new ObservableCollection<BCValue>();
        public ObservableCollection<BCValue> data
        {
            get
            {
                if (_floatArray != null)
                {
                    // Note: this is never efficient!
                    for (int i=_data.Count; i<_floatArray.Length; i++)
                    {
                        _data.Add(new BCValue (_floatArray[i]));
                    }
                }
                return _data;
            }
        }

        float[] _floatArray = null;
        public float[] floatArray
        {
            get
            {
                return _floatArray;
            }
            set
            {
                _floatArray = value;
            }
        }

        public float[] AsFloatArray()
        {
            if (_floatArray == null && _data == null) return null;
            if (_floatArray != null) return _floatArray;
            float[] Retval = new float[data.Count];
            for (int i = 0; i < data.Count; i++)
            {
                Retval[i] = (float)data[i].AsDouble;
            }
            return Retval;
        }


        byte[] _byteArray = null;
        public byte[] AsByteArray()
        {
            if (_byteArray == null && _data == null) return null;
            if (_byteArray != null) return _byteArray;
            byte[] Retval = new byte[data.Count];
            for (int i = 0; i < data.Count; i++)
            {
                Retval[i] = (byte)data[i].AsDouble;
            }
            return Retval;
        }


        public BCValueList()
        {
            PreferredName = "BCValueList";
        }

        public BCValueList(int maxIndex)
        {
            PreferredName = "BCValueList";
            MaxIndex = maxIndex;
            for (int i=0; i<maxIndex; i++)
            {
                data.Add (new BCValue());
            }
        }

        // Fills in a BCValueList with a double[] array
        public BCValueList(double[] input)
        {
            for (int i = 0; i < input.Count(); i++)
            {
                data.Add(new BCValue(input[i]));
            }
        }

        public BCValueList(float[] input)
        {
            _floatArray = input;
        }

        public BCValueList(string name, IList<string> propertyNames)
        {
            PreferredName = name;
            PropertyNames = new Dictionary<string, int>();
            for (int i=0; i<propertyNames.Count; i++)
            {
                PropertyNames[propertyNames[i]] = i;
            }
        }

        // Always add the property at the end of data.  If the name is duplicated, don't update the name's index (will have the first value)
        public void AddPropertyAllowDuplicates(string name, BCValue value)
        {
            var idx = data.Count;
            data.Add(value);
            if (PropertyNames == null) PropertyNames = new Dictionary<string, int>();
            if (!PropertyNames.ContainsKey(name))
            {
                PropertyNames.Add(name, idx);
            }
        }
        public void SetProperty(string name, BCValue value)
        {
            switch (name)
            {
                case "MaxCount":
                    MaxCount = value.AsInt;
                    break;
                case "RemoveAlgorithm":
                    switch (value.AsString)
                    {
                        case "First": CurrRemoveAlgorithm = RemoveAlgorithm.First; break;
                        case "Last": CurrRemoveAlgorithm = RemoveAlgorithm.Last; break;
                        case "Random": CurrRemoveAlgorithm = RemoveAlgorithm.Random; break;
                    }
                    break;
                default:
                    if (PropertyNames != null && PropertyNames.ContainsKey(name))
                    {
                        var idx = PropertyNames[name];
                        data[idx] = value;
                    }
                    else
                    {
                        var idx = data.Count;
                        data.Add(value);
                        if (PropertyNames == null) PropertyNames = new Dictionary<string, int>();
                        PropertyNames.Add(name, idx);
                        // Alternative: I could have just invalidated the cache.
                        if (cachedPropertyNames != null)
                        {
                            cachedPropertyNames.Add(name);
                        }
                    }
                    break;
            }
        }

        Dictionary<string, int> PropertyNames = null;

        public string PreferredName { get; set; }

        IList<string> cachedPropertyNames = null;

        public IList<string> GetNames() {
            if (cachedPropertyNames == null)
            {
                cachedPropertyNames = new List<string>() { "Count", "Max", "MaxCount", "Methods", "Min", "RemoveAlgorithm" };
                if (PropertyNames != null)
                {
                    foreach (var entry in PropertyNames)
                    {
                        cachedPropertyNames.Add(entry.Key);
                    }
                }
            }
            return cachedPropertyNames;
        }
        public double Max
        {
            get
            {
                if (_floatArray != null)
                {
                    if (_floatArray.Length == 0) return double.NaN;
                    var value = _floatArray[0];
                    foreach (var item in _floatArray)
                    {
                        if (item > value) value = item;
                    }
                    return value;

                }
                else
                {
                    if (_data.Count == 0) return double.NaN;
                    var value = _data[0].AsDouble;
                    foreach (var item in _data)
                    {
                        if (item.AsDouble > value) value = item.AsDouble;
                    }
                    return value;
                }
            }
        }
        public double Min
        {
            get
            {
                if (_floatArray != null)
                {
                    if (_floatArray.Length == 0) return double.NaN;
                    var value = _floatArray[0];
                    foreach (var item in _floatArray)
                    {
                        if (item < value) value = item;
                    }
                    return value;

                }
                else
                {
                    if (_data.Count == 0) return double.NaN;
                    var value = _data[0].AsDouble;
                    foreach (var item in _data)
                    {
                        if (item.AsDouble < value) value = item.AsDouble;
                    }
                    return value;
                }
            }
        }
        public Tuple<double,double> GetMinMaxOf(int col)
        {
            bool anySet = false;
            double minval = double.MaxValue;
            double maxval = double.MinValue;
            foreach (var row in data)
            {
                if (row.IsArray && row.AsArray.data.Count >= col)
                {
                    var item = row.AsArray.data[col];
                    if (item.AsDouble < minval) { anySet = true; minval = item.AsDouble; }
                    if (item.AsDouble > maxval) { anySet = true; maxval = item.AsDouble; }
                }
                // Not array?  Just skip over it.
            }
            if (!anySet) { minval = double.NaN; maxval = double.NaN; }
            return new Tuple<double,double>(minval, maxval);
        }

        public double Mean
        {
            get
            {
                if (_floatArray != null)
                {
                    if (_floatArray.Length == 0) return double.NaN;
                    var value = 0.0;
                    foreach (var item in _floatArray)
                    {
                        value += item;
                    }
                    return value / _floatArray.Length;

                }
                else
                {
                    if (_data.Count == 0) return double.NaN;
                    var value = 0.0;
                    foreach (var item in _data)
                    {
                        var itemValue = item.AsDouble;
                        value += itemValue;
                    }
                    return value / _data.Count;
                }
            }
        }
        public double SumOfSquares
        {
            get
            {
                if (_floatArray != null)
                {
                    if (_floatArray.Length == 0) return double.NaN;
                    var value = 0.0;
                    foreach (var item in _floatArray)
                    {
                        value += (item * item);
                    }
                    return value;

                }
                else
                {
                    if (_data.Count == 0) return double.NaN;
                    var value = 0.0;
                    foreach (var item in _data)
                    {
                        var itemValue = item.AsDouble;
                        value += itemValue * itemValue;
                    }
                    return value;
                }
            }
        }

        public BCValue GetValue(string name)
        {
            switch (name)
            {
                case "Count":
                    if (this._floatArray != null) return new BCBasic.BCValue(_floatArray.Length);
                    return new BCValue(data.Count);
                case "Methods":
                    var set = ReadOnly ? "" : "Set,";
                    return new BCBasic.BCValue($"Fill,Get,GetValue,{set}ToString");
                case "Max": return new BCValue(Max);
                case "MaxCount": return new BCValue(MaxCount);
                case "Mean": return new BCValue(Mean);
                case "Min": return new BCValue(Min);
                case "RemoveAlgorithm": return new BCValue(CurrRemoveAlgorithm.ToString());
                case "SumOfSquares": return new BCValue(SumOfSquares);
                case "Type":
                    return new BCValue("ARRAY");
                default:
                    if (PropertyNames != null && PropertyNames.ContainsKey(name))
                    {
                        var index = PropertyNames[name];
                        if (index >= 0 && index < data.Count)
                        {
                            return data[index];
                        }
                    }
                    break;
            }
            return BCValue.MakeNoSuchProperty(this, name);
        }

        public void InitializeForRun()
        {
        }

        public void RunComplete()
        {
        }

        public void SetValue(string name, BCValue value)
        {
            switch (name)
            {
                case "MaxCount":
                    MaxCount = value.AsInt;
                    break;
                case "RemoveAlgorithm":
                    switch (value.AsString)
                    {
                        case "First": CurrRemoveAlgorithm = RemoveAlgorithm.First; break;
                        case "Last": CurrRemoveAlgorithm = RemoveAlgorithm.Last; break;
                        case "Random": CurrRemoveAlgorithm = RemoveAlgorithm.Random; break;
                    }
                    break;
            }
        }

        public double GetValue(int index, string type)
        {
            double Retval = double.NaN;
            index = index - 1; // Input is 1-based; data is 0-based
            try
            {
                switch (type)
                {
//#if NEVER_EVER_DEFINED
                    // Written but not tested
                    case "uint8": // 8 bits, unsigned integer
                        {
                            var b1 = (byte)data[index].AsInt;
                            Retval = b1;
                        }
                        break;
                    case "int8": // 8 bits, unsigned integer
                        {
                            var b1 = (sbyte)data[index].AsInt;
                            Retval = b1;
                        }
                        break;

//#endif
                    case "int16-le": // 16 bits, signed integer, little-endian
                        {
                            var b1 = (int)(byte)data[index].AsInt;
                            var b2 = (int)(sbyte)(byte)(data[index + 1].AsInt);
                            Retval = b1 + (b2 * 256);
                        }
                        break;
                }
            }
            catch (Exception)
            {
                // Just in case the user goes over the end of the array.
            }
            return Retval;

        }

        public RunResult.RunStatus GetAt(int idx1, int idx2, BCValue Retval)
        {
            if (!BCObjectUtilities.CheckValueRange ("Array.GetAt", idx1, "index", 1, data.Count, Retval)) return RunResult.RunStatus.ErrorStop;

            var row = data[idx1 - 1];
            if (idx2 < 0) // No second index
            {
                Retval.CopyFrom(data[idx1 - 1]); // idx is 1-based but data is zero-based.
            }
            else
            {
                if (row.IsArray)
                {
                    var data2 = row.AsArray.data;
                    if (!BCObjectUtilities.CheckValueRange("Array.GetAt", idx2, "index2", 1, data2.Count, Retval)) return RunResult.RunStatus.ErrorStop;
                    Retval.CopyFrom(data2[idx2 - 1]); // idx is 1-based but data is zero-based.
                }
                else
                {
                    Retval.SetError(1, $"Array.GetAt[{idx1},{idx2}] is a 1-dimensional array");
                    return RunResult.RunStatus.ErrorStop;
                }
            }
            return RunResult.RunStatus.OK;
        }

        public void Append(BCValue value)
        {
            if (data.Count + 1 <= MaxCount)
            {
                data.Add(value); // Create as IsNoValue
            }
            else
            {
                switch (this.CurrRemoveAlgorithm)
                {
                    case RemoveAlgorithm.Last:
                        // Literally does nothing.  The array is full, and we
                        // don't add anything new.
                        break;
                    case RemoveAlgorithm.First:
                        data.RemoveAt(0);
                        data.Add(value);
                        break;
                    case RemoveAlgorithm.Random:
                        var idx = r.Next(TotalAdds);
                        if (idx < data.Count)
                        {
                            data.RemoveAt(idx);
                            data.Add(value);
                        }
                        break;
                }
            }
        }

        public async Task<RunResult.RunStatus> RunAsync(BCRunContext context, string name, IList<IExpression> ArgList, BCValue Retval)
        {
            switch (name)
            {
                case "Add":
                    {
                        // Adds to the end using the RemoveAlgorithm if the array is too large.
                        if (!BCObjectUtilities.CheckArgs(1, 9999, name, ArgList, Retval)) return RunResult.RunStatus.ErrorStop;

                        for (int i = 0; i < ArgList.Count; i++)
                        {
                            var value = await ArgList[i].EvalAsync(context);
                            TotalAdds++; // even if we don't actually add the data in
                            Append(value);
                        }
                        return RunResult.RunStatus.OK;
                    }
                case "AddRow":
                    {
                        var row = new BCValueList();
                        foreach (var item in ArgList)
                        {
                            var value = (await item.EvalAsync(context)).Duplicate();
                            row.data.Add(value);
                        }
                        Retval.AsObject = row;
                        data.Add(Retval);
                        return RunResult.RunStatus.OK;
                    }
                case "Clear":
                    // Remove all objects from the array.
                    data.Clear();
                    return RunResult.RunStatus.OK;

                case "Fill":
                    {
                        if (!BCObjectUtilities.CheckArgs(1, 2, name, ArgList, Retval)) return RunResult.RunStatus.ErrorStop;
                        var value = (await ArgList[0].EvalAsync(context));
                        for (int i=0; i<data.Count; i++)
                        {
                            var cell = data[i];
                            if (cell.IsArray)
                            {
                                var row = cell.AsArray;
                                for (int j=0; j<row.data.Count; j++)
                                {
                                    row.data[j].CopyFrom(value);
                                }
                            }
                            else
                            {
                                cell.CopyFrom(value);
                            }
                        }

                        return RunResult.RunStatus.OK;
                    }

                case "Get":
                    {
                        if (!BCObjectUtilities.CheckArgs(1, 2, name, ArgList, Retval)) return RunResult.RunStatus.ErrorStop;
                        var idx1 = (await ArgList[0].EvalAsync(context)).AsInt;
                        var idx2 = ArgList.Count <= 1 ? -1: (await ArgList[1].EvalAsync(context)).AsInt;
                        return GetAt(idx1, idx2, Retval);
                        //Retval.CopyFrom(data[idx - 1]); // idx is 1-based but data is zero-based.
                        //return RunResult.RunStatus.OK;
                    }

                case "GetValue":
                    {
                        if (!BCObjectUtilities.CheckArgs(2, 2, name, ArgList, Retval)) return RunResult.RunStatus.ErrorStop;
                        if (!await BCObjectUtilities.CheckArgValue(0, "index", 1, data.Count, context, name, ArgList, Retval)) return RunResult.RunStatus.ErrorStop;
                        var idx = (await ArgList[0].EvalAsync(context)).AsInt;
                        var type = (await ArgList[1].EvalAsync(context)).AsString;
                        var dvalue = GetValue(idx, type);
                        Retval.AsDouble = dvalue;
                        return RunResult.RunStatus.OK;
                    }
                case "HasKey":
                    {
                        if (!BCObjectUtilities.CheckArgs(1, 1, name, ArgList, Retval)) return RunResult.RunStatus.ErrorStop;
                        var key = (await ArgList[0].EvalAsync(context)).AsString;
                        Retval.AsDouble = this.PropertyNames != null && this.PropertyNames.ContainsKey(key) ? 1.0 : 0.0;
                        return RunResult.RunStatus.OK;
                    }
                case "RemoveAt":
                    {
                        if (!BCObjectUtilities.CheckArgs(1, 1, name, ArgList, Retval)) return RunResult.RunStatus.ErrorStop;
                        if (!await BCObjectUtilities.CheckArgValue(0, "index", 1, data.Count, context, name, ArgList, Retval)) return RunResult.RunStatus.ErrorStop;

                        var idx = (await ArgList[0].EvalAsync(context)).AsInt;
                        var status = GetAt(idx, -1, Retval);
                        data.RemoveAt(idx-1); // BC BASIC is 1..Count but the underlying data is 0..count-1
                        return status;
                    }
                case "MaxOf":
                    {
                        if (!BCObjectUtilities.CheckArgs(1, 1, name, ArgList, Retval)) return RunResult.RunStatus.ErrorStop;
                        var column = (await ArgList[0].EvalAsync(context)).AsInt;
                        var value = GetMinMaxOf(column-1); //BASIC is 1-based; GetMinMaxOf is 0-based
                        Retval.AsDouble = value.Item2;
                        return RunResult.RunStatus.OK;
                    }

                case "MinOf":
                    {
                        if (!BCObjectUtilities.CheckArgs(1, 1, name, ArgList, Retval)) return RunResult.RunStatus.ErrorStop;
                        var column = (await ArgList[0].EvalAsync(context)).AsInt;
                        var value = GetMinMaxOf(column-1); //BASIC is 1-based; GetMinMaxOf is 0-based
                        Retval.AsDouble = value.Item1;
                        return RunResult.RunStatus.OK;
                    }

                case "Set": // data.Set(3, <null>, "this is my third item!") or data.Set (3, 4, "item [3,4]")
                    {
                        // Max length is 1..9999
                        // NOTE: what's the real maximum index for an array?  9999? Should it be settable?
                        if (!BCObjectUtilities.CheckArgs(3, 3, name, ArgList, Retval)) return RunResult.RunStatus.ErrorStop;
                        if (MaxIndex >= 0)
                        {
                            if (!await BCObjectUtilities.CheckArgValue(0, "index1", 1, MaxIndex, context, name, ArgList, Retval)) return RunResult.RunStatus.ErrorStop;
                        }

                        if (!await BCObjectUtilities.CheckArgValue(0, "index", 1, 9999, context, name, ArgList, Retval)) return RunResult.RunStatus.ErrorStop;
                        var idx1 = (await ArgList[0].EvalAsync(context)).AsInt;

                        var value = await ArgList[2].EvalAsync(context);
                        for (int i = data.Count; i < idx1; i++)
                        {
                            // Add either as a IsNoValue or as a BCValueList if it's a 2D array.
                            var newvalue = new BCValue();
                            if (ArgList[1] != null) newvalue.AsObject = new BCValueList();
                            data.Add(newvalue); 
                        }

                        if (ArgList[1] != null)
                        {
                            // Am trying to set a 2D array element
                            var idx2 = (await ArgList[1].EvalAsync(context)).AsInt;
                            var row = data[idx1 - 1];
                            if (!row.IsArray)
                            {
                                Retval.SetError(25, $"{name} is not a 2D array");
                                return RunResult.RunStatus.ErrorStop;
                            }
                            else if (idx2 < 1)
                            {
                                Retval.SetError(25, $"{name}[{idx1},{idx2}] second index must be 1 or more");
                                return RunResult.RunStatus.ErrorStop;
                            }
                            var data2 = row.AsArray.data;
                            // Expand as needed.
                            for (int i = data2.Count; i < idx2; i++)
                            {
                                // Add as a IsNoValue.
                                var newvalue = new BCValue();
                                data.Add(newvalue);
                            }
                            data2[idx2 - 1] = value;
                        }
                        else
                        {
                            data[idx1 - 1] = value;
                        }
                        return RunResult.RunStatus.OK;
                    }
                case "SetProperty":
                    // Adds a name/value pair
                    {
                        if (!BCObjectUtilities.CheckArgs(2, 2, name, ArgList, Retval)) return RunResult.RunStatus.ErrorStop;
                        var dataName = (await ArgList[0].EvalAsync(context)).AsString;
                        var dataValue = await ArgList[1].EvalAsync(context);
                        SetProperty(dataName, dataValue);
                    }
                    return RunResult.RunStatus.OK;
                case "ToString":
                    Retval.AsString = $"{PreferredName} Length={data.Count}";
                    return RunResult.RunStatus.OK;

                default:
                    await Task.Delay(0);
                    BCValue.MakeNoSuchMethod(Retval, this, name);
                    return RunResult.RunStatus.ErrorContinue;
            }
        }

        public void Dispose()
        {
        }

        private static string AsSuperscript(string str)
        {
            string Retval = "";
            foreach (var ch in str)
            {
                char newch = ch;
                switch (ch)
                {
                    case '0': newch = '⁰'; break;
                    case '1': newch = '¹'; break;
                    case '2': newch = '²'; break;
                    case '3': newch = '³'; break;
                    case '4': newch = '⁴'; break;
                    case '5': newch = '⁵'; break;
                    case '6': newch = '⁶'; break;
                    case '7': newch = '⁷'; break;
                    case '8': newch = '⁸'; break;
                    case '9': newch = '⁹'; break;
                }
                Retval += newch;

            }
            return Retval;
        }
        public override string ToString()
        {
            var str = "";
            if (data.Count > 5)
            {
                var last = data.Count - 1;
                str = $"{data[0].ToString()},{data[1].ToString()},...,{data[last-1].ToString()},{data[last].ToString()}";
            }
            else
            {
                foreach (var item in data)
                {
                    if (str != "") str += ",";
                    if (item.IsArray)
                    {
                        var len = item.AsArray.data.Count.ToString();
                        len = BCValueList.AsSuperscript(len);
                        str += $"[{len}]";
                    }
                    else str += item.ToString();
                }
            }
            return $"[{str}]";
        }
    }
}
