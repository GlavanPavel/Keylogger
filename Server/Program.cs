using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class ClientHandler
{
    public TcpClient TcpClient { get; set; }
    public string Id { get; set; }
    public NetworkStream Stream => TcpClient.GetStream();
}

class Program
{
    private static readonly List<ClientHandler> _clients = new();
    private static readonly object _lock = new();
    private static TcpListener _listener;
    private static bool _running = true;
    private static readonly List<ClientHandler> _observers = new();

    static async Task Main(string[] args)
    {
        Console.WriteLine("Starting server on port 5000...");
        _listener = new TcpListener(IPAddress.Any, 5000);
        _listener.Start();

        _ = Task.Run(AcceptClientsLoop); // run accept loop in background

        Console.WriteLine("Server is running. Press Enter to stop...");
        Console.ReadLine();
        _running = false;
        _listener.Stop();
    }

    private static async Task AcceptClientsLoop()
    {
        while (_running)
        {
            try
            {
                TcpClient tcpClient = await _listener.AcceptTcpClientAsync();
                string clientId = ((IPEndPoint)tcpClient.Client.RemoteEndPoint).ToString();

                var handler = new ClientHandler
                {
                    TcpClient = tcpClient,
                    Id = clientId
                };

                var buffer = new byte[10];
                await tcpClient.GetStream().ReadAsync(buffer, 0, buffer.Length);
                string role = Encoding.UTF8.GetString(buffer).Trim('\0').Trim();
                Console.WriteLine(role);

                if (role == "observer")
                {
                    lock (_lock)
                        _observers.Add(handler);
                    Console.WriteLine("Observer client registered.");
                }
                else
                {
                    lock (_lock)
                        _clients.Add(handler);
                    Console.WriteLine("Standard client connected.");
                    _ = HandleClientAsync(handler);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Accept error: {ex.Message}");
                break;
            }
        }
    }

    private static async Task HandleClientAsync(ClientHandler handler)
    {
        var stream = handler.Stream;
        var buffer = new byte[1024];

        // Create a file for this client
        string safeFileName = handler.Id.Replace(":", "_"); // replace colon in IP:port
        string filePath = $"SaveData\\{safeFileName}.txt";

        try
        {
            using var writer = new StreamWriter(filePath, append: true); // open file for appending

            while (handler.TcpClient.Connected)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                    break; // client disconnected

                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"[{handler.Id}] {message}");

                await writer.WriteLineAsync(message);
                await writer.FlushAsync(); // write immediately

                // Notify all observers here:
                NotifyObservers($"[{handler.Id}] {message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error with {handler.Id}: {ex.Message}");
        }
        finally
        {
            Console.WriteLine($"Client disconnected: {handler.Id}");
            handler.TcpClient.Close();

            lock (_lock)
                _clients.Remove(handler);
        }
    }
    private static void NotifyObservers(string message)
    {
        lock (_lock)
        {
            foreach (var obs in _observers.ToList()) // clone to avoid concurrent modification
            {
                try
                {
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    obs.Stream.Write(data, 0, data.Length);
                }
                catch
                {
                    Console.WriteLine($"Observer {obs.Id} failed. Removing.");
                    _observers.Remove(obs);
                }
            }
        }
    }

    public void ProcessIncomingMessage(string message)
    {
        // Save, process or log the message here

        // Then notify observers (Type B clients)
        NotifyObservers(message);
    }

}
