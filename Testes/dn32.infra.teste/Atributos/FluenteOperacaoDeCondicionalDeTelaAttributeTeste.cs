

using Newtonsoft.Json;
using NUnit.Framework;

namespace dn32.infra
{
    public class DnOperacaoDeCondicionalDeTelaAttributeTeste
    {
        [Test]
        public void ComparacaoDeValorOk()
        {
            const string propriedadeAvaliada = "propriedade01";
            const string valorParaODisparo = "valor-de-teste";
            const EnumTipoDeEventoDeTela tipoDeDisparo = EnumTipoDeEventoDeTela.Alterar;
            const EnumTipoDeOperacaoDeTela operacao = EnumTipoDeOperacaoDeTela.Exibir;
            const EnumTipoDeComparacaoDeTela tipoDeComparacao = EnumTipoDeComparacaoDeTela.Igual;

            var atributo = new DnOperacaoDeCondicionalDeTelaAttribute(
                propriedadeAvaliada, valorParaODisparo);

            Assert.AreEqual(propriedadeAvaliada, atributo.PropriedadeAvaliada);
            Assert.AreEqual(operacao, atributo.Operacao);
            Assert.AreEqual(tipoDeComparacao, atributo.TipoDeComparacao);
            Assert.AreEqual(tipoDeDisparo, atributo.TipoDeDisparo);
            Assert.IsNull(atributo.Valor);
            Assert.AreEqual(valorParaODisparo, JsonConvert.DeserializeObject<string>(atributo.ValorParaODisparo));
        }

        [Test]
        public void ComparacaoDeValorOperacaoOk()
        {
            const string propriedadeAvaliada = "propriedade01";
            const string valorParaODisparo = "valor-de-teste";
            const EnumTipoDeEventoDeTela tipoDeDisparo = EnumTipoDeEventoDeTela.Alterar;
            const EnumTipoDeOperacaoDeTela operacao = EnumTipoDeOperacaoDeTela.Focar;
            const EnumTipoDeComparacaoDeTela tipoDeComparacao = EnumTipoDeComparacaoDeTela.Igual;

            var atributo = new DnOperacaoDeCondicionalDeTelaAttribute(
                propriedadeAvaliada, valorParaODisparo, operacao);

            Assert.AreEqual(propriedadeAvaliada, atributo.PropriedadeAvaliada);
            Assert.AreEqual(operacao, atributo.Operacao);
            Assert.AreEqual(tipoDeComparacao, atributo.TipoDeComparacao);
            Assert.AreEqual(tipoDeDisparo, atributo.TipoDeDisparo);
            Assert.IsNull(atributo.Valor);
            Assert.AreEqual(valorParaODisparo, JsonConvert.DeserializeObject<string>(atributo.ValorParaODisparo));
        }

        [Test]
        public void ComparacaoDeValorTipoDeDisparoOk()
        {
            const string propriedadeAvaliada = "propriedade01";
            const string valorParaODisparo = "valor-de-teste";
            const EnumTipoDeEventoDeTela tipoDeDisparo = EnumTipoDeEventoDeTela.Nenhum;
            const EnumTipoDeOperacaoDeTela operacao = EnumTipoDeOperacaoDeTela.Focar;
            const EnumTipoDeComparacaoDeTela tipoDeComparacao = EnumTipoDeComparacaoDeTela.Diferente;

            var atributo = new DnOperacaoDeCondicionalDeTelaAttribute(
                propriedadeAvaliada,
                valorParaODisparo,
                operacao,
                tipoDeDisparo
            )
            {
                TipoDeComparacao = EnumTipoDeComparacaoDeTela.Diferente
            };

            Assert.AreEqual(propriedadeAvaliada, atributo.PropriedadeAvaliada);
            Assert.AreEqual(operacao, atributo.Operacao);
            Assert.AreEqual(tipoDeComparacao, atributo.TipoDeComparacao);
            Assert.AreEqual(tipoDeDisparo, atributo.TipoDeDisparo);
            Assert.IsNull(atributo.Valor);
            Assert.AreEqual(valorParaODisparo, JsonConvert.DeserializeObject<string>(atributo.ValorParaODisparo));
        }
    }
}