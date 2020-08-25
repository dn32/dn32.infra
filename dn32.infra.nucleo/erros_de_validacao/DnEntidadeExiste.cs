using Newtonsoft.Json;

namespace dn32.infra
{
    public class DnEntidadeExisteErroDeValidacao : DnErroDeValidacao
    {
        [JsonProperty("chave_de_globalizacao")]
        public override string ChaveDeGlobalizacao => nameof(DnEntidadeExisteErroDeValidacao);

        public string ChavesDaEntidade { get; set; }

        public DnEntidadeExisteErroDeValidacao(string chavesDaEntidade) :
            base($"Já existe um registro com esses dados cadastrado no sistema", false, chavesDaEntidade)
        {
            ChavesDaEntidade = chavesDaEntidade;
        }
    }
}