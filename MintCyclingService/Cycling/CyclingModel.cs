using MintCyclingService.Common;
using System;
using System.Collections.Generic;

namespace MintCyclingService.Cycling
{
    /// <summary>
    /// 半径两公里范围内的车辆电子围栏 参数模型
    /// </summary>
    public class NearBicycleEnclosure_PM
    {
        /// <summary>
        /// 查询的经度
        /// </summary>
        public decimal CurLongitude { get; set; }

        /// <summary>
        /// 查询的纬度
        /// </summary>
        public decimal CurLatitude { get; set; }

        /// <summary>
        /// 查询的半径
        /// </summary>
        public decimal? Radius { get; set; }

        ///// <summary>
        ///// 车辆类型：0非助力车；1助力车;2全部;
        ///// </summary>
        //public int BicyCleTypeName { get; set; }

    }

    /// <summary>
    /// App地图半径两公里范围内的车辆电子围栏 输出模型
    /// </summary>
    public class NearBicycleEnclosure_OM
    {
        /// <summary>
        /// 地图电子围栏列表
        /// </summary>
        public List<MapEnclosureModel> EnclosureList { get; set; }

        /// <summary>
        /// 地图中的非助力车辆列表
        /// </summary>
        public List<MapBicycleModel> BicycleList { get; set; }

        /// <summary>
        /// 地图中的助力车辆列表
        /// </summary>
        public List<MapHelpBicycleModel>  HelpBicycleList { get; set; }


        /// <summary>
        /// 地图中显示全部车辆
        /// </summary>
        //public List<MapHelpBicycleModel> ALLBicycleList { get; set; }


    }

    /// <summary>
    /// 车辆数据 输出模型
    /// </summary>
    public class BicycleData_OM
    {
        /// <summary>
        /// 车辆编号
        /// </summary>
        public string BicycleNo { get; set; }
    }

    /// <summary>
    /// APP地图输出模型
    /// </summary>
    public class NearBicycleEnclosureMap_OM
    {
        /// <summary>
        /// APP地图电子围栏列表
        /// </summary>
        public List<MapEnclosureModel> EnclosureList { get; set; }

        /// <summary>
        /// APP地图车辆列表
        /// </summary>
        public List<MapBicycleModel> BicycleList { get; set; }

 

        /// <summary>
        /// 电子围栏的集合数量
        /// </summary>
        public int Totalcount { get; set; }

       

    }



    /// <summary>
    /// 预约用车成功 输出模型
    /// </summary>
    public class ReservationBicycleSucess_OM
    {
        /// <summary>
        /// 预约Guid编号
        /// </summary>
        public Guid ReservationGuid { get; set; }

        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceNo { get; set; }
 
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }

        ///// <summary>
        ///// 预约时间
        ///// </summary>
        //public string StartTme { get; set; }

        /// <summary>
        /// 预约中保留时间为20分钟
        /// </summary>
        public int  CountDownTime { get; set; }

        /// <summary>
        /// 详细地址-通过百度API接口转换
        /// </summary>
        public string Address { get; set; }
 
        /// <summary>
        /// 单车经度
        /// </summary>
        public decimal? BicycleLongitude { get; set; }

        /// <summary>
        /// 单车纬度
        /// </summary>
        public decimal? BicycleLatitude { get; set; }


        /// <summary>
        /// 车辆类型：0非助力车；1助力车
        /// </summary>
        public string BicycleTypeName { get; set; }


        ///// <summary>
        ///// 电量-按小数
        ///// </summary>
        //public decimal ElectricQuantity { get; set; }


        /// <summary>
        /// 用1表示低电量；2电量正常；3电量充足
        /// </summary>
        public int ElectricQuantityStatus { get; set; }

        /// <summary>
        /// 电量描述
        /// </summary>
        public string ElectricQuantityDesc { get; set; }

