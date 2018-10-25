using DTcms.Common;
using MintCyclingData;
using MintCyclingService.AdminLog;
using MintCyclingService.Common;
using MintCyclingService.Role;
using MintCyclingService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using Utility;
using Utility.Hash;

namespace MintCyclingService.Admin
{
    public class AdminService : IAdminService
    {
        private readonly ImanagerlogService _LogService = new ManagerlogService();

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">口令</param>
        /// <returns>用户登录结果</returns>
        public ResultModel Login(string userName, string password)
        {
            var result = new ResultModel();

            using (var db = new MintBicycleDataContext())
            {
                //var bicycles = db.BicycleLockInfo.Where(q => (q.GEOHash == null || q.GEOHash == string.Empty) && q.Longitude.HasValue && q.Latitude.HasValue);
                //if (bicycles.Any())
                //{
                //    foreach (var item in bicycles)
                //    {
                //        item.GEOHash = Utility.Common.GEOHashHelper.Encode((double)item.Latitude.Value, (double)item.Longitude.Value);
                //    }

                //    db.SubmitChanges();
                //}

                //var eles = db.Electronic_FenCing.Where(q => (q.GEOHash == null || q.GEOHash == string.Empty) && q.Longitude > 0 && q.Latitude > 0);
                //if (eles.Any())
                //{
                //    foreach (var item in eles)
                //    {
                //        item.GEOHash = Utility.Common.GEOHashHelper.Encode((double)item.Latitude, (double)item.Longitude);
                //    }

                //    db.SubmitChanges();
                //}

                var user = db.Admin.SingleOrDefault(q => q.UserName == userName && q.Status == (byte)AdminStatusEnum.Enable);

                if (user != null)
                {
                    var admin = user;
                    var hashCode = password;
                    if (!PasswordHash.ValidatePassword(password, admin.Password))
                    {
                        result.IsSuccess = true;
                        result.MsgCode = ResPrompt.PasswordHashCodeError;
                        result.Message = ResPrompt.PasswordHashCodeErrorMessage;
                        result.ResObject = false;
                        return result;
                    }
                    else
                    {
                        result.IsSuccess = true;
                        result.MsgCode = "0";
                        result.Message = "";
                    }


                    admin.LastLoginTime = DateTime.Now;
                    admin.LoginCount += 1;
                    db.SubmitChanges();

                    var role = db.AdminRole.FirstOrDefault(k => k.AdminRoleGuid == admin.RoleGuid);
                    var data = new AdminLoginData_OM { AdminGuid = admin.AdminGuid, RoleName = role == null ? string.Empty : role.RoleName, RoleGuid = role == null ? (Guid?)null : role.AdminRoleGuid };
                    result.ResObject = data;

                    //操作日志记录
                    string parameters = "userName:" + userName + ",password:" + password + ",RoleGuid:" + admin.RoleGuid;
                    dt_manager_log LogModel = new dt_manager_log();
                    LogModel.action_type = ActionEnum.Add.ToString();
                    LogModel.remark = "更新角色关联的权限成功,参数:" + parameters;
                    LogModel.AdminGuid = admin.AdminGuid;

                    _LogService.AddManagerLog(LogModel);

                }
                else
                {
                    result.IsSuccess = true;
                    result.MsgCode = ResPrompt.UserNameNotExisted;
                    result.Message = ResPrompt.UserNameNotExistedMessage;
                    return result;
                }
            }

            return result;
        }
        /// <summary>
        /// 新增或更新管理员
        /// </summary>
        /// <param name="data"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public ResultModel AddOrUpdateAdmin(AddAdmin_PM data)
        {
            if (string.IsNullOrEmpty(data.UserName))
                return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.AdminUserNameCanNotNull, Message = ResPrompt.AdminUserNameCanNotNullMessage };
            if (string.IsNullOrEmpty(data.Password))
                return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.AdminPasswordCanNotNull, Message = ResPrompt.AdminPasswordCanNotNullMessage };
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var role = db.AdminRole.FirstOrDefault(k => k.RoleName == data.RoleName && !k.IsDeleted);
                if (data.AdminGuid == null)
                {
                    if (db.Admin.Any(s => s.UserName == data.UserName && !s.IsDeleted))
                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.AdminUserNameExsited, Message = ResPrompt.AdminUserNameExsitedMessage };
                    var admin = new MintCyclingData.Admin
                    {
                        AdminGuid = Guid.NewGuid(),
                        UserName = data.UserName,
                        Password = PasswordHash.CreateHash(data.Password),
                        LoginCount = 0,
                        Status = (byte)AdminStatusEnum.Enable,
                        CreateBy = data.OperatorGuid,
                        UpdateTime = DateTime.Now,
                        CreateTime = DateTime.Now,
                        IsDeleted = false,
                        ProvinceID = data.Province,
                        CityID = data.City,
                        DistrictID = data.Area
                    };
                    if (role != null)
                    {
                        admin.RoleGuid = role.AdminRoleGuid;
                    }
                    db.Admin.InsertOnSubmit(admin);
                    db.SubmitChanges();
                    result.ResObject = true;

