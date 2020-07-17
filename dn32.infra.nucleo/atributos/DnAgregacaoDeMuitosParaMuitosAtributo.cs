using System;

namespace dn32.infra
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DnAgregacaoDeMuitosParaMuitosAtributo : DnAgregacaoAtributo
    {
        public bool EhMuitosParaMuitos { get; } = true;
    }
}