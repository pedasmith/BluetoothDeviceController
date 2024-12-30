# CopyBCValueCSFile FileName=BCValue.cs DirName=CsBluetoothFoundation SuppressFile=:SuppressCSharpProtocol:

Copy of the BCValue.cs file. Does not require any customization per-BT device.


```
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace BCBasic
{
    public interface IRTLDateTime
    {
        double GetCurrentTotalSeconds();
    }

    public class BCValue : IObjectValue
    {
        public BCValue(BCValue src)
        {
            this.DoubleValue = src.DoubleValue;
            this.StringValue = src.StringValue;
            this.ObjectValue = src.ObjectValue;
            this.CurrentType = src.CurrentType;
        }
        public BCValue(bool value)
        {
            DoubleValue = value ? 1.0 : 0.0;
            CurrentType = ValueType.IsDouble;
        }
        public BCValue(byte[] value)
        {
            var vlist = new BCValueList();
            foreach (var b in value)
            {
                vlist.data.Add(new BCValue(b));
            }
            ObjectValue = vlist;
            CurrentType = ValueType.IsObject;
        }
        public BCValue(double value)
        {
            DoubleValue = value;
            CurrentType = ValueType.IsDouble;
        }
        public BCValue(string value)
        {
            StringValue = value;
            CurrentType = ValueType.IsString;
        }
        public BCValue(IObjectValue value)
        {
            ObjectValue = value;
            CurrentType = ValueType.IsObject;
        }

        public BCValue()
        {
            StringValue = "";
            CurrentType = ValueType.IsNoValue;
        }

        public static BCValue MakeSpecialNoValue(double code)
        {
            BCValue Retval = new BCValue(); // NoValue
            Retval.DoubleValue = code;
            return Retval;
        }

        public static BCValue MakeError(double code, string error)
        {
            BCValue Retval = new BCValue();
            Retval.SetError(code, error);
            return Retval;
        }

        public const int NO_SUCH_PROPERTY = 999413;
        public const int NO_SUCH_METHOD = 999419;
        public static BCValue MakeNoSuchProperty(IObjectValue obj, string propertyName, double code = NO_SUCH_PROPERTY)
        {
            BCValue Retval = new BCValue();
            Retval.SetError(code, $"Object type {obj.PreferredName} has no property {propertyName}");
            return Retval;
        }

        public static BCValue MakeNoSuchMethod(BCValue Retval, IObjectValue obj, string methodName, double code = NO_SUCH_METHOD)
        {
            Retval.SetError(code, $"Object type {obj.PreferredName} has no method {methodName}");
            return Retval;
        }

        public static BCValue MakeFrom(BCValue source)
        {
            var Retval = new BCValue();
            Retval.CopyFrom(source);
            return Retval;
        }

        public BCValue Duplicate()
        {
            var Retval = new BCValue();
            Retval.CopyFrom(this);
            return Retval;
        }

        public void CopyFrom(BCValue source)
        {
            switch (source.CurrentType)
            {
                case ValueType.IsDouble:
                    this.AsDouble = source.AsDouble;
                    break;
                case ValueType.IsNoValue:
                    this.SetNoValue();
                    break;
                case ValueType.IsObject:
                    this.AsObject = source.AsObject;
                    break;
                case ValueType.IsString:
                    this.AsString = source.AsString;
                    break;
            }
        }

        public void SetNoValue()
        {
            DoubleValue = Double.NaN;
            AsObject = null;
            CurrentType = ValueType.IsNoValue;
        }

        public enum ValueType { IsNoValue, IsDouble, IsString, IsObject, IsError };
        public ValueType CurrentType { get; internal set; }
        public bool IsArray {  get { return (CurrentType == ValueType.IsObject) && (AsObject is BCValueList); } }

        double DoubleValue = Double.NaN;
        string StringValue;
        IObjectValue ObjectValue = null;

        public int AsInt
        {
            get
            {
                var d = AsDouble;
                if (double.IsNaN(d)) return int.MinValue;
                return (int)d;
            }
        }

        public double AsDouble
        {
            get
            {
                double retval = DoubleValue;
                switch (CurrentType)
                {
                    case ValueType.IsObject:
                        {
                            var obj = this.AsObject;
                            if (obj is IRTLDateTime)
                            {
                                var dt = obj as IRTLDateTime;
                                DoubleValue = dt.GetCurrentTotalSeconds();
                                retval = DoubleValue;
                            }
                        }
                        //NOTE: do something better, like call ToValue("something")?
                        break;
                    case ValueType.IsString:
                        DoubleValue = Double.NaN;
                        var ok = Double.TryParse(StringValue, out DoubleValue);
                        if (!ok) DoubleValue = Double.NaN;
                        retval = DoubleValue;
                        break;
                    case ValueType.IsNoValue:
                        retval = Double.NaN;
                        DoubleValue = retval;
                        break;
                    case ValueType.IsError:
                        // Gets the error code as a number.
                        break;
                }
                return retval;
            }
            set
            {
                switch (CurrentType)
                {
                    case ValueType.IsObject:
                        //NOTE: call SetValue
                        break;
                    default:
                        DoubleValue = value;
                        CurrentType = ValueType.IsDouble;
                        break;
                }
            }
        }

        public IObjectValue AsObject
        {
            get
            {
                switch (CurrentType)
                {
                    case ValueType.IsObject:
                        return ObjectValue;
                    default:
                        return null;
                }
            }
            set
            {
                ObjectValue = value;
                CurrentType = ValueType.IsObject;
            }
        }

        public BCValueList AsArray { get { if (IsArray) return AsObject as BCValueList; return null; } }

        public string AsString
        {
            get
            {
                switch (CurrentType)
                {
                    case ValueType.IsObject:
                        StringValue = ObjectValue == null ? "" : ObjectValue.ToString();
                        break;
                    case ValueType.IsDouble:
                        StringValue = DoubleValue.ToString();
                        break;
                    case ValueType.IsNoValue:
                        StringValue = "(not set)";
                        break;
                    case ValueType.IsError:
                        if (StringValue == null)
                        {
                            StringValue = $"Error code: {DoubleValue}"; 
                        }
                        break;
                }
                return StringValue;
            }
            set
            {
                switch (CurrentType)
                {
                    case ValueType.IsObject:
                        //TODO: call SetValue
                        break;
                    default:
                        StringValue = value;
                        CurrentType = ValueType.IsString;
                        break;
                }
            }
        }
        public static string ReplaceNewline(string str)
        {
            var Retval = str.Replace("\\n", "\n").Replace("\\r", "\r");
            return Retval;
        }

        public bool AsDoubleToBoolean
        {
            get
            {
                var value = AsDouble;
                bool Retval = !(value == 0);

                // XPATH booleans are false https://msdn.microsoft.com/en-us/library/ms256159(v=vs.110).aspx
                // The C# rule is true https://msdn.microsoft.com/en-us/library/ms256159(v=vs.110).aspx
                // C boolean are true 
                if (Double.IsNaN(value)) Retval = true; // Python and C both use NaN-->true because it's not equal to zero.

                return Retval;
            }
        }

        public string PreferredName { get { return "variable"; } }

        public void SetError (double code, string error)
        {
            CurrentType = ValueType.IsError;
            DoubleValue = code;
            StringValue = error;
        }

        public override string ToString()
        {
            return AsString;
        }

        public BCValue GetValue(string name)
        {
            switch (name)
            {
                case "ErrorCode": return new BCValue(this.CurrentType == ValueType.IsError ? this.AsDouble : 0);
                case "ErrorString": return new BCValue(this.CurrentType == ValueType.IsError ? this.AsString : "No error");

                case "IsError": return new BCValue(this.CurrentType == ValueType.IsError);
                case "IsNaN": return new BCValue(this.CurrentType == ValueType.IsDouble  ? Double.IsNaN (this.DoubleValue) : false);
                case "IsNumber": return new BCValue(this.CurrentType == ValueType.IsDouble);
                case "IsObject": return new BCValue(this.CurrentType == ValueType.IsObject);
                case "IsString": return new BCValue(this.CurrentType == ValueType.IsString);
                case "Type":
                    switch (this.CurrentType)
                    {
                        case ValueType.IsDouble: return new BCValue ("NUMBER");
                        case ValueType.IsError: return new BCValue ("ERROR");
                        case ValueType.IsNoValue: return new BCValue("NOTHING");
                        case ValueType.IsObject: return new BCValue("OBJECT");
                        case ValueType.IsString: return new BCValue("STRING");
                        default:
                            return new BCValue("VARIABLE");
                    }
            }
            return BCValue.MakeNoSuchProperty(this, name);
        }

        public void SetValue(string name, BCValue value)
        {
            // Can't set any of the values.
        }

        public IList<string> GetNames()
        {
            return new List<string>() { "ErrorCode", "ErrorString", "IsError", "IsNaN", "IsNumber", "IsObject", "IsString", "Type"};
        }

        public void InitializeForRun() // For IObjectValue; don't need to do anything.
        {
        }

        public void RunComplete()
        {
        }

        // No actual commands to run.
        public async Task<RunResult.RunStatus> RunAsync(BCRunContext context, string name, IList<IExpression> ArgList, BCValue Retval)
        {
            await Task.Delay(0);
            return RunResult.RunStatus.OK;
        }

        public void Dispose() // For IObjectValue; don't need to do anything.
        {
        }
    }
}
```

# CopyBCValueListCSFile FileName=BCValueList.cs DirName=CsBluetoothFoundation SuppressFile=:SuppressCSharpProtocol:

Copy of the BCValueList.cs file. Does not require any customization per-BT device.


```
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

```


# CopyBluetoothStatusEventCSFile FileName=BluetoothStatusEvent.cs DirName=CsBluetoothFoundation SuppressFile=:SuppressCSharpProtocol:




```
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace BluetoothProtocols
{

    /// <summary>
    /// Common event delegate for all Bluetooth status values.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="status"></param>
    public delegate void BluetoothStatusHandler(object source, BluetoothCommunicationStatus status);

    public class BluetoothStatusEvent
    {
        public event BluetoothStatusHandler OnBluetoothStatus;
        public void ReportStatus(string report, GattDeviceServicesResult status)
        {
            if (status == null)
            {
                ;
            }
            OnBluetoothStatus?.Invoke(this, new BluetoothCommunicationStatus(report, status.ProtocolError, status.Status));
        }
        public void ReportStatus(string report, GattCharacteristicsResult status)
        {
            if (status == null)
            {
                OnBluetoothStatus?.Invoke(this, new BluetoothCommunicationStatus(report, null, GattCommunicationStatus.ProtocolError));
            }
            else
            {
                OnBluetoothStatus?.Invoke(this, new BluetoothCommunicationStatus(report, status.ProtocolError, status == null ? GattCommunicationStatus.ProtocolError : status.Status));
            }
        }
        public void ReportStatus(string report, GattCommunicationStatus status)
        {
            OnBluetoothStatus?.Invoke(this, new BluetoothCommunicationStatus(report, null, status));
        }
    }
}
```






