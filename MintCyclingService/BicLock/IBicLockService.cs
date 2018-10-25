using MintCyclingService.Utils;

namespace MintCyclingService.BicLock
{
    public interface IBicLockService
    {
        /// <summary>
        /// 获取生产锁管理列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        ResultModel GetLockSupplierList(BicLock_PM data);

        /// <summary>
        /// 新增或修改车辆信息
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        ResultModel AddAndUpdateBicycleBase(AddBicycleOrUpdate_PM para);

        /// <summary>
        /// 删除车辆信息
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        ResultModel DeleteBicycleByBicycleGuid(BicycleDelete_PM para);

        /// <summary>
        /// 根据查询条件搜索后台车辆列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResultModel GetBicycleBaseList(GetBicycleBaseList_PM model);

        /// <summary>
        /// 批量生成车牌号-废弃
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        ResultModel GetBicycleProduceBicNumber1(GetRandomNumModel_PM1 model);

        /// <summary>
        /// 批量生成车牌号
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        ResultModel GetBicycleProduceBicNumber(GetRandomNumModel_PM model);

        /// <summary>
        /// 根据查询条件搜索批量生成车牌号列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResultModel GetBicycleNumberListByCondition(GetBicycleNumberList_PM model);

        /// <summary>
        /// 分配客户车锁信息
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        ResultModel AddOrUpdateCustomerBicycleLock(AddCustomerBicycleLockJson_PM para);

        /// <summary>
        /// 客户车辆绑定列表
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        ResultModel CustomerBicycleLockList(CustomerBicycleLocklist_PM Model);






    }
}