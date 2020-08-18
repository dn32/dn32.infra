using Newtonsoft.Json;

namespace dn32.infra
{
    public class DnEntidadeNaoEncontradaErroDeValidacao : DnErroDeValidacao
    {
        [JsonProperty("chave_de_globalizacao")]
        public override string ChaveDeGlobalizacao => nameof(DnEntidadeNaoEncontradaErroDeValidacao);

        public string ChavesDaEntidade { get; set; }

        public DnEntidadeNaoEncontradaErroDeValidacao(string chavesDaEntidade) :
            base($"O registro solicitado não foi encontrado no sistema. Ele pode ter sido excluído", false, chavesDaEntidade)
        {
            ChavesDaEntidade = chavesDaEntidade;
        }
    }
}