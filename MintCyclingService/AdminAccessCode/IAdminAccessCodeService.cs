using MintCyclingService.Utils;
using System;
using Utility;
using Utility.Common;

namespace MintCyclingService.AdminAccessCode
{
    public interface IAdminAccessCodeService
    {
        /// <summary>
        /// 查询后台用户所在的地区
        /// </summary>
        /// <param name="utk"></param>
        /// <returns></returns>
        RegionData GetLoginAdminRegion(string utk);

        /// <summary>
        /// 根据AccessCode获取管理员Guid
        /// </summary>
        /// <param name="accessCode">Access Code</param>
        /// <returns>管理员Guid</returns>
        Guid GetAdminGuidByAccessCode(Guid accessCode);

        ///// <summary>
        ///// 根据AccessCode获取管理员Guid
        ///// </summary>
        ///// <param name="accessCode">Access Code</param>
        ///// <returns>管理员Guid</returns>
        ResultModel GetAdminGuidByAccessCodes(Guid accessCode);

        /// <summary>
        /// 根据AccessCode获取管理员
        /// </summary>
        /// <param name="accessCode">Access Code</param>
        /// <returns>管理员</returns>
        ResultModel GetAdminByAccessCode(Guid accessCode);

        /// <summary>
        /// 获取管理员新的AccessCode
        /// </summary>
        /// <param name="AdminGuid">管理员Guid</param>
        /// <returns>管理员新的AccessCode</returns>
        ResultModel GetNewAccessCode(Guid AdminGuid);
        /// <summary>
        /// 获取管理员新的AccessCode
        /// </summary>
        /// <param name="AdminGuid">管理员Guid</param>
        /// <returns>管理员新的AccessCode</returns>
        ResultModel GetRepiarNewAccessCode(Guid userGuid);

        /// <summary>
        /// 验证AccessCode是否有效
        /// </summary>
        /// <param name="accessCode">Access Code</param>
        /// <returns>AccessCode是否有效</returns>
        ResultModel VeriftyAccessCode(Guid accessCode);

        /// <summary>
        /// 根据用户Guid删除AccessCode
        /// </summary>
        /// <param name="adminGuid">管理员Guid</param>
        void RemoveByAdminGuid(Guid adminGuid);

        /// <summary>
        /// 根据用户Guid删除AccessCode
        /// </summary>
        /// <param name="adminGuid">管理员Guid</param>
        void RemoveByRepiarGuid(Guid userGuid);
    }
}