        ///// <summary>
        ///// 电量充足时预约返回助力的距离
        ///// </summary>
        //public string DistanceDesc { get; set; }
        

    }

    /// <summary>
    /// 判断电量的范围实体
    /// </summary>
    public class BicElectricModel
    {

        /// <summary>
        /// 用1表示低电量；2电量正常；3电量充足
        /// </summary>
        public int ElectricQuantityStatus { get; set; }


        /// <summary>
        /// 电量描述
        /// </summary>
        public string ElectricQuantityDesc { get; set; }

    }


    /// <summary>
    /// 是否骑行中  输出模型
    /// </summary>
    public class IsRidingModel_OM
    {
        /// <summary>
        /// 查询用户开关锁的设备编号
        /// </summary>
        public string OpenOrCloseDeviceNo { get; set; }
        /// <summary>
        ///  骑行模式:0表示关锁时传骑行模式值为0；1非助力车模式(普通车类型)；助力车模式：2助力模式；3普通模式(非助力模式)；4锻炼模式
        /// </summary>

        public int? CyclingMode { get; set; }

        /// <summary>
        /// 电量
        /// </summary>
        public  decimal ElectricQuantity { get; set; }

    }



    /// <summary>
    /// 状态 输出模型
    /// </summary>
    public class Status_OM
    {
        /// <summary>
        /// 单车状态
        /// </summary>
        public int? statusB { get; set; }

        /// <summary>
        /// 预约状态
        /// </summary>
        public int? statusR { get; set; }

    }



    /// <summary>
    /// 地图电子围栏模型
    /// </summary>
    public class MapEnclosureModel
    {
        /// <summary>
        /// 电子围栏Guid
        /// </summary>
        public Guid EnclosureGuid { get; set; }

        /// <summary>
        /// 电子围栏编号[暂时废弃]
        /// </summary>
        public string ElectronicFenCingNumber { get; set; }

        
        /// <summary>
        /// 电子围栏的经度
        /// </summary>
        public decimal? EnclosureLongitude { get; set; }

        /// <summary>
        /// 电子围栏的纬度
        /// </summary>
        public decimal? EnclosureLatitude { get; set; }

        /// <summary>
        /// 电子围栏里非助力车的数量
        /// </summary>
        public int BicycleNum { get; set; }

        /// <summary>
        /// 电子围栏里助力车的数量
        /// </summary>
        public int HelpBicycleNum { get; set; }

        /// <summary>
        /// 电子围栏里车辆类型：0非助力车;
        /// </summary>
        public string FZLCleTypeName { get; set; }

        /// <summary>
        /// 电子围栏里车辆类型：1助力车
        /// </summary>
        public string HelpCleTypeName { get; set; }

 
        /// <summary>
        /// 设备编号[新增字段-暂时没用]
        /// </summary>
        public string DeviceNo { get; set; }

        /// <summary>
        /// 默认0.5元/30
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 距离：单位M
        /// </summary>
        public double Distance { get; set; }


        /// <summary>
        /// 步行时间：分钟
        /// </summary>
        public double Mintues { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        public string Address { get; set; }



    }


    public class BicycleOrEnclosureModel
    {
        /// <summary>
        /// 电子围栏Guid
        /// </summary>
        public Guid EnclosureGuid { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceNo { get; set; }
    }


    /// <summary>
    /// APP地图电子围栏外非助力车辆模型
    /// </summary>
    public class MapBicycleModel
    {
        /// <summary>
        /// 电子围栏外单车经度
        /// </summary>
        public decimal? BicycleLongitude { get; set; }

        /// <summary>
        /// 电子围栏外单车纬度
        /// </summary>
        public decimal? BicycleLatitude { get; set; }

        /// <summary>
        /// 设备编号[新增字段]
        /// </summary>
        public string DeviceNo { get; set; }

        //新增字段
        /// <summary>
        /// 默认0.5元/30
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 距离：单位M
        /// </summary>
        public double Distance { get; set; }


        /// <summary>
        /// 步行时间：分钟
        /// </summary>
        public double Mintues { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        public string Address { get; set; }

        ///// <summary>
        ///// 获取输出参数距离和时间
        ///// </summary>
        //public string Distance_TimeParam { get; set; }

    }


    /// <summary>
    /// APP地图电子围栏外助力车辆模型
    /// </summary>
    public class MapHelpBicycleModel
    {
        /// <summary>
        /// 电子围栏外单车经度
        /// </summary>
        public decimal? BicycleLongitude { get; set; }

        /// <summary>
        /// 电子围栏外单车纬度
        /// </summary>
        public decimal? BicycleLatitude { get; set; }

        /// <summary>
        /// 设备编号[新增字段]
        /// </summary>
        public string DeviceNo { get; set; }

        //新增字段
        /// <summary>
        /// 默认0.5元/30
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 距离：单位M
        /// </summary>
        public double Distance { get; set; }


        /// <summary>
        /// 步行时间：分钟
        /// </summary>
        public double Mintues { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        public string Address { get; set; }

 

    }


    /// <summary>
    /// 指定地点的车辆信息 参数模型
    /// </summary>
    public class CoordinateBicycleList_PM
    {
        /// <summary>
        /// 电子围栏Guid
        /// </summary>
        public Guid? EnclosureGuid { get; set; }

        /// <summary>
        /// 电子围栏外的经度
        /// </summary>
        public decimal? EnclosureLongitude { get; set; }

        /// <summary>
        /// 电子围栏外的纬度
        /// </summary>
        public decimal? EnclosureLatitude { get; set; }
    }

    /// <summary>
    /// 指定地点的车辆信息 输出模型
    /// </summary>
    public class CoordinateBicycleList_OM
    {
        /// <summary>
        /// 选择区域的地址
        /// </summary>
        public string DetailAddress { get; set; }

        /// <summary>
        /// 选择区域的单车数量
        /// </summary>
        public int BicycleNum { get; set; }

        /// <summary>
        /// 当前区域车辆列表
        /// </summary>
        public List<CurRegionBicycleModel> CurRegionBicycleList { get; set; }
    }

    /// <summary>
    /// 当前区域车辆模型
    /// </summary>
    public class CurRegionBicycleModel
    {
        /// <summary>
        /// 车辆类型
        /// </summary>
        public string BicyCleTypeName { get; set; }

        /// <summary>
        /// 车辆数
        /// </summary>
        public int RegionBicycleNum { get; set; }
    }

    /// <summary>
    /// 根据编码开锁 参数模型
    /// </summary>
    public class UnLockBicycle_PM
    {
        /// <summary>
        /// 单车锁编码
        /// </summary>
        public string LockNumber { get; set; }

        /// <summary>
        /// 开锁位置的经度
        /// </summary>
        public decimal? Longitude { get; set; }

        /// <summary>
        /// 开锁位置的纬度
        /// </summary>
        public decimal? Latitude { get; set; }

        /// <summary>
        /// 用户Guid
        /// </summary>
        public Guid UserInfoGuid { get; set; }

        /// <summary>
        /// 锁密钥
        /// </summary>
        public string SecretKey { get; set; }
        
    }

    /// <summary>
    /// 添加预约用车输入 参数模型
    /// </summary>
    public class AddRegionInfo_PM
    {

        /// <summary>
        /// 电子围栏Guid
        /// </summary>
        public Guid ElectronicFenCingGuid { get; set; }
        /// <summary>
        /// 预约者
        /// </summary>
        public Guid UserGuid { get; set; }

        /// <summary>
        /// 锁的编号
        /// </summary>
        public string DeviceNo { get; set; }

        ///// <summary>
        ///// 车辆编号
        ///// </summary>
        //public string BicycleNumber { get; set; }

        /// <summary>
        /// 类型0表示电子围栏内，1电子围栏外
        /// </summary>
        public int TypeName { get; set; }

        /// <summary>
        /// 当前的经度
        /// </summary>
        public decimal Longitude { get; set; }

        /// <summary>
        /// 当前的纬度
        /// </summary>
        public decimal Latitude { get; set; }


        /// <summary>
        /// 车辆类型：0非助力车；1助力车
        /// </summary>
        public int BicyCleTypeName { get; set; }


    }

    /// <summary>
    /// 输入车辆编号或者扫码输入 参数模型
    /// </summary>
    public class InputOrScan_PM
    {
        /// <summary>
        /// 用户Guid
        /// </summary>
        public Guid UserGuid { get; set; }

        /// <summary>
        /// 锁的编号
        /// </summary>
        public string DeviceNo { get; set; }
    }


        /// <summary>
        /// 取消预约输入 参数模型
        /// </summary>
     public class UpdateReservation_PM
    {
        /// <summary>
        /// 预约者
        /// </summary>
        public Guid UserGuid { get; set; }

        /// <summary>
        /// 预约的Guid
        /// </summary>
        public Guid ReservationGuid { get; set; }

    }


    /// <summary>
    ///预约用车倒计时20分钟后 输入参数模型
    /// </summary>
    public class UpdateOvertime_PM
    {
        /// <summary>
        /// 预约者
        /// </summary>
        public Guid UserGuid { get; set; }

        /// <summary>
        /// 预约的Guid
        /// </summary>
        public Guid ReservationGuid { get; set; }

    }



    /// </summary>
    public class GetEncyptionKey_PM
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        ///
        /// </summary>
        public string KeySerial { get; set; }

        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceNo { get; set; }


        /// <summary>
        /// 用户Guid
        /// </summary>
        public Guid UserInfoGuid { get; set; }

 
    }

    /// <summary>
    /// 获取锁密钥信息返回参数模型
    /// </summary>
    public class GetEncryptionKey_OM
    {
        public int EncryptionKey { get; set; }

        public string EncryptionInfo { get; set; }
    }


    /// <summary>
    /// 获取锁密钥信息返回参数模型
    /// </summary>
    public class GetBicycleTypeName_OM
    {
        /// <summary>
        /// 车辆类型
        /// </summary>
        public string BicycleTypeName { get; set; }

        /// <summary>
        /// 电量=电池剩余容量/电池满充容量
        /// </summary>
        public decimal? electricQuantity { get; set; }

        //如果是助力车并且电量低于20%Ischoice的值为0，app不能选择助力模式骑行，
        public int Ischoice { get; set; }

        

    }

        /// <summary>
        ///开锁或者关锁 传入参数模型
        /// </summary>
  public class OpenOrCloseLockBicycle_PM
    {
        /// <summary>
        /// Guid
        /// </summary>
        public Guid UserInfoGuid { get; set; }
        /// <summary>
        /// 锁设备号
        /// </summary>
        public string DeviceNo { get; set; }

        /// <summary>
        /// 关锁位置的经度
        /// </summary>
        public decimal? Longitude { get; set; }

        /// <summary>
        /// 关锁位置的纬度
        /// </summary>
        public decimal? Latitude { get; set; }

        /// <summary>
        /// 锁电压
        /// </summary>
        public Decimal? Voltage { get; set; }

        /// <summary>
        /// 处理类型  0：开锁，  1 关锁
        /// </summary>
        public int LockAction { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public string Timestamp { get; set; }

        /// <summary>
        /// 区分是手机端上传交易记录还是硬件上传
        /// TypeName=hardware;TypeName=mobile;
        /// </summary>
        public string TypeName { get; set; }

        /// <summary>
        ///  骑行模式:0表示关锁时传骑行模式值为0；1非助力车模式(普通车类型)；助力车模式：2助力模式；3普通模式(非助力模式)；4锻炼模式
        /// </summary>
        public int CyclingMode { get; set; }

    }

    /// <summary>
    /// 是否停放在电子围栏区域 参数模型
    /// </summary>
    public class BicycleIsinRange_PM
    {
        /// <summary>
        /// 查询的经度
        /// </summary>
        public decimal CurLongitude { get; set; }

        /// <summary>
        /// 查询的纬度
        /// </summary>
        public decimal CurLatitude { get; set; }
    }

    #region 后台实体模型



    /// <summary>
    ///硬件上传锁信息 输入参数模型
    /// </summary>
    public class AddBicycleHardWare_PM
    {
        //设备编号
        public string DeviceNo { get; set; }

        //密钥
        public string SecretKey { get; set; }
 
        //车锁MAC
        public string LockMac { get; set; }

        //电压
        public string Voltage { get; set; }


 
    }


    /// <summary>
    /// 后台地图显示非助力车车辆输出模型
    /// </summary>
    public class NoHelpMapBicycleModel
    {
        /// <summary>
        /// 电子围栏外单车经度
        /// </summary>
        public decimal? BicycleLongitude { get; set; }

        /// <summary>
        /// 电子围栏外单车纬度
        /// </summary>
        public decimal? BicycleLatitude { get; set; }

        /// <summary>
        /// 车辆编号
        /// </summary>
        public string DeviceNo { get; set; }

        /// <summary>
        /// 车辆类型：0非助力车；1助力车
        /// </summary>
        public int BicyCleTypeName { get; set; }

    }



    #endregion

    /// <summary>
    ///骑行结束计算费用 传入参数模型
    /// </summary>
    public class CyclingEndCalculateMoney_PM
    {
        /// <summary>
        /// 用户Guid
        /// </summary>
        public Guid UserInfoGuid { get; set; }
        /// <summary>
        /// 单车锁设备号
        /// </summary>
        public string DeviceNo { get; set; }

        /// <summary>
        /// 锁状态 0：开锁，  1 关锁
        /// </summary>
        public int LockAction { get; set; }

    }

    /// <summary>
    ///骑行结束计算费用 输出参数模型
    /// </summary>
    public class CyclingEndCalculateMoney_OM
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public String OrderNumber { get; set; }
        /// <summary>
        /// 单车设备编号
        /// </summary>
        public string DeviceNo { get; set; }


        /// <summary>
        /// 骑行总时长
        /// </summary>
        public string TotalTimeStr { get; set; }


        /// <summary>
        /// 运动消耗卡路里（kg）
        /// </summary>
        public string CalorieExpend { get; set; }

        /// <summary>
        /// 骑行总费用
        /// </summary>
        public decimal? TotalAmount { get; set; }


        /// <summary>
        /// 钱包余额
        /// </summary>
        public decimal? UsableAmount { get; set; }

    }


    /// <summary>
    ///硬件开锁或者关锁 传入参数模型 暂时没用
    /// </summary>
    public class HOpenOrCloseLockBicycle_PM
    {
        ///// <summary>
        ///// 推送编号
        ///// </summary>
        //public string PushId { get; set; }

        /// <summary>
        /// 用户Guid
        /// </summary>
        public Guid UserInfoGuid { get; set; }
        /// <summary>
        /// 单车锁设备号
        /// </summary>
        public string DeviceNo { get; set; }

        /// <summary>
        /// 关锁位置的经度
        /// </summary>
        public decimal? Longitude { get; set; }

        /// <summary>
        /// 关锁位置的纬度
        /// </summary>
        public decimal? Latitude { get; set; }

        /// <summary>
        /// 锁电压
        /// </summary>
        public Decimal? Voltage { get; set; }

        /// <summary>
        /// 处理类型  0：开锁，  1 关锁
        /// </summary>
        public int LockAction { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public string Timestamp { get; set; }
    }


    /// <summary>
    /// 测试开关锁传入参数模型
    /// </summary>
    public class Test_OpenOrCloseLockBicycle_PM
    {
        public Guid UserGuid { get; set; }
    }








}