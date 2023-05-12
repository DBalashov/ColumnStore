using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FileContainer;

namespace ColumnStore;

public partial class PersistentColumnStore
{
    public ColumnStoreEntry[] Find(params string[] keys)
    {
        var items = Container.Find(keys);

        var buff = new byte[8];
        var r    = new List<ColumnStoreEntry>();
        foreach (var item in items.Where(p => p.Name != SECTION_SETTINGS))
        {
            using (var stm = Container.GetStream(item.Name))
            {
                if (stm != null)
                    stm.Read(buff, 0, 4 + 4); // length + datatype // hack :(
            }

            r.Add(new ColumnStoreEntry(item,
                                       BitConverter.ToInt32(buff, 0),
                                       (StoredDataType) BitConverter.ToInt32(buff, 4)));
        }

        return r.ToArray();
    }
}