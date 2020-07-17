

namespace dn32.infra
{
    public class Filtro
    {
        public EnumTipoDeFiltro TipoDeFiltro { get; set; }

        public EnumTipoDeJuncao TipoDeJuncao { get; set; }

        public bool Inverter { get; set; }

        public string Valor { get; set; }

        public string NomeDaPropriedade { get; set; }

        public bool Inclusive { get; set; }
    }
}