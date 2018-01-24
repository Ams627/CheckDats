using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
                var pattern = @"^.*\s*/!!\s*Generated:\s+(\d\d/\d\d/\d\d\d\d)";
                var datlist = new List<Dat>();
                GetRjisZips(ref datlist, "s:\\");
                //datlist.ForEach(x => Console.WriteLine($"{x.Name}"));
                foreach (var zip in datlist)
                {
                    using (var archive = ZipFile.OpenRead(zip.Name))
                    {
                        var result = from entry in archive.Entries where Path.GetExtension(entry.Name).ToUpper() == ".DAT" select entry;
                        foreach (var entry in result)
                        {
                            var extractPath = Path.Combine("q:\\temp\\datresults", entry.Name);
                            entry.ExtractToFile(extractPath, true);

                            var fileDate = File.GetLastWriteTime(extractPath);
                            foreach (var line in File.ReadLines(extractPath))
                            {
                                var match = Regex.Match(line, pattern);
                                if (match.Success)
                                {
                                    var dateString = match.Groups[1].Value;
                                    if (!DateTime.TryParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var generatedDate))
                                    {
                                        Console.WriteLine("date error");
                                    }
                                    Console.WriteLine($"{generatedDate:dd/MM/yyyy}\r\n{fileDate:dd/MM/yyyy}");
                                    //Console.WriteLine($"generated date {generatedDate:dd/MM/yyyy} file date {fileDate:dd/MM/yyyy}");
                                }
                            }
                        }
                    }
                }
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                var codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                var progname = Path.GetFileNameWithoutExtension(codeBase);
                Console.Error.WriteLine(progname + ": Error: " + ex.Message);
                Console.WriteLine();
            }

        }
    }
}
