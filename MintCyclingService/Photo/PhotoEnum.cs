using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MintCyclingService.Photo
{
    /// <summary>
    /// 图片类型枚举
    /// </summary>
    public enum PhotoTypeEnum
    {
        /// <summary>
        /// 头像
        /// </summary>
        Avatar,

        /// <summary>
        /// 引导页
        /// </summary>
        GuidePage,

        /// <summary>
        /// 广告弹窗
        /// </summary>
        Commercial,

        /// <summary>
        /// 分享时用户分享照片
        /// </summary>
        SharePhoto,


        /// <summary>
        /// 故障图片
        /// </summary>
        BreakDownPhoto,

        /// <summary>
        /// 持工证照片
        /// </summary>
        ICBCCreditPhoto

    }

    /// <summary>
    /// 广告位图片类型
    /// </summary>
    public enum AdvertisingType
    {
        /// <summary>
        /// 引导页
        /// </summary>
        GuidePage = 1,
        /// <summary>
        /// 广告弹窗
        /// </summary>
        Commercial = 2
    }
}
