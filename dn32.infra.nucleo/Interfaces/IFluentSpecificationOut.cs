using System;

namespace dn32.infra
{
    public interface IDnEspecificacaoAlternativa : IDnEspecificacaoBase
    {
        Type TipoDeEntidade { get; }
        Type TipoDeRetorno { get; }
    }
}