using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ColumnStore;
using CommandLine;
using FileContainer;

namespace CSCLI
{
    partial class Program
    {
        const string FMT_DT = "dd.MM.yyyy HH:mm:ss";

        static void actionDump(ActionDumpParams parms)
        {
            if (!File.Exists(parms.FileName))
            {
                Console.WriteLine("Container not found: {0}", parms.FileName);
                return;
            }

            var nameParts = parms.Mask.Split('/');

            ColumnStoreEntry[] rawEntries;
            using (var c = new PersistentColumnStore(new PersistentReadonlyContainer(parms.FileName), CDTUnit.Month, false,
                                                     string.Join('/', nameParts.Take(nameParts.Length - 1))))
            {
                rawEntries = string.IsNullOrEmpty(parms.Mask) ? c.Find() : c.Find(parms.Mask);
                if (!rawEntries.Any())
                {
                    Console.WriteLine("No entries found");
                    return;
                }
            }

            var sd = parseDT(parms.StartPeriod) ?? new DateTime(2010, 1, 1);
            var ed = parseDT(parms.EndPeriod) ?? new DateTime(2050, 1, 1);

            foreach (var entry in rawEntries
                                  .GroupBy(p => p.CommonPath + "/" + p.ColumnName)
                                  .ToDictionary(p => p.Key,
                                                p => p.ToArray()))
            {
                nameParts = entry.Key.Split('/');
                var commonPath = string.Join('/', nameParts.Take(nameParts.Length - 1));
                var columnName = nameParts.Last();

                using var c = new PersistentColumnStore(new PersistentReadonlyContainer(parms.FileName), CDTUnit.Month, false, commonPath);

                var value = c.Untyped.Read(sd, ed, columnName).FirstOrDefault().Value;
                if (value == null)
                {
                    Console.WriteLine("No entry found");
                    return;
                }

                dumpTo(parms.Target, entry.Key, value, parms.OutputFormat);
            }
        }

        #region dumpTo

        static void dumpTo(string targetDir, string columnName, UntypedColumn data, string format)
        {
            columnName = columnName.Replace('/', '_').Replace('\\', '_') + "." + format;
            var fileName = Path.Combine(targetDir, columnName);
            var formatter = getFormatter(data.Values.DetectDataType(),
                                         string.Compare(format, "json", StringComparison.InvariantCultureIgnoreCase) == 0);

            using var fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write);
            fs.SetLength(0);
            using (var sw = new StreamWriter(fs, Encoding.UTF8, leaveOpen: true))
            {
                if (string.Compare(format, "tsv", StringComparison.InvariantCultureIgnoreCase) == 0)
                    dumpToTSV(sw, data, formatter);
                else if (string.Compare(format, "json", StringComparison.InvariantCultureIgnoreCase) == 0)
                    dumpToJSON(sw, data, formatter);

                sw.Flush();
            }

            fs.Flush();
        }

        static void dumpToTSV(StreamWriter sw, UntypedColumn data, Func<Array, int, string> formatter)
        {
            for (var i = 0; i < data.Keys.Length; i++)
            {
                sw.Write(((DateTime) data.Keys[i]).ToString(FMT_DT));
                sw.Write('\t');
                sw.WriteLine(formatter(data.Values, i));
            }
        }

        static void dumpToJSON(StreamWriter sw, UntypedColumn data, Func<Array, int, string> formatter)
        {
            sw.WriteLine('{');
            var lastIndex = data.Keys.Length - 1;
            for (var i = 0; i < data.Keys.Length; i++)
            {
                sw.Write("  {\"");
                sw.Write(((DateTime) data.Keys[i]).ToString(FMT_DT));
                sw.Write("\": ");

                sw.Write(formatter(data.Values, i));
                if (i < lastIndex) sw.WriteLine("},");
                else sw.WriteLine('}');
            }

            sw.WriteLine("}");
        }

        static Func<Array, int, string> getFormatter(StoredDataType dataType, bool wrapQuotes = false) =>
            dataType switch
            {
                StoredDataType.String => (arr, index) =>
                {
                    var v = arr.GetValue(index)?.ToString();
                    return (wrapQuotes ? "\"" + v?.Replace("\"", @"\""") + "\"" : v) ?? "";
                },
                StoredDataType.Double => (arr, index) => ((double) arr.GetValue(index)).ToString("F7", CultureInfo.InvariantCulture),
                StoredDataType.Guid => (arr, index) =>
                {
                    var v = ((Guid) arr.GetValue(index)).ToString();
                    return wrapQuotes ? "\"" + v + "\"" : v;
                },
                StoredDataType.DateTime => (arr, index) =>
                {
                    var v = ((DateTime) arr.GetValue(index)).ToString(FMT_DT);
                    return wrapQuotes ? "\"" + v + "\"" : v;
                },
                StoredDataType.TimeSpan => (arr, index) =>
                {
                    var v = ((TimeSpan) arr.GetValue(index)).ToString();
                    return wrapQuotes ? "\"" + v + "\"" : v;
                },
                _ => (arr, index) => arr.GetValue(index).ToString()
            };

        static DateTime? parseDT(string s) =>
            DateTime.TryParseExact(s, new[] {"dd.MM.yyyy", "dd.MM.yyyy HH:mm", FMT_DT},
                                   CultureInfo.InvariantCulture,
                                   DateTimeStyles.AdjustToUniversal, out var dt)
                ? dt
                : new DateTime?();

        #endregion
    }

    [ExcludeFromCodeCoverage]
    [Verb("dump", HelpText = "Show directory of container")]
    public class ActionDumpParams
    {
        [Option('f', "file", Required         = true, HelpText  = "File name of container")]                     public string FileName     { get; set; }
        [Option('m', "mask", Required         = false, HelpText = "Mask for names (* and ? supported)")]         public string Mask         { get; set; }
        [Option('t', "target", Required       = true, HelpText  = "Target directory")]                           public string Target       { get; set; }
        [Option('o', "outputFormat", Required = false, Default  = "tsv", HelpText = "Output format (TSV/JSON)")] public string OutputFormat { get; set; }

        [Option('s', "startPeriod", Required = false, HelpText = "Start of period (dd.MM.yyyy [HH:mm])")] public string StartPeriod { get; set; }
        [Option('e', "endPeriod", Required   = false, HelpText = "End of period (dd.MM.yyyy [HH:mm])")]   public string EndPeriod   { get; set; }
    }
}