using System;
using System.Linq;
using JetBrains.Annotations;

namespace ColumnStore.Tests
{
    public static class PublicExtenders
    {
        [NotNull]
        public static DateTime[] Convert([NotNull] this CDT[] source) => source.Select(p => (DateTime) p).ToArray();
    }
}