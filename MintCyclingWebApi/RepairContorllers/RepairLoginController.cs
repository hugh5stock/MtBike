using Autofac;
using MintCyclingService.AdminAccessCode;
using MintCyclingService.Common;
using MintCyclingService.ServicePerson;
using MintCyclingService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MintCyclingWebApi.RepairContorllers
{
    public class RepairLoginController : ApiController
    {
        private IServicePersonaService ServicePersonaService;
        private IAdminAccessCodeService _adminAccessCodeService;
        public RepairLoginController()
        {

            ServicePersonaService = AutoFacConfig.Container.Resolve<IServicePersonaService>();
            _adminAccessCodeService = AutoFacConfig.Container.Resolve<IAdminAccessCodeService>();

        }

        /// <summary>
        /// 维护人员登录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultModel Login([FromBody]LoginModel_PM data)
        {

            var res = ServicePersonaService.Login(data);

            if (res.IsSuccess == false)
            {

                return new ResultModel { IsSuccess=false, MsgCode = res.MsgCode, Message = res.Message };

            }

            var user = (res.ResObject as LoginModel_OM);

            // 删除此管理员的其他AccessCode, 实现只能在一个页面登录
            _adminAccessCodeService.RemoveByRepiarGuid(user.UserInfoGuid);

            // 获取AccessCode
            var result = _adminAccessCodeService.GetRepiarNewAccessCode(user.UserInfoGuid);
            ///修改用户pushid
            ServicePersonaService.SetpushIdByUserGuid(user.UserInfoGuid, data.PushId);


            if (result.IsSuccess == true)
            {

                user.AccessCode = Guid.Parse(result.ResObject.ToString());

                return new ResultModel { ResObject = user };
            }

            return new ResultModel {  IsSuccess=false,MsgCode = ResPrompt.UserTokenNotEixist,   Message=ResPrompt.UserTokenNotEixistMessage };


        }
       


    }
}
