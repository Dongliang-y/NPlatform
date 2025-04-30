using StackExchange.Redis;

namespace NPlatform.Infrastructure.Redis
{
    public interface IRedisService
    {
        void ClearAll();
        ITransaction CreateTransaction();
        IDatabase GetDatabase();
        IServer GetServer(string hostAndPort);
        Task<bool> GlobalHashDeleteAsync(string key, string dataKey);
        Task<bool> GlobalHashExistsAsync(string key, string dataKey);
        Task<T> GlobalHashGetAsync<T>(string key, string dataKey);
        Task<bool> GlobalHashSetAsync<T>(string key, string dataKey, T t);
        Task<string> GlobalStringGetAsync(string key);
        Task<bool> GlobalStringSetAsync(string key, string value, TimeSpan? expiry = null);
        Task<double> HashDecrementAsync(string key, string dataKey, double val = 1);
        Task<long> HashDeleteAsync(string key, List<RedisValue> dataKeys);
        Task<bool> HashDeleteAsync(string key, string dataKey);
        Task<bool> HashExistsAsync(string key, string dataKey);
        Task<T> HashGeAsync<T>(string key, string dataKey);
        Task<double> HashIncrementAsync(string key, string dataKey, double val = 1);
        Task<List<T>> HashKeysAsync<T>(string key);
        Task<bool> HashSetAsync<T>(string key, string dataKey, T t);
        long KeyDelete(List<string> keys);
        bool KeyDelete(string key);
        long KeyDeletePattern(string pattern);
        bool KeyExists(string key);
        bool KeyExpire(string key, TimeSpan? expiry = null);
        bool KeyRename(string key, string newKey);
        Task<T> ListLeftPopAsync<T>(string key);
        Task<long> ListLeftPushAsync<T>(string key, T value);
        long ListLeftPushs<T>(string key, List<T> values);
        Task<long> ListLengthAsync(string key);
        Task<List<T>> ListRangeAsync<T>(string key);
        Task<long> ListRemoveAsync<T>(string key, T value);
        Task<T> ListRightPopAsync<T>(string key);
        Task<long> ListRightPushAsync<T>(string key, T value);
        bool Lock(string key, int seconds);
        Task<bool> LockAsync(string key, int seconds);
        Task<long> Publish<T>(string channel, T msg);
        bool SetAdd(string key, string obj);
        RedisValue[] SetMembers(string key);
        long SetRemove(string key, string[] obj);
        bool SetRemove<T>(string key, T obj);
        void SetSysCustomKey(string prefix);
        Task<bool> SortedSetAddAsync<T>(string key, T value, double score);
        Task<long> SortedSetLengthAsync(string key);
        Task<List<T>> SortedSetRangeByRankAsync<T>(string key);
        Task<bool> SortedSetRemoveAsync<T>(string key, T value);
        Task<double> StringDecrementAsync(string key, double val = 1);
        string StringGet(string key);
        object StringGet(string key, Type type);
        T StringGet<T>(string key);
        Task<RedisValue[]> StringGetAsync(List<string> listKey);
        Task<string> StringGetAsync(string key);
        Task<T> StringGetAsync<T>(string key);
        Task<double> StringIncrementAsync(string key, double val = 1);
        bool StringSet(string key, string value, TimeSpan? expiry = null);
        bool StringSet<T>(string key, T obj, TimeSpan? expiry = null);
        Task<bool> StringSetAsync(List<KeyValuePair<RedisKey, RedisValue>> keyValues);
        Task<bool> StringSetAsync(string key, string value, TimeSpan? expiry = null);
        Task<bool> StringSetAsync<T>(string key, T obj, TimeSpan? expiry = null);
        void Subscribe(string subChannel, Action<RedisChannel, RedisValue> handler = null);
        bool UnLock(string key);
        Task<bool> UnLockAsync(string key);
        void Unsubscribe(string channel);
        void UnsubscribeAll();
    }
}