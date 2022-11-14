using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ColumnStore;
using FileContainer;

namespace Examples
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Keys: {0} (each {1} minutes), {2:u} - {3:u}",
                              TestAbstractRunner.Keys.Count(),
                              TestAbstractRunner.Every,
                              TestAbstractRunner.StartDate,
                              TestAbstractRunner.EndDate);

            var typedRunner = new TestTypedRunner();
            typedRunner.Run(false);
            typedRunner.Run(true);
            
            var entitesRunner = new TestEntitiesRunner();
            entitesRunner.Run(false);
            entitesRunner.Run(true);
        }
    }
}