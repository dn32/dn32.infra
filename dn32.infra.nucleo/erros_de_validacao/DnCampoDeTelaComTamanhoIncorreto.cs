using Newtonsoft.Json;
using System.Reflection;

namespace dn32.infra
{
    public class DnCampoDeTelaComTamanhoIncorretoErroDeValidacao : DnCampoDeTelaErroDeValidacao
    {
        [JsonProperty("chave_de_globalizacao")]
        public override string ChaveDeGlobalizacao => nameof(DnCampoDeTelaComTamanhoIncorretoErroDeValidacao);

        public int Minimo { get; set; }

        public double Maximo { get; set; }

        public DnCampoDeTelaComTamanhoIncorretoErroDeValidacao(
                PropertyInfo propriedade,
                string propriedadeDeComposicao,
                string campoDeComposicao) :
            base(propriedade, true, "", propriedadeDeComposicao, campoDeComposicao)
        {
            var ret = propriedade.GetPropertyRange();
            this.Minimo = ret?.min ?? 0;
            this.Maximo = ret?.max ?? 0;

            Mensagem = $"O campo '{(campoDeComposicao == null ? propriedade.GetUiPropertyName() : campoDeComposicao + "." + propriedade.GetUiPropertyName())}' possui uma quantidade indevida de caracteres.";

            if (Minimo > 0)
            {
                if (Maximo > 0)
                    Mensagem += $" Espera-se entre {Minimo} e {Maximo}.";
                else
                    Mensagem += $" Espera-se no mínimo {Minimo}.";
            }

            this.Campo = propriedade.GetUiPropertyName();
        }
    }
}