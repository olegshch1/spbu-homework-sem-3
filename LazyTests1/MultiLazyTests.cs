using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace Lazy.Tests
{
    [TestClass()]
    public class MultiLazyTests
    {
        private ILazy<int> lazy;
        private Thread[] threads;

        [TestMethod()]
        public void GetTest()
        {
            int number = 1;
            lazy = LazyFactory<int>.CreateMultiLazy(() => ++number);
            Assert.AreEqual(2, lazy.Get());
        }

        [TestMethod()]
        public void GetOnlyOnce()
        {
            int number = 1;
            lazy = LazyFactory<int>.CreateMultiLazy(() => ++number);
            Assert.AreEqual(2, lazy.Get());
            Assert.AreEqual(2, lazy.Get());
        }

        [TestMethod()]
        public void GetInManyThreads()
        {
            int number = 1;
            lazy = LazyFactory<int>.CreateMultiLazy(() => ++number);
            threads = new Thread[10];
            for(int i = 0; i < 10; i++)
            {
                threads[i] = new Thread(()=> 
                {
                    Assert.AreEqual(2, lazy.Get());
                });
            }
        }

        [TestMethod()]
        public void GetOnlyOnceInManyThreads()
        {
            int number = 1;
            lazy = LazyFactory<int>.CreateMultiLazy(() => ++number);
            threads = new Thread[10];
            for (int i = 0; i < 10; i++)
            {
                threads[i] = new Thread(() =>
                {
                    Assert.AreEqual(2, lazy.Get());
                    Assert.AreEqual(2, lazy.Get());
                    Assert.AreEqual(2, lazy.Get());
                });
            }
        }
    }
}