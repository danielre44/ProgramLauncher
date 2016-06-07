using ProgramLauncher.Common;
using ProgramLauncher.Model.FileSystem;
using ProgramLauncher.ViewModel.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;

namespace ProgramLauncher.ViewModel
{
    public class ConfigurationWindowViewModel : BaseNotifyPropertyChanged
    {
        #region Fields

        private readonly FileSystemModel _fileSystemModel;
        private DirectoryViewData[] _originalDirectoryData;
        private readonly ObservableCollection<DirectoryViewData> _directories;

        private readonly HashSet<DirectoryViewData> _directoriesWithListeners;

        private readonly EnumCommandHandler<ConfigurationWindowCommand> _commandHandler;

        private bool _directoriesModified;
        private bool _directoriesValid;
        private DirectoryViewData _selectedDirectory;

        #endregion

        #region Constructors

        public ConfigurationWindowViewModel(FileSystemModel fileSystemModel)
        {
            this._fileSystemModel = fileSystemModel;
            this._directories = new ObservableCollection<DirectoryViewData>();
            this._directoriesWithListeners = new HashSet<DirectoryViewData>();
            this._commandHandler = new EnumCommandHandler<ConfigurationWindowCommand>(this.CanExecute, this.Execute);


            this._directories.Clear();
            {
                string[] directoryData = this._fileSystemModel.AllDirectories;
                this._originalDirectoryData = new DirectoryViewData[directoryData.Length];

                for (int i = 0; i < directoryData.Length; i++)
                {
                    DirectoryViewData directoryViewData = new DirectoryViewData(directoryData[i]);
                    this._originalDirectoryData[i] = directoryViewData;
                    this._directories.Add(directoryViewData);
                }
            }

            this._directoriesModified = false;
            this._directoriesValid = true;
            this._selectedDirectory = null;

            this._fileSystemModel.DirectoryAdded += this.DirectoryAddedHandler;
            this._fileSystemModel.DirectoryAdded += this.DirectoryRemovedHandler;

            this._directories.CollectionChanged += this.DirectoriesCollectionChanged;
        }

        private void DirectoryRemovedHandler(string directory)
        {
            //if (this._directories.Contains(Directory))
            // TODO???
            if (false == this.DirectoriesModified)
            {
                this.ResetDirectories();
            }
        }

        private void DirectoryAddedHandler(string directory)
        {
            //throw new NotImplementedException();
            if (false == this.DirectoriesModified)
            {
                this.ResetDirectories();
            }
        }


        #endregion


        private void ResetDirectories()
        {
            this._directories.Clear();
            {
                string[] directoryData = this._fileSystemModel.AllDirectories;
                this._originalDirectoryData = new DirectoryViewData[directoryData.Length];

                for (int i = 0; i < directoryData.Length; i++)
                {
                    DirectoryViewData directoryViewData = new DirectoryViewData(directoryData[i]);
                    this._originalDirectoryData[i] = directoryViewData;
                    this._directories.Add(directoryViewData);
                }
            }

            this.DirectoriesModified = false;
            this.DirectoriesValid = true;
        }


        #region Properties

        /*
         * TODO: Not observable collection!?
         */
        public ObservableCollection<DirectoryViewData> Directories
        {
            get { return this._directories; }
        }

        public bool DirectoriesModified
        {
            get { return this._directoriesModified; }
            private set { this.SetProperty(value, ref this._directoriesModified); }
        }

        public bool DirectoriesValid
        {
            get { return this._directoriesValid; }
            private set { this.SetProperty(value, ref this._directoriesValid); }
        }

        public ICommand CommandHandler
        {
            get { return this._commandHandler; }
        }

        public DirectoryViewData SelectedDirectory
        {
            get { return this._selectedDirectory; }
            set { this.SetProperty(value, ref this._selectedDirectory); }
        }


        #endregion

        #region Public Methods

        public void ApplyData()
        {
            Logger.LogTrace("Applying data!");

            string[] dirs = this._fileSystemModel.AllDirectories.Select(x => DirectoryStringFormatter.Format(x)).ToArray();
            string[] internalDirStringList = this._directories.Select(x => DirectoryStringFormatter.Format(x.AbsolutePath)).ToArray();

            // Remove directories
            foreach (string absolutePath in dirs)
            {
                if (!internalDirStringList.Contains(absolutePath))
                {
                    this._fileSystemModel.StopListeningToDirectory(absolutePath);
                }
            }

            // Add necessary directories
            foreach (string currentAbsolutePath in internalDirStringList)
            {
                if (!dirs.Contains(currentAbsolutePath))
                {
                    this._fileSystemModel.ListenToDirectory(currentAbsolutePath);
                }
            }

            // Reset our internal directories
            this.ResetDirectories();
        }

