using ProgramLauncher.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProgramLauncher.Main
{
    internal abstract class ProgramLauncher
    {

        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Starting Logger");

            Logger.Instance.Init();

            //foreach (string s in System.IO.Directory.GetFiles("D:\\program_launcher_test"))
            //{
            //    Console.WriteLine("File: \"" + s + "\"");
            //}

            //DirectoryFileManager p = new DirectoryFileManager();

            FileModel model = new FileModel();

            model.ListenToDirectory("D:\\program_launcher_test");

            //System.Threading.Thread.Sleep(1000);

            //model.StopListeningToDirectory("D:\\program_launcher_test");

            //System.Threading.Thread.Sleep(10000);

            /*
             * Run application until it's closed...
             */
            Application app = new Application();
            app.Run(new TestWindow());

            // Shutdown FileModel.
            Logger.LogInfo("Start - Shutting down FileModel.");
            model.Stop();
            Logger.LogInfo("Finished - Shutting down FileModel.");

            // Shutdown Logger.
            Console.WriteLine("Shutting down logger...");
            Logger.Instance.Shutdown();
            Console.WriteLine("Logger shutdown...");

        }
    }

}

