using System;
using System.Linq;
using System.Reflection;

namespace dn32.infra
{
    internal static class UtilitarioDeFabrica
    {
        internal static EnumTipoDeBancoDeDados ObterTipoDebancoDeDados(Type tipoDeEntidade)
        {
            if (tipoDeEntidade == null)
            {
                return EnumTipoDeBancoDeDados.NENHUM;
            }

            var dnAtributoTipoDeBD = tipoDeEntidade.GetCustomAttribute<DnTipoDeBancoDeDadosAttribute>();

            var conexoes = Setup.ConfiguracoesGlobais
                            .Conexoes
                            .Select(x => x.TipoDoContexto.GetCustomAttribute<DnTipoDeBancoDeDadosAttribute>()?.TipoDeBancoDeDados)
                            .Where(x => x != null)
                            .ToList();

            if (conexoes.Count == 1) return conexoes.FirstOrDefault() ?? EnumTipoDeBancoDeDados.MEMORY;
            return dnAtributoTipoDeBD?.TipoDeBancoDeDados ?? EnumTipoDeBancoDeDados.MEMORY;
        }

        internal static Type ObterTipoDeServicoPorEntidade(this Type tipoDeEntidade)
        {
            if (tipoDeEntidade == null) return null;
            if (!tipoDeEntidade.Is(typeof(DnEntidadeBase))) throw new DesenvolvimentoIncorretoException($"{nameof(ObterTipoDeServicoPorEntidade)} recebeu um tipo que não é uma {nameof(DnEntidadeBase)}.");

            if (Setup.Servicos.TryGetValue(tipoDeEntidade, out var tipoDeServico))
                return tipoDeServico;

            var tipoBasico = (Setup.ConfiguracoesGlobais.TipoGenericoDeServico) ?? typeof(DnServico<>);
            return tipoBasico.MakeGenericType(tipoDeEntidade);
        }

        internal static Type ObterServicoPorEntidade(this Type tipoDeEntidade)
        {
            if (tipoDeEntidade == null) return null;

            if (Setup.Servicos.TryGetValue(tipoDeEntidade, out var tipoDeServico))
                return tipoDeServico;

            var tipoBasico = (Setup.ConfiguracoesGlobais.TipoGenericoDeServico) ?? typeof(DnServico<>);
            return tipoBasico.MakeGenericType(tipoDeEntidade);
        }
    }
}
