using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Features;
using AzureStorageQueueTransport = NServiceBus.AzureStorageQueueTransport;

class Program
{
    static async Task Main()
    {
        var endpointName = "queue";
        var endpointConfiguration = new EndpointConfiguration(endpointName);

        Console.Title = endpointName;

        var transport = endpointConfiguration.UseTransport<AzureStorageQueueTransport>();
        transport.ConnectionString("<storage-connection-string>");

        endpointConfiguration.UsePersistence<LearningPersistence>();
        endpointConfiguration.DisableFeature<TimeoutManager>();
        endpointConfiguration.DisableFeature<MessageDrivenSubscriptions>();
        endpointConfiguration.UseSerialization<NewtonsoftSerializer>();
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.LimitMessageProcessingConcurrencyTo(1);

        transport.EnableSupportForEventGridEvents();

        var endpointInstance = await Endpoint.Start(endpointConfiguration)
            .ConfigureAwait(false);
        Console.WriteLine("Press any other key to exit");

        while (true)
        {
            var key = Console.ReadKey();
            Console.WriteLine();

            if (key.Key != ConsoleKey.Enter)
            {
                break;
            }
        }
        await endpointInstance.Stop()
            .ConfigureAwait(false);
    }
}

public class BlogPostPublished : IEvent
{
    public string ItemUri { get; set; }
}