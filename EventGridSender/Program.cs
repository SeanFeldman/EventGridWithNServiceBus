using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.EventGrid;
using Microsoft.Azure.EventGrid.Models;

class Program
{
    static async Task Main()
    {
        var topicEndpoint = "https://events.westus2-1.eventgrid.azure.net/api/events";
        var topicKey = "<topic-key>";
        var topicHostname = new Uri(topicEndpoint).Host;

        var topicCredentials = new TopicCredentials(topicKey);
        var client = new EventGridClient(topicCredentials);

        await client.PublishEventsAsync(topicHostname, GetEventsList());

        Console.Write("Published events to Event Grid.");
    }

    static IList<EventGridEvent> GetEventsList()
    {
        var eventsList = new List<EventGridEvent>();

        foreach (var post in posts)
        {
            eventsList.Add(new EventGridEvent
            {
                Id = Guid.NewGuid().ToString(),
                Data = new BlogPostPublished
                {
                    ItemUri = post
                },
                EventType = nameof(BlogPostPublished),
                EventTime = DateTime.Now,
                Subject = "Processing Azure Event Grid events with NServiceBus",
                DataVersion = "1.0"
            });
        }

        return eventsList;
    }

    class BlogPostPublished
    {
        public string ItemUri;
    }

    static readonly List<string> posts = new List<string>
    {
        "https://weblogs.asp.net/sfeldman/eventgrid-events-with-nservicebus",
        "https://weblogs.asp.net/sfeldman/what-you-pay-is-what-you-get",
        "https://weblogs.asp.net/sfeldman/asb-subs-with-correlation-filters"
    };
}