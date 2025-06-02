/*************************************************************************
 *                                                                       *
 *  File:        KeyloggerTests.cs                                       *
 *  Copyright:   (c) 2025, Glavan Pavel, Albu Sorin, Begu Alexandru,     *
 *               Cojocaru Valentin                                       *
 *  Website:     https://github.com/GlavanPavel/Keylogger                *
 *  Description: Contains unit tests for the KeyloggerClient and related *
 *               components, verifying functionality such as server      *
 *               discovery and client operations.                        *
 *                                                                       *
 *  This code and information is provided "as is" without warranty of    *
 *  any kind, either expressed or implied, including but not limited     *
 *  to the implied warranties of merchantability or fitness for a        *
 *  particular purpose. You are free to use this source code in your     *
 *  applications as long as the original copyright notice is included.   *
 *                                                                       *
 *************************************************************************/

using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServerCore;
using System.Windows.Forms;
using System.Net.Sockets;

namespace UnitTestKeyLogger
{
    /// <summary>
    /// Contains unit tests for verifying the behavior of the KeyloggerClient components.
    /// </summary>
    [TestClass]
    public class KeyloggerClientTests
    {
        [TestMethod]
        [Timeout(5000)]
        public async Task ServerDiscoveryNotDetecting()
        {
            var cts = new CancellationTokenSource();
            try
            {
                String val = await KeyloggerClient.ServerDiscovery.DiscoverServerAsync();

                Console.WriteLine(val);

                Assert.IsTrue(val == null, "Server inexistent a fost detectat");

            }
            finally
            {
                cts.Cancel();
            }
        }

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
            var cts = new CancellationTokenSource();
            KeyloggerClient.KeyloggerClient client = new KeyloggerClient.KeyloggerClient();

            var clientTask = client.StartAsync(cts.Token);

            //Console.WriteLine("Client started", client.IsConnected);

            Assert.IsFalse(client.IsConnected, "Client-ul s-a conectat la un server necunoscut");
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
            client.CaptureKeys('~');
            client.CaptureKeys('`');
            client.CaptureKeys('+');

            Assert.AreEqual("汉~`+", client.getKeyBuffer());
        }


    }

    /// <summary>
    /// Contains unit tests for verifying the behavior of the ServerCore components.
    /// </summary>
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
        public async Task TestServerClientsDetectionConnection()
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
                Assert.IsTrue(_server._clients.Count>0, "Clientul nu a fost detectat");

            }
            finally
            {
                cts.Cancel();
            }
        }

        [TestMethod]
        [Timeout(5000)]
        public async Task TestServerClientsDetectionDisconection()
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

                _client.Stop();

                await Task.Delay(1000);
                Console.WriteLine(_server._clients.Count);

                Assert.IsTrue(_server._clients.Count == 0, "Clientul a ramas detectat");
            }
            finally
            {
                cts.Cancel();
            }
        }
    }

    /// <summary>
    /// Contains unit tests for verifying the behavior of the ClientObserverUI components.
    /// </summary>
    [TestClass]
    public class ObserverTests
    {

        [TestMethod]
        [Timeout(5000)]
        public async Task StartObserverWithoutServer()
        {
            var cts = new CancellationTokenSource();

            Console.WriteLine("Starting client...");
            TcpClient _client = new TcpClient("127.0.0.1", 5000);
            var observer = new ObserverClient(_client);

            Assert.IsNotNull(observer, "ups, oberver is null");
        }
        [TestMethod]
        [Timeout(5000)]
        public async Task StartObserver()
        {
            var cts = new CancellationTokenSource();

            //Pornire server
            KeyloggerServer _server = new KeyloggerServer();
            Console.WriteLine("Starting server...");
            var serverTask = _server.RunAsync(cts.Token);

            Console.WriteLine("Starting client...");
            TcpClient _client = new TcpClient("127.0.0.1", 5000);
            var observer = new ObserverClient(_client);

            Assert.IsNotNull(observer);
        }
        [TestMethod]
        [Timeout(5000)]
        public async Task TestObserverUpdate()
        {
            var cts = new CancellationTokenSource();
            KeyloggerServer _server = new KeyloggerServer();
            Console.WriteLine("Starting server...");
            var serverTask = _server.RunAsync(cts.Token);

            TcpClient _client = new TcpClient("127.0.0.1", 5000);
            var observer = new ObserverClient(_client);

            Assert.IsTrue(observer.Update("test message"));
        }
    }
}
