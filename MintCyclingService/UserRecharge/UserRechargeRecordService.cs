using MintCyclingData;
using MintCyclingService.UserAccount;
using MintCyclingService.Utils;
using MintCyclingService.WeixinApliay;
using System;
using System.Linq;

namespace MintCyclingService.UserRecharge
{
    public class UserRechargeRecordService : IUserRechargeRecordService
    {
        /// <summary>
        /// 用户充值余额记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel AddUserRechargeRecord(UserRechargeRecordModel_PM model)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                try
                {
                    #region
                    if (model.UserInfoGuid != null && model.Amount != null || model.Amount != 0)
                    {
                        var query = db.AccountInfo.FirstOrDefault(s => s.UserInfoGuid == model.UserInfoGuid);

                        #region 充值余额信息表
                        if (query != null)
                        {
                            query.BalanceAmount += model.Amount;
                            query.UsableAmount += model.Amount;
                            query.UpdateBy = model.UserInfoGuid;
                            query.UpdateTime = DateTime.Now;
                            db.SubmitChanges();
                        }
                        else
                        {
                            #region 充值账户信息表

                            var Account = new AccountInfo();
                            Account.AccountGuid = Guid.NewGuid();
                            Account.UserInfoGuid = model.UserInfoGuid;
                            Account.BalanceAmount = model.Amount;
                            Account.UsableAmount = model.Amount;

                            Account.CreateTime = DateTime.Now;
                            Account.CreateBy = model.UserInfoGuid;
                            Account.IsDeleted = false;

                            db.AccountInfo.InsertOnSubmit(Account);
                            db.SubmitChanges();

                            #endregion 充值账户信息表
                        }

                        #endregion

                        #region 充值钱包金额记录表
                        try
                        {
                            //充值钱包金额记录表
                            var UserRechargeRecord = new UserRechargeRecord();
                            UserRechargeRecord.UserRechargeGuid = Guid.NewGuid();
                            UserRechargeRecord.UserInfoGuid = model.UserInfoGuid;
                            UserRechargeRecord.Amount = model.Amount;
                            //充值的方式：1支付宝；2微信
                            UserRechargeRecord.RechargeType = model.RechargeType == 1 ? "支付宝" : "微信";
                            //充值的状态：0充值失败；1充值成功
                            UserRechargeRecord.Status = model.Status;
                            UserRechargeRecord.PayDate = DateTime.Now;
                            UserRechargeRecord.OutTradeNo = model.Out_trade_no;      //商户唯一订单号码
                            UserRechargeRecord.Trade_no = model.Trade_no;               //支付宝交易号
                            UserRechargeRecord.Sign_type = model.Sign_type;            // 签名类型RSA2
                            UserRechargeRecord.Sign = model.Sign;                      // 签名
                            UserRechargeRecord.Remark = " 用户Guid为" + model.UserInfoGuid + "充值钱包余额为：" + model.Amount + "元。";
                            UserRechargeRecord.CreateBy = model.UserInfoGuid;
                            UserRechargeRecord.CreateTime = DateTime.Now;
                            UserRechargeRecord.IsDeleted = false;

                            db.UserRechargeRecord.InsertOnSubmit(UserRechargeRecord);
                            db.SubmitChanges();
                        }
                        catch (Exception ex)
                        {
                            return new ResultModel { IsSuccess = false, MsgCode = "1103", Message = "保存充值记录信息失败" };
                        }
                        //result.ResObject = true; //表示充值成功
                        #endregion 充值钱包金额记录表

                       // return new ResultModel { IsSuccess = true, MsgCode = "0", Message = "充值金额成功！" };
                        result.IsSuccess = true;
                        result.MsgCode = "0";
                        result.Message = "充值金额成功!";
                        result.ResObject = null;
                    }
                    else
                    {
                        return new ResultModel { IsSuccess = false, MsgCode = "1101", Message = "用户的Guid和金额不能为空" };
                    }
                    #endregion

                }
                catch (Exception ex)
                {
                    return new ResultModel { IsSuccess = false, MsgCode = "1100", Message = "充值金额失败，请重新充值！" };
                }
            }
            return result;
        }

        /// <summary>
        /// 修改用户账户余额订单状态信息--针对支付宝
        /// </summary>
        public bool UserRechargeOrAccountInfo(UserReDepositUpdate_PM model)
        {
            bool flag = false;
            using (var db = new MintBicycleDataContext())
            {
                //修改账户信息和订单状态
                var UserReQuery = db.UserRechargeRecord.FirstOrDefault(s => s.OutTradeNo == model.out_trade_no);
                if (UserReQuery != null)
                {
                    UserReQuery.Status = model.Status;
                    //UserReQuery.Amount = decimal.Parse(total_amount);
                    UserReQuery.PayDate = model.gmt_payment;
                    //UserReQuery.Out_trade_no = out_trade_no;
                    UserReQuery.Trade_no = model.trade_no;
                    UserReQuery.Sign = model.sign;
                    UserReQuery.Sign_type = model.sign_type;
                    UserReQuery.UpdateTime = DateTime.Now;
                    db.SubmitChanges();

                    //账户信息
                    var QueryAccount = db.AccountInfo.FirstOrDefault(s => s.UserInfoGuid == UserReQuery.UserInfoGuid);
                    if (QueryAccount != null)
                    {
                        QueryAccount.BalanceAmount += model.total_amount;
                        QueryAccount.UsableAmount += model.total_amount;
                        QueryAccount.UpdateBy = UserReQuery.UserInfoGuid;
                        QueryAccount.UpdateTime = DateTime.Now;
                        db.SubmitChanges();
                    }

                }
                flag = true;
                }
            return flag;
        }



        /// <summary>
        /// 修改用户账户余额订单状态信息--针对微信
        /// </summary>
        public bool UserRechargeOrAccountInfoWeiXinPay(NotifyWeinXinResultModel model)
        {
            bool flag = false;
            using (var db = new MintBicycleDataContext())
            {
                //修改账户信息和订单状态
                var UserReQuery = db.UserRechargeRecord.FirstOrDefault(s => s.OutTradeNo == model.out_trade_no);
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
                    UserReQuery.BankName = null;  //暂时不用处理


                    db.SubmitChanges();

                    //账户信息
                    var QueryAccount = db.AccountInfo.FirstOrDefault(s => s.UserInfoGuid == UserReQuery.UserInfoGuid);
                    if (QueryAccount != null)
                    {
                        QueryAccount.BalanceAmount +=decimal.Parse(model.total_fee.ToString())/100;
                        QueryAccount.UsableAmount += decimal.Parse(model.total_fee.ToString())/100;
                        QueryAccount.UpdateBy = UserReQuery.UserInfoGuid;
                        QueryAccount.UpdateTime = DateTime.Now;
                        db.SubmitChanges();
                    }
                }
                flag = true;
            }
            return flag;
        }

        /// <summary>
        /// 校验通知数据的正确性
        /// </summary>
        /// <param name="out_trade_no">订单号码</param>
        /// <returns></returns>
        public ResultModel GetUserRechargeRecordByout_trade_no(string out_trade_no, string total_amount)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var UserRecharge = db.UserRechargeRecord.FirstOrDefault(s => s.OutTradeNo == out_trade_no);
                if (UserRecharge != null)
                {
                    if (UserRecharge.OutTradeNo != out_trade_no)
                    {
                        result.IsSuccess = false;
                    }
                    if (UserRecharge.Amount != decimal.Parse(total_amount))
                    {
                        result.IsSuccess = false;
                    }

                    if (UserRecharge.Status == 1) //表示已充值成功
                    {
                        result.IsSuccess = true;
                        result.MsgCode = "1";
                    }
                }
            }
            return result;

        }

        /// <summary>
        /// 校验通知数据的正确性
        /// </summary>
        /// <param name="out_trade_no">订单号码</param>
        /// <returns></returns>
        public ResultModel WeixinGetUserRechargeRecordByout_trade_no(string out_trade_no, string total_amount)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var UserRecharge = db.UserRechargeRecord.FirstOrDefault(s => s.OutTradeNo == out_trade_no);
                if (UserRecharge != null)
                {
                    if (UserRecharge.OutTradeNo != out_trade_no)
                    {
                        result.IsSuccess = false;
                    }
                    if (UserRecharge.Amount != decimal.Parse(total_amount)/100)
                    {
                        result.IsSuccess = false;
                    }

                    if (UserRecharge.Status == 1) //表示已充值成功
                    {
                        result.IsSuccess = true;
                        result.MsgCode = "1";
                    }
                }
            }
            return result;

        }


    }
}