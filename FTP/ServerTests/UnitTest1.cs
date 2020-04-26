using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleApp1;
namespace ServerTests
{
    [TestClass]
    public class UnitTest1
    {
        private Server server;
        private Client client;

        [TestInitialize]
        public void Initialize()
        {
            server = new Server(1234);
            server.Start();
            client = new Client("localhost", 1234);
            client.Connect();
        }
        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
