using StackExchange.Redis;
using System.Text.Json;

namespace MagicVilla_VillaAPI.Services
{
    public class CacheServices : ICacheService
    {
        private readonly IDatabase _cacheDB;
        private readonly IServer _server;

        public CacheServices()
        {
            //var connectionMultiplexer = ConnectionMultiplexer.Connect("localhost:6379");
            var connectionMultiplexer = ConnectionMultiplexer.Connect("redis-13843.c251.east-us-mz.azure.redns.redis-cloud.com:13843,password=InMDniAyogNT20Rhn1FMSnuiECURS7ud");
            _server = connectionMultiplexer.GetServer(connectionMultiplexer.GetEndPoints()[0]);
            _cacheDB = connectionMultiplexer.GetDatabase();
        }

        public T GetData<T>(string key)
        {
            var value = _cacheDB.StringGet(key);
            if (!string.IsNullOrEmpty(value)) return JsonSerializer.Deserialize<T>(value);
            else return default;
        }

        public object RemoveData(string key)
        {
            var _exist = _cacheDB.KeyExists(key);

            if (_exist) return _cacheDB.KeyDelete(key);
            else return false;
        }

        public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            var expirtyTime = expirationTime.DateTime.Subtract(DateTime.Now);
            return _cacheDB.StringSet(key, JsonSerializer.Serialize(value), expirtyTime);
        }
    }
}
