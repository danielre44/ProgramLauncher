using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using ProgramLauncher.Common;

namespace ProgramLauncher.Model
{
    public class FileModel : IStoppable
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
            void Process(FileModel fileModel);

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

            public void Process(FileModel fileModel)
            {
                FileData addedFile = FileData.TryCreateFromAbsolutePath(this._absoluteFilePath);

                if (addedFile != null)
                {
                    lock (fileModel._fileMapLock)
                    {
                        if (!fileModel._fileMap.ContainsKey(addedFile.AbsoluteFilePath))
                        {
                            fileModel._fileMap.Add(addedFile.AbsoluteFilePath, addedFile);
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

            public void Process(FileModel fileModel)
            {
                lock (fileModel._fileMapLock)
                {
                    if (fileModel._fileMap.ContainsKey(this._absoluteFilePath))
                    {
                        fileModel._fileMap.Remove(this._absoluteFilePath);
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
                this._absoluteDirectoryPath = absoluteDirectoryPath;
            }

            #endregion

            #region Public Methods 

            public void Process(FileModel fileModel)
            {
                lock (fileModel._directoryListenerMapLock)
                {
                    if (!fileModel._directoryListenerMap.ContainsKey(this._absoluteDirectoryPath))
                    {
                        fileModel._directoryListenerMap.Add(this._absoluteDirectoryPath, new DirectoryListener(fileModel, this._absoluteDirectoryPath));
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
                this._absoluteDirectoryPath = absoluteDirectoryPath;
            }

            #endregion

            #region Public Methods 

            public void Process(FileModel fileModel)
            {
                lock (fileModel._directoryListenerMapLock)
                {
                    if (fileModel._directoryListenerMap.ContainsKey(this._absoluteDirectoryPath))
                    {
                        fileModel._directoryListenerMap[this._absoluteDirectoryPath].Stop();

                        fileModel._directoryListenerMap.Remove(this._absoluteDirectoryPath);
                        //GC.Collect();
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

        /*
            * TODO:
            *   - Replace with faster collection...
            */
        private readonly object _processQueueLock;
        private readonly Queue<IProcessableEvent> _processQueue;
        private bool _processing;

        private readonly Dictionary<string, FileData> _fileMap;
        private readonly object _fileMapLock;


        private readonly Dictionary<string, DirectoryListener> _directoryListenerMap;
        private readonly object _directoryListenerMapLock;

        private readonly Thread _processingThread;
        private readonly object _stopLock;

        //private readonly Queue<>

        #endregion

        #region Constructors

        public FileModel()
        {
            this._processQueueLock = new object();

            //this._processQueue
            this._processQueue = new Queue<IProcessableEvent>();
            this._processing = false;
            this._fileMap = new Dictionary<string, FileData>();
            this._fileMapLock = new object();
            this._directoryListenerMap = new Dictionary<string, DirectoryListener>();
            this._directoryListenerMapLock = new object();
            this._processingThread = new Thread(this.ProcessThreadEntryPoint);
            this._stopLock = new object();

            this._processingThread.Name = "FileModel - Processing Thread";
            this._processingThread.Start();
        }

        #endregion

        #region Properties

        public bool Stopped
        {
            get
            {
                return (false == this._processing);
            }
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


        #endregion

        #region Public Methods
        
        // TODO
        public void Stop()
        {
            if (false == this.Stopped)
            {
                this._processing = false;

                // TODO
                lock (this._processQueueLock)
                {
                    Monitor.PulseAll(this._processQueueLock);
                }

                
                this._processQueue.Clear();
                this._fileMap.Clear();

                foreach (DirectoryListener directoryListener in _directoryListenerMap.Values)
                {
                    directoryListener.Stop();
                }
                this._directoryListenerMap.Clear();

                // TODO
                this._processingThread.Join();
            }
        }
        
        public void ListenToDirectory(string absoluteDirectoryPath)
        {
            this.InternalAddProcessableEvent(new AddDirectoryListenerEvent(absoluteDirectoryPath));
        }

        public void StopListeningToDirectory(string absoluteDirectoryPath)
        {
            this.InternalAddProcessableEvent(new RemoveDirectoryListenerEvent(absoluteDirectoryPath));
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
        
        private void InternalAddProcessableEvent(IProcessableEvent processableEvent)
        {
            lock (this._processQueueLock)
            {
                this._processQueue.Enqueue(processableEvent);
                Monitor.Pulse(this._processQueueLock);
            }
        }

        private void ProcessThreadEntryPoint()
        {
            Logger.LogTrace("Entered");
            this._processing = true;

            while (this._processing)
            {
                var localQueue = new Queue<IProcessableEvent>();

                // Get all items from processing queue
                lock (this._processQueueLock)
                {
                    // Wait until we have new data
                    if (this._processQueue.Count <= 0 && this._processing)
                    {
                        Monitor.Wait(this._processQueueLock);
                    }

                    // In case we're signaled to shutdown, make sure we should process the queue
                    while (this._processing && (this._processQueue.Count > 0))
                    {
                        localQueue.Enqueue(this._processQueue.Dequeue());
                    }
                }

                // Process items
                Logger.LogTrace("Starting to processes, Queue Size: " + localQueue.Count);

                while (this._processing && (localQueue.Count > 0))
                {
                    IProcessableEvent processableEvent = localQueue.Dequeue();

                    processableEvent.Process(this);                    
                }
            }

            Logger.LogTrace("Exiting");
        }

        #endregion
    }
}
