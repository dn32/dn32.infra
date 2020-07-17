using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;

namespace dn32.infra
{
    public class DnDadosDeEntidadeAlterada
    {
        public List<DnDadosDePropriedadeAlterada> Propriedades { get; set; }
        public object EntidadeAtual { get; set; }
        public Type TipoDaEntidadeAtual { get; set; }
        public EntityEntry EntradaDeEntidade { get; set; }
        public EntityState EstadoDaEntidade { get; set; }
    }
}