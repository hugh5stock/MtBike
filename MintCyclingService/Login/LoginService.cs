using MintCyclingData;
using MintCyclingService.Common;
using MintCyclingService.Utils;
using System;
using System.Linq;
using Utility.Common;
using Utility.LogHelper;

namespace MintCyclingService.Login
{
    public class LogService : ILoginService
    {
        /// <summary>
        /// 新增短信验证记录
        /// </summary>
        /// <param name="phoneNo">手机号码</param>
        /// <param name="validateCode">验证码</param>
        /// <returns>新增短信验证记录</returns>
        public ResultModel AddSmsValidateCode(string phoneNo, string validateCode)
        {
            var result = new ResultModel();

            var now = DateTime.Now;

            try
            {
                using (var db = new MintCyclingData.MintBicycleDataContext())
                {
                    var querySms = (from sms in db.SmsValidateCode
                                    where (sms.Status == (byte)LoginEnum.Wait) && sms.Phone == phoneNo
                                    select sms).FirstOrDefault();

                    if (querySms != null)
                    {
                        querySms.ValidateCode = validateCode;
                        querySms.ExpiredTime = now.AddMinutes(10);
                    }
                    else
                    {
                        var newSms = new MintCyclingData.SmsValidateCode();
                        newSms.Guid = Guid.NewGuid();
                        newSms.Phone = phoneNo;
                        newSms.ValidateCode = validateCode;
                        newSms.Status = (byte)LoginEnum.Wait;
                        newSms.CheckDateTime = null;
                        newSms.ExpiredTime = now.AddMinutes(10);
                        newSms.CreateTime = now;

                        db.SmsValidateCode.InsertOnSubmit(newSms);
                    }

                    db.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ResObject = ResPrompt.SystemError;
                result.Message = ex.Message;

                return result;
            }

            return result;
        }

        /// <summary>
        /// 搜索手机号与验证码匹配记录
        /// </summary>
        /// <param name="data">参数集合</param>
        /// <returns>手机号与验证码匹配记录是否存在</returns>
        public ResultModel CheckPhoneNoAndValidateCode(ValidateLogin_PM model)
        {
            var result = new ResultModel();
            var now = DateTime.Now;

            using (var db = new MintCyclingData.MintBicycleDataContext())
            {
                //判断是否验证码过期ExpiredTime
                var querySms = (from sms in db.SmsValidateCode
                                    //where (sms.Status == (byte)LoginEnum.Wait) && sms.Phone == model.PhoneNo && sms.ValidateCode == model.ValidateCode
                                    //&& sms.ExpiredTime >= now
                                where (sms.Status == (byte)LoginEnum.Wait) && sms.Phone == model.PhoneNo 
                                select sms).FirstOrDefault();

                if (querySms != null)
                {
                    //// 测试账号例外
                    //if (System.Web.Configuration.WebConfigurationManager.AppSettings["TestAccount"] == TestAccountStatusEnum.Enable.ToString())
                    //{
                    //    if (model.PhoneNo == System.Web.Configuration.WebConfigurationManager.AppSettings["TestAccountNo"])
                    //    {
                    //        result.MsgCode = ResPrompt.TestAccount;
                    //        result.Message = ResPrompt.TestAccountMessage;
                    //        result.ResObject = true;

                    //        return result;
                    //    }
                    //}
                    if (querySms.ValidateCode != model.ValidateCode)
                    {
                        result.MsgCode = ResPrompt.ValiCodeInputError;
                        result.Message = ResPrompt.ValiCodeInputErrorMessage;
                        result.ResObject = null;
                    }
                    else if (querySms.ExpiredTime <= now)
                    {
                        result.MsgCode = ResPrompt.ValiCodeNotError;
                        result.Message = ResPrompt.ValiCodeNotErrorMessage;
                        result.ResObject = null;
                    }
                    else
                    {
                        querySms.Status = (byte)LoginEnum.Checked;
                        db.SubmitChanges();
                        result.ResObject = true;
                    }

                }
                else
                {
                    result.MsgCode = ResPrompt.ValiCodeServiceError;
                    result.Message = ResPrompt.ValiCodeServiceErrorMessage;
                    result.ResObject = null;
                }
            }

            return result;
        }

        /// <summary>
        /// 添加或者修改用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel AddUserInfoOrUpdate(ValidateLogin_PM model)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var query = db.UserInfo.FirstOrDefault(x => x.Phone == model.PhoneNo);
                if (query == null)
                {
                    if (db.UserInfo.Any(x => x.Phone == model.PhoneNo))
                    {
                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.UsersNotExist, Message = ResPrompt.UsersNotExistMessage };
                    }
                    Guid UserInfoGuid = Guid.NewGuid();
                    var User = new UserInfo
                    {
                        UserInfoGuid = UserInfoGuid,
                        Phone = model.PhoneNo,
                        CreditScore = 100,          //信用积分
                        Status = 1,                //启用
                        PushId = model.PushId,    //推送的编号
                        CreateBy = UserInfoGuid,
                        UpdateTime = DateTime.Now,
                        CreateTime = DateTime.Now,
                        IsDeleted = false
                    };
                    db.UserInfo.InsertOnSubmit(User);
                    db.SubmitChanges();
                    result.ResObject = User.UserInfoGuid;
                }
                else  //修改
                {
                    query.PushId = model.PushId;  //修改推送编号
                    db.SubmitChanges();
                    result.ResObject = model.PushId;
                }
            }
            return result;
        }

        /// <summary>
        /// 添加用户过期信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ResultModel AddOrUpdateCustomerAccessToken(Guid userInfoGuid)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                if (userInfoGuid != null) //修改过期信息
                {
                    var query = db.CustomerAccessToken.FirstOrDefault(s => s.UserInfoGuid == userInfoGuid);
                    if (query != null) //修改过期信息
                    {
                        query.ProvisionalToken = Guid.NewGuid();
                        query.UserInfoGuid = userInfoGuid;
                        //query.ExpirationTime =
                        query.AccessCount = query.AccessCount + 1;
                        query.UpdateBy = userInfoGuid;
                        query.UpdateTime = DateTime.Now;
                        db.SubmitChanges();
                    }
                    else  //添加信息
                    {
                        if (System.Web.Configuration.WebConfigurationManager.AppSettings["ExpirationTime"] == "-1") //表示永不过期
                        {
                            DateTime dt = DateTime.Now.AddYears(10);
                            var User = new CustomerAccessToken
                            {
                                ProvisionalToken = Guid.NewGuid(),
                                AccessTokenGuid = Guid.NewGuid(),
                                UserInfoGuid = userInfoGuid,
                                ExpirationTime = dt,
                                AccessCount = 1,
                                CreateBy = Guid.NewGuid(),
                                UpdateTime = DateTime.Now,
                                CreateTime = DateTime.Now
                            };

                            db.CustomerAccessToken.InsertOnSubmit(User);
                            db.SubmitChanges();
                        }
                    }
                }
                result.ResObject = true;
            }
            return result;
        }

        /// <summary>
        /// 是否已注册过
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ResultModel GetUserInfoByPhone(ValidateLogin_PM data)
        {
            Guid UserGuid = new Guid("{00000000-0000-0000-0000-000000000000}");
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var query = db.UserInfo.FirstOrDefault(s => s.Phone == data.PhoneNo);
                if (query != null && query.Phone == data.PhoneNo && query.Status == 1)
                {
                    //return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.UsersNotExist, Message = ResPrompt.UsersNotExistMessage };
                    result.ResObject = query.UserInfoGuid;
                } else if (query != null && query.Phone == data.PhoneNo && query.Status == 0)
                {
                    result.ResObject = UserGuid;  //禁止用户
                }
                else
                {
                    result.ResObject = null;
                }
            }
            return result;
        }

        /// <summary>
        /// 登录成功后获取用户相关信息
        /// </summary>
        /// <param name="userInfoGuid"></param>
        /// <returns></returns>
        public ResultModel GetLoginSuccessUserInfoByUserGuid(Guid userInfoGuid)
        {
            var result = new ResultModel();

            try
            {
                using (var db = new MintBicycleDataContext())
                {
                    var query = (from users in db.UserInfo
                                 join a in db.UserAuthentication on users.UserInfoGuid equals a.UserInfoGuid into temp1
                                 from aa in temp1.DefaultIfEmpty()
                                 join t in db.BicycleLockInfo on users.LockGuid equals t.LockGuid into temp
                                 from tt in temp.DefaultIfEmpty()
                                 join g in db.CustomerAccessToken on users.UserInfoGuid equals g.UserInfoGuid
                                 where users.UserInfoGuid == userInfoGuid
                                 orderby users.CreateTime descending
                                 select new
                                 {
                                     LoginToken = g.ProvisionalToken,
                                     UserGuid = users.UserInfoGuid,
                                     StatusStr = GetAuthState(int.Parse(aa.status.ToString() == null ? "0" : aa.status.ToString())),
                                     DeviceNo = tt.DeviceNo == null ? "0" : tt.DeviceNo,
                                     AccessCount = g.AccessCount
                                 });
                    if (query.Any())
                    {
                        var sk = query.FirstOrDefault();
                        var dt = DateTime.Now;
                        var dt0 = dt.AddMinutes(-20);
                        var qReservation = db.Reservation.OrderByDescending(p => p.StartTme).FirstOrDefault(p => p.UserInfoGuid == sk.UserGuid && p.Status == 1 && p.StartTme >= dt0 && p.StartTme <= dt);
                        var model = new LoginSuccess_OM
                        {
                            LoginToken = sk.LoginToken,
                            UserInfoGuid = sk.UserGuid,
                            StatusStr = sk.StatusStr,
                            DeviceNo = sk.DeviceNo,
                            AccessCount = sk.AccessCount
                        };
                        if (qReservation != null)
                        {
                            model.ReservationBicycleNo = qReservation.DeviceNo;
                            model.ReservationStartTme = qReservation.StartTme.Value.ToString("yyyy-MM-dd HH:mm:ss");
                            model.ReservationAddress = qReservation.Address;
                            model.ReservationGuid = qReservation.ReservationGuid.ToString();
                        }
                        result.ResObject = model;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("登录成功后获取用户相关信息异常:" + ex.Message + "用户Guid：" + userInfoGuid, ex);
            }
            return result;
        }

        /// <summary>
        /// 认证状态
        /// </summary>
        /// <param name="st"></param>
        /// <returns></returns>
        public static string GetAuthState(int st)
        {
            //1无法开关锁、2无法骑行、3无法结算、4二维码受损、5其它问题
            var str = string.Empty;
            switch (st)
            {
                case 0:
                    str = "未认证";
                    break;

                case 1:
                    str = "已认证";
                    break;

                default:
                    str = "认证状态异常";
                    break;
            }
            return str;
        }



        /// <summary>
        /// 添加账户余额信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel AddAccountUserInfo(UserAccount_PM model)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var Account = new AccountInfo();
                Account.AccountGuid = Guid.NewGuid();
                Account.UserInfoGuid = model.UserInfoGuid;
                Account.BalanceAmount = 0;
                Account.UsableAmount = 0;
                Account.CreateTime = DateTime.Now;
                Account.CreateBy = model.UserInfoGuid;
                Account.IsDeleted = false;
                db.AccountInfo.InsertOnSubmit(Account);
                db.SubmitChanges();
            }
            return result;
        }



    }
}