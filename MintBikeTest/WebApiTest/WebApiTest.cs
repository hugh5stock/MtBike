using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MintCyclingService.Supplier;
using MintCyclingService.Utils;
using MintCyclingService.Role;

namespace MintBikeTest
{
    /// <summary>
    /// 共享单车单元测试
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// 根据查询条件搜索供应商管理列表
        /// </summary>
        [TestMethod]
        public void TestGetSupplierInfoList()
        {
            var result = new ResultModel();
            ISupplierService _SupplierService = new SupplierService();


            GetSupplierList_PM model = new GetSupplierList_PM();
            model.PageIndex = 0;
            model.PageSize = 12;
            model.SupplierName = "德国西门子股份公司";
            model.SupplierNumber = "001";
            result = _SupplierService.GetSupplierInfoList(model);

            Assert.IsNotNull(result);
        }


        /// <summary>
        /// 根据查询条件搜索角色管理列表
        /// </summary>
        [TestMethod]
        public void TestGetAdminRolePageList()
        {
            var result = new ResultModel();
            IAdminRoleService _adminRoleService = new AdminRoleService();


            GetAdminRolePageList_PM model = new GetAdminRolePageList_PM();
            model.PageIndex = 0;
            model.PageSize = 12;
            model.RoleName = "超级管理员";
            result = _adminRoleService.GetAdminRolePageList(model);

            Assert.IsNotNull(result);
        }


        /// <summary>
        /// 删除角色信息
        /// </summary>
        [TestMethod]
        public void TestAdminRoleDeleteList()
        {
            var result = new ResultModel();
            IAdminRoleService _adminRoleService = new AdminRoleService();
            Guid AdminRoleGuid = new Guid("{00000000-0000-0000-0000-000000000003}");
            DeleteAdminRole_PM model = new DeleteAdminRole_PM();
            model.AdminRoleGuid = AdminRoleGuid;
            result = _adminRoleService.DelAdmin(model);

            Assert.IsNotNull(result);
        }



        /// <summary>
        /// 新增或者删除角色信息
        /// </summary>
        [TestMethod]
        public void TestAddOrUpdateAdminRole()
        {
            var result = new ResultModel();
            IAdminRoleService _adminRoleService = new AdminRoleService();
            Guid AdminRoleGuid = new Guid("{00000000-0000-0000-0000-000000000005}");
            Guid adminGuid = new Guid("{00000000-0000-0000-0000-000000000001}");

            AddAdminRole_PM model = new AddAdminRole_PM();

            //model.AdminRoleGuid = AdminRoleGuid;
            model.RoleName = "test001";
            model.OperatorGuid = adminGuid;
            result = _adminRoleService.AddOrUpdateAdminRole(model);

            Assert.IsNotNull(result);
        }


    }
}
