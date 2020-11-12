

namespace dn32.infra
{
    public static class DnRedisExtensao
    {
        public static DnConfiguracoesGlobais UsarRedis<Service>(this DnConfiguracoesGlobais configClass, string stringDeConexao, string senha = "") where Service : DnServicoDoRedis
        {
            configClass.StringDeConexaoDoRedis = stringDeConexao;
            configClass.StringDeConexaoDoRedisSenha = senha;
            configClass.TipoDeServicoDoRedis = typeof(Service);
            return configClass;
        }
    }
}