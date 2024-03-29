﻿using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace dn32.infra
{
    [Route("/api/[controller]/[action]")]
    [ApiController] // Nunca defina como abstrato, pois o controle de log espera essa classe como concreta
    public partial class DnApiController<T> : DnController<T> where T : DnEntidade, new()
    {
        [HttpPost]
        public virtual async Task<DnResultadoPadrao<T>> Adicionar([FromBody] T entidade)
        {
            return await this.CrieResultadoAsync(await this.Servico.AdicionarAsync(entidade));
        }

        [HttpPost]
        public virtual async Task<DnResultadoPadrao<T>> AdicionarOuAtualizar([FromBody] T entidade)
        {
            return await this.CrieResultadoAsync(await this.Servico.AdidionarOuAtualizarAsync(entidade));
        }

        [HttpPost]
        public virtual async Task<DnResultadoPadrao<T[]>> AdicionarLista([FromBody] T[] entidades)
        {
            await this.Servico.AdicionarListaAsync(entidades);
            return await this.CrieResultadoAsync(entidades);
        }

        [HttpPut]
        public virtual async Task<DnResultadoPadrao<bool>> Atualizar([FromBody] T entidade)
        {
            await this.Servico.AtualizarAsync(entidade);
            return await this.CrieResultadoAsync<bool>(true);
        }

        [HttpPut]
        public virtual async Task<DnResultadoPadrao<bool>> AtualizarLista([FromBody] T[] entidades)
        {
            await this.Servico.AtualizarListaAsync(entidades);
            return await this.CrieResultadoAsync(true);
        }

        [HttpPost]
        [HttpDelete]
        public virtual async Task<DnResultadoPadrao<bool>> Remover([FromBody] T entidade)
        {
            await this.Servico.RemoverAsync(entidade);
            return await this.CrieResultadoAsync(true);
        }

        [HttpPost]
        [HttpDelete]
        public virtual async Task<DnResultadoPadrao<bool>> RemoverLista([FromBody] T[] entidades)
        {
            await this.Servico.RemoverListaAsync(entidades);
            return await this.CrieResultadoAsync<bool>(true);
        }

        [HttpDelete]
        public virtual async Task<DnResultadoPadrao<bool>> EliminarTudo([FromHeader] string APAGAR_TUDO = "false")
        {
            await this.Servico.EliminarTudoAsync(APAGAR_TUDO);
            return await this.CrieResultadoAsync(true);
        }
    }
}