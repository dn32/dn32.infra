using System;
using dn32.infra;
using NUnit.Framework;

namespace dn32.infrae.Extensoes {
    public class ExtensoesDeTipoTeste {
        [Test]
        public void DnCastTiposIguaisOk () {
            const int valor = 1;
            var segundoValor = valor.DnCast<int> ();

            Assert.AreEqual (valor, segundoValor);
        }

        [Test]
        public void DnCastTiposDiferentesErro () {
            const int valor = 1;
            var ex = Assert.Catch<InvalidOperationException> (() => {
                valor.DnCast<string> ();
            });

            Assert.AreEqual ("O tipo 'Int32' não é um 'String'.", ex.Message);
        }

        [Test]
        public void DnCastTiposDiferentesOk () {
            const int valor = 1;
            var segundoValor = valor.DnCast<string> (dispararExcecaoCasoAConversaoFalhe: false);

            Assert.AreEqual (default (string), segundoValor);
        }
    }
}