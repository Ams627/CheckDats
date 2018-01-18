using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckDats
{
    class Dat
    {
        public string Name { get; set; }
        public DateTime FileSystemDate { get; set; }
        public DateTime InFileDate { get; set; }

    }
    internal class Program
    {
        static void GetRjisZips(ref List<Dat> datlist, string path)
        {
            var files = Directory.GetFiles(path, "RJFA*.ZIP");
            datlist.AddRange(files.Select(x => new Dat { Name = x }));
            var dirs = Directory.GetDirectories(path);
            foreach (var dir in dirs)
            {
                GetRjisZips(ref datlist, dir);
            }
        }
        private static void Main(string[] args)
        {
            try
            {
                var datlist = new List<Dat>();
                GetRjisZips(ref datlist, "s:\\");
                datlist.ForEach(x => Console.WriteLine($"{x.Name}"));
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                var codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                var progname = Path.GetFileNameWithoutExtension(codeBase);
                Console.Error.WriteLine(progname + ": Error: " + ex.Message);
            }

        }
    }
}
