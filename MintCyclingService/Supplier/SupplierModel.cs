using MintCyclingService.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MintCyclingService.Supplier
{
    /// <summary>
    /// 供应商添加或者修改输入参数模型
    /// </summary>
    public class AddSupplierOrUpdate_PM
    {
        /// <summary>
        /// 供应商id
        /// </summary>

        public int SupplierID { get; set; }
        /// <summary>
        /// 供应商编号
        /// </summary>

        public string SupplierNumber { get; set; }
        /// <summary>
        /// 供应商名称
        /// </summary>

        public string SupplierName { get; set; }

        /// <summary>
        /// 供应商描述
        /// </summary>

        public string Remark { get; set; }



        /// <summary>
        /// 操作者Guid
        /// </summary>
        public Guid OperatorGuid { get; set; }

    }


    /// <summary>
    /// 供应商列表 参数模型
    /// </summary>
    public class GetSupplierList_PM : Paging_Model
    {
        /// <summary>
        /// 供应商编号
        /// </summary>

        public string SupplierNumber { get; set; }
        /// <summary>
        /// 供应商名称
        /// </summary>

        public string SupplierName { get; set; }

    }


    /// <summary>
    /// 供应商删除 输入参数模型
    /// </summary>
    public class SupplierDelete_PM
    {
        /// <summary>
        /// 供应商id
        /// </summary>

        public int SupplierID { get; set; }

        /// <summary>
        /// 操作者Guid
        /// </summary>
        public Guid OperatorGuid { get; set; }
    }

    /// <summary>
    /// 供应商列表输出参数模型
    /// </summary>
    public class SupplierList_PM
    {
        /// <summary>
        /// 供应商id
        /// </summary>

        public int SupplierID { get; set; }
        /// <summary>
        /// 供应商编号
        /// </summary>

        public string SupplierNumber { get; set; }
        /// <summary>
        /// 供应商名称
        /// </summary>

        public string SupplierName { get; set; }

        /// <summary>
        /// 供应商描述
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
