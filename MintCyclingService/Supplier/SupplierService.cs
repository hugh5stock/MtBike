using HuRongClub.DBUtility;
using MintCyclingData;
using MintCyclingService.Common;
using MintCyclingService.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MintCyclingService.Supplier
{
    public class SupplierService : ISupplierService
    {
        /// <summary>
        /// 删除供应商信息
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public ResultModel DeleteSupplierBySupplierID(SupplierDelete_PM Model)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var query = db.SupplierInfo.FirstOrDefault(s => s.SupplierID == Model.SupplierID && !s.IsDeleted);
                if (query == null)
                    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.NoUserTransationCode, Message = ResPrompt.NoUserTransationMessage };

                query.IsDeleted = true;
                query.UpdateBy = Model.OperatorGuid;
                query.UpdateTime = DateTime.Now;
                db.SubmitChanges();
                result.ResObject = true;
            }
            return result;
        }

        /// <summary>
        /// 新增或修改供应商信息
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public ResultModel AddOrUpdateSupplier(AddSupplierOrUpdate_PM Model)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                #region 添加或者修改

                if (Model.SupplierID != 0)
                {
                    var Supplier = db.SupplierInfo.FirstOrDefault(p => p.SupplierID == Model.SupplierID && !p.IsDeleted);
                    if (Supplier == null)
                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.NoUserTransationCode, Message = ResPrompt.NoUserTransationMessage };

                    Supplier.SupplierName = Model.SupplierName;
                    Supplier.SupplierNumber = Model.SupplierNumber;
                    Supplier.Remark = Model.Remark;
                    Supplier.UpdateBy = Model.OperatorGuid;
                    Supplier.UpdateTime = DateTime.Now;
                    db.SubmitChanges();
                    result.ResObject = true;
                }
                else
                {
                    //判断是否存在相同供应商编号的信息
                    if (db.SupplierInfo.Any(x => x.SupplierID == Model.SupplierID && x.SupplierNumber==Model.SupplierNumber && x.SupplierName == Model.SupplierName))
                    {
                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.SupplierError, Message = ResPrompt.SupplierErrorMessage };
                    }
                    var Supplier = new SupplierInfo
                    {
                        SupplierName = Model.SupplierName,
                        SupplierNumber = Model.SupplierNumber,
                        Remark = Model.Remark,
                        CreateBy = Model.OperatorGuid,
                        CreateTime = DateTime.Now,
                        IsDeleted = false
                    };
                    db.SupplierInfo.InsertOnSubmit(Supplier);
                    db.SubmitChanges();
                    result.ResObject = true;
                }

                #endregion 添加或者修改
            }
            return result;
        }

        /// <summary>
        /// 根据查询条件搜索供应商列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel GetSupplierInfoList(GetSupplierList_PM model)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var query = (from s in db.SupplierInfo
                            where ((string.IsNullOrEmpty(model.SupplierName)) || s.SupplierName.Contains(model.SupplierName))
                             && ((string.IsNullOrEmpty(model.SupplierNumber)) || s.SupplierNumber.Contains(model.SupplierNumber))
                             && !s.IsDeleted
                             orderby s.CreateTime descending
                             select new SupplierList_PM
                             {
                                SupplierID = s.SupplierID,
                                SupplierName = s.SupplierName,
                                SupplierNumber = s.SupplierNumber,
                                 Remark = s.Remark,
                                 OperatorGuid = s.CreateBy,
                                 CreateTime = s.CreateTime
                             });
                if (query.Any())
                {
                    var tsk = new PagedList<SupplierList_PM>(query, model.PageIndex, model.PageSize, query.Count());
                    result.ResObject = new { Total = query.Count(), List = tsk };
                }
            }
            return result;
        }
    }
}