using dn32.infra.dados;

namespace dn32.infra.nucleo.especificacoes
{
    public class DnTudoEspecificacao<T> : DnDinamicaEspeficicacao<T> where T : DnEntidade
    {
        public DnTudoEspecificacao<T> AdicionarParametro(bool ehListagem)
        {
            EhListagem = ehListagem;
            return this;
        }
    }
}
