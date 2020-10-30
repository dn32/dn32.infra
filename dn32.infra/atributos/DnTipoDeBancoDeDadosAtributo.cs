using System;


namespace dn32.infra
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class DnTipoDeBancoDeDadosAttribute : Attribute
    {
        private EnumTipoDeBancoDeDados TipoDeBancoDeDados { get; set; }
        public EnumTipoDeBancoDeDados ObterTipo() => TipoDeBancoDeDados;
        public string Identifier { get; set; }

        public DnTipoDeBancoDeDadosAttribute(EnumTipoDeBancoDeDados tipoDeBancoDeDados)
        {
            TipoDeBancoDeDados = tipoDeBancoDeDados;
        }

        public DnTipoDeBancoDeDadosAttribute(EnumTipoDeBancoDeDados tipoDeBancoDeDados, string identifier)
        {
            TipoDeBancoDeDados = tipoDeBancoDeDados;
            Identifier = identifier;
        }
    }
}