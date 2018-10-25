using HuRongClub.DBUtility;
using MintCyclingService.Common;
using MintCyclingService.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using Utility;

namespace MintCyclingService.Statistics
{
    public class StatisticsService : IStatisticsService
    {
        /// <summary>
        /// 平台运营统计数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel GetStatisticsData()
        {
            var result = new ResultModel();
            string SqlStr = string.Empty;
            
         //获取总账号省市区
         var region = Config.UserRegion;
            if (region == null)
                return result;

            GetDataModel_OM model = new GetDataModel_OM();
            var data = new GetDataList_OM { };
            List<GetDataModel_OM> list = new List<GetDataModel_OM>();
            //&& (region.ExceptUserRegion || (province.Id == region.UserProvince && (region.UserCity == null || city.Id == region.UserCity)
            //                      && (region.UserDistrict == null || t.Id == region.UserDistrict)))
            if ((region.UserProvince != 0 || region.UserProvince != 9) && (region.UserCity != 0 || region.UserCity != 73) && (region.UserDistrict != 0 && region.UserDistrict != 759))
            {
                SqlStr = " select ";
                //--平台用户注册总数
                SqlStr += " (select count(*)  from UserInfo as u where u.ProvinceID=" + region.UserProvince + " and u.CityID=" + region.UserCity + " and u.DistrictID=" + region.UserDistrict + ")  as TotalUserCount, ";
                //--本月用户注册总数
                SqlStr += " (select count(*)  from UserInfo as u where u.ProvinceID=" + region.UserProvince + " and u.CityID=" + region.UserCity + " and u.DistrictID=" + region.UserDistrict + ")   as TotalMonthUserCount, ";
                //--本年用户注册总数
                SqlStr += " (select count(*)  from UserInfo as u where DATEDIFF(year,CreateTime,GETDATE())=0 and u.ProvinceID=" + region.UserProvince + " and u.CityID=" + region.UserCity + " and u.DistrictID=" + region.UserDistrict + ")   as TotalYearUserCount,";
                //总故障数
                SqlStr += " (select count(*)  from BreakdownLog as b where status<>1 and b.ProvinceID=" + region.UserProvince + " and b.CityID=" + region.UserCity + " and b.DistrictID=" + region.UserDistrict + ") as TotalBreakCount, ";
                //本月故障总数
                SqlStr += "(select count(*)  from BreakdownLog as b where status<>1 and DATEDIFF(mm,CreateTime,GETDATE())=0 and b.ProvinceID=" + region.UserProvince + " and b.CityID=" + region.UserCity + " and b.DistrictID=" + region.UserDistrict + ") as TotalMonthBreakCount, ";
                //本年故障数
                SqlStr += "(select count(*)  from BreakdownLog as b Where status<>1 and DateDiff(year, GetDate(), CreateTime ) = 0 and b.ProvinceID=" + region.UserProvince + " and b.CityID=" + region.UserCity + " and b.DistrictID=" + region.UserDistrict + ") as TotalYearBreakCount,";

                //押金充值总金额
                SqlStr += "(select sum(amount)   from Deposit inner join UserInfo as u on u.ProvinceID=" + region.UserProvince + " and u.CityID=" + region.UserCity + " and u.DistrictID=" + region.UserDistrict + " )  as TotalDepositAmount,";

                //本月押金充值总金额
                SqlStr += "(select sum(amount)  from Deposit as d  inner join UserInfo as u on u.ProvinceID=" + region.UserProvince + " and u.CityID=" + region.UserCity + " and u.DistrictID=" + region.UserDistrict + " where DATEDIFF(mm,d.UpdateTime,GETDATE())=0 ) as TotalMonthDepositAmount,";
                //本年押金充值总金额
                SqlStr += "(select sum(amount)  from Deposit  as d  inner join UserInfo as u on u.ProvinceID=" + region.UserProvince + " and u.CityID=" + region.UserCity + " and u.DistrictID=" + region.UserDistrict + "  where DATEDIFF(year,d.UpdateTime,GETDATE())=0 ) as TotalYearDepositAmount ,";

                //余额充值总金额
                SqlStr += "(select sum(BalanceAmount)  from AccountInfo inner join UserInfo as u on u.ProvinceID=" + region.UserProvince + " and u.CityID=" + region.UserCity + " and u.DistrictID=" + region.UserDistrict + " )  as TotalAccountAmount ,";
                //本月余额充值总金额
                SqlStr += "(select sum(BalanceAmount)   from AccountInfo as a inner join UserInfo as u on u.ProvinceID=" + region.UserProvince + " and u.CityID=" + region.UserCity + " and u.DistrictID=" + region.UserDistrict + " where DATEDIFF(mm,a.UpdateTime,GETDATE())=0 ) as TotalMonthAccountAmount ,";
                //本年余额充值总金额
                SqlStr += "(select sum(BalanceAmount)   from AccountInfo  as a inner join UserInfo as u on u.ProvinceID=" + region.UserProvince + " and u.CityID=" + region.UserCity + " and u.DistrictID=" + region.UserDistrict + "  where DATEDIFF(year,a.UpdateTime,GETDATE())=0 ) as TotalYearAccountAmount ,";

                //统计车辆和电子围栏相关数据
                //总的车辆数
                SqlStr += " (select count(*)  from BicycleBaseInfo inner join BicycleLockInfo  as u on u.ProvinceID=" + region.UserProvince + " and u.CityID=" + region.UserCity + " and u.DistrictID=" + region.UserDistrict + " ) as TotalBicycleCount,";
                //车辆开锁总数
                SqlStr += "  (select count(*) from BicycleBaseInfo as b inner join  BicycleLockInfo c on b.LockGuid=c.LockGuid  where c.LockStatus=0 and c.ProvinceID=" + region.UserProvince + " and c.CityID=" + region.UserCity + " and c.DistrictID=" + region.UserDistrict + ") as TotalOpenCount,";
                //车辆总故障数
                SqlStr += "  (select count(*) from BicycleBaseInfo as b inner join  BicycleLockInfo c on b.LockGuid=c.LockGuid  where c.LockStatus=2 and c.ProvinceID=" + region.UserProvince + " and c.CityID=" + region.UserCity + " and c.DistrictID=" + region.UserDistrict + ") as TotalRepairCount,";
                //电子围栏总数
                SqlStr += " (select count(*)  from Electronic_FenCing as u where  u.ProvinceID=" + region.UserProvince + " and u.CityID=" + region.UserCity + " and u.DistrictID=" + region.UserDistrict + ")  as TotalElectronicCount,";
                //当天的开锁次数
                SqlStr += " (select count(*) from BicycleLockInfo  where LockStatus=0 and datediff(day,updatetime,getdate()) = 0 and  ProvinceID=" + region.UserProvince + " and CityID=" + region.UserCity + " and DistrictID=" + region.UserDistrict + ") as OpenCount ";
               
            }
            else
            {
                SqlStr = @"
              select
--平台用户注册总数
(select count(*)  from UserInfo)   as TotalUserCount,
--本月用户注册总数
(select count(*)  from UserInfo)   as TotalMonthUserCount,
--本年用户注册总数
(select count(*)  from UserInfo  where DATEDIFF(year,CreateTime,GETDATE())=0)   as TotalYearUserCount,

--总故障数
(select count(*)  from BreakdownLog where status<>1) as TotalBreakCount,
--本月故障总数
(select count(*)  from BreakdownLog where status<>1 and DATEDIFF(mm,CreateTime,GETDATE())=0) as TotalMonthBreakCount,
--本年故障数
(select count(*)  from BreakdownLog Where status<>1 and DateDiff(year, GetDate(), CreateTime ) = 0) as TotalYearBreakCount,

--押金充值总金额
(select sum(amount)  from Deposit)   as TotalDepositAmount,
 --本月押金充值总金额
(select sum(amount)  from Deposit  where DATEDIFF(mm,UpdateTime,GETDATE())=0)   as TotalMonthDepositAmount,
--本年押金充值总金额
(select sum(amount)  from Deposit where DATEDIFF(year,UpdateTime,GETDATE())=0)   as TotalYearDepositAmount,

--余额充值总金额
(select sum(BalanceAmount)  from  AccountInfo)   as TotalAccountAmount,
--本月余额充值总金额
(select sum(BalanceAmount)  from AccountInfo  where DATEDIFF(mm,UpdateTime,GETDATE())=0)   as TotalMonthAccountAmount,
--本年余额充值总金额
(select sum(BalanceAmount)  from AccountInfo where DATEDIFF(year,UpdateTime,GETDATE())=0)   as TotalYearAccountAmount,

-----统计车辆和电子围栏相关数据
 --总的车辆数
 (select count(*) from BicycleBaseInfo) as TotalBicycleCount,
  --车辆开锁总数
 (select count(*) from BicycleBaseInfo as b inner join  BicycleLockInfo c on b.LockGuid=c.LockGuid  where c.LockStatus=0) as TotalOpenCount,
 --车辆总故障数
 (select count(*) from BicycleBaseInfo as b inner join  BicycleLockInfo c on b.LockGuid=c.LockGuid  where c.LockStatus=2) as TotalRepairCount,
  --电子围栏总数
 (select count(*) from Electronic_FenCing) as TotalElectronicCount,
    --当天的开锁次数
    (select count(*) from BicycleLockInfo  where LockStatus = 0 and datediff(day, updatetime, getdate()) = 0 ) as OpenCount";   

            }
 
            DataSet ds = DbHelperSQL.Query(SqlStr);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                //用户统计
                model.TotalUserCount = int.Parse(ds.Tables[0].Rows[0]["TotalUserCount"].ToString());
                model.TotalMonthUserCount = int.Parse(ds.Tables[0].Rows[0]["TotalMonthUserCount"].ToString());
                model.TotalYearUserCount = int.Parse(ds.Tables[0].Rows[0]["TotalYearUserCount"].ToString());
                //故障统计
                model.TotalBreakCount = int.Parse(ds.Tables[0].Rows[0]["TotalBreakCount"].ToString());
                model.TotalMonthBreakCount = int.Parse(ds.Tables[0].Rows[0]["TotalMonthBreakCount"].ToString());
                model.TotalYearBreakCount = int.Parse(ds.Tables[0].Rows[0]["TotalYearBreakCount"].ToString());
                //押金统计
                model.TotalDepositAmount = ds.Tables[0].Rows[0]["TotalDepositAmount"] == DBNull.Value ? 0m : decimal.Parse(ds.Tables[0].Rows[0]["TotalDepositAmount"].ToString());
                model.TotalMonthDepositAmount = ds.Tables[0].Rows[0]["TotalMonthDepositAmount"] == DBNull.Value ? 0m : decimal.Parse(ds.Tables[0].Rows[0]["TotalMonthDepositAmount"].ToString());
                model.TotalYearDepositAmount = ds.Tables[0].Rows[0]["TotalYearDepositAmount"] == DBNull.Value ? 0m : decimal.Parse(ds.Tables[0].Rows[0]["TotalYearDepositAmount"].ToString());

                //余额统计
                model.TotalAccountAmount = ds.Tables[0].Rows[0]["TotalAccountAmount"] == DBNull.Value ? 0m : decimal.Parse(ds.Tables[0].Rows[0]["TotalAccountAmount"].ToString());
                model.TotalMonthAccountAmount = ds.Tables[0].Rows[0]["TotalMonthAccountAmount"] == DBNull.Value ? 0m : decimal.Parse(ds.Tables[0].Rows[0]["TotalMonthAccountAmount"].ToString());
                model.TotalYearAccountAmount = ds.Tables[0].Rows[0]["TotalYearAccountAmount"] == DBNull.Value ? 0m : decimal.Parse(ds.Tables[0].Rows[0]["TotalYearAccountAmount"].ToString());

                //统计车辆和电子围栏相关数据
                //车辆开锁总数
                model.TotalBicycleCount = ds.Tables[0].Rows[0]["TotalBicycleCount"] == DBNull.Value ? 0 : int.Parse(ds.Tables[0].Rows[0]["TotalBicycleCount"].ToString());
                model.TotalOpenCount = ds.Tables[0].Rows[0]["TotalOpenCount"] == DBNull.Value ? 0 : int.Parse(ds.Tables[0].Rows[0]["TotalOpenCount"].ToString());
                model.TotalRepairCount = ds.Tables[0].Rows[0]["TotalRepairCount"] == DBNull.Value ? 0 : int.Parse(ds.Tables[0].Rows[0]["TotalRepairCount"].ToString());
                model.TotalElectronicCount = ds.Tables[0].Rows[0]["TotalElectronicCount"] == DBNull.Value ? 0 : int.Parse(ds.Tables[0].Rows[0]["TotalElectronicCount"].ToString());
                //单车开锁率=当天的开锁次数/车辆数总数
                if (model.TotalBicycleCount == 0)
                {
                    model.OpenRate = 0;
                }
                else
                {
                    model.OpenRate = Math.Round(ds.Tables[0].Rows[0]["OpenCount"] == DBNull.Value ? 0 : decimal.Parse(ds.Tables[0].Rows[0]["OpenCount"].ToString()) / model.TotalBicycleCount, 2);
                }
           

                list.Add(model);
                data.dataList = list;

                result.Message = "查询统计平台相关数据成功";

                result.ResObject = data.dataList == null && data.dataList == null ? null : data;
            }
            else
            {
                return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.NoUserTransationCode, Message = ResPrompt.NoUserTransationMessage };
            }

            return result;
        }

        /// <summary>
        /// 根据条件搜索平台收益情况统计数据
        /// </summary>
        /// <returns></returns>
        public ResultModel GetStatisticsDataByCondition(GetCondition_PM model)
        {
            var result = new ResultModel();
            GetCondition_OM models = new GetCondition_OM();
            var data = new GetDataCondition_OM { };
            List<GetCondition_OM> list = new List<GetCondition_OM>();

            //获取总账号省市区
            var region = Config.UserRegion;
            if (region == null)
                return result;

            string SqlStr = string.Empty;
            // select
            // --总收入=查询累计充值押金总金额+查询累计充值余额总金额;
            // --查询累计充值押金总金额
            //  (select SUM(Amount) from  Deposit) as TotalAmount,
            // --查询累计充值余额总金额
            //   (select SUM(BalanceAmount) from  AccountInfo) as TotalAAmount,";
            SqlStr = "  select ";
            //总收入=查询累计充值押金总金额+查询累计充值余额总金额;
            SqlStr += "  (select sum(amount)   from Deposit inner join UserInfo as u on u.ProvinceID=" + region.UserProvince + " and u.CityID=" + region.UserCity + " and u.DistrictID=" + region.UserDistrict + " )  as TotalAmount, ";
            //查询累计充值余额总金额
            SqlStr += "  (select sum(BalanceAmount)   from AccountInfo  inner join UserInfo as u on u.ProvinceID=" + region.UserProvince + " and u.CityID=" + region.UserCity + " and u.DistrictID=" + region.UserDistrict + " )  as TotalAAmount, ";
 
            #region 押金统计

            if (model.TypeID == 1)
            {
                //当天1
                SqlStr += "(select SUM(Amount) from Deposit as d  inner join UserInfo as u on u.ProvinceID=" + region.UserProvince + " and u.CityID=" + region.UserCity + " and u.DistrictID=" + region.UserDistrict + " where datediff(day, d.createtime, getdate()) = 0) as TotalDAmount,";
            }
            else if (model.TypeID == 2)
            {
                //昨天2
                SqlStr += "(select SUM(Amount) from Deposit  as d  inner join UserInfo as u on u.ProvinceID=" + region.UserProvince + " and u.CityID=" + region.UserCity + " and u.DistrictID=" + region.UserDistrict + " where datediff(day, d.createtime, getdate()) = 1) as TotalDAmount,";
            }
            else if (model.TypeID == 3)
            {
                //本月3
                SqlStr += " (select SUM(Amount) from  Deposit  as d  inner join UserInfo as u on u.ProvinceID=" + region.UserProvince + " and u.CityID=" + region.UserCity + " and u.DistrictID=" + region.UserDistrict + " where  datediff(month, d.createtime ,getdate()) = 0) as TotalDAmount,";
            }
            else if (model.TypeID == 4)
            {
                //上月4
                SqlStr += "  (select SUM(Amount) from  Deposit  as d  inner join UserInfo as u on u.ProvinceID=" + region.UserProvince + " and u.CityID=" + region.UserCity + " and u.DistrictID=" + region.UserDistrict + " where  datediff(month, d.createtime ,getdate()) = 1) as TotalDAmount,";
            }
            else if (model.TypeID == 5)
            {
                //本年5
                SqlStr += " (select SUM(Amount) from  Deposit  as d  inner join UserInfo as u on u.ProvinceID=" + region.UserProvince + " and u.CityID=" + region.UserCity + " and u.DistrictID=" + region.UserDistrict + " where  datediff(year, d.createtime ,getdate()) = 0) as TotalDAmount,";
            }
            else if (model.TypeID == 6)
            {
                //去年6
                SqlStr += " (select SUM(Amount) from  Deposit  as d  inner join UserInfo as u on u.ProvinceID=" + region.UserProvince + " and u.CityID=" + region.UserCity + " and u.DistrictID=" + region.UserDistrict + " where  datediff(year, d.createtime ,getdate()) = 1) as TotalDAmount,";
            }

            #endregion 押金统计

            #region 余额统计

            if (model.TypeID == 1)
            {
                //当天1
                SqlStr += "  (select SUM(BalanceAmount) from  AccountInfo  as a  inner join UserInfo as u on u.ProvinceID=" + region.UserProvince + " and u.CityID=" + region.UserCity + " and u.DistrictID=" + region.UserDistrict + " where  datediff(day, a.createtime ,getdate()) = 0) as TotalAccountAmount,";
            }
            else if (model.TypeID == 2)
            {
                //昨天2
                SqlStr += "(select SUM(BalanceAmount) from   AccountInfo  as a  inner join UserInfo as u on u.ProvinceID=" + region.UserProvince + " and u.CityID=" + region.UserCity + " and u.DistrictID=" + region.UserDistrict + " where datediff(day, a.createtime, getdate()) = 1) as TotalAccountAmount,";
            }
            else if (model.TypeID == 3)
            {
                //本月3
                SqlStr += " (select SUM(BalanceAmount) from  AccountInfo  as a  inner join UserInfo as u on u.ProvinceID=" + region.UserProvince + " and u.CityID=" + region.UserCity + " and u.DistrictID=" + region.UserDistrict + " where  datediff(month, a.createtime ,getdate()) = 0) as TotalAccountAmount,";
            }
            else if (model.TypeID == 4)
            {
                //上月4
                SqlStr += " (select SUM(BalanceAmount) from  AccountInfo  as a  inner join UserInfo as u on u.ProvinceID=" + region.UserProvince + " and u.CityID=" + region.UserCity + " and u.DistrictID=" + region.UserDistrict + " where  datediff(month, a.createtime ,getdate()) = 1) as TotalAccountAmount,";
            }
            else if (model.TypeID == 5)
            {
                //本年5
                SqlStr += " (select SUM(BalanceAmount) from  AccountInfo  as a  inner join UserInfo as u on u.ProvinceID=" + region.UserProvince + " and u.CityID=" + region.UserCity + " and u.DistrictID=" + region.UserDistrict + " where  datediff(year, a.createtime ,getdate()) = 0) as TotalAccountAmount,";
            }
            else if (model.TypeID == 6)
            {
                //去年6
                SqlStr += " (select SUM(BalanceAmount) from  AccountInfo  as a  inner join UserInfo as u on u.ProvinceID=" + region.UserProvince + " and u.CityID=" + region.UserCity + " and u.DistrictID=" + region.UserDistrict + " where  datediff(year, a.createtime ,getdate()) = 1) as TotalAccountAmount, ";
            }

            #endregion 余额统计

            #region 退押金统计

            if (model.TypeID == 1)
            {
                //当天1
                SqlStr += "  (select SUM(amount) from  UserDepositRechargeRecord  as a  inner join UserInfo as u on u.ProvinceID=" + region.UserProvince + " and u.CityID=" + region.UserCity + " and u.DistrictID=" + region.UserDistrict + " where  datediff(day, a.createtime ,getdate()) = 0 and a.MoneyType=2) as TotalReturnAmount";
            }
            else if (model.TypeID == 2)
            {
                //昨天2
                SqlStr += " (select SUM(amount) from  UserDepositRechargeRecord  as a  inner join UserInfo as u on u.ProvinceID=" + region.UserProvince + " and u.CityID=" + region.UserCity + " and u.DistrictID=" + region.UserDistrict + " where  datediff(day, a.createtime ,getdate()) = 1 and a.MoneyType=2) as TotalReturnAmount";
            }
            else if (model.TypeID == 3)
            {
                //本月3
                SqlStr += "  (select SUM(amount) from  UserDepositRechargeRecord  as a  inner join UserInfo as u on u.ProvinceID=" + region.UserProvince + " and u.CityID=" + region.UserCity + " and u.DistrictID=" + region.UserDistrict + " where  datediff(month, a.createtime ,getdate()) = 0 and a.MoneyType=2) as TotalReturnAmount";
            }
            else if (model.TypeID == 4)
            {
                //上月4
                SqlStr += "  (select SUM(amount) from  UserDepositRechargeRecord  as a  inner join UserInfo as u on u.ProvinceID=" + region.UserProvince + " and u.CityID=" + region.UserCity + " and u.DistrictID=" + region.UserDistrict + " where  datediff(month, a.createtime ,getdate()) = 1 and a.MoneyType=2) as TotalReturnAmount";
            }
            else if (model.TypeID == 5)
            {
                //本年5
                SqlStr += "  (select SUM(amount) from  UserDepositRechargeRecord  as a  inner join UserInfo as u on u.ProvinceID=" + region.UserProvince + " and u.CityID=" + region.UserCity + " and u.DistrictID=" + region.UserDistrict + " where  datediff(year, a.createtime ,getdate()) = 0 and a.MoneyType=2) as TotalReturnAmount";
            }
            else if (model.TypeID == 6)
            {
                //去年6
                SqlStr += "  (select SUM(amount) from  UserDepositRechargeRecord  as a  inner join UserInfo as u on u.ProvinceID=" + region.UserProvince + " and u.CityID=" + region.UserCity + " and u.DistrictID=" + region.UserDistrict + " where  datediff(year, a.createtime ,getdate()) = 1 and a.MoneyType=2) as TotalReturnAmount ";
            }

            #endregion 退押金统计

            DataSet ds = DbHelperSQL.Query(SqlStr);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                //查询累计充值押金总金额
                //查询累计充值余额总金额
                if (ds.Tables[0].Rows[0]["TotalAmount"].ToString() == "")
                {
                    models.TotalDAmount = 0;
                }
                else
                {
                    models.TotalDAmount = decimal.Parse(ds.Tables[0].Rows[0]["TotalAmount"].ToString());
                }

                //查询累计充值余额总金额
                if (ds.Tables[0].Rows[0]["TotalAAmount"].ToString() == "")
                {
                    models.TotalAAmount = 0;
                }
                else
                {
                    models.TotalAAmount = decimal.Parse(ds.Tables[0].Rows[0]["TotalAAmount"].ToString());
                }

                //按条件查询押金充值总金额
                if (ds.Tables[0].Rows[0]["TotalDAmount"].ToString() == "")
                {
                    models.TotalDepositAmount = 0;
                }
                else
                {
                    models.TotalDepositAmount = decimal.Parse(ds.Tables[0].Rows[0]["TotalDAmount"].ToString());
                }

                //按条件查询余额充值总金额
                if (ds.Tables[0].Rows[0]["TotalAccountAmount"].ToString() == "")
                {
                    models.TotalAccountAmount = 0;
                }
                else
                {
                    models.TotalAccountAmount = decimal.Parse(ds.Tables[0].Rows[0]["TotalAccountAmount"].ToString());
                }

                //用户退押金总金额
                if (ds.Tables[0].Rows[0]["TotalReturnAmount"].ToString() == "")
                {
                    models.TotalReturnAmount = 0;
                }
                else
                {
                    models.TotalReturnAmount = decimal.Parse(ds.Tables[0].Rows[0]["TotalReturnAmount"].ToString());
                }

                models.TotalAmounts = models.TotalDAmount + models.TotalAAmount;
                list.Add(models);
                data.ConditionList = list;

                result.Message = "按条件查询统计平台相关数据成功";
                result.ResObject = data.ConditionList == null && data.ConditionList == null ? null : data;
            }
            else
            {
                return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.NoUserTransationCode, Message = ResPrompt.NoUserTransationMessage };
            }
            return result;
        }
    }
}