using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Common;

namespace MintCyclingService.Admin
{
    /// <summary>
    /// 检验管理员是否登录 参数模型
    /// </summary>
    public class CheckAdminLogin_PM
    {
        /// <summary>
        /// 管理员名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 管理员口令
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string ValidateCode { get; set; }
    }

    /// <summary>
    /// 检验管理员是否登录 输出模型
    /// </summary>
    public class CheckAdminLogin_OM
    {
        public Guid AccessCode { get; set; }

        public string UserName { get; set; }

        public string RoleName { get; set; }

        public Guid AdminGuid { get; set; }

        public string PermUrls { get; set; }

        public string Menus { get; set; }
    }

    /// <summary>
    /// 管理员登录 输出模型
    /// </summary>
    public class AdminLoginData_OM
    {
        public Guid AdminGuid { get; set; }

        public string RoleName { get; set; }

        public Guid? RoleGuid { get; set; }
    }

    /// <summary>
    /// 新增或更新管理员 参数模型
    /// </summary>
    public class AddAdmin_PM
    {
        /// <summary>
        /// 管理员Guid
        /// </summary>
        public Guid? AdminGuid { get; set; }

        /// <summary>
        /// 管理员名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// AES加密口令
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 角色名
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        /// 管理员状态
        /// </summary>
        public byte AdminStatus { get; set; }

        ///以下是新增字段属性
        /// <summary>
        /// 归属者名称
        /// </summary>
        public string AttributableName { get; set; }
        ///// <summary>
        ///// 创建者
        ///// </summary>
        //public Guid CreateBy { get; set; }
        ///// <summary>
        ///// 更新者
        ///// </summary>
        //public Guid UpdateBy { get; set; }


        /// <summary>
        /// 操作者Guid
        /// </summary>
        public Guid OperatorGuid { get; set; }

        public int? Province { get; set; }

        public int? City { get; set; }

        public int? Area { get; set; }
    }

    /// <summary>
    /// 管理员修改口令 参数模型
    /// </summary>
    public class ChangePassword_PM
    {
        /// <summary>
        /// 管理员Guid
        /// </summary>
        public Guid AdminGuid { get; set; }

        /// <summary>
        /// AES加密旧口令
        /// </summary>
        public string OldPassword { get; set; }

        /// <summary>
        /// AES加密新口令
        /// </summary>
        public string NewPassword { get; set; }

        /// <summary>
        /// 操作者Guid
        /// </summary>
        public Guid OperatorGuid { get; set; }

    }

    /// <summary>
    /// 设置管理员状态 参数模型
    /// </summary>
    public class SetAdminStatus_PM
    {
        /// <summary>
        /// 管理员Guid
        /// </summary>
        public Guid AdminGuid { get; set; }

        /// <summary>
        /// 管理员状态 1可用,0不可用
        /// </summary>
        public byte AdminStatus { get; set; }

        /// <summary>
        /// 操作者Guid
        /// </summary>
        public Guid OperatorGuid { get; set; }

    }

    /// <summary>
    /// 删除管理员 参数模型
    /// </summary>
    public class DeleteAdmin_PM
    {
        /// <summary>
        /// 管理员Guid
        /// </summary>
        public Guid AdminGuid { get; set; }

        /// <summary>
        /// 操作者Guid
        /// </summary>
        public Guid OperatorGuid { get; set; }

    }

    /// <summary>
    /// 获取管理员分页列表 参数模型
    /// </summary>
    public class GetAdminPageList_PM : Paging_Model
    {
        /// <summary>
        /// 搜索用户名
        /// </summary>
        public string SearchUserName { get; set; }

        /// <summary>
        /// 角色名
        /// </summary>
        public string RoleName { get; set; }
    }

    /// <summary>
    /// 获取管理员分页列表 输出模型
    /// </summary>
    public class GetAdminPageList_OM
    {
        /// <summary>
        /// 管理员Guid
        /// </summary>
        public Guid AdminGuid { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 角色名
        /// </summary>
        public string RoleName { get; set; }


        /// <summary>
        /// 归属者名称2017-02-17新增字段
        /// </summary>
        public string AttributableName { get; set; }

        /// <summary>
        /// 注册时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime? LastLoginTime { get; set; }

        /// <summary>
        /// 登录次数
        /// </summary>
        public int? LoginCount { get; set; }


        /// <summary>
        /// 状态
        /// </summary>
        public byte Status { get; set; }

        public int? Province { get; set; }

        public int? City { get; set; }

        public int? Area { get; set; }
    }

    /// <summary>
    /// 角色权限的查询参数
    /// </summary>
    public class RolePerm_PM
    {
        public Guid RoleGuid { get; set; }
    }

    /// <summary>
    /// 角色权限的输出模型
    /// </summary>
    public class RolePerm_OM
    {
        public Guid MenuGuid { get; set; }
        public string MenuName { get; set; }
        public bool IsNode { get; set; }
        public bool ExistNode { get; set; }
        public bool Checked { get; set; }
        public Guid? TMenuGuid { get; set; }
    }

    /// <summary>
    /// 更新角色权限的参数
    /// </summary>
    public class UpdateRolePerm_PM
    {
        public Guid RoleGuid { get; set; }
        public Guid OperatorGuid { get; set; }
        public List<RolePerm_OM> RolePerms { get; set; }
    }
}
