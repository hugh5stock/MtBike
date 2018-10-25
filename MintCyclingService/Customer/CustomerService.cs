using MintCyclingData;
using MintCyclingService.AdminLog;
using MintCyclingService.Common;
using MintCyclingService.Utils;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Linq;

namespace MintCyclingService.Customer
{
    public class CustomerService : ICustomerService
    {
        private readonly ImanagerlogService _LogService = new ManagerlogService();

        /// <summary>
        /// 删除客户信息
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public ResultModel DeleteCustomerByCustomerID(CustomerDelete_PM Model)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var query = db.CustomerInfo.FirstOrDefault(s => s.CustomerID == Model.CustomerID && !s.IsDeleted);
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
        /// 新增或修改客户信息
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public ResultModel AddOrUpdateCustomer(AddCustomerOrUpdate_PM Model)
        {
            var result = new ResultModel();
            string EditText = string.Empty;

            try
            {
                using (var db = new MintBicycleDataContext())
                {
                    #region 添加或者修改

                    if (Model.CustomerID != 0)
                    {
                        var Customer = db.CustomerInfo.FirstOrDefault(p => p.CustomerID == Model.CustomerID && !p.IsDeleted);
                        if (Customer == null)
                            return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.NoUserTransationCode, Message = ResPrompt.NoUserTransationMessage };
                        Customer.CustomerName = Model.CustomerName;
                        Customer.Phone = Model.Phone;
                        Customer.IsLock = Model.IsLock;
                        Customer.ProvinceID = Model.ProvinceId;
                        Customer.CityID = Model.CityId;
                        Customer.DistrictID = Model.DistinctId;
                        Customer.Remark = Model.Remark;
                        Customer.Address = Model.Address;
                        Customer.UpdateBy = Model.OperatorGuid;
                        Customer.UpdateTime = DateTime.Now;
                        db.SubmitChanges();
                        result.ResObject = true;
                        EditText = "修改客户信息成功";
                    }
                    else
                    {
                        //判断是否存在相同客户的信息
                        if (db.CustomerInfo.Any(x => x.CustomerID == Model.CustomerID && x.CustomerName == Model.CustomerName))
                        {
                            return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.CustomerError, Message = ResPrompt.CustomerErrorMessage };
                        }
                        var Customer = new CustomerInfo
                        {
                            CustomerName = Model.CustomerName,
                            Phone = Model.Phone,
                            IsLock = Model.IsLock,
                            ProvinceID = Model.ProvinceId,
                            CityID = Model.CityId,
                            DistrictID = Model.DistinctId,
                            Remark = Model.Remark,
                            Address = Model.Address,
                            CreateBy = Model.OperatorGuid,
                            CreateTime = DateTime.Now,
                            IsDeleted = false
                        };
                        db.CustomerInfo.InsertOnSubmit(Customer);
                        db.SubmitChanges();
                        result.ResObject = true;
                        EditText = "添加客户信息成功";
                    }

                    //操作日志记录
                    string parameters = "CustomerName:" + Model.CustomerName + ",IsLock:" + Model.IsLock + ",CreateTime:" + DateTime.Now + "";
                    dt_manager_log LogModel = new dt_manager_log();
                    LogModel.action_type = ActionEnum.Add.ToString();
                    LogModel.remark = EditText + ",参数:" + parameters;
                    LogModel.AdminGuid = Model.OperatorGuid;

                    _LogService.AddManagerLog(LogModel);

                    #endregion 添加或者修改
                }
            }
            catch (Exception ex)
            {
                //操作日志记录
                string parameters = "CustomerName:" + Model.CustomerName + ",IsLock:" + Model.IsLock + ",CreateTime:" + DateTime.Now + "";
                dt_manager_log LogModel = new dt_manager_log();
                LogModel.action_type = ActionEnum.Add.ToString();
                LogModel.remark = EditText + ",参数:" + parameters;
                LogModel.AdminGuid = Model.OperatorGuid;
                _LogService.AddManagerLog(LogModel);
                result.ResObject = false;
            }
            return result;
        }

        /// <summary>
        /// 根据查询条件搜索客户列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel GetCustomerInfoList(GetCustomerList_PM model)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var query = (from s in db.CustomerInfo
                             join p in db.Province on s.ProvinceID equals p.Id into temp1
                             from pp in temp1.DefaultIfEmpty()
                             join c in db.City on s.CityID equals c.Id into temp2
                             from cc in temp2.DefaultIfEmpty()
                             join d in db.District on s.DistrictID equals d.Id into temp
                             from tt in temp.DefaultIfEmpty()
                             where ((string.IsNullOrEmpty(model.CustomerName)) || s.CustomerName.Contains(model.CustomerName))
                              && !s.IsDeleted
                             orderby s.CreateTime descending
                             select new CustomerList_PM
                             {
                                 CustomerID = s.CustomerID,
                                 CustomerName = s.CustomerName,
                                 IsLock = s.IsLock ?? 0,
                                 Phone = s.Phone,
                                 ProvinceId = pp == null ? 0 : pp.Id,
                                 CityId = cc == null ? 0 : cc.Id,
                                 DistinctId = tt == null ? 0 : tt.Id,
                                 DistricName = string.Format("{0}{1}{2}", new object[] { pp.Name, cc.Name, tt.Name }),  //所在地-省市区
                                 Remark = s.Remark,
                                 OperatorGuid = s.CreateBy,
                                 CreateTime = s.CreateTime,
                                 Address = s.Address
                             });
                if (query.Any())
                {
                    var tsk = new PagedList<CustomerList_PM>(query, model.PageIndex, model.PageSize, query.Count());
                    result.ResObject = new { Total = query.Count(), List = tsk };
                }
            }
            return result;
        }

        /// <summary>
        /// 查询客户列表
        /// </summary>
        /// <returns></returns>
        public ResultModel GetCustomerInfo(GetCustomerList_PM data)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var query = db.CustomerInfo.Where(s => s.IsLock == 1 && !s.IsDeleted&&(s.CustomerName.Contains(data.CustomerName)||string.IsNullOrEmpty(data.CustomerName)));
                if (query.Any())
                {
                    var list = new PagedList<CustomerInfo>(query, data.PageIndex, data.PageSize);
                      var  Rlist=list.Select(k => new { k.CustomerName, k.CustomerID }).ToList();
                    result.ResObject = Rlist;
                }
            }
            return result;
        }

        /// <summary>
        /// 查询客户列表
        /// </summary>
        /// <returns></returns>
        public ResultModel GetCustomerList()
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var query = db.CustomerInfo.Where(s => s.IsLock ==1 && !s.IsDeleted);
                if (query.Any())
                {
                    var list = query.Select(k => new { k.CustomerName, k.CustomerID }).ToList();
                    result.ResObject = list;
                }
            }
            return result;
        }



    }
}