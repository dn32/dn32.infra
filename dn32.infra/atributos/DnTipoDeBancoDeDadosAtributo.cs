using dn32.infra.enumeradores;
using System;

namespace dn32.infra.atributos
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class DnTipoDeBancoDeDadosAtributo : Attribute
    {
        public EnumTipoDeBancoDeDados TipoDeBancoDeDados { get; set; }

        public string Identifier { get; set; }

        public DnTipoDeBancoDeDadosAtributo(EnumTipoDeBancoDeDados tipoDeBancoDeDados)
        {
            TipoDeBancoDeDados = tipoDeBancoDeDados;
        }

        public DnTipoDeBancoDeDadosAtributo(EnumTipoDeBancoDeDados tipoDeBancoDeDados, string identifier)
        {
            TipoDeBancoDeDados = tipoDeBancoDeDados;
            Identifier = identifier;
        }
    }
}
