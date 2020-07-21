using System;

namespace dn32.infra
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DnAgregacaoDeMuitosParaMuitosAttribute : DnAgregacaoAttribute
    {
        public bool EhMuitosParaMuitos { get; } = true;
    }
}