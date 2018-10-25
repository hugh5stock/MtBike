using Autofac;
using MintCyclingService.Common;
using MintCyclingService.Cycling;
using MintCyclingService.Login;
using MintCyclingService.Utils;
using MintCyclingWebApi.Filter;
using System;
using System.Web.Http;
using Utility.LogHelper;
using Utility.SMS;

namespace MintCyclingWebApi.AppControllers
{
    /// <summary>
    /// 用户登录相关的控制器
    /// </summary>
    public class loginController : ApiController
    {

        private ILoginService _loginService;
        private ICyclingService _cyclingService;

        /// <summary>
        /// 初始化用户登录相关的控制器
        /// </summary>
        public loginController()
        {
            _loginService = AutoFacConfig.Container.Resolve<ILoginService>();
            _cyclingService = AutoFacConfig.Container.Resolve<ICyclingService>();
        }

        #region 用户相关接口

        /// <summary>
        /// 用户登录   omplete TOM
        /// DATE:2017-02-27
        /// </summary>
        /// <param name="data">参数集合</param>
        /// <returns>用户登录</returns>
        [HttpPost]
        public ResultModel ValidateLogin([FromBody]ValidateLogin_PM data)
        {
            var result = new ResultModel();

            try
            {
                if (data.PhoneNo.IndexOf("00000000") > -1) //表示测试账号
                {
                    //string str = "{00000000-0000-0000-0000-000000000003}";
                    Guid UserGuid = new Guid();
                    //判断是否已注册
                    var UsersGuid = _loginService.GetUserInfoByPhone(data).ResObject as Guid?;
                    if (UsersGuid != null)
                    {
                        //修改推送编号
                        var PushId = _loginService.AddUserInfoOrUpdate(data);
                        if (PushId == null)
                        {
                            return new ResultModel { IsSuccess = false, MsgCode = "2010", Message = "推送编号不能为空！" };
                        }
                    }
                    //修改过期信息
                    result = _loginService.AddOrUpdateCustomerAccessToken(UsersGuid.Value);
                    if (!result.IsSuccess)
                    {
                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.CustomerAccessTokenNotError, Message = ResPrompt.CustomerAccessTokenMessage };
                    }

                    //处理用户预约是否超时
                    result = _cyclingService.UserReservationOvertime(UsersGuid.Value);
                    //登录成功数据输出
                    result = _loginService.GetLoginSuccessUserInfoByUserGuid(UsersGuid.Value);
                }
                else
                {
                    #region 正式环境
                    // 验证手机号和验证码是否正确
                    result = _loginService.CheckPhoneNoAndValidateCode(data);
                    if (result.IsSuccess && result.ResObject != null)
                    {
                        Guid uGuids = new Guid("{00000000-0000-0000-0000-000000000000}");
                        //判断是否已注册
                        var UsersGuid = _loginService.GetUserInfoByPhone(data).ResObject as Guid?;
                        if (UsersGuid != null && UsersGuid != uGuids)
                        {
                            #region 已注册
                            //修改推送编号
                            var PushId = _loginService.AddUserInfoOrUpdate(data);
                            if (PushId == null)
                            {
                                return new ResultModel { IsSuccess = false, MsgCode = "2010", Message = "推送编号不能为空！" };
                            }

                            //类型转换-用户Guid
                            //var UserGuid = UsersGuid.ResObject as Guid?;
                            //修改过期信息
                            result = _loginService.AddOrUpdateCustomerAccessToken(UsersGuid.Value);
                            if (!result.IsSuccess)
                            {
                                return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.CustomerAccessTokenNotError, Message = ResPrompt.CustomerAccessTokenMessage };
                            }
                            //处理用户预约是否超时
                            result = _cyclingService.UserReservationOvertime(UsersGuid.Value);

                            //登录成功数据输出
                            result = _loginService.GetLoginSuccessUserInfoByUserGuid(UsersGuid.Value);

                            #endregion
                        }
                        else if (UsersGuid != null && UsersGuid == uGuids)
                        {
                            return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.StopUserNotError, Message = ResPrompt.StartUserNotRegisterMessage };
                        }
                        else
                        {
                            #region 未注册
                            //手机号和验证码通过验证后添加用户手机信息
                            var userInfoGuid = _loginService.AddUserInfoOrUpdate(data); //添加用户信息成功后返回用户的Guid
                            if (userInfoGuid == null) //添加失败时
                            {
                                return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.UserinfoNotError, Message = ResPrompt.UserinfoNotRegisterMessage };
                            }

                            //类型转换
                            var UserGuid = userInfoGuid.ResObject as Guid?;
                            if (UserGuid == null) //添加失败时
                            {
                                return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.UserinfoNotError, Message = ResPrompt.UserinfoNotRegisterMessage };
                            }
                            //添加账户余额信息
                            UserAccount_PM account = new UserAccount_PM();
                            account.UserInfoGuid = UserGuid;
                            result = _loginService.AddAccountUserInfo(account);
                            if (!result.IsSuccess)
                            {
                                return new ResultModel { IsSuccess = false, MsgCode = "0", Message = "登录-添加账户余额信息失败" };
                            }


                            //添加过期信息
                            result = _loginService.AddOrUpdateCustomerAccessToken(UserGuid.Value);
                            if (!result.IsSuccess)
                            {
                                return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.CustomerAccessTokenNotError, Message = ResPrompt.CustomerAccessTokenMessage };
                            }

                            //登录成功数据输出
                            result = _loginService.GetLoginSuccessUserInfoByUserGuid(UserGuid.Value);

                            #endregion
                        }
                    }
                    else
                    {
                        // 登录失败
                        result.IsSuccess = false;
                    }

                    #endregion

                
                }
            } catch (Exception ex)
            {
                LogHelper.Error("用户登录异常，请稍后：手机号码" + data.PhoneNo + "", ex);
            }
            return result;
        }

 
        /// <summary>
        /// 获取手机验证码 complete TOM
        /// DATE:2017-02-27
        /// </summary>
        /// <param name="data">参数集合</param>
        /// <returns>获取手机验证码</returns>
        [HttpPost]
        public ResultModel SendSMSForValidateCode([FromBody]SendSMSForValidateCode_PM data)
        {
            var result = new ResultModel();

            // 生成验证码
            var code = Utility.Common.Helper.GetRandomNumString(6);

            
            result = Utility.SMS.SMSUtility.SendSMS(data.phoneNo, "【xxxxxxx】您的验证码是\"" + code + "\",有效期10分钟内请在页面完成验证码验证。");

            if (result.IsSuccess)
            {
                result = _loginService.AddSmsValidateCode(data.phoneNo, code);
            }
            else if (result.Message == "107")
            {
                result.MsgCode = ResPrompt.PhoneNoFormatError;
                result.Message = ResPrompt.PhoneNoFormatErrorMessage;
            }
            result.ResObject = code;
            return result;
        }

        /// <summary>
        /// 获取页面协议URL   complete TOM
        /// DATE:2017-03-31
        /// </summary>
        /// <param name="data">类别：typeName=RechargePro表示充值协议;typeName=DepositPro押金协议;typeName=RentPro租赁协议;</param>
        /// <returns></returns>
        [HttpPost]
        public ResultModel GetProtocolUrl([FromBody]ProUrl_PM data)
        {
            var result = new ResultModel();
            if (string.IsNullOrEmpty(data.typeName))
            {
                result.MsgCode = ResPrompt.ParaModelNotExist;
                result.Message = ResPrompt.ParaModelNotExistMessage;
            }

            string ProUrl = System.Web.Configuration.WebConfigurationManager.AppSettings["ProUrl"];          
            string Url = string.Empty;
            if (data.typeName == "RechargePro")
            {
                Url = ProUrl + "czxy.html";
            } else if (data.typeName == "DepositPro")
            {
                Url = ProUrl + "yjxy.html";
            }
            else if (data.typeName == "RentPro")
            {
                Url = ProUrl + "zlfw.html";
            }
            result.ResObject = Url;
            return result;
        }

        #endregion



    }
}