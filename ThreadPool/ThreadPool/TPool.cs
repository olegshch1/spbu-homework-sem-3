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
                new Thread(()=> { }).Start();
            }
        }

    }
}
