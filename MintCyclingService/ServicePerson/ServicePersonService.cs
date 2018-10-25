using DTcms.Common;
using MintCyclingData;
using MintCyclingService.Common;
using MintCyclingService.Utils;
using System;
using System.Linq;
using Utility;

namespace MintCyclingService.ServicePerson
{
    public class ServicePersonService : IServicePersonaService
    {
        /// <summary>
        /// 维护人员登录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ResultModel Login(LoginModel_PM data)
        {
            string password = DESEncrypt.Encrypt(data.PassWord);

            using (var db = new MintBicycleDataContext())

            {
                var query = from x in db.ServicePersonInfo
                            join p in db.Province on x.ProvinceID equals p.Id
                            join c in db.City on x.CityID equals c.Id
                            join d in db.District on x.DistrictID equals d.Id
                            where x.UserName == data.UserName && x.Password == password && !x.IsDeleted
                            select new LoginModel_OM
                            {
                                UserInfoGuid = x.ServicePersonID,
                                CityId = x.CityID,
                                CityName = c.Name,
                                DistinctId = x.DistrictID,
                                DistinctName = d.Name,
                                ProvinceId = x.ProvinceID,
                                ProvinceName = p.Name,
                                RealName = x.RealName,

                                Address = x.Address
                            };
                if (query.Any())
                {
                    var Fir = query.FirstOrDefault();

                    return new ResultModel { ResObject = Fir };
                }
                return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.PasswordHashCodeError, Message = ResPrompt.PasswordHashCodeErrorMessage };
            }
        }

        /// <summary>
        /// 维修人员添加和修改(后台接口)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel AddServicePersonInfo(ServicePerson_PM model)
        {
            if (string.IsNullOrEmpty(model.UserName))
                return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ParaModelNotExist, Message = ResPrompt.ParaModelNotExistMessage };
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                if (model.ServicePersonID != null)  //修改
                {
                    var query = db.ServicePersonInfo.FirstOrDefault(s => s.ServicePersonID == model.ServicePersonID && !s.IsDeleted);
                    if (query == null)
                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.NoUserTransationCode, Message = ResPrompt.NoUserTransationMessage };
                    if (string.IsNullOrEmpty(query.Password) && string.IsNullOrEmpty(model.Password))
                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ParaModelNotExist, Message = ResPrompt.ParaModelNotExistMessage };
                    query.UserName = model.UserName;
                    query.RealName = model.RealName;
                    query.Phone = model.Phone;
                    query.Status = model.Status;
                    if (model.Password != Config.TmpPwd) { query.Password = DESEncrypt.Encrypt(model.Password); }
                    query.Sex = model.Sex;
                    query.ProvinceID = model.ProvinceID;
                    query.CityID = model.CityID;
                    query.DistrictID = model.DistinctId;
                    query.Address = model.Address;
                    query.Remark = model.Remark;
                    query.UpdateTime = DateTime.Now;
                    query.UpdateBy = model.AdminGuid;
                    db.SubmitChanges();

                    result.Message = "修改维修人员信息成功";
                }
                else//添加
                {
                    if (string.IsNullOrEmpty(model.Password))
                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ParaModelNotExist, Message = ResPrompt.ParaModelNotExistMessage };
                    var query = db.ServicePersonInfo.FirstOrDefault(s => s.UserName == model.UserName && !s.IsDeleted);
                    if (query != null)
                    {
                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ServicePersonCode, Message = ResPrompt.ServicePersonMessage };
                    }
                    else
                    {
                        var person = new ServicePersonInfo
                        {
                            ServicePersonID = Guid.NewGuid(),
                            UserName = model.UserName,
                            Phone = model.Phone,
                            Status = model.Status,
                            RealName = model.RealName,
                            Sex = model.Sex,
                            Password = DESEncrypt.Encrypt(model.Password),
                            ProvinceID = model.ProvinceID,
                            CityID = model.CityID,
                            DistrictID = model.DistinctId,
                            Address = model.Address,
                            Remark = model.Remark,
                            CreateBy = model.AdminGuid,
                            CreateTime = DateTime.Now,
                            IsDeleted = false
                        };

                        db.ServicePersonInfo.InsertOnSubmit(person);
                        db.SubmitChanges();
                        result.Message = "添加维修人员信息成功";
                    }
                }
                result.ResObject = true;
            }
            return result;
        }

        /// <summary>
        /// 维护人员列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel GetServicePersonInfList(SearchServicePerson_PM model)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var query = (from c in db.ServicePersonInfo
                             join p in db.Province on c.ProvinceID equals p.Id
                             join t in db.City on c.CityID equals t.Id
                             join d in db.District on c.DistrictID equals d.Id

                             where ((string.IsNullOrEmpty(model.Phone)) || c.Phone.Contains(model.Phone))
                             && ((string.IsNullOrEmpty(model.UserName)) || c.UserName.Contains(model.UserName))
                             && (model.ProvinceId == 0 || model.ProvinceId == c.ProvinceID)
                             && (model.CityId == 0 || model.CityId == c.CityID)
                             && (model.DistinctId == 0 || model.DistinctId == c.DistrictID)
                             && !c.IsDeleted
                             orderby c.CreateTime descending
                             select new ServicePersonList_OM
                             {
                                 ServicePersonID = c.ServicePersonID,
                                 UserName = c.UserName,
                                 Phone = c.Phone,
                                 ProvinceName = p.Name,
                                 Status = c.Status ?? 0,
                                 CityName = t.Name,
                                 DistinctName = d.Name,
                                 RealName = c.RealName,
                                 Address = c.Address,
                                 Sex = c.Sex,
                                 ProvinceID = c.ProvinceID,
                                 CityID = c.CityID,
                                 DistrictID = c.DistrictID,
                                 Remark = c.Remark,
                                 CreateTime = DateTime.Now
                             });
                if (query.Any())
                {
                    var list = query;
                    var cnt = list.Count();
                    var tsk = new PagedList<ServicePersonList_OM>(query, model.PageIndex, model.PageSize, query.Count());
                    result.ResObject = new { Total = query.Count(), List = tsk };
                }
            }
            return result;
        }

        /// <summary>
        /// 删除维护人员信息
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public ResultModel DeleteServicePersonByServiceGuid(DeleteServicePerson_PM Model)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var query = db.ServicePersonInfo.FirstOrDefault(s => s.ServicePersonID == Model.ServicePersonID && !s.IsDeleted);
                if (query == null)
                    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.NoUserTransationCode, Message = ResPrompt.NoUserTransationMessage };
                query.IsDeleted = true;
                query.UpdateBy = Model.AdminGuid;
                query.UpdateTime = DateTime.Now;
                db.SubmitChanges();
                result.Message = "删除维护人员成功";
            }
            return result;
        }

        /// <summary>
        /// 修改pushid
        /// </summary>
        /// <param name="userGuid"></param>
        /// <param name="pushid"></param>
        /// <returns></returns>
        public bool SetpushIdByUserGuid(Guid userGuid, string pushid)
        {
            using (var db = new MintBicycleDataContext())
            {
                var query = from x in db.ServicePersonInfo
                            where x.ServicePersonID == userGuid && !x.IsDeleted
                            select x;
                if (query.Any())
                {
                    var fir = query.FirstOrDefault();
                    fir.PushId = pushid;
                    fir.UpdateTime = DateTime.Now;
                    db.SubmitChanges();
                    return true;
                }
                return false;
            }
        }
    }
}