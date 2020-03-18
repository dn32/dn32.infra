using dn32.infra.nucleo.interfaces;
using dn32.infra.servicos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dn32.infra.dados;

namespace dn32.infra.nucleo.interfaces
{
    public interface IDnRepositorio<TE> : IDnRepositorioTransacional where TE : EntidadeBase
    {
        #region PROPERTIES

        DnServico<TE> Servico { get; set; }
        IDnObjetosTransacionais ObjetosTransacionais { get; set; }
        Type TipoDeObjetosTransacionais { get; }

        #endregion

        void RemoverLista(IDnEspecificacao spec);
        TX Desanexar<TX>(TX entity);
        Task RemoverListaAsync(params TE[] entities);
        Task<TE> AtualizarAsync(TE entity);
        Task AtualizarListaAsync(IEnumerable<TE> entities);
        Task EliminarTudoAsync();
        Task<TE> RemoverAsync(TE entity);
        Task<bool> ExisteAlternativoAsync<TO>(IDnEspecificacaoBase spec);
        Task<bool> ExisteAsync(IDnEspecificacaoBase spec);
        Task<List<TE>> ListarAsync(IDnEspecificacao spec, DnPaginacao pagination = null);
        Task<List<TO>> ListarAlternativoAsync<TO>(IDnEspecificacaoAlternativaGenerica<TO> spec, DnPaginacao pagination = null);
        Task<TO> PrimeiroOuPadraoAlternativoAsync<TO>(IDnEspecificacaoAlternativaGenerica<TO> spec);
        Task<TE> PrimeiroOuPadraoAsync(IDnEspecificacao spec);
        Task<TE> SingleOrDefaultAsync(IDnEspecificacao spec);
        Task<TO> UnicoOuPadraoAlternativoAsync<TO>(IDnEspecificacaoAlternativaGenerica<TO> spec);
        Task<bool> ExisteAsync(TE entity, bool includeExcludedLogically = false);
        Task<TE> BuscarAsync(TE entity);
        Task<TE> AdicionarAsync(TE entity);
        Task AdicionarListaAsync(TE[] entities);
        Task<bool> HaSomenteUmAsync(TE entity, bool includeExcludedLogically);
        Task<int> QuantidadeAlternativoAsync<TO>(IDnEspecificacaoAlternativaGenerica<TO> spec);
        Task<int> QuantidadeAsync(TE entity, bool includeExcludedLogically);
        Task<int> QuantidadeAsync(IDnEspecificacao spec);
        Task<int> QuantidadeTotalAsync();
    }
}
