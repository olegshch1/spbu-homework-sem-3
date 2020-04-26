using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyFTP;
using System.Threading.Tasks;

namespace ServerTests
{
    [TestClass]
    public class UnitTest1
    {
        
        private Server server;
        private Client client;
        private string path="../../../../ServerTests/TestDir";

        [TestInitialize]
        public void Initialize()
        {
            
            server = new Server(1234);
            server.Start();
            client = new Client("localhost", 1234);
            client.Connect();
        }
        [TestMethod]
        public async Task GetCommand()
        {
            var (size, data) = await client.Get(path + "/example.txt");
            Assert.AreEqual(24, data);
            server.Stop();
            client.Dispose();
        }

        [TestMethod]
        public async Task ListCommandTestAsync()
        {
            var answer = await client.List(path);
            Assert.AreEqual("2 example.txt False ", answer);
            server.Stop();
            client.Dispose();
        }

    }
}
