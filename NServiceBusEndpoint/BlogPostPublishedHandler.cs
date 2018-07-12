using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

public class BlogPostPublishedHandler : IHandleMessages<BlogPostPublished>
{
    static ILog log = LogManager.GetLogger<BlogPostPublishedHandler>();

    public Task Handle(BlogPostPublished message, IMessageHandlerContext context)
    {
        log.Info($"Received {nameof(BlogPostPublished)}: {message.ItemUri}");
        log.Info($"Topic: {context.MessageHeaders["EventGrid.topic"]}");
        log.Info($"Subject: {context.MessageHeaders["EventGrid.subject"]}");
        log.Info($"Event time: {context.MessageHeaders["EventGrid.eventTime"]}");

        return Task.CompletedTask;
    }
}