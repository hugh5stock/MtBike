using Autofac;
using MintCyclingService.Admin;
using MintCyclingService.AdminAccessCode;
using MintCyclingService.Role;
using MintCyclingService.Utils;
using MtBikeAdminWebApi;
using MtBikeAdminWebApi.Filter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.Http;

namespace MtBikeAdminWebApi.AdminControllers
{
    /// <summary>
    /// 管理员控制器
    /// </summary>
    [CheckAdminAccessCodeFilter]
    public class AdminAdminController : ApiController
    {
        private IAdminService _adminService;
        private ResultModel _adminModel = null;
        private IAdminAccessCodeService _adminAccessCodeService;
        private IAdminRoleService _adminRoleService;

        /// <summary>
        /// 管理员构造函数
        /// </summary>
        public AdminAdminController()
        {
            _adminService = AutoFacConfig.Container.Resolve<IAdminService>();
            _adminModel = WebApiApplication.GetAdminUserData();
            _adminAccessCodeService = AutoFacConfig.Container.Resolve<IAdminAccessCodeService>();
            _adminRoleService = AutoFacConfig.Container.Resolve<IAdminRoleService>();
        }

        /// <summary>
        /// 新增管理员  complete TOM
        /// DATE:2017-02-18
        /// </summary>
        /// <param name="data">参数集合</param>
        /// <returns>一般结果</returns>
        [HttpPost]
        public ResultModel AddOrUpdateAdmin(AddAdmin_PM data)
        {
            //if (!_adminModel.IsSuccess)
            //    return _adminModel;
            //var st = (_adminModel.ResObject) as Guid?;
            return _adminService.AddOrUpdateAdmin(data);
        }



        /// <summary>
        /// 管理员修改口令 complete TOM
        /// DATE:2017-02-16
        /// </summary>
        /// <param name="data">参数集合</param>
        /// <returns>一般结果</returns>
        [HttpPost]
        public ResultModel ChangePassword(ChangePassword_PM data)
        {
            //var accessCode = Guid.Parse(Request.Headers.GetValues(WebApiApplication.AccessCodeName).First());
            //var adminGuid = _adminAccessCodeService.GetAdminGuidByAccessCode(accessCode);
            return _adminService.ChgPwd(data);
        }

        /// <summary>
        /// 设置管理员状态 complete TOM
        /// DATE:2017-03-01
        /// </summary>
        /// <param name="data">参数集合</param>
        /// <returns>一般结果</returns>
        [HttpPost]
        public ResultModel SetAdminStatus(SetAdminStatus_PM data)
        {
            //if (!_adminModel.IsSuccess)
            //    return _adminModel;
            //var st = (_adminModel.ResObject) as Guid?;
            return _adminService.UpdateAdminStatus(data);
        }

        /// <summary>
        /// 删除管理员 complete TOM
        /// DATE:2017-03-01
        /// </summary>
        /// <param name="data">参数集合</param>
        /// <returns>一般结果</returns>
        [HttpPost]
        public ResultModel DeleteAdmin(DeleteAdmin_PM data)
        {
            //if (!_adminModel.IsSuccess)
            //    return _adminModel;
            //var st = (_adminModel.ResObject) as Guid?;
            return _adminService.DelAdmin(data);
        }

        /// <summary>
        /// 获取管理员分页列表 complete TOM
        /// DATE:2017-03-01
        /// </summary>
        /// <param name="data">参数集合</param>
        /// <returns>管理员分页列表</returns>
        public ResultModel GetAdminPageList([FromUri]GetAdminPageList_PM data)
        {
            return _adminService.GetAdminPageList(data);
        }

        /// <summary>
        /// 查询后台用户角色 complete TOM
        /// DATE:2017-06-01
        /// </summary>
        /// <returns></returns>
        public ResultModel GetAdminRole()
        {
            return _adminService.GetAdminRole();
        }

        /// <summary>
        /// 查询后台角色列表
        /// </summary>
        /// <returns></returns>
        public ResultModel GetAdminRoleList()
        {
            return _adminService.GetAdminRoleList();
        }

        /// <summary>
        /// 获取角色管理分页列表 complete TOM
        /// DATE:2017-06-01
        /// </summary>
        /// <param name="data">参数集合</param>
        /// <returns>角色管理分页列表</returns>
        public ResultModel GetAdminRolePageList([FromUri]GetAdminRolePageList_PM data)
        {
            return _adminRoleService.GetAdminRolePageList(data);
        }


        /// <summary>
        /// 删除角色信息 complete TOM
        /// DATE:2017-06-01
        /// </summary>
        /// <param name="data">参数集合</param>
        /// <returns>一般结果</returns>
        [HttpPost]
        public ResultModel DeleteAdminRole(DeleteAdminRole_PM data)
        {
            //if (!_adminModel.IsSuccess)
            //    return _adminModel;
            //var st = (_adminModel.ResObject) as Guid?;
            return _adminRoleService.DelAdmin(data);
        }



        /// <summary>
        /// 新增或者删除角色信息  complete TOM
        /// DATE:2017-06-01
        /// </summary>
        /// <param name="data">参数集合</param>
        /// <returns>一般结果</returns>
        [HttpPost]
        public ResultModel AddOrUpdateAdminRole(AddAdminRole_PM data)
        {
            return _adminRoleService.AddOrUpdateAdminRole(data);
        }

        /// <summary>
        /// 查询角色关联的权限
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultModel QueryAdminRolePerm([FromBody]RolePerm_PM data)
        {
            return _adminService.GetAdminRolePerm(data);
        }

        /// <summary>
        /// 更新角色关联的权限
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultModel ModifyAdminRolePerm([FromBody]UpdateRolePerm_PM data)
        {
            if (!_adminModel.IsSuccess)
                return _adminModel;
            var st = (_adminModel.ResObject) as Guid?;
            data.OperatorGuid = st ?? Guid.Empty;
            return _adminService.UpdateAdminRolePerm(data);
        }
    }
}