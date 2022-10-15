using System;
using Testing1;
using Testing4;
using Testing5;
using Xunit;

namespace My_NUnitTests
{
    public class RunnerTests
    {
        [Fact]
        public void RunTest()
        {
            TestingClass1.count = 4;
            My_NUnit.Runner.Run("..\\..\\..\\..\\Testing1");
            Assert.Equal(5, TestingClass1.count);
        }

        [Fact]
        public void IgnoreAnnotationsMessageTest()
        {
            My_NUnit.Runner.Run("..\\..\\..\\..\\Testing2");
            Assert.Equal("ignore", My_NUnit.Runner.TestInformation.Take().Ignore);
        }

        [Fact]
        public void FailedDueToExceptionTest()
        {
            My_NUnit.Runner.Run("..\\..\\..\\..\\Testing3");
            Assert.False(My_NUnit.Runner.TestInformation.Take().IsPassed);
        }

        [Fact]
        public void SimpleWithAttributesBeforeAndAfterTest()
        {
            TestingClass4.array = new bool[] { false, false, false, false };
            My_NUnit.Runner.Run("..\\..\\..\\..\\Testing4");
            Assert.True(TestingClass4.array[0]);
            Assert.True(TestingClass4.array[1]);
            Assert.False(TestingClass4.array[2]);
            Assert.True(TestingClass4.array[3]);
        }

        [Fact]
        public void MenhodWithAttributesBeforeClassAndAfterClassTest()
        {
            TestingClass5.array = new bool[] { false, false };
            My_NUnit.Runner.Run("..\\..\\..\\..\\Testing5");
            Assert.True(TestingClass5.array[0]);
            Assert.True(TestingClass5.array[1]);
        }
    }
}
