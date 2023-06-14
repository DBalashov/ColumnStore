using System;

#pragma warning disable 659

namespace ColumnStore.Tests
{
    public class SimpleEntity
    {
        public int      ColumnInt      { get; set; }
        public uint     ColumnUInt     { get; set; }
        public short    ColumnInt16    { get; set; }
        public ushort   ColumnUInt16   { get; set; }
        public Int64    ColumnInt64    { get; set; }
        public UInt64   ColumnUInt64   { get; set; }
        public Guid     ColumnGuid     { get; set; }
        public DateTime ColumnDateTime { get; set; }
        public string   ColumnString   { get; set; }
        public TimeSpan ColumnTimeSpan { get; set; }
        public double   ColumnDouble   { get; set; }
        public byte     ColumnByte     { get; set; }
        public sbyte    ColumnSByte    { get; set; }
        public bool     ColumnBool     { get; set; }
        public Half     ColumnHalf     { get; set; }
        public Decimal  ColumnDecimal  { get; set; }
        public DateOnly ColumnDateOnly { get; set; }
        public TimeOnly ColumnTimeOnly { get; set; }

        public int      ColumnInt2      { get; set; }
        public uint     ColumnUInt2     { get; set; }
        public short    ColumnInt16_2   { get; set; }
        public ushort   ColumnUInt16_2  { get; set; }
        public Int64    ColumnInt64_2   { get; set; }
        public UInt64   ColumnUInt64_2  { get; set; }
        public Guid     ColumnGuid2     { get; set; }
        public DateTime ColumnDateTime2 { get; set; }
        public string   ColumnString2   { get; set; }
        public TimeSpan ColumnTimeSpan2 { get; set; }
        public double   ColumnDouble2   { get; set; }
        public byte     ColumnByte2     { get; set; }
        public sbyte    ColumnSByte2    { get; set; }
        public bool     ColumnBool2     { get; set; }
        public Half     ColumnHalf2     { get; set; }
        public Decimal  ColumnDecimal2  { get; set; }
        public DateOnly ColumnDateOnly2 { get; set; }
        public TimeOnly ColumnTimeOnly2 { get; set; }

        public int      ColumnInt3      { get; set; }
        public short    ColumnInt16_3   { get; set; }
        public Int64    ColumnInt64_3   { get; set; }
        public Guid     ColumnGuid3     { get; set; }
        public DateTime ColumnDateTime3 { get; set; }
        public string   ColumnString3   { get; set; }
        public TimeSpan ColumnTimeSpan3 { get; set; }
        public double   ColumnDouble3   { get; set; }
        public byte     ColumnByte3     { get; set; }
        public bool     ColumnBool3     { get; set; }
        public Half     ColumnHalf3     { get; set; }
        public Decimal  ColumnDecimal3  { get; set; }
        public DateOnly ColumnDateOnly3 { get; set; }
        public TimeOnly ColumnTimeOnly3 { get; set; }

        public int      ColumnInt4      { get; set; }
        public short    ColumnInt16_4   { get; set; }
        public Int64    ColumnInt64_4   { get; set; }
        public Guid     ColumnGuid4     { get; set; }
        public DateTime ColumnDateTime4 { get; set; }
        public string   ColumnString4   { get; set; }
        public TimeSpan ColumnTimeSpan4 { get; set; }
        public double   ColumnDouble4   { get; set; }
        public byte     ColumnByte4     { get; set; }
        public bool     ColumnBool4     { get; set; }
        public Half     ColumnHalf4     { get; set; }
        public Decimal  ColumnDecimal4  { get; set; }
        public DateOnly ColumnDateOnly4 { get; set; }
        public TimeOnly ColumnTimeOnly4 { get; set; }

