using System;
using System.IO;
using System.Text;
using SpanByteExtenders;

namespace ColumnStore;

// todo nullable bitmap
sealed class ReadWriteHandlerString : ReadWriteBase
{
    public override void Pack(Array values, IVirtualWriteStream targetStream, Range range)
    {
        //using var bw = new BinaryWriter(targetStream.GetUnderlyingStreamn(), Encoding.UTF8, true);

        var r = values.Dictionarize(range, "");

        // write dictionary values
        targetStream.Write((ushort) r.Values.Length);
        foreach (var item in r.Values)
            if (item == null)
            {
                targetStream.Write((short) -1);
            }
            else
            {
                var buff = Encoding.UTF8.GetBytes(item);
                targetStream.Write((ushort) buff.Length);
                targetStream.Write(buff.AsSpan(0, buff.Length));
            }

        // write value indexes
        var compactType = r.Values.Length.GetCompactType();
        targetStream.Write((byte) compactType);

        var requireBytes = range.Length() * (1 << (int) compactType);
        var buffIndexes  = poolBytes.Rent(requireBytes);
        var span         = buffIndexes.AsSpan(0, requireBytes);
        r.Indexes.CompactValues(span, compactType);
        targetStream.Write(span);
        poolBytes.Return(buffIndexes);
    }

    public override Array Unpack(Span<byte> span, int count)
    {
        // read dictionary values
        var dictionaryValuesCount = span.ReadUInt16();

        var dictionaryValues = new string?[dictionaryValuesCount];
        for (var i = 0; i < dictionaryValuesCount; i++)
        {
            var length = span.ReadInt16();
            dictionaryValues[i] = length >= 0 ? Encoding.UTF8.GetString(span.ReadBytes(length)) : null;
        }

        // read value indexes
        var compactType = (CompactType) span.ReadByte();
        var indexes     = span.UncompactValues(count, compactType);

        var values = new string?[count];
        for (var i = 0; i < count; i++)
            values[i] = dictionaryValues[indexes[i]];

        return values;
    }
}