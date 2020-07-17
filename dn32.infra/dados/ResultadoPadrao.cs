using dn32.infra;

namespace dn32.infra {
    [DnDocAttribute]
    public class ResultadoPadrao<T> {
        public T Dados { get; set; }

        public ResultadoPadrao () {
            Dados = default;
        }

        public ResultadoPadrao (T dados) {
            Dados = dados;
        }
    }
}