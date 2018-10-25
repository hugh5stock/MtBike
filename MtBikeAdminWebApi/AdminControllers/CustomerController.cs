using Autofac;
using MintCyclingService.ChargingRules;
using MintCyclingService.Customer;
using MintCyclingService.Cycling;
using MintCyclingService.Supplier;
using MintCyclingService.Utils;
using MtBikeAdminWebApi.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MtBikeAdminWebApi.AdminControllers
{
    /// <summary>
    /// 客户管理控制器
    /// </summary>
    [CheckAdminAccessCodeFilter]
    public class CustomerController : ApiController
    {
        ICustomerService _CustomerService;
        /// <summary>
        /// 初始化单车控制器
        /// </summary>
        public CustomerController()
        {
            _CustomerService = AutoFacConfig.Container.Resolve<ICustomerService>();

        }
 
        /// <summary>
        ///添加或者修改客户信息 complete TOM
        ///DATE：2017-05-31
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ResultModel AddOrUpdateCustomer([FromBody] AddCustomerOrUpdate_PM model)
        {
            return _CustomerService.AddOrUpdateCustomer(model);
        }

        /// <summary>
        ///删除客户信息 complete TOM
        ///DATE：2017-05-31
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ResultModel DeleteCustomerByCustomerID([FromBody] CustomerDelete_PM model)
        {
            return _CustomerService.DeleteCustomerByCustomerID(model);
        }

        /// <summary>
        /// 根据查询条件搜索客户列表  complete TOM
        ///DATE：2017-05-31
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultModel GetCustomerInfoList([FromUri]GetCustomerList_PM model)
        {
            return _CustomerService.GetCustomerInfoList(model);
        }


        /// <summary>
        /// 查询客户列表信息  complete TOM
        ///DATE：2017-06-20
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ResultModel GetCustomerList([FromUri]GetCustomerList_PM data)
        {
            return _CustomerService.GetCustomerInfo(data);  
        }


        /// <summary>
        /// 客户列表 complete TOM
        /// DATE：2017-06-21
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ResultModel GetCustomerInfo()
        {
            return _CustomerService.GetCustomerList();
        }


    }
}