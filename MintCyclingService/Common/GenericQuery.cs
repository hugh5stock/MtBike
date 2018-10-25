using MintCyclingData;
using MintCyclingService.Cycling;
using MintCyclingService.User;
using MintCyclingService.UserAccount;
using MintCyclingService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MintCyclingService.Common
{
    public class GenericQuery
    {
        public static string AdminRoleMenuName = "权限管理";
        public static string ExcAdminRoleName = "超级管理员";

        /// <summary>
        /// 查询用户数据
        /// </summary>
        /// <param name="db"></param>
        /// <param name="userGuid"></param>
        /// <returns></returns>
        public static ResultModel GetUserData(MintBicycleDataContext db, Guid userGuid)
        {
            var result = new ResultModel();
            var q = (from s in db.UserInfo
                     join t in db.UserAuthentication on s.UserInfoGuid equals t.UserInfoGuid
                     where s.UserInfoGuid == userGuid
                     select new UserData_OM
                     {
                         UserGuid = s.UserInfoGuid,
                         UserAuthGuid = t.UserAuthGuid,
                         UserNickName = s.UserNickName
                     });
            if (q.Any())
            {
                result.ResObject = q.FirstOrDefault();
            }
            else
            {
                result.IsSuccess = false;
                result.MsgCode = ResPrompt.UserUserAuthNotExist;
                result.Message = ResPrompt.UserUserAuthNotExistMessage;
            }
            return result;
        }

        /// <summary>
        /// 查询车辆数据
        /// </summary>
        /// <param name="db"></param>
        /// <param name="bicycleGuid"></param>
        /// <returns></returns>
        public static BicycleData_OM GetBicycleData(MintBicycleDataContext db, Guid? bicycleGuid)
        {
            var q = (from s in db.BicycleLockInfo
                     where s.LockGuid == bicycleGuid
                     select new BicycleData_OM
                     {
                         BicycleNo = s.LockNumber
                     });
            if (q.Any())
                return q.FirstOrDefault();
            return null;
        }






    }
}