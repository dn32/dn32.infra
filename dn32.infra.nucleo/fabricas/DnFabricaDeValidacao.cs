using dn32.infra.Util;
using dn32.infra.validacoes;
using System;
using dn32.infra.dados;
using dn32.infra.nucleo.validacoes;
using dn32.infra.nucleo.configuracoes;

namespace dn32.infra.Factory
{
    internal class DnFabricaDeValidacao
    {
        internal static DnDnValidacao<T> Criar<T>() where T : EntidadeBase
        {
            var tipoLocal = Setup.ConfiguracoesGlobais.GenericValidationType?.MakeGenericType(typeof(T)) ?? typeof(DnDnValidacao<T>);
            return Criar(tipoLocal) as DnDnValidacao<T>;
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