                    //操作日志记录
                    string parameters = "AdminGuid:" + admin.AdminGuid + ",UserName:" + admin.UserName + ",Password:"+ data.Password;
                    dt_manager_log LogModel = new dt_manager_log();
                    LogModel.action_type = ActionEnum.Add.ToString();
                    LogModel.remark = "新增管理员成功,参数:" + parameters;
                    LogModel.AdminGuid = data.OperatorGuid;
                  
                    _LogService.AddManagerLog(LogModel);
                }
                else
                {
                    var query = db.Admin.FirstOrDefault(p => p.AdminGuid == data.AdminGuid);
                    if (query != null)
                    {
                        var pwd = "00000000-0000-0000-0000-00000000000x";
                        if (db.Admin.Any(s => s.UserName == data.UserName && !s.IsDeleted && s.AdminGuid != query.AdminGuid))
                            return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.AdminUserNameExsited, Message = ResPrompt.AdminUserNameExsitedMessage };
                        query.UserName = data.UserName;
                        if (pwd != data.Password)
                        {
                            query.Password = PasswordHash.CreateHash(data.Password);
                        }
                        query.Status = data.AdminStatus;
                        query.UpdateBy = data.AdminGuid;
                        query.UpdateTime = DateTime.Now;
                        query.ProvinceID = data.Province;
                        query.CityID = data.City;
                        query.DistrictID = data.Area;
                        if (role != null)
                        {
                            query.RoleGuid = role.AdminRoleGuid;
                        }
                        db.SubmitChanges();
                        result.ResObject = true;

                        //操作日志记录
                        string parameters = "AdminGuid:" + data.AdminGuid + ",UserName:" + data.UserName + ",Password:" + data.Password;
                        dt_manager_log LogModel = new dt_manager_log();
                        LogModel.action_type = ActionEnum.Add.ToString();
                        LogModel.remark = "修改管理员成功,参数:" + parameters;
                        LogModel.AdminGuid = data.OperatorGuid;

                        _LogService.AddManagerLog(LogModel);
                    }
                    else
                    {
                        //操作日志记录
                        string parameters = "AdminGuid:" + data.AdminGuid + ",UserName:" + data.UserName + ",Password:" + data.Password;
                        dt_manager_log LogModel = new dt_manager_log();
                        LogModel.action_type = ActionEnum.Add.ToString();
                        LogModel.remark = "修改管理员失败,参数:" + parameters;
                        LogModel.AdminGuid = data.OperatorGuid;

                        _LogService.AddManagerLog(LogModel);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 管理员修改口令
        /// </summary>
        /// <param name="data"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public ResultModel ChgPwd(ChangePassword_PM data)
        {
            if (string.IsNullOrEmpty(data.OldPassword) || string.IsNullOrEmpty(data.NewPassword))
                return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.AdminPasswordCanNotNull, Message = ResPrompt.AdminPasswordCanNotNullMessage };
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var adminGuid = data.AdminGuid != null ? data.AdminGuid : data.OperatorGuid;
                var query = db.Admin.FirstOrDefault(s => s.AdminGuid == adminGuid);
                if (query != null)
                {
                    if (!PasswordHash.ValidatePassword(data.OldPassword, query.Password))
                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.AdminOldPasswordError, Message = ResPrompt.AdminOldPasswordErrorMessage };
                    query.Password = PasswordHash.CreateHash(data.NewPassword);
                    query.CreateBy = adminGuid;
                    query.UpdateTime = DateTime.Now;
                    db.SubmitChanges();
                    result.ResObject = true;

                    //操作日志记录
                    string parameters = "AdminGuid:" + data.AdminGuid + ",Password:" + data.NewPassword;
                    dt_manager_log LogModel = new dt_manager_log();
                    LogModel.action_type = ActionEnum.Add.ToString();
                    LogModel.remark = "管理员修改口令成功,参数:" + parameters;
                    LogModel.AdminGuid = data.OperatorGuid;

                    _LogService.AddManagerLog(LogModel);
                }
            }
            return result;
        }

        /// <summary>
        /// 设置管理员状态
        /// </summary>
        /// <param name="data"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public ResultModel UpdateAdminStatus(SetAdminStatus_PM data)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var query = db.Admin.FirstOrDefault(s => s.AdminGuid == data.AdminGuid);
                if (query != null)
                {
                    query.CreateBy = data.OperatorGuid;
                    query.Status = data.AdminStatus;
                    query.UpdateTime = DateTime.Now;
                    db.SubmitChanges();
                    result.ResObject = true;

                    //操作日志记录
                    string parameters = "AdminGuid:" + data.AdminGuid + ",Status:" + data.AdminStatus;
                    dt_manager_log LogModel = new dt_manager_log();
                    LogModel.action_type = ActionEnum.Add.ToString();
                    LogModel.remark = "设置管理员状态成功,参数:" + parameters;
                    LogModel.AdminGuid = data.OperatorGuid;

                    _LogService.AddManagerLog(LogModel);
                }
            }
            return result;
        }

        /// <summary>
        /// 删除管理员
        /// </summary>
        /// <param name="data"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public ResultModel DelAdmin(DeleteAdmin_PM data)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var query = db.Admin.FirstOrDefault(s => s.AdminGuid == data.AdminGuid);
                if (query != null)
                {
                    query.CreateBy = data.OperatorGuid;
                    query.IsDeleted = true;
                    query.UpdateTime = DateTime.Now;
                    db.SubmitChanges();
                    result.ResObject = true;

                    //操作日志记录
                    string parameters = "AdminGuid:" + data.AdminGuid;
                    dt_manager_log LogModel = new dt_manager_log();
                    LogModel.action_type = ActionEnum.Add.ToString();
                    LogModel.remark = "删除管理员成功,参数:" + parameters;
                    LogModel.AdminGuid = data.OperatorGuid;

                    _LogService.AddManagerLog(LogModel);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取管理员分页列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ResultModel GetAdminPageList(GetAdminPageList_PM data)
        {
            var result = new ResultModel();
            var region = Config.UserRegion;
            if (region == null)
                return result;
            using (var db = new MintBicycleDataContext())
            {
                var query = (from s in db.Admin
                             join t in db.AdminRole on s.RoleGuid equals t.AdminRoleGuid into temp
                             from tmp in temp.DefaultIfEmpty()
                             where ((string.IsNullOrEmpty(data.SearchUserName)) || s.UserName.Contains(data.SearchUserName)) && !s.IsDeleted
                             && (string.IsNullOrEmpty(data.RoleName) || tmp == null || (tmp != null && tmp.RoleName == data.RoleName))
                             && (region.ExceptUserRegion || (s.ProvinceID == region.UserProvince && (region.UserCity == null || s.CityID == region.UserCity)
                                   && (region.UserDistrict == null || s.DistrictID == region.UserDistrict)))
                             orderby s.LastLoginTime descending
                             orderby s.CreateTime descending
                             select new GetAdminPageList_OM
                             {
                                 AdminGuid = s.AdminGuid,
                                 UserName = s.UserName,
                                 RoleName = tmp == null ? "" : tmp.RoleName,
                                 CreateTime = s.CreateTime,
                                 LastLoginTime = s.LastLoginTime,
                                 LoginCount = s.LoginCount,
                                 Status = s.Status,
                                 Province = s.ProvinceID,
                                 City = s.CityID,
                                 Area = s.DistrictID
                             });
                if (query.Any())
                {
                    var list = query;
                    var cnt = list.Count();
                    var tsk = new PagedList<GetAdminPageList_OM>(list, data.PageIndex, data.PageSize, cnt);
                    result.ResObject = new { Total = cnt, List = tsk };
                }
            }
            return result;
        }

        /// <summary>
        /// 查询后台用户角色
        /// </summary>
        /// <returns></returns>
        public ResultModel GetAdminRole()
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var query = db.AdminRole.Where(s => s.Status == (byte)AdminStatusEnum.Enable && !s.IsDeleted);
                if (query.Any())
                {
                    var list = query.Select(k => k.RoleName).ToList();
                    result.ResObject = list;
                }
            }
            return result;
        }

        /// <summary>
        /// 查询后台角色列表
        /// </summary>
        /// <returns></returns>
        public ResultModel GetAdminRoleList()
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var query = db.AdminRole.Where(s => s.Status == (byte)AdminStatusEnum.Enable && !s.IsDeleted);
                if (query.Any())
                {
                    var list = query.Select(k => new { k.RoleName, k.AdminRoleGuid }).ToList();
                    result.ResObject = list;
                }
            }
            return result;
        }

        /// <summary>
        /// 查询角色关联的权限
        /// </summary>
        /// <returns></returns>
        public ResultModel GetAdminRolePerm(RolePerm_PM para)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var role = db.AdminRole.FirstOrDefault(s => s.AdminRoleGuid == para.RoleGuid && !s.IsDeleted);
                if (role == null)
                    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.NoUserTransationCode, Message = ResPrompt.NoUserTransationMessage };
                var query = (from t in db.AdminSystemMenuInfo
                             where !t.IsDeleted
                             orderby t.SortID
                             select new { t.AdminMenuGuid, t.ParentID, Checked = false, t.Title });
                if (query.Any())
                {
                    var list = new List<RolePerm_OM>();
                    var menus = db.AdminSystemMenuInfo.Where(qk => !qk.IsDeleted);
                    var perms = db.AdminRolePermissionValue.Where(ts => !ts.IsDeleted && ts.AdminRoleGuid == para.RoleGuid);
                    var func = new Func<Guid, bool>(sk =>
                    {
                        return menus.Any(pt => pt.ParentID == sk);
                    });
                    var existPerm = new Func<Guid, bool>(qp =>
                    {
                        return perms.Any(t => t.AdminMenuGuid == qp);
                    });
                    foreach (var q in query)
                    {
                        list.Add(new RolePerm_OM
                        {
                            TMenuGuid = q.ParentID,
                            MenuGuid = q.AdminMenuGuid,
                            MenuName = q.Title,
                            IsNode = q.ParentID == null,
                            ExistNode = func(q.AdminMenuGuid),
                            Checked = existPerm(q.AdminMenuGuid)
                        });
                    }
                    result.ResObject = new { Total = list.Count, List = list };
                }
                else
                {
                    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.NoUserTransationCode, Message = ResPrompt.NoUserTransationMessage };
                }
            }
            return result;
        }

        /// <summary>
        /// 更新角色关联的权限
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public ResultModel UpdateAdminRolePerm(UpdateRolePerm_PM para)
        {

            if (para == null || para.RolePerms == null || !para.RolePerms.Any())
                return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ParaModelNotExist, Message = ResPrompt.ParaModelNotExistMessage };
            var result = new ResultModel();

            try
            {
                using (var db = new MintBicycleDataContext())
                {
                    var role = db.AdminRole.FirstOrDefault(s => s.AdminRoleGuid == para.RoleGuid && !s.IsDeleted);
                    if (role == null)
                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ParaModelNotExist, Message = ResPrompt.ParaModelNotExistMessage };
                    var qt = db.Admin.FirstOrDefault(t => t.AdminGuid == para.OperatorGuid && !t.IsDeleted);
                    if (qt == null)
                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ParaModelNotExist, Message = ResPrompt.ParaModelNotExistMessage };
                    var perm = db.AdminSystemMenuInfo.FirstOrDefault(qk => qk.Title == GenericQuery.AdminRoleMenuName);
                    if (perm == null)
                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ParaModelNotExist, Message = ResPrompt.ParaModelNotExistMessage };
                    if (role.RoleName != GenericQuery.ExcAdminRoleName && !db.AdminRolePermissionValue.Any(st => st.AdminMenuGuid == perm.AdminMenuGuid && !st.IsDeleted && st.AdminRoleGuid == qt.RoleGuid))
                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.UserPermNotExist, Message = ResPrompt.UserPermNotExistMessage };
                    var perms = db.AdminRolePermissionValue.Where(sk => sk.AdminRoleGuid == role.AdminRoleGuid);
                    using (var scope = new TransactionScope())
                    {
                        if (perms.Any())
                        {
                            db.AdminRolePermissionValue.DeleteAllOnSubmit(perms);
                            db.SubmitChanges();
                        }
                        var data = new List<AdminRolePermissionValue>();
                        foreach (var tq in para.RolePerms)
                        {
                            if (tq.Checked)
                            {
                                data.Add(new AdminRolePermissionValue
                                {
                                    AdminPermissionGuid = Guid.NewGuid(),
                                    AdminMenuGuid = tq.MenuGuid,
                                    AdminRoleGuid = role.AdminRoleGuid,
                                    CreateBy = qt.AdminGuid,
                                    CreateTime = DateTime.Now,
                                    IsDeleted = false
                                });
                            }
                        }
                        if (data.Any())
                        {
                            db.AdminRolePermissionValue.InsertAllOnSubmit(data);
                            db.SubmitChanges();
                        }
                        scope.Complete();
                    }
                    //操作日志记录
                    string parameters = "RoleGuid:" + role.AdminRoleGuid;
                    dt_manager_log LogModel = new dt_manager_log();
                    LogModel.action_type = ActionEnum.Add.ToString();
                    LogModel.remark = "更新角色关联的权限成功,参数:" + parameters;
                    LogModel.AdminGuid = qt.AdminGuid;

                    _LogService.AddManagerLog(LogModel);
                }
            } catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = "更新角色关联的权限出现异常！";

                //操作日志记录
                string parameters = "AdminGuid:" + para.OperatorGuid;
                dt_manager_log LogModel = new dt_manager_log();
                LogModel.action_type = ActionEnum.Edit.ToString();
                LogModel.remark = "更新角色关联的权限出现异常" + ex.Message + ",参数:" + parameters;
                LogModel.AdminGuid = para.OperatorGuid;

                _LogService.AddManagerLog(LogModel);
            }
            return result;
        }

        /// <summary>
        /// 查询后台用户的权限
        /// </summary>
        /// <param name="roleGuid"></param>
        /// <returns></returns>
        public ResultModel GetAdminRolePermUrl(Guid roleGuid)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var perms = (from s in db.AdminRolePermissionValue
                             join t in db.AdminSystemMenuInfo on s.AdminMenuGuid equals t.AdminMenuGuid
                             where !s.IsDeleted && !t.IsDeleted && s.AdminRoleGuid == roleGuid && (t.LinkUrl != null && t.LinkUrl != "" && t.LinkUrl != "#")
                             select t.LinkUrl);
                if (perms.Any())
                {
                    var str = string.Join(",", perms.ToList());
                    result.ResObject = HttpUtility.UrlEncode(str);
                }
                else
                {
                    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.NoUserTransationCode, Message = ResPrompt.NoUserTransationMessage };
                }
            }
            return result;
        }

        /// <summary>
        /// 查询后台用户的菜单
        /// </summary>
        /// <param name="roleGuid"></param>
        /// <returns></returns>
        public ResultModel GetAdminRoleMenu(Guid roleGuid)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var perms = (from s in db.AdminRolePermissionValue
                             join t in db.AdminSystemMenuInfo on s.AdminMenuGuid equals t.AdminMenuGuid
                             where !s.IsDeleted && !t.IsDeleted && s.AdminRoleGuid == roleGuid && (t.LinkUrl != null && t.LinkUrl != "")
                             select t.Title);
                if (perms.Any())
                {
                    var str = string.Join(",", perms.ToList());
                    result.ResObject = HttpUtility.UrlEncode(str);
                }
                else
                {
                    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.NoUserTransationCode, Message = ResPrompt.NoUserTransationMessage };
                }
            }
            return result;
        }
    }
}