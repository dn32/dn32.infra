﻿using Newtonsoft.Json;
using System;

namespace dn32.infra
{
    public class DnEsquemaJsonAttribute : Attribute
    {
        private string nome;

        public string Nome { get => nome ?? NomeDaPropriedade; set => nome = value; }

        public string Descricao { get; set; }

        public string NomeDaPropriedade { get; set; }

        public string Link { get; set; }

        [JsonIgnore]
        public string NomeDaPropriedadeCaseSensitive { get; set; }

        public string Grupo { get; set; }

        [JsonIgnore]
        public override object TypeId => base.TypeId;
    }
}