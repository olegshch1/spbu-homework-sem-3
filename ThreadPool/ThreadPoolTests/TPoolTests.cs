using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ThreadPool.Tests
{
    [TestClass()]
    public class TPoolTests
    {
        private TPool pool;

        [TestMethod]
        public void ThreadNumber()
        {
            pool = new TPool(10);
            Assert.AreEqual(10, pool.ThreadNumber);
        }

        [TestMethod]
        public void CompletingTask()
        {
            pool = new TPool(1);
            var task = pool.Add(() => 2 * 2);
            Assert.AreEqual(4, task.Result);
            Assert.IsTrue(task.IsCompleted);

        }

        [TestMethod]
        public void OneThreadSeveralTasksTest()
        {
            pool = new TPool(1);
            var task1 = pool.Add(() => 2 + 3);
            var task2 = pool.Add(() => 8 * 8);
            var task3 = pool.Add(() => "123" + "456");

            Assert.AreEqual(5, task1.Result);
            Assert.AreEqual(64, task2.Result);
            Assert.AreEqual("123456", task3.Result);

            Assert.IsTrue(task1.IsCompleted);
            Assert.IsTrue(task2.IsCompleted);
            Assert.IsTrue(task3.IsCompleted);
        }

        [TestMethod]
        public void SeveralThreadsSeveralTasksTest()
        {
            pool = new TPool(4);
            var task1 = pool.Add(() => 2 + 3);
            var task2 = pool.Add(() => 8 * 8);
            var task3 = pool.Add(() => "123" + "456");
            pool.Shutdown();

            Assert.AreEqual(5, task1.Result);
            Assert.AreEqual(64, task2.Result);
            Assert.AreEqual("123456", task3.Result);

            Assert.IsTrue(task1.IsCompleted);
            Assert.IsTrue(task2.IsCompleted);
            Assert.IsTrue(task3.IsCompleted);
            Assert.IsTrue(pool.ClosedPool);
        }

        [TestMethod]
        public void TasksWhileShutdownTest()
        {
            pool = new TPool(2);
            var list = new List<IMyTask<int>>();
            for(int i = 0; i < 5; i++)
            {
                list.Add(pool.Add(() =>
                {
                    Thread.Sleep(1000);
                    return 4;
                }));
            }
            pool.Shutdown();
            Assert.AreEqual(4, list[4].Result);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TaskAfterShutdownTest()
        {
            pool = new TPool(1);
            pool.Shutdown();
            var task1 = pool.Add(() => 2 + 3);           
        }
    }
}