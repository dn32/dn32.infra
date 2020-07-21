using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace dn32.infra
{
    public partial class DnApiController<T>
    {
        [HttpGet]
        public virtual T Exemplo() => typeof(T).GetExampleValue() as T;

        [HttpGet]
        public virtual async Task<DnResultadoPadrao<int>> QuantidadeTotal() =>
        await this.CrieResultadoAsync(await this.Servico.QuantidadeTotalAsync());

        [HttpPost]
        public virtual async Task<DnResultadoPadrao<int>> QuantidadePorFiltro([FromBody] DnFiltro[] filtros)
        {
            var especificacao = this.CriarEspecificacaoDeFiltros(filtros, true);
            var quantidade = await this.Servico.QuantidadeAsync(especificacao);
            return await this.CrieResultadoAsync(quantidade);
        }

        [HttpGet]
        [Route("/api/[controller]/EntidadeExiste")]
        public virtual async Task<DnResultadoPadrao<bool>> EntidadeExisteGet([FromQuery] T entidade) =>
        await this.CrieResultadoAsync(await this.Servico.ExisteAsync(entidade));

        [HttpPost]
        [Route("/api/[controller]/EntidadeExiste")]
        public virtual async Task<DnResultadoPadrao<bool>> EntidadeExistePost([FromBody] T entidade) =>
        await this.CrieResultadoAsync(await this.Servico.ExisteAsync(entidade));

        [HttpGet]
        public virtual string Formulario(bool usarLayoutCompacto = false) =>
        typeof(T).GetDnJsonSchema(usarLayoutCompacto).SerializarParaDnJson();
    }
}