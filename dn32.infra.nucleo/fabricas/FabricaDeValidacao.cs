using dn32.infra.Util;
using dn32.infra.validacoes;
using System;
using dn32.infra.dados;
using dn32.infra.nucleo.validacoes;
using dn32.infra.nucleo.configuracoes;

namespace dn32.infra.nucleo.fabricas
{
    internal class FabricaDeValidacao
    {
        internal static DnValidacao<T> Criar<T>() where T : EntidadeBase
        {
            var tipoLocal = Setup.ConfiguracoesGlobais.TipoGenericoDeValidacao?.MakeGenericType(typeof(T)) ?? typeof(DnValidacao<T>);
            return Criar(tipoLocal) as DnValidacao<T>;
        }

        internal static DnValidacaoTransacional Criar(Type tipoDaValidacao)
        {
            var tipoLocal = tipoDaValidacao;
            var tipoDaEntidadeLocal = tipoDaValidacao.GetDnEntityType();

            if (tipoDaEntidadeLocal != null)
            {
                if (Setup.Validacoes.TryGetValue(tipoDaEntidadeLocal, out var tipoDeValidacaoDeSaida))
                {
                    tipoLocal = tipoDeValidacaoDeSaida;
                }
            }

            return Activator.CreateInstance(tipoLocal) as DnValidacaoTransacional;
        }
    }
}
