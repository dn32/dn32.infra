using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace dn32.infra.Redis
{
    internal class DnRepositorioDoRedis
    {
        private DnRedisContext Context { get; set; }

        internal DnRepositorioDoRedis(string connectionString) =>
                 Context = new DnRedisContext(connectionString);

        internal async Task<T> GetValueAsync<T>(string key, bool renewTimeout = false) =>
                 await Context.GetObjectAsync<T>($"{key}", renewTimeout);

        internal async Task<bool> SetValueAsync(string key, object value, TimeSpan? timeOut = null) =>
                 await Context.SetObjectAsync(key, value, timeOut);

        internal async Task<bool> SetPrimitiveValueAsync(string key, RedisValue value, TimeSpan? timeOut = null) =>
                 await Context.SetPrimitiveValueAsync(key, value, timeOut);

        internal async Task InscreverAsync(string canal, Func<string, Task> callbackAsync) =>
                 await GetSubscriber().SubscribeAsync(canal, async (channel, message) => await callbackAsync(message));

        internal async Task Inscrever(string canal, Action<string> callback) =>
                       await GetSubscriber().SubscribeAsync(canal, (channel, message) => callback(message));

        internal async Task RemoverInscricao(string canal) =>
                 await GetSubscriber().UnsubscribeAsync(canal);

        internal async Task<long> Publicar(string canal, string mensagem) => await GetSubscriber().PublishAsync(canal, mensagem);

        internal async Task<bool> RenewTimeOutAsync(string key, object stringValue = null) => await Context.RenewTimeOut(key, stringValue);

        private ISubscriber GetSubscriber() => Context.Multiplexer.GetSubscriber();
    }
}
