using Newtonsoft.Json;

namespace dn32.infra {
    public static class ExtensoesJson {
        public static JsonSerializerSettings ConfiguracoesDeSerializacao { get; set; }

        public static string SerializarParaDnJson (this object obj, Formatting formato = Formatting.None) =>
            JsonConvert.SerializeObject (obj, formato, ConfiguracoesDeSerializacao);
    }
}