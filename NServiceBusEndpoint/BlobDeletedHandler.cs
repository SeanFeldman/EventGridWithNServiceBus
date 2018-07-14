using System.Threading.Tasks;
using Microsoft.Storage;
using NServiceBus;
using NServiceBus.Logging;

public class BlobDeletedHandler : IHandleMessages<BlobDeleted>
{
    static ILog log = LogManager.GetLogger<BlogPostPublishedHandler>();

    public Task Handle(BlobDeleted message, IMessageHandlerContext context)
    {
        log.Info($"URL: {message.Url}");
        log.Info($"API: {message.Api}");
        log.Info($"BlobType: {message.BlobType}");
        log.Info($"ContentType: {message.ContentType}");

        return Task.CompletedTask;
    }
}