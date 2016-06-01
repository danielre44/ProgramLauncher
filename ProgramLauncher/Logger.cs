using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProgramLauncher
{
    public class Logger
    {
        #region Fields

        private static Logger _logger = null;
        private static object _creationLock = new object();

        private readonly Thread _loggerThread;
        private Queue<Tuple<string, string, string, DateTime, string>> _logQueue;
        private readonly object _logQueueLock;
        private bool _running;

        #endregion

        #region Constructors

        private Logger()
        {
            _loggerThread = new Thread(this.LogThreadEntryPoint);
            _loggerThread.Name = "Logger";

            _logQueue = new Queue<Tuple<string, string, string, DateTime, string>>();
            _logQueueLock = new object();

            _running = false;
        }

        #endregion

        public void Init()
        {
            // TODO
            _running = true;

            if (_loggerThread.ThreadState != System.Threading.ThreadState.Unstarted)
            {
                // TODO
                throw new Exception();
            }

            _loggerThread.Start();
        }

        public void Shutdown()
        {
            _running = false;

            _loggerThread.Join();
        }

        public static Logger Instance
        {
            get
            {
                if (_logger == null)
                {
                    lock (_creationLock)
                    {
                        if (_logger == null)
                        {
                            _logger = new Logger();
                        }
                    }
                }

                return _logger;
            }
        }

        public static void LogDebug(string text, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerName = "")
        {
            Instance.Log("Debug", text, callerFile, callerName);
        }

        public static void LogTrace(string text, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerName = "")
        {
            Instance.Log("Trace", text, callerFile, callerName);
        }

        public static void LogInfo(string text, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerName = "")
        {
            Instance.Log("Info", text, callerFile, callerName);
        }

        public static void LogWarning(string text, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerName = "")
        {
            Instance.Log("Warning", text, callerFile, callerName);
        }

        public static void LogError(string text, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerName = "")
        {
            Instance.Log("Error", text, callerFile, callerName);
        }

        public static void LogException(Exception e, [CallerFilePath] string callerFile = "", [CallerMemberName] string callerName = "")
        {
            Instance.Log("Excption", e.ToString(), callerFile, callerName);
        }

        private void Log(string level, string text, string callerFile, string callerName)
        {
            lock (_logQueueLock)
            {
                _logQueue.Enqueue(Tuple.Create(callerFile, callerName, level, DateTime.Now, text));
            }
        }



        private void LogThreadEntryPoint()
        {
            var localQueue = new Queue<Tuple<string, string, string, DateTime, string>>();

            while (_running)
            {
                if (_logQueue.Count > 0)
                {
                    {
                        var workingQueue = _logQueue;

                        lock (_logQueueLock)
                        {
                            _logQueue = localQueue;
                        }
                        localQueue = workingQueue;
                    }

                    while (localQueue.Count > 0)
                    {
                        var cur = localQueue.Dequeue();

                        string file     = Path.GetFileNameWithoutExtension(cur.Item1);
                        string method   = cur.Item2;
                        string level    = cur.Item3;
                        string date     = cur.Item4.ToString("hh:mm:ss.fff");
                        string message  = cur.Item5;


                        Console.WriteLine("[" + level + "] " + date + ": " + file + " " + method + "() - " + message);
                    }
                }

                // TODO
                Thread.Sleep(10);
            }

            while (_logQueue.Count > 0)
            {
                var cur = _logQueue.Dequeue();

                string file = Path.GetFileNameWithoutExtension(cur.Item1);
                string method = cur.Item2;
                string level = cur.Item3;
                string date = cur.Item4.ToString("hh:mm:ss.fff");
                string message = cur.Item5;


                Console.WriteLine("[" + level + "] " + date + ": " + file + " " + method + "() - " + message);
            }

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Shutdown logger");

        }

    }
}

