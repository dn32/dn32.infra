namespace dn32.infra
{
    [DnDocAttribute]
    public class DnResultadoPadraoComTermo<T> : DnResultadoPadrao<T>
    {
        public string Termo { get; }

        public DnResultadoPadraoComTermo() { }

        public DnResultadoPadraoComTermo(T dados, string termo) : base(dados)
        {
            Termo = termo;
        }
    }
}