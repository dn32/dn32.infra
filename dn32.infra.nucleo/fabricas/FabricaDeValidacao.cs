using System;





namespace dn32.infra
{
    internal class FabricaDeValidacao
    {
        internal static DnValidacao<T> Criar<T>() where T : DnEntidadeBase
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