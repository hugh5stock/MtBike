using MintCyclingService.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MintCyclingService.Customer
{
    /// <summary>
    /// 客户添加或者修改输入参数模型
    /// </summary>
    public class AddCustomerOrUpdate_PM
    {
        /// <summary>
        /// 客户id
        /// </summary>

        public int CustomerID { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>

        public string CustomerName { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 是否启用0未启用；1启用
        /// </summary>
        public int IsLock { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        public int ProvinceId { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        public int CityId { get; set; }

        /// <summary>
        /// 区
        /// </summary>
        public int DistinctId { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 客户描述
        /// </summary>

        public string Remark { get; set; }



        /// <summary>
        /// 操作者Guid
        /// </summary>
        public Guid OperatorGuid { get; set; }

    }


    /// <summary>
    /// 客户列表 参数模型
    /// </summary>
    public class GetCustomerList_PM : Paging_Model
    {
        /// <summary>
        /// 客户名称
        /// </summary>

        public string CustomerName { get; set; }

    }


    /// <summary>
    /// 客户删除 输入参数模型
    /// </summary>
    public class CustomerDelete_PM
    {
        /// <summary>
        /// 客户id
        /// </summary>

        public int CustomerID { get; set; }

        /// <summary>
        /// 操作者Guid
        /// </summary>
        public Guid OperatorGuid { get; set; }
    }

    /// <summary>
    /// 客户列表输出参数模型
    /// </summary>
    public class CustomerList_PM
    {
        /// <summary>
        /// 客户id
        /// </summary>

        public int CustomerID { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>

        public string CustomerName { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 是否启用0未启用；1启用
        /// </summary>
        public int IsLock { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        public int ProvinceId { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        public int CityId { get; set; }

        /// <summary>
        /// 区
        /// </summary>
        public int DistinctId { get; set; }

        /// <summary>
        /// 所在省市区
        /// </summary>

        public string DistricName { get; set; }


        /// <summary>
        /// 详细地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Remark { get; set; }


        /// <summary>
        /// 操作者
        /// </summary>
        public Guid? OperatorGuid { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

    }



}
