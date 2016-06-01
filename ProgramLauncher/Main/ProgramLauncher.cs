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
        public static void FileAddedHandlerImpl(FileData fileData)
        {
            Logger.LogInfo("File Added: " + fileData.FileName + " Ext: " + fileData.FileExtension + " Path: " + fileData.AbsoluteFilePath);
        }

        [STAThread]
        static void Main(string[] args)
        {
            Application app = new Application();

            Logger.Instance.Init();


            FileModel model = new FileModel();

            model.ListenToDirectory("D:\\program_launcher_test");

            model.FileAdded += FileAddedHandlerImpl;

            /*
             * Run application until it's closed...
             */
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

