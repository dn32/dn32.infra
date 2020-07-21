using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace dn32.infra
{
    [Route("/api/[controller]/[action]")]
    [ApiController]
    public class DnApiSomenteLeituraController<T> : DnApiController<T> where T : DnEntidade, new()
    {
        [NonAction]
        public override Task<DnResultadoPadrao<T>> Adicionar([FromBody] T entidade) =>
            throw new InvalidOperationException();
        [NonAction]
        public override Task<DnResultadoPadrao<T>> AdicionarOuAtualizar([FromBody] T entidade) =>
            throw new InvalidOperationException();
        [NonAction]
        public override Task<DnResultadoPadrao<T[]>> AdicionarLista([FromBody] T[] entidades) =>
            throw new InvalidOperationException();
        [NonAction]
        public override Task<DnResultadoPadrao<bool>> Atualizar([FromBody] T entidade) =>
            throw new InvalidOperationException();
        [NonAction]
        public override Task<DnResultadoPadrao<bool>> AtualizarLista([FromBody] T[] entidades) =>
            throw new InvalidOperationException();
        [NonAction]
        public override Task<DnResultadoPadrao<bool>> Remover([FromBody] T entidade) =>
            throw new InvalidOperationException();
        [NonAction]
        public override Task<DnResultadoPadrao<bool>> RemoverLista([FromBody] T[] entidades) =>
            throw new InvalidOperationException();
        [NonAction]
        public override Task<DnResultadoPadrao<bool>> EliminarTudo([FromHeader] string APAGAR_TUDO = "false") =>
            throw new InvalidOperationException();
    }
}