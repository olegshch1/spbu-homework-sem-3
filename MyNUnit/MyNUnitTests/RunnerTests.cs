using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyNUnit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testing1;

namespace MyNUnit.Tests
{
    [TestClass()]
    public class RunnerTests
    {
        [TestMethod()]
        public void RunTest()
        {
            TestingClass1.count = 4;
            MyNUnit.Runner.Run("..\\..\\..\\Testing1");
            Assert.AreEqual(5, TestingClass1.count);
        }

        [TestMethod()]
        public void IgnoreAnnotationsMessageTest()
        {
            MyNUnit.Runner.Run("..\\..\\..\\Testing2");
            Assert.AreEqual("ignore", MyNUnit.Runner.TestInformation.Take().Ignore);
        }
    }
}