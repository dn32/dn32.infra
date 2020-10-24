using MongoDB.Driver;
using System;
using System.Linq;

namespace dn32.infra
{
    public class MongoDBObjetosDeTransacao : IDnObjetosTransacionais
    {
        public IDnDbContext contexto { get; set; }

        public MongoDbContexto Contexto => contexto as MongoDbContexto;

        public SessaoDeRequisicaoDoUsuario UserSessionRequest { get; set; }

        public MongoDBObjetosDeTransacao(Conexao connection, SessaoDeRequisicaoDoUsuario userSessionRequest)
        {
            UserSessionRequest = userSessionRequest;
            contexto = MongoDbContextoFabrica.Create(connection, UserSessionRequest);
        }

        public IQueryable<TX> ObterObjetoInputInterno<TX>() where TX : DnEntidadeBase =>
              Contexto.Database.GetCollection<TX>(typeof(TX).Name).AsQueryable();

        public virtual IQueryable<TX> ObterObjetoQueryInterno<TX>() where TX : DnEntidadeBase =>
              Contexto.Database.GetCollection<TX>(typeof(TX).Name).AsQueryable();

        public void Dispose() => Contexto.Dispose();

        public IQueryable ObterObjetoInputDataInterno(Type type)
        {
            throw new NotImplementedException();
        }
    }
}
