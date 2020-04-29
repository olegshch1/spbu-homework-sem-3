using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace ThreadPool
{
    /// <summary>
    /// MyThreadPool class
    /// </summary>
    public class TPool
    {
        public int ThreadNumber { get; }
        private BlockingCollection<Action> taskQueue = new BlockingCollection<Action>();
        private CancellationTokenSource token = new CancellationTokenSource();
        private int finishedThreads = 0;
        private object locker = new object();

        public TPool(int numberThreads)
        {
            ThreadNumber = numberThreads;
            StartThread();
        }

        /// <summary>
        /// Stopping threadpool work
        /// </summary>
        public void Shutdown()
        {
            lock (locker)
            {
                token.Cancel();
                taskQueue?.CompleteAdding();
                taskQueue = null;
            }
        }

        /// <summary>
        /// true if all threads have been closed
        /// </summary>
        public bool ClosedPoll => ThreadNumber == finishedThreads;

        /// <summary>
        /// Adding task to ThreadPool queue
        /// </summary>
        public IMyTask<TResult> Add<TResult>(Func<TResult> func)
        {
            lock (locker)
            {
                if (token.Token.IsCancellationRequested)
                {
                    throw new InvalidOperationException();
                }
                try
                {
                    var task = new MyTask<TResult>(func, this);
                    taskQueue.Add(task.Calculate);
                    return task;
                }
                catch (Exception exception) when (exception is InvalidOperationException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        /// <summary>
        /// Starting with "ThreadNumber" number of threads
        /// </summary>
        private void StartThread()
        {
            for (int i = 0; i < ThreadNumber; i++)
            {
                new Thread(()=> 
                {
                    while (true)
                    {
                        if (token.IsCancellationRequested)
                        {
                            Interlocked.Increment(ref finishedThreads);
                        }
                        taskQueue?.Take().Invoke();
                    }
                }).Start();
            }
        }

        private Action ActionAdd(Action action)
        {
            taskQueue?.Add(action);
            return action;
        }

        /// <summary>
        /// MyTask class
        /// </summary>
        private class MyTask<TResult> : IMyTask<TResult>
        {
            private TPool pool;
            private ManualResetEvent flag = new ManualResetEvent(false);
            private object locker = new object();
            private Func<TResult> function;
            private Queue<Action> local;
            private TResult result;
            private AggregateException exception;
            public bool IsCompleted { get; set; }
            public TResult Result
            {
                get
                {
                    flag.WaitOne();
                    if (exception == null)
                    {
                        return result;
                    }
                    throw exception;
                }
            }
            

            public MyTask(Func<TResult> task, TPool threadpool)
            {
                function = task;
                pool = threadpool;
                local = new Queue<Action>();
            }

            public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult,TNewResult> func)
            {
                var task = new MyTask<TNewResult>(() => func(Result), pool);
                lock (locker)
                {
                    if (IsCompleted)
                    {
                        return pool.Add(() => func(Result));
                    }
                    local.Enqueue(task.Calculate);
                    return task;
                }
                
            }

            public void Calculate()
            {
                try
                {
                    result = function();
                }
                catch (Exception calcexception)
                {
                    exception = new AggregateException(calcexception);
                }
                finally
                {
                    lock (locker)
                    {
                        IsCompleted = true;
                        function = null;
                        flag.Set();
                        while (local.Count != 0)
                        {
                            pool.ActionAdd(local.Dequeue());
                        }
                    }
                }
            }
        }

    }
}
