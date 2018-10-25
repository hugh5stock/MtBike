using Autofac;
using MintCyclingService.UserAccount;
using MintCyclingService.UserRecharge;
using MintCyclingService.Utils;
using MintCyclingWebApi.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MintCyclingWebApi.AppControllers
{
    /// <summary>
    /// 用户账户控制器
    /// </summary>
    [RequestCheck]
    public class accountController : ApiController
    {
        IUserAccountService _userAccountService;
        IUserRechargeRecordService _uerRechargeService;

        /// <summary>
        /// 初始化用户账户控制器
        /// </summary>
        public accountController()
        {
            _userAccountService = AutoFacConfig.Container.Resolve<IUserAccountService>();
            _uerRechargeService = AutoFacConfig.Container.Resolve<IUserRechargeRecordService>();
        }


        /// <summary>
        /// 查询用户钱包余额和押金是否充足 complete TOM
        /// DATE:2017-06-19
        /// </summary>
        /// <returns></returns>
       
        [HttpGet]
        public ResultModel GetUsableAmountByUserInfoGuid([FromUri]Guid UserInfoGuid)
        {

           return _userAccountService.GetUserAccountDetailByUserGuid(UserInfoGuid);

        }

        #region 用户充值相关方法废弃

        ///// <summary>
        ///// 用户充值押金[暂时废弃] complete TOM
        ///// DATE:2017-03-01
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //public ResultModel AddDeposit([FromBody]AddDeposit_PM data)
        //{
        //    var result = new ResultModel();
        //    result = _userAccountService.AddDeposit(data);
        //    return result;
        //}

        ///// <summary>
        ///// 用户退押金[暂时废弃]  complete TOM
        ///// DATE:2017-03-09
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //public ResultModel RefundDeposit([FromBody]RefundDeposit_PM data)
        //{
        //    var result = new ResultModel();
        //    result = _userAccountService.RefundDeposit(data);
        //    return result;
        //}


        ///// <summary>
        ///// 用户钱包充值[暂时废弃]  complete TOM
        ///// DATE:2017-03-09
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //public ResultModel AddUserChargeRecor([FromBody]UserRechargeRecordModel_PM data)
        //{
        //    var result = new ResultModel();
        //    result = _uerRechargeService.AddUserRechargeRecord(data);
        //    return result;
        //}



        ///// <summary>
        ///// App界面跳转功能[暂时废弃]   complete TOM 
        ///// DATE:2017-03-03
        ///// </summary>
        ///// <param name="data"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public ResultModel GetPageJumpByUserInfoGuid([FromBody]MyAccountUsableAmount_PM data)
        //{
        //    var result = new ResultModel();
        //    result = _userAccountService.GetUrlPageByUserInfoGuid(data);
        //    return result;
        //}

        #endregion



    }
}


