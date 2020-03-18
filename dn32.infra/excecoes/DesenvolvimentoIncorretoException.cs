using System;

namespace dn32.infra.excecoes
{
    public class DesenvolvimentoIncorretoException : Exception
    {
        public DesenvolvimentoIncorretoException(string mensagem) : base(mensagem)
        {
        }
    }
}
