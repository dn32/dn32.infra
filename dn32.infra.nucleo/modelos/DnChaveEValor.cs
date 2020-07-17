using System.Reflection;

namespace dn32.infra
{
    public class DnChaveEValor
    {
        public PropertyInfo Propriedade { get; set; }
        public object Valor { get; set; }
        public string NomeDaColuna { get; set; }
    }
}