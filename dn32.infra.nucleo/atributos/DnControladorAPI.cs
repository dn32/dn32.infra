using System;

namespace dn32.infra
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DnControladorApiAttribute : Attribute
    {
        public bool GerarAutomaticamente { get; set; }

        public DnControladorApiAttribute()
        {
            GerarAutomaticamente = true;
        }

        public DnControladorApiAttribute(bool gerarAutomaticamente = true)
        {
            GerarAutomaticamente = gerarAutomaticamente;
        }
    }
}