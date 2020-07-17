using dn32.infra;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace dn32.infrae.Extensoes {
    public class ExtensoesJsonTeste {
        [Test]
        public void SerializarParaDnJsonSerializacaoSimples () {
            const int obj = 1;
            const string json = "1";
            var jsonRetornado = obj.SerializarParaDnJson ();

            Assert.AreEqual (json, jsonRetornado);
        }

        [Test]
        public void SerializarParaDnJsonConfiguracoesDeSerializacaoMinusculo () {
            var obj = new { Valor = 1 };
            const string json = "{\"valor\":1}";
            ExtensoesJson.ConfiguracoesDeSerializacao = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver (), NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            var jsonRetornado = obj.SerializarParaDnJson ();

            Assert.AreEqual (json, jsonRetornado);
        }
    }
}