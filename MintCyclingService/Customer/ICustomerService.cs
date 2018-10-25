using MintCyclingService.Common;
using MintCyclingService.Utils;
using System;

namespace MintCyclingService.Customer
{
    public interface ICustomerService
    {
        /// <summary>
        /// 删除客户信息
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        ResultModel DeleteCustomerByCustomerID(CustomerDelete_PM Model);

        /// <summary>
        /// 新增或修改客户信息
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        ResultModel AddOrUpdateCustomer(AddCustomerOrUpdate_PM Model);

        /// <summary>
        /// 根据查询条件搜索客户列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResultModel GetCustomerInfoList(GetCustomerList_PM model);



        /// <summary>
        /// 查询客户列表
        /// </summary>
        /// <returns></returns>
        ResultModel GetCustomerInfo(GetCustomerList_PM data);



        /// <summary>
        /// 查询客户列表
        /// </summary>
        /// <returns></returns>
        ResultModel GetCustomerList();

    }
}