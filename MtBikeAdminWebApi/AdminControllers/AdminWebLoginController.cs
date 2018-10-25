using Autofac;
using MintCyclingService.Admin;
using MintCyclingService.AdminAccessCode;
using MintCyclingService.Common;
using MintCyclingService.Utils;
using MtBikeAdminWebApi;
using MtBikeAdminWebApi.Filter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Utility.Common;

namespace MtBikeAdminWebApi.AdminControllers
{
    /// <summary>
    /// 管理员后台登录控制器
    /// </summary>
    public class AdminWebLoginController : ApiController
    {
        private IAdminService _adminService;
        private IAdminAccessCodeService _adminAccessCodeService;
        /// <summary>
        /// 管理员后台登录控制器初始化
        /// </summary>
        public AdminWebLoginController()
        {
            // DateTime time = DateTime.Parse("20170427193345");
            _adminService = AutoFacConfig.Container.Resolve<IAdminService>();
            _adminAccessCodeService = AutoFacConfig.Container.Resolve<IAdminAccessCodeService>();
        }

        /// <summary>
        /// 检验管理员登录 omplete TOM
        /// DATE:2017-02-20
        /// </summary>
        /// <param name="data">参数集合</param>
        /// <returns>检验管理员登录结果</returns>
        [HttpPost]
        public ResultModel CheckAdminLogin([FromBody]CheckAdminLogin_PM data)
        {
            var result = new ResultModel();
            var validateCode = HttpContext.Current.Cache.Get("ValidateCode") as string;

            if (string.IsNullOrEmpty(data.UserName))
            {
                result.IsSuccess = true;
                result.MsgCode = ResPrompt.UserNameCanNotEmpty;
                result.Message = ResPrompt.UserNameCanNotEmptyMessage;
                return result;
            }
            if (string.IsNullOrEmpty(data.Password))
            {
                result.IsSuccess = true;
                result.MsgCode = ResPrompt.PasswordCanNotEmpty;
                result.Message = ResPrompt.PasswordCanNotEmptyMessage;
                return result;
            }

            // 检验验证码是否过期
            if (validateCode == null)
            {
                result.IsSuccess = true;
                result.MsgCode = ResPrompt.ValidateCodeExpired;
                result.Message = ResPrompt.ValidateCodeExpiredMessage;
                return result;
            }

            // 检验验证码是否正确
            if (validateCode != data.ValidateCode.ToUpper())
            {
                result.IsSuccess = true;
                result.MsgCode = ResPrompt.ValiCodeNotMatch;
                result.Message = ResPrompt.ValiCodeNotMatchMessage;
                return result;
            }


            result = _adminService.Login(data.UserName, data.Password);

            if (result.MsgCode != "0") return result;

            var adminGuid = result.ResObject as AdminLoginData_OM ?? new AdminLoginData_OM { AdminGuid = Guid.Empty };
            var now = DateTime.Now;

            // 删除此管理员的其他AccessCode, 实现只能在一个页面登录
            _adminAccessCodeService.RemoveByAdminGuid(adminGuid.AdminGuid);

            // 获取AccessCode
            result = _adminAccessCodeService.GetNewAccessCode(adminGuid.AdminGuid);

            if (result.IsSuccess)
            {
                var model = new CheckAdminLogin_OM();
                model.AccessCode = Guid.Parse(result.ResObject.ToString());
                model.UserName = data.UserName;
                model.RoleName = adminGuid.RoleName;
                model.AdminGuid = Guid.Parse(adminGuid.AdminGuid.ToString());

                if (adminGuid.RoleGuid.HasValue)
                {
                    var sk = _adminService.GetAdminRolePermUrl(adminGuid.RoleGuid.Value);
                    if (sk.IsSuccess && sk.ResObject != null)
                    {
                        model.PermUrls = sk.ResObject as string;
                    }
                    var st = _adminService.GetAdminRoleMenu(adminGuid.RoleGuid.Value);
                    if (st.IsSuccess && st.ResObject != null)
                    {
                        model.Menus = st.ResObject as string;
                    }
                }
                result.ResObject = model;
            }

            return result;
        }

        /// <summary>
        /// 验证码 omplete TOM
        /// DATE:2017-02-24
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ResultModel VerifyImage()
        {

            //DateTime time =  DateTime.ParseExact("20170427193345","yyyyMMddHHmmss",System.Globalization.CultureInfo.CurrentCulture);
            var result = new ResultModel();

            string verificationCode = Helper.CreateVerificationText(4);
            Bitmap _img = Helper.CreateVerificationImage(verificationCode, 100, 30);
            _img.Save(HttpContext.Current.Response.OutputStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            HttpContext.Current.Cache.Remove("ValidateCode");
            HttpContext.Current.Cache.Insert("ValidateCode", verificationCode.ToUpper(), null, DateTime.Now.AddMinutes(1), TimeSpan.Zero);
            return result;
        }
    }
}
