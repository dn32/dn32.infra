﻿using Newtonsoft.Json;
using System;

namespace dn32.infra
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DnFormularioJsonAttribute : DnEsquemaJsonAttribute
    {
        [JsonIgnore]
        public Type Tipo { get; set; }

        public bool EhTabelaIntermediaria { get; set; }

        public bool EhSomenteLeitura { get; set; }
    }
}