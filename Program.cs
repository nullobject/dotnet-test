using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public interface IMessageWriter
{
    void Write(string message);
}

public class MessageWriter : IMessageWriter
{
    public void Write(string message)
    {
        Console.WriteLine($"MessageWriter.Write(message: \"{message}\")");
    }
}

public class Worker(IMessageWriter messageWriter) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            messageWriter.Write($"Worker running at: {DateTimeOffset.Now}");
            await Task.Delay(1_000, stoppingToken);
        }
    }
}

class App
{
    static void Main(string[]? args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddHostedService<Worker>();
        builder.Services.AddSingleton<IMessageWriter, MessageWriter>();

        using IHost host = builder.Build();

        host.Run();
    }
}
