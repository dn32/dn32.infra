using System;

namespace dn32.infra.nucleo.interfaces
{
    public interface IDnEspecificacao : IDnEspecificacaoBase
    {
        Type TipoDeEntidade { get; }
    }
}
