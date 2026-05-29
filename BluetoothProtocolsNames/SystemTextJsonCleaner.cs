using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothWinUI3
{
    // Utility to convert an object graph to JsonNode while omitting
    // empty strings and empty enumerables/arrays. Uses System.Text.Json only.
    public static class SystemTextJsonCleaner
    {
        public static JsonNode ToJsonNode(object obj)
        {
            if (obj == null) return null;
            return WriteValue(obj);
        }

        private static JsonNode WriteValue(object obj)
        {
            if (obj == null) return null;
            var type = obj.GetType();

            if (type == typeof(string))
            {
                var s = (string)obj;
                if (string.IsNullOrEmpty(s)) return null;
                return JsonValue.Create(s);
            }

            if (type.IsPrimitive || type.IsEnum)
            {
                return JsonValue.Create(obj);
            }

            if (obj is decimal || obj is double || obj is float)
            {
                return JsonValue.Create(obj);
            }

            if (obj is IEnumerable && !(obj is string))
            {
                var arr = new JsonArray();
                foreach (var item in (IEnumerable)obj)
                {
                    var child = WriteValue(item);
                    if (child != null)
                        arr.Add(child);
                }
                if (arr.Count == 0) return null;
                return arr;
            }

            // For objects, reflect properties
            var jobj = new JsonObject();
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var p in props)
            {
                if (!p.CanRead) continue;
                var val = p.GetValue(obj);
                bool shouldWrite = true;
                AttributeCollection attr = TypeDescriptor.GetProperties(obj)[p.Name].Attributes;;
                var dva = attr[typeof(DefaultValueAttribute)] as DefaultValueAttribute;
                if (dva != null)
                {
                    if (val.Equals(dva.Value)) // can't use == because that does object equality
                    {
                        shouldWrite = false;
                        ; // is the default
                    }
                    else
                    {
                        ; // is not the default
                    }
                }
                var jsonignore = attr[typeof(JsonIgnoreAttribute)] as JsonIgnoreAttribute;
                if (jsonignore != null)
                {
                    // SHORTCOMING: In theory there's a bunch of conditions. But I will ignore them all.
                    shouldWrite = false;
                    switch (jsonignore.Condition)
                    {
                        case JsonIgnoreCondition.Always:
                            shouldWrite = false;
                            break;
                        default:
                            Log("Error: the only [JsonIgnore] allowed is the default");
                            break;
                    }
                }
                if (shouldWrite)
                {
                    var child = WriteValue(val);
                    if (child == null) continue;
                    jobj[p.Name] = child;
                }
            }
            if (jobj.Count == 0) return null;
            return jobj;
        }

        static void Log(string str)
        {
            Console.WriteLine(str);
            System.Diagnostics.Debug.WriteLine(str);
        }
    }
}
