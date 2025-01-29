using BluetoothDeviceController.Names;
using System.Collections.Generic;

namespace BluetoothProtocols.IotNumberFormats
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