# CopyValueToStringCSFile FileName=ValueToString.cs DirName=CsBluetoothFoundation SuppressFile=:SuppressCSharpProtocol:



```
using BluetoothDeviceController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Windows.Storage.Streams;
using static Utilities.DataReaderReadStringRobust;

namespace BluetoothDeviceController.BleEditor
{
    /// <summary>
    /// Converts an IBuffer to a string using the given type. Default is HEX.
    /// </summary>
    /// 
    /// See https://shipwrecksoftware.wordpress.com/2019/10/13/modern-iot-number-formats/ for details
    /// 
    // Description is Field [SP Field]*
    // Field is Format|Display|Name|Units  e.g. U8|HEX|Green
    // Format is format [^calculation]
    //     U<bitsize> or I<bitsize> or F<bitsize>
    //      bitsize is 8, 16, 24, 32 for U and I, 32 and 64 for F
    //     Q<intbits>Q<fractionalbits>  fixed point number e.g. Q6Q10|HEX|AccelX|G
    //         total of intbits + fractionalbits must be 8, 16, 32
    //     /[U|I]<intbits>/[U|I|P]<fractionalbits> fixed point number
    //         intbits and fractionalbits are each 8,16, or 32
    //         P means the number is a decimal e.g. for P8 the number is 0..99 
    //     BYTES
    //     STRING -- display is ASCII
    //     OEB OEL order endian; default is little-endian
    //     OOPT reset of fields are optional
    //
    //  Display is DEC HEX FIXED STRING
    public class ValueParserResult
    {
        public static ValueParserResult CreateError(string str, string error)
        {
            return new ValueParserResult()
            {
                Result = ResultValues.Error,
                UserString = str,
                ErrorString = error,
            };
        }
        public static ValueParserResult CreateOk(string str, BCBasic.BCValueList valueList = null)
        {
            return new ValueParserResult()
            {
                Result = ResultValues.Ok,
                UserString = str,
                ValueList = valueList,
            };
        }
        public enum ResultValues {  Ok, Error };
        public ResultValues Result = ResultValues.Ok;
        /// <summary>
        /// The nice decoded value. If there was an error, this might be ??-??
        /// </summary>
        public string UserString = "??-??";
        /// <summary>
        /// The error string from any errors encountere while parsing; otherwise just blank.
        /// </summary>
        public string ErrorString = "";
        public IList<byte> ByteResult = null;
        /// <summary>
        /// Either the error string or the user string depending on whether there was an error or not.
        /// </summary>
        public string AsString {  get { if (ErrorString == "") return UserString; return ErrorString; } }
        //NOTE: can also add in e.g. 
        /// <summary>
        /// List of name/value pairs in BC BASIC format (strings/doubles)
        /// </summary>
        public BCBasic.BCValueList ValueList = new BCBasic.BCValueList();
    }

    /// <summary>
    /// Class to perfectly parse the binary value descriptor strings.
    /// Simple example: "U8 U8" "U8|DEC|Temp|C U8|HEX|Mode"
    /// Complex example: "Q12Q4^_125_/|FIXED|Pressure|mbar"
    /// Three levels of splitting using space, vertical-bar (|) and caret (^)
    /// </summary>
    public class ValueParserSplit
    {
        ValueParserSplit(string value)
        {
            Parse(value);
        }
        /// <summary>
        /// Examples: U8 F32 BYTES STRING
        /// </summary>
        public string ByteFormatPrimary { get; set; } = "U8";
        /// <summary>
        /// Examples: HEX DEC FIXED STRING 
        /// </summary>
        public string DisplayFormatPrimary { get; set; } = "";
        public string NamePrimary { get; set; } = "";
        public string UnitsPrimary { get; set; } = "";
        /// <summary>
        /// Universal GET by index. Get (0,0) is the same as getting the ByteFormatPrimary. Invalid indexes return ""
        /// </summary>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        /// <returns>either the value or "" for indexes that are out of range</returns>
        public string Get(int index1, int index2)
        {
            if (index1 < 0 || index2 < 0) return "";
            if (index1 == 0 && index2 == 0) return ByteFormatPrimary;
            if (index1 == 1 && index2 == 0) return DisplayFormatPrimary;
            if (index1 == 2 && index2 == 0) return NamePrimary;
            if (index2 == 3 && index2 == 0) return UnitsPrimary;
            if (index1 >= SplitData.Length) return "";
            if (index2 >= SplitData[index1].Length) return "";
            return SplitData[index1][index2];
        }
        private static char[] Space = new char[] { ' ' };
        private static char[] Bar = new char[] { '|' };
        private static char[] Caret = new char[] { '^' };


        private string[][] SplitData = null;
        /// <summary>
        /// ParseScanResponseServiceData after the string is split into units
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void Parse (string value)
        {
            var barsplit = value.Split(Bar);
            SplitData = new string[barsplit.Length][];
            for (int i=0; i<barsplit.Length; i++)
            {
                SplitData[i] = barsplit[i].Split(Caret);
                switch (i)
                {
                    case 0: ByteFormatPrimary = SplitData[i][0]; break;
                    case 1: DisplayFormatPrimary = SplitData[i][0]; break;
                    case 2: NamePrimary = SplitData[i][0]; break;
                    case 3: UnitsPrimary = SplitData[i][0]; break;
                }
            }
        }

        public static IList<ValueParserSplit> ParseLine(string value)
        {
            var Retval = new List<ValueParserSplit>();
            var split = value.Split(Space);
            for (int i=0; i<split.Length; i++)
            {
                var item = new ValueParserSplit(split[i]);
                Retval.Add(item);
            }
            return Retval;
        }
    }
    public static class ValueParser
    {
        /// <summary>
        /// Given a byte buffer of data and a string of decode commands (like "U8|DEC I8|HEX") produce a string representation of the byte buffer
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="decodeCommands"></param>
        /// <returns></returns>
        public static ValueParserResult Parse(IBuffer buffer, string decodeCommands)
        {
            var dr = DataReader.FromBuffer(buffer);
            dr.ByteOrder = ByteOrder.LittleEndian; // default to little endian because it's common for bluetooth/
            return ConvertHelper(dr, decodeCommands);
        }
        enum ResultState { NoResult, IsString, IsDouble, IsBytes };

        private static string DoubleToString(double dvalue, string displayFormat, string displayFormatSecondary, string fixedFormat="F2", string hexFormat="X6", string decFormat="D")
        {
            switch (displayFormat)
            {
                case "":
                case "FIXED":
                    if (!String.IsNullOrEmpty(displayFormatSecondary)) fixedFormat = displayFormatSecondary;
                    return dvalue.ToString(fixedFormat);
                case "HEX":
                    if (!String.IsNullOrEmpty(displayFormatSecondary)) hexFormat = displayFormatSecondary;
                    return ((int)dvalue).ToString(hexFormat);
                case "DEC":
                    if (!String.IsNullOrEmpty(displayFormatSecondary)) decFormat = displayFormatSecondary;
                    return ((int)dvalue).ToString(decFormat);
                case "Speciality":
                    switch (displayFormatSecondary)
                    {
                        case "Appearance":
                            return BluetoothDefinitionLanguage.BluetoothAppearance.AppearaceToString((ushort)dvalue);
                    }
                    break;
            }
            return null; // null means we couldn't do the conversion
        }

        /// <summary>
        /// Returns a ValueParserResult: a string and a ValueList of the result
        /// </summary>
        private static ValueParserResult ConvertHelper(DataReader dr, string decodeCommands)
        {
            var str = "";
            var vps = ValueParserSplit.ParseLine(decodeCommands);
            var valueList = new BCBasic.BCValueList();
            bool isOptional = false;

            for (int i = 0; i < vps.Count; i++)
            {
                var stritem = "";
                byte[] byteArrayItem = null;

                var command = vps[i];
                var readcmd = command.ByteFormatPrimary;
                var readindicator = readcmd[0];
                var displayFormat = command.DisplayFormatPrimary;
                var displayFormatSecondary = command.Get(1, 1);

                var name = command.NamePrimary;
                if (string.IsNullOrEmpty(name)) name = $"param{i}";
                var units = command.UnitsPrimary;

                var resultState = ResultState.IsDouble; // the most common result
                double dvalue = double.NaN;
                try
                {
                    switch (readindicator)
                    {
                        case 'F': // FLOAT
                            if (dr.UnconsumedBufferLength == 0)
                            {
                                if (isOptional)
                                {
                                    dvalue = double.NaN;
                                    stritem = "";
                                    break;
                                }
                                else
                                {
                                    return ValueParserResult.CreateError(decodeCommands, $"Missing data with {readcmd} field {i+1}");
                                }
                            }
                            switch (readcmd)
                            {
                                case "F32":
                                    {
                                        dvalue = dr.ReadSingle();
                                        switch (displayFormat)
                                        {
                                            case "":
                                            case "FIXED":
                                                displayFormat = (displayFormatSecondary == "") ? "N3" : displayFormatSecondary;
                                                break;
                                            case "DEC":
                                                displayFormat = (displayFormatSecondary == "") ? "N0" : displayFormatSecondary;
                                                break;
                                            case "HEX":
                                                return ValueParserResult.CreateError(decodeCommands, $"Float displayFormat unrecognized; should be FIXED {displayFormat}");
                                        }
                                        stritem = dvalue.ToString(displayFormat); // e.g. N3 for 3 digits
                                    }
                                    break;
                                case "F64":
                                    {
                                        dvalue = dr.ReadDouble();
                                        switch (displayFormat)
                                        {
                                            case "":
                                            case "FIXED":
                                                displayFormat = (displayFormatSecondary == "") ? "N3" : displayFormatSecondary;
                                                break;
                                            case "DEC":
                                                displayFormat = (displayFormatSecondary == "") ? "N0" : displayFormatSecondary;
                                                break;
                                            case "HEX":
                                                return ValueParserResult.CreateError(decodeCommands, $"Float displayFormat unrecognized; should be FIXED {displayFormat}");
                                        }
                                        stritem = dvalue.ToString(displayFormat); // e.g. N3 for 3 digits
                                    }
                                    break;
                                default:
                                    return ValueParserResult.CreateError(decodeCommands, $"Float command unrecognized; should be F32 or F64 not {readcmd}");
                            }
                            break;
                        case 'I':
                            if (dr.UnconsumedBufferLength == 0)
                            {
                                if (isOptional)
                                {
                                    dvalue = double.NaN;
                                    stritem = "";
                                    break;
                                }
                                else
                                {
                                    return ValueParserResult.CreateError(decodeCommands, $"Missing data with {readcmd} field {i + 1}");
                                }
                            }
                            switch (readcmd)
                            {
                                case "I8":
                                case "I16":
                                case "I24":
                                case "I32":
                                    {
                                        string floatFormat = "F2";
                                        string intFormat = "X6";
                                        switch (readcmd)
                                        {
                                            case "I8":
                                                {
                                                    var value = (sbyte)dr.ReadByte();
                                                    dvalue = (double)value;
                                                }
                                                break;
                                            case "I16":
                                                {
                                                    var value = dr.ReadInt16();
                                                    dvalue = (double)value;
                                                }
                                                break;
                                            case "I24":
                                                {
                                                    var b0 = dr.ReadByte();
                                                    var b1 = dr.ReadByte();
                                                    var b2 = dr.ReadByte();
                                                    var msb = (sbyte)(dr.ByteOrder == ByteOrder.BigEndian ? b0 : b2);
                                                    var lsb = dr.ByteOrder == ByteOrder.BigEndian ? b2 : b0;
                                                    int value = (int)(msb << 16) + (b1 << 8) + (lsb);
                                                    dvalue = (double)value;
                                                }
                                                break;
                                            case "I32":
                                                {
                                                    var value = dr.ReadInt32();
                                                    dvalue = (double)value;
                                                    intFormat = "X8";
                                                }
                                                break;
                                        }
                                        string calculateCommand = command.Get(0, 1); // e.g. for I24^100_/ for TI 1350 barometer values
                                        if (!string.IsNullOrEmpty(calculateCommand))
                                        {
                                            dvalue = ValueCalculate.Calculate(calculateCommand, dvalue).D;
                                            if (double.IsNaN(dvalue))
                                            {
                                                return ValueParserResult.CreateError(decodeCommands, $"Calculation failed for {calculateCommand} in {readcmd}");
                                            }
                                            else
                                            {
                                                // Everything worked and got a value
                                                stritem = DoubleToString(dvalue, displayFormat, displayFormatSecondary);
                                                if (stritem == null)
                                                {
                                                    return ValueParserResult.CreateError(decodeCommands, $"Integer display format command unrecognized; should be FIXED or HEX or DEC not {displayFormat} in {readcmd}");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (displayFormat == "") displayFormat = "HEX";
                                            stritem = DoubleToString(dvalue, displayFormat, displayFormatSecondary, floatFormat, intFormat);
                                            if (stritem == null)
                                            {
                                                return ValueParserResult.CreateError(decodeCommands, $"Integer display format command unrecognized; should be FIXED or HEX or DEC not {displayFormat} in {readcmd}");
                                            }
                                        }

                                    }
                                    break;

                                default:
                                    return ValueParserResult.CreateError(decodeCommands, $"Integer command unrecognized; should be I8/16/24/32 not {readcmd}");
                            }
                            break;
                        case 'O':
                            switch (readcmd)
                            {
                                case "OEB":
                                    resultState = ResultState.NoResult;
                                    dr.ByteOrder = ByteOrder.LittleEndian;
                                    break;
                                case "OEL":
                                    resultState = ResultState.NoResult;
                                    dr.ByteOrder = ByteOrder.LittleEndian;
                                    break;
                                case "OOPT":
                                    isOptional = true;
                                    break;
                                default:
                                    return ValueParserResult.CreateError(decodeCommands, $"Optional command unrecognized; should be OEL or OEB not {readcmd}");
                            }
                            break;
                        case 'Q':
                            if (dr.UnconsumedBufferLength == 0)
                            {
                                if (isOptional)
                                {
                                    dvalue = double.NaN;
                                    stritem = "";
                                    break;
                                }
                                else
                                {
                                    return ValueParserResult.CreateError(decodeCommands, $"Missing data with {readcmd} field {i + 1}");
                                }
                            }
                            // e.g. Q12Q4.Fixed
                            {
                                var subtypes = readcmd.Split(new char[] { 'Q' });
                                if (subtypes.Length != 3) // Actually 2, but first is blank
                                {
                                    return ValueParserResult.CreateError(decodeCommands, $"Q command unrecognized; wrong number of Qs {readcmd}");
                                }
                                stritem = FixedQuotientHelper(dr, subtypes[1], subtypes[2], displayFormat, out dvalue);
                                //NOTE: fail on failure
                            }
                            break;
                        case 'U':
                            if (dr.UnconsumedBufferLength == 0)
                            {
                                if (isOptional)
                                {
                                    dvalue = double.NaN;
                                    stritem = "";
                                    break;
                                }
                                else
                                {
                                    return ValueParserResult.CreateError(decodeCommands, $"Missing data with {readcmd} field {i + 1}");
                                }
                            }
                            switch (readcmd)
                            {
                                case "U8":
                                case "U16":
                                case "U24":
                                case "U32":
                                    string xfmt = "X2";
                                    switch (readcmd)
                                    {
                                        case "U8":
                                            {
                                                var value = dr.ReadByte();
                                                dvalue = (double)value;
                                                xfmt = "X2";
                                            }
                                            break;
                                        case "U16":
                                            {
                                                var value = dr.ReadUInt16();
                                                dvalue = (double)value;
                                                xfmt = "X4";
                                            }
                                            break;
                                        case "U24":
                                            {
                                                var b0 = dr.ReadByte();
                                                var b1 = dr.ReadByte();
                                                var b2 = dr.ReadByte();
                                                var msb = (byte)(dr.ByteOrder == ByteOrder.BigEndian ? b0 : b2);
                                                var lsb = dr.ByteOrder == ByteOrder.BigEndian ? b2 : b0;
                                                int value = (int)(msb << 16) + (b1 << 8) + (lsb); //TODO: this should be unsigned??
                                                dvalue = (double)value;
                                            }
                                            break;
                                        case "U32":
                                            {
                                                var value = dr.ReadUInt32();
                                                dvalue = (double)value;
                                                xfmt = "X8";
                                            }
                                            break;
                                    }
                                    string calculateCommand = command.Get(0, 1); // e.g. for I24^100_/ for TI 1350 barometer values
                                    if (!string.IsNullOrEmpty(calculateCommand))
                                    {
                                        dvalue = ValueCalculate.Calculate(calculateCommand, dvalue).D;
                                        if (double.IsNaN(dvalue))
                                        {
                                            return ValueParserResult.CreateError(decodeCommands, $"Calculation failed for {calculateCommand} in {readcmd}");
                                        }
                                        else
                                        {
                                            stritem = DoubleToString(dvalue, displayFormat, displayFormatSecondary);
                                            if (stritem == null)
                                            {
                                                return ValueParserResult.CreateError(decodeCommands, $"Integer display format command unrecognized; should be FIXED or HEX or DEC not {displayFormat} in {readcmd}");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (displayFormat == "") displayFormat = "HEX";
                                        stritem = DoubleToString(dvalue, displayFormat, displayFormatSecondary, "F2", xfmt);
                                        if (stritem == null)
                                        {
                                            return ValueParserResult.CreateError(decodeCommands, $"Integer display format command unrecognized;\nshould be FIXED or HEX or DEC not {displayFormat} in {readcmd}");
                                        }
                                    }
                                    break;
                                default:
                                    return ValueParserResult.CreateError(decodeCommands, $"UInteger command unrecognized;\nshould be U8/U16/U24/U32 not {readcmd}");
                            }
                            break;
                        case '/':
                            // e.g. /U8/I16|Fixed
                            if (dr.UnconsumedBufferLength == 0)
                            {
                                if (isOptional)
                                {
                                    dvalue = double.NaN;
                                    stritem = "";
                                    break;
                                }
                                else
                                {
                                    return ValueParserResult.CreateError(decodeCommands, $"Missing data with {readcmd} field {i + 1}");
                                }
                            }
                            {
                                var subtypes = readcmd.Split(new char[] { '/' });
                                if (subtypes.Length != 3) // Actually 2, but first is blank
                                {
                                    return ValueParserResult.CreateError(decodeCommands, $"/ command unrecognized; wrong number of slashes {readcmd}");
                                }
                                stritem = FixedFractionHelper(dr, subtypes[1], subtypes[2], displayFormat, out dvalue);
                                // NOTE: fail on failure
                            }
                            break;
                        default:
                            if (readcmd != readcmd.ToUpper())
                            {
                                System.Diagnostics.Debug.WriteLine("ERROR: readcmd {readcmd} but should be uppercase");
                            }
                            switch (readcmd.ToUpper()) //NOTE: should be all-uppercase; any lowercase is wrong
                            {
                                case "STRING":
                                    {
                                        ReadStatus readStatus;
                                        (stritem, readStatus) = DataReaderReadStringRobust.ReadString(dr, dr.UnconsumedBufferLength);
                                        switch (readStatus)
                                        {
                                            case ReadStatus.OK:
                                                switch (displayFormat)
                                                {
                                                    case "":
                                                    case "ASCII":
                                                        stritem = EscapeString(stritem, displayFormatSecondary);
                                                        break;
                                                    case "Eddystone":
                                                        stritem = BluetoothDefinitionLanguage.Eddystone.EddystoneUrlToString(stritem);
                                                        break;
                                                    default:
                                                        return ValueParserResult.CreateError(decodeCommands, $"Unknown string format type {displayFormat}");

                                                }
                                                break;
                                            case ReadStatus.Hex:
                                                break;
                                        }

                                        resultState = ResultState.IsString;
                                    }
                                    break;
                                case "BYTES":
                                    {
                                        //You might want bytes, but this methods is explicitly generating a string.
                                        if (dr.UnconsumedBufferLength == 0)
                                        {
                                            stritem = "(no bytes)";
                                            resultState = ResultState.IsString;
                                        }
                                        else
                                        {
                                            IBuffer buffer = dr.ReadBuffer(dr.UnconsumedBufferLength);
                                            byteArrayItem = buffer.ToArray();
                                            var format = "X2";
                                            switch (displayFormat)
                                            {
                                                case "DEC": format = ""; break;
                                                default:
                                                case "HEX": format = "X2"; break;
                                            }
                                            for (uint ii = 0; ii < byteArrayItem.Length; ii++)
                                            {
                                                if (ii != 0) stritem += " ";
                                                stritem += byteArrayItem[ii].ToString(format);
                                            }
                                            resultState = ResultState.IsBytes;
                                        }
                                    }
                                    break;
                                default:
                                    return ValueParserResult.CreateError(decodeCommands, $"Other command unrecognized; should be STRING or BYTES {readcmd}");
                            }
                            break;
                    }
                }
                catch (Exception e)
                {
                    stritem = $"EXCEPTION reading data {e} index {i} command {command} len {dr.UnconsumedBufferLength}";
                    return ValueParserResult.CreateError(str + stritem, stritem);
                }
                BCBasic.BCValue resultValue = null;
                switch (resultState)
                {
                    case ResultState.IsBytes:
                        resultValue = new BCBasic.BCValue(byteArrayItem);
                        valueList.SetProperty(name, resultValue);
                        break;
                    case ResultState.IsDouble:
                        resultValue = new BCBasic.BCValue(dvalue);
                        valueList.SetProperty(name, resultValue);
                        break;
                    case ResultState.IsString:
                        resultValue = new BCBasic.BCValue(stritem);
                        valueList.SetProperty(name, resultValue);
                        break;
                }

                if (str != "") str += " ";
                str += stritem;

                if (dr.UnconsumedBufferLength <= 0)
                {
                    break;
                }

            }
            return ValueParserResult.CreateOk(str, valueList);
        }

        // Command is e.g. U8 or I32 or P8
        private static double ReadValue(DataReader dr, string command)
        {
            double Retval = Double.NaN;
            switch (command)
            {
                case "I8":
                    Retval = (sbyte)dr.ReadByte();
                    break;
                case "I16":
                    Retval = dr.ReadInt16();
                    break;
                case "I32":
                    Retval = dr.ReadInt32();
                    break;

                case "P8":
                    Retval = (sbyte)dr.ReadByte();
                    break;
                case "P16":
                    Retval = dr.ReadInt16();
                    break;
                case "P32":
                    Retval = dr.ReadInt32();
                    break;

                case "U8":
                    Retval = dr.ReadByte();
                    break;
                case "U16":
                    Retval = dr.ReadUInt16();
                    break;
                case "U32":
                    Retval = dr.ReadUInt32();
                    break;
            }
            return Retval;
        }


        /// <summary>
        /// Handles /I16/U8.FIXED type requests. The I16 is the integer part and the U8 is the fraction. The fraction for U and I is binary and for P types is decimal (e.g. P8 is an out-of-100 portion)
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="integralType"></param>
        /// <param name="fractionType"></param>
        /// <param name="presentation"></param>
        /// <returns></returns>
        private static string FixedFractionHelper(DataReader dr, string integralType, string fractionType, string presentation, out double dvalue)
        {
            string stritem;
            var integral = ReadValue(dr, integralType);
            var fraction = ReadValue(dr, fractionType);
            var denominator = 1.0;
            switch (fractionType)
            {
                case "I8":
                case "U8":
                    denominator = 1 << 8;
                    break;
                case "P8":
                    denominator = 100;
                    break;
                case "I16":
                case "U16":
                    denominator = 1 << 16;
                    break;
                case "P16":
                    denominator = 10000; //  1 << 16;
                    break;
                case "I32":
                case "U32":
                    denominator = 1 << 32;
                    break;
                case "P32":
                    denominator = 1000000; //  1 << 32;
                    break;

            }
            switch (presentation)
            {
                default: // NOTE: error!
                case "FIXED":
                    if (fraction > denominator)
                    {
                        ; //TODO: failure
                    }
                    dvalue = integral + (fraction / denominator);
                    stritem = dvalue.ToString();
                    break;
                    // NOTE: need to add a failure path pretty much everywhere.
            }
            return stritem;
        }
        private static string FixedQuotientHelper(DataReader dr, string integralType, string fractionType, string presentation, out double dvalue)
        {
            dvalue = double.NaN;
            string stritem;
            var integralBits = Int32.Parse(integralType);
            var fractionBits = Int32.Parse(fractionType);
            var nbits = integralBits + fractionBits;
            Int32 rawValue = 0;
            switch (nbits)
            {
                default:
                    return $"ERROR: incorrect Qbit encoding {integralType}Q{fractionType}";
                case 8: rawValue = (sbyte)dr.ReadByte(); break;
                case 16: rawValue = dr.ReadInt16(); break;
                case 32: rawValue = dr.ReadInt32(); break;
            }
            string idealFormat = "N";
            // How many bits of precision?
            // 3--> 1 chars
            // 4-->2 chars
            var maxSize = 1<<fractionBits; // e.g. 10-->1024
            var displaybits = Math.Ceiling(Math.Log10(maxSize));
            idealFormat = $"N{displaybits}";

            // An example: we get a number in 12Q4 format.
            // That means that the top 12 bits are the integer part
            // and the bottom 4 are the fractional part.
            // This is actually a simple calculation!
            // The divideBy is 1<<4 (remember that 1<<0==1, 1<<1==2, 1<<2==4 1<<3=8 1<<4==16)
            double divideBy = (double)(1 << fractionBits);
            dvalue = ((double)rawValue) / divideBy;

            switch (presentation)
            {
                default: // TODO: error!
                case "FIXED":
                    stritem = dvalue.ToString(idealFormat);
                    break;
                    // TODO: need to add a failure path pretty much everywhere.
            }
            return stritem;
        }

        public static string ConvertToStringHex(IBuffer buffer)
        {
            var str = "";
            for (uint i = 0; i < buffer.Length; i++)
            {
                if (str != "") str += " ";
                str += $"{buffer.GetByte(i):X2}";
            }
            return str;
        }


        private static string EscapeString(string rawstr, string displayFormatSecondary) // displayFormatSecondary is either "" or LONG
        {
            rawstr = rawstr.Replace("\\", "\\\\"); // escape all back-slashes
            rawstr = rawstr.Replace("\0", "\\0"); // replace a real NUL with an escaped null.
            switch (displayFormatSecondary)
            {
                default:
                    rawstr = rawstr.Replace("\r", "\\r");
                    rawstr = rawstr.Replace("\n", "\\n");
                    break;
                case "LONG":
                    // The LONG style display can easily display CR and LF 
                    // The concept where was to make it trivial to both get the CR LF and
                    // also see the values on the screen. The goal was to be easier to distinguish.
                    // The actual result looked cluttered and ugly.
                    //rawstr = rawstr.Replace("\r", "\\r\r");
                    //rawstr = rawstr.Replace("\n", "\\n\n");
                    break;
            }
            rawstr = rawstr.Replace("\v", "\\v");
            return rawstr;
        }

        public static string UnescapeString (string rawstr)
        {
            var retval = rawstr;
            // Do in reverse order of the EscapeString
            retval = retval.Replace("\\v", "\v").Replace("\\n", "\n").Replace("\\r", "\r").Replace("\\0", "\0").Replace("\\\\", "\\");
            return retval;
        }

        public static ValueParserResult ConvertToBuffer (this string value, string type)
        {
            var Retval = new ValueParserResult();
            Retval.ByteResult = new List<byte>() { Capacity = (value.Length + 1) / 3 }; // most likely size
            type = type.ToUpper();
            switch (type)
            {
                case "ASCII":
                    foreach (var ch in value)
                    {
                        Retval.ByteResult.Add((byte)(ch & 0xFF)); //just whack them.
                    }
                    break;
                case "DEC":
                case "HEX":
                    var items = value.Split(" ");
                    foreach (var item in items)
                    {
                        byte b = 0;
                        bool converted;

                        var specifier = System.Globalization.NumberStyles.None;
                        if (type == "HEX")
                        {
                            specifier = System.Globalization.NumberStyles.AllowHexSpecifier;
                        }
                        converted = byte.TryParse(item, specifier, null, out b);
                        if (converted)
                        {
                            Retval.ByteResult.Add(b);
                        }
                        else
                        {
                            Retval.Result = ValueParserResult.ResultValues.Error;
                            Retval.ErrorString += $"Item {item} could not be converted as {type}\n";
                        }
                    }
                    break;
            }
            return Retval;
        }
    }
}
```





