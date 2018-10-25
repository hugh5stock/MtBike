using MintCyclingService.Common;
using System;

namespace MintCyclingService.BicLock
{
    /// <summary>
    /// 后台供应商列表输出参数模型
    /// </summary>
    public class BicLock_PM : Paging_Model
    {
        public string CustormName { get; set; }
        public string SupplierName { get; set; }
        public string LockNumber { get; set; }
    }

    /// <summary>
    /// 后台获取生产锁管理列表
    /// </summary>
    public class BicLock_OM
    {
        public int SupplierID { get; set; }
        public string SupplierName { get; set; }

        public string BikeNumber { get; set; }
        public string LockNumber { get; set; }

        public string LockType { get; set; }
        public string Remark { get; set; }
        public string CustomName { get; set; }
        public DateTime? ProductTime { get; set; }
    }


    #region 分配客户车锁管理模型

    public class CustomerBicycleLocklist_PM : Paging_Model
    {
    
        /// <summary>
        /// 锁编号
        /// </summary>
        public string LockNumber { get; set; }

     
        /// <summary>
        /// 车辆编号
        /// </summary>
        public string BicycleNumber { get; set; }
 
        /// <summary>
        /// 客户名称
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }


        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }


    }



    /// <summary>
    /// 分配客户车锁列表输出参数模型
    /// </summary>
    public class CustomerBicycleLocklist_OM
    {
        /// <summary>
        /// 分配Guid
        /// </summary>
        public Guid? DistributionGuid { get; set; }

        /// <summary>
        /// 锁的Guid
        /// </summary>
        public Guid? LockGuid { get; set; }


        /// <summary>
        /// 锁编号
        /// </summary>
        public string LockNumber { get; set; }

        /// <summary>
        /// 车辆Guid
        /// </summary>
        public Guid? BicycleGuid { get; set; }

        /// <summary>
        /// 车辆编号
        /// </summary>
        public string BicycleNumber { get; set; }
 
        /// <summary>
        /// 客户编号
        /// </summary>
        public int CustomerID { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string CustomerName { get; set; }


        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }


        /// <summary>
        /// 分配时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

 
    }


    /// <summary>
    /// 分配客户车锁输入参数模型
    /// </summary>
    public class AddCustomerBicycleLockJson_PM
    {
        /// <summary>
        /// 接受页面的Json对象
        /// </summary>
        public string StrJson { get; set; }
    }

    /// <summary>
    /// 分配客户车锁输入参数模型
    /// </summary>
    public class AddCustomerBicycleLock_PM
    {
        /// <summary>
        /// 分配Guid
        /// </summary>
        public Guid? DistributionGuid { get; set; }

        /// <summary>
        /// 锁的Guid
        /// </summary>
        public string LockGuid { get; set; }
        /// <summary>
        /// 车辆Guid
        /// </summary>
        public string BicycleGuid { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        public int CustomerID { get; set; }


        /// <summary>
        /// 是否启用0未启用；1启用，默认启用
        /// </summary>
        public int Status { get; set; }

        //备注
        public string Remark { get; set; }

        /// <summary>
        /// 操作者Guid
        /// </summary>
        public Guid OperatorGuid { get; set; }


    }

    #endregion
 
    #region 车辆列表管理模型

    /// <summary>
    /// 后台车辆列表 参数模型
    /// </summary>
    public class GetBicycleBaseList_PM : Paging_Model
    {
        /// <summary>
        /// 车辆编号
        /// </summary>
        public string BicyCleNumber { get; set; }


        /// <summary>
        /// 状态：0未使用；1使用中；2故障；
        /// </summary>
        public int LockStatus { get; set; }

        /// <summary>
        /// 所在省
        /// </summary>
        public int ProvinceID { get; set; }
        /// <summary>
        /// 所在市
        /// </summary>
        public int CityID { get; set; }
        /// <summary>
        /// 所在区
        /// </summary>
        public int DistrictID { get; set; }

    }

    /// <summary>
    /// 后台车辆列表 输出模型
    /// </summary>
    public class GetBicycleBaseList_OM
    {
        /// <summary>
        /// 车辆Guid
        /// </summary>
        public Guid BicycleGuid { get; set; }
        /// <summary>
        /// 锁的Guid
        /// </summary>
        public Guid LockGuid { get; set; }

        //车辆编号
        public string BicyCleNumber { get; set; }


        //锁编号
        public string LockNumber { get; set; }


        /// <summary>
        /// 车辆类型
        /// </summary>
        public string BicycleTypeName { get; set; }

        /// <summary>
        /// 区域
        /// </summary>
        public string DistricName { get; set; }

        /// <summary>
        /// 状态 车锁状态：0已开锁；1已关锁；2故障；3电量不足
        /// </summary>
        public string LockStatus { get; set; }

        /// <summary>
        /// 电压
        /// </summary>
        public decimal? Voltage { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public bool Checked { get; set; }

    }


    /// <summary>
    /// 后台车辆详情 输入模型
    /// </summary>
    public class GetBicDeatilsModel_PM
    {
        /// <summary>
        /// 车辆Guid
        /// </summary>
        public Guid BicycleBaseGuid { get; set; }
    }
    /// <summary>
    /// 后台车辆详情 输出模型
    /// </summary>
    public class GetBicDeatilsModel_OM
    {
        /// <summary>
        /// 车辆编号
        /// </summary>
        public String BicycleNumber { get; set; }

        /// <summary>
        /// 车辆类型
        /// </summary>
        public String BicyCleTypeName { get; set; }

        //锁编号
        public string LockNumber { get; set; }


        /// <summary>
        /// 状态 车锁状态：0已开锁；1已关锁；2故障；3电量不足
        /// </summary>
        public string LockStatus { get; set; }

        /// <summary>
        /// 区域
        /// </summary>
        public string DistricName { get; set; }


        /// <summary>
        /// 电压
        /// </summary>
        public decimal? Voltage { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

    }

    /// <summary>
    /// 后台车辆删除 输入参数模型
    /// </summary>
    public class BicycleDelete_PM
    {
        /// <summary>
        /// 车辆Guid
        /// </summary>
        public Guid BicycleBaseGuid { get; set; }

        /// <summary>
        /// 操作者Guid
        /// </summary>
        public Guid OperatorGuid { get; set; }
    }

    /// <summary>
    /// 后台添加或者更新车辆信息 输入参数模型
    /// </summary>
    public class AddBicycleOrUpdate_PM
    {
        //车辆Guid
        public Guid? LockGuid { get; set; }

        //锁的编号
        public string LockNumber { get; set; }


        //设备编号
        public string DeviceNo { get; set; }

        //密钥
        public string SecretKey { get; set; }

        //车辆类型
        public Guid BicycleTypeGuid { get; set; }


        //车辆归属者
        public Guid AdminGuid { get; set; }

        ////车锁编号
        //public string LockNumber { get; set; }

        ////车锁MAC
        //public string LockMac { get; set; }

        ////经度
        //public decimal Longitude { get; set; }

        ////纬度
        //public decimal Latitude { get; set; }


        ////地址
        //public string Address { get; set; }

        //电量状态
        public int ElectricQuantityStatus { get; set; }


        //车锁状态
        public int LockStatus { get; set; }


        //备注
        public string Remark { get; set; }

        /// <summary>
        /// 操作者Guid
        /// </summary>
        public Guid OperatorGuid { get; set; }

    }

    #endregion

    #region 生成车辆编号管理模型
    /// <summary>
    /// 生成随机的自行车编号-废弃
    /// </summary>
    public class GetRandomNumModel_PM1
    {

        /// <summary>
        /// 生成的个数
        /// </summary>
        public int num { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        public int CustomerID { get; set; }

    }


    /// <summary>
    /// 生成随机的自行车编号
    /// </summary>
    public class GetRandomNumModel_PM
    {

        /// <summary>
        /// 生成的个数
        /// </summary>
        public int num { get; set; }

        /// <summary>
        /// 最小数
        /// </summary>
        public int minValue { get; set; }

        /// <summary>
        /// 最大数
        /// </summary>
        public int maxValue { get; set; }


        /// <summary>
        /// 客户编号
        /// </summary>
        public int CustomerID { get; set; }

    }

    /// <summary>
    /// 批量查询自行车编号列表输入参数模型
    /// </summary>
    public class GetBicycleNumberList_PM : Paging_Model
    {

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }


        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }



        /// <summary>
        /// 客户名称
        /// </summary>
        public string CustomerName { get; set; }

    }


    /// <summary>
    /// 批量查询自行车编号列表输出参数模型
    /// </summary>
    public class GetBicycleOutList_PM
    {
        /// <summary>
        /// ID
        /// </summary>
        public int ProduceID { get; set; }

        /// <summary>
        /// 车辆编号
        /// </summary>
        public long BicycleNumber { get; set; }

        //客户名称
        public string CustomerName { get; set; }

        /// <summary>
        /// 生成时间
        /// </summary>
        public DateTime CreateTime { get; set; }

    }


    #endregion




  
}