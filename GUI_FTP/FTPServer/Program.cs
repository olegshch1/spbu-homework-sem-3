using System.Threading.Tasks;

namespace FTPServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var server = new Server(1234);
            await server.Start();
        }
    }
}
