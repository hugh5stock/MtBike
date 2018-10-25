using System;

namespace MintCyclingService.Login
{
    /// <summary>
    /// 登录验证 输入参数模型
    /// </summary>
    public class ValidateLogin_PM
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string PhoneNo { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string ValidateCode { get; set; }

        /// <summary>
        /// 推送编号-主要用于在蓝牙关锁成功时，给手机自动推送一条消息
        /// </summary>
        public string PushId { get; set; }

    }

    /// <summary>
    /// 登录成功后 输出参数模型
    /// </summary>
    public class LoginSuccess_OM
    {
        /// <summary>
        /// 已预约的车辆
        /// </summary>
        public string ReservationBicycleNo { get; set; }
        /// <summary>
        /// 已预约的开始时间
        /// </summary>
        public string ReservationStartTme { get; set; }
        /// <summary>
        /// 已预约车辆的地址
        /// </summary>
        public string ReservationAddress { get; set; }
        /// <summary>
        /// 预约Guid
        /// </summary>
        public string ReservationGuid { get; set; }
        /// <summary>
        /// 登录Token
        /// </summary>
        public Guid LoginToken { get; set; }

        /// <summary>
        /// 用户Guid
        /// </summary>
        public Guid UserInfoGuid { get; set; }

        /// <summary>
        ///  认证状态：0未认证；1已认证
        /// </summary>
        public string StatusStr { get; set; }


        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceNo { get; set; }

        /// <summary>
        /// 登录次数
        /// </summary>
        public int AccessCount { get; set; }


    }

    /// <summary>
    /// 发送验证短信  输入参数模型
    /// </summary>
    public class SendSMSForValidateCode_PM
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string phoneNo { get; set; }
    }

    /// <summary>
    ///用户注册信息  输入参数模型
    /// </summary>
    public class UserInfo_PM
    {
        /// <summary>
        /// 用户Guid
        /// </summary>
        public Guid UserInfoGuid { get; set; }

        /// <summary>
        /// 用户头像Guid
        /// </summary>
        public Guid? PhotoGuid { get; set; }

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
        /// 信用积分默认100
        /// </summary>
        public int CreditScore { get; set; }

        /// <summary>
        /// 状态默认启用：1
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 推送的编号
        /// </summary>
        public string PushId { get; set; }
    }

    /// <summary>
    ///用户注册信息  输入参数模型
    /// </summary>
    public class CustomerAccessToken_PM
    {
        ///// <summary>
        ///// Guid
        ///// </summary>
        //public Guid AccessTokenGuid { get; set; }

        /// <summary>
        /// 用户的Guid
        /// </summary>
        public Guid UserInfoGuid { get; set; }

        ///// <summary>
        ///// 过期的时间
        ///// </summary>
        //public DateTime ExpirationTime { get; set; }

        ///// <summary>
        ///// 登录次数
        ///// </summary>
        //public int AccessCount { get; set; }
    }

    /// <summary>
    /// 协议Url  输入参数模型
    /// </summary>
    public class ProUrl_PM
    {
        /// <summary>
        /// 类别
        /// </summary>
        public string typeName { get; set; }
    }

    //添加账户信息
    public class UserAccount_PM
    {
        /// <summary>
        /// Guid
        /// </summary>
        public Guid? UserInfoGuid { get; set; }

    }
}