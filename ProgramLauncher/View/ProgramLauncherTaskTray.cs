using ProgramLauncher.Common;
using ProgramLauncher.Model;
using ProgramLauncher.ViewModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProgramLauncher.View
{
    public class ProgramLauncherTaskTray : Form
    {

        private NotifyIcon trayIcon;
        private ContextMenu trayMenu;

        public ProgramLauncherTaskTray()
        {
            // Create a simple tray menu with only one item.
            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Configuration", OnConfiguration);
            trayMenu.MenuItems.Add("Window", OnWindow);
            trayMenu.MenuItems.Add("Exit", OnExit);

            // Create a tray icon. In this example we use a
            // standard system icon for simplicity, but you
            // can of course use your own custom icon too.
            trayIcon = new NotifyIcon();
            trayIcon.Text = "MyTrayApp";
            trayIcon.Icon = new Icon(SystemIcons.Application, 40, 40);

            // Add menu to tray icon and show it.
            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;
        }

        private void OnConfiguration(object sender, EventArgs e)
        {
            ConfigurationWindow configWindow = new ConfigurationWindow(new ConfigurationWindowViewModel(ProgramLauncherModel.Instance.FileSystemModel));
            configWindow.Show();
        }

        private void OnWindow(object sender, EventArgs e)
        {
            BasicFileWindow fileWindow = new BasicFileWindow(new BasicFileViewModel(ProgramLauncherModel.Instance.FileSystemModel));
            fileWindow.Show();
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = false; // Hide form window.
            ShowInTaskbar = false; // Remove from taskbar.

            base.OnLoad(e);
        }

        private void OnExit(object sender, EventArgs e)
        {
            this.Dispose();

            //Application.Exit();
            System.Windows.Application.Current.Shutdown();
        }

        protected override void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                // Release the icon resource.
                trayIcon.Dispose();
            }

            base.Dispose(isDisposing);
        }
    }

}