# CopyMockClassesCSFile FileName=MockClasses.cs DirName=CsBluetoothFoundation SuppressFile=:SuppressCSharpProtocol:


```
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BCBasic
{
    public interface IObjectValue : IDisposable
    {
        string PreferredName { get; }
        BCValue GetValue(string name);
        void SetValue(string name1, BCValue value);
        IList<string> GetNames();
        void InitializeForRun(); // Called before each run 
        void RunComplete(); // The program has finished running and services can be cleaned up

        // IObjectValue can also call a function by name
        Task<RunResult.RunStatus> RunAsync(BCRunContext context, string name, IList<IExpression> ArgList, BCValue Retval);
    }

    public interface IExpression
    {
        Task<BCValue> EvalAsync(BCRunContext context);
    }

    public class RunResult
    {
        public RunResult()
        {
        }
        public RunResult(string str)
        {
            Result = new BCValue(str);
        }
        public enum RunStatus { OK, ErrorContinue, ErrorStop }; // ErrorStop means we can't keep trying.  ErrorContinue means we can.
        // ErrorContinue is used for when an object doesn't have the requested method
        // It's also used when a method is being called 'correctly' but the method can't run
        // For example, when calling g.UseScale ("meter") and there's no such scale.

        public RunStatus Status = RunStatus.OK;
        public BCValue Result = new BCValue("");
        public string Error = "";
        public string Message = "";
        public string GetMessage(string defaultValue)
        {
            if (Message == null || Message == "") return defaultValue;
            return Message;
        }
        public override string ToString() { if (Status == RunStatus.OK) return Result.AsString; return Error; }
    }

    public class BCRunContext
    {

    }

    public class BCObjectUtilities
    {
        public static bool CheckArgs(int min, int max, string name, IList<IExpression> ArgList, BCValue Retval)
        {
            if (ArgList.Count < min)
            {
                if (min == max)
                    Retval.SetError(1, $"{name} requires {min} args, not {ArgList.Count}.");
                else
                    Retval.SetError(1, $"{name} requires {min} to {max} args, not {ArgList.Count}.");
                return false;
            }
            if (ArgList.Count > max)
            {
                if (min == max)
                    Retval.SetError(1, $"{name} requires {min} args, not {ArgList.Count}.");
                else
                    Retval.SetError(1, $"{name} requires {min} to {max} args, not {ArgList.Count}.");
                return false;
            }
            return true;
        }

        public static async Task<bool> CheckArgValue(int index, string argName, int min, int max, BCRunContext context, string name, IList<IExpression> ArgList, BCValue Retval)
        {
            if (index >= ArgList.Count)
            {
                Retval.SetError(1, $"{name} is missing the {argName} argument.");
                return false;
            }
            double value = (await ArgList[index].EvalAsync(context)).AsDouble;
            return CheckValueRange(name, value, argName, min, max, Retval);
        }
        // name and argName are used only for error messages
        public static bool CheckValueRange(string name, double value, string argName, double min, double max, BCValue Retval)
        {
            if (!Double.IsNaN(min) && value < min)
            {
                Retval.SetError(1, $"{name} {argName} argument is {value} but must be >= {min}");
                return false;
            }
            if (!Double.IsNaN(max) && value > max)
            {
                Retval.SetError(1, $"{name} {argName} argument is {value} but must be <= {max}");
                return false;
            }
            return true;
        }
    }

}
```



