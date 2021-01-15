using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FileContainer;
using JetBrains.Annotations;

namespace ColumnStore
{
    public partial class PersistentColumnStore
    {
        [NotNull]
        public ColumnStoreEntry[] Find(params string[] keys)
        {
            var items = Container.Find(keys);

            var buff = new byte[8];
            var r    = new List<ColumnStoreEntry>();
            foreach (var item in items.Where(p => p.Name != SECTION_SETTINGS))
            {
                using (var stm = Container.GetStream(item.Name))
                    stm.Read(buff, 0, 8); // 4+4 = length + datatype // hack :(

                r.Add(new ColumnStoreEntry(item, BitConverter.ToInt32(buff, 0), (StoredDataType) BitConverter.ToInt32(buff, 4)));
            }

            return r.ToArray();
        }
    }

    public class ColumnStoreEntry
    {
        [NotNull] public string         Name     { get; }
        public           DateTime       Modified { get; }
        public           int            Length   { get; }
        public           int            Count    { get; }
        public           StoredDataType DataType { get; }

        [NotNull] public string CommonPath   { get; }
        [NotNull] public string ColumnName   { get; }
        public           CDT    PartitionKey { get; }

        internal ColumnStoreEntry([NotNull] PagedContainerEntry entry, int count, StoredDataType dataType)
        {
            Name     = entry.Name;
            Modified = entry.Modified;
            Length   = entry.Length;
            DataType = dataType;
            Count    = count;

            var pathItems = Name.Split('/');

            CommonPath   = string.Join("/", pathItems.Take(pathItems.Length - 2));
            ColumnName   = pathItems.Skip(pathItems.Length - 2).Take(1).First();
            PartitionKey = int.TryParse(pathItems.Last(), out var partitionKey) ? new CDT(partitionKey) : default;
        }

#if DEBUG
        [ExcludeFromCodeCoverage]
        public override string ToString() => $"{Name} [{DataType}]: {Length} bytes ({Modified:u}), Count: {Count}, PartitionKey: {PartitionKey}";
#endif
    }
}