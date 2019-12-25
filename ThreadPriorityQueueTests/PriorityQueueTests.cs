using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace ThreadPriorityQueue.Tests
{
    [TestClass()]
    public class PriorityQueueTests
    {
        private PriorityQueue queue = new PriorityQueue();

        [TestMethod()]
        public void SizeTest()
        {
            Assert.IsTrue(queue.Count==0);
        }

        [TestMethod()]
        public void AddTest()
        {
            queue.Enqueue(5, 10);
            queue.Enqueue(5, 5);
            queue.Enqueue(6, 7);
            Assert.AreEqual(3, queue.Count);
        }

        [TestMethod()]
        public void DequeTest()
        {
            queue.Enqueue(5, 10);
            queue.Enqueue(5, 5);
            queue.Enqueue(6, 7);
            Assert.AreEqual(5, queue.Deque());
            Assert.AreEqual(6, queue.Deque());
            Assert.AreEqual(5, queue.Deque());
        }

        [TestMethod()]
        public void WorkingThread()
        {
            var thread1 = new Thread(() =>
            {
                queue.Deque();
                queue.Enqueue(3, 11);
                queue.Enqueue(2, 22);
            });
            var thread2 = new Thread(() =>
            {
                queue.Enqueue(13, 3);
                queue.Deque();
                queue.Enqueue(332, 222);
            });
            var thread3 = new Thread(() =>
            {
                queue.Deque();
                queue.Enqueue(333, 11);
                queue.Enqueue(2333, 2);
            });

            thread1.Start();
            thread2.Start();
            thread3.Start();

            thread1.Join();
            thread2.Join();
            thread3.Join();

            Assert.IsTrue(queue.Count > 0);
        }
    }
}