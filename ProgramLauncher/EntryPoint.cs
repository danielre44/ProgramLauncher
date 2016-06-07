using ProgramLauncher.Common;
using ProgramLauncher.Model;
using ProgramLauncher.Model.FileSystem;
using ProgramLauncher.View;
using ProgramLauncher.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProgramLauncher
{
    internal abstract class EntryPoint
    {
        [STAThread]
        static void Main(string[] args)
        {
            // Create application
            Application app = new Application();
            // Create Logger
            Logger.Instance.Init();
            // Create program launcher model
            ProgramLauncherModel.Instance.Initialize();
            // Create task tray item
            ProgramLauncherTaskTray taskTrayItem = new ProgramLauncherTaskTray();

            app.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            try
            {
                // Run Application
                app.Run();
            }
            finally
            {
                taskTrayItem.Dispose();
                // Shutdown program launcher model
                ProgramLauncherModel.Instance.Shutdown();

                // Shutdown Logger.
                Console.WriteLine("Shutting down logger...");
                Logger.Instance.Shutdown();
                Console.WriteLine("Logger shutdown...");

            }
            
        }
    }

}

