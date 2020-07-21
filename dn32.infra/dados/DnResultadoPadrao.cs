

namespace dn32.infra
{
    [DnDocAttribute]
    public class DnResultadoPadrao<T>
    {
        public T Dados { get; set; }

        public DnResultadoPadrao()
        {
            Dados = default;
        }

        public DnResultadoPadrao(T dados)
        {
            Dados = dados;
        }
    }
}