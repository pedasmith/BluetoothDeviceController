using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothDeviceController.SpecialtyPagesCustom
{
    public class OscDataRecord
    {
        public OscDataRecord() { }
        public OscDataRecord(DateTime time, double value)
        {
            EventTime = time;
            Value = value;
        }
        public DateTime EventTime { get; set; }
        public double Value { get; set; }

        /// <summary>
        /// Get the Time EventTime property as a PropertyInfo. This is needed for initializing
        /// a ChartControl
        /// </summary>
        /// <returns></returns>
        public static PropertyInfo GetTimeProperty()
        {
            var EventTimeProperty = typeof(OscDataRecord).GetProperty("EventTime");
            return EventTimeProperty;
        }

        public static List<PropertyInfo> GetValuePropertyList()
        {
            var properties = new System.Collections.Generic.List<System.Reflection.PropertyInfo>()
                {
typeof(OscDataRecord).GetProperty("Value"),
                };
            return properties;
        }

        public static List<string> GetNames()
        {
            var names = new List<string>()
                {
"Osc.",
                };
            return names;
        }

        public override string ToString()
        {
            return $"{Value:F2} at {EventTime.Second}.{EventTime.Millisecond}";
        }
    }

}
