using OnlineCode;
using ProgramLauncher.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestPerformance
{
    class Program
    {
        static void Main(string[] args)
        {
            ObservableCollection<CustomFileData> dataList = new ObservableCollection<CustomFileData>();

            DateTime start = DateTime.Now;

            for (int i = 0; i < 1000000; i++)
            {
                dataList.Add(new CustomFileData(Guid.NewGuid().ToString().Substring(0, 4), Guid.NewGuid().ToString().Substring(0, 4), Guid.NewGuid().ToString().Substring(0, 8)));        
            }

            TimeSpan elapsedTime = DateTime.Now - start;

            Thread.Sleep(1000);

            Levenshtein l = new Levenshtein();

            for (int i = 0; i < 5; i++)
            {
                string s = Guid.NewGuid().ToString().Substring(0, 8);

                DateTime startLoop = DateTime.Now;

                foreach (CustomFileData d in dataList)
                {
                    //d.val = String.Compare(d.FileName, s);
                    d.val = Levenshtein.iLD(d.FileName, s);
                }

                TimeSpan elapsedLoop = DateTime.Now - startLoop;
                Console.WriteLine("Loop Took (ms): " + elapsedLoop.TotalMilliseconds);

                Thread.Sleep(1000);
            }

            Console.WriteLine("Create Took: " + elapsedTime);

            Console.WriteLine("Text : " + Guid.NewGuid().ToString());
        }
    }
}
