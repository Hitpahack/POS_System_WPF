using Prism.Events;

namespace FalcaPOS.Common.Cache.Events
{
    public class RequestCacheEvent : PubSubEvent<RequestCacheModel> { }
    public class ResponseCacheEvent : PubSubEvent<RequestCacheModel> { }

    public class RequestDictionaryCacheEvent : PubSubEvent<RequestCacheModel> { }
    public class ResponseDictionaryCacheEvent : PubSubEvent<RequestCacheModel> { }


}