        public int      ColumnInt5      { get; set; }
        public short    ColumnInt16_5   { get; set; }
        public Int64    ColumnInt64_5   { get; set; }
        public Guid     ColumnGuid5     { get; set; }
        public DateTime ColumnDateTime5 { get; set; }
        public string   ColumnString5   { get; set; }
        public TimeSpan ColumnTimeSpan5 { get; set; }
        public double   ColumnDouble5   { get; set; }
        public byte     ColumnByte5     { get; set; }
        public bool     ColumnBool5     { get; set; }
        public Half     ColumnHalf5     { get; set; }
        public Decimal  ColumnDecimal5  { get; set; }
        public DateOnly ColumnDateOnly5 { get; set; }
        public TimeOnly ColumnTimeOnly5 { get; set; }

        public SimpleEntity()
        {
        }

        public SimpleEntity(DateTime d)
        {
            ColumnByte  = ColumnByte2  = ColumnByte3 = ColumnByte4 = ColumnByte5 = (byte) (d.Minute + d.Second + d.Day);
            ColumnSByte = ColumnSByte2 = (sbyte) (d.Second + d.Day);

            ColumnDouble  = ColumnDouble2  = ColumnDouble3  = ColumnDouble4  = ColumnDouble5  = d.TimeOfDay.TotalMilliseconds;
            ColumnHalf    = ColumnHalf2    = ColumnHalf3    = ColumnHalf4    = ColumnHalf5    = (Half) (d.TimeOfDay.TotalMilliseconds    / (double) Half.MaxValue);
            ColumnDecimal = ColumnDecimal2 = ColumnDecimal3 = ColumnDecimal4 = ColumnDecimal5 = (decimal) (d.TimeOfDay.TotalMilliseconds / 2.123);
            ColumnGuid = ColumnGuid2 = ColumnGuid3 = ColumnGuid4 = ColumnGuid5 = new Guid((uint) d.Year, 0, 0, (byte) d.Year, (byte) d.Month, (byte) d.Day, (byte) d.Hour, 0, 0, 0,
                                                                                          0);
            ColumnString   = ColumnString2   = ColumnString3   = ColumnString4   = ColumnString5   = "Item Address " + d.ToString("yyyyMMdd") + "/" + d.Month + "/" + d.Minute + "/" + d.Day;
            ColumnTimeSpan = ColumnTimeSpan2 = ColumnTimeSpan3 = ColumnTimeSpan4 = ColumnTimeSpan5 = d.TimeOfDay;
            ColumnDateTime = ColumnDateTime2 = ColumnDateTime3 = ColumnDateTime4 = ColumnDateTime5 = d;

            ColumnInt  = ColumnInt2  = ColumnInt3 = ColumnInt4 = ColumnInt5 = d.Minute + d.Second + d.Day;
            ColumnUInt = ColumnUInt2 = (uint) (d.Minute + d.Second + d.Day);

            ColumnInt16  = ColumnInt16_2  = ColumnInt16_3 = ColumnInt16_4 = ColumnInt16_5 = (short) (d.Minute + d.Second + d.Day + 1);
            ColumnUInt16 = ColumnUInt16_2 = (ushort) (d.Minute + d.Second + d.Day + 1);

            Int64 x                       = d.Minute + d.Second + d.Day + 1;
            ColumnInt64  = ColumnInt64_2  = ColumnInt64_3 = ColumnInt64_4 = ColumnInt64_5 = x | ((x + 2) << 32);
            ColumnUInt64 = ColumnUInt64_2 = (UInt64) (x | ((x + 2) << 32));

            ColumnBool     = ColumnBool2     = ColumnBool3     = ColumnBool4     = ColumnBool5     = (d.Minute + d.Second + d.Day) % d.Day > 0;
            ColumnDecimal  = ColumnDecimal2  = ColumnDecimal3  = ColumnDecimal4  = ColumnDecimal5  = (decimal) (d.TimeOfDay.TotalMilliseconds / 2.123);
            ColumnTimeOnly = ColumnTimeOnly2 = ColumnTimeOnly3 = ColumnTimeOnly4 = ColumnTimeOnly5 = TimeOnly.FromTimeSpan(d.TimeOfDay);
            ColumnDateOnly = ColumnDateOnly2 = ColumnDateOnly3 = ColumnDateOnly4 = ColumnDateOnly5 = new DateOnly(d.Year, d.Month, d.Day);
        }

