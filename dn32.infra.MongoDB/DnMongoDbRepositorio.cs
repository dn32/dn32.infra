using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace dn32.infra
{
    public abstract class DnMongoDBRepositorio<TE> : DnRepositorio<TE> where TE : DnMongoDBEntidadeBase
    {
        public override Type TipoDeObjetosTransacionais => typeof(MongoDBObjetosDeTransacao);

        public SessaoDeRequisicaoDoUsuario SessaoDaRequisicao => Servico.SessaoDaRequisicao;

        protected MongoDBObjetosDeTransacao ObjetosTransacionaisMongoDB => ObjetosTransacionais as MongoDBObjetosDeTransacao;

        protected IMongoCollection<TE> ObterColecao() =>
                    ObjetosTransacionaisMongoDB.Contexto.Database.GetCollection<TE>(typeof(TE).Name);

        protected internal IMongoQueryable<TE> Query => this.ObjetosTransacionais.ObterObjetoQueryInterno<TE>() as IMongoQueryable<TE>;

        private static bool IndicesJaCriados { get; set; }

        private static SemaphoreSlim Semafoto { get; set; } = new SemaphoreSlim(1);

        protected abstract Task CriarEstruturaDaColecao();

        public DnMongoDBRepositorio()
        {
        }

        public override void Inicializar()
        {
            VerificarIndices().Wait();
        }

        private async Task VerificarIndices()
        {
            await Semafoto.WaitAsync();

            if (!IndicesJaCriados)
            {
                await CriarEstruturaDaColecao();
            }

            IndicesJaCriados = true;
            Semafoto.Release();
        }

        public async Task CriarIndiceSeNaoExistirAsync(IndexKeysDefinition<TE> indice, string NomeDoIndice, CreateIndexOptions<TE> options = null)
        {
            if (!await IndiceExisteAsync(NomeDoIndice))
            {
                var options_ = options ?? new CreateIndexOptions<TE> { Name = NomeDoIndice };
                await ObterColecao().Indexes.CreateOneAsync(new CreateIndexModel<TE>(indice, options_));
            }
        }

        public override Task ForEachAlternativoAsync<TO>(IDnEspecificacaoAlternativaGenerica<TO> ispec, Action<TO> action, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        private async Task<bool> IndiceExisteAsync(string nome)
        {
            var indices = await ObterColecao().Indexes.ListAsync();

            foreach (var indice in indices.ToList())
            {
                var codUserAdminCodUsuario = indice.GetElement("name");
                var name = codUserAdminCodUsuario.Value.ToString();
                if (name == nome) return true;
            }

            return false;
        }

        public override async Task ForEachAsync(Expression<Func<TE, bool>> expression, Action<TE> action, CancellationToken cancellationToken = default)
        {
            await Query.Where(expression).ForEachAsync(action, cancellationToken);
        }

        public override async Task<TE> AdicionarAsync(TE entidade)
        {
            await ObterColecao().InsertOneAsync(entidade);
            return entidade;
        }

        public override async Task AdicionarListaAsync(TE[] entidades)
        {
            await ObterColecao().InsertManyAsync(entidades);
        }

        public async Task AtualizarOuAdicionarAsync(Expression<Func<TE, bool>> expression, TE atualizacao)
        {
            var filtro = Builders<TE>.Filter.Where(expression);
            await ObterColecao().ReplaceOneAsync(filtro, atualizacao, new ReplaceOptions { IsUpsert = true });
        }

        public async Task RemoverEntidadeAsync(Expression<Func<TE, bool>> expression)
        {
            var filtro = Builders<TE>.Filter.Where(expression);
            await ObterColecao().DeleteOneAsync(filtro);
        }

        public async Task RemoverVariosAsync(Expression<Func<TE, bool>> expression)
        {
            var filtro = Builders<TE>.Filter.Where(expression);
            await ObterColecao().DeleteManyAsync(filtro);
        }

        public async Task<bool> AtualizarAsync(Expression<Func<TE, bool>> expression, UpdateDefinition<TE> entiade)
        {
            var filtro = Builders<TE>.Filter.Where(expression);
            var ret = await ObterColecao().UpdateOneAsync(filtro, entiade);
            return ret.ModifiedCount == 1;
        }

        public IMongoQueryable<TE> ListarQuery(Expression<Func<TE, bool>> expression)
        {
            var filtro = Builders<TE>.Filter.Where(expression);
            return Query.Where(expression);
        }

        public override async Task<List<TE>> ListarAsync(IDnEspecificacao ispec, DnPaginacao pagination = null)
        {
            var spec = ispec.ObterSpec<TE>();
            var query = spec.ConverterParaIQueryable(Query);
            var queryPaginada = await query.PaginarAsync(Servico, pagination, ef: false) as IMongoQueryable<TE>;

            return await queryPaginada.ToListAsync(); ;
        }

        public override async Task<List<TO>> ListarAlternativoAsync<TO>(IDnEspecificacaoAlternativaGenerica<TO> ispec, DnPaginacao pagination = null)
        {
            var spec = ispec.ObterSpecAlternativo<TE, TO>();
            var query = spec.ConverterParaIQueryable(Query);
            var DnPagination = await query.PaginarAsync<TO>(Servico, pagination, ef: false) as IMongoQueryable<TO>;
            return await DnPagination.ToListAsync();
        }

        public async Task<TE> PrimeiroOuPadraoAsync(Expression<Func<TE, bool>> expression) => await Query.FirstOrDefaultAsync(expression);

        public async Task<bool> RemoverAsync(Expression<Func<TE, bool>> expression)
        {
            var filtro = Builders<TE>.Filter.Where(expression);
            var ret = await ObterColecao().DeleteOneAsync(filtro);
            return ret.DeletedCount == 1;
        }

        public override Task<bool> ExisteAsync(TE entity, bool incluirExcluidosLogicamente = false)
        {
            //Todo - Remover validações desnecessárias, pois estão gerando custo de busca no BD
            throw new NotImplementedException();
        }

        public override TE Desanexar(TE entidade) => entidade;

        public override TX Desanexar<TX>(TX entidade) => entidade;

        public override Task<TE> AtualizarAsync(TE entity) => throw new NotImplementedException();

        public override Task AtualizarListaAsync(IEnumerable<TE> entidades) => throw new NotImplementedException();

        public override Task<TE> BuscarAsync(TE entity) => throw new NotImplementedException();

        public override Task EliminarTudoAsync() => throw new NotImplementedException();

        public override Task<bool> ExisteAlternativoAsync<TO>(IDnEspecificacaoBase spec) => throw new NotImplementedException();

        public override Task<bool> ExisteAsync(IDnEspecificacaoBase spec) => throw new NotImplementedException();

        public override Task<TE> RemoverAsync(TE entity)
        {
            throw new NotImplementedException();
        }

        public override Task<object> FindAsync(object entity) => throw new NotImplementedException();

        public override Task<bool> HaSomenteUmAsync(TE entity, bool incluirExcluidosLogicamente) => throw new NotImplementedException();

        public override Task<TO> PrimeiroOuPadraoAlternativoAsync<TO>(IDnEspecificacaoAlternativaGenerica<TO> spec)
        {
            throw new NotImplementedException();
        }

        public override Task<TE> PrimeiroOuPadraoAsync(IDnEspecificacao spec)
        {
            throw new NotImplementedException();
        }

        public override Task<int> QuantidadeAlternativoAsync<TO>(IDnEspecificacaoAlternativaGenerica<TO> spec)
        {
            throw new NotImplementedException();
        }

        public override Task<int> QuantidadeAsync(TE entity, bool incluirExcluidosLogicamente)
        {
            throw new NotImplementedException();
        }

        public override Task<int> QuantidadeAsync(IDnEspecificacao spec)
        {
            throw new NotImplementedException();
        }

        public override Task<int> QuantidadeTotalAsync()
        {
            throw new NotImplementedException();
        }



        public override void RemoverLista(IDnEspecificacao spec)
        {
            throw new NotImplementedException();
        }

        public override Task RemoverListaAsync(params TE[] entities)
        {
            throw new NotImplementedException();
        }

        public override Task<TE> SingleOrDefaultAsync(IDnEspecificacao spec)
        {
            throw new NotImplementedException();
        }

        public override Task<TO> UnicoOuPadraoAlternativoAsync<TO>(IDnEspecificacaoAlternativaGenerica<TO> spec)
        {
            throw new NotImplementedException();
        }
    }
}