# CopyBluetoothCommunicationStatusCSFile FileName=BluetoothCommunicationStatus.cs DirName=CsBluetoothFoundation SuppressFile=:SuppressCSharpProtocol:


```
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace BluetoothProtocols
{
    public class BluetoothCommunicationStatus
    {
        public BluetoothCommunicationStatus(string about, byte? error, GattCommunicationStatus status)
        {
            About = about;
            ProtocolError = error;
            Status = status;
        }

        public string About { get; internal set; }
        public byte? ProtocolError { get; internal set; }
        public GattCommunicationStatus Status { get; internal set; }

        /// <summary>
        /// Converts the status values here into a human readable but technical string
        /// </summary>
        public string AsStatusString {
            get
            {
                switch (Status)
                {
                    case GattCommunicationStatus.Success:
                        return $"OK: {About}";
                    case GattCommunicationStatus.AccessDenied:
                        return $"Access denied: {About}";
                    case GattCommunicationStatus.Unreachable:
                        return $"Unreachable: {About}";
                    case GattCommunicationStatus.ProtocolError:
                        if (ProtocolError.HasValue)
                        {
                            return $"Protocol error: value={ProtocolError.Value} {About}";
                        }
                        else
                        {
                            return $"Protocol error: {About}";
                        }
                    default:
                        return $"??: {About}";
                }
            }
        }
    }
}
```



