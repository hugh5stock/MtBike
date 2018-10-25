using MintCyclingService.Utils;
using System;

namespace MintCyclingService.Cycling
{
    public interface ICyclingService
    {
        /// <summary>
        /// 根据输入的地址搜索半径两公里范围内的车辆、电子围栏
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        ResultModel GetNearBicycleEnclosure(NearBicycleEnclosure_PM para);

        ///// <summary>
        ///// 查询指定地点的车辆信息
        ///// </summary>
        ///// <param name="para"></param>
        ///// <returns></returns>
        //ResultModel GetBicycleListByCoordinate(CoordinateBicycleList_PM para);

        /// <summary>
        /// 根据编码开锁
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        ResultModel UnLockBicycle(UnLockBicycle_PM para);

        /// <summary>
        /// 服务器查询开锁状态
        /// </summary>
        /// <param name="deviceNo"></param>
        /// <returns></returns>
        ResultModel QueryUnLockBicycleStatus(string deviceNo);

        /// <summary>
        /// 新增锁信息-提供给硬件锁调用
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        ResultModel AddBicycleLockInfo(AddBicycleHardWare_PM Model);

        /// <summary>
        /// 维修人员开关锁
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        ResultModel WxOpenOrClosekBicycleLock(OpenOrCloseLockBicycle_PM para);

        /// <summary>
        /// 上传交易记录-手机蓝牙或者硬件锁上传
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        ResultModel OpenOrCloseBicycleLockData(OpenOrCloseLockBicycle_PM para);

        /// <summary>
        /// 查询是否停放在电子围栏区域
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        ResultModel QueryBicycleIsinRange(BicycleIsinRange_PM para);

        /// <summary>
        /// 查询指定车辆状态
        /// </summary>
        /// <param name="deviceNo"></param>
        /// <returns></returns>
        ResultModel GetLockStatus(string deviceNo);

        /// <summary>
        /// 查看单车是否可用
        /// </summary>
        /// <param name="deviceNo"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        ResultModel CheckBicycleAvailable(string deviceNo, Guid userInfoGuid);

        /// <summary>
        /// 根据车编号查询车辆类型
        /// </summary>
        /// <param name="BicycleNumber"></param>
        /// <returns></returns>
        ResultModel GetBicycleTypeNameByBicycleNumber(string BicycleNumber);

        /// <summary>
        /// 根据车辆编号或者锁编号查询匹配信息
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        bool GetBicycleLockByNumber(string Number);

        /// <summary>
        /// 根绝锁编号查询车辆类型
        /// </summary>
        /// <param name="LockNumber"></param>
        /// <returns></returns>
        int GetBicycleTypeNameBy(string LockNumber);

        /// <summary>
        /// 查看单车是否可用--用于添加预约用车
        /// </summary>
        /// <param name="deviceNo"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        ResultModel CheckBicycle(string deviceNo);

        /// <summary>
        /// 添加预约
        /// </summary>
        /// <param name="model"></param>
        ResultModel AddReservation(AddRegionInfo_PM model);

        /// <summary>
        /// 处理某用户预约超时，如果预约超时20分钟后，修改预约状态为2已结束
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResultModel UserReservationOvertime(Guid userInfoGuid);

        /// <summary>
        /// 获取用户的预约倒计时信息
        /// </summary>
        /// <param name="model"></param>
        ResultModel GetCountDownTimeByUserGuid(Guid? userInfoGuid);

        /// <summary>
        /// 是否骑行中
        /// </summary>
        /// <param name="reservationGuid"></param>
        ResultModel GetIsRidingByUserGuid(Guid? userInfoGuid);

        /// <summary>
        /// 检查预约是否存在
        /// </summary>
        /// <param name="reservationGuid"></param>
        /// <returns></returns>
        bool CheckReservationExist(Guid reservationGuid);

        /// <summary>
        /// 判断输入车辆编号或者扫码开锁
        /// </summary>
        /// <param name="model"></param>
        ResultModel CheckInputOrScan(InputOrScan_PM model);

        /// <summary>
        /// 取消预约
        /// </summary>
        /// <param name="reservationGuid"></param>
        ResultModel CancelReservation(UpdateReservation_PM model);

        /// <summary>
        /// 如果倒计时20分钟后，APP调用此接口修改预约状态为已结束
        /// </summary>
        /// <param name="model"></param>
        ResultModel ReservationOvertimeUpdateStatus(UpdateOvertime_PM model);

        /// <summary>
        /// 检查预约用户是否已经有其他预约
        /// </summary>
        /// <param name="userGuid"></param>
        /// <returns></returns>
        bool CheckUserReservationExist(Guid userGuid);

        /// <summary>
        /// 用户预约的次数
        /// </summary>
        /// <param name="userGuid"></param>
        /// <returns></returns>
        int GetCountByUserInfoGuid(Guid userInfoGuid);

        /// <summary>
        /// 电子围栏中的车辆数
        /// </summary>
        /// <param name="ElectronicFenCingGuid"></param>
        /// <returns></returns>
        int GetBicCountByElectronicGuid(Guid ElectronicFenCingGuid);

        /// <summary>
        /// 计算两个时间的差值,取分钟
        /// </summary>
        /// <returns></returns>
        int GetTotalMinutes(DateTime? dt);

        /// 测试模拟硬件开关锁--用极光推送
        ResultModel HardwareOpenOrClosekLock_Test(Test_OpenOrCloseLockBicycle_PM para);

        /// <summary>
        /// 查询车辆是否已被预约
        /// </summary>
        /// <param name="DeviceNo"></param>
        /// <returns></returns>
        ResultModel GetIsUserReservation(string DeviceNo, Guid UserGuid);

        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        ResultModel GetEncryptionKey(GetEncyptionKey_PM para);

        /// <summary>
        /// 通过蓝牙关锁判断当前车辆是否在电子围栏内
        /// 如果不在，不用更新我的行程表中的EndTime时间，如果在则更新EndTime，同时更新车辆表中的ElectronicFenCingGuid和IsInElectronicFenCing两个字段
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        BicycleOrEnclosureModel GetBicycleIsElectronicFenCing(decimal CurLongitude, decimal CurLatitude, decimal Radius, string DeviceNo);

        /// <summary>
        /// 服务器端处理预约超过20分钟的数据
        /// 把status修改为2已结束
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        ResultModel ReservationTimedTask();
    }
}