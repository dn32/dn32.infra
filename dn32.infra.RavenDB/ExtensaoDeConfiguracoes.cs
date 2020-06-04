using dn32.infra.dados;
using dn32.infra.nucleo.configuracoes;
using dn32.infra.nucleo.repositorios;

namespace dn32.infra.RavenDB
{
    public static class ExtensaoDeConfiguracoes
    {
        public static DnConfiguracoesGlobais UsarRavenDB(this DnConfiguracoesGlobais configuracoes, string nomeDoBD,string enderecoDoCertificado)
        {
            configuracoes.Valores.Add("nomeDoBD", nomeDoBD);
            configuracoes.Valores.Add("enderecoDoCertificado", enderecoDoCertificado);

            return configuracoes.DefinirFabricaDeRepositorio(new FabricaDeRepositorioDoRavenDB());
        }
    }
}
