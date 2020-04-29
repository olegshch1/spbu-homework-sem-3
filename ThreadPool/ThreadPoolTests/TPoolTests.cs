using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThreadPool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadPool.Tests
{
    [TestClass()]
    public class TPoolTests
    {
        private TPool pool;

        [TestInitialize]
        public void Initialize()
        {
            pool = new TPool(10);
        }

        [TestMethod]
        public void ThreadNumber()
        {
            Assert.AreEqual(10, pool.ThreadNumber);
        }

        [TestMethod]
        public void CompletingTask()
        {
            var task = pool.Add(() => 2 * 2);
            Assert.AreEqual(4, task.Result);
            Assert.IsTrue(task.IsCompleted);

        }

        [TestMethod]
        public void OneThreadSeveralTasksTest()
        {
            var pool = new TPool(1);
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
            var pool = new TPool(4);
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
        }
    }
}