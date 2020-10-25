

namespace dn32.infra
{
    public static class DnRedisExtensao
    {
        public static DnConfiguracoesGlobais UsarRedis<Service>(this DnConfiguracoesGlobais configClass, string stringDeConexao) where Service : DnServicoDoRedis
        {
            configClass.StringDeConexaoDoRedis = stringDeConexao;
            configClass.TipoDeServicoDoRedis = typeof(Service);
            return configClass;
        }
    }
}