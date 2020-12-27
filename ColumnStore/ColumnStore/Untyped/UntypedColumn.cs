using System;
using JetBrains.Annotations;

namespace ColumnStore
{
    public sealed class UntypedColumn
    {
        [NotNull] public readonly CDT[] Keys;
        [NotNull] public readonly Array Values;
        
        /// <summary>
        /// Keys and Values for column
        /// <code>
        /// * Keys.Length must be == Values.Length
        /// * Keys must be sorted and must be unique
        /// * Values should not contain null values
        /// </code>
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public UntypedColumn([NotNull] CDT[] keys, [NotNull] Array values)
        {
            Keys   = keys ?? throw new ArgumentException("Can't be null", nameof(keys));
            Values = values ?? throw new ArgumentException("Can't be null", nameof(values));

            if (Keys.Length != Values.Length)
                throw new ArgumentException($"Keys.Length != Values.Length ({Keys.Length} != {Values.Length})");

            if (Keys.Length == 0)
                throw new ArgumentException($"Empty keys/values not allowed");

            if (Values.Rank > 1)
                throw new ArgumentException($"Values can't be multidimensional ({Values.Rank})");
        }

#if DEBUG
        public override string ToString() => $"{Keys.First()} - ${Keys.Last()}: {Keys.Length}";
#endif
    }
}