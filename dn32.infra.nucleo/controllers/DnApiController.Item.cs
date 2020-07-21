using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace dn32.infra
{
    public partial class DnApiController<T>
    {
        [HttpGet]
        [Route("/api/[controller]/BuscarPorEntidade")]
        public virtual async Task<DnResultadoPadrao<T>> BuscarPorEntidadeGet([FromQuery] T entidade)
        {
            return await this.CrieResultadoAsync(await this.Servico.BuscarAsync(entidade, false));
        }

        [HttpPost]
        [Route("/api/[controller]/BuscarPorEntidade")]
        public virtual async Task<DnResultadoPadrao<T>> BuscarPorEntidadePost([FromBody] T entidade)
        {
            return await this.CrieResultadoAsync(await this.Servico.BuscarAsync(entidade, false));
        }

        [HttpGet]
        [Route("/api/[controller]/BuscarPorFiltro")]
        [DnActionAttribute(EspecificacaoDinamica = true)]
        public virtual async Task<DnResultadoPadrao<T>> BuscarPorFiltroGet([FromQuery] DnFiltro[] filtros)
        {
            var especificacao = this.CriarEspecificacaoDeFiltros(filtros, false);
            var item = await this.Servico.UnicoOuPadraoAsync(especificacao);
            return await this.CrieResultadoAsync(item);
        }

        [HttpPost]
        [Route("/api/[controller]/BuscarPorFiltro")]
        [DnActionAttribute(EspecificacaoDinamica = true)]
        public virtual async Task<DnResultadoPadrao<T>> BuscarPorFiltroPost([FromBody] DnFiltro[] filtros)
        {
            var especificacao = this.CriarEspecificacaoDeFiltros(filtros, false);
            var item = await this.Servico.UnicoOuPadraoAsync(especificacao);
            return await this.CrieResultadoAsync(item);
        }

        [HttpGet]
        [DnActionAttribute(EspecificacaoDinamica = true)]
        public virtual async Task<DnResultadoPadrao<T>> BuscarPorTermo(string termo)
        {
            var especificacao = this.CriarEspecificacaoPorTermo(termo, ehLista: false);
            var item = await this.Servico.UnicoOuPadraoAsync(especificacao);
            return await this.CrieResultadoAsync(item);
        }

        private DnTermoEspecificacao<T> CriarEspecificacaoPorTermo(string termo, bool ehLista) =>
        this.CriarEspecificacao<DnTermoEspecificacao<T>>().SetParameter(termo, ehLista);

        private DnFiltrosEspecificacao<T> CriarEspecificacaoDeFiltros(DnFiltro[] filtros, bool ehLista) =>
        this.CriarEspecificacao<DnFiltrosEspecificacao<T>>().SetParameter(filtros, ehLista);
    }
}