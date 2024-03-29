﻿
namespace dn32.infra
{
    public static class ExtensaoDeConfiguracoes
    {
        public static DnConfiguracoesGlobais UsarRavenDB(this DnConfiguracoesGlobais configuracoes, string nomeDoBD, string enderecoDoCertificado)
        {
            configuracoes.Valores.Add("ravenDBNomeDoBD", nomeDoBD);
            configuracoes.Valores.Add("enderecoDoCertificado", enderecoDoCertificado);

            return configuracoes.DefinirFabricaDeRepositorio(new FabricaDeRepositorioDoRavenDB());
        }
    }
}