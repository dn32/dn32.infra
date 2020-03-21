using dn32.infra.servicos;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;
using dn32.infra.dados;
using dn32.infra.nucleo.configuracoes;

namespace dn32.infra.Redis
{
    public class DnRedisService : DnServicoTransacionalBase
    {
        private DnRedisRepository RedisRepository { get; set; }

        public DnRedisService()
        {
            RedisRepository = new DnRedisRepository(Setup.ConfiguracoesGlobais.StringDeConexaoDoRedis);
        }

        public async Task<T> ObterValorAsync<T>(string key) => await RedisRepository.GetValueAsync<T>(key);

        public async Task<T> ObterEntidadeAsync<T>(DnEntidade entity) => await RedisRepository.GetValueAsync<T>(entity.GetHashCode().ToString());

        public async Task<bool> SalvarEntidadeAsync(DnEntidade entity, TimeSpan? timeOut = null) => await RedisRepository.SetValueAsync(entity.GetHashCode().ToString(), entity, timeOut);

        public async Task<bool> SalvarValorAsync(string key, object value, TimeSpan? timeOut = null) => await RedisRepository.SetValueAsync(key, value, timeOut);

        public async Task<bool> SalvarValorPrimitivoAsync(string key, RedisValue value, TimeSpan? timeOut = null) => await RedisRepository.SetPrimitiveValueAsync(key, value, timeOut);

        public async Task<bool> ReiniviarTimeOutAsync(string key, object stringValue = null) => await RedisRepository.RenewTimeOutAsync(key, stringValue);
    }
}
