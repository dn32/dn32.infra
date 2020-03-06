using System;

namespace dn32.infra.nucleo.interfaces
{
    public interface IDnEspecificacaoAlternativa : IDnEspecificacaoBase
    {
        Type TipoDeEntidade { get; }
        Type TipoDeRetorno { get; }
    }
}