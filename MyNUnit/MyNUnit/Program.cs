using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNUnit
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = args[0];
            Console.WriteLine("Execution started");
            MyNUnit.Runner.Run(path);
            MyNUnit.Runner.Print();
            Console.WriteLine("Execution finished");
        }
    }
}
