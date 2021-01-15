using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ColumnStore;
using CommandLine;
using FileContainer;

namespace CSCLI
{
    partial class Program
    {
        static void actionDir(ActionDirParams parms)
        {
            if (!File.Exists(parms.FileName))
            {
                Console.WriteLine("Container not found: {0}", parms.FileName);
                return;
            }

            using var c          = new PersistentColumnStore(new PersistentReadonlyContainer(parms.FileName));
            var       rawEntries = string.IsNullOrEmpty(parms.Mask) ? c.Find() : c.Find(parms.Mask);
            if (!rawEntries.Any())
            {
                Console.WriteLine("No entries found");
                return;
            }

            var entries = rawEntries
                          .GroupBy(p => p.CommonPath + "/" + p.ColumnName)
                          .ToDictionary(p => p.Key,
                                        p => p.ToArray());

            foreach (var entry in entries)
            {
                var f = entry.Value.First();
                Console.WriteLine("{0,-59}  {1,8}", entry.Key, f.DataType);
                Console.WriteLine("---------------------------------------------------------------------");
                Console.WriteLine("   #     Length   Modified time           Items   PartitionKey");
                Console.WriteLine("---------------------------------------------------------------------");

                var summaryLength     = 0;
                var summaryCount      = 0;
                var summaryByDataType = new Dictionary<StoredDataType, (int count, int length)>();
                int index             = 0;
                foreach (var item in entry.Value)
                {
                    Console.WriteLine("{0,4} {1,10}   {2:dd.MM.yyyy HH:mm:ss}   {3,7}   {4:dd.MM.yyyy HH:mm:ss}",
                                      index++, item.Length, item.Modified.ToLocalTime(), item.Count, (DateTime) item.PartitionKey);
                    summaryLength += item.Length;
                    summaryCount  += item.Count;
                    if (!summaryByDataType.TryGetValue(item.DataType, out var existing))
                        summaryByDataType.Add(item.DataType, (item.Length, item.Count));
                    else
                        summaryByDataType[item.DataType] = (existing.length + item.Length, existing.count + item.Count);
                }

                Console.WriteLine("---------------------------------------------------------------------");
                Console.WriteLine("{0,4} {1,10} bytes total             {2,7}", entry.Value.Length, summaryLength, summaryCount);
                Console.WriteLine();
            }
        }
    }

    [Verb("dir", HelpText = "Show directory of container")]
    public class ActionDirParams
    {
        [Option('f', "file", Required = true, HelpText  = "File name of container")]             public string FileName { get; set; }
        [Option('m', "mask", Required = false, HelpText = "Mask for names (* and ? supported)")] public string Mask     { get; set; }
    }
}