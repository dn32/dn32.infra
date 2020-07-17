using System;
using dn32.infra;

namespace dn32.infra {
    internal class DnDescricaoDePropriedade {
        public string Nome { get; set; }

        public Type Tipo { get; set; }

        public Type TipoDaPropriedadeDinamica { get; set; }

        public DnDescricaoDaClasse DescricaoDaClasse { get; set; }
    }
}