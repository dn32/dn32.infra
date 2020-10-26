
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System.Linq;
using System.Threading.Tasks;

namespace dn32.infra
{
    public static class DnPaginacaoUtil
    {
        private static DnPaginacao OnterPaginacaoDoContexto(DnServicoBase Servico)
        {
            var currentPageInt = int.TryParse(GetParameter(Servico, DnParametros.NomePaginaAtual), out var currentPageInt_) ? currentPageInt_ : 0;
            var itemsPerPageInt = int.TryParse(GetParameter(Servico, DnParametros.NomeItensPorPagina), out var itemsPerPageInt_) ? itemsPerPageInt_ : DnPaginacao.ITENS_POR_PAGINA_PADRAO;
            var startAtZeroBool = bool.TryParse(GetParameter(Servico, DnParametros.NomeIniciarNaPaginaZero), out var startAtZeroBool_) && startAtZeroBool_;
            var liberarMaisDe100Itens = bool.TryParse(GetParameter(Servico, DnParametros.LiberarMaisDe100Itens), out var liberarMaisDe100Itens_) && liberarMaisDe100Itens_;

            return DnPaginacao.Criar(currentPageInt, startAtZeroBool, itemsPerPageInt, liberarMaisDe100Itens);
        }

        public static async Task<IQueryable<TX>> PaginarAsync<TX>(this IQueryable<TX> query, DnServicoBase Servico, DnPaginacao pagination, bool ef)
        {
            await Task.CompletedTask;
            if (pagination == null)
            {
                pagination = OnterPaginacaoDoContexto(Servico) ?? DnPaginacao.Criar(0);
            }

            pagination.QuantidadeTotalDeItens = (ef == true) ? await query.CountAsync() : 0;
            Servico.SessaoDaRequisicao.Paginacao = pagination;

            return query.Skip(pagination.Salto).Take(pagination.ItensPorPagina);
        }

        internal static string GetParameter(DnServicoBase Servico, string key)
        {
            if (Servico.SessaoDaRequisicao.SessaoSemContexto) return string.Empty;
            var request = Servico.SessaoDaRequisicao.HttpContextLocal.Request;

            {
                if (request.Headers.TryGetValue(key, out StringValues valor) && !string.IsNullOrEmpty(valor))
                    return valor;
            }

            {
                if (request.Query.TryGetValue(key, out StringValues valor) && !string.IsNullOrEmpty(valor))
                    return valor;
            }

            {
                var valor = request.Query[key];
                if (!string.IsNullOrEmpty(valor))
                    return valor;
            }

            if (request.Method == "GET" || request.HasFormContentType == false)
                return "";

            return request.Form[key];
        }
    }
}
