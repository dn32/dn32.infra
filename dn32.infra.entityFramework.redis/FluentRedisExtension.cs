using dn32.infra;

namespace dn32.infra
{
    public static class DnRedisExtension
    {
        public static DnConfiguracoesGlobais UseRedis<Service>(this DnConfiguracoesGlobais configClass, string redisConnectionString) where Service : DnServicoDoRedis
        {
            configClass.StringDeConexaoDoRedis = redisConnectionString;
            configClass.TipoDeServicoDoRedis = typeof(Service);
            return configClass;
        }
    }
}