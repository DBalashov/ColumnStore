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
        public float    ColumnFloat    { get; set; }

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
        public float    ColumnFloat2    { get; set; }

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
        public float    ColumnFloat3    { get; set; }

        public SimpleEntity()
        {
        }

        public SimpleEntity(DateTime d)
        {
            ColumnByte  = ColumnByte2  = ColumnByte3 = (byte) (d.Minute + d.Second + d.Day);
            ColumnSByte = ColumnSByte2 = (sbyte) (d.Second + d.Day);

            ColumnDouble  = ColumnDouble2  = ColumnDouble3  = d.TimeOfDay.TotalMilliseconds;
            ColumnHalf    = ColumnHalf2    = ColumnHalf3    = (Half) (d.TimeOfDay.TotalMilliseconds    / (double) Half.MaxValue);
            ColumnDecimal = ColumnDecimal2 = ColumnDecimal3 = (decimal) (d.TimeOfDay.TotalMilliseconds / 2.123);
            ColumnGuid = ColumnGuid2 = ColumnGuid3 = new Guid((uint) d.Year, 0, 0, (byte) d.Year, (byte) d.Month, (byte) d.Day, (byte) d.Hour, 0, 0, 0,
                                                              0);
            ColumnString   = ColumnString2   = ColumnString3   = "Item Address " + d.ToString("yyyyMMdd") + "/" + d.Month + "/" + d.Minute + "/" + d.Day;
            ColumnTimeSpan = ColumnTimeSpan2 = ColumnTimeSpan3 = d.TimeOfDay;
            ColumnDateTime = ColumnDateTime2 = ColumnDateTime3 = d;

            ColumnInt  = ColumnInt2  = ColumnInt3 = d.Minute + d.Second + d.Day;
            ColumnUInt = ColumnUInt2 = (uint) (d.Minute + d.Second + d.Day);

            ColumnInt16  = ColumnInt16_2  = ColumnInt16_3 = (short) (d.Minute + d.Second + d.Day + 1);
            ColumnUInt16 = ColumnUInt16_2 = (ushort) (d.Minute + d.Second + d.Day + 1);

            Int64 x                       = d.Minute + d.Second + d.Day + 1;
            ColumnInt64  = ColumnInt64_2  = ColumnInt64_3 = x | ((x + 2) << 32);
            ColumnUInt64 = ColumnUInt64_2 = (UInt64) (x | ((x + 2) << 32));

            ColumnBool     = ColumnBool2     = ColumnBool3     = (d.Minute + d.Second + d.Day) % d.Day > 0;
            ColumnDecimal  = ColumnDecimal2  = ColumnDecimal3  = (decimal) (d.TimeOfDay.TotalMilliseconds / 2.123);
            ColumnTimeOnly = ColumnTimeOnly2 = ColumnTimeOnly3 = TimeOnly.FromTimeSpan(d.TimeOfDay);
            ColumnDateOnly = ColumnDateOnly2 = ColumnDateOnly3 = new DateOnly(d.Year, d.Month, d.Day);
        }

        public override bool Equals(object obj)
        {
            const double EPSILON = 0.00001;
            return obj is SimpleEntity o                                                                                                                          &&
                   ColumnByte                                == o.ColumnByte                                                                                      &&
                   ColumnByte2                               == o.ColumnByte2                                                                                     &&
                   ColumnByte3                               == o.ColumnByte3                                                                                     &&
                   ColumnSByte                               == o.ColumnSByte                                                                                     &&
                   ColumnSByte2                              == o.ColumnSByte2                                                                                    &&
                   ColumnBool                                == o.ColumnBool                                                                                      &&
                   ColumnBool2                               == o.ColumnBool2                                                                                     &&
                   ColumnBool3                               == o.ColumnBool3                                                                                     &&
                   ColumnInt                                 == o.ColumnInt                                                                                       &&
                   ColumnInt2                                == o.ColumnInt2                                                                                      &&
                   ColumnInt3                                == o.ColumnInt3                                                                                      &&
                   ColumnUInt                                == o.ColumnUInt                                                                                      &&
                   ColumnUInt2                               == o.ColumnUInt2                                                                                     &&
                   ColumnInt16                               == o.ColumnInt16                                                                                     &&
                   ColumnInt16_2                             == o.ColumnInt16_2                                                                                   &&
                   ColumnInt16_3                             == o.ColumnInt16_3                                                                                   &&
                   ColumnUInt16                              == o.ColumnUInt16                                                                                    &&
                   ColumnUInt16_2                            == o.ColumnUInt16_2                                                                                  &&
                   ColumnInt64                               == o.ColumnInt64                                                                                     &&
                   ColumnInt64_2                             == o.ColumnInt64_2                                                                                   &&
                   ColumnInt64_3                             == o.ColumnInt64_3                                                                                   &&
                   ColumnUInt64                              == o.ColumnUInt64                                                                                    &&
                   ColumnUInt64_2                            == o.ColumnUInt64_2                                                                                  &&
                   Math.Abs(ColumnDouble  - o.ColumnDouble)  < EPSILON                                                                                            &&
                   Math.Abs(ColumnDouble2 - o.ColumnDouble2) < EPSILON                                                                                            &&
                   Math.Abs(ColumnDouble3 - o.ColumnDouble3) < EPSILON                                                                                            &&
                   Math.Abs(ColumnFloat   - o.ColumnFloat)   < EPSILON                                                                                            &&
                   Math.Abs(ColumnFloat2  - o.ColumnFloat2)  < EPSILON                                                                                            &&
                   Math.Abs(ColumnFloat3  - o.ColumnFloat3)  < EPSILON                                                                                            &&
                   ((ColumnString  == null && o.ColumnString  == null) || string.Compare(ColumnString,  o.ColumnString,  StringComparison.InvariantCulture) == 0) &&
                   ((ColumnString2 == null && o.ColumnString2 == null) || string.Compare(ColumnString2, o.ColumnString2, StringComparison.InvariantCulture) == 0) &&
                   ((ColumnString3 == null && o.ColumnString3 == null) || string.Compare(ColumnString3, o.ColumnString3, StringComparison.InvariantCulture) == 0) &&
                   ColumnGuid                                              == o.ColumnGuid                                                                        &&
                   ColumnGuid2                                             == o.ColumnGuid2                                                                       &&
                   ColumnGuid3                                             == o.ColumnGuid3                                                                       &&
                   ColumnDateTime                                          == o.ColumnDateTime                                                                    &&
                   ColumnDateTime2                                         == o.ColumnDateTime2                                                                   &&
                   ColumnDateTime3                                         == o.ColumnDateTime3                                                                   &&
                   ColumnTimeSpan                                          == o.ColumnTimeSpan                                                                    &&
                   ColumnTimeSpan2                                         == o.ColumnTimeSpan2                                                                   &&
                   ColumnTimeSpan3                                         == o.ColumnTimeSpan3                                                                   &&
                   ColumnDecimal                                           == o.ColumnDecimal                                                                     &&
                   ColumnDecimal2                                          == o.ColumnDecimal2                                                                    &&
                   ColumnDecimal3                                          == o.ColumnDecimal3                                                                    &&
                   Math.Abs((double) ColumnHalf  - (double) o.ColumnHalf)  < (double) Half.Epsilon                                                                &&
                   Math.Abs((double) ColumnHalf2 - (double) o.ColumnHalf2) < (double) Half.Epsilon                                                                &&
                   Math.Abs((double) ColumnHalf3 - (double) o.ColumnHalf3) < (double) Half.Epsilon                                                                &&
                   Math.Abs((double) (ColumnDecimal  - o.ColumnDecimal))   < EPSILON                                                                              &&
                   Math.Abs((double) (ColumnDecimal2 - o.ColumnDecimal2))  < EPSILON                                                                              &&
                   Math.Abs((double) (ColumnDecimal3 - o.ColumnDecimal3))  < EPSILON                                                                              &&
                   ColumnTimeOnly                                          == o.ColumnTimeOnly                                                                    &&
                   ColumnTimeOnly2                                         == o.ColumnTimeOnly2                                                                   &&
                   ColumnTimeOnly3                                         == o.ColumnTimeOnly3                                                                   &&
                   ColumnDateOnly                                          == o.ColumnDateOnly                                                                    &&
                   ColumnDateOnly2                                         == o.ColumnDateOnly2                                                                   &&
                   ColumnDateOnly3                                         == o.ColumnDateOnly3;
        }
    }
}