using MintCyclingService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Common;

namespace GreenShowRepo.UserMemberAccessCode
{
    public class UserMemberAccessCodeService : IUserMemberAccessCodeService
    {



        /// <summary>
        /// 根据AccessCode获取用户的PlayerGuid
        /// </summary>
        /// <param name="accessCode">Access Code</param>
        /// <returns>用户的PlayerGuid</returns>
        //public ResultModel GetPlayerGuidByAccessCode(Guid accessCode)
        //{
        //    var result = new ResultModel();
        //    var now = DateTime.Now;

        //    using (var db = new MintCyclingData.MintBicycleDataContext())
        //    {
        //        var queryAccessCode = (from code in db.UserMemberAccessCode
        //                               where code.Guid == accessCode && code.EndTime > now
        //                               select code).FirstOrDefault();

        //        if (queryAccessCode == null)
        //        {
        //            throw new AccessCodeException();
        //        }

        //        var queryPlayer = (from player in db.Player
        //                           where player.UserMemberGuid == queryAccessCode.UserMemberGuid
        //                           select player).FirstOrDefault();

        //        result.ReturnObject = queryPlayer.Guid;
        //    }

        //    return result;
        //}

        ///// <summary>
        ///// 根据AccessCode获取用户Guid
        ///// </summary>
        ///// <param name="accessCode">Access Code</param>
        ///// <returns>用户Guid</returns>
        //public ResultModel GetUserMemberGuidByAccessCode(Guid accessCode)
        //{
        //    var result = new ResultModel();
        //    var now = DateTime.Now;

        //    using (var db = new MintCyclingData.MintBicycleDataContext())
        //    {
        //        var queryAccessCode = (from code in db.UserMemberAccessCode
        //                               where code.Guid == accessCode && code.EndTime > now
        //                               select code).FirstOrDefault();

        //        if (queryAccessCode == null)
        //        {
        //            throw new AccessCodeException();
        //        }

        //        result.ResObject = queryAccessCode.UserMemberGuid;
        //    }

        //    return result;
        //}

        ///// <summary>
        ///// 根据AccessCode获取用户
        ///// </summary>
        ///// <param name="accessCode">Access Code</param>
        ///// <returns>用户</returns>
        //public ResultModel GetUserMemberByAccessCode(Guid accessCode)
        //{
        //    var result = new ResultModel();
        //    var now = DateTime.Now;

        //    using (var db = new MintCyclingData.MintBicycleDataContext())
        //    {
        //        var queryAccessCode = (from code in db.UserMemberAccessCode
        //                               where code.Guid == accessCode && code.EndTime > now
        //                               select code).FirstOrDefault();

        //        if (queryAccessCode == null)
        //        {
        //            throw new AccessCodeException();
        //        }

        //        result.ResObject = queryAccessCode.UserMember;
        //    }

        //    return result;
        //}

        ///// <summary>
        ///// 获取用户新的AccessCode
        ///// </summary>
        ///// <param name="userMemberGuid">用户Guid</param>
        ///// <returns>用户新的AccessCode</returns>
        //public ResultModel GetNewAccessCode(Guid userMemberGuid)
        //{
        //    var result = new ResultModel();
        //    var now = DateTime.Now;

        //    using (var db = new MintCyclingData.MintBicycleDataContext())
        //    {

        //        var queryAccessCode = (from accessCode in db.UserMemberAccessCode
        //                               where accessCode.UserMemberGuid == userMemberGuid
        //                               select accessCode).FirstOrDefault();


        //        if (queryAccessCode != null)
        //        {
        //            db.UserMemberAccessCode.DeleteOnSubmit(queryAccessCode);
        //        }

        //        var newAccessCode = new GreenShowData.UserMemberAccessCode();
        //        newAccessCode.Guid = Guid.NewGuid();
        //        newAccessCode.UserMemberGuid = userMemberGuid;
        //        newAccessCode.StartTime = now;
        //        newAccessCode.EndTime = now.AddYears(100);
        //        newAccessCode.AccessCount = 0;

        //        db.UserMemberAccessCode.InsertOnSubmit(newAccessCode);
        //        db.SubmitChanges();

        //        result.ResObject = newAccessCode.Guid;
        //    }

        //    return result;
        //}

        ///// <summary>
        ///// 验证AccessCode是否有效
        ///// </summary>
        ///// <param name="accessCode">Access Code</param>
        ///// <returns>AccessCode是否有效</returns>
        //public ResultModel VeriftyAccessCode(Guid accessCode)
        //{
        //    var result = new ResultModel();


        //    using (var db = new MintCyclingData.MintBicycleDataContext())
        //    {

        //        var queryAccessCode = (from code in db.UserMemberAccessCode
        //                               where code.Guid == accessCode
        //                               select code).FirstOrDefault();


        //        if (queryAccessCode == null)
        //        {
        //            result.ResObject = false;
        //            return result;
        //        }

        //        result.ResObject = true;

        //        queryAccessCode.AccessCount += 1;
        //        db.SubmitChanges();
        //    }


        //    return result;
        //}
    }
}
