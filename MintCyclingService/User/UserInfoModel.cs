using MintCyclingService.Common;
using System;

namespace MintCyclingService.User
{
    /// <summary>
    /// 用户数据
    /// </summary>
    public class UserData_OM
    {
        public Guid UserGuid { get; set; }
        public Guid UserAuthGuid { get; set; }
        public string UserNickName { get; set; }
    }

    /// <summary>
    /// 个人行程参数
    /// </summary>
    public class UserTravel_PM : Paging_Model
    {
        /// <summary>
        /// 用户Guid
        /// </summary>
        public Guid UserGuid { get; set; }
    }

    /// <summary>
    /// 个人行程 输出模型
    /// </summary>
    public class UserTravel_OM
    {
        /// <summary>
        /// 行程开始时间
        /// </summary>
        public string UserTravelStartTime { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNo { get; set; }

        /// <summary>
        /// 自行车编号
        /// </summary>
        public string BicycleNo { get; set; }

        /// <summary>
        /// 骑车时间
        /// </summary>
        public string BicycleUsingTime { get; set; }

        /// <summary>
        /// 骑行花费
        /// </summary>
        public string BicycleSpend { get; set; }
    }

    /// <summary>
    /// 骑行结束参数
    /// </summary>
    public class CyclingEnd_PM
    {
        /// <summary>
        /// 用户Guid
        /// </summary>
        public Guid UserInfoGuid { get; set; }

        ///// <summary>
        ///// 单车锁设备号
        ///// </summary>
        //public string DeviceNo { get; set; }
        /////// <summary>
        ///// 查询的经度
        ///// </summary>
        //public decimal CurLongitude { get; set; }
        ///// <summary>
        ///// 查询的纬度
        ///// </summary>
        //public decimal CurLatitude { get; set; }
    }

    /// <summary>
    /// 修改车辆信息表中是否在电子围栏内-模型
    /// </summary>
    public class IsElectronicModel
    {
        ///// <summary>
        ///// 是否在电子围栏范围内
        ///// </summary>
        //public bool IsInElectronicFenCing { get; set; }

        /// <summary>
        /// 电子围栏Guid
        /// </summary>
        public Guid ElectronicFenCingGuid { get; set; }
    }

    /// <summary>
    /// 骑行结束 输出模型
    /// </summary>
    public class CyclingEnd_OM
    {
        public bool IsinRange { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNo { get; set; }

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
        public double CalorieExpend { get; set; }

        /// <summary>
        /// 节约的碳排放量
        /// </summary>
        public double TotalCarbon { get; set; }

        /// <summary>
        /// 骑行的距离
        /// </summary>
        public decimal TotalDistance { get; set; }

        /// <summary>
        /// 骑行总费用
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// 钱包余额
        /// </summary>
        public decimal? UsableAmount { get; set; }


        /// <summary>
        /// 电力值
        /// </summary>
        public decimal TotalPowerValue { get; set; }
        
    }

    /// <summary>
    /// 骑行费用模型
    /// </summary>
    public class CyclingCostModel
    {
        /// <summary>
        /// 每小时骑行单价
        /// </summary>
        public double Price { get; set; }

        /// <summary>
        /// 骑行耗时
        /// </summary>
        public TimeSpan Tick { get; set; }
    }

    /// <summary>
    /// 查询个人中心用户信息 输入模型
    /// </summary>
    public class GetUserInfo_PM
    {
        /// <summary>
        /// 用户Guid
        /// </summary>
        public Guid UserGuid { get; set; }
    }

    /// <summary>
    /// 查询个人中心用户信息 输出模型
    /// </summary>
    public class GetUsersInfoCenter_OM
    {
        /// <summary>
        /// 用户Guid
        /// </summary>
        public Guid UserGuid { get; set; }

        /// <summary>
        /// 用户头像
        /// </summary>
        public string PhotoUrl { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 卡路里或者运动成就
        /// </summary>
        public double? CalorieExpend { get; set; }

        /// <summary>
        /// 节约碳排量:单位KG
        /// </summary>
        public double? Carbon { get; set; }

        /// <summary>
        /// 骑行的距离KM
        /// </summary>
        public decimal? Distance { get; set; }

        /// <summary>
        /// 新增电力值
        /// </summary>
        public decimal? TotalPowerValue { get; set; }

        /// <summary>
        /// 充电宝
        /// </summary>
        public    string PowerBank{get;set;}

    }

    /// <summary>
    /// 查询用户个人信息 输出模型
    /// </summary>
    public class GetUsersInfo_OM
    {
        /// <summary>
        /// 用户Guid
        /// </summary>
        public Guid UserGuid { get; set; }

