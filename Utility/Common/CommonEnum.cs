using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Common
{
    /// <summary>
    /// 性别通用枚举
    /// </summary>
    public enum GendarEnum
    {
        /// <summary>
        /// 女
        /// </summary>
        Female,

        /// <summary>    
        /// 男
        /// </summary>      
        Man
    }

    /// <summary>
    /// 排序枚举
    /// </summary>
    public enum SortEnum
    {
        /// <summary>
        /// 升序
        /// </summary>
        Asc,

        /// <summary>
        /// 降序
        /// </summary>
        Desc,

        /// <summary>
        /// 不排序
        /// </summary>
        None
    }

    /// <summary>
    /// 测试账号状态
    /// </summary>
    public enum TestAccountStatusEnum
    {
        /// <summary>
        /// 可用
        /// </summary>
        Enable,

        /// <summary>
        /// 不可用
        /// </summary>
        Disable
    }
}
