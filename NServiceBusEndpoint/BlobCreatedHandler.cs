using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;

namespace Microsoft.Storage
{
    public class BlobCreatedHandler : IHandleMessages<BlobCreated>
    {
        static ILog log = LogManager.GetLogger<BlobCreated>();

        public Task Handle(BlobCreated message, IMessageHandlerContext context)
        {
            log.Info($"URL: {message.Url}");
            log.Info($"API: {message.Api}");
            log.Info($"BlobType: {message.BlobType}");
            log.Info($"ContentType: {message.ContentType}");
            log.Info($"ContentLength: {message.ContentLength}");

            return Task.CompletedTask;
        }
    }
}