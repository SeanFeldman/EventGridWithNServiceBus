using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NServiceBus;
using NServiceBus.Azure.Transports.WindowsAzureStorageQueues;
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

        var jsonSerializer = new Newtonsoft.Json.JsonSerializer();

        transport.UnwrapMessagesWith(cloudQueueMessage =>
        {
            using (var stream = new MemoryStream(cloudQueueMessage.AsBytes))
            using (var streamReader = new StreamReader(stream))
            using (var textReader = new JsonTextReader(streamReader))
            {
                var jObject = JObject.Load(textReader);

                using (var jsonReader = jObject.CreateReader())
                {
                    //try deserialize to a NServiceBus envelope first
                    var wrapper = jsonSerializer.Deserialize<MessageWrapper>(jsonReader);

                    if (wrapper.MessageIntent != default)
                    {
                        //this was a envelope message
                        return wrapper;
                    }
                }

                //this was an EventGrid event
                using (var jsonReader = jObject.CreateReader())
                {
                    var @event = jsonSerializer.Deserialize<EventGridEvent>(jsonReader);

                    var wrapper = new MessageWrapper
                    {
                        Id = @event.Id,
                        Headers = new Dictionary<string, string>
                        {
                            { "NServiceBus.EnclosedMessageTypes", @event.EventType },
                            { "EventGrid.topic", @event.Topic },
                            { "EventGrid.subject", @event.Subject },
                            { "EventGrid.eventTime", @event.EventTime.ToString("u") },
                            { "EventGrid.dataVersion", @event.DataVersion },
                            { "EventGrid.metadataVersion", @event.MetadataVersion },
                        },
                        Body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event.Data)),
                        MessageIntent = MessageIntentEnum.Publish
                    };
                    return wrapper;
                }
            }
        });

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

public class EventGridEvent
{
    /// <summary>
    /// Gets or sets an unique identifier for the event.
    /// </summary>
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    /// <summary>
    /// Gets or sets the resource path of the event source.
    /// </summary>
    [JsonProperty(PropertyName = "topic")]
    public string Topic { get; set; }

    /// <summary>
    /// Gets or sets a resource path relative to the topic path.
    /// </summary>
    [JsonProperty(PropertyName = "subject")]
    public string Subject { get; set; }

    /// <summary>
    /// Gets or sets event data specific to the event type.
    /// </summary>
    [JsonProperty(PropertyName = "data")]
    public object Data { get; set; }

    /// <summary>
    /// Gets or sets the type of the event that occurred.
    /// </summary>
    [JsonProperty(PropertyName = "eventType")]
    public string EventType { get; set; }

    /// <summary>
    /// Gets or sets the time (in UTC) the event was generated.
    /// </summary>
    [JsonProperty(PropertyName = "eventTime")]
    public DateTime EventTime { get; set; }

    /// <summary>
    /// Gets the schema version of the event metadata.
    /// </summary>
    [JsonProperty(PropertyName = "metadataVersion")]
    public string MetadataVersion { get; private set; }

    /// <summary>
    /// Gets or sets the schema version of the data object.
    /// </summary>
    [JsonProperty(PropertyName = "dataVersion")]
    public string DataVersion { get; set; }
}

public class BlogPostPublished : IEvent
{
    public string ItemUri { get; set; }
}