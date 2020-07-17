using dn32.infra;

namespace dn32.infra {
    public class DnTudoEspecificacao<T> : DnDinamicaEspeficicacao<T> where T : DnEntidade {
        public DnTudoEspecificacao<T> AdicionarParametro (bool ehListagem) {
            EhListagem = ehListagem;
            return this;
        }
    }
}