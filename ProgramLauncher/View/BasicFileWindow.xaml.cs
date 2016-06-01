using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProgramLauncher.View
{
    /// <summary>
    /// Interaction logic for BasicFileWindow.xaml
    /// </summary>
    public partial class BasicFileWindow : Window
    {
        public BasicFileWindow(BasicFileViewModel viewModel)
        {
            this.ViewModel = viewModel;

            InitializeComponent();
        }


        public BasicFileViewModel ViewModel
        {
            get;
            private set;
        }

    }
}
