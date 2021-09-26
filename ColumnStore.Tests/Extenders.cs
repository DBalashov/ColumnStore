using System;
using System.Linq;

namespace ColumnStore.Tests
{
    public static class PublicExtenders
    {
        public static DateTime[] Convert(this CDT[] source) => source.Select(p => (DateTime)p).ToArray();
    }
}