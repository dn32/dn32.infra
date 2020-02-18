using dn32.infra.atributos;
using dn32.infra.enumeradores;
using NUnit.Framework;

namespace dn32.infra.Teste.Atributos
{
    public class FluenteDocAtributoTeste
    {
        [Test]
        public void MostrarOk()
        {
            var atributo = new FluenteDocAtributo(EnumMostrar.Mostrar);
            Assert.AreEqual(EnumMostrar.Mostrar, atributo.Apresentacao);
        }

        [Test]
        public void OcultarOk()
        {
            var atributo = new FluenteDocAtributo(EnumMostrar.Ocultar);
            Assert.AreEqual(EnumMostrar.Ocultar, atributo.Apresentacao);
        }
    }
}