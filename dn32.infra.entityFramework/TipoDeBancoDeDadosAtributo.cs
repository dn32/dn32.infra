using System;

namespace dn32.infra.EntityFramework
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class TipoDeBancoDeDadosAtributo : Attribute
    {
        public DnEnumTipoDeBancoDeDados TipoDeBancoDeDados { get; set; }

        public string Identifier { get; set; }

        public TipoDeBancoDeDadosAtributo(DnEnumTipoDeBancoDeDados tipoDeBancoDeDados)
        {
            TipoDeBancoDeDados = tipoDeBancoDeDados;
        }

        public TipoDeBancoDeDadosAtributo(DnEnumTipoDeBancoDeDados tipoDeBancoDeDados, string identifier)
        {
            TipoDeBancoDeDados = tipoDeBancoDeDados;
            Identifier = identifier;
        }
    }
}