# CopyBluetoothAppearanceCSFile FileName=BluetoothAppearance.cs DirName=CsBluetoothFoundation SuppressFile=:SuppressCSharpProtocol:

```
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothDeviceController.BluetoothDefinitionLanguage
{
    public class BluetoothAppearance
    {
        // Data from https://www.bluetooth.com/specifications/gatt/characteristics/
        // https://www.bluetooth.com/wp-content/uploads/Sitecore-Media-Library/Gatt/Xml/Characteristics/org.bluetooth.characteristic.gap.appearance.xml
        // Characteristic 0x2A01, Appearance
        // Values are automatically converted from the XML (plus hand corrections)
        public enum Appearance
        {
            Unknown = 0,
            Phone = 64,
            Computer = 128,
            Watch = 192,
            Sports_Watch = 193,
            Clock = 256,
            Display = 320,
            Remote_Control = 384,
            Eyeglasses = 448,
            Tag = 512,
            Keyring = 576,
            Media_Player = 640,
            Barcode_Scanner = 704,
            Thermometer = 768,
            Thermometer_Ear = 769,
            Heart_rate_Sensor = 832,
            Heart_Rate_Belt = 833,
            Blood_Pressure = 896,
            Blood_Pressure_Arm = 897,
            Blood_Pressure_Wrist = 898,
            Human_Interface_Device = 960,
            Keyboard = 961,
            Mouse = 962,
            Joystick = 963,
            Gamepad = 964,
            Digitizer_Tablet = 965,
            Card_Reader = 966,
            Digital_Pen = 967,
            Barcode_Scanner_HID = 968,
            Glucose_Meter = 1024,
            Running_Walking_Sensor = 1088,
            Running_Walking_Sensor_In_Shoe = 1089,
            Running_Walking_Sensor_On_Shoe = 1090,
            Running_Walking_Sensor_On_Hip = 1091,
            Cycling = 1152,
            Cycling_Computer = 1153,
            Cycling_Speed_Sensor = 1154,
            Cycling_Cadence_Sensor = 1155,
            Cycling_Power_Sensor = 1156,
            Cycling_Speed_and_Cadence_Sensor = 1157,
            Pulse_Oximeter = 3136,
            Pulse_Oximeter_Fingertip = 3137,
            Pulse_Oximeter_Wrist_Worn = 3138,
            Weight_Scale = 3200,
            Personal_Mobility_Device = 3264,
            Powered_Wheelchair = 3265,
            Mobility_Scooter = 3266,
            Continuous_Glucose_Monitor = 3328,
            Insulin_Pump = 3392,
            Insulin_Pump_durable = 3393,
            Insulin_Pump_patch = 3396,
            Insulin_Pen = 3400,
            Medication_Delivery = 3456,
            Outdoor_Sports_Activity = 5184,
            Location_Display = 5185,
            Location_and_Navigation_Display = 5186,
            Location_Pod = 5187,
            Location_and_Navigation_Pod = 5188,
            Environmental_Sensor = 5696,
        };
        public static string AppearaceToString(UInt16 appearance)
        {
            try
            {
                // My "Gems" activity tracker has appearance 0x1234 (!)
                // which is not a valid appearance. Incorrect appearance
                // values can be caught by seeing if the resulting string
                // is in fact all-digits.

                Appearance a = (Appearance)appearance;
                var astr = a.ToString();
                if (IsAllDigits (astr))
                {
                    astr = $"?{appearance}";
                }
                else
                {
                    ; // just here for a nice debugger breakpoint.
                }
                return astr;
            }
            catch (Exception)
            {
                ;
            }
            return $"??{appearance}";
        }

        private static bool IsAllDigits(string str)
        {
            foreach (var ch in str)
            {
                if (ch < '0' || ch > '9') return false;
            }
            return true;
        }
#if NEVER_EER_DEFINED

<Enumeration description="Generic category" value="Generic Remote Control" key="384"/>

<Enumeration description="Generic category" value="Generic Eye-glasses" key="448"/>

<Enumeration description="Generic category" value="Generic Tag" key="512"/>

<Enumeration description="Generic category" value="Generic Keyring" key="576"/>

<Enumeration description="Generic category" value="Generic Media Player" key="640"/>

<Enumeration description="Generic category" value="Generic Barcode Scanner" key="704"/>

<Enumeration description="Generic category" value="Generic Thermometer" key="768"/>

<Enumeration description="Thermometer subtype" value="Thermometer: Ear" key="769"/>

<Enumeration description="Generic category" value="Generic Heart rate Sensor" key="832"/>

<Enumeration description="Heart Rate Sensor subtype" value="Heart Rate Sensor: Heart Rate Belt" key="833"/>

<!-- Added Blood pressure support on December 09, 2011 -->


<Enumeration description="Generic category" value="Generic Blood Pressure" key="896"/>

<Enumeration description="Blood Pressure subtype" value="Blood Pressure: Arm" key="897"/>

<Enumeration description="Blood Pressure subtype" value="Blood Pressure: Wrist" key="898"/>

<!-- Added HID Related appearance values on January 03, 2012 approved by BARB -->


<Enumeration description="HID Generic" value="Human Interface Device (HID)" key="960"/>

<Enumeration description="HID subtype" value="Keyboard" key="961"/>

<Enumeration description="HID subtype" value="Mouse" key="962"/>

<Enumeration description="HID subtype" value="Joystick" key="963"/>

<Enumeration description="HID subtype" value="Gamepad" key="964"/>

<Enumeration description="HID subtype" value="Digitizer Tablet" key="965"/>

<Enumeration description="HID subtype" value="Card Reader" key="966"/>

<Enumeration description="HID subtype" value="Digital Pen" key="967"/>

<Enumeration description="HID subtype" value="Barcode Scanner" key="968"/>

<!-- Added Generic Glucose Meter value on May 10, 2012 approved by BARB -->


<Enumeration description="Generic category" value="Generic Glucose Meter" key="1024"/>

<!-- Added additional appearance values on June 26th, 2012 approved by BARB -->


<Enumeration description="Generic category" value="Generic: Running Walking Sensor" key="1088"/>

<Enumeration description="Running Walking Sensor subtype" value="Running Walking Sensor: In-Shoe" key="1089"/>

<Enumeration description="Running Walking Sensor subtype" value="Running Walking Sensor: On-Shoe" key="1090"/>

<Enumeration description="Running Walking Sensor subtype" value="Running Walking Sensor: On-Hip" key="1091"/>

<Enumeration description="Generic category" value="Generic: Cycling" key="1152"/>

<Enumeration description="Cycling subtype" value="Cycling: Cycling Computer" key="1153"/>

<Enumeration description="Cycling subtype" value="Cycling: Speed Sensor" key="1154"/>

<Enumeration description="Cycling subtype" value="Cycling: Cadence Sensor" key="1155"/>

<Enumeration description="Cycling subtype" value="Cycling: Power Sensor" key="1156"/>

<Enumeration description="Cycling subtype" value="Cycling: Speed and Cadence Sensor" key="1157"/>

<!-- Added appearance values for Pulse Oximeter on July 30th, 2013 approved by BARB -->


<Enumeration description="Pulse Oximeter Generic Category" value="Generic: Pulse Oximeter" key="3136"/>

<Enumeration description="Pulse Oximeter subtype" value="Fingertip" key="3137"/>

<Enumeration description="Pulse Oximeter subtype" value="Wrist Worn" key="3138"/>

<!-- Added appearance values for Generic Weight Scale on May 21, 2014 approved by BARB -->


<Enumeration description="Weight Scale Generic Category" value="Generic: Weight Scale" key="3200"/>

<!-- Added additional appearance values on October 2nd, 2016 approved by BARB -->


<Enumeration description="Personal Mobility Device" value="Generic Personal Mobility Device" key="3264"/>

<Enumeration description="Personal Mobility Device" value="Powered Wheelchair" key="3265"/>

<Enumeration description="Personal Mobility Device" value="Mobility Scooter" key="3266"/>

<Enumeration description="Continuous Glucose Monitor" value="Generic Continuous Glucose Monitor" key="3328"/>

<!-- Added additional appearance values on February 1st, 2018 approved by BARB -->


<Enumeration description="Insulin Pump" value="Generic Insulin Pump" key="3392"/>

<Enumeration description="Insulin Pump" value="Insulin Pump, durable pump" key="3393"/>

<Enumeration description="Insulin Pump" value="Insulin Pump, patch pump" key="3396"/>

<Enumeration description="Insulin Pump" value="Insulin Pen" key="3400"/>

<Enumeration description="Medication Delivery" value="Generic Medication Delivery" key="3456"/>

<!-- Added appearance values for L&N on July 30th, 2013 approved by BARB -->


<Enumeration description="Outdoor Sports Activity Generic Category" value="Generic: Outdoor Sports Activity" key="5184"/>

<Enumeration description="Outdoor Sports Activity subtype" value="Location Display Device" key="5185"/>

<Enumeration description="Outdoor Sports Activity subtype" value="Location and Navigation Display Device" key="5186"/>

<Enumeration description="Outdoor Sports Activity subtype" value="Location Pod" key="5187"/>

<Enumeration description="Outdoor Sports Activity subtype" value="Location and Navigation Pod" key="5188"/>

<!-- Added appearance values for Generic Environmental Sensor on May 21, 2014 approved by BARB<Enumeration key="5696" value="Generic: Environmental Sensor" description="Environmental Sensor Generic Category" />-->
#endif
    }
}
```


