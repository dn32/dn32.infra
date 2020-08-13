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

            var campo_ = campoDeComposicao == null ? propriedade.GetUiPropertyName() : campoDeComposicao + "." + propriedade.GetUiPropertyName();

            Mensagem = $"O campo '{campo_}' possui uma quantidade indevida de caracteres.";

            if (Minimo > 0)
            {
                if (Maximo > 0)
                {
                    if (Minimo == Maximo)
                        Mensagem = $"O campo '{campo_}' deve ter exatamente {Minimo} caractere{(Minimo > 1 ? "s" : "")}.";
                    else
                        Mensagem = $"O campo '{campo_}' deve ter entre {Minimo} e {Maximo} caracteres.";
                }
                else
                        Mensagem = $"O campo '{campo_}' deve ter no mínimo {Minimo} caractere{(Minimo > 1 ? "s" : "")}.";
            }

            this.Campo = propriedade.GetUiPropertyName();
        }
    }
}