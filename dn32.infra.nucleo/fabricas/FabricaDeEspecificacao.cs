using dn32.infra.nucleo.especificacoes;
using dn32.infra.nucleo.excecoes;
using dn32.infra.servicos;
using System;

namespace dn32.infra.nucleo.fabricas
{
    public static class FabricaDeEspecificacao
    {
        public static T Criar<T>(DnServicoTransacionalBase service) where T : DnEspecificacaoBase
        {
            if (!(Activator.CreateInstance(typeof(T)) is T ts))
                throw new DesenvolvimentoIncorretoException($"Failed to initialize specification [{typeof(T).Name}] type with specified constructor parameters not found.");

            ts.DefinirServico(service);
            return ts;
        }
    }
}
