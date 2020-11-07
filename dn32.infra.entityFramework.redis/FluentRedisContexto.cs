using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dn32.infra
{
    public class DnRedisContexto
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

        public DnRedisContexto(string stringDeConexao, string senha)
        {
            ObterConfiguracoes(stringDeConexao, senha);
        }

        protected virtual void ObterConfiguracoes(string connectionString, string senha)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new InvalidOperationException("Uma string de conexão deve ser informada para o REDIS");

            ConfigurationOptions = new ConfigurationOptions
            {
                AbortOnConnectFail = false,
                SyncTimeout = int.MaxValue,
                ConnectTimeout = 3000,
                Password = senha,
                EndPoints = { connectionString },
                AllowAdmin = true
            };
        }

        public virtual async Task<List<T>> ListarPorPrefixo<T>(string padrao)
        {
            var keys = Server.KeysAsync(pattern: padrao);
            var elementos = new List<T>();
            await foreach (var key in keys)
            {
                var json = await Db.StringGetAsync(key);
                if (!json.HasValue) continue;
                var obj = JsonConvert.DeserializeObject<T>(json);
                elementos.Add(obj);
            }

            return elementos;
        }

        public virtual async Task<bool> SalvarObjetoAsync(string chave, object value, TimeSpan? timeOut = null)
        {
            return await Db.StringSetAsync($"{chave}:valor", JsonConvert.SerializeObject(value), timeOut);
        }

        public virtual async Task<bool> SalvarPrimitivoAsync(string chave, RedisValue valorRedis, TimeSpan? tempoDeExpiracao = null)
        {
            return await Db.StringSetAsync($"{chave}:valor", valorRedis, tempoDeExpiracao);
        }

        public virtual async Task<T> ObterPrimitivoAsync<T>(string chave, bool renovarTimeOut = false)
        {
            var stringValue = await Db.StringGetAsync($"{chave}:valor");
            if (string.IsNullOrEmpty(stringValue)) return default;
            if (renovarTimeOut) { await RenovarTimeOutAsync(chave, stringValue); }
            var newValue = Convert.ChangeType(stringValue, typeof(T));
            return newValue.DnCast<T>();
        }

        public virtual async Task<T> ObterObjetoAsync<T>(string chave, bool renovarTimeOut = false)
        {
            var stringValue = await Db.StringGetAsync($"{chave}:valor");
            if (string.IsNullOrEmpty(stringValue)) { return default; }
            if (renovarTimeOut) { await RenovarTimeOutAsync(chave, stringValue); }
            return JsonConvert.DeserializeObject<T>(stringValue);
        }

        public virtual async Task<bool> RenovarTimeOutAsync(string chave, object valorString = null)
        {
            var redisValueTime = await Db.StringGetAsync($"{chave}:time");
            valorString ??= await Db.StringGetAsync($"{chave}:valor");
            var time = redisValueTime.ToString();
            if (!string.IsNullOrEmpty(time))
            {
                var redisValue = valorString.DnCast<RedisValue>();
                return await Db.StringSetAsync($"{chave}:valor", redisValue, TimeSpan.FromMinutes(Convert.ToDouble(time)));
            }

            return false;
        }
    }
}