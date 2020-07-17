using System;
using dn32.infra;
using dn32.infra;
using dn32.infra;
using dn32.infra;
using Microsoft.AspNetCore.Mvc;

namespace dn32.infra {
    public abstract class DnControlador<T> : DnControladorDeServico<DnServico<T>> where T : EntidadeBase {
        protected T2 CriarEspecificacao<T2> () where T2 : DnEspecificacaoBase => FabricaDeEspecificacao.Criar<T2> (this.Servico);

        [NonAction]
        protected override void Dispose (bool finalizando) => base.Dispose (finalizando);

        [NonAction]
        public new Type GetType () => base.GetType ();

        [NonAction]
        public override string ToString () => base.ToString ();

        [NonAction]
        public override bool Equals (object obj) => base.Equals (obj);

        [NonAction]
        public override int GetHashCode () => base.GetHashCode ();
    }
}