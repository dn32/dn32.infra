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
    }
}
