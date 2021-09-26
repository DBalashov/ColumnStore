using System;
using System.Collections.Generic;
using System.Linq;

namespace ColumnStore
{
    static class GroupToDictionaryExtenders
    {
        internal static Dictionary<int, KeyValue<V>[]> GroupToDictionary<V>(this Dictionary<CDT, V> values, Func<CDT, int> truncator)
        {
            var r    = new Dictionary<int, KeyValue<V>[]>();
            var vals = new List<KeyValue<V>>();

            int key = 0;
            foreach (var kv in values.OrderBy(p => p.Key))
            {
                if (key == 0)
                    key = truncator(kv.Key);
                if (truncator(kv.Key) == key)
                    vals.Add(new KeyValue<V>(kv.Key.Value, kv.Value));
                else
                {
                    r.Add(key, vals.ToArray());
                    key = kv.Key.Value;
                    vals.Clear();
                    vals.Add(new KeyValue<V>(kv.Key.Value, kv.Value));
                }
            }

            if (key > 0 && vals.Any())
                r.Add(key, vals.ToArray());

            return r;
        }
    }
}