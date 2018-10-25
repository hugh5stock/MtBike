namespace Utility
{
    /// <summary>
    /// 通用配置
    /// </summary>
    public class Config
    {
        public static readonly string TmpPwd = "00000000-0000-0000-0000-00000000000x";
        public static RegionData UserRegion { get; set; }
        //后台总账户的省市区配置
        public static RegionData HqUserRegion = new RegionData { UserProvince = 9, UserCity = 73, UserDistrict = 759 };

        public static string UserAccessCode { get; set; }

        /// <summary>
        /// 签名参数名
        /// </summary>
        public static readonly string SignName = "sign";

        /// <summary>
        /// 时间戳参数名
        /// </summary>
        public static readonly string TimeStampName = "timestamp";

        /// <summary>
        /// 秘钥参数名
        /// </summary>
        public static readonly string SecretKeyName = "secretKey";

        /// <summary>
        /// 接口时间戳+-有效时间范围,单位秒
        /// </summary>
        public static readonly int TimeScopeInSec = 3000;

        /// <summary>
        /// AccessCode参数名
        /// </summary>
        public static readonly string AccessCodeName = "accessCode";


    }

    public class RegionData
    {
        public int? UserProvince { get; set; }
        public int? UserCity { get; set; }
        public int? UserDistrict { get; set; }
        public bool ExceptUserRegion { get; set; }
    }
}
