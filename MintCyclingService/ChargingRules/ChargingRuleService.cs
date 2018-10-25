using MintCyclingData;
using MintCyclingService.Common;
using MintCyclingService.Utils;
using System;
using System.Linq;

namespace MintCyclingService.ChargingRules
{
    public class ChargingRuleService : IChargingRuleService
    {
        /// <summary>
        /// 计费规则列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel GetChargingRuleList()
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var query = (from c in db.ChargingRule
                             where !c.IsDeleted
                             orderby c.CreateTime descending
                             select new GetChargingRuleList_OM
                             {
                                 RuleID = c.RuleID,
                                 FreeTime = c.FreeTime,
                                 ChargingTime = c.ChargingTime,
                                 Price = c.Price,
                                 PriceMax = c.PriceMax,
                                 Remark = c.Remark,
                                 CreateTime = DateTime.Now
                             });
                if (query.Any())
                {
                    result.ResObject = new { List = query.ToList() };
                }
            }
            return result;
        }

        /// <summary>
        /// 添加或编辑计费规则
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public ResultModel AddChargingRule(ChargingRule_PM model)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                if (model.RuleID != 0)  //修改
                {
                    var query = db.ChargingRule.FirstOrDefault(s => s.RuleID == model.RuleID && !s.IsDeleted);
                    if (query == null)
                    {
                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.NoUserTransationCode, Message = ResPrompt.NoUserTransationMessage };
                    }
                    query.FreeTime = model.FreeTime;
                    query.ChargingTime = model.ChargingTime;
                    query.Price = model.Price;
                    query.PriceMax = model.PriceMax;
                    query.Remark = model.Remark;
                    query.UpdateTime = DateTime.Now;
                    db.SubmitChanges();
                }
                else
                {
                    var addRule = new ChargingRule
                    {
                        FreeTime = model.FreeTime,
                        ChargingTime = model.ChargingTime,
                        Price = model.Price,
                        PriceMax = model.PriceMax,
                        Remark = model.Remark,
                        CreateTime = DateTime.Now,
                        IsDeleted = false
                    };
                    db.ChargingRule.InsertOnSubmit(addRule);
                    db.SubmitChanges();
                }
                result.ResObject = true;
            }
            return result;
        }


        /// <summary>
        /// 删除计费规则信息
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public ResultModel DeleteChargingRuleByServiceGuid(DeleteChargingRule_PM Model)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var query = db.ChargingRule.FirstOrDefault(s => s.RuleID == Model.RuleID && !s.IsDeleted);
                if (query == null)
                    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.NoUserTransationCode, Message = ResPrompt.NoUserTransationMessage };

                query.IsDeleted = true;
                query.UpdateTime = DateTime.Now;
                db.SubmitChanges();
                result.Message = "删除维护人员成功";
            }
            return result;
        }




    }
}