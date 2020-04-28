using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ThreadPool
{
    /// <summary>
    /// MyThreadPool class
    /// </summary>
    public class TPool
    {
        public int ThreadNumber { get; }
        private BlockingCollection<Action> taskqueue = new BlockingCollection<Action>();

        public TPool(int numberthreads)
        {
            ThreadNumber = numberthreads;
            StartThread();
        }

        /// <summary>
        /// Adding task to ThreadPool queue
        /// </summary>
        public IMyTask<TResult> Add<TResult>(Func<TResult> func)
        {
            var task = new MyTask<TResult>(func, this);
            taskqueue.Add(task.Calculate());
            return task;
        }

        /// <summary>
        /// Starting with "ThreadNumber" number of threads
        /// </summary>
        private void StartThread()
        {
            for(int i = 0; i < ThreadNumber; i++)
            {
                new Thread(()=> 
                {
                    while (true)
                    {
                        taskqueue.Take().Invoke();
                    }
                }).Start();
            }
        }

        /// <summary>
        /// MyTask class
        /// </summary>
        private class MyTask<TResult> : IMyTask<TResult>
        {
            private TPool pool;
            private object locker = new object();
            private Func<TResult> function;
            private TResult result;
            public bool IsCompleted { get; set; }

            public TResult Result
            {
                get
                {
                    return result;
                }
            }

            public MyTask(Func<TResult> task, TPool threadpool)
            {
                function = task;
                pool = threadpool;
            }

            public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult,TNewResult> func)
            {
                var task = new MyTask<TNewResult>(() => func(Result), pool);
                return task;
            }

            public Action Calculate()
            {
                result = function();
                IsCompleted = true;
                function = null;
                return result;
            }
        }

    }
}
