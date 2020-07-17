﻿using System;
using System.Threading.Tasks;
using dn32.infra;
using dn32.infra;
using Microsoft.AspNetCore.Mvc;

namespace dn32.infra {
    public partial class DnApiControlador<T> {
        [HttpGet]
        public virtual T Exemplo () => typeof (T).GetExampleValue () as T;

        [HttpGet]
        public virtual async Task<ResultadoPadrao<int>> QuantidadeTotal () =>
        await this.CrieResultadoAsync (await this.Servico.QuantidadeTotalAsync ());

        [HttpPost]
        public virtual async Task<ResultadoPadrao<int>> QuantidadePorFiltro ([FromBody] Filtro[] filtros) {
            var especificacao = this.CriarEspecificacaoDeFiltros (filtros, true);
            var quantidade = await this.Servico.QuantidadeAsync (especificacao);
            return await this.CrieResultadoAsync (quantidade);
        }

        [HttpGet]
        [Route ("/api/[controller]/EntidadeExiste")]
        public virtual async Task<ResultadoPadrao<bool>> EntidadeExisteGet ([FromQuery] T entidade) =>
        await this.CrieResultadoAsync (await this.Servico.ExisteAsync (entidade));

        [HttpPost]
        [Route ("/api/[controller]/EntidadeExiste")]
        public virtual async Task<ResultadoPadrao<bool>> EntidadeExistePost ([FromBody] T entidade) =>
        await this.CrieResultadoAsync (await this.Servico.ExisteAsync (entidade));

        [HttpGet]
        public virtual string Formulario (bool usarLayoutCompacto = false) =>
        typeof (T).GetDnJsonSchema (usarLayoutCompacto).SerializarParaDnJson ();
    }
}