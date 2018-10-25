using MintCyclingData;
using MintCyclingService.Common;
using MintCyclingService.Utils;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using Utility.Common;

namespace MintCyclingService.AdminAccessCode
{
    public class AdminAccessCodeService : IAdminAccessCodeService
    {
        /// <summary>
        /// 查询后台用户所在的地区
        /// </summary>
        /// <param name="utk"></param>
        /// <returns></returns>
        public RegionData GetLoginAdminRegion(string utk)
        {
            var guid = Guid.Empty;
            if (!Guid.TryParse(utk, out guid))
                return null;
            RegionData loc = null;
            using (var db = new MintBicycleDataContext())
            {
                var query = (from s in db.Admin
                             join t in db.AdminAccessCode on s.AdminGuid equals t.AdminGuid
                             where t.Guid == guid && !s.IsDeleted
                             select new RegionData
                             {
                                 UserProvince = s.ProvinceID,
                                 UserCity = s.CityID,
                                 UserDistrict = s.DistrictID
                             });
                if (query.Any())
                {
                    loc = query.FirstOrDefault();
                    var sk = Config.HqUserRegion;
                    loc.ExceptUserRegion = loc.UserProvince == sk.UserProvince && loc.UserCity == sk.UserCity && loc.UserDistrict == sk.UserDistrict;
                }
            }
            return loc;
        }

        /// <summary>
        /// 根据AccessCode获取管理员Guid
        /// </summary>
        /// <param name="accessCode">Access Code</param>
        /// <returns>管理员Guid</returns>
        public Guid GetAdminGuidByAccessCode(Guid accessCode)
        {
            var result = new ResultModel();
            var now = DateTime.Now;

            using (var db = new MintCyclingData.MintBicycleDataContext())
            {
                var queryAccessCode = (from code in db.AdminAccessCode
                                       where code.Guid == accessCode && code.EndTime > now
                                       select code).FirstOrDefault();

                if (queryAccessCode == null)
                {
                    throw new AccessCodeException();
                }

                return queryAccessCode.AdminGuid;
            }

        }


