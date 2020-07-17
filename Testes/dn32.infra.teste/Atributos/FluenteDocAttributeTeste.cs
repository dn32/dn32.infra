using dn32.infra;
using dn32.infra;
using NUnit.Framework;

namespace dn32.infrae.Atributos {
    public class DnDocAtributoTeste {
        [Test]
        public void MostrarOk () {
            var atributo = new DnDocAttribute (EnumApresentar.Mostrar);
            Assert.AreEqual (EnumApresentar.Mostrar, atributo.Apresentacao);
        }

        [Test]
        public void OcultarOk () {
            var atributo = new DnDocAttribute (EnumApresentar.Ocultar);
            Assert.AreEqual (EnumApresentar.Ocultar, atributo.Apresentacao);
        }
    }
}