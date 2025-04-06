using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace BluetoothProtocols.IotNumberFormats
{
    /// <summary>
    /// Represents a parsed list of IotNumber Formats. In original string form, they look like "U8|HEX|Opcode U16|DEC|Value U16|HEX|Options||0"
    /// where it's a list of fields seperated by spaces; each field has multiple sections (type, display format, name, units, default value) 
    /// seperated by vertical bars (|); each section can have multiple sub-fields seperated by carets (^). For a small number of sub-fields,
    /// there are multiple items which are seperated by underscores (_).
    /// 
    /// see: https://shipwrecksoftware.wordpress.com/2019/10/13/modern-iot-number-formats/
    /// </summary>
    public class ParserFieldList
    {
        public IList<ParserField> Fields = new List<ParserField>();
        public Dictionary<string, ParserField> ODEs = new Dictionary<string, ParserField>();

        /// <summary>
        /// ODRRecord is like ODR^Temperature_Humidity||EnvironmentData where the Temperature and Humidity are names
        /// of ODE elements (ODE=optional define element and ODR=optional define record)
        /// </summary>
        public class ODRRecord
        {
            public ODRRecord(ParserField field)
            {
                Name = field.NamePrimary;
                var allOdeNames = field.Get(0, 1); // for ODR^Temperature_Pressure||EnvironmentalData return Temperature_Pressure
                var list = allOdeNames.Split(new char[] { '_' });
                ODENames = list;
            }
            public string Name = "not-set";
            public IList<string> ODENames = new List<string>();

        }
        private Dictionary<string, ODRRecord> ODRs = new Dictionary<string, ODRRecord>();
        public ParserFieldList Globals = null;
        public IList<ParserField> GetRecordDefinition(string recordname)
        {
            ODRRecord definition = null;
            var isLocal = ODRs.TryGetValue(recordname, out definition);
            if (!isLocal && Globals != null)
            {
                return Globals.GetRecordDefinition(recordname);
            }
            if (!isLocal)
            {
                ParserField.Log($"ERROR: looking for {recordname} but cant find it (no global)");
                return null; // never return a partial value. Either we're all the way successful, or failed completely.
            }
            var retval = new List<ParserField>();
            var names = definition.ODENames;
            foreach (var name in names)
            {
                var value = GetElementDefinition(name);
                if (value == null)
                {
                    ParserField.Log($"ERROR: looking for {name} for {recordname} but cant find it");
                    return null; // never return a partial value. Either we're all the way successful, or failed completely.
                }
                retval.Add(value);
            }

            return retval;
        }

        private ParserField GetElementDefinition(string elementName)
        {
            ParserField retval = null;
            var isLocal = ODEs.TryGetValue(elementName, out retval);
            if (!isLocal && Globals != null)
            {
                return Globals.GetElementDefinition(elementName);
            }
            return retval;
        }
        public static ParserFieldList ParseLine(string value, ParserFieldList globalFields=null)
        {
            var Retval = new ParserFieldList();
            Retval.Globals = globalFields;

            var split = value.Split(ParserField.Space);
            for (int i = 0; i < split.Length; i++)
            {
                var item = new ParserField(split[i]);
                switch (item.FieldTypePrimary)
                {
                    case ParserField.FieldType.Regular:
                        Retval.Fields.Add(item);
                        break;
                    case ParserField.FieldType.ODE:
                        // Add to the ODEs using the field's name
                        Retval.ODEs.Add(item.NamePrimary, item);
                        break;
                    case ParserField.FieldType.ODR:
                        // Add to the ODEs using the field's name
                        Retval.ODRs.Add(item.NamePrimary, new ODRRecord(item));
                        break;
                }
            }

            // Now go back and fill in the RemainingSize value for the last BYTES or STRING item.
            var remainder = 0;
            for (int i = Retval.Fields.Count - 1; i >= 0; i--)
            {
                var item = Retval.Fields[i];
                if (item.NBytes >= 0) remainder += item.NBytes;
                else if (item.NBytes <= -2) remainder = -2; // meaning we couldn't calculate the amount
                else if (item.NBytes == -1)
                {
                    if (remainder <= -2)
                    {
                        // There was a error; spit it out.
                        ParserField.Log($"ERROR: ValueParserSplit:ParseLine: remainder is {remainder} at index {i}; that's a failure");
                    }
                    else if (remainder == -1)
                    {
                        // Also a error, but a different one
                        ParserField.Log($"ERROR: ValueParserSplit:ParseLine: remainder is {remainder} at index {i}");
                    }
                    else if (remainder > 0)
                    {
                        item.MaxBytesRemaining = remainder; // if the BYTE is the last item, keep MaxBytesRemaining the same.
                    }
                    break; // Only fill in one value.
                }
            }
            return Retval;
        }

        public override string ToString()
        {
            var retval = "";
            foreach (var field in Fields)
            {
                retval += " " + field.ToString();
            }
            return retval;
        }
    }
}