        /// <summary>
        /// 用户头像
        /// </summary>
        public string PhotoUrl { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string UserNickName { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        ///  认证状态：0未认证；1已认证
        /// </summary>
        public string StatusStr { get; set; }
    }

    /// <summary>
    /// 修改用户手机号码 输入模型
    /// </summary>
    public class EditUserPhoneOrNickName_PM
    {
        /// <summary>
        /// 用户Guid
        /// </summary>
        public Guid UserGuid { get; set; }

        /// <summary>
        /// 用户头像Guid
        /// </summary>
        public Guid? PhotoGuid { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }
    }

    /// <summary>
    /// 用户认证  输入模型
    /// </summary>
    public class AddUserAuth_PM
    {
        /// <summary>
        /// 认证的用户Guid
        /// </summary>
        public Guid UserInfoGuid { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string CardNumber { get; set; }
    }

    /// <summary>
    /// 微信用户信息授权信息输入参数
    /// </summary>
    public class MicroMsgUserInfo_PM
    {
        /// <summary>
        /// 加密Guid
        /// </summary>
        public string userid { get; set; }
        
        /// <summary>
        /// 加密数据
        /// </summary>
        public string encryptedData { get; set; }
        
        /// <summary>
        /// 加密向量
        /// </summary>
        public string iv { get; set; }
    }

    /// <summary>
    /// GNets位置点输入参数
    /// </summary>
    public class GNetsPosition_PM
    {
        /// <summary>
        /// 位置经度
        /// </summary>
        public string Longitude { get; set; }

        /// <summary>
        /// 位置纬度
        /// </summary>
        public string Latitude { get; set; }
    }

    /// <summary>
    /// 后台用户列表输入参数
    /// </summary>
    public class AdminUserInfo_PM : Paging_Model
    {
        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string UserNickName { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
    }

    /// <summary>
    /// 后台用户列表 输出模型
    /// </summary>
    public class AdminUserInfo_OM
    {
        /// <summary>
        /// 用户Guid
        /// </summary>
        public Guid UserGuid { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string UserNickName { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        ///  认证状态：0未认证；1已认证
        /// </summary>
        public string StatusStr { get; set; }

        /// <summary>
        ///  用户状态
        /// </summary>
        public int UserStatus { get; set; }

        /// <summary>
        ///  押金
        /// </summary>
        public decimal? Amount { get; set; }

        /// <summary>
        ///  余额
        /// </summary>
        public decimal? UsableAmount { get; set; }

        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceNo { get; set; }

        /// <summary>
        /// 推送ID
        /// </summary>
        public string PushId { get; set; }

        /// <summary>
        /// 是否推送
        /// </summary>
        public string IsPush { get; set; }

        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }



    /// <summary>
    /// 用户详情信息输入参数
    /// </summary>
    public class UserInfoDetails_PM
    {
        /// <summary>
        /// 用户Guid
        /// </summary>
        public Guid UserGuid { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string UserNickName { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 用户状态：0未启用；1启用
        /// </summary>
        public string UserStatus { get; set; }

        /// <summary>
        ///  认证状态：0未认证；1已认证
        /// </summary>
        public string StatusStr { get; set; }

        /// <summary>
        /// 用户图像
        /// </summary>
        public string PhotoUrl { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        public string CardNumber { get; set; }

    }


    /// <summary>
    /// 修改用户信息输入参数
    /// </summary>
    public class EditUserInfo_PM
    {
        /// <summary>
        /// 用户Guid
        /// </summary>
        public Guid UserGuid { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string UserNickName { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 用户状态：0未启用；1启用
        /// </summary>
        public string UserStatus { get; set; }

        /// <summary>
        ///  认证状态：0未认证；1已认证
        /// </summary>
        public string StatusStr { get; set; }
    }

    /// <summary>
    /// 删除用户输入参数模型
    /// </summary>
    public class DeleteUserInfo_PM
    {
        /// <summary>
        /// 用户Guid
        /// </summary>
        public Guid UserGuid { get; set; }
    }

    /// <summary>
    /// 还车异常处理
    /// </summary>
    public class ReturnCar_PM
    {
        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 操作者Guid
        /// </summary>
        public Guid OperatorGuid { get; set; }

    }

    /// <summary>
    /// 锁定用户状态
    /// </summary>
    public class LockUserInfo_PM
    {
        /// <summary>
        /// 用户Guid
        /// </summary>
        public Guid UserGuid { get; set; }
    }

    public class  BindPowerBank
    {
        /// <summary>
        /// 充电宝编码
        /// </summary>
        public string PowerBank { get; set; }
        /// <summary>
        /// 用户guid
        /// </summary>
        public    Guid UserGuid { get; set; }



    }



}