﻿using Newtonsoft.Json;
using System;

namespace dn32.infra
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class DnOperacaoDeCondicionalDeTelaAttribute : Attribute
    {
        #region PROPRIEDADES

        private string _valor;

        public string PropriedadeAvaliada { get; }

        public string ValorParaODisparo { get; }

        [JsonIgnore]
        public object Valor { get => _valor; set => _valor = value?.ToString(); }

        public EnumTipoDeEventoDeTela TipoDeDisparo { get; set; } = EnumTipoDeEventoDeTela.Alterar;

        public EnumTipoDeOperacaoDeTela Operacao { get; set; } = EnumTipoDeOperacaoDeTela.Exibir;

        public EnumTipoDeComparacaoDeTela TipoDeComparacao { get; set; } = EnumTipoDeComparacaoDeTela.Igual;

        [JsonIgnore]
        public override object TypeId => base.TypeId;

        #endregion

        public DnOperacaoDeCondicionalDeTelaAttribute(string propriedadeAvaliada, object valorParaODisparo)
        {
            PropriedadeAvaliada = propriedadeAvaliada;
            ValorParaODisparo = valorParaODisparo?.SerializarParaDnJson();
        }

        public DnOperacaoDeCondicionalDeTelaAttribute(string propriedadeAvaliada, object valorParaODisparo, EnumTipoDeOperacaoDeTela operacao)
        {
            PropriedadeAvaliada = propriedadeAvaliada;
            ValorParaODisparo = valorParaODisparo?.SerializarParaDnJson();
            Operacao = operacao;
        }

        public DnOperacaoDeCondicionalDeTelaAttribute(string propriedadeAvaliada, object valorParaODisparo, EnumTipoDeOperacaoDeTela operacao, EnumTipoDeEventoDeTela tipoDeDisparo)
        {
            PropriedadeAvaliada = propriedadeAvaliada;
            TipoDeDisparo = tipoDeDisparo;
            ValorParaODisparo = valorParaODisparo?.SerializarParaDnJson();
            Operacao = operacao;
        }
    }
}