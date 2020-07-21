using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dn32.infra
{
    public partial class DnApiController<T>
    {
        [HttpGet]
        [DnActionAttribute(Paginacao = true, EspecificacaoDinamica = true)]
        public virtual async Task<DnResultadoPadraoPaginado<List<T>>> Listar()
        {
            var especificacao = this.CriarEspecificacaoDeConsultarTudo();
            var lista = await this.Servico.ListarAsync(especificacao);
            return await this.CrieResultadoAsync(lista, this.PaginacaoDaUltimaRequisicao);
        }

        [HttpGet]
        [Route("/api/[controller]/ListarPorFiltro")]
        [DnActionAttribute(Paginacao = true, EspecificacaoDinamica = true)]
        public virtual async Task<DnResultadoPadraoPaginado<List<T>>> ListarPorFiltroGet([FromQuery] DnFiltro[] filtros)
        {
            return await this.ListarPorFiltroInterno(filtros);
        }

        [HttpPost]
        [Route("/api/[controller]/ListarPorFiltro")]
        [DnActionAttribute(Paginacao = true, EspecificacaoDinamica = true)]
        public virtual async Task<DnResultadoPadraoPaginado<List<T>>> ListarPorFiltroPost([FromBody] DnFiltro[] filtros)
        {
            return await this.ListarPorFiltroInterno(filtros);
        }

        [HttpGet]
        [DnActionAttribute(Paginacao = true, EspecificacaoDinamica = true)]
        public virtual async Task<DnResultadoPadraoPaginadoComTermo<List<T>>> ListarPorTermo(string termo)
        {
            var especificacao = this.CriarEspecificacaoPorTermo(termo, ehLista: true);
            var lista = await this.Servico.ListarAsync(especificacao);
            return await this.CrieResultadoAsync(lista, this.PaginacaoDaUltimaRequisicao, termo);
        }

        protected virtual async Task<DnResultadoPadraoPaginado<List<T>>> ListarPorFiltroInterno([FromBody] DnFiltro[] filtros)
        {
            var especificacao = this.CriarEspecificacaoDeFiltros(filtros, false);
            var lista = await this.Servico.ListarAsync(especificacao);
            return await this.CrieResultadoAsync(lista, this.PaginacaoDaUltimaRequisicao);
        }

        private DnTudoEspecificacao<T> CriarEspecificacaoDeConsultarTudo() => this.CriarEspecificacao<DnTudoEspecificacao<T>>().AdicionarParametro(ehListagem: true);
    }
}