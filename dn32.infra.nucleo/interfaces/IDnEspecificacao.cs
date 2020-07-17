using System;

namespace dn32.infra
{
    public interface IDnEspecificacao : IDnEspecificacaoBase
    {
        Type TipoDeEntidade { get; }
    }
}