using dn32.infra;

namespace dn32.infra {
    [DnDocAttribute]
    public class ResultadoPadraoComTermo<T> : ResultadoPadrao<T> {
        public string Termo { get; }

        public ResultadoPadraoComTermo () { }

        public ResultadoPadraoComTermo (T dados, string termo) : base (dados) {
            Termo = termo;
        }
    }
}