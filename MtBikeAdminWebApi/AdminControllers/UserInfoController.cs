using Autofac;
using MintCyclingService.Common;
using MintCyclingService.Transaction;
using MintCyclingService.User;
using MintCyclingService.UserAccount;
using MintCyclingService.Utils;
using MtBikeAdminWebApi;
using MtBikeAdminWebApi.Filter;
using Newtonsoft.Json.Linq;
using System;
using System.Web.Http;

namespace MintCyclingWebApi.AdminControllers
{
    /// <summary>
    /// 用户管理控制器
    /// </summary>
    [CheckAdminAccessCodeFilter]
    public class UserInfoController : ApiController
    {
        IUserInfoService _userService;
        IUserAccountService _userAccountService;
        ITransactionInfoService _transacationService;

        /// <summary>
        /// 初始化单车控制器
        /// </summary>
        public UserInfoController()
        {
            _userService = AutoFacConfig.Container.Resolve<IUserInfoService>();
            _userAccountService = AutoFacConfig.Container.Resolve<IUserAccountService>();
            _transacationService = AutoFacConfig.Container.Resolve<ITransactionInfoService>();
        }

        #region 微信用户数据接口

        /// <summary>
        /// 微信用户登陆
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultModel Login([FromUri]string code)
        {
            var ures = _userService.AddOrUpdateMicroMsgUser(code);
            if (ures.IsSuccess)
            {
                var user = (MintCyclingData.UserInfo)ures.ResObject;
                var userGuid = user.UserInfoGuid;
                var dres = _userAccountService.GetUserDepositByUserGuid(userGuid);
                if (dres.IsSuccess)
                {
                    var deposit = (MintCyclingData.Deposit)dres.ResObject;

                    var data = new JObject
                    {
                        // 加密的UserInfoGuid
                        { "userid", EncryptTool.DefaultEncryptDES(userGuid.ToString()) },
                        // 手机号
                        { "hasphone", user.Phone == null ? "no" : "ok" },
                        // 押金余额
                        { "deposit", deposit.Amount }
                    };

                    return new ResultModel() { IsSuccess = true, ResObject = data };
                }
                else
                {
                    return dres;
                }
            }
            else
            {
                return ures;
            }
        }

        /// <summary>
        /// 微信用户信息授权
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultModel UserAuthentic([FromBody]MicroMsgUserInfo_PM mminfo)
        {
            return _userService.EditMicroMsgUserInfo(Guid.Parse(EncryptTool.DefaultDecryptDES(mminfo.userid)), mminfo.encryptedData, mminfo.iv);
        }

        /// <summary>
        /// 微信用户手机认证
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultModel PhoneAuthentic([FromUri]MicroMsgUserInfo_PM mminfo)
        {

        }

        /// <summary>
        /// 获取周围设备位置
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultModel GetPositions([FromUri]GNetsPosition_PM centerPos)
        {
            var result = new ResultModel();
            return result;
        }

        #endregion

        #region 后台用户管理接口
        /// <summary>
        /// 根据查询条件搜索用户列表  complete TOM
        /// DATE：2017-05-22
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultModel GetUserInfoByCondition([FromUri]AdminUserInfo_PM model)
        {
            return _userService.GetUserInfoList(model);
        }
        /// <summary>
        /// 通过用户的Guid查询个人信息
        /// 作者：TOM
        /// 时间：2017-05-23
        /// </summary>
        /// <param name="UserGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultModel GetUserByUserGuid(Guid UserGuid)
        {
            return _userService.GetUserByUserGuid(UserGuid);

        }


        /// <summary>
        /// 编辑用户信息    complete TOM
        /// DATE：2017-05-22
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultModel EditUserInfoByUserGuid([FromBody]EditUserInfo_PM model)
        {
            return _userService.EditUserInfoByUserGuid(model);
        }



        /// <summary>
        /// 删除用户信息    complete TOM
        /// DATE：2017-05-22
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultModel DeleteUserByUserGuid([FromBody]DeleteUserInfo_PM model)
        {
            return _userService.DeleteUserByUserGuid(model);
        }



        /// <summary>
        /// 锁定用户状态 complete TOM
        /// DATE：2017-05-23
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel LockUserStatusByUserGuid(LockUserInfo_PM model)
        {
            return _userService.LockUserStatusByUserGuid(model);
        }


        #endregion

        #region  用户交易记录管理

        /// <summary>
        /// 当前交易记录API列表 complete TOM
        ///  DATE：2017-05-24
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultModel GetTransactionList([FromBody]UserTransaction_PM model)
        {

            return _transacationService.GetTransactionList(model);

        }

        /// <summary>
        /// 手动还车详细  complete TOM
        /// DATE：2017-05-24
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultModel GetUserByUserGuid([FromBody]RetrunBicycle_PM model)
        {
            return _transacationService.GetUserByUserGuid(model);
        }


        /// <summary>
        /// 手动还车处理 complete TOM
        /// DATE：2017-05-23
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ResultModel ReturnCarByPhone([FromBody] ReturnCar_PM model)
        {
            return _userService.ReturnCarByPhone(model);
        }

        /// <summary>
        /// 历史交易记录API列表  complete TOM
        /// DATE：2017-05-24
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultModel GetHistoryUserTransactionList([FromBody]HistoryUserTransaction_PM model)
        {

            return _transacationService.GetHistoryUserTransactionList(model);

        }


        /// <summary>
        /// 用户充值押金交易记录API列表  complete TOM
        /// DATE：2017-05-25
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel GetUserDepositRechargeRecordList([FromUri]UserDepositRecharge_PM model)
        {

            return _transacationService.GetUserDepositRechargeRecordList(model);

        }

        /// <summary>
        /// 用户充值余额交易记录API列表  complete TOM
        /// DATE：2017-05-25
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel GetUserAccountRechargeRecordList([FromUri]UserDepositRecharge_PM model)
        {
            return _transacationService.GetUserAccountRechargeRecordList(model);
        }


        /// <summary>
        /// 用户退款交易记录API列表  complete TOM
        /// DATE：2017-05-25
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel GetUserRefundDepositRecordList([FromUri]UserDepositRecharge_PM model)
        {
            return _transacationService.GetUserRefundDepositRecordList(model);
        }


        #endregion

    }
}

