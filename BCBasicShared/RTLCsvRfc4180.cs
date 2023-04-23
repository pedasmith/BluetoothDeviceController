using BCBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedCalculator.BCBasic.RunTimeLibrary
{
    // RFC 4180 has a complete description of the CSV format
    class RTLCsvRfc4180
    {
        public static string Encode(string data)
        {
            string Retval = data;
            if (Retval == null) return "";
            // double-quote is escaped by doubling it.
            bool mustQuote = false;
            if (Retval.Contains('"'))
            {
                mustQuote = true;
                Retval = Retval.Replace("\"", "\"\""); // replace each individual double-quote with two double-quotes
            }
            if (Retval.Contains(',') || Retval.Contains('\r') || Retval.Contains('\n'))
            {
                mustQuote = true;
            }

            if (mustQuote)
            {
                Retval = "\"" + Retval + "\"";
            }
            return Retval;
        }

        public static string Encode(BCValueList array)
        {
            string Retval = "";
            bool firstOnLine = true;
            bool lastIsArray = false;
            foreach (var item in array.data)
            {
                if (!firstOnLine) Retval += ",";
                firstOnLine = false;

                string itemstr;
                if (item.IsArray)
                {
                    firstOnLine = true; // next thing will be the first on the line
                    itemstr = Encode(item.AsArray); // will add the CR/LF
                    Retval += itemstr;
                    lastIsArray = true;
                }
                else
                {
                    itemstr = item.AsString;
                    Retval += Encode(itemstr);
                    lastIsArray = false;
                }
            }
            // Arrays already have the CR-LF, so adding this additional one makes for too many lines.
            if (!lastIsArray) Retval += "\r\n";
            return Retval;
        }

        enum State { AboutToReadField, Field, FieldQuoted };
        private static void AddField (BCValueList row, BCValueList topRow, ref string field)
        {
            var name = "";
            if (row != topRow && row.data.Count < topRow.data.Count)
            {
                name = topRow.data[row.data.Count].AsString;
            }
            double dvalue;
            bool canParseAsDouble = Double.TryParse(field, out dvalue);
            BCValue value = canParseAsDouble ? new BCValue (dvalue) : new BCValue(field);

            if (name == "")
            {
                row.data.Add(value);
            }
            else
            {
                row.AddPropertyAllowDuplicates(name, value);
            }
            field = "";
        }
        public static BCValueList Parse(string str)
        {
            BCValueList row = new BCValueList();
            BCValueList table = new BCValueList();
            BCValueList toprow = row; // stays the same

            string field = "";
            State state = State.AboutToReadField;
            for (int i = 0; i < str.Length; i++)
            {
                char ch = str[i];
                char nextch = i + 1 < str.Length ? str[i + 1] : '\0';

                UpdateStateAndRetry:
                switch (state)
                {
                    case State.AboutToReadField:
                        switch (ch)
                        {
                            case '"':
                                // Don't put the quote into the field.
                                state = State.FieldQuoted;
                                break;
                            default:
                                state = State.Field;
                                goto UpdateStateAndRetry;
                        }
                        break;
                    case State.Field:
                        switch (ch)
                        {
                            case ',':
                                AddField(row, toprow, ref field);
                                state = State.AboutToReadField;
                                break;
                            case '\r':
                                if (nextch == '\n') // got a CRLF; treat as one
                                {
                                    i++;
                                }
                                AddField(row, toprow, ref field);

                                // Add-row
                                table.data.Add(new BCValue(row));
                                row = new BCValueList();

                                state = State.AboutToReadField;
                                break;
                            case '\n':
                                AddField(row, toprow, ref field);

                                // Add-row
                                table.data.Add(new BCValue(row));
                                row = new BCValueList();

                                state = State.AboutToReadField;
                                break;
                            default:
                                field += ch;
                                break;
                        }
                        break;
                    case State.FieldQuoted:
                        switch (ch)
                        {
                            case '"':
                                if (nextch == '"')
                                {
                                    // e.g., 1,2,"this is ""quoted""",4,5
                                    field += ch;
                                    i += 1; // skip over the doubled quote
                                }
                                else
                                {
                                    // If the files were always correct, we could just add the
                                    // field, skip the next comma and switch to AboutToReadField.
                                    // But if there are characters after the double-quote,
                                    // they should be appended to the field as if in Field mode.
                                    // So skip over the quote and switch to field mode.
                                    state = State.Field; 
                                }
                                break;

                            default:
                                field += ch;
                                break;
                        }
                        break;
                }
            }
            // We might have ended with a partial field e.g., 1,2,3 <-- field just ends
            if (field != "")
            {
                // Add-field
                row.data.Add(new BCValue(field));
                field = "";
            }
            if (row.data.Count > 0)
            {
                // Add-row
                table.data.Add(new BCValue(row));
                row = new BCValueList();
            }
            return table;
        }

        public static int TestParseCsv()
        {
            var csv = @"Year,Make,Model,Description,Price
1997,Ford,E350,""ac, abs, moon"",3000.00
1999,Chevy,""Venture """"Extended Edition"""""","""",4900.00
1999,Chevy,""Venture """"Extended Edition, Very Large"""""",,5000.00
1996,Jeep,Grand Cherokee,""MUST SELL!
air, moon roof, loaded"",4799.00""";
            int NError = 0;
            var tbl = Parse(csv);

            var err = "";
            if (tbl.data.Count != 5)
            {
                err += $"ERROR:CSV: table count is {tbl.data.Count} expected 5.\n";
                NError++;
            }
            else
            {
                for (int i = 0; i < tbl.data.Count; i++)
                {
                    if (!tbl.data[i].IsArray)
                    {
                        err += $"ERROR:CSV: table {i} type is {tbl.data[i].CurrentType} expected an array.\n";
                        NError++;
                    }
                    else
                    {
                        // Each row is 5 long
                        if (tbl.data[i].AsArray.data.Count != 5)
                        {
                            err += $"ERROR:CSV: table{i} count is {tbl.data[i].AsArray.data.Count} expected 5.\n";
                            NError++;
                        }
                    }
                }
                // Now do spot-checks on the data
                var val = tbl.data[0].AsArray.data[1].AsString;
                if (val != "Make")
                {
                    err += $"ERROR:CSV: table[0,1] is {val} should be Make.\n";
                    NError++;
                }
                val = tbl.data[2].AsArray.data[2].AsString;
                if (val != "Venture \"Extended Edition\"")
                {
                    err += $"ERROR:CSV: table[0,1] is {val} should be Venture 'Extended Edition'.\n";
                    NError++;
                }

                // And check some of the names
                var row = tbl.data[3].AsArray;
                val = row.GetValue("Price").AsString;
                if (val != "5000")
                {
                    err += $"ERROR:CSV: table[3,Price] is {val} should be 5000.00 \n";
                    NError++;
                }
            }


            if (err != "" && NError == 0) { err += "ERROR:CSV: miscounted the errors\n"; NError++; } // error: unknown error!
            return NError;
        }
    }

}
