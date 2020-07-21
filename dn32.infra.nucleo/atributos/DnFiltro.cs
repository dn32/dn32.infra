using Newtonsoft.Json;
using System;
using System.Linq;

namespace dn32.infra
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DnFiltroAttribute : Attribute
    {
        public EnumTipoDeFiltro TipoDeFiltro { get; set; } = EnumTipoDeFiltro.Igual;

        public string[] ChavesLocais { get; set; }

        public string[] ChavesExternas { get; set; }

        public string[] CamposParaLimpar { get; set; }

        [JsonIgnore]
        public string ChaveLocal
        {
            get => this.ChavesExternas?.First();
            set
            {
                this.ChavesLocais = new[] { value };
            }
        }

        [JsonIgnore]
        public string ChaveExterna
        {
            get => this.ChavesExternas?.First();
            set
            {
                this.ChavesExternas = new[] { value };
            }
        }

        [JsonIgnore]
        public string CampoParaLimpar
        {
            get => this.CamposParaLimpar?.First();
            set
            {
                this.CamposParaLimpar = new[] { value };
            }
        }

        [JsonIgnore]
        public override object TypeId => base.TypeId;

        public string NomeDaPropriedade { get; internal set; }
    }
}