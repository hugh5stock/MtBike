using Autofac;
using MintCyclingService.ServicePerson;
using MintCyclingService.Utils;
using MtBikeAdminWebApi.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;


namespace MtBikeAdminWebApi.AdminControllers
{
    /// <summary>
    /// 维修人员控制器后台
    /// </summary>
    [CheckAdminAccessCodeFilter]
    public class ServicePersonController : ApiController
    {
        private IServicePersonaService _PersonService;
        private ResultModel _adminModel = null;

        /// <summary>
        /// 维护人员构造函数
        /// </summary>
        public ServicePersonController()
        {
            _PersonService = AutoFacConfig.Container.Resolve<IServicePersonaService>();
            _adminModel = WebApiApplication.GetAdminUserData();
        }

        /// <summary>
        /// 添加或者编辑维护人员信息 complete TOM
        /// DATE：2017-02-26
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultModel AddServicePersonInfo([FromBody] ServicePerson_PM model)
        {
            if (!_adminModel.IsSuccess)
                return _adminModel;
            var st = (_adminModel.ResObject) as Guid?;
            model.AdminGuid = st ?? Guid.Empty;
            return _PersonService.AddServicePersonInfo(model);
        }

        /// <summary>
        /// 维修人员列表 complete TOM
        /// DATE：2017-02-26
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultModel GetServicePersonInfList([FromUri]SearchServicePerson_PM model)
        {
            return _PersonService.GetServicePersonInfList(model);
        }


        /// <summary>
        ///删除维护人员信息 complete TOM
        ///DATE：2017-05-26
        /// <returns></returns>
        [HttpPost]
        public ResultModel DeleteServicePersonByServiceGuid([FromBody] DeleteServicePerson_PM model)
        {
            if (!_adminModel.IsSuccess)
                return _adminModel;
            var st = (_adminModel.ResObject) as Guid?;
            model.AdminGuid = st ?? Guid.Empty;
            return _PersonService.DeleteServicePersonByServiceGuid(model);
        }



    }
}