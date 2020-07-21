using dn32.infra.Mock;
using dn32.infra.Mock.ControllerMock;
using Newtonsoft.Json;
using System;

namespace dn32.infra
{
    public class DnTest<TModel> where TModel : DnEntidade, new()
    {
        public virtual DnApiController<TModel> GetNewController()
        {
            return MockUtil.GetMockController<DnApiController<TModel>>();
        }

        public virtual TController GetNewController<TController>() where TController : DnApiController<TModel>
        {
            return MockUtil.GetMockController<TController>();
        }

        public virtual bool Remove(TModel model)
        {
            var newController = GetNewController();
            return TestUtil.Execute<DnApiController<TModel>, bool>(newController, (DnApiController<TModel> controller) => controller.Remover(model).Result);
        }

        public virtual TModel Add(TModel model)
        {
            return Execute((DnApiController<TModel> controller) => controller.Adicionar(model).Result);
        }

        //public virtual string Schema()
        //{
        //    var newController = GetNewController();
        //    return TestUtil.Execute(newController, (DnApiControlador<TModel> controller) => controller.Schema()) as string;
        //}

        public virtual bool Update(TModel model)
        {
            var newController = GetNewController();
            return TestUtil.Execute<DnApiController<TModel>, bool>(newController, (DnApiController<TModel> controller) => controller.Atualizar(model).Result);
        }

        public virtual TModel Add()
        {
            return Execute((DnApiController<TModel> controller) => controller.Adicionar(GetNew()).Result);
        }

        public virtual bool Exists(TModel model)
        {
            var newController = GetNewController();
            return TestUtil.Execute<DnApiController<TModel>, bool>(newController, (DnApiController<TModel> controller) => controller.EntidadeExisteGet(model).Result);
        }

        public virtual TModel[] AddRange(TModel[] models)
        {
            var newController = GetNewController();
            return TestUtil.Execute<DnApiController<TModel>, TModel[]>(newController, (DnApiController<TModel> controller) => controller.AdicionarLista(models).Result);
        }

        public virtual TModel[] List()
        {
            var newController = GetNewController();
            return TestUtil.Execute<DnApiController<TModel>, TModel[]>(newController, (DnApiController<TModel> controller) => controller.Listar().Result);
        }

        public virtual TModel[] List(DnFiltro[] filters)
        {
            var newController = GetNewController();
            return TestUtil.Execute<DnApiController<TModel>, TModel[]>(newController, (DnApiController<TModel> controller) => controller.ListarPorFiltroGet(filters).Result);
        }
        public virtual int Count()
        {
            var newController = GetNewController();
            return TestUtil.Execute<DnApiController<TModel>, int>(newController, (DnApiController<TModel> controller) => controller.QuantidadeTotal().Result);
        }

        public virtual int Count(DnFiltro[] filters)
        {
            var newController = GetNewController();
            return TestUtil.Execute<DnApiController<TModel>, int>(newController, (DnApiController<TModel> controller) => controller.QuantidadePorFiltro(filters).Result);
        }

        public virtual bool UpdateRange(TModel[] models)
        {
            var newController = GetNewController();
            return TestUtil.Execute<DnApiController<TModel>, bool>(newController, (DnApiController<TModel> controller) => controller.AtualizarLista(models).Result);
        }

        public virtual TModel Find(TModel model)
        {
            return Execute((DnApiController<TModel> controller) => controller.BuscarPorEntidadePost(model).Result);
        }

        public virtual bool RemoverLista(TModel[] models)
        {
            var newController = GetNewController();
            return TestUtil.Execute<DnApiController<TModel>, bool>(newController, (DnApiController<TModel> controller) => controller.RemoverLista(models).Result);
        }

        public virtual bool Truncate(string APAGAR_TUDO = "no")
        {
            var newController = GetNewController();
            return TestUtil.Execute<DnApiController<TModel>, bool>(newController, (DnApiController<TModel> controller) => controller.EliminarTudo(APAGAR_TUDO).Result);
        }

        public virtual TModel GetNew()
        {
            throw new NotImplementedException();
            //   return new DnApiControlador<TModel>().ExampleData();
        }

        public virtual TModel Execute(Func<DnApiController<TModel>, DnResultadoPadrao<TModel>> actionMethod)
        {
            var newController = GetNewController();
            newController.OnActionExecuting(MockActionExecutingContextFactory.Create(newController));
            var ret = actionMethod(newController);
            newController.OnActionExecuted(MockActionExecutedContextFactory.Create(newController));
            return JsonConvert.DeserializeObject<TModel>(JsonConvert.SerializeObject(ret.Dados));
        }
    }
}