using dn32.infra.servicos;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;
using dn32.infra.dados;
using dn32.infra.nucleo.configuracoes;

namespace dn32.infra.Redis
{
    public class DnServicoDoRedis : DnServicoTransacionalBase
    {
        private DnRepositorioDoRedis RepositorioDoRedis { get; set; }

        public DnServicoDoRedis()=>
            RepositorioDoRedis = new DnRepositorioDoRedis(Setup.ConfiguracoesGlobais.StringDeConexaoDoRedis);

        public async Task InscreverAsync(string canal, Func<string, Task> callbackAsync) => await RepositorioDoRedis.InscreverAsync(canal, callbackAsync);
      
        public async Task Inscrever(string canal, Action<string> callback) => await RepositorioDoRedis.Inscrever(canal, callback);

        public async Task RemoverInscricao(string canal) => await RepositorioDoRedis.RemoverInscricao(canal); 

        public async Task Publicar(string canal, string mensagem) => await RepositorioDoRedis.Publicar(canal, mensagem);

        public async Task<T> ObterValorAsync<T>(string key) => await RepositorioDoRedis.GetValueAsync<T>(key);

        public async Task<T> ObterEntidadeAsync<T>(DnEntidade entity) => await RepositorioDoRedis.GetValueAsync<T>(entity.GetHashCode().ToString());

        public async Task<bool> SalvarEntidadeAsync(DnEntidade entity, TimeSpan? timeOut = null) => await RepositorioDoRedis.SetValueAsync(entity.GetHashCode().ToString(), entity, timeOut);

        public async Task<bool> SalvarValorAsync(string key, object value, TimeSpan? timeOut = null) => await RepositorioDoRedis.SetValueAsync(key, value, timeOut);

        public async Task<bool> SalvarValorPrimitivoAsync(string key, RedisValue value, TimeSpan? timeOut = null) => await RepositorioDoRedis.SetPrimitiveValueAsync(key, value, timeOut);
     
        public async Task<bool> SalvarValorBoolAsync(string key, bool value, TimeSpan? timeOut = null) => await RepositorioDoRedis.SetPrimitiveValueAsync(key, value, timeOut);

        public async Task<bool> ReiniviarTimeOutAsync(string key, object stringValue = null) => await RepositorioDoRedis.RenewTimeOutAsync(key, stringValue);
    }
}
