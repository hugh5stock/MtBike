using MintCyclingData;
using MintCyclingService.Admin;
using MintCyclingService.AdminLog;
using MintCyclingService.Common;
using MintCyclingService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Hash;

namespace MintCyclingService.Role
{
    public class AdminRoleService : IAdminRoleService
    {
        private readonly ImanagerlogService _LogService = new ManagerlogService();

        /// <summary>
        /// 新增或更新角色
        /// </summary>
        /// <param name="data"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public ResultModel AddOrUpdateAdminRole(AddAdminRole_PM data)
        {
            var result = new ResultModel();

            if (string.IsNullOrEmpty(data.RoleName))
                return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.AdminRoleCanNotNull, Message = ResPrompt.AdminRoleCanNotNullMessage };
         

            using (var db = new MintBicycleDataContext())
            {
                var role = db.AdminRole.FirstOrDefault(k => k.RoleName == data.RoleName && !k.IsDeleted);
                if (data.AdminRoleGuid == null)
                {
                    if (db.AdminRole.Any(s => s.RoleName == data.RoleName && !s.IsDeleted))
                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.AdminRoleExisted, Message = ResPrompt.AdminRoleExistedMessage };
                    var adminRole = new MintCyclingData.AdminRole
                    {
                        AdminRoleGuid = Guid.NewGuid(),
                        RoleName = data.RoleName,
                        Status = 1,
                        CreateBy = data.OperatorGuid,
                        UpdateTime = DateTime.Now,
                        CreateTime = DateTime.Now,
                        IsDeleted = false
                    };
                    if (role != null)
                    {
                        adminRole.AdminRoleGuid = role.AdminRoleGuid;
                    }
                    db.AdminRole.InsertOnSubmit(adminRole);
                    db.SubmitChanges();
                    result.ResObject = true;

                    //操作日志记录
                    string parameters = "AdminRoleGuid:" + adminRole.AdminRoleGuid + ",RoleName:" + data.RoleName;
                    dt_manager_log LogModel = new dt_manager_log();
                    LogModel.action_type = ActionEnum.Add.ToString();
                    LogModel.remark = "新增角色成功,参数:" + parameters;
                    LogModel.AdminGuid = data.OperatorGuid;

                    _LogService.AddManagerLog(LogModel);
                }
                else
                {
                    var query = db.AdminRole.FirstOrDefault(p => p.AdminRoleGuid == data.AdminRoleGuid);
                    if (query != null)
                    {
                        if (db.AdminRole.Any(s => s.RoleName == data.RoleName && !s.IsDeleted && s.AdminRoleGuid ==query.AdminRoleGuid))
                            return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.AdminRoleExisted, Message = ResPrompt.AdminRoleExistedMessage };
                        query.RoleName = data.RoleName;
                        query.UpdateBy = data.OperatorGuid;
                        query.UpdateTime = DateTime.Now;
                        if (role != null)
                        {
                            query.AdminRoleGuid = role.AdminRoleGuid;
                        }
                        db.SubmitChanges();
                        result.ResObject = true;

                        //操作日志记录
                        string parameters = "AdminRoleGuid:" + query.AdminRoleGuid + ",RoleName:" + data.RoleName;
                        dt_manager_log LogModel = new dt_manager_log();
                        LogModel.action_type = ActionEnum.Add.ToString();
                        LogModel.remark = "修改角色成功,参数:" + parameters;
                        LogModel.AdminGuid = data.OperatorGuid;

                        _LogService.AddManagerLog(LogModel);
                    }
                }
            }
            return result;
        }



        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="data"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public ResultModel DelAdmin(DeleteAdminRole_PM data)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var query = db.AdminRole.FirstOrDefault(s => s.AdminRoleGuid == data.AdminRoleGuid);
                if (query != null)
                {
                    query.CreateBy = data.AdminRoleGuid;
                    query.IsDeleted = true;
                    query.UpdateTime = DateTime.Now;
                    db.SubmitChanges();
                    result.ResObject = true;


                    //操作日志记录
                    string parameters = "AdminRoleGuid:" + data.AdminRoleGuid+",Adimin:"+data.OperatorGuid;
                    dt_manager_log LogModel = new dt_manager_log();
                    LogModel.action_type = ActionEnum.Add.ToString();
                    LogModel.remark = "删除角色成功,参数:" + parameters;
                    LogModel.AdminGuid = data.OperatorGuid;

                    _LogService.AddManagerLog(LogModel);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取角色分页列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ResultModel GetAdminRolePageList(GetAdminRolePageList_PM data)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var query = (from s in db.AdminRole
                             where ((string.IsNullOrEmpty(data.RoleName)) || s.RoleName.Contains(data.RoleName)) && !s.IsDeleted
                             orderby s.CreateTime descending
                             select new GetAdminRolePageList_OM
                             {
                                 AdminRoleGuid = s.AdminRoleGuid,
                                 RoleName = s.RoleName,             //角色名称
                                 CreateTime = s.CreateTime,         //创建时间
                             });
                if (query.Any())
                {
                    var list = query;
                    var cnt = list.Count();
                    var tsk = new PagedList<GetAdminRolePageList_OM>(list, data.PageIndex, data.PageSize, cnt);
                    result.ResObject = new { Total = cnt, List = tsk };
                }
            }
            return result;
        }
 
    }
}