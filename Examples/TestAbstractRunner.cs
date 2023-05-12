using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Examples
{
    [ExcludeFromCodeCoverage]
    abstract class TestAbstractRunner
    {
        protected static readonly Random r         = new(Guid.NewGuid().GetHashCode());
        protected static readonly int[]  PageSizes = {2048, 4096, 8192, 16384};
        protected const           int    ReadDays  = 63;

        public static readonly DateTime StartDate = new(2020, 1, 1);
        public static readonly DateTime EndDate   = new(2021, 1, 1);
        public static readonly int      Every     = 1;

        public static readonly DateTime[] Keys = Enumerable.Range(0, (int) (EndDate.Subtract(StartDate).TotalMinutes / Every))
                                                           .Select(p => StartDate.AddMinutes(p * Every))
                                                           .ToArray();
    }
}