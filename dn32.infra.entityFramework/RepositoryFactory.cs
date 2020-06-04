﻿using dn32.infra.dados;
using dn32.infra.nucleo.repositorios;
using System;

namespace dn32.infra.EntityFramework
{
    /// <summary>
    /// Classe interna. Nunca a deixe pública, pois o acesso a um repositório à partir de um serviço terceiro não deve ser permitido.
    /// A fábrica de repositórios.
    /// </summary>
    /// <typeparam Nome="T">
    ///  O tipo da entidade do repositório a ser criado.
    /// </typeparam>
    internal class RepositoryFactory : FrabricaDeRepositorioBase
    {
        protected override Type ObterTipoDeRepositorioPadrao<T>() => typeof(DnEFRepository<T>);
    }
}