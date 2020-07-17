using System;

namespace dn32.infra
{
    public class DnDesabilitadoAttribute : Attribute
    {
        public string Motivo { get; set; } = "Desabilitado";

        public DnDesabilitadoAttribute() { }

        public DnDesabilitadoAttribute(string motivo) => Motivo = motivo;
    }
}