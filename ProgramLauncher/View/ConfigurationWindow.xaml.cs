using ProgramLauncher.Common;
using ProgramLauncher.ViewModel;
using ProgramLauncher.ViewModel.Data;
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
using System.ComponentModel;

namespace ProgramLauncher.View
{
    /// <summary>
    /// Interaction logic for ConfigurationWindow.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window
    {

        #region Fields

        private EnumCommandHandler<OkCancelApplyWindowCommand> _okCancelApplyCommandHandler;

        #endregion

        #region Constructors

        public ConfigurationWindow(ConfigurationWindowViewModel configurationViewModel)
        {
            // Set the View Model
            this.ViewModel = configurationViewModel;
            // Create the command handler
            this._okCancelApplyCommandHandler = new EnumCommandHandler<OkCancelApplyWindowCommand>(this.CanExecute, this.Execute);

            // Set property changed event
            this.ViewModel.PropertyChanged += this.OnViewModelPropertyChanged;

            // Initialize the internal window components
            this.InitializeComponent();
        }

        #endregion

        #region Public Properties

        public ConfigurationWindowViewModel ViewModel
        {
            get;
            private set;
        }

        public ICommand OkCancelApplyCommandHandler
        {
            get { return this._okCancelApplyCommandHandler; }
        }

        #endregion

        protected override void OnClosed(EventArgs e)
        {
            if (null != this.ViewModel)
            {
                // Remove property change listener from ViewModel
                this.ViewModel.PropertyChanged -= this.OnViewModelPropertyChanged;
                // Call into viewmodel
                this.ViewModel.OnClosed();
                // Unset viewmodel
                this.ViewModel = null;
            }

            base.OnClosed(e);
        }



        #region Private Methods

        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // TODO
            this._okCancelApplyCommandHandler.FireCanExecuteChanged(this, new EventArgs());
        }

        private void Execute(OkCancelApplyWindowCommand command)
        {
            switch (command)
            {
                case OkCancelApplyWindowCommand.Ok:
                    this.ViewModel.ApplyData();
                    this.Close();
                    break;

                case OkCancelApplyWindowCommand.Apply:
                    this.ViewModel.ApplyData();
                    break;

                case OkCancelApplyWindowCommand.Cancel:
                    this.Close();
                    break;
            }
        }


        private bool CanExecute(OkCancelApplyWindowCommand command)
        {
            bool canExecute = false;

            switch (command)
            {
                case OkCancelApplyWindowCommand.Ok:
                case OkCancelApplyWindowCommand.Apply:
                    canExecute = this.ViewModel.DirectoriesModified && this.ViewModel.DirectoriesValid;
                    break;

                case OkCancelApplyWindowCommand.Cancel:
                    canExecute = true;
                    break;

                default:
                    // TODO
                    break;
            }

            return canExecute;
        }

        #endregion
    }
}
