using MintCyclingService.BicycleHandle;
using MintCyclingService.Utils;

namespace MintCyclingService.Remote
{
    public interface IRemoteService
    {
        /// <summary>
        /// 发送远程控制骑行模式
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        ResultModel SendHandleCyclingModeByLockNumber(HandleCyclingMode_PM Model);

        /// <summary>
        /// 接受远程控制骑行模式
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        ResultModel ReceiveUpdateCylingModeByLockNumber(HandleCyclingMode_PM Model);

        /// <summary>
        /// 发送查询控制器与BMS序列号
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        ResultModel SendSearchControllerBMS(SearchControllerBMS_PM Model);

        /// <summary>
        /// 接受查询控制器与BMS序列号
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        ResultModel ReceiveSearchControllerBMS(ReceiveSearchControllerBMS_PM Model);

        /// <summary>
        /// 接受远程升级锁程序包完成后锁的应答
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResultModel ReceiveUpgradeFinish(ReceiveUpgradeFinish_PM model);


        /// <summary>
        /// 控制器上报信息
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        ResultModel HardWareControllerReport(ControllerReport_PM Model);

        /// <summary>
        /// BMS上报信息
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        ResultModel HardWareBMSReport(BMSReport_PM Model);

        /// <summary>
        /// 锁自动上报信息
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        ResultModel HardWareAutoUpadate(HardWareAuto_PM Model);

        /// <summary>
        /// 控制器和电池组上报信息
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        ResultModel HardWareHelpUpadate(ComponentsBinding Model);


        /// <summary>
        /// 发送远程升级锁程序包
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        ResultModel SendUpgradeBicLockProgram(RemoteUpgradeBicLockProgram_PM Model);


    }
}