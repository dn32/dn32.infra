namespace dn32.infra
{
    internal class MongoDbContextoFabrica
    {
        internal static MongoDbContexto Create(Conexao connection, SessaoDeRequisicaoDoUsuario userSessionRequest)
        {
            var connectionString = connection.ObterStringDeConexao(userSessionRequest);
            var createDatabaseIfNotExists = connection.CriarOBancoDeDadosCasoNaoExista;
            var contexto = new MongoDbContexto(connectionString)
            {
                UserSessionRequest = userSessionRequest
            };

            CreateDB(createDatabaseIfNotExists, contexto);

            return contexto;
        }

        private static void CreateDB(bool createDatabaseIfNotExists, MongoDbContexto contexto)
        {
            if (createDatabaseIfNotExists)
            {
            }
        }
    }
}
