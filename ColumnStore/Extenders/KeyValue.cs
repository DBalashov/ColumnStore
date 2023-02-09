namespace ColumnStore;

readonly struct KeyValue<V>
{
    public readonly int Key;
    public readonly V   Value;

    internal KeyValue(int key, V value)
    {
        Key   = key;
        Value = value;
    }

#if DEBUG
    public override string ToString() => $"{new CDT(Key)}: {Value}";
#endif
}