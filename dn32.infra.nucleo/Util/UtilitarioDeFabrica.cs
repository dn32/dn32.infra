using System;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace dn32.infra
{
    internal static class UtilitarioDeFabrica
    {
        internal static EnumTipoDeBancoDeDados ObterTipoDebancoDeDados(Type tipoDeEntidade)
        {
            if (tipoDeEntidade == null) return EnumTipoDeBancoDeDados.NENHUM;

            var dnAtributoTipoDeBD = tipoDeEntidade.GetCustomAttribute<DnTipoDeBancoDeDadosAttribute>();

            var conexoes = Setup.ConfiguracoesGlobais.ObterConexoes(dnAtributoTipoDeBD);

            if (conexoes.Count == 1) return conexoes.FirstOrDefault().TipoDeBancoDeDados;
            return dnAtributoTipoDeBD?.ObterTipo() ?? EnumTipoDeBancoDeDados.NENHUM;
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
