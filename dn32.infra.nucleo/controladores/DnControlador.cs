using dn32.infra.dados;
using dn32.infra.Factory;
using dn32.infra.nucleo.especificacoes;
using dn32.infra.servicos;
using Microsoft.AspNetCore.Mvc;
using System;

namespace dn32.infra.nucleo.controladores
{
    public abstract class DnControlador<T> : DnControladorDeServico<DnServico<T>> where T : EntidadeBase
    {
        protected T2 CriarEspecificacao<T2>() where T2 : DnEspecificacaoBase
            => SpecFactory.Criar<T2>(this.Servico);

        [NonAction]
        protected override void Dispose(bool finalizando) => base.Dispose(finalizando);

        [NonAction]
        public new Type GetType() => base.GetType();

        [NonAction]
        public override string ToString() => base.ToString();


        [NonAction]
        public override bool Equals(object obj) => base.Equals(obj);

        [NonAction]
        public override int GetHashCode() => base.GetHashCode();
    }
}

