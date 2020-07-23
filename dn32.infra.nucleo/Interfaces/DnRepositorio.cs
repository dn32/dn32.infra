using System;
using System.Collections.Generic;
using System.Threading.Tasks;




namespace dn32.infra
{
    public abstract class DnRepositorio<TE> : IDnRepositorioTransacional where TE : DnEntidadeBase
    {
        #region PROPERTIES

        public virtual DnServico<TE> Servico { get; set; }
        public virtual IDnObjetosTransacionais ObjetosTransacionais { get; set; }
        public abstract Type TipoDeObjetosTransacionais { get; }

        #endregion

        public abstract void RemoverLista(IDnEspecificacao spec);
        public abstract TE Desanexar(TE entity);
        public abstract TX Desanexar<TX>(TX entity);
        public abstract Task RemoverListaAsync(params TE[] entities);
        public abstract Task<TE> AtualizarAsync(TE entity);
        public abstract Task AtualizarListaAsync(IEnumerable<TE> entities);
        public abstract Task EliminarTudoAsync();
        public abstract Task<TE> RemoverAsync(TE entity);
        public abstract Task<bool> ExisteAlternativoAsync<TO>(IDnEspecificacaoBase spec);
        public abstract Task<bool> ExisteAsync(IDnEspecificacaoBase spec);
        public abstract Task<List<TE>> ListarAsync(IDnEspecificacao spec, DnPaginacao pagination = null);
        public abstract Task<List<TO>> ListarAlternativoAsync<TO>(IDnEspecificacaoAlternativaGenerica<TO> spec, DnPaginacao pagination = null);
        public abstract Task<TO> PrimeiroOuPadraoAlternativoAsync<TO>(IDnEspecificacaoAlternativaGenerica<TO> spec);
        public abstract Task<TE> PrimeiroOuPadraoAsync(IDnEspecificacao spec);
        public abstract Task<TE> SingleOrDefaultAsync(IDnEspecificacao spec);
        public abstract Task<TO> UnicoOuPadraoAlternativoAsync<TO>(IDnEspecificacaoAlternativaGenerica<TO> spec);
        public abstract Task<bool> ExisteAsync(TE entity, bool incluirExcluidosLogicamente = false);
        public abstract Task<TE> BuscarAsync(TE entity);
        public abstract Task<TE> AdicionarAsync(TE entity);
        public abstract Task AdicionarListaAsync(TE[] entities);
        public abstract Task<bool> HaSomenteUmAsync(TE entity, bool incluirExcluidosLogicamente);
        public abstract Task<int> QuantidadeAlternativoAsync<TO>(IDnEspecificacaoAlternativaGenerica<TO> spec);
        public abstract Task<int> QuantidadeAsync(TE entity, bool incluirExcluidosLogicamente);
        public abstract Task<int> QuantidadeAsync(IDnEspecificacao spec);
        public abstract Task<int> QuantidadeTotalAsync();
        public abstract Task<object> FindAsync(object entity);
    }
}