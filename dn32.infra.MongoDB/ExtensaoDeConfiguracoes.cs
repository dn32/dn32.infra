namespace dn32.infra
{
    public static class ExtensaoDeConfiguracoes
    {
        public static DnConfiguracoesGlobais UsarMongoDB(this DnConfiguracoesGlobais configuracoes, string nomeDoBD)
        {
            configuracoes.Valores.Add("mongoDBNomeDoBD", nomeDoBD);

            return configuracoes.DefinirFabricaDeRepositorio(new FabricaDeRepositorioDoMongoDB());
        }
    }
}
