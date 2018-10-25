using MintCyclingService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Common;

namespace MintCyclingService.Role
{
    public interface IAdminRoleService
    {
 
        /// <summary>
        /// 新增或更新角色
        /// </summary>
        /// <param name="data"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        ResultModel AddOrUpdateAdminRole(AddAdminRole_PM data);


        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="data"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        ResultModel DelAdmin(DeleteAdminRole_PM data);

        /// <summary>
        /// 获取角色分页列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        ResultModel GetAdminRolePageList(GetAdminRolePageList_PM data);

 
    }
}