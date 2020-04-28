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
    }
}