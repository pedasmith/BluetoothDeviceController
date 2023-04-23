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