        public void OnClosed()
        {
            Logger.LogTrace("Configuration window closing!");

            this._directories.Clear();

            this._directories.CollectionChanged -= this.DirectoriesCollectionChanged;

            Logger.LogTrace("End");
        }

        #endregion

        #region Private Methods

        private bool CanExecute(ConfigurationWindowCommand command)
        {
            bool canExecute = false;

            switch (command)
            {
                case ConfigurationWindowCommand.AddDirectory:
                    canExecute = true;
                    break;

                case ConfigurationWindowCommand.RemoveDirectory:
                    canExecute = (null != this.SelectedDirectory);
                    break;
            }

            return canExecute;
        }

        private void Execute(ConfigurationWindowCommand command)
        {
            switch (command)
            {
                case ConfigurationWindowCommand.AddDirectory:
                    this._directories.Add(new DirectoryViewData("c:\\"));
                    break;

                case ConfigurationWindowCommand.RemoveDirectory:
                    if (null != this.SelectedDirectory)
                    {
                        this._directories.Remove(this.SelectedDirectory);
                    }
                    this.SelectedDirectory = null;

                    break;
            }
        }

        private bool AreDirectoriesValid()
        {
            bool directoriesValid = true;

            foreach (DirectoryViewData data in this._directories)
            {
                if (false == data.PathValid)
                {
                    directoriesValid = false;
                    break;
                }
            }

            return directoriesValid;
        }

        private bool AreDirectoriesModified()
        {
            bool modified = false;

            if (this._originalDirectoryData.Length == this._directories.Count())
            {
                foreach (DirectoryViewData data in this._originalDirectoryData)
                {
                    if (!this._directories.Contains(data))
                    {
                        modified = true;
                        break;
                    }
                }

                if (false == modified)
                {
                    foreach (DirectoryViewData data in this._directories)
                    {
                        if (data.Modified)
                        {
                            modified = true;
                            break;
                        }
                    }
                }

            }
            else
            {
                modified = true;
            }
            
            return modified;
        }

        protected override void HandleInternalPropertyChanged(string property)
        {
            // TODO
            if (property == "SelectedDirectory")
            {
                this._commandHandler.FireCanExecuteChanged(this, new EventArgs());
            }
        }

        private void DirectoryChanged(object sender, PropertyChangedEventArgs e)
        {
            this.UpdateDirectoryFlags();
        }

        private void UpdateDirectoryFlags()
        {
            this.DirectoriesModified = this.AreDirectoriesModified();
            this.DirectoriesValid = this.AreDirectoriesValid();
        }

        private void DirectoriesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            bool reset = false;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                    break;

                case NotifyCollectionChangedAction.Replace:
                    Logger.LogWarning("No implementation for Replace! Using same implementation as add/remove.");
                    break;

                case NotifyCollectionChangedAction.Move:
                    Logger.LogWarning("No implementation for Move! Using same implementation as add/remove.");
                    break;

                case NotifyCollectionChangedAction.Reset:
                    reset = true;
                    break;

                default:
                    Logger.LogWarning("Unhandled case hit: " + e.Action);
                    break;
            }

            if (reset)
            {
                foreach (DirectoryViewData data in this._directoriesWithListeners)
                {
                    data.PropertyChanged -= this.DirectoryChanged;
                }

                this._directoriesWithListeners.Clear();
            }
            else
            {
                if (null != e.OldItems)
                {
                    foreach (DirectoryViewData data in e.OldItems)
                    {
                        if (this._directoriesWithListeners.Contains(data))
                        {
                            data.PropertyChanged -= this.DirectoryChanged;
                            this._directoriesWithListeners.Remove(data);

                        }
                    }
                }

                if (null != e.NewItems)
                {
                    foreach (DirectoryViewData data in e.NewItems)
                    {
                        if (!this._directoriesWithListeners.Contains(data))
                        {
                            data.PropertyChanged += this.DirectoryChanged;
                            this._directoriesWithListeners.Add(data);
                        }
                    }
                }
            }

            this.UpdateDirectoryFlags();
        }


        #endregion


    }
}
