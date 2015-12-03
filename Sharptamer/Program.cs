using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sharptamer
{
    class Program
    {
        static void Main(string[] args)
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            IDictionary<Sequence, int> d = new Dictionary<Sequence, int>();
            var s = new List<Sequence>();
            int count = 0;
            using (System.IO.FileStream file = System.IO.File.OpenRead(@"../../70HRT14.fastq"))
            //using (System.IO.FileStream file = System.IO.File.OpenRead(@"../../test.fastq"))
            //using (System.IO.FileStream file = System.IO.File.OpenRead(@"../../tiny.fastq"))
            {

                const int l = 1024 * 1024;
                byte[] b = new byte[l];
                int read = file.Read(b, 0, l);
                if (read == 0)
                {
                    Console.WriteLine("File was empty");
                    return;
                }
                int pos = 0;
                while (read > 0)
                {
                    //Read the first header line
                    discardLine(file, b, ref read, ref pos, l);  // Header
                    //new Sequence(file, b, ref read, ref pos, l); // Sequence
                    discardLine(file, b, ref read, ref pos, l); // +
                    discardLine(file, b, ref read, ref pos, l); // Quality
                    count++;
                }

            }
            /*s.AsParallel<String>().Select<String, Sequence>(x => new Sequence(x)).
            Aggregate<Sequence, IDictionary<Sequence, int>>(d, (IDictionary<Sequence, int> dict, Sequence ss) =>
            {
                if (!dict.ContainsKey(ss))
                {
                    dict.Add(ss, 1);
                }
                else
                {
                    dict[ss]++;
                } 
                return dict;
            });
            */
            watch.Stop();
            Console.WriteLine(count + ": " + s.Count + " unique in " + watch.ElapsedMilliseconds / 1000f + " seconds");

        }

        private static void discardLine(FileStream file, byte[] b, ref int read, ref int pos, int maxRead)
        {
            bool header = false;
            while (!header && read > 0)
            {
                for (;pos < read && !(header = (b[pos] == '\n')); pos++) ;
                if (!header)
                {
                    read = file.Read(b, 0, maxRead);
                    pos = 0;
                }
                else
                {
                    pos++;
                }
            }
        }
    }
}
