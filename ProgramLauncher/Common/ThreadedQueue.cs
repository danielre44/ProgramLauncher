using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProgramLauncher.Common
{
    public class ThreadedQueue<T> 
    {
        #region Fields

        private readonly object _queueLock;
        private readonly object _startStopLock;
        private readonly string _threadIdentifier;

        private Thread _thread;
        private bool _running;
        private Queue<T> _queue;
        private ProcessThreadedQueueItem<T> _callback;

        #endregion

        #region Constructors

        public ThreadedQueue(string threadIdentifier)
        {
            this._queueLock = new object();
            this._startStopLock = new object();
            this._threadIdentifier = threadIdentifier;
            this._thread = null;
            this._running = false;
            this._queue = null;
            this._callback = null;
        }

        #endregion

        #region Properties

        public bool IsRunning
        {
            get
            {
                return this._running;
            }
        }
        

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts the Queue Thread.
        /// </summary>
        /// <param name="callback"></param>
        public void Start(ProcessThreadedQueueItem<T> callback)
        {
            // TODO
            if (callback == null)
            {
                Logger.LogError("Attempted to start threaded queue with null callback!");
                return;
            }

            lock (this._startStopLock)
            {
                if (false == this._running)
                {
                    // Create all necessary objects
                    this._running = true;
                    this._queue = new Queue<T>();
                    this._callback = callback;
                    this._thread = new Thread(this.ThreadEntryPoint);

                    // Set thread name
                    this._thread.Name = "ThreadedQueue - Processing Thread: " + this._threadIdentifier;
                    
                    // Start thread
                    this._thread.Start();
                }
                else
                {
                    Logger.LogWarning("Attempted to start when it is already running!");
                }
            }
        }

        /// <summary>
        /// Stops the QueueThread from processing.
        /// </summary>
        public void Stop()
        {
            lock (this._startStopLock)
            {
                // Note - HasValidFields only run if _running is true...
                if (this._running && this.HasValidFields())
                {
                    // Set running to false, so that the thread ends
                    this._running = false;

                    // Signal thread to wakeup
                    lock (this._queueLock)
                    {
                        Monitor.PulseAll(this._queueLock);
                    }

                    // Wait for thread to end
                    this._thread.Join();

                    // Clear the queue
                    this._queue.Clear();

                    // Unset class fields
                    this._thread = null;
                    this._queue = null;
                    this._callback = null;
                }
                else
                {
                    Logger.LogWarning("Failed to stop thread: " + this._threadIdentifier);
                }
            }
        }

        /// <summary>
        /// Enqueues an item to be processed on it's internal thread.
        /// </summary>
        /// <param name="item">The item to enqueue.</param>
        public void Enqueue(T item)
        {
            // TODO
            if (null == item)
            {
                Logger.LogError("Cannot Enqueue null object!");
                return;
            }

            if (this.HasValidFields())
            {
                lock (this._queueLock)
                {
                    this._queue.Enqueue(item);
                    Monitor.Pulse(this._queueLock);
                }
            }
            else
            {
                Logger.LogError("Queue doesn't have valid fields, not adding data...");
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Entry point for the internal processing Thread.
        /// </summary>
        private void ThreadEntryPoint()
        {
            Logger.LogTrace("Entered for thread: " + this._threadIdentifier);

            // TODO
            if (false == this.HasValidFields())
            {
                Logger.LogError("Entered with invalid fields... Exiting thread: " + this._threadIdentifier);
                return;
            }
            
            while (this._running)
            {
                var localQueue = new Queue<T>();

                // Get all items from processing queue
                lock (this._queueLock)
                {
                    /*
                     * Wait until we have new data
                     *  OR 
                     * Are signaled to stop
                     */
                    if ((this._queue.Count <= 0) && this._running)
                    {
                        Monitor.Wait(this._queueLock);
                    }

                    /*
                     * Run until the queue is empty
                     *  OR
                     * We were told to stop running
                     */
                    while ((this._queue.Count > 0) && this._running)
                    {
                        localQueue.Enqueue(this._queue.Dequeue());
                    }
                }

                /*
                 * Process items on local Queue until either:
                 *   1 - Local Queue is empty
                 *   2 - We're told to stop running
                 */
                while ((localQueue.Count > 0) && this._running)
                {
                    this._callback(localQueue.Dequeue());
                }
            }

            Logger.LogTrace("Exiting for thread: " + this._threadIdentifier);
        }

        /// <summary>
        /// Checks the class fields to see if they're valid, and logs errors if not.
        /// </summary>
        /// <returns>A value indicating whether or not the class fields are valid.</returns>
        private bool HasValidFields()
        {
            // Innocent until proven guilty
            bool fieldsValid = true;

            if (false == this._running)
            {
                Logger.LogError("Not running!");
                fieldsValid = false;
            }

            if (null == this._queue)
            {
                Logger.LogError("Queue is null!");
                fieldsValid = false;
            }

            if (null == this._thread)
            {
                Logger.LogError("Thread is null!");
                fieldsValid = false;
            }
            //else if (ThreadState.Running != this._thread.ThreadState)
            //{
            //    Logger.LogError("Thread is not running!");
            //    fieldsValid = false;
            //}

            if (null == this._callback)
            {
                Logger.LogError("Callback is null!");
                fieldsValid = false;
            }

            return fieldsValid;
        }

        #endregion

    }
}
