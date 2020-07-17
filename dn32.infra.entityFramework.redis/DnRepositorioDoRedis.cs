using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dn32.infra
{
    public class DnRepositorioDoRedis
    {
        protected DnRedisContext Context { get; set; }

        public DnRepositorioDoRedis(string connectionString) => Context ??= ObterContexto(connectionString);

        protected virtual DnRedisContext ObterContexto(string connectionString) => new DnRedisContext(connectionString);

        public virtual async Task<T> GetValueAsync<T>(string key, bool renewTimeout = false) =>
                 await Context.GetObjectAsync<T>($"{key}", renewTimeout);

        public async Task<List<T>> ListarPorPrefixo<T>(string pattern) => await Context.ListarPorPrefixo<T>(pattern);

        public virtual async Task<bool> SetValueAsync(string key, object value, TimeSpan? timeOut = null) =>
                 await Context.SetObjectAsync(key, value, timeOut);

        public virtual async Task<bool> SetPrimitiveValueAsync(string key, RedisValue value, TimeSpan? timeOut = null) =>
                 await Context.SetPrimitiveValueAsync(key, value, timeOut);

        public virtual async Task InscreverAsync(string canal, Func<string, Task> callbackAsync) =>
                 await GetSubscriber().SubscribeAsync(canal, async (channel, message) => await callbackAsync(message));

        public virtual async Task Inscrever(string canal, Action<string> callback) =>
                       await GetSubscriber().SubscribeAsync(canal, (channel, message) => callback(message));

        public virtual async Task RemoverInscricao(string canal) =>
                 await GetSubscriber().UnsubscribeAsync(canal);

        public virtual async Task<long> Publicar(string canal, string mensagem) => await GetSubscriber().PublishAsync(canal, mensagem);

        public virtual async Task<bool> RenewTimeOutAsync(string key, object stringValue = null) => await Context.RenewTimeOut(key, stringValue);

        protected virtual ISubscriber GetSubscriber() => Context.Multiplexer.GetSubscriber();
    }
}
