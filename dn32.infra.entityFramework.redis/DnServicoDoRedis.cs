using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dn32.infra
{
    public class DnServicoDoRedis : DnServicoTransacional
    {
        protected DnRepositorioDoRedis RepositorioDoRedis { get; set; }

        public DnServicoDoRedis() => RepositorioDoRedis = ObterRepositorio(Setup.ConfiguracoesGlobais.StringDeConexaoDoRedis);

        protected virtual DnRepositorioDoRedis ObterRepositorio(string connectionString) => new DnRepositorioDoRedis(connectionString);

        public virtual async Task InscreverAsync(string canal, Func<string, Task> callbackAsync) => await RepositorioDoRedis.InscreverAsync(canal, callbackAsync);

        public virtual async Task Inscrever(string canal, Action<string> callback) => await RepositorioDoRedis.Inscrever(canal, callback);

        public virtual async Task<List<T>> ListarPorPrefixo<T>(string pattern) => await RepositorioDoRedis.ListarPorPrefixo<T>(pattern);

        public virtual async Task RemoverInscricao(string canal) => await RepositorioDoRedis.RemoverInscricao(canal);

        public virtual async Task Publicar(string canal, string mensagem) => await RepositorioDoRedis.Publicar(canal, mensagem);

        public virtual async Task<T> ObterValorAsync<T>(string key) => await RepositorioDoRedis.GetValueAsync<T>(key);

        public virtual async Task<T> ObterEntidadeAsync<T>(DnEntidade entity) => await RepositorioDoRedis.GetValueAsync<T>(entity.GetHashCode().ToString());

        public virtual async Task<bool> SalvarEntidadeAsync(DnEntidade entity, TimeSpan? timeOut = null) => await RepositorioDoRedis.SetValueAsync(entity.GetHashCode().ToString(), entity, timeOut);

        public virtual async Task<bool> SalvarValorAsync(string key, object value, TimeSpan? timeOut = null) => await RepositorioDoRedis.SetValueAsync(key, value, timeOut);

        public virtual async Task<bool> SalvarValorPrimitivoAsync(string key, RedisValue value, TimeSpan? timeOut = null) => await RepositorioDoRedis.SetPrimitiveValueAsync(key, value, timeOut);

        public virtual async Task<bool> SalvarValorBoolAsync(string key, bool value, TimeSpan? timeOut = null) => await RepositorioDoRedis.SetPrimitiveValueAsync(key, value, timeOut);

        public virtual async Task<bool> ReiniviarTimeOutAsync(string key, object stringValue = null) => await RepositorioDoRedis.RenewTimeOutAsync(key, stringValue);
    }
}