using MintCyclingService.Common;
using MintCyclingService.Utils;
using System;

namespace MintCyclingService.Supplier
{
    public interface ISupplierService
    {
        /// <summary>
        /// 删除供应商信息
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        ResultModel DeleteSupplierBySupplierID(SupplierDelete_PM Model);

        /// <summary>
        /// 新增或修改供应商信息
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        ResultModel AddOrUpdateSupplier(AddSupplierOrUpdate_PM Model);
      
        /// <summary>
        /// 根据查询条件搜索供应商列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResultModel GetSupplierInfoList(GetSupplierList_PM model);
        

    }
}