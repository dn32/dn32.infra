﻿using Newtonsoft.Json;

namespace dn32.infra {
    public class DnEntidadeNaoEncontradaErroDeValidacao : DnErroDeValidacao {
        [JsonProperty ("chave_de_globalizacao")]
        public override string ChaveDeGlobalizacao => nameof (DnEntidadeNaoEncontradaErroDeValidacao);

        public DnEntidadeNaoEncontradaErroDeValidacao (string chaves) : base ($"Nenhuma entidade foi encontrada com as chaves: {chaves}", false, chaves) { }
    }
}