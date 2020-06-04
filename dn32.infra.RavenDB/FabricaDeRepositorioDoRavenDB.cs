﻿using dn32.infra.nucleo.repositorios;
using System;

namespace dn32.infra.RavenDB
{
    /// <summary>
    /// Classe interna. Nunca a deixe pública, pois o acesso a um repositório à partir de um serviço terceiro não deve ser permitido.
    /// A fábrica de repositórios.
    /// </summary>
    /// <typeparam Nome="T">
    ///  O tipo da entidade do repositório a ser criado.
    /// </typeparam>
    internal class FabricaDeRepositorioDoRavenDB : FrabricaDeRepositorioBase
    {
        protected override Type ObterTipoDeRepositorioPadrao<T>() => typeof(DnRavenDbRepositorio<RavenDBEntidadeBase>);
    }
}