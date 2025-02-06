using FalcaPOS.Common.Cache.Events;
using System;
using System.Collections.ObjectModel;

namespace FalcaPOS.Common.Cache
{
    public class RequestCacheModel
    {
        public CacheKeyEnum Id { get; set; }
        public ObservableCollection<String> Payload { get; set; }
    }
}
