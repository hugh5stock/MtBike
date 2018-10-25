using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Common;

namespace MintCyclingService.Role
{
 
    /// <summary>
    /// 新增或更新角色参数模型
    /// </summary>
    public class AddAdminRole_PM
    {
        /// <summary>
        /// 角色Guid
        /// </summary>
        public Guid? AdminRoleGuid { get; set; }

        /// <summary>
        /// 角色名
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 操作者Guid
        /// </summary>
        public Guid OperatorGuid { get; set; }

    }

 

    /// <summary>
    /// 删除角色 参数模型
    /// </summary>
    public class DeleteAdminRole_PM
    {
        /// <summary>
        /// 角色Guid
        /// </summary>
        public Guid AdminRoleGuid { get; set; }

        /// <summary>
        /// 操作者Guid
        /// </summary>
        public Guid OperatorGuid { get; set; }

    }

    /// <summary>
    /// 获取角色分页列表 参数模型
    /// </summary>
    public class GetAdminRolePageList_PM : Paging_Model
    {
 
        /// <summary>
        /// 角色名
        /// </summary>
        public string RoleName { get; set; }
    }

    /// <summary>
    /// 获取角色分页列表 输出模型
    /// </summary>
    public class GetAdminRolePageList_OM
    {
        /// <summary>
        /// 角色Guid
        /// </summary>
        public Guid AdminRoleGuid { get; set; }


        /// <summary>
        ///   //角色名称
        /// </summary>
        public string RoleName { get; set; }
 

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

 

    }


}
