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

        protected virtual DnRepositorioDoRedis ObterRepositorio(string stringDeConexao) => new DnRepositorioDoRedis(stringDeConexao);

        public virtual async Task InscreverAsync(string canal, Func<string, Task> callbackAsync) => await RepositorioDoRedis.InscreverAsync(canal, callbackAsync);

        public virtual async Task Inscrever(string canal, Action<string> callback) => await RepositorioDoRedis.Inscrever(canal, callback);

        public virtual async Task<List<T>> ListarPorPrefixo<T>(string padrao) => await RepositorioDoRedis.ListarPorPrefixo<T>(padrao);

        public virtual async Task RemoverInscricao(string canal) => await RepositorioDoRedis.RemoverInscricao(canal);

        public virtual async Task Publicar(string canal, string mensagem) => await RepositorioDoRedis.Publicar(canal, mensagem);

        public virtual async Task<T> ObterValorAsync<T>(string chave) => await RepositorioDoRedis.ObterObjetoAsync<T>(chave);

        public virtual async Task<T> ObterEntidadeAsync<T>(DnEntidade entidade) => await RepositorioDoRedis.ObterObjetoAsync<T>(entidade.GetHashCode().ToString());

        public virtual async Task<bool> SalvarEntidadeoAsync(DnEntidade entidade, TimeSpan? timeOut = null) => await RepositorioDoRedis.SalvarObjetoAsync(entidade.GetHashCode().ToString(), entidade, timeOut);

        public virtual async Task<bool> SalvarObjetoAsync(string chave, object value, TimeSpan? timeOut = null) => await RepositorioDoRedis.SalvarObjetoAsync(chave, value, timeOut);

        public virtual async Task<bool> SalvarPrimitivoAsync(string chave, RedisValue value, TimeSpan? timeOut = null) => await RepositorioDoRedis.SalvarPrimitivoAsync(chave, value, timeOut);

        public virtual async Task<bool> SalvarValorBoolAsync(string chave, bool value, TimeSpan? timeOut = null) => await RepositorioDoRedis.SalvarPrimitivoAsync(chave, value, timeOut);

        public virtual async Task<bool> RenovarTimeOutAsync(string chave, object stringValue = null) => await RepositorioDoRedis.RenovarTimeOutAsync(chave, stringValue);
    }
}