        public override bool Equals(object obj)
        {
            const double EPSILON = 0.00001;
            return obj is SimpleEntity o                                                                                                                          &&
                   ColumnByte                                == o.ColumnByte                                                                                      &&
                   ColumnByte2                               == o.ColumnByte2                                                                                     &&
                   ColumnByte3                               == o.ColumnByte3                                                                                     &&
                   ColumnByte4                               == o.ColumnByte4                                                                                     &&
                   ColumnByte5                               == o.ColumnByte5                                                                                     &&
                   ColumnSByte                               == o.ColumnSByte                                                                                     &&
                   ColumnSByte2                              == o.ColumnSByte2                                                                                    &&
                   ColumnBool                                == o.ColumnBool                                                                                      &&
                   ColumnBool2                               == o.ColumnBool2                                                                                     &&
                   ColumnBool3                               == o.ColumnBool3                                                                                     &&
                   ColumnBool4                               == o.ColumnBool4                                                                                     &&
                   ColumnBool5                               == o.ColumnBool5                                                                                     &&
                   ColumnInt                                 == o.ColumnInt                                                                                       &&
                   ColumnInt2                                == o.ColumnInt2                                                                                      &&
                   ColumnInt3                                == o.ColumnInt3                                                                                      &&
                   ColumnInt4                                == o.ColumnInt4                                                                                      &&
                   ColumnInt5                                == o.ColumnInt5                                                                                      &&
                   ColumnUInt                                == o.ColumnUInt                                                                                      &&
                   ColumnUInt2                               == o.ColumnUInt2                                                                                     &&
                   ColumnInt16                               == o.ColumnInt16                                                                                     &&
                   ColumnInt16_2                             == o.ColumnInt16_2                                                                                   &&
                   ColumnInt16_3                             == o.ColumnInt16_3                                                                                   &&
                   ColumnInt16_4                             == o.ColumnInt16_4                                                                                   &&
                   ColumnInt16_5                             == o.ColumnInt16_5                                                                                   &&
                   ColumnUInt16                              == o.ColumnUInt16                                                                                    &&
                   ColumnUInt16_2                            == o.ColumnUInt16_2                                                                                  &&
                   ColumnInt64                               == o.ColumnInt64                                                                                     &&
                   ColumnInt64_2                             == o.ColumnInt64_2                                                                                   &&
                   ColumnInt64_3                             == o.ColumnInt64_3                                                                                   &&
                   ColumnInt64_4                             == o.ColumnInt64_4                                                                                   &&
                   ColumnInt64_5                             == o.ColumnInt64_5                                                                                   &&
                   ColumnUInt64                              == o.ColumnUInt64                                                                                    &&
                   ColumnUInt64_2                            == o.ColumnUInt64_2                                                                                  &&
                   Math.Abs(ColumnDouble  - o.ColumnDouble)  < EPSILON                                                                                            &&
                   Math.Abs(ColumnDouble2 - o.ColumnDouble2) < EPSILON                                                                                            &&
                   Math.Abs(ColumnDouble3 - o.ColumnDouble3) < EPSILON                                                                                            &&
                   Math.Abs(ColumnDouble4 - o.ColumnDouble4) < EPSILON                                                                                            &&
                   Math.Abs(ColumnDouble5 - o.ColumnDouble5) < EPSILON                                                                                            &&
                   ((ColumnString  == null && o.ColumnString  == null) || string.Compare(ColumnString,  o.ColumnString,  StringComparison.InvariantCulture) == 0) &&
                   ((ColumnString2 == null && o.ColumnString2 == null) || string.Compare(ColumnString2, o.ColumnString2, StringComparison.InvariantCulture) == 0) &&
                   ((ColumnString3 == null && o.ColumnString3 == null) || string.Compare(ColumnString3, o.ColumnString3, StringComparison.InvariantCulture) == 0) &&
                   ((ColumnString4 == null && o.ColumnString4 == null) || string.Compare(ColumnString4, o.ColumnString4, StringComparison.InvariantCulture) == 0) &&
                   ((ColumnString5 == null && o.ColumnString5 == null) || string.Compare(ColumnString5, o.ColumnString5, StringComparison.InvariantCulture) == 0) &&
                   ColumnGuid                                              == o.ColumnGuid                                                                        &&
                   ColumnGuid2                                             == o.ColumnGuid2                                                                       &&
                   ColumnGuid3                                             == o.ColumnGuid3                                                                       &&
                   ColumnGuid4                                             == o.ColumnGuid4                                                                       &&
                   ColumnGuid5                                             == o.ColumnGuid5                                                                       &&
                   ColumnDateTime                                          == o.ColumnDateTime                                                                    &&
                   ColumnDateTime2                                         == o.ColumnDateTime2                                                                   &&
                   ColumnDateTime3                                         == o.ColumnDateTime3                                                                   &&
                   ColumnDateTime4                                         == o.ColumnDateTime4                                                                   &&
                   ColumnDateTime5                                         == o.ColumnDateTime5                                                                   &&
                   ColumnTimeSpan                                          == o.ColumnTimeSpan                                                                    &&
                   ColumnTimeSpan2                                         == o.ColumnTimeSpan2                                                                   &&
                   ColumnTimeSpan3                                         == o.ColumnTimeSpan3                                                                   &&
                   ColumnTimeSpan4                                         == o.ColumnTimeSpan4                                                                   &&
                   ColumnTimeSpan5                                         == o.ColumnTimeSpan5                                                                   &&
                   ColumnDecimal                                           == o.ColumnDecimal                                                                     &&
                   ColumnDecimal2                                          == o.ColumnDecimal2                                                                    &&
                   ColumnDecimal3                                          == o.ColumnDecimal3                                                                    &&
                   ColumnDecimal4                                          == o.ColumnDecimal4                                                                    &&
                   ColumnDecimal5                                          == o.ColumnDecimal5                                                                    &&
                   Math.Abs((double) ColumnHalf  - (double) o.ColumnHalf)  < (double) Half.Epsilon                                                                &&
                   Math.Abs((double) ColumnHalf2 - (double) o.ColumnHalf2) < (double) Half.Epsilon                                                                &&
                   Math.Abs((double) ColumnHalf3 - (double) o.ColumnHalf3) < (double) Half.Epsilon                                                                &&
                   Math.Abs((double) ColumnHalf4 - (double) o.ColumnHalf4) < (double) Half.Epsilon                                                                &&
                   Math.Abs((double) ColumnHalf5 - (double) o.ColumnHalf5) < (double) Half.Epsilon                                                                &&
                   Math.Abs((double) (ColumnDecimal  - o.ColumnDecimal))   < EPSILON                                                                              &&
                   Math.Abs((double) (ColumnDecimal2 - o.ColumnDecimal2))  < EPSILON                                                                              &&
                   Math.Abs((double) (ColumnDecimal3 - o.ColumnDecimal3))  < EPSILON                                                                              &&
                   Math.Abs((double) (ColumnDecimal4 - o.ColumnDecimal4))  < EPSILON                                                                              &&
                   Math.Abs((double) (ColumnDecimal5 - o.ColumnDecimal5))  < EPSILON                                                                              &&
                   ColumnTimeOnly                                          == o.ColumnTimeOnly                                                                    &&
                   ColumnTimeOnly2                                         == o.ColumnTimeOnly2                                                                   &&
                   ColumnTimeOnly3                                         == o.ColumnTimeOnly3                                                                   &&
                   ColumnTimeOnly4                                         == o.ColumnTimeOnly4                                                                   &&
                   ColumnTimeOnly5                                         == o.ColumnTimeOnly5                                                                   &&
                   ColumnDateOnly                                          == o.ColumnDateOnly                                                                    &&
                   ColumnDateOnly2                                         == o.ColumnDateOnly2                                                                   &&
                   ColumnDateOnly3                                         == o.ColumnDateOnly3                                                                   &&
                   ColumnDateOnly4                                         == o.ColumnDateOnly4                                                                   &&
                   ColumnDateOnly5                                         == o.ColumnDateOnly5;
        }
    }
}