using FalcaPOS.Common.Cache.Events;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace FalcaPOS.Common.Cache
{
    public class CacheHub
    {
        private readonly IEventAggregator _eventAggregator;

        public Dictionary<String, CacheCollections> _cacheItems = new Dictionary<String, CacheCollections>();

        public CacheHub(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            //_eventAggregator.GetEvent<RequestCacheEvent>().Subscribe( async(x) =>
            //{
            //    try
            //    {
            //        if (x != null)
            //        {
            //            switch (x.Id)
            //            {
            //                case CacheKeyEnum.States:

            //                    break;
            //                case CacheKeyEnum.Districts:
            //                    await ReadFileCache(x);
            //                    break;                        
            //                default:
            //                    break;
            //            }

            //        }

            //    }
            //    catch (System.Exception)
            //    {

            //        throw;
            //    }
            //});

            InitialLoad();

            _eventAggregator.GetEvent<ResponseDictionaryCacheEvent>().Subscribe((x) =>
            {
                if (x != null)
                {
                    if (_cacheItems.ContainsKey(x.Id.ToString()))
                    {
                        _eventAggregator.GetEvent<ResponseCacheEvent>().Publish(new RequestCacheModel()
                        {
                            Id = x.Id,
                            Payload = _cacheItems[x.Id.ToString()].Collections
                        });
                    }
                }
            });
        }

        public async void InitialLoad()
        {
            await ReadFileCache(new RequestCacheModel() { Id = CacheKeyEnum.States });
            await ReadFileCache(new RequestCacheModel() { Id = CacheKeyEnum.Districts });
        }

        private async Task ReadFileCache(RequestCacheModel x)
        {
            try
            {
                await Task<string>.Factory.StartNew(() =>
                {
                    if ((!_cacheItems.ContainsKey(x.Id.ToString())))
                    {
                        var path = Path.Combine(ApplicationSettings.CachePath + @"\" + ((int)(Enum.Parse(typeof(CacheKeyEnum), x.Id.ToString()))).ToString());
                        if (File.Exists(path))
                        {
                            var cacheData = File.ReadAllText(path);
                            if (cacheData != null)
                            {
                                return cacheData;
                            }
                        }
                    }
                    //else
                    //{
                    //    _eventAggregator.GetEvent<ResponseCacheEvent>().Publish(new RequestCacheModel()
                    //    {
                    //        Id = x.Id,
                    //        Payload = _cacheItems[x.Id.ToString()].Collections
                    //    });
                    //}
                    return String.Empty;

                }).ContinueWith((y) =>
                {
                    Task.Run(() =>
                    {
                        if (!_cacheItems.ContainsKey(x.Id.ToString()))
                        {
                            _cacheItems.Add(x.Id.ToString(), new CacheCollections(x.Id.ToString()) { Collections = new ObservableCollection<string>() { y.Result } });

                            //if (_cacheItems.ContainsKey(x.Id.ToString())){
                            //    _eventAggregator.GetEvent<ResponseCacheEvent>().Publish(new RequestCacheModel()
                            //    {
                            //        Id = x.Id,
                            //        Payload = _cacheItems[x.Id.ToString()].Collections
                            //    });
                            //}
                        }
                    });

                }, TaskContinuationOptions.RunContinuationsAsynchronously);
            }
            catch (Exception ex) { throw; }
        }
    }


    public class CacheCollections
    {
        private String Key { get; set; }
        public ObservableCollection<String> Collections { get; set; } = new ObservableCollection<string>();
        public CacheCollections(String key)
        {
            Key = key;
            Collections.CollectionChanged += Collections_CollectionChanged;
        }

        private void Collections_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {

            }
        }
    }






}
