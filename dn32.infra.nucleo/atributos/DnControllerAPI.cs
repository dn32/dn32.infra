using System;

namespace dn32.infra
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DnControllerApiAttribute : Attribute
    {
        public bool GerarAutomaticamente { get; set; }

        public DnControllerApiAttribute()
        {
            GerarAutomaticamente = true;
        }

        public DnControllerApiAttribute(bool gerarAutomaticamente = true)
        {
            GerarAutomaticamente = gerarAutomaticamente;
        }
    }
}