using MintCyclingService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Common;

namespace MintCyclingService.Admin
{
    public interface IAdminService
    {
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">口令</param>
        /// <returns>用户登录结果</returns>
        ResultModel Login(string userName, string password);
        /// <summary>
        /// 新增或更新管理员
        /// </summary>
        /// <param name="data"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        ResultModel AddOrUpdateAdmin(AddAdmin_PM data);

        /// <summary>
        /// 管理员修改口令
        /// </summary>
        /// <param name="data"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        ResultModel ChgPwd(ChangePassword_PM data);

        /// <summary>
        /// 设置管理员状态
        /// </summary>
        /// <param name="data"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        ResultModel UpdateAdminStatus(SetAdminStatus_PM data);

        /// <summary>
        /// 删除管理员
        /// </summary>
        /// <param name="data"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        ResultModel DelAdmin(DeleteAdmin_PM data);

        /// <summary>
        /// 获取管理员分页列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        ResultModel GetAdminPageList(GetAdminPageList_PM data);

        /// <summary>
        /// 查询后台用户角色
        /// </summary>
        /// <returns></returns>
        ResultModel GetAdminRole();

        /// <summary>
        /// 查询后台角色列表
        /// </summary>
        /// <returns></returns>
        ResultModel GetAdminRoleList();

        /// <summary>
        /// 查询角色关联的权限
        /// </summary>
        /// <returns></returns>
        ResultModel GetAdminRolePerm(RolePerm_PM para);

        /// <summary>
        /// 更新角色关联的权限
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        ResultModel UpdateAdminRolePerm(UpdateRolePerm_PM para);

        /// <summary>
        /// 查询后台用户的权限
        /// </summary>
        /// <param name="roleGuid"></param>
        /// <returns></returns>
        ResultModel GetAdminRolePermUrl(Guid roleGuid);

        /// <summary>
        /// 查询后台用户的菜单
        /// </summary>
        /// <param name="roleGuid"></param>
        /// <returns></returns>
        ResultModel GetAdminRoleMenu(Guid roleGuid);
    }
}