using MongoDB.Driver;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace dn32.infra
{
    [DnTipoDeBancoDeDados(EnumTipoDeBancoDeDados.MONGO_DB)]
    public class MongoDbContexto : IDnDbContext
    {
        internal protected SessaoDeRequisicaoDoUsuario UserSessionRequest { get; internal set; }

        public bool EnableLogicalDeletion { get; set; }

        protected internal string ConnectionString { get; set; }

        internal string NomeDoBD { get; set; }

        internal IMongoDatabase Database { get; set; }

        public bool HaAlteracao => throw new NotImplementedException();// Sessao.Advanced.HasChanges;


        public MongoDbContexto(string connectionString)
        {
            NomeDoBD = Setup.ConfiguracoesGlobais.Valores["mongoDBNomeDoBD"];
            ConnectionString = connectionString;
            var client = new MongoClient(connectionString);
            Database = client.GetDatabase(NomeDoBD);
        }

        public void Dispose()
        {
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
