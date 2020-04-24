using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ThreadPool
{
    public class TPool
    {
        public int ThreadNumber { get; }
        private BlockingCollection<Action> taskqueue = new BlockingCollection<Action>();

        public TPool(int numberthreads)
        {
            ThreadNumber = numberthreads;
            StartThread();
        }

        private Action Add(Action task)
        {
            taskqueue.Add(task);
            return task;
        }

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

        private class MyTask<TResult> : IMyTask<TResult>
        {
            private object locker = new object();
            private Func<TResult> function;
            public bool IsCompleted { get; set; }

            public TResult Result { get; }

            public MyTask(Func<TResult> task)
            {
                function = task;
            }

            public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult,TNewResult> func)
            {
                var task = new MyTask<TNewResult>(()=>func(Result));
                return task;
            }
        }

    }
}
