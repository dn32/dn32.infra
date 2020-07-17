using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using dn32.infra;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace dn32.infra
{
    public class DnRedisContext
    {
        protected ConnectionMultiplexer _multiplexer { get; set; }

        protected ConfigurationOptions ConfigurationOptions { get; set; }

        protected IDatabase Db => Multiplexer.GetDatabase();

        public virtual IServer Server => Multiplexer.GetServer(ConfigurationOptions.EndPoints[0]);

        public virtual ConnectionMultiplexer Multiplexer
        {
            get
            {
                if (_multiplexer == null || !_multiplexer.IsConnected)
                {
                    _multiplexer = ConnectionMultiplexer.Connect(ConfigurationOptions);
                }

                return _multiplexer;
            }
        }

        public DnRedisContext(string connectionString)
        {
            ObterConfiguracoes(connectionString);
        }

        protected virtual void ObterConfiguracoes(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new InvalidOperationException("Uma string de conexão deve ser informada para o REDIS");

            ConfigurationOptions = new ConfigurationOptions
            {
                AbortOnConnectFail = false,
                SyncTimeout = int.MaxValue,
                ConnectTimeout = 3000,
                EndPoints = { connectionString },
                AllowAdmin = true
            };
        }

        public virtual async Task<List<T>> ListarPorPrefixo<T>(string pattern)
        {
            var keys = Server.KeysAsync(pattern: pattern);
            var servidores = new List<T>();
            await
            foreach (var key in keys)
            {
                var json = await Db.StringGetAsync(key);
                servidores.Add(JsonConvert.DeserializeObject<T>(json));
            }

            return servidores;
        }

        public virtual async Task<bool> SetObjectAsync(string key, object value, TimeSpan? expireTime = null)
        {
            return await Db.StringSetAsync($"{key}:valor", JsonConvert.SerializeObject(value), expireTime);
        }

        public virtual async Task<bool> SetPrimitiveValueAsync(string key, RedisValue redisValue, TimeSpan? expireTime = null)
        {
            return await Db.StringSetAsync($"{key}:valor", redisValue, expireTime);
        }

        public virtual async Task<T> GetPrimitiveAsync<T>(string key, bool renewTimeout = false)
        {
            var stringValue = await Db.StringGetAsync($"{key}:valor");
            if (string.IsNullOrEmpty(stringValue)) return default;
            if (renewTimeout) { await RenewTimeOut(key, stringValue); }
            var newValue = Convert.ChangeType(stringValue, typeof(T));
            return newValue.DnCast<T>();
        }

        public virtual async Task<T> GetObjectAsync<T>(string key, bool renewTimeout = false)
        {
            var stringValue = await Db.StringGetAsync($"{key}:valor");
            if (string.IsNullOrEmpty(stringValue)) { return default; }
            if (renewTimeout) { await RenewTimeOut(key, stringValue); }
            return JsonConvert.DeserializeObject<T>(stringValue);
        }

        //public virtual async Task<T> ListObjectsAsync<T>(string key)
        //{

        //    IServer server = _multiplexer.GetServer("redisdb:6379");
        //    IEnumerable<RedisKey> list = server.Keys(pattern: "XYZ.*", pageOffset: 0, pageSize: 1000);

        //    // return JsonConvert.DeserializeObject<T>(list);
        //}

        public virtual async Task<bool> RenewTimeOut(string key, object stringValue = null)
        {
            var redisValueTime = await Db.StringGetAsync($"{key}:time");
            stringValue ??= await Db.StringGetAsync($"{key}:valor");
            var time = redisValueTime.ToString();
            if (!string.IsNullOrEmpty(time))
            {
                var redisValue = stringValue.DnCast<RedisValue>();
                return await Db.StringSetAsync($"{key}:valor", redisValue, TimeSpan.FromMinutes(Convert.ToDouble(time)));
            }

            return false;
        }
    }
}