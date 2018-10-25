using MintCyclingData;
using MintCyclingService.Common;
using MintCyclingService.Cycling;
using MintCyclingService.Utils;
using MintCyclingService.WeixinApliay;
using System;
using System.Linq;
using Utility.LogHelper;

namespace MintCyclingService.UserAccount
{
    public class UserAccountService : IUserAccountService
    {
        ICyclingService _cyclingService = new CyclingService();
        static readonly string YjAmoiunt = System.Web.Configuration.WebConfigurationManager.AppSettings["YjAmount"];   //押金

        /// <summary>
        /// 通过用户UserGuid查询处理跳转界面
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ResultModel GetUrlPageByUserInfoGuid(MyAccountUsableAmount_PM data)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                try
                {
                    //查询押金信息
                    var DepositQuery = db.Deposit.FirstOrDefault(x => x.UserInfoGuid == data.UserInfoGuid);
                    //是否认证
                    var UserAuth = db.UserAuthentication.FirstOrDefault(x => x.UserInfoGuid == data.UserInfoGuid && x.status == 1 && !x.IsDeleted);
                    //查询余额
                    var AccountQuery = db.AccountInfo.FirstOrDefault(x => x.UserInfoGuid == data.UserInfoGuid && !x.IsDeleted);


                    //1.没有押金，没有认证，跳认证流程的押金充值界面
                    if ((DepositQuery == null || DepositQuery.Amount == 0) && UserAuth == null)
                    {
                        return new ResultModel { IsSuccess = false, MsgCode = "1", Message = "没有押金，没有认证，跳认证流程的押金充值界面" };

                    }
                    else if ((DepositQuery != null || DepositQuery.Amount != 0) && UserAuth == null) //2.有押金，没有认证，跳认证流程的认证界面
                    {

                        return new ResultModel { IsSuccess = false, MsgCode = "2", Message = "有押金，没有认证，跳认证流程的认证界面" };
                    }
                    else if ((DepositQuery == null || DepositQuery.Amount == 0) && UserAuth != null) //3.没有押金，有认证，跳非认证流程的押金充值界面
                    {

                        return new ResultModel { IsSuccess = false, MsgCode = "3", Message = "没有押金，有认证，跳非认证流程的押金充值界面" };
                    }
                    else if ((DepositQuery != null || DepositQuery.Amount != 0) && (AccountQuery.UsableAmount.ToString() != "0.00"))  //4.有押金，有认证，检查余额是否为0，为零跳余额充值界面，不为零继续功能实现
                    {

                        return new ResultModel { IsSuccess = false, MsgCode = "4", Message = "有押金，有认证，账户余额不为0继续功能实现" };
                    }
                    else if ((DepositQuery != null || DepositQuery.Amount != 0) && UserAuth != null && (AccountQuery.UsableAmount.ToString() == "0.00"))  //4.有押金，有认证，检查余额是否为0，为零跳余额充值界面，不为零继续功能实现
                    {

                        return new ResultModel { IsSuccess = false, MsgCode = "5", Message = "有押金，有认证，账户余额为0跳余额充值界面" };
                    }
                }
                catch (Exception ex)
                {
                    return new ResultModel { IsSuccess = false, MsgCode = "10101", Message = "查询用户相关数据异常" + ex.Message };
                }
            }
            return result;
        }

        /// <summary>
        /// 通过用户的Guid查询用户的钱包余额和押金详情
        /// </summary>
        /// <param name="UserInfoGuid"></param>
        /// <returns></returns>
        public ResultModel GetUserAccountDetailByUserGuid(Guid UserInfoGuid)
        {
            var result = new ResultModel();

            try
            {
                using (var db = new MintBicycleDataContext())
                {
                    //查询余额和押金是否充足 && !dd.IsDeleted && account1.IsDeleted
                    var query = (from user in db.UserInfo
                                 join account in db.AccountInfo on user.UserInfoGuid equals account.UserInfoGuid into temp
                                 from account1 in temp.DefaultIfEmpty()
                                 join d in db.Deposit on user.UserInfoGuid equals d.UserInfoGuid into temp1
                                 from dd in temp1.DefaultIfEmpty()
                                 where user.UserInfoGuid == UserInfoGuid && !dd.IsDeleted
                                 select new
                                 {
                                     //DepositGuid = dd.DepositGuid,
                                     UsableAmount = Math.Round(decimal.Parse(account1.UsableAmount.ToString()), 2),
                                     YAmount = dd.Amount
                                 });
                    if (query.Any())
                    {
                        var sk = query.FirstOrDefault();
                        var model = new MyAccountUsableAmount_OM
                        {
                            UsableAmount = (sk.UsableAmount == null ? 0 : sk.UsableAmount),
                            YAmount = sk.YAmount == null ? 0 : sk.YAmount,
                        };
                        result.ResObject = model;
                    }//decimal.Round(decimal.Parse("0.3333333"),2)
                    else
                    {
                        var model = new MyAccountUsableAmount_OM
                        {
                            UsableAmount = 0,
                            YAmount = 0
                        };
                        result.ResObject = model;
                        //return new ResultModel() { IsSuccess = false, MsgCode = "519", Message = "暂无押金数据！" };
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("查询用户余额和押金是否充足异常:" + ex.Message + ",用户Guid:" + UserInfoGuid + "", ex);
                result.IsSuccess = false;
                result.Message = "查询用户余额和押金是否充足异常,请稍后！";
                result.ResObject = null;
            }
            return result;
        }

        /// <summary>
        /// 用户充值押金
        /// </summary>
        /// <param name="model"></param>
        /// <param name="Deposit"></param>
        /// <returns></returns>
        public ResultModel AddDeposit(AddDeposit_PM model, Deposit Deposit)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                //判断用户是否已充值押金
                if (db.Deposit.Any(x => x.UserInfoGuid == model.UserInfoGuid && (x.Amount != 0 && x.Amount == decimal.Parse(YjAmoiunt))))
                {
                    return new ResultModel() { IsSuccess = false, MsgCode = ResPrompt.UserDepositError, Message = ResPrompt.UserDepositFormatErrorMessage };
                }
                try
                {
                    #region 充值处理业务

                    var query = db.Deposit.FirstOrDefault(s => s.UserInfoGuid == model.UserInfoGuid && !s.IsDeleted);
                    if (query != null && query.Amount == 0)
                    {
                        query.Amount = model.Amount;
                        query.IsDeleted = false;
                        query.UpdateBy = model.UserInfoGuid;
                        query.UpdateTime = DateTime.Now;
                        db.SubmitChanges();
                        result.Message = "充押金成功!";
                        result.ResObject = true;
                    }
                    else
                    {
                        Deposit.DepositGuid = Guid.NewGuid();
                        Deposit.Amount = model.Amount;
                        Deposit.UserInfoGuid = model.UserInfoGuid;
                        Deposit.CreateBy = model.UserInfoGuid;
                        Deposit.CreateTime = DateTime.Now;
                        Deposit.IsDeleted = false;

                        db.Deposit.InsertOnSubmit(Deposit);
                        db.SubmitChanges();

                        result.Message = "充押金成功!";
                        result.ResObject = true;
                    }
                    //添加用户充值押金记录信息
                    #region 用户充值押金记录信息
                    try
                    {
                        UserDepositRechargeRecord UserDepositRecharge = new UserDepositRechargeRecord
                        {
                            UserDepositRechargeGuid = Guid.NewGuid(),
                            UserInfoGuid = model.UserInfoGuid,
                            Amount = model.Amount,
                            //充值的方式：1支付宝；2微信
                            RechargeType = model.RechargeType == 1 ? "支付宝" : "微信",
                            //充值的状态：0充值失败；1充值成功
                            Status = model.Status,
                            PayDate = DateTime.Now,
                            OutTradeNo = model.Out_trade_no,           //订单号码
                            Trade_no = model.Trade_no,               //支付宝交易号
                            Sign_type = model.Sign_type,            // 签名类型RSA2
                            Sign = model.Sign,                      // 签名
                            Remark = " 用户Guid为" + model.UserInfoGuid + "充值押金为：" + model.Amount + "元。",
                            CreateBy = model.UserInfoGuid,
                            CreateTime = DateTime.Now,
                            IsDeleted = false
                        };

                        db.UserDepositRechargeRecord.InsertOnSubmit(UserDepositRecharge);
                        db.SubmitChanges();
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("保存充值押金记录信息失败：当前用户Guid为" + model.UserInfoGuid + "", ex);
                        return new ResultModel { IsSuccess = false, MsgCode = "1103", Message = "保存充值押金记录信息失败" };
                    }
                    //result.ResObject = true; //表示充值成功
                    #endregion 用户充值押金记录信息
                }
                catch (Exception ex)
                {
                    return new ResultModel { IsSuccess = false, MsgCode = "404", Message = "充值押金失败" + ex.Message };
                }

                #endregion 充值处理业务
            }
            return result;
        }

        /// <summary>
        /// 用户退押金
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel RefundDeposit(RefundDeposit_PM model)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var query = db.Deposit.FirstOrDefault(s => s.UserInfoGuid == model.UserInfoGuid && !s.IsDeleted);
                try
                {
                    if (query != null)
                    {
                        query.IsDeleted = true;
                        query.Amount = 0;
                        query.UpdateBy = model.UserInfoGuid;
                        query.UpdateTime = DateTime.Now;
                        db.SubmitChanges();
                        result.ResObject = true;
                    }
                    #region 用户退押金记录信息
                    try
                    {
                        var UserDepositRecharge = new UserDepositRechargeRecord
                        {
                            UserDepositRechargeGuid = Guid.NewGuid(),
                            UserInfoGuid = model.UserInfoGuid,
                            Amount = 99,
                            //退款的方式：1支付宝；2微信
                            RechargeType = model.RechargeType == 1 ? "支付宝" : "微信",
                            //退款的状态：0退款失败；1退款成功
                            Status = model.Status,
                            PayDate = DateTime.Now,
                            OutTradeNo = model.Out_trade_no,        //订单号码
                            Trade_no = model.Trade_no,               //支付宝交易号
                            Sign_type = model.Sign_type,            // 签名类型RSA2
                            Sign = model.Sign,                      // 签名
                            Remark = " 用户Guid为" + model.UserInfoGuid + "退押金了99元。",
                            CreateBy = model.UserInfoGuid,
                            CreateTime = DateTime.Now,
                            IsDeleted = false
                        };

                        db.UserDepositRechargeRecord.InsertOnSubmit(UserDepositRecharge);
                        db.SubmitChanges();
                    }
                    catch (Exception ex)
                    {
                        return new ResultModel { IsSuccess = false, MsgCode = "1103", Message = "保存退押金记录信息失败" };
                    }
                    #endregion 用户退押金记录信息
                }
                catch (Exception ex)
                {
                    return new ResultModel { IsSuccess = false, MsgCode = "404", Message = "退押金失败" + ex.Message };
                }
            }
            return result;
        }

        /// <summary>
        /// 通过用户的Guid查询用户的押金余额
        /// </summary>
        /// <param name="UserInfoGuid"></param>
        /// <returns></returns>
        public ResultModel GetUserDepositByUserGuid(Guid UserInfoGuid)
        {
            var result = new ResultModel();
            try
            {
                using (var db = new MintBicycleDataContext())
                {
                    var deposit = db.Deposit.FirstOrDefault(a => a.UserInfoGuid == UserInfoGuid && !a.IsDeleted);
                    if (deposit == null)
                    {
                        var ndeposit = new Deposit()
                        {
                            DepositGuid = Guid.NewGuid(),
                            UserInfoGuid = UserInfoGuid,
                            Amount = 0,
                            CreateTime = DateTime.Now,
                            IsDeleted = false,
                        };
                        db.Deposit.InsertOnSubmit(ndeposit);
                        db.SubmitChanges();

                        result.ResObject = ndeposit;
                    }
                    else
                    {
                        result.ResObject = deposit;
                    }
                }
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Message = e.Message;
            }
            return result;
        }

        /// <summary>
        /// 检查用户的余额
        /// </summary>
        /// <param name="userGuid"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool CheckUserAvailableBalance(Guid userGuid, out string msg)
        {
            msg = string.Empty;
            var accountResultModel = GetAccountUsableAmountByUserGuid(new MyAccountUsableAmount_PM { UserInfoGuid = userGuid });
            if (!accountResultModel.IsSuccess)
            {
                msg = "无法获取可用余额";
                return false;
            }
            else
            {
                //获取用户的预约用车次数--第一次用户的账户余额为0时，可以预约用车，当预约次数大于2次，余额为负数时，提示
                var Count = _cyclingService.GetCountByUserInfoGuid(userGuid);

                MyAccountUsableAmount_OM accountOM = accountResultModel.ResObject as MyAccountUsableAmount_OM;
                //check minimum balance?
                if ((accountOM == null || accountOM.UsableAmount < 0) && Count > 1)
                {
                    msg = "余额不足，请及时充值";
                    return false;
                }
            }
            return true;
        }

        //检查用户的余额
        public bool CheckUserAvailableBalances(Guid userGuid, out string msg)
        {
            msg = "";
            bool flg = CheckUserAvailableBalance(userGuid, out msg);
            return flg;
        }

        /// <summary>
        /// 通过用户的Guid查询可用余额
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ResultModel GetAccountUsableAmountByUserGuid(MyAccountUsableAmount_PM data)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var query = (from account in db.AccountInfo
                             where account.UserInfoGuid == data.UserInfoGuid && !account.IsDeleted
                             select new
                             {
                                 account.UsableAmount
                             });
                if (query.Any())
                {
                    var sk = query.FirstOrDefault();
                    var model = new MyAccountUsableAmount_OM
                    {
                        UsableAmount = sk.UsableAmount
                    };
                    result.ResObject = model;
                }
            }
            return result;
        }

        /// <summary>
        /// 用户充值押金-主要用于支付宝充值成功后修改用户押金充值记录表信息
        /// </summary>
        public bool UserReOrUpdateDeposit(UserReDepositUpdate_PM model)
        {
            bool flag = false;
            using (var db = new MintBicycleDataContext())
            {
                var UserReQuery = db.UserDepositRechargeRecord.FirstOrDefault(s => s.OutTradeNo == model.out_trade_no && !s.IsDeleted);
                if (UserReQuery != null)
                {
                    UserReQuery.Status = model.Status;  //成功

                    //UserReQuery.Amount = decimal.Parse(total_amount);
                    UserReQuery.PayDate = model.gmt_payment;
                    //UserReQuery.Out_trade_no = out_trade_no;
                    UserReQuery.Trade_no = model.trade_no;
                    UserReQuery.Sign = model.sign;
                    UserReQuery.Sign_type = model.sign_type;
                    UserReQuery.UpdateTime = DateTime.Now;
                    db.SubmitChanges();

                    //押金信息
                    var query = db.Deposit.FirstOrDefault(s => s.UserInfoGuid == UserReQuery.UserInfoGuid && !s.IsDeleted);
                    if (query != null)
                    {
                        query.Amount = model.total_amount;
                        query.IsDeleted = false;
                        query.UpdateBy = UserReQuery.UserInfoGuid;
                        query.UpdateTime = DateTime.Now;
                        db.SubmitChanges();
                    }
                    flag = true;
                }
            }
            return flag;
        }


        /// <summary>
        /// 用户充值押金-主要用于微信充值成功后修改用户押金充值记录表信息
        /// </summary>
        public bool UserReOrUpdateDepositToWeiXinPay(NotifyWeinXinResultModel model)
        {
            bool flag = false;
            using (var db = new MintBicycleDataContext())
            {
                var UserReQuery = db.UserDepositRechargeRecord.FirstOrDefault(s => s.OutTradeNo == model.out_trade_no && !s.IsDeleted);
                if (UserReQuery != null)
                {
                    UserReQuery.Status = model.Status;  //成功
                    //UserReQuery.Amount = decimal.Parse(total_amount);
                    UserReQuery.PayDate = DateTime.ParseExact(model.time_end, "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);  //支付完成时间20170427193345   DateTime time =  DateTime.ParseExact("20170427193345","yyyyMMddHHmmss",System.Globalization.CultureInfo.CurrentCulture);
                    //UserReQuery.Out_trade_no = out_trade_no;
                    UserReQuery.Trade_no = model.transaction_id;
                    UserReQuery.Sign = model.sign;
                    UserReQuery.Sign_type = "MD5";
                    UserReQuery.Nonce_str = model.nonce_str;
                    UserReQuery.Openid = model.openid;
                    UserReQuery.Bank_type = model.bank_type;
                    UserReQuery.BankName = null;                      //暂时不用处理
                    UserReQuery.UpdateTime = DateTime.Now;
                    db.SubmitChanges();

                    //押金信息
                    var query = db.Deposit.FirstOrDefault(s => s.UserInfoGuid == UserReQuery.UserInfoGuid && !s.IsDeleted);
                    if (query != null)
                    {
                        query.Amount = decimal.Parse(model.total_fee.ToString()) / 100;  //金额
                        query.IsDeleted = false;
                        query.UpdateBy = UserReQuery.UserInfoGuid;
                        query.UpdateTime = DateTime.Now;
                        db.SubmitChanges();
                    }
                    flag = true;
                }
            }
            return flag;
        }

        /// <summary>
        /// 校验通知数据的正确性--支付宝
        /// </summary>
        /// <param name="out_trade_no"></param>
        /// <returns></returns>
        public ResultModel GetUserDepositByout_trade_no(string out_trade_no, string total_amount)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var UserDeposit = db.UserDepositRechargeRecord.FirstOrDefault(s => s.OutTradeNo == out_trade_no && !s.IsDeleted);
                if (UserDeposit != null)
                {
                    if (UserDeposit.OutTradeNo != out_trade_no)
                    {
                        result.IsSuccess = false;
                    }
                    if (UserDeposit.Amount != decimal.Parse(total_amount))
                    {
                        result.IsSuccess = false;
                    }
                    if (UserDeposit.Status == 1) //表示已充值成功
                    {
                        result.MsgCode = "1";
                        result.IsSuccess = true;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 校验通知数据的正确性--针对微信
        /// </summary>
        /// <param name="out_trade_no"></param>
        /// <returns></returns>
        public ResultModel WeixinGetUserDepositByout_trade_no(string out_trade_no, string total_amount)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var UserDeposit = db.UserDepositRechargeRecord.FirstOrDefault(s => s.OutTradeNo == out_trade_no && !s.IsDeleted);
                if (UserDeposit != null)
                {
                    if (UserDeposit.OutTradeNo != out_trade_no)
                    {
                        result.IsSuccess = false;
                    }
                    if (UserDeposit.Amount != decimal.Parse(total_amount) / 100)
                    {
                        result.IsSuccess = false;
                    }
                    if (UserDeposit.Status == 1) //表示已充值成功
                    {
                        result.MsgCode = "1";
                        result.IsSuccess = true;
                    }
                }
            }
            return result;
        }



        /// <summary>
        /// 给用户手机发送手机信息通用接口
        /// </summary>
        /// <param name="userinfguid"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public bool UsersSendSMS(Guid userinfguid, string out_trade_no, string T, string remark)
        {
            bool flg = false;
            var result = new ResultModel();

            using (var db = new MintBicycleDataContext())
            {
                if (userinfguid != null && T == "押金")
                {
                    var UserGuid = db.UserDepositRechargeRecord.FirstOrDefault(s => s.OutTradeNo == out_trade_no && !s.IsDeleted);
                    if (UserGuid != null)
                    {
                        var query = db.UserInfo.FirstOrDefault(s => s.UserInfoGuid == UserGuid.UserInfoGuid);
                        result = Utility.SMS.SMSUtility.SendSMS(query.Phone.ToString(), remark);
                        flg = true;
                    }
                    else
                    {
                        flg = false;
                    }
                }
                else if (userinfguid != null && T == "余额")
                {
                    var UserGuid = db.UserRechargeRecord.FirstOrDefault(s => s.OutTradeNo == out_trade_no && !s.IsDeleted);
                    if (UserGuid != null)
                    {
                        var query = db.UserInfo.FirstOrDefault(s => s.UserInfoGuid == UserGuid.UserInfoGuid);
                        result = Utility.SMS.SMSUtility.SendSMS(query.Phone.ToString(), remark);
                        flg = true;
                    }
                    else
                    {
                        flg = false;
                    }
                }
                else
                {
                    var userInfo = db.UserInfo.FirstOrDefault(s => s.UserInfoGuid == userinfguid && !s.IsDeleted);
                    if (userInfo != null)
                    {
                        result = Utility.SMS.SMSUtility.SendSMS(userInfo.Phone, remark);
                        flg = true;
                    }
                    else
                    {
                        flg = false;
                    }
                }
            }

            return flg;
        }
        
    }
}