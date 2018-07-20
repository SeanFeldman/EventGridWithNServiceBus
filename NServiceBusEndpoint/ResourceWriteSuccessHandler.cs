//using System.Threading.Tasks;
//using Microsoft.Resources;
//using NServiceBus;
//using NServiceBus.Logging;
//
//namespace Microsoft.Storage
//{
//    public class ResourceWriteSuccessHandler : IHandleMessages<ResourceWriteSuccess>
//    {
//        static ILog log = LogManager.GetLogger<ResourceWriteSuccess>();
//
//        public Task Handle(ResourceWriteSuccess message, IMessageHandlerContext context)
//        {
//            log.Info($"TenantId: {message.TenantId}");
//            log.Info($"SubscriptionId: {message.SubscriptionId}");
//            log.Info($"ResourceUri: {message.ResourceUri}");
////            log.Info($"Authorization: {message.Authorization}");
//            log.Info($"Claims: {message.Claims}");
//            log.Info($"OperationName: {message.OperationName}");
//            log.Info($"Status: {message.Status}");
//            log.Info($"HttpRequest: {message.HttpRequest}");
//
//            return Task.CompletedTask;
//        }
//    }
//}