using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServerCore;

namespace UnitTestKeyLogger
{
    [TestClass]
    public class KeyloggerIntegrationTests
    {
        private KeyloggerServer _server;
        private KeyloggerClient.KeyloggerClient _client;


        [TestMethod]
        [Timeout(5000)]
        public async Task StartClientWithServer()
        {
            var cts = new CancellationTokenSource();
            try
            {
                // Pornire server
                KeyloggerServer _server = new KeyloggerServer();
                Console.WriteLine("Starting server...");
                var serverTask = _server.RunAsync(cts.Token);

                // Pornire client
                KeyloggerClient.KeyloggerClient _client = new KeyloggerClient.KeyloggerClient();
                Console.WriteLine("Starting client...");
                var clientTask = _client.StartAsync(cts.Token);

                Console.WriteLine("Testing connection...");
                // Verificare conexiune
                await Task.Delay(1000);
                Assert.IsTrue(_client.IsConnected, "Clientul nu s-a conectat la server");

            }
            finally
            {
                cts.Cancel();
                _server?.StopAsync().Wait();
            }
        }

        [TestMethod]
        [Timeout(5000)]
        [ExpectedException(typeof(KeyloggerException))]
        public async Task StartClientWithoutServer()
        {
            KeyloggerClient.KeyloggerClient client = new KeyloggerClient.KeyloggerClient();
            await client.StartAsync();
        }

        [TestMethod]
        [Timeout(5000)]
        public void TestCaptureKeys()
        {
            KeyloggerClient.KeyloggerClient _client = new KeyloggerClient.KeyloggerClient();


            // Arrange
            var client = new KeyloggerClient.KeyloggerClient();

            client.CaptureKeys('T');
            client.CaptureKeys('E');
            client.CaptureKeys('S');
            client.CaptureKeys('T');

            // Assert
            Assert.AreEqual("TEST", client.getKeyBuffer());

        }

    }
}
