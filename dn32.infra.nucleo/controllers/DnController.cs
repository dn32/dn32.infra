﻿using Microsoft.AspNetCore.Mvc;
using System;

namespace dn32.infra
{
    public abstract class DnController<T> : DnControllerDeServico<DnServico<T>> where T : DnEntidadeBase
    {
        protected T2 CriarEspecificacao<T2>() where T2 : DnEspecificacaoBase => FabricaDeEspecificacao.Criar<T2>(this.Servico);

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