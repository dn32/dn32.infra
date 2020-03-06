using dn32.infra.dados;
using dn32.infra.nucleo.atributos;
using dn32.infra.nucleo.erros_de_validacao;
using dn32.infra.nucleo.interfaces;
using dn32.infra.Nucleo.Models;
using dn32.infra.servicos;
using dn32.infra.validacoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace dn32.infra.nucleo.validacoes
{
    public class DnValidacao<T> : DnValidacaoTransacional, IDnValidacao where T : EntidadeBase
    {
        #region INTERNAL

        protected internal new DnServico<T> Servico
        {
            get => base.Servico as DnServico<T>;
            set => base.Servico = value;
        }

        public SessaoDeRequisicaoDoUsuario SessaoDaRequisicao => Servico.SessaoDaRequisicao;

        public bool ChecagemDeParametroNuloOk { get; set; } = true;

        public bool ChecagemDeChavesOk { get; set; } = true;

        #endregion

        #region VALIDAÇÃO DE COMPOSIÇÕES

        public virtual async Task AdicionarAsync(T entidade)
        {
            var metodo = typeof(DnValidacao<T>).GetMethod(nameof(AdicionarInternoAsync), BindingFlags.NonPublic | BindingFlags.Static);
            var outrosServicos = await (this).ExecuteEntityAndCompositions(entidade, metodo);
            ExecutarAsValidacoes(outrosServicos);
        }

        public virtual async Task<List<DnServicoTransacionalBase>> AdidionarOuAtualizarAsync(T entidade)
        {
            var metodo = typeof(DnValidacao<T>).GetMethod(nameof(AdicionarInternoAsync), BindingFlags.NonPublic | BindingFlags.Static);
            return await (this).ExecuteEntityAndCompositions(entidade, metodo);
        }

        public virtual async Task AtualizarAsync(T entidade)
        {
            var metodo = typeof(DnValidacao<T>).GetMethod(nameof(AtualizarListaInternoAsync), BindingFlags.NonPublic | BindingFlags.Static);
            var outrosServicos = await (this).ExecuteEntityAndCompositions(entidade, metodo);

            if (ChecagemDeChavesOk)
            {
                await this.EntityMustExistInDatabaseAsync(entidade, true);
                await this.ThereIsOnlyOneEntityAsync(entidade, false);
            }

            ExecutarAsValidacoes(outrosServicos);
        }

        public virtual async Task AtualizarListaAsync(IEnumerable<T> entidades)
        {
            this.ParameterMustBeInformed(entidades, nameof(entidades));
            var outrosServicos = new List<DnServicoTransacionalBase>();
            var metodo = typeof(DnValidacao<T>).GetMethod(nameof(AtualizarListaInternoAsync), BindingFlags.NonPublic | BindingFlags.Static);

            if (entidades != null)
            {
                foreach (var entity_ in entidades)
                {
                    var services = await (this).ExecuteEntityAndCompositions(entity_, metodo);
                    outrosServicos.AddRange(services);

                    if (ChecagemDeChavesOk)
                    {
                        await this.EntityMustExistInDatabaseAsync(entity_);
                        //Todo validate ThereIsOnlyOneEntity(entidade, false);
                        //Todo validate logical delete
                    }
                }
            }

            ExecutarAsValidacoes(outrosServicos);
        }

        public virtual async Task AdicionarListaAsync(IEnumerable<T> entidades)
        {
            this.ParameterMustBeInformed(entidades, nameof(entidades));
            var outrosServicos = new List<DnServicoTransacionalBase>();
            if (entidades != null)
            {
                var metodo = typeof(DnValidacao<T>).GetMethod(nameof(AdicionarInternoAsync), BindingFlags.NonPublic | BindingFlags.Static);

                foreach (var entidade in entidades)
                {
                    var services = await (this).ExecuteEntityAndCompositions(entidade, metodo);
                    outrosServicos.AddRange(services);
                }
            }

            ExecutarAsValidacoes(outrosServicos);
        }

        #endregion

        public virtual async Task RemoverAsync(T entidade)
        {
            this.ParameterMustBeInformed(entidade, null);
            if (entidade != null)
            {
                this.AllKeysMustBeInformed(entidade, null, null);
                await this.EntityMustExistInDatabaseAsync(entidade);
            }

            ExecutarAsValidacoes();
        }

        public virtual async Task RemoverListaAsync(T[] entidades)
        {
            this.ParameterMustBeInformed(entidades, null);

            if (entidades != null)
            {
                foreach (var entidade in entidades)
                {
                    this.ParameterMustBeInformed(entidade, null);
                    await this.EntityMustExistInDatabaseAsync(entidade);
                }
            }

            ExecutarAsValidacoes();
        }

        internal void ListaFiltrada(Filtro[] filtros)
        {
            this.ParameterMustBeInformed(filtros, nameof(filtros));
            var propriedades = typeof(T).GetProperties().ToList();

            foreach (var filtro in filtros)
            {
                var propriedade = propriedades.SingleOrDefault(x => x.Name.Equals(filtro.NomeDaPropriedade, StringComparison.InvariantCultureIgnoreCase));
                if (propriedade == null)
                {
                    AdicionarInconsistencia(new DnPropriedadeASerFiltradaNaoEncontradaErroDeValidacao(typeof(T).Name, filtro.NomeDaPropriedade));
                }
            }

            ExecutarAsValidacoes();
        }

        public virtual void Buscar(T entidade, bool checarId = true)
        {
            this.ParameterMustBeInformed(entidade, null);

            if (checarId && entidade != null)
            {
                this.AllKeysMustBeInformed(entidade, null, null);
            }

            ExecutarAsValidacoes();
        }

        public virtual void BuscarPorTermo(string termo)
        {
            if (string.IsNullOrEmpty(termo))
            {
                AdicionarInconsistencia(new DnParametroNuloErroDeValidacao(nameof(termo)));
            }

            if (!typeof(T).GetProperties().Any(x => x.GetCustomAttribute<DnBuscavelAtributo>() != null))
            {
                AdicionarInconsistencia(new DnEntidadeNaoPossuiUmaPropriedadeBuscavelErroDeValidacao(typeof(T).Name));
            }

            ExecutarAsValidacoes();
        }

        public void EliminarTudo(string APAGAR_TUDO)
        {
            if (APAGAR_TUDO?.Equals("Yes", StringComparison.InvariantCultureIgnoreCase) != true)
            {
                AdicionarInconsistencia(new DnFalhaNaRemocaoDeDadosErroDeValidacao());
            }

            ExecutarAsValidacoes();
        }

        #region INTERNAL

        internal static void AtualizarInternoAsync<T2>(IDnValidacao validacao, T2 entidade, string propriedadeDaComposicao, string campoDaComposicao) where T2 : EntidadeBase
        {
            validacao.ParameterMustBeInformed(entidade, propriedadeDaComposicao);
            validacao.DnValidateAttribute(entidade, propriedadeDaComposicao, campoDaComposicao);
            validacao.RequiredPropertyMustBeInformed(entidade, propriedadeDaComposicao, campoDaComposicao);
            validacao.MaxMinLenghtPropertyMustBeInformed(entidade, propriedadeDaComposicao, campoDaComposicao);
            validacao.AllKeysShouldBeInformedWhenThereAreMoreThanOne(entidade, propriedadeDaComposicao, campoDaComposicao);
        }

        internal static async Task AdicionarInternoAsync<T2>(IDnValidacao validacao, T2 entidade, string propriedadeDaComposicao, string campoDaComposicao) where T2 : EntidadeBase
        {
            validacao.ParameterMustBeInformed(entidade, propriedadeDaComposicao);
            validacao.DnValidateAttribute(entidade, propriedadeDaComposicao, campoDaComposicao);
            validacao.RequiredPropertyMustBeInformed(entidade, propriedadeDaComposicao, campoDaComposicao);
            validacao.MaxMinLenghtPropertyMustBeInformed(entidade, propriedadeDaComposicao, campoDaComposicao);
            validacao.AllKeysShouldBeInformedWhenThereAreMoreThanOne(entidade, propriedadeDaComposicao, campoDaComposicao);

            if (validacao.ChecagemDeChavesOk)
            {
                await validacao.EntityShouldNotExistInDatabaseBasedOnKeysAsync(entidade, false);
            }
        }

        internal static void AtualizarListaInternoAsync<T2>(IDnValidacao validacao, T2 entidade, string propriedadeDaComposicao, string campoDaComposicao) where T2 : EntidadeBase
        {
            validacao.ParameterMustBeInformed(entidade, propriedadeDaComposicao);
            validacao.DnValidateAttribute(entidade, propriedadeDaComposicao, campoDaComposicao);
            validacao.RequiredPropertyMustBeInformed(entidade, propriedadeDaComposicao, campoDaComposicao);
            validacao.MaxMinLenghtPropertyMustBeInformed(entidade, propriedadeDaComposicao, campoDaComposicao);
            validacao.AllKeysShouldBeInformedWhenThereAreMoreThanOne(entidade, propriedadeDaComposicao, campoDaComposicao, isUpdate: true);
        }

        #endregion
    }
}
