
using dn32.infra.nucleo.configuracoes;

namespace dn32.infra.Redis
{
    public static class DnRedisExtension
    {
        public static DnConfiguracoesGlobais UseRedis<Service>(this DnConfiguracoesGlobais configClass, string redisConnectionString) where Service : DnRedisService
        {
            configClass.StringDeConexaoDoRedis = redisConnectionString;
            configClass.TipoDeServicoDoRedis = typeof(Service);
            return configClass;
        }
    }
}
