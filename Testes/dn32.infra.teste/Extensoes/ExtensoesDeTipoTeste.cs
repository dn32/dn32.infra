using System;
using dn32.infra.extensoes;
using NUnit.Framework;

namespace dn32.infra.Teste.Extensoes
{
    public class ExtensoesDeTipoTeste
    {
        [Test]
        public void FluenteCastTiposIguaisOk()
        {
            const int valor = 1;
            var segundoValor = valor.FluenteCast<int>();

            Assert.AreEqual(valor, segundoValor);
        }

        [Test]
        public void FluenteCastTiposDiferentesErro()
        {
            const int valor = 1;
            var ex = Assert.Catch<InvalidOperationException>(() =>
            {
                valor.FluenteCast<string>();
            });

            Assert.AreEqual("O tipo 'Int32' não é um 'String'.", ex.Message);
        }

        [Test]
        public void FluenteCastTiposDiferentesOk()
        {
            const int valor = 1;
            var segundoValor = valor.FluenteCast<string>(dispararExcecaoCasoAConversaoFalhe: false);

            Assert.AreEqual(default(string), segundoValor);
        }
    }
}
