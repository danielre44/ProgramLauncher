using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ProgramLauncher.Common;
using System.Windows;

namespace ProgramLauncher.Model.FileSystem
{
    public class FileSystemModel : IStoppable
    {

        #region Types

        /// <summary>
        /// Interface for Processable Events. 
        /// </summary>
        private interface IProcessableEvent
        {
            #region Methods

            /// <summary>
            /// Process implementation for the Event.
            /// </summary>
            /// <param name="fileModel">Access to the FileModel class.</param>
            void Process(FileSystemModel fileModel);

            #endregion
        }

        /// <summary>
        /// Processes a File Add event.
        /// </summary>
        private class AddFileEvent : IProcessableEvent
        {
            #region Fields

            private readonly string _absoluteFilePath;

            #endregion

            #region Constructors 

            public AddFileEvent(string absoluteFilePath)
            {
                this._absoluteFilePath = absoluteFilePath;
            }

            #endregion

            #region Public Methods 

            public void Process(FileSystemModel fileModel)
            {
                FileData addedFile = FileData.TryCreateFromAbsolutePath(this._absoluteFilePath);

                if (addedFile != null)
                {
                    lock (fileModel._fileMapLock)
                    {
                        if (!fileModel._fileMap.ContainsKey(addedFile.AbsoluteFilePath))
                        {
                            fileModel._fileMap.Add(addedFile.AbsoluteFilePath, addedFile);

                            fileModel.FireFileAddedEvent(addedFile);
                        }
                        else
                        {
                            // TODO
                            Logger.LogWarning("Attempted to add file that already exists.");
                        }
                    }
                }
                else
                {
                    Logger.LogError("Failed to create FileData with absolute path: " + this._absoluteFilePath);
                }
            }

            #endregion
        }

        /// <summary>
        /// Processes a File Remove event.
        /// </summary>
        private class RemoveFileEvent : IProcessableEvent
        {
            #region Fields

            private readonly string _absoluteFilePath;

            #endregion

            #region Constructors 

            public RemoveFileEvent(string absoluteFilePath)
            {
                this._absoluteFilePath = absoluteFilePath;
            }

            #endregion

            #region Public Methods 

            public void Process(FileSystemModel fileModel)
            {
                lock (fileModel._fileMapLock)
                {
                    if (fileModel._fileMap.ContainsKey(this._absoluteFilePath))
                    {
                        FileData removedFile = fileModel._fileMap[this._absoluteFilePath];
                        fileModel._fileMap.Remove(this._absoluteFilePath);

                        fileModel.FireFileRemovedEvent(removedFile);
                    }
                    else
                    {
                        // TODO
                        Logger.LogWarning("Attempted to remove file that not in map.");
                    }
                }
            }

            #endregion
        }

        /// <summary>
        /// Processes a DirectoryListener Add event.
        /// </summary>
        private class AddDirectoryListenerEvent : IProcessableEvent
        {
            #region Fields

            private readonly string _absoluteDirectoryPath;

            #endregion

            #region Constructors 

            public AddDirectoryListenerEvent(string absoluteDirectoryPath)
            {
                this._absoluteDirectoryPath = DirectoryStringFormatter.Format(absoluteDirectoryPath);
            }

            #endregion

            #region Public Methods 

            public void Process(FileSystemModel fileModel)
            {
                lock (fileModel._directoryListenerMapLock)
                {
                    if (!fileModel._directoryListenerMap.ContainsKey(this._absoluteDirectoryPath))
                    {
                        fileModel._directoryListenerMap.Add(this._absoluteDirectoryPath, new DirectoryListener(fileModel, this._absoluteDirectoryPath));

                        fileModel.FireDirectoryAddedEvent(this._absoluteDirectoryPath);
                    }
                    else
                    {
                        // TODO
                        Logger.LogWarning("Attempted to add listener for directory that we're already listening to.");
                    }
                }
            }
            #endregion
        }

        /// <summary>
        /// Processes a DirectoryListener Remove event.
        /// </summary>
        private class RemoveDirectoryListenerEvent : IProcessableEvent
        {
            #region Fields

            private readonly string _absoluteDirectoryPath;

            #endregion

            #region Constructors 

            public RemoveDirectoryListenerEvent(string absoluteDirectoryPath)
            {
                this._absoluteDirectoryPath = DirectoryStringFormatter.Format(absoluteDirectoryPath);
            }

            #endregion

            #region Public Methods 

            public void Process(FileSystemModel fileModel)
            {
                lock (fileModel._directoryListenerMapLock)
                {
                    if (fileModel._directoryListenerMap.ContainsKey(this._absoluteDirectoryPath))
                    {
                        DirectoryListener directoryListener = fileModel._directoryListenerMap[this._absoluteDirectoryPath];
                        /*
                         * Stop the listener.
                         */
                        directoryListener.Stop();
                        // Finish removing the directory from our internal map
                        fileModel._directoryListenerMap.Remove(this._absoluteDirectoryPath);

                        fileModel.FireDirectoryRemovedEvent(this._absoluteDirectoryPath);
                    }
                    else
                    {
                        // TODO
                        Logger.LogWarning("Attempted to remove listener for directory that are not listening to.");
                    }
                }
            }

            #endregion
        }

        #endregion

        #region Fields

        private readonly ThreadedQueue<IProcessableEvent> _processingQueue;

        private readonly Dictionary<string, FileData> _fileMap;
        private readonly object _fileMapLock;


        private readonly Dictionary<string, DirectoryListener> _directoryListenerMap;
        private readonly object _directoryListenerMapLock;

        private readonly object _stopLock;

        #endregion

        #region Constructors

        public FileSystemModel()
        {
            this._processingQueue = new ThreadedQueue<IProcessableEvent>("FileModel Processor");

            this._fileMap = new Dictionary<string, FileData>();
            this._fileMapLock = new object();

            this._directoryListenerMap = new Dictionary<string, DirectoryListener>();
            this._directoryListenerMapLock = new object();

            this._stopLock = new object();


            this._processingQueue.Start(this.Process);
        }

        #endregion

        #region Events

        /*
         * TODO: On GUI thread...
         */
        public event Action<FileData> FileAdded;
        public event Action<FileData> FileRemoved;
        public event Action<string> DirectoryAdded;
        public event Action<string> DirectoryRemoved;

        #endregion

        #region Properties

        public bool Stopped
        {
            get;
            private set;
        }

        public FileData[] AllFiles
        {
            get
            {
                lock (this._fileMapLock)
                {
                    return this._fileMap.Values.ToArray();
                }
            }
        }

        public string[] AllDirectories
        {
            get
            {
                lock (this._directoryListenerMapLock)
                {
                    return this._directoryListenerMap.Keys.ToArray();
                }
            }
        }


        #endregion

        #region Public Methods

        // TODO
        public void Stop()
        {
            if (false == this.Stopped)
            {
                lock (this._stopLock)
                {
                    if (false == this.Stopped)
                    {
                        this._processingQueue.Stop();


                        this._fileMap.Clear();

                        foreach (DirectoryListener directoryListener in _directoryListenerMap.Values)
                        {
                            directoryListener.Stop();
                        }
                        this._directoryListenerMap.Clear();
                        
                    }
                }
            }
        }
        
        public void ListenToDirectory(string absoluteDirectoryPath)
        {
            /*
             * Note:
             *  - ToLower is called because windows is not case sensitive, 
             *    and we do not want duplicates..
             */
            this.InternalAddProcessableEvent(new AddDirectoryListenerEvent(absoluteDirectoryPath.ToLower()));
        }

        public void StopListeningToDirectory(string absoluteDirectoryPath)
        {
            /*
             * Note:
             *  - ToLower is called because windows is not case sensitive, 
             *    and we want to make sure we remove the specified
             *    directory.
             */
            this.InternalAddProcessableEvent(new RemoveDirectoryListenerEvent(absoluteDirectoryPath.ToLower()));
        }

        public void AddFile(string filePath)
        {
            this.InternalAddProcessableEvent(new AddFileEvent(filePath));
        }

        public void RemoveFile(string filePath)
        {
            this.InternalAddProcessableEvent(new RemoveFileEvent(filePath));
        }

        #endregion

        #region Private Methods

        private void Process(IProcessableEvent processableEvent)
        {
            processableEvent.Process(this);
        }

        private void InternalAddProcessableEvent(IProcessableEvent processableEvent)
        {
            this._processingQueue.Enqueue(processableEvent);
        }      

        private void FireFileAddedEvent(FileData fileData)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(
                () =>
                {
                    if (this.FileAdded != null)
                    {
                        this.FileAdded(fileData);
                    }
                }));
        }

        private void FireFileRemovedEvent(FileData fileData)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(
                () =>
                {
                    if (this.FileRemoved != null)
                    {
                        this.FileRemoved(fileData);
                    }
                } ));
        }

        private void FireDirectoryAddedEvent(string directory)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(
                () =>
                {
                    if (this.DirectoryAdded != null)
                    {
                        this.DirectoryAdded(directory);
                    }
                }));
        }

        private void FireDirectoryRemovedEvent(string directory)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(
                () =>
                {
                    if (this.DirectoryRemoved != null)
                    {
                        this.DirectoryRemoved(directory);
                    }
                }));
        }

        #endregion
    }
}
