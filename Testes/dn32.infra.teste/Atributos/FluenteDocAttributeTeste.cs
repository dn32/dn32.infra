using dn32.infra.atributos;
using dn32.infra.enumeradores;
using NUnit.Framework;

namespace dn32.infra.Teste.Atributos
{
    public class DnDocAtributoTeste
    {
        [Test]
        public void MostrarOk()
        {
            var atributo = new DnDocAtributo(EnumApresentar.Mostrar);
            Assert.AreEqual(EnumApresentar.Mostrar, atributo.Apresentacao);
        }

        [Test]
        public void OcultarOk()
        {
            var atributo = new DnDocAtributo(EnumApresentar.Ocultar);
            Assert.AreEqual(EnumApresentar.Ocultar, atributo.Apresentacao);
        }
    }
}