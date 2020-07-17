using System;

namespace dn32.infra
{
    public class DesenvolvimentoIncorretoException : Exception
    {
        public DesenvolvimentoIncorretoException(string mensagem) : base(mensagem) { }
    }
}