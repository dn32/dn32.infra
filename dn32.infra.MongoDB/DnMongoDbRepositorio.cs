using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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

        protected internal IQueryable<TE> Query => this.ObjetosTransacionais.ObterObjetoQueryInterno<TE>();

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

        public override async Task<TE> AdicionarAsync(TE entidade)
        {
            await ObterColecao().InsertOneAsync(entidade);
            return entidade;
        }

        public override async Task AdicionarListaAsync(TE[] entidades)
        {
            await ObterColecao().InsertManyAsync(entidades);
        }

        public async Task<TE> AtualizarAsync(FilterDefinition<TE> filtro, UpdateDefinition<TE> entiade)
        {
            return await ObterColecao().FindOneAndUpdateAsync(filtro, entiade);
        }

        protected DateTime LimparDataParaMongo(DateTime data)
        { // Um cuidado para que o BD não use sua propria data
            return JsonConvert.DeserializeObject<DateTime>(JsonConvert.SerializeObject(data));
        }

        public override async Task<List<TE>> ListarAsync(IDnEspecificacao ispec, DnPaginacao pagination = null)
        {
            var spec = ispec.ObterSpec<TE>();
            var query = spec.ConverterParaIQueryable(Query);
            var queryPaginada = await query.PaginarAsync(Servico, pagination, ef: false);
            return await queryPaginada.ToListAsync();
        }

        public override Task<TE> AtualizarAsync(TE entity) => throw new NotImplementedException();

        public override Task AtualizarListaAsync(IEnumerable<TE> entidades) => throw new NotImplementedException();

        public override Task<TE> BuscarAsync(TE entity) => throw new NotImplementedException();

        public override TE Desanexar(TE entity) => throw new NotImplementedException();

        public override TX Desanexar<TX>(TX entity) => throw new NotImplementedException();

        public override Task EliminarTudoAsync() => throw new NotImplementedException();

        public override Task<bool> ExisteAlternativoAsync<TO>(IDnEspecificacaoBase spec) => throw new NotImplementedException();

        public override Task<bool> ExisteAsync(IDnEspecificacaoBase spec) => throw new NotImplementedException();

        public override Task<bool> ExisteAsync(TE entity, bool incluirExcluidosLogicamente = false) => throw new NotImplementedException();

        public override Task<object> FindAsync(object entity) => throw new NotImplementedException();

        public override Task<bool> HaSomenteUmAsync(TE entity, bool incluirExcluidosLogicamente) => throw new NotImplementedException();

        public override Task<List<TO>> ListarAlternativoAsync<TO>(IDnEspecificacaoAlternativaGenerica<TO> spec, DnPaginacao pagination = null)
        {
            throw new NotImplementedException();
        }


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

        public override Task<TE> RemoverAsync(TE entity)
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