# CopyValueCalculateCSFile FileName=ValueCalculate.cs DirName=CsBluetoothFoundation SuppressFile=:SuppressCSharpProtocol:


```
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

// I16^*500/65536.FIXED.GyroX.dps

namespace BluetoothDeviceController.BleEditor
{
    /// <summary>
    /// Implements a tiny calculator. It's got just enough power to handle common IOT scenarios
    /// </summary>
    public static class ValueCalculate
    {
        /// <summary>
        /// Commands (like AN for an AND) are always exactly 2 uppercase letters. String constants are not commands.
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private static bool IsCommand(string cmd)
        {
            if (cmd == null) return false;
            char c1 = cmd[0];
            if (cmd.Length != 2) return false;
            char c2 = cmd[1];
            var retval = (c1 >= 'A' && c1 <= 'Z' && c2 >= 'A' && c2 <= 'Z');
            return retval;
        }

        /// <summary>
        /// Operands are always exactly 1 character + - * /
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private static bool IsOp (string cmd)
        {
            if (cmd == null) return false;
            if (cmd.Length != 1) return false;
            char c1 = cmd[0];
            var retval = c1 == '+' || c1 == '-' || c1 == '*' || c1 == '/';
            return retval;
        }

        /// <summary>
        /// Replaces chars in the string with the escaped version
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EscapeString (string value)
        {
            var retval = value;
            retval = retval.Replace("\\", "\\\\");

            // All of the 'C' style chars
            retval = retval.Replace("\a", "\\a"); // NOT ALLOWED IN JSON
            retval = retval.Replace("\b", "\\b");
            retval = retval.Replace("\f", "\\f");
            retval = retval.Replace("\n", "\\n");
            retval = retval.Replace("\r", "\\r");
            retval = retval.Replace("\t", "\\t");
            retval = retval.Replace("\v", "\\v"); // NOT ALLOWED IN JSON
            retval = retval.Replace("'", "\\'"); // NOT ALLOWED IN JSON
            retval = retval.Replace("\"", "\\\"");
            retval = retval.Replace("?", "\\?"); // NOT ALLOWED IN JSON
            retval = retval.Replace("\0", "\\00"); // JSON REQUIRES THIS TO BE \u0000
            // JSON ALSO CAN ESCAPE /
            // JSON ALSO ALLOWS \uXXXX for arbitrary hex
            // LASTLY, JSON DISALLOWS E.G. ^A to be in the string; it must be escaped.

            // All of the special chars TODO: JSON does not permit the use of weird escapes like \s \p \c
            // TODO: Recommend using $ instead. And following exactly the JSON escape syntax.
            retval = retval.Replace(" ", "\\s");
            retval = retval.Replace("|", "\\p");
            retval = retval.Replace("^", "\\c");

            return retval;
        }
        public static string ReverseReplace(this string str, string arg1, string arg2)
        {
            return str.Replace(arg2, arg1);
        }
        public static string UnescapeString(string value)
        {
            var retval = value;
            // All of the special chars
            retval = retval.ReverseReplace("^", "\\c");
            retval = retval.ReverseReplace("|", "\\p");
            retval = retval.ReverseReplace(" ", "\\s");

            // All of the 'C' style chars
            retval = retval.ReverseReplace("\0", "\\00");
            retval = retval.ReverseReplace("?", "\\?");
            retval = retval.ReverseReplace("\"", "\\\"");
            retval = retval.ReverseReplace("'", "\\'");
            retval = retval.ReverseReplace("\v", "\\v");
            retval = retval.ReverseReplace("\t", "\\t");
            retval = retval.ReverseReplace("\r", "\\r");
            retval = retval.ReverseReplace("\n", "\\n");
            retval = retval.ReverseReplace("\f", "\\f");
            retval = retval.ReverseReplace("\b", "\\b");
            retval = retval.ReverseReplace("\a", "\\a");

            retval = retval.ReverseReplace("\\", "\\\\");

            return retval;
        }

        public class CalculateResult
        {
            public CalculateResult (double d, string s) => (D, S) = (d, s);
            public double D { get; internal set; }
            public string S { get; internal set; }
            public void Deconstruct (out double d, out string s)
            {
                (d, s) = (D, S);
            }
        }

        /// <summary>
        /// Given a string like 1000_/ do a calculation. The string is split by underscore and the 'words'
        /// are RPN with values (1000), operations (AN) or opcodes (+). 1000_/ will divide the value on the 
        /// stack by 1000.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="startValue"></param>
        /// <returns></returns>
        public static CalculateResult Calculate(string str, double startValue, string startStringValue = "", VariableSet variables = null)
        {
            int index = 0;
            try
            {
                var stack = new Stack<double>();
                stack.Push(startValue);
                var stringstack = new Stack<string>();
                stringstack.Push(startStringValue); 

                var commands = str.Split(new char[] { '_' });
                index = 0;
                while (index < commands.Length)
                {
                    var command = commands[index];
                    var nextindex = index + 1;
                    if (IsCommand(command) || IsOp (command))
                    {
                        double d1;
                        double d2;
                        double value;
                        switch (command) // or op...
                        {
                            case "+":
                                d1 = stack.Pop();
                                d2 = stack.Pop();
                                value = d2 + d1;
                                stack.Push(value);
                                break;
                            case "-": // 10_2_- should do 10-2
                                d1 = stack.Pop();
                                d2 = stack.Pop();
                                value = d2 - d1;
                                stack.Push(value);
                                break;
                            case "*":
                                d1 = stack.Pop();
                                d2 = stack.Pop();
                                value = d2 * d1;
                                stack.Push(value);
                                break;
                            case "/":
                                d1 = stack.Pop();
                                d2 = stack.Pop();
                                value = d2 / d1;
                                stack.Push(value);
                                break;
                            case "AN": // bitwise AND
                                d1 = stack.Pop();
                                d2 = stack.Pop();
                                value = ((int)d2) & ((int)d1);
                                stack.Push(value);
                                break;
                            case "DU": // dup
                                d1 = stack.Pop();
                                stack.Push(d1);
                                stack.Push(d1);
                                break;
                            case "GO": // go to a location
                                d1 = stack.Pop();
                                nextindex = (int)d1;
                                break;


                            case "GN": // Get a name, look up in current (new)
                                {
                                    string name = stringstack.Pop();
                                    value = variables.GetCurrDouble(name); 
                                    stack.Push(value);
                                }
                                break;
                            case "GP": // Get a name, look up in previous dictionaries
                                {
                                    string name = stringstack.Pop();
                                    value = variables.GetPreviousDouble(name);
                                    stack.Push(value);
                                }
                                break;
                            case "GS": // Get a name, look up in current (new) as a string TODO: document
                                {
                                    string name = stringstack.Pop();
                                    var valuestr = variables.GetCurrString(name);
                                    stringstack.Push(valuestr);
                                }
                                break;
                            case "GD": // Get a name, look up in current and previous dictionaries. If same, return blank; otherwise return new
                                {
                                    string name = stringstack.Pop();
                                    double prev = variables.GetPreviousDouble(name);
                                    double curr = variables.GetCurrDouble(name);
                                    if (prev == curr)
                                    {
                                        stringstack.Push("");
                                    }
                                    else
                                    {
                                        stack.Push(curr);
                                    }
                                }
                                break;
                            case "IV": // negate (inverse)
                                d1 = stack.Pop();
                                value = -d1;
                                stack.Push(value);
                                break;
                            case "JN": // Jump conditional on non-zero skip the given number of instructions 0_2_JZ jumps 2 because the zero
                                d1 = stack.Pop();
                                d2 = stack.Peek();
                                if (d2 != 0)
                                {
                                    nextindex = index + (int)d1;
                                }
                                break;
                            case "JZ": // Jump conditional on zero skip the given number of instructions 0_2_JZ jumps 2 because the zero
                                d1 = stack.Pop();
                                d2 = stack.Peek();
                                if (d2 == 0)
                                {
                                    nextindex = index + (int)d1;
                                }
                                break;
                            case "JU": // skip the given number of instructions 1_JU is like a no-op! (0_JU is infinite loop!)
                                d1 = stack.Pop();
                                nextindex = index + (int)d1;
                                break;
                            case "LS": // left shift 3_1_LS is 6 (11 -> 110)
                                d1 = stack.Pop();
                                d2 = stack.Pop();
                                value = ((int)d2) << ((int)d1);
                                stack.Push(value);
                                break;
                            case "NO": // no-op
                                break;
                            case "PO": // pop
                                d1 = stack.Pop();
                                break;
                            case "RS": // right shift 5_1_RS is 2 (101 -> 10)
                                d1 = stack.Pop();
                                d2 = stack.Pop();
                                value = ((int)d2) >> ((int)d1);
                                stack.Push(value);
                                break;
                            case "SW": // swap
                                d1 = stack.Pop();
                                d2 = stack.Pop();
                                stack.Push(d1);
                                stack.Push(d2);
                                break;

                            case "XY": // 10_2_XY is 2**10
                                d1 = stack.Pop();
                                d2 = stack.Pop();
                                value = Math.Pow(d1, d2);
                                stack.Push(value);
                                break;
                            case "YX": // 10_2_YX is 10**2
                                d1 = stack.Pop();
                                d2 = stack.Pop();
                                value = Math.Pow(d2, d1);
                                stack.Push(value);
                                break;
                            case "ZE": // ZEro is an easy masking operation; if <n> is on the stack then 3_ZE will clear the bottom 2 bits
                                d1 = stack.Pop();
                                d2 = stack.Pop();
                                value = ((int)d2) & ~((int)d1);
                                stack.Push(value);
                                break;
                        }
                    }
                    else if (command.Length >= 1 && command[0] == '$')
                    {
                        // Is a string constant. 
                        // String constants include escaped chars like \s=SP \p=BAR (pipe) \c=CARET
                        // TODO: that's not how space etc is escaped after re-checking the JSON spec :-(
                        // and also escaped chars for \r\n
                        // They include all C escapes except for octal and \uXXXX is the code for Unicode
                        var constString = UnescapeString (command.Substring(1));
                        stringstack.Push(constString);
                    }
                    else
                    {
                        double value = 0.0;
                        var parseOk = double.TryParse(command, out value);
                        if (!parseOk)
                        {
                            Error($"ERROR: ValueCalculate: not a constant {command} index {index} command {str}");
                            return new CalculateResult(double.NaN, null);
                        }
                        stack.Push(value);
                    }
                    index = nextindex;

                    // This is for small calculations, not giant ones!
                    if (stack.Count > 10)
                    {
                        Error($"ERROR: ValueCalculate: stack too large {stack.Count} index {index} command {str}");
                        return new CalculateResult(double.NaN, null);
                    }
                }

                double d = stack.Count > 0 ? stack.Pop() : double.NaN;
                string s = stringstack.Count > 0 ? stringstack.Pop() : null;
                return new CalculateResult(d, s);
            }
            catch (Exception)
            {
                Error($"ERROR: ValueCalculate: not enough params index {index} command {str}");
                return new CalculateResult(double.NaN, null);
            }
        }

        private static void Error (string text)
        {
            System.Diagnostics.Debug.WriteLine(text);
            ;
        }
    }
}
```


# CopyVariableSetCSFile FileName=VariableSet.cs DirName=CsBluetoothFoundation SuppressFile=:SuppressCSharpProtocol:


```
using BluetoothDeviceController.Names;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothDeviceController.BleEditor
{
    public class VariableSet
    {
        public VariableSet()
        {
            PreviousValues = new Dictionary<string, double>();
            CurrValues = new Dictionary<string, double>();
            CurrStringValues = new Dictionary<string, string>();
        }
        Dictionary<string, double> PreviousValues = null;
        Dictionary<string, double> CurrValues = null;
        Dictionary<string, string> CurrStringValues = null;

        public double GetCurrDouble(string name)
        {
            return CurrValues[name];
        }
        public void SetCurrDouble(string name, double value)
        {
            CurrValues[name] = value;
        }
        public string GetCurrString(string name)
        {
            return CurrStringValues[name];
        }
        public void SetCurrString(string name, string value)
        {
            CurrStringValues[name] = value;
        }
        public double GetPreviousDouble(string name)
        {
            return PreviousValues[name];
        }
        public void SetPreviousDouble(string name, double value)
        {
            PreviousValues[name] = value;
        }
        public double GetDeltaDouble(string name)
        {
            return CurrValues[name] - PreviousValues[name];
        }

        public void Clear()
        {
            CurrValues.Clear();
            PreviousValues.Clear();
            CurrStringValues.Clear();
        }

        public void CopyToPrev()
        {
            foreach (var (name, value) in CurrValues)
            {
                PreviousValues[name] = value;
            }
        }
        public void Init(Dictionary<string, double> values)
        {
            foreach (var (name, newValue) in values)
            {
                CurrValues[name] = newValue;
            }
        }
        public void Init(Dictionary<string, VariableDescription> values)
        {
            foreach (var (name, newValue) in values)
            {
                if (newValue.CurrValueIsString)
                {
                    CurrStringValues[name] = newValue.InitString;
                }
                else
                {
                    CurrValues[name] = newValue.Init;
                }
            }
        }
        public void SetCurrValue(Dictionary<string, VariableDescription> values)
        {
            foreach (var (name, newValue) in values)
            {
                if (newValue.CurrValueIsString)
                {
                    CurrStringValues[name] = newValue.CurrValueString;
                }
                else
                {
                    CurrValues[name] = newValue.CurrValue;
                }
            }
        }
    }
}
```


# CopySerialConfigCSFile FileName=SerialConfig.cs DirName=CsBluetoothFoundation SuppressFile=:SuppressCSharpProtocol:



```
using BluetoothDeviceController.BleEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace BluetoothDeviceController.Names
{
    public class SerialConfig
    {
        public string Id { get; set; } = "(name)";
        public string DeviceName { get; set; } = ""; // e.g. SlantRobotics is "Dev B"
        public string Name { get; set; } = "(name)";
        public string ClassName { get; set; }
        public string GetClassName() { return String.IsNullOrEmpty(ClassName) ? (Name ?? "") : ClassName; }
        public string ClassModifiers { get; set; }
        public string Description { get; set; }
        public string DefaultPin { get; set; }
        public List<string> Aliases { get; set; } = new List<string>(); // Must not be null
        public IList<String> Links { get; } = new List<String>();

        public Dictionary<string, VariableDescription> Settings = new Dictionary<string, VariableDescription>();
        public Dictionary<string, Command> Commands = new Dictionary<string, Command>();

        public override string ToString()
        {
            return $"{Name} Settings #{Settings.Count} Commands #{Commands.Count}";
        }
        public bool IsMatch(string deviceName, string id)
        {
            // The matches are optimistic -- goal is I can ask for deviceName="" id="Slant-Robotics-LittleBot" and get
            // the exact one I want.
            //var btMatch = BluetoothName == "" || bluetoothName == "" || bluetoothName.StartsWith(BluetoothName);
            var idMatch = id == "" || (!String.IsNullOrEmpty(Id) && id == Id);
            if (id == "(name)") idMatch = false; // default names don't match!
            var devMatch = deviceName == "" || (!String.IsNullOrEmpty(DeviceName) && deviceName.StartsWith(DeviceName));
            var match = idMatch && devMatch;
            return match;
        }
    }

    public class VariableDescription
    {
        public string Label { get; set; } = null;
        public enum UiType { TextBox, Slider, Hide, ComboBox };

        public string Name { get; set; } = null;
        public double Min { get; set; } = 0.0;
        public double Max { get; set; } = 100.0;
        public double Init { get; set; } = 0.0;
        public string InitString { get; set; } = "";
        public double CurrValue { get; set; } = 0.0;
        public string CurrValueString { get; set; } = "";
        public bool CurrValueIsString { get; set; } = false;
        /// <summary>
        /// Set of enum values e.g. ValueNames: { "All":0, "Lights_Off":1, "Stop":2, "Mute":3 }
        /// </summary>
        public Dictionary<string, double> ValueNames { get; } = new Dictionary<string, double>();
        /// <summary>
        /// Input type, one of TextBox (default), Slider, Hide, and ComboBox or ComboBoxOrSlider. The ComboBox
        /// will only be used when ValueNames are provided.
        /// </summary>
        public UiType InputType { get; set; } = UiType.TextBox;
        public string OnChange { get; set; } = null;
        public string CmdName { get; set; }
        public override string ToString()
        {
            return $"Settings {Name} Min={Min} Init={Init} Max={Max}";
        }
    }
    public interface IWriteCharacteristic
    {
        Task<GattWriteResult> DoWriteString(string str);
    }
    public class Command
    {
        public string Label { get; set; } = null;
        /// <summary>
        /// Compute is a computed value: e.g. STRING^$BERO1.0. Values like space must be escaped. Use Replace for simple scenarios that involve just sending a string.
        /// Values are seperated by spaces e.g "${CLEAR[ $ClearMode_GN $]}" is split into three values
        /// which will be concatenated together.
        /// </summary>
        public string Compute { get; set; } = "";
        /// <summary>
        /// A simple string to send to the device. Why replace? Because why not, that's why.
        /// </summary>
        public string Replace { get; set; } = "";
        /// <summary>
        /// The name of a file like SerialFiles/Espruino_Modules_Smartibot.js
        /// The file will be in the Assets directory.
        /// </summary>
        public string ReplaceFile { get; set; } = "";
        public string Alt { get; set; } = "";
        /// <summary>
        /// When set to true, the command is hidden
        /// </summary>
        public bool IsHidden { get; set; } = false;
        public string OnChange { get; set; } = null;
        /// <summary>
        /// Provides explicit values for the values needed by a command
        /// </summary>
        public Dictionary<string, double> Set { get; } = new Dictionary<string, double>();

        /// <summary>
        /// Variables that can be used with the command. These are per-command.
        /// </summary>
        public Dictionary<string, VariableDescription> Parameters { get; } = new Dictionary<string, VariableDescription>();

        public IWriteCharacteristic WriteCharacteristic { get; set; } = null;

        VariableSet Variables = new VariableSet();
        public void InitVariables()
        {
            Variables.Init(Set);
            Variables.Init(Parameters);
        }

        public string DoCompute()
        {
            Variables.SetCurrValue(Parameters);

            var list = Compute.Split(new char[] { ' ' });
            var cmd = "";
            foreach (var strcommand in list)
            {
                var calculateResult = BleEditor.ValueCalculate.Calculate(strcommand, double.NaN, null, Variables);
                cmd += calculateResult.S ?? calculateResult.D.ToString();
            }
            Variables.CopyToPrev();
            return cmd;
        }
        public async Task DoCommand()
        {
            if (WriteCharacteristic == null) return; // can happen at startup
            var cmd = DoCompute();
            var result = await WriteCharacteristic.DoWriteString(cmd);
            System.Diagnostics.Debug.WriteLine($"COMMAND: {cmd}");
            ;
        }

        public void SetCurrDouble(string name, double value)
        {
            Parameters[name].CurrValue = value;
        }
    }
}
```



# CopyDataReaderReadStringRobustCSFile FileName=DataReaderReadStringRobust.cs DirName=CsBluetoothFoundation SuppressFile=:SuppressCSharpProtocol:



```
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;

namespace Utilities
{
    class DataReaderReadStringRobust
    {
        public enum ReadStatus {  OK, Hex };
        public static (string, ReadStatus) ReadString(DataReader dr, uint len)
        {
            string stritem;
            var readStatus = ReadStatus.Hex;
            try
            {
                stritem = dr.ReadString(len); // len is often dr.UnconsumedBufferLength
                readStatus = ReadStatus.OK;
            }
            catch (Exception)
            {
                // The string can't be read. Let's try reading as a buffer instead.
                stritem = "";
                IBuffer buffer = dr.ReadBuffer(dr.UnconsumedBufferLength);
                for (uint ii = 0; ii < buffer.Length; ii++)
                {
                    if (ii != 0) stritem += " ";
                    stritem += buffer.GetByte(ii).ToString("X2");
                }
            }
            return (stritem, readStatus);
        }
        public static (string, ReadStatus) ReadStringEntire(DataReader dr)
        {
            return ReadString(dr, dr.UnconsumedBufferLength);
        }
    }
}
```


# CopyEddystoneCSFile FileName=Eddystone.cs DirName=CsBluetoothFoundation SuppressFile=:SuppressCSharpProtocol:



```
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothDeviceController.BluetoothDefinitionLanguage
{
    public static class Eddystone
    {
        public static string EddystoneUrlToString (string url)
        {
            // Takes in an eddystone-encoded URL
            // char 0 = 0=http://www. 1=https://www. 2=http:// 3=https://
            string result = "";
            char b1 = url[0];
            switch (b1)
            {
                case '\x00': result = "http://www."; break;
                case '\x01': result = "https://www."; break;
                case '\x02': result = "http://"; break;
                case '\x03': result = "https://"; break;
            }
            for (int i=1; i<url.Length; i++)
            {
                var ch = url[i];
                switch (ch)
                {
                    case '\x00': result += ".com/"; break;
                    case '\x01': result += ".org/"; break;
                    case '\x02': result += ".edu/"; break;
                    case '\x03': result += ".net/"; break;
                    case '\x04': result += ".info/"; break;
                    case '\x05': result += ".biz/"; break;
                    case '\x06': result += ".gov/"; break;
                    case '\x07': result += ".com"; break;
                    case '\x08': result += ".org"; break;
                    case '\x09': result += ".edu"; break;
                    case '\x0a': result += ".net"; break;
                    case '\x0b': result += ".info"; break;
                    case '\x0c': result += ".biz"; break;
                    case '\x0d': result += ".gov"; break;
                    default:
                        result += ch; //NOTE: technically wrong. The spec says that 14..32 and 127..255 are reserved.
                        break;
                }
            }

            return result;
        }
    }
}
```



# CopyGuidGetCommonCSFile FileName=GuidGetCommon.cs DirName=CsBluetoothFoundation SuppressFile=:SuppressCSharpProtocol:



```
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothDeviceController.Names
{
    public static class GuidGetCommon
    {
        /// <summary>
        /// Given a string like BluetoothLE#BluetoothLEbc:83:85:22:5a:70-5c:31:3e:89:ad:5c return 5c:31:3e:89:ad:5c
        /// which is the Bluetooth address (the stuff before the - is the Windows ID. The GUID 83:85:22... will
        /// change at some interval, so it cannot be relied on.
        /// </summary>
        /// <param name="id">String like BluetoothLE#BluetoothLEbc:83:85:22:5a:70-5c:31:3e:89:ad:5c</param>
        /// <returns></returns>
        public static string NiceId(string id, string prefixIfReplaced="") // was DeviceInformation args)
        {
            var retval = id;
            var idx = retval.IndexOf('-');
            if (retval.StartsWith("BluetoothLE#BluetoothLE") && idx >= 0)
            {
                retval = prefixIfReplaced + retval.Substring(idx + 1);
            }
            return retval;
        }
        /// <summary>
        /// Given two bluetooth GUIDs, return a short version of string B such that it contains
        /// the characters that are different from A such that the differences are "bluetooth-like".
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public static string GetCommon (this string A, string B)
        {
            //    0..7   9..12 14..17 19..22 24..35
            // A: EF680400-9B35-4933-9B10-52FFA9740042
            // B: EF680404-9B35-4933-9B10-52FFA9740042
            // Return: 0404

            // A: 9dc84838-7619-4f09-a1ce-ddcf63225b10
            // B: 9dc84838-7619-4f09-a1ce-ddcf63225b11
            // Return: 5b10

            var m1 = A.SubstringEqual(B, 0, 4);
            var m2 = A.SubstringEqual(B, 4, 4);
            var m3 = A.SubstringEqual(B, 9, 4);
            var m4 = A.SubstringEqual(B, 14, 4);
            var m5 = A.SubstringEqual(B, 19, 4);
            var m6 = A.SubstringEqual(B, 24, 8);
            var m7 = A.SubstringEqual(B, 32, 4);
            var commonMatch = m1 && !m2 && m3 && m4 && m5 && m6 && m7;
            if (commonMatch) return B.Substring(4, 4);

            commonMatch = m1 && m2 && m3 && m4 && m5 && m6 && !m7;
            if (commonMatch) return B.Substring(32, 4);

            return B;
        }

        public static int Test()
        {
            int NError = 0;
            NError += TestGetCommon();
            return NError;
        }

        private static int TestGetCommon()
        {
            int NError = 0;
            // Not a match in various ways
            NError += TestGetCommonOne("EF680400-9B35-4933-9B10-52FFA9740042", "EF680400-9B35-4933-9B10-52FFA9740042", "EF680400-9B35-4933-9B10-52FFA9740042");
            NError += TestGetCommonOne("EF680400-9B35-4933-9B10-52FFA9740042", "EF680404-9B35-4933-9B10-52FFA9740044", "EF680404-9B35-4933-9B10-52FFA9740044");

            // Proper match
            NError += TestGetCommonOne("EF680400-9B35-4933-9B10-52FFA9740042", "EF680404-9B35-4933-9B10-52FFA9740042", "0404");
            NError += TestGetCommonOne("9dc84838-7619-4f09-a1ce-ddcf63225b10", "9dc84838-7619-4f09-a1ce-ddcf63225b11", "5b11");

            return NError;
        }
        private static int TestGetCommonOne(string A, string B, string expected)
        {
            int NError = 0;
            try
            {
                var actual = A.GetCommon(B);
                if (actual != expected)
                {
                    NError++;
                    System.Diagnostics.Debug.WriteLine($"ERROR: TestGetCommon ({A}, {B}) expected {expected} actually got {actual}");
                }
            }
            catch (Exception e)
            {
                NError++;
                System.Diagnostics.Debug.WriteLine($"ERROR: TestGetCommon ({A}, {B}) threw exception {e.Message}");
            }


            return NError;
        }

        public static bool SubstringEqual (this string A, string B, int startIndex, int length)
        {
            var partA = A.Substring(startIndex, length);
            var partB = B.Substring(startIndex, length);
            return partA == partB;
        }
        /// <summary>
        /// Sets the UUID as a string. Will take in short version (like 1800) but will
        /// always return full UUID values.
        /// </summary>
        public static string AsFullUuid(this string uuidRaw)
        {
            var retval = uuidRaw;
            if (retval.Length == 4) // for example, Common Configuration is 1800
            {
                // A value like 1800 (Common Configuration) or 2a00 (Device Name)
                // 00002a00-0000-1000-8000-00805f9b34fb
                retval = "0000" + retval + "-0000-1000-8000-00805f9b34fb";
            }
            return retval;
        }

        /// <summary>
        /// Returns the shortest UUID. Will convert long UUID like 00002a00-0000-1000-8000-00805f9b34fb
        /// into the short version 2a00. Will not shorten non-standard UUIDs
        /// </summary>
        /// <param name="uuidRaw"></param>
        /// <returns></returns>
        public static string AsShortestUuid(this string uuidRaw)
        {
            var retval = uuidRaw.ToLower();
            if (retval.Length == 36) // for example, Common Configuration is 1800
            {
                if (retval.StartsWith ("0000") && 
                    retval.EndsWith("-0000-1000-8000-00805f9b34fb"))
                {
                    retval = retval.Substring(4, 4);
                }
                // A value like 1800 (Common Configuration) or 2a00 (Device Name)
                // 00002a00-0000-1000-8000-00805f9b34fb
            }
            return retval;
        }
    }
}
```



# CopyBluetoothAddressCSFile FileName=BluetoothAddress.cs DirName=CsBluetoothFoundation SuppressFile=:SuppressCSharpProtocol:



```

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothDeviceController.Beacons
{
    class BluetoothAddress
    {
        /// <summary>
        /// Converts a bluetooth address provided as a long into a useful string
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static string AsString(ulong address)
        {
            var b5 = (byte)((address >> 40) & 0xFF);
            var b4 = (byte)((address >> 32) & 0xFF);
            var b3 = (byte)((address >> 24) & 0xFF);
            var b2 = (byte)((address >> 16) & 0xFF);
            var b1 = (byte)((address >>  8) & 0xFF);
            var b0 = (byte)((address >>  0) & 0xFF);
            var retval = $"{b5:X2}:{b4:X2}:{b3:X2}:{b2:X2}:{b1:X2}:{b0:X2}";
            return retval;
        }
    }
}
```



# CopyUIThreadHelperCSFile FileName=UIThreadHelper.cs DirName=CsBluetoothFoundation SuppressFile=:SuppressCSharpProtocol:


```
using System;

namespace Utilities
{
    static class UIThreadHelper
    {
        /// <summary>
        /// Returns TRUE iff the current thread is the UI thread
        /// </summary>
        /// <returns></returns>
        public static bool IsOnUIThread()
        {
            var dispather = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
            var retval = dispather.HasThreadAccess;
            return retval;
        }

        /// <summary>
        /// Calls the given function either directly or on the UI thread the TryRunAsync(). The resulting task is just thrown away.
        /// </summary>
        /// <param name="f"></param>
        /// <param name="priority"></param>
        public static void CallOnUIThread(Action f, Windows.UI.Core.CoreDispatcherPriority priority = Windows.UI.Core.CoreDispatcherPriority.Low)
        {
            if (IsOnUIThread())
            {
                f();
            }
            else
            {
                var dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
                var task = dispatcher.TryRunAsync(priority, () => { f(); });
            }
        }
    }
}
```


