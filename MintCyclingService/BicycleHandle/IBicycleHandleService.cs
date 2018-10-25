
using MintCyclingService.Common;
using MintCyclingService.Utils;
using System;

namespace MintCyclingService.BicycleHandle
{
    public interface IBicycleHandleService
    {
 
        /// <summary>
        /// 单车入库
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        ResultModel AddBicycleBaseInfoRK(AddBicycleBase_PM model);

        /// <summary>
        /// 根据车牌号获取锁编号
        /// </summary>
        /// <param name="BikeNumber"></param>
        /// <returns></returns>
        ResultModel GetLockNumber(string BikeNumber);

        /// <summary>
        /// 车跟锁进行匹配
        /// </summary>
        /// <returns></returns>
        ResultModel HelpComponentsBinding(ComponentsBinding data);

        /// <summary>
        /// 车辆检测
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        ResultModel BicycleDetecting(Detecting data);

        /// <summary>
        /// 检测报告列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        ResultModel GetBicycleDetecting(Detec_PM data);

    }
}