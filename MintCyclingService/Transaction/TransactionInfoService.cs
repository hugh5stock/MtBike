using MintCyclingData;
using MintCyclingService.Common;
using MintCyclingService.User;
using MintCyclingService.Utils;
using System;
using System.Linq;
using Utility;

namespace MintCyclingService.Transaction
{
    public class TransactionInfoService : ITransactionInfoService
    {
        /// <summary>
        /// 当前交易记录API列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel GetTransactionList(UserTransaction_PM model)
        {
            var result = new ResultModel();
            var region = Config.UserRegion;
            if (region == null)
                return result;
            using (var db = new MintBicycleDataContext())
            {
                try
                {
                    var query = (from s in db.MyTravel
                                 join u in db.UserInfo on s.UserInfoGuid equals u.UserInfoGuid
                                 join q in db.AccountInfo on u.UserInfoGuid equals q.UserInfoGuid
                                 join y in db.BicycleLockInfo on s.LockGuid equals y.LockGuid
                                 join t in db.TransactionInfo on new { MyTravelGuid = s.MyTravelGuid, UserInfoGuid = u.UserInfoGuid } equals new { MyTravelGuid = t.MyTravelGuid, UserInfoGuid = t.UserInfoGuid } into tmp
                                 from temp in tmp.DefaultIfEmpty()
                                 where ((string.IsNullOrEmpty(model.Phone)) || u.Phone.Contains(model.Phone))
                                  && ((string.IsNullOrEmpty(model.DeviceNo)) || y.LockNumber.Contains(model.DeviceNo))
                                    && !u.IsDeleted && s.EndTime == null && s.StartTime != null
                                  && (region.ExceptUserRegion || (u.ProvinceID == region.UserProvince && (region.UserCity == null || u.CityID == region.UserCity)
                                   && (region.UserDistrict == null || u.DistrictID == region.UserDistrict)))
                                 orderby u.CreateTime descending
                                 select new UserTransactionInfo_PM
                                 {
                                     UserinfoGuid = u.UserInfoGuid,
                                     Phone = u.Phone,
                                     BicycleNumber = y.LockNumber,
                                     StartTime = s.StartTime == null ? DateTime.Now : s.StartTime,
                                     TotalAmout = temp == null ? 0m : temp.TotalAmount,
                                     UsableAmount = q.UsableAmount,
                                     ReturnStatus = s.Status == 0 ? "手动还车" : "自动还车",
                                     CreateTime = u.CreateTime,
                                 }).Distinct();
                    if (query.Any())
                    {
                        var list = query.ToList();
                        var cnt = list.Count();
                        var tsk = new PagedList<UserTransactionInfo_PM>(query, model.PageIndex, model.PageSize, query.Count());
                        result.ResObject = new { Total = query.Count(), List = tsk };
                    }
                }
                catch (Exception ex)
                {
                    return new ResultModel() { IsSuccess = false, MsgCode = ResPrompt.NoUserTransationCode, Message = ResPrompt.NoUserTransationMessage + ex.Message };
                }
                return result;
            }
        }

        /// <summary>
        /// 手动还车详细
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel GetUserByUserGuid(RetrunBicycle_PM model)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                try
                {
                    var query = (from u in db.UserInfo
                                 join b in db.BicycleLockInfo on u.LockGuid equals b.LockGuid into tep
                                 from bb in tep.DefaultIfEmpty()
                                 join m in db.MyTravel on u.UserInfoGuid equals m.UserInfoGuid into mtep
                                 from mm in mtep.DefaultIfEmpty()
                                 where u.UserInfoGuid == model.UserInfoGuid && !u.IsDeleted
                                 orderby u.CreateTime descending
                                 select new
                                 {
                                     UserinfoGuid = u.UserInfoGuid,
                                     Phone = u.Phone,
                                     BicycleNumber = bb.LockNumber,
                                     StartTime = mm.StartTime == null ? DateTime.Now : mm.StartTime,
                                     EndTime = mm.EndTime == null ? DateTime.Now : mm.EndTime,
                                 });
                    var sk = query.FirstOrDefault();
                    var userModel = new UserTransactionInfo_PM
                    {
                        UserinfoGuid = sk.UserinfoGuid,
                        Phone = sk.Phone,
                        BicycleNumber = sk.BicycleNumber,
                        StartTime = sk.StartTime,
                        EndTime = sk.EndTime
                    };
                    result.ResObject = userModel;
                }
                catch (Exception ex)
                {
                    return new ResultModel() { IsSuccess = false, MsgCode = ResPrompt.NoUserTransationCode, Message = ResPrompt.NoUserTransationMessage + ex.Message };
                }
            }
            return result;
        }

        /// <summary>
        /// 历史交易记录API列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel GetHistoryUserTransactionList(HistoryUserTransaction_PM model)
        {
            var result = new ResultModel();
            var region = Config.UserRegion;
            if (region == null)
                return result;
            using (var db = new MintBicycleDataContext())
            {
                try
                {
                    var returnStatus = -1;
                    if (!int.TryParse(model.ReturnStatus, out returnStatus))
                    {
                        returnStatus = -1;
                    }
                    var query = (from s in db.MyTravel
                                 join u in db.UserInfo on s.UserInfoGuid equals u.UserInfoGuid
                                 join q in db.AccountInfo on u.UserInfoGuid equals q.UserInfoGuid
                                 join y in db.BicycleLockInfo on s.LockGuid equals y.LockGuid
                                 join t in db.TransactionInfo on new { MyTravelGuid = s.MyTravelGuid, UserInfoGuid = u.UserInfoGuid } equals new { MyTravelGuid = t.MyTravelGuid, UserInfoGuid = t.UserInfoGuid } into tmp
                                 from temp in tmp.DefaultIfEmpty()
                                 where ((string.IsNullOrEmpty(model.Phone)) || u.Phone.Contains(model.Phone))
                                 && ((string.IsNullOrEmpty(model.DeviceNo)) || y.LockNumber.Contains(model.DeviceNo))
                                 && !u.IsDeleted && ((returnStatus == -1) || s.Status == returnStatus) && s.EndTime != null && s.StartTime != null
                                 && (region.ExceptUserRegion || (u.ProvinceID == region.UserProvince && (region.UserCity == null || u.CityID == region.UserCity)
                                   && (region.UserDistrict == null || u.DistrictID == region.UserDistrict)))
                                 orderby u.CreateTime descending
                                 select new UserTransactionInfo_PM
                                 {
                                     UserinfoGuid = u.UserInfoGuid,
                                     Phone = u.Phone,
                                     BicycleNumber = y.LockNumber,
                                     StartTime = s.StartTime == null ? DateTime.Now : s.StartTime,
                                     EndTime = s.EndTime == null ? DateTime.Now : s.EndTime,
                                     TotalAmout = temp == null ? 0m : temp.TotalAmount,
                                     UsableAmount = q.UsableAmount,
                                     ReturnStatus = s.Status == 0 ? "手动还车" : "自动还车",
                                     CreateTime = u.CreateTime,
                                 }).Distinct();
                    if (query.Any())
                    {
                        var list = query;
                        var cnt = list.Count();
                        var tsk = new PagedList<UserTransactionInfo_PM>(query, model.PageIndex, model.PageSize, cnt);
                        result.ResObject = new { Total = cnt, List = tsk };
                    }
                }
                catch (Exception ex)
                {
                    return new ResultModel() { IsSuccess = false, MsgCode = ResPrompt.NoUserTransationCode, Message = ResPrompt.NoUserTransationMessage + ex.Message };
                }
            }
            return result;
        }

        /// <summary>
        /// 用户充值押金交易记录API列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel GetUserDepositRechargeRecordList(UserDepositRecharge_PM model)
        {
            var result = new ResultModel();
            var region = Config.UserRegion;
            if (region == null)
                return result;
            string rechargeType = string.Empty;
            using (var db = new MintBicycleDataContext())
            {
                try
                {
                    if (model.RechargeType == 1)
                    {
                        rechargeType = "支付宝";
                    }
                    else if (model.RechargeType == 2)
                    {
                        rechargeType = "微信";
                    }
                    var IsRecharge = model.IsRecharge;
                    var query = (from d in db.UserDepositRechargeRecord
                                 join u in db.UserInfo on d.UserInfoGuid equals u.UserInfoGuid into tep
                                 from uu in tep.DefaultIfEmpty()
                                 where ((string.IsNullOrEmpty(model.Phone)) || (uu != null && uu.Phone != null && uu.Phone != "" && uu.Phone.Contains(model.Phone)))
                                 && (string.IsNullOrEmpty(rechargeType) || d.RechargeType.Contains(rechargeType))
                                 && (d.PayDate.Value.Date >= model.StartTime.Date && d.PayDate.Value.Date <= model.EndTime.Date)
                                 && (IsRecharge == -1 || d.Status == IsRecharge)
                                 && d.MoneyType == "1"
                                 && !d.IsDeleted
                                 && (region.ExceptUserRegion || (uu.ProvinceID == region.UserProvince && (region.UserCity == null || uu.CityID == region.UserCity)
                                   && (region.UserDistrict == null || uu.DistrictID == region.UserDistrict)))
                                 orderby d.CreateTime descending
                                 select new UserDepositRechargeList_OM
                                 {
                                     UserRechargeGuid = d.UserDepositRechargeGuid,
                                     Phone = uu.Phone,
                                     RechargeType = d.RechargeType,
                                     MoneyType = d.MoneyType == "1" ? "充值" : "退款",
                                     RechargeStatus = RecharStatusStr(d.Status),
                                     PayDate = d.PayDate,
                                     Trade_no = d.Trade_no,
                                     OutTradeNo = d.OutTradeNo,
                                     Remark = d.Remark,
                                     CreateTime = d.CreateTime
                                 });
                    if (query.Any())
                    {
                        var list = query;
                        var cnt = list.Count();
                        var tsk = new PagedList<UserDepositRechargeList_OM>(query, model.PageIndex, model.PageSize, query.Count());
                        result.ResObject = new { Total = query.Count(), List = tsk };
                    }
                }
                catch (Exception ex)
                {
                    return new ResultModel() { IsSuccess = false, MsgCode = ResPrompt.NoUserTransationCode, Message = ResPrompt.NoUserTransationMessage + ex.Message };
                }
                return result;
            }
        }

        /// <summary>
        /// 用户充值余额交易记录API列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel GetUserAccountRechargeRecordList(UserDepositRecharge_PM model)
        {
            var result = new ResultModel();
            var region = Config.UserRegion;
            if (region == null)
                return result;
            string rechargeType = string.Empty;
            using (var db = new MintBicycleDataContext())
            {
                try
                {
                    if (model.RechargeType == 1)
                    {
                        rechargeType = "支付宝";
                    }
                    else if (model.RechargeType == 2)
                    {
                        rechargeType = "微信";
                    }
                    var IsRecharge = model.IsRecharge;
                    var query = (from d in db.UserRechargeRecord
                                 join u in db.UserInfo on d.UserInfoGuid equals u.UserInfoGuid into tep
                                 from uu in tep.DefaultIfEmpty()
                                 where ((string.IsNullOrEmpty(model.Phone)) || (uu != null && uu.Phone != null && uu.Phone != "" && uu.Phone.Contains(model.Phone)))
                                 && (string.IsNullOrEmpty(rechargeType) || d.RechargeType.Contains(rechargeType))
                                 && (d.PayDate.Value.Date >= model.StartTime.Date && d.PayDate.Value.Date <= model.EndTime.Date)
                                 && ((IsRecharge == -1) || d.Status == IsRecharge)
                                 && !d.IsDeleted
                                 && (region.ExceptUserRegion || (uu.ProvinceID == region.UserProvince && (region.UserCity == null || uu.CityID == region.UserCity)
                                   && (region.UserDistrict == null || uu.DistrictID == region.UserDistrict)))
                                 orderby d.CreateTime descending
                                 select new UserDepositRechargeList_OM
                                 {
                                     UserRechargeGuid = d.UserRechargeGuid,
                                     Phone = uu.Phone,
                                     RechargeType = d.RechargeType,
                                     RechargeStatus = RecharStatusStr(d.Status),
                                     PayDate = d.PayDate,
                                     Trade_no = d.Trade_no,
                                     OutTradeNo = d.OutTradeNo,
                                     Remark = d.Remark,
                                     CreateTime = d.CreateTime,
                                     Amount = d.Amount ?? 0
                                 });
                    if (query.Any())
                    {
                        var list = query;
                        var cnt = list.Count();
                        var tsk = new PagedList<UserDepositRechargeList_OM>(query, model.PageIndex, model.PageSize, query.Count());
                        result.ResObject = new { Total = query.Count(), List = tsk };
                    }
                }
                catch (Exception ex)
                {
                    return new ResultModel() { IsSuccess = false, MsgCode = ResPrompt.NoUserTransationCode, Message = ResPrompt.NoUserTransationMessage + ex.Message };
                }
                return result;
            }
        }

        /// <summary>
        /// 用户退款交易记录API列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel GetUserRefundDepositRecordList(UserDepositRecharge_PM model)
        {
            var result = new ResultModel();
            var region = Config.UserRegion;
            if (region == null)
                return result;
            string rechargeType = string.Empty;
            using (var db = new MintBicycleDataContext())
            {
                try
                {
                    if (model.RechargeType == 1)
                    {
                        rechargeType = "支付宝";
                    }
                    else if (model.RechargeType == 2)
                    {
                        rechargeType = "微信";
                    }
                    var IsRecharge = model.IsRecharge;
                    var query = (from d in db.UserDepositRechargeRecord
                                 join u in db.UserInfo on d.UserInfoGuid equals u.UserInfoGuid into tep
                                 from uu in tep.DefaultIfEmpty()
                                 where ((string.IsNullOrEmpty(model.Phone)) || (uu != null && uu.Phone != null && uu.Phone != "" && uu.Phone.Contains(model.Phone)))
                                 && (string.IsNullOrEmpty(rechargeType) || d.RechargeType.Contains(rechargeType))
                                 && (d.PayDate.Value.Date >= model.StartTime.Date && d.PayDate.Value.Date <= model.EndTime.Date)
                                 && ((IsRecharge == -1) || d.Status == IsRecharge)
                                 && d.MoneyType == "2"
                                 && !d.IsDeleted
                                 && (region.ExceptUserRegion || (uu.ProvinceID == region.UserProvince && (region.UserCity == null || uu.CityID == region.UserCity)
                                   && (region.UserDistrict == null || uu.DistrictID == region.UserDistrict)))
                                 orderby d.CreateTime descending
                                 select new UserDepositRechargeList_OM
                                 {
                                     UserRechargeGuid = d.UserDepositRechargeGuid,
                                     Phone = uu.Phone,
                                     RechargeType = d.RechargeType,
                                     RechargeStatus = ReturnStatusStr(d.Status),
                                     PayDate = d.PayDate,
                                     Trade_no = d.Trade_no,
                                     OutTradeNo = d.OutTradeNo,
                                     Remark = d.Remark,
                                     CreateTime = d.CreateTime,
                                 });
                    if (query.Any())
                    {
                        var list = query;
                        var cnt = list.Count();
                        var tsk = new PagedList<UserDepositRechargeList_OM>(query, model.PageIndex, model.PageSize, query.Count());
                        result.ResObject = new { Total = query.Count(), List = tsk };
                    }
                }
                catch (Exception ex)
                {
                    return new ResultModel() { IsSuccess = false, MsgCode = ResPrompt.NoUserTransationCode, Message = ResPrompt.NoUserTransationMessage + ex.Message };
                }
                return result;
            }
        }

        /// <summary>
        /// 充值状态
        /// </summary>
        /// <returns></returns>
        private string RecharStatusStr(int? status)
        {
            string str = string.Empty;

            switch (status)
            {
                case 1:
                    str = "充值成功";
                    break;

                case 2:
                    str = "正在充值";
                    break;
            }
            return str;
        }


        /// <summary>
        /// 退款状态
        /// </summary>
        /// <returns></returns>
        private string ReturnStatusStr(int? status)
        {
            string str = string.Empty;

            switch (status)
            {
                case 1:
                    str = "退款成功";
                    break;

                case 2:
                    str = "正在退款";
                    break;
            }
            return str;
        }


    }
}