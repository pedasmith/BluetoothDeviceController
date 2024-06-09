using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothDeviceController.SpecialtyPagesCustom
{
    public class TriggerSetting
    {
        public enum TriggerType { None, Rising }; // TODO: should falling trigger, too!
        public TriggerType Trigger = TriggerType.Rising;
        public double Level { get; set; } = 2.5;
        public override string ToString()
        {
            return $"{Trigger} Level={Level}";
        }

        public List<int> FindTriggeredIndex(IList<OscDataRecord> list)
        {
            List<int> retval = new List<int>();
            var index = 0;
            do
            {
                var next = FindNextTriggeredIndex(index, list);
                if (next > 0)
                {
                    retval.Add(next);
                }
                index = next;
            }
            while (index > 0);
            return retval;
        }

        private int FindNextTriggeredIndex(int startIndex, IList<OscDataRecord> list)
        {
            if (list.Count < 2) return 0;
            if (Trigger == TriggerType.None) return 0;

            double prev = list[startIndex].Value;
            for (int i = startIndex + 1; i < list.Count; i++)
            {
                var curr = list[i].Value;
                if (prev < Level && curr >= Level)
                {
                    return i;
                }
                prev = curr;
            }
            return 0;
        }

        public string ZZZAdjustDataTimeFromTriggerIndex(int triggerIndex, DateTime lineStartTime, IList<OscDataRecord> list)
        {
            var logstr = "";
            var triggerTime = list[triggerIndex].EventTime;
            var delta = triggerTime.Subtract(lineStartTime);
            logstr = $" Trigger: time={triggerTime} delta(ms)={delta.TotalMilliseconds}";
            for (int i = 0; i < list.Count; i++)
            {
                var value = list[i];
                value.EventTime = value.EventTime.Subtract(delta);
            }
            return logstr;
        }
    }
}

