using System;

namespace dn32.infra
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DnCriarValorRandomicoAoAdicionarEntidadeAttribute : Attribute
    {
        public int TamanhoMaximo { get; set; }

        public DnCriarValorRandomicoAoAdicionarEntidadeAttribute(int tamanhoMaximo = 0)
        {
            this.TamanhoMaximo = tamanhoMaximo;
        }
    }
}