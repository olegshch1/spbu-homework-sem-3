using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lazy.Tests
{
    [TestClass()]
    public class LazyTests
    {
        private ILazy<int> lazy;

        [TestMethod()]
        public void GetTest()
        {
            int number = 1;
            lazy = LazyFactory<int>.CreateLazy(() => ++number);
            Assert.AreEqual(2, lazy.Get());
        }

        [TestMethod()]
        public void GetOnlyOnce()
        {
            int number = 1;
            lazy = LazyFactory<int>.CreateLazy(() => ++number);
            Assert.AreEqual(2, lazy.Get());
            Assert.AreEqual(2, lazy.Get());
        }
    }
}