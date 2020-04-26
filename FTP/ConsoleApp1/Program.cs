using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFTP
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = "../../../ServerTests";
            var dirInfo = new DirectoryInfo(path);
            var files = dirInfo.GetFiles();
            var dirNames = dirInfo.GetDirectories();
            Console.Write( files.Length + dirNames.Length + "\n"
                + string.Join("", files.Select(name => $"{name.Name} False \n"))
                + string.Join("", dirNames.Select(name => $"{name.Name} True \n")));
            Console.ReadKey();
        }
    }
}
