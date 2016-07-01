using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MonoGameEngine.Engine.Threading {
    public class ThreadManager {
        private List<Thread> _runningThreads = new List<Thread>();

        private static ThreadManager _instance;

        public static Thread NewThread(ThreadStart start) {
            if(_instance == null)
                _instance = new ThreadManager();
            
            var thread = new Thread(start);

            _instance._runningThreads.Add(thread);

            thread.Start();
            return thread;
        }

        public static void StopThread(Thread thread) {
            if (_instance == null)
                return;

            thread.Abort();

            _instance._runningThreads.Remove(thread);
        }

        public static void StopAllThreads() {
            foreach (var t in _instance._runningThreads) {
                t.Abort();
            }

            _instance._runningThreads.Clear();
        }
    }
}
