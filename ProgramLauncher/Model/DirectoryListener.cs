using ProgramLauncher.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgramLauncher.Model
{
    public class DirectoryListener
    {
        #region Fields

        // TODO: Write why only one
        private readonly object _lock;

        private FileModel _fileModel;
        private FileSystemWatcher _fileSystemWatcher;
        private HashSet<string> _fileSet;
        
        #endregion

        #region Constructors

        public DirectoryListener(FileModel fileModel, string absoluteDirectoryPath)
        {
            // Initialize class members
            this._lock = new object();

            this._fileModel = fileModel;
            this._fileSystemWatcher = new FileSystemWatcher(absoluteDirectoryPath);
            this._fileSet = new HashSet<string>();
            this.Stopped = false;
            
            // Add event handlers
            this._fileSystemWatcher.Created += new FileSystemEventHandler   ( this.FileSystemWatcher_OnCreated );
            this._fileSystemWatcher.Deleted += new FileSystemEventHandler(this.FileSystemWatcher_OnDeleted);
            this._fileSystemWatcher.Renamed += new RenamedEventHandler(this.FileSystemWatcher_OnRenamed);

            // Get existing files in directory
            foreach (string existingFileAbsolutePath in System.IO.Directory.GetFiles(absoluteDirectoryPath))
            {
                this._fileModel.AddFile(existingFileAbsolutePath);
            }

            // Start listening for changes
            this._fileSystemWatcher.EnableRaisingEvents = true;

            Logger.LogDebug("Started listening to changes for Directory: " + absoluteDirectoryPath);
        }

        #endregion

        #region Public Properties

        public string Directory
        {
            get
            {
                lock (this._lock)
                {
                    if (this.FieldsValid())
                    {
                        return this._fileSystemWatcher.Path;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }
        }

        public bool Stopped
        { 
            get;
            private set;
        }
        
        public string[] DiscoveredFiles
        {
            get
            {
                lock (this._lock)
                {
                    if (this.FieldsValid())
                    {
                        return this._fileSet.ToArray();
                    }
                    else
                    {
                        Logger.LogWarning("Fields not valid, returning empty string array.");
                        return new string[0];
                    }
                }
            }
        }

        #endregion


        #region Public Methods

        public void Stop()
        {
            Logger.LogTrace("Entered");

            // Double check locking
            if (false == this.Stopped)
            {
                lock (this._lock)
                {
                    if (false == this.Stopped)
                    {
                        // Set stopped flag
                        this.Stopped = true;

                        // Stop events
                        this._fileSystemWatcher.EnableRaisingEvents = false;

                        // Remove event handlers
                        this._fileSystemWatcher.Created -= new FileSystemEventHandler(this.FileSystemWatcher_OnCreated);
                        this._fileSystemWatcher.Deleted -= new FileSystemEventHandler(this.FileSystemWatcher_OnDeleted);
                        this._fileSystemWatcher.Renamed -= new RenamedEventHandler(this.FileSystemWatcher_OnRenamed);

                        // Remove files from model
                        foreach (string addedFiles in this.DiscoveredFiles)
                        {
                            this._fileModel.RemoveFile(addedFiles);
                        }

                        // Unset fields
                        this._fileSystemWatcher = null;
                        this._fileModel = null;
                        this._fileSet = null;
                    }
                    else
                    {
                        Logger.LogWarning("DirectoryListener has already been stopped...");
                    }
                }
            }
            else
            {
                Logger.LogWarning("DirectoryListener has already been stopped...");
            }

            Logger.LogTrace("Exiting");
        }
        
        #endregion

        private void FileSystemWatcher_OnCreated(object source, FileSystemEventArgs e)
        {
            Logger.LogTrace("File: " + e.FullPath + " " + e.ChangeType);
            this.InternalAddFile(e.FullPath);
        }

        private void FileSystemWatcher_OnDeleted(object source, FileSystemEventArgs e)
        {
            Logger.LogTrace("File: " + e.FullPath + " " + e.ChangeType);
            this.InternalRemoveFile(e.FullPath);
        }

        private void FileSystemWatcher_OnRenamed(object source, RenamedEventArgs e)
        {
            Logger.LogTrace("File: {0} renamed to {1}", e.OldFullPath, e.FullPath);
            this.InternalRemoveFile(e.OldFullPath);
            this.InternalAddFile(e.FullPath);
        }


        private bool FieldsValid()
        {
            bool fieldsValid = true;

            if (this.Stopped)
            {
                Logger.LogError("DirectoryListener is Stopped!");
                fieldsValid = false;
            }

            if (null == this._fileSystemWatcher)
            {
                Logger.LogError("_fileSystemWatcher is null!");
                fieldsValid = false;
            }

            if (null == this._fileSet)
            {
                Logger.LogError("_fileSet is null!");
                fieldsValid = false;
            }

            return fieldsValid;
        }

        private void InternalAddFile(string absoluteFilePath)
        {
            lock (this._lock)
            {
                if (this.FieldsValid())
                {
                    this._fileModel.AddFile(absoluteFilePath);
                    this._fileSet.Add(absoluteFilePath);
                }
            }
        }

        private void InternalRemoveFile(string absoluteFilePath)
        {
            lock (this._lock)
            {
                if (this.FieldsValid())
                {
                    this._fileModel.RemoveFile(absoluteFilePath);

                    if (this._fileSet.Contains(absoluteFilePath))
                    {
                        this._fileSet.Remove(absoluteFilePath);
                    }
                    else
                    {
                        Logger.LogWarning("Attempted to remove a file path that doesnt exist!");
                    }
                }
            }
        }

    }
}
