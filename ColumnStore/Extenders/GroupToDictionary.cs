using System;
using System.Collections.Generic;
using System.Linq;

namespace ColumnStore;

static class GroupToDictionaryExtenders
{
    internal static Dictionary<int, KeyValue<V>[]> GroupToDictionary<V>(this Dictionary<CDT, V> values, Func<CDT, int> groupKey)
    {
        var r    = new Dictionary<int, KeyValue<V>[]>();
        var vals = new List<KeyValue<V>>();

        int key        = 0;
        foreach (var (k, v) in values.OrderBy(p => p.Key))
        {
            if (key == 0)
                key = groupKey(k);
            if (groupKey(k) == key)
                vals.Add(new KeyValue<V>(k.Value, v));
            else
            {
                r.Add(key, vals.ToArray());
                key = k.Value;
                vals.Clear();
                vals.Add(new KeyValue<V>(k.Value, v));
            }
        }

        if (key > 0 && vals.Any())
            r.Add(key, vals.ToArray());

        return r;
    }
}