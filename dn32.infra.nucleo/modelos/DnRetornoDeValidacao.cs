using System.Collections.Generic;
using dn32.infra;

namespace dn32.infra {
    public class DnRetornoDeValidacao {
        public string Mensagem { get; set; }

        public bool EhErroDeValidacao { get; set; }

        public List<DnInconsistencia> Inconsistencias { get; set; }
    }
}