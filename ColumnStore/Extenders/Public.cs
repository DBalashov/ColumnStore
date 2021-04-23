using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;

namespace ColumnStore
{
    [ExcludeFromCodeCoverage]
    public static class PublicExtenders
    {
        // [NotNull]
        // public static Dictionary<CDT, E> Convert<E>([NotNull] this Dictionary<DateTime, E> entities) =>
        //     entities.ToDictionary(p => (CDT) p.Key, p => p.Value);
        //
        // [NotNull]
        // public static Dictionary<DateTime, E> Convert<E>([NotNull] this Dictionary<CDT, E> entities) =>
        //     entities.ToDictionary(p => (DateTime) p.Key, p => p.Value);
        //
        // [NotNull]
        // public static CDT[] Convert([NotNull] this DateTime[] source) => source.Select(p => (CDT) p).ToArray();

        [NotNull]
        public static DateTime[] Convert([NotNull] this CDT[] source) => source.Select(p => (DateTime) p).ToArray();
    }
}