using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using CommandLine;
using CommandLine.Text;

namespace CSCLI
{
    [ExcludeFromCodeCoverage]
    partial class Program
    {
        static void Main(string[] args)
        {
            var parserResult = new Parser(with => with.HelpWriter = null)
                .ParseArguments<ActionDirParams, ActionDumpParams, ActionDeleteParams>(args);

            try
            {
                parserResult
                    .WithParsed<ActionDirParams>(actionDir)
                    .WithParsed<ActionDeleteParams>(actionDelete)
                    .WithParsed<ActionDumpParams>(actionDump)
                    .WithNotParsed(errs =>
                    {
                        var helpText = HelpText.AutoBuild(parserResult, h =>
                        {
                            h.AdditionalNewLineAfterOption = false;
                            h.Heading                      = "ColumnStore CLI";
                            h.Copyright                    = "Copyright (c) 2020-2021";
                            h.MaximumDisplayWidth          = 999;
                            return HelpText.DefaultParsingErrorsHandler(parserResult, h);
                        }, e => e);

                        Console.WriteLine(helpText);
                    });
            }
            catch (Exception e)
            {
                e = e.InnerException ?? e;
                Console.WriteLine(e.Message);
            }
        }
    }
}