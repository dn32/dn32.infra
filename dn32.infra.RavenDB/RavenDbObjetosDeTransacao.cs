﻿using Raven.Client.Documents;
using System;
using System.Linq;

namespace dn32.infra
{
    /// <summary>
    /// Obtetos de transação.
    /// </summary>
    public class RavenDBObjetosDeTransacao : IDnObjetosTransacionais
    {
        public IDnDbContext contexto { get; set; }

        public RavenDbContexto Contexto => contexto as RavenDbContexto;

        public IDocumentStore Store => Contexto.Store as IDocumentStore;

        public SessaoDeRequisicaoDoUsuario UserSessionRequest { get; set; }

        public void Dispose()
        {
            this.Contexto.Dispose();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RavenDBObjetosDeTransacao"/> class. 
        /// Inicializa objeto de transação.
        /// </summary>
        /// <param Nome="dataBaseConnectionString">
        /// String de conexão com o banco de dados.
        /// </param>
        public RavenDBObjetosDeTransacao(Conexao connection, SessaoDeRequisicaoDoUsuario userSessionRequest)
        {
            this.UserSessionRequest = userSessionRequest;
            this.contexto = RavenDbContextoFabrica.Create(connection, UserSessionRequest);
        }

        public IQueryable<TX> ObterObjetoInputInterno<TX>() where TX : DnEntidadeBase
        {
            return Contexto.Sessao.Query<TX>();
        }

        public IQueryable ObterObjetoInputDataInterno(Type type)
        {
            throw new NotImplementedException();

        }

        /// <summary>
        /// Obtem o referência de uma tabela do banco de dados.
        /// </summary>
        /// <typeparam Nome="TX">
        /// Tipo de entidade desejada.
        /// </typeparam>
        /// <returns>
        /// A referência da tabela do banco de dados.
        /// </returns>
        public virtual IQueryable<TX> ObterObjetoQueryInterno<TX>() where TX : DnEntidadeBase
        {
            return Contexto.Sessao.Query<TX>();
        }
    }
}