using dn32.infra.Nucleo.Models;
using dn32.infra.nucleo.configuracoes;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;
using Raven.Client.Documents;
using Raven.Client.Exceptions.Database;
using Raven.Client.Exceptions;
using Raven.Client.Documents.Operations;
using System;
using System.Threading.Tasks;

namespace dn32.infra.RavenDB
{
    internal class RavenDbContextoFabrica
    {
        internal static RavenDbContexto Create(Conexao connection, SessaoDeRequisicaoDoUsuario userSessionRequest)
        {
            var connectionString = connection.ObterStringDeConexao(userSessionRequest);
            var createDatabaseIfNotExists = connection.CriarOBancoDeDadosCasoNaoExista;
            var contexto = new RavenDbContexto(connectionString)
            {
                UserSessionRequest = userSessionRequest
            };

            CreateDB(createDatabaseIfNotExists, contexto);

            return contexto;
        }

        private static void CreateDB(bool createDatabaseIfNotExists, RavenDbContexto contexto)
        {
            if (createDatabaseIfNotExists)
            {
                 EnsureDatabaseExistsAsync(contexto.Store, contexto.NomeDoBD, true).Wait();
            }
        }

        public static async Task EnsureDatabaseExistsAsync(IDocumentStore store, string database = null, bool createDatabaseIfNotExists = true)
        {
            database = database ?? store.Database;

            if (string.IsNullOrWhiteSpace(database))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(database));

            try
            {
                await store.Maintenance.ForDatabase(database).SendAsync(new GetStatisticsOperation());
            }
            catch (DatabaseDoesNotExistException)
            {
                if (createDatabaseIfNotExists == false)
                    throw;

                try
                {
                    await store.Maintenance.Server.SendAsync(new CreateDatabaseOperation(new DatabaseRecord(database)));
                }
                catch (ConcurrencyException)
                {
                }
            }
        }
    }
}
