using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dn32.infra
{
    public class DnRepositorioDoRedis
    {
        protected DnRedisContexto Context { get; set; }

        public DnRepositorioDoRedis(string stringDeConexao, string senha) => Context ??= ObterContexto(stringDeConexao, senha);

        protected virtual DnRedisContexto ObterContexto(string stringDeConexao, string senha) => new DnRedisContexto(stringDeConexao, senha);

        public virtual async Task<T> ObterObjetoAsync<T>(string chave, bool renovarTimeOut = false) =>
                 await Context.ObterObjetoAsync<T>($"{chave}", renovarTimeOut);

        public async Task<List<T>> ListarPorPrefixo<T>(string padrao) => await Context.ListarPorPrefixo<T>(padrao);

        public virtual async Task<bool> SalvarObjetoAsync(string chave, object valor, TimeSpan? timeOut = null) =>
                 await Context.SalvarObjetoAsync(chave, valor, timeOut);

        public virtual async Task<bool> SalvarPrimitivoAsync(string chave, RedisValue valor, TimeSpan? timeOut = null) =>
                 await Context.SalvarPrimitivoAsync(chave, valor, timeOut);

        public virtual async Task InscreverAsync(string canal, Func<string, Task> callbackAsync) =>
                 await GetSubscriber().SubscribeAsync(canal, async (channel, message) => await callbackAsync(message));

        public virtual async Task Inscrever(string canal, Action<string> callback) =>
                       await GetSubscriber().SubscribeAsync(canal, (channel, message) => callback(message));

        public virtual async Task RemoverInscricao(string canal) =>
                 await GetSubscriber().UnsubscribeAsync(canal);

        public virtual async Task<long> Publicar(string canal, string mensagem) => await GetSubscriber().PublishAsync(canal, mensagem);

        public virtual async Task<bool> RenovarTimeOutAsync(string chave, object valorString = null) => await Context.RenovarTimeOutAsync(chave, valorString);

        protected virtual ISubscriber GetSubscriber() => Context.Multiplexer.GetSubscriber();
    }
}
