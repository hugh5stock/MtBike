using Autofac;
using MintCyclingService.UserAccount;
using MtBikeAdminWebApi.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MtBikeAdminWebApi.AdminControllers
{
    /// <summary>
    /// 用户钱包控制器
    /// </summary>
    [CheckAdminAccessCodeFilter]
    public class WalletController : ApiController
    {
        IUserAccountService _userAccountService;

        /// <summary>
        /// 初始化用户钱包控制器
        /// </summary>
        public WalletController()
        {
            _userAccountService = AutoFacConfig.Container.Resolve<IUserAccountService>();
        }



    }
}