        /// <summary>
        /// 根据AccessCode获取管理员Guid
        /// </summary>
        /// <param name="accessCode">Access Code</param>
        /// <returns>管理员Guid</returns>
        public ResultModel GetAdminGuidByAccessCodes(Guid accessCode)
        {
            var result = new ResultModel();
            var now = DateTime.Now;
            try
            {
                using (var db = new MintCyclingData.MintBicycleDataContext())
                {
                    var queryAccessCode = (from code in db.AdminAccessCode
                                           where code.Guid == accessCode
                                           select code).FirstOrDefault();

                    if (queryAccessCode == null)
                    {
                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.AccessCodeNotExisted, Message = ResPrompt.AccessCodeNotExistedMessage };
                    }

                    result.ResObject = queryAccessCode.AdminGuid;
                }
            }
            catch
            {
                return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.DBOperateFalied, Message = ResPrompt.DBOperateFaliedMessage };
            }
            return result;
        }

        /// <summary>
        /// 根据AccessCode获取管理员
        /// </summary>
        /// <param name="accessCode">Access Code</param>
        /// <returns>管理员</returns>
        public ResultModel GetAdminByAccessCode(Guid accessCode)
        {
            var result = new ResultModel();
            var now = DateTime.Now;

            using (var db = new MintCyclingData.MintBicycleDataContext())
            {
                var queryAccessCode = (from code in db.AdminAccessCode
                                       where code.Guid == accessCode && code.EndTime > now
                                       select code).FirstOrDefault();

                if (queryAccessCode == null)
                {
                    result.IsSuccess = false;
                    result.MsgCode = ResPrompt.AccessCodeNotExisted;
                    result.Message = ResPrompt.AccessCodeNotExistedMessage;
                    return result;
                }

                result.ResObject = queryAccessCode.Admin;
            }

            return result;
        }

        /// <summary>
        /// 获取管理员新的AccessCode
        /// </summary>
        /// <param name="AdminGuid">管理员Guid</param>
        /// <returns>管理员新的AccessCode</returns>
        public ResultModel GetNewAccessCode(Guid AdminGuid)
        {
            var result = new ResultModel();
            var now = DateTime.Now;

            using (var db = new MintCyclingData.MintBicycleDataContext())
            {

                var queryAccessCode = (from accessCode in db.AdminAccessCode
                                       where accessCode.AdminGuid == AdminGuid
                                       select accessCode).FirstOrDefault();


                if (queryAccessCode != null)
                {
                    db.AdminAccessCode.DeleteOnSubmit(queryAccessCode);
                }

                var newAccessCode = new MintCyclingData.AdminAccessCode();
                newAccessCode.Guid = Guid.NewGuid();
                newAccessCode.AdminGuid = AdminGuid;
                newAccessCode.StartTime = now;
                newAccessCode.EndTime = now.AddMinutes(30);
                newAccessCode.AccessCount = 0;

                db.AdminAccessCode.InsertOnSubmit(newAccessCode);
                db.SubmitChanges();

                result.ResObject = newAccessCode.Guid;
            }

            return result;
        }



        /// <summary>
        /// 获取管理员新的AccessCode
        /// </summary>
        /// <param name="AdminGuid">管理员Guid</param>
        /// <returns>管理员新的AccessCode</returns>
        public ResultModel GetRepiarNewAccessCode(Guid userGuid)
        {
            var result = new ResultModel();
            var now = DateTime.Now;

            using (var db = new MintCyclingData.MintBicycleDataContext())
            {

                var queryAccessCode = (from accessCode in db.RepairAccessCode
                                       where accessCode.AdminGuid == userGuid
                                       select accessCode).FirstOrDefault();


                if (queryAccessCode != null)
                {
                    db.RepairAccessCode.DeleteOnSubmit(queryAccessCode);
                }

                var newAccessCode = new MintCyclingData.RepairAccessCode();
                newAccessCode.Guid = Guid.NewGuid();
                newAccessCode.AdminGuid = userGuid;
                newAccessCode.StartTime = now;
                newAccessCode.EndTime = now.AddMinutes(30);
                newAccessCode.AccessCount = 0;

                db.RepairAccessCode.InsertOnSubmit(newAccessCode);
                db.SubmitChanges();

                result.ResObject = newAccessCode.Guid;
            }

            return result;
        }


        /// <summary>
        /// 验证AccessCode是否有效
        /// </summary>
        /// <param name="accessCode">Access Code</param>
        /// <returns>AccessCode是否有效</returns>
        public ResultModel VeriftyAccessCode(Guid accessCode)
        {
            var result = new ResultModel();

            try
            {
                using (var db = new MintBicycleDataContext())
                {

                    var queryAccessCode = (from code in db.AdminAccessCode
                                           where code.Guid == accessCode && code.EndTime > DateTime.Now
                                           select code).FirstOrDefault();


                    if (queryAccessCode == null)
                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.AccessCodeNotExisted, Message = ResPrompt.AccessCodeNotExistedMessage };

                    result.ResObject = true;

                    queryAccessCode.EndTime = DateTime.Now.AddMinutes(30);
                    queryAccessCode.AccessCount += 1;
                    try
                    {
                        db.SubmitChanges(ConflictMode.ContinueOnConflict);
                    }
                    catch (ChangeConflictException ex)
                    {
                        foreach (ObjectChangeConflict occ in db.ChangeConflicts)
                        {
                            occ.Resolve(RefreshMode.KeepCurrentValues);
                        }
                        db.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.DBOperateFalied, Message = ex.Message };
            }

            return result;
        }

        /// <summary>
        /// 根据用户Guid删除AccessCode
        /// </summary>
        /// <param name="adminGuid">管理员Guid</param>
        public void RemoveByAdminGuid(Guid adminGuid)
        {
            using (var db = new MintBicycleDataContext())
            {
                var tempAccessCodes = (from p in db.AdminAccessCode where p.AdminGuid == adminGuid select p);

                if (tempAccessCodes != null)
                {
                    db.AdminAccessCode.DeleteAllOnSubmit(tempAccessCodes);
                    db.SubmitChanges();
                }
            }
        }


        /// <summary>
        /// 根据用户Guid删除AccessCode
        /// </summary>
        /// <param name="adminGuid">管理员Guid</param>
        public void RemoveByRepiarGuid(Guid userGuid)
        {
            using (var db = new MintBicycleDataContext())
            {
                var tempAccessCodes = (from p in db.RepairAccessCode where p.AdminGuid == userGuid select p);

                if (tempAccessCodes != null)
                {
                    db.RepairAccessCode.DeleteAllOnSubmit(tempAccessCodes);
                    db.SubmitChanges();
                }
            }
        }



    }
}
