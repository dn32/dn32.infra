using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;





namespace dn32.infra
{
    public class DnTermoEspecificacao<T> : DnEspecificacao<T> where T : DnEntidade
    {
        private string Termo { get; set; }

        public bool EhListagem { get; set; }

        public DnTermoEspecificacao<T> SetParameter(string termo, bool ehListagem)
        {
            Termo = termo;
            EhListagem = ehListagem;
            return this;
        }

        public override IQueryable<T> Where(IQueryable<T> query)
        {
            var expression = ConverterTermoParaExpressao(Termo);

            return query
                .Where(expression)
                .ObterInclusoes(EhListagem)
                .ProjetarDeFormaDinamica(Servico);
        }

        public override IOrderedQueryable<T> Order(IQueryable<T> query) =>
            query.ProjetarDeFormaDinamicaOrdenada(Servico);

        private Expression<Func<T, bool>> ConverterTermoParaExpressao(string termo)
        {
            Expression<Func<T, bool>> expressaoCompleta = null;

            if (!string.IsNullOrEmpty(termo))
            {
                var propriedades = typeof(T).GetProperties().Where(x => x.GetCustomAttribute<DnBuscavelAttribute>() != null).ToArray();
                foreach (var propriedade in propriedades)
                {
                    var expressao = DnExpressoesExtensao.EhNulo<T>(propriedade.Name).Not();
                    expressao = expressao.And(DnExpressoesExtensao.Contem<T>(propriedade.Name, termo, propriedade.PropertyType));
                    expressaoCompleta = expressaoCompleta == null ? expressao : expressaoCompleta.Or(expressao);
                }
            }

            return expressaoCompleta ?? (x => true);
        }
    }
}