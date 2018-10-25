using MintCyclingService.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MintCyclingService.Breakdown
{
    /// <summary>
    /// 后台故障维护列表 参数模型
    /// </summary>
    public class GetBreakdownList_PM : Paging_Model
    {
        /// <summary>
        /// 车辆编号
        /// </summary>
        public string BicyCleNumber { get; set; }
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

        /// <summary>
        /// 区分是查询还是导出Excel
        /// </summary>
        public string T { get; set; }

    }

    /// <summary>
    /// 后台故障维护列表 输出模型
    /// </summary>
    public class GetBreakdownList_OM
    {
        /// <summary>
        /// 故障Guid
        /// </summary>
        public Guid BreakdownGuid { get; set; }

        //车辆编号
        public string BicyCleNumber { get; set; }


        /// <summary>
        /// 区域
        /// </summary>
        public string DistricName { get; set; }

        /// <summary>
        /// 故障类型
        /// </summary>
        public string BreakTypeName { get; set; }

        /// <summary>
        /// 维修等级
        /// </summary>
        public string GradeName { get; set; }


        /// <summary>
        /// 经度
        /// </summary>
        public decimal? Longitude { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public decimal? Latitude { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int? Status { get; set; }

        //地址
        public string Address { get; set; }

        //备注
        public string Remark { get; set; }

        //故障维护图片
        public string PicLoc { get; set; }

        public Guid? PhotoGuid { get; set; }


        public DateTime? BreakDownTime { get; set; }
    }


    public class BreakdownListreturn
    {

        public List<GetBreakdownList_OM> List { get; set; }

        public int Total { get; set; }


    }


    /// <summary>
    /// 后台故障维护详情 输出模型
    /// </summary>
    public class GetBreakdownDetail_OM
    {
        /// <summary>
        /// 故障Guid
        /// </summary>
        public Guid BreakdownGuid { get; set; }

        //车辆编号
        public string BicyCleNumber { get; set; }

        /// <summary>
        /// 故障类型
        /// </summary>
        public int? BreakTypeNameID { get; set; }

        /// <summary>
        /// 维修等级
        /// </summary>
        public int? GradeNameID { get; set; }


        /// <summary>
        /// 经度
        /// </summary>
        public decimal? Longitude { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public decimal? Latitude { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int? Status { get; set; }

        //地址
        public string Address { get; set; }

        //备注
        public string Remark { get; set; }
        public List<string> PicLoc { get; set; }
    }


    /// <summary>
    /// 故障维护编辑或添加   输入参数
    /// </summary>
    public class AddOrUpdateBreakdown_PM
    {
        /// <summary>
        /// 故障Guid
        /// </summary>
        public Guid BreakdownGuid { get; set; }

        //车辆编号
        public string BicyCleNumber { get; set; }

        /// <summary>
        /// 故障类型
        /// </summary>
        public int BreakTypeNameID { get; set; }

        /// <summary>
        /// 维修等级
        /// </summary>
        public int GradeNameID { get; set; }


        /// <summary>
        /// 经度
        /// </summary>
        public decimal Longitude { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public decimal Latitude { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }

        //详细地址
        public string Address { get; set; }

        //备注
        public string Remark { get; set; }

        /// <summary>
        /// 操作者Guid
        /// </summary>
        public Guid OperatorGuid { get; set; }


    }


    public class listModel_OM
    {
        public int Total { get; set; }
        public List<GetBreakdownList_OM> List { get; set; }

    }

    /// <summary>
    /// 导入Excel   输入参数
    /// </summary>
    public class ImportExcelData_PM
    {
        public string exportFile { get; set; }

    }

    /// <summary>
    /// 故障Guid 输入模型
    /// </summary>
    public class GetBreakdown_PM
    {
        /// <summary>
        /// guid
        /// </summary>
        public Guid BreakdownPhotoGuid { get; set; }
    }

    /// <summary>
    /// 上报故障信息   输入参数
    /// </summary>
    public class AddBreakdownLog_PM
    {

        //用户Guid
        public Guid UserInfoGuid { get; set; }

        //车辆编号
        public string BicyCleNumber { get; set; }

        /// <summary>
        /// 故障类型
        /// </summary>
        public List<int> BreakTypeName { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public decimal? Longitude { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public decimal? Latitude { get; set; }

        //备注
        public string Remark { get; set; }


        //故障图片Guid
        public Guid BreakdownPhotoGuid { get; set; }

    }

    /// <summary>
    /// 后台故障维护列表 参数模型
    /// </summary>
    public class GetBreakList_PM : Paging_Model
    {
        /// <summary>
        /// Guid
        /// </summary>
        public Guid ServicePersonID { get; set; }
    }

    public  class PhotoBreak_OM
    {
        public  Guid PhotoGuid { get; set; }
        public Guid BreakDownPhotoGuid { get; set; }

    }



    public class GetCarModel_PM
    {
        public int ProvinceID { get; set; }

        public int CityID { get; set; }

        public int DistinctId { get; set; }

    }

    public class BreakDownList_PM : Paging_Model
    {
        public bool TodayBreakDown { get; set; }

        public DateTime? BreakDownStartTime { get; set; }
        public DateTime? BreakDownEndTime { get; set; }

        public int CityId { get; set; }

        public int DistinctId { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public string KeyWord { get; set; }

    }

    public class BreakDownList_OM
    {
        public Guid BreakDownGuid { get; set; }

        public string BikeNumber { get; set; }
    
        public decimal Distance { get; set; }
        public string DistictName { get; set; }

        public DateTime? ReportTime { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }


    }

    public class BreakDownList
    {
        public Guid BreakDownGuid { get; set; }

        public string BikeNumber { get; set; }
        public  List<int?> BreakDownType { get; set; }
        public decimal Distance { get; set; }
        public string DistictName { get; set; }

        public DateTime? ReportTime { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }


    }

    public class BreakDownCondition : Paging_Model
    {

        public string KeyWord { get; set; }

        public BreakdownEnum BreakdownType { get; set; }

        public int ProvinceId { get; set; }
        public int CityId { get; set; }
        public int DistinctId { get; set; }

        public DateTime? StartReportTime { get; set; }
        public DateTime? EndReportTime { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }


    }

    public class UpdateBreakDown_PM
    {

        public string Adress { get; set; }

        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }

        public string Remark { get; set; }
        public Guid UserInfoGuid { get; set; }
        public ServiceStatus ServiceStatus { get; set; }

        public GreaGradeNameEnum GreaGradeType { get; set; }
        public string BikeNumber { get; set; }
        public List<int> BreakDownType { get; set; }

        public Guid ICBCPhotoGuid { get; set; }

        public  Guid  BreakPhotoGuid { get; set; }
    }

    public class Latlng
    {

        public bool IsSuccess { get; set; }

        public string message { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }

    }

    public class RepairRecord_PM : Paging_Model
    {

        public string BikeNumber { get; set; }

        public bool LastRepair { get; set; }


    }

    public class AdminRepairRecord_PM : Paging_Model
    {

        public string BikeNumber { get; set; }

        public string  UserName{ get; set; }

        public DateTime? RepirtStartTime { get; set; }

         public  DateTime? RepirtEndTime { get; set; }

   

    }


    public class RepairRecord_OM
    {

        public string BikeNumber { get; set; }
        public string BreakDownType { get; set; }
        public DateTime RepirtTime { get; set; }

        public string ServiceStatus { get; set; }

        public string RepirtName { get; set; }



    }


    public class RepairRecord
    {

        public string BikeNumber { get; set; }
        public string BreakDownType { get; set; }
        public DateTime RepirtTime { get; set; }

        public string ServiceStatus { get; set; }

        public string RepirtName { get; set; }

        public  Guid? BreakDownPhoto { get; set; }

        public Guid ICBCPhoto { get; set; }

    }


    public class AdminRepairRecord_OM
    {

        public string BikeNumber { get; set; }
        public string BreakDownType { get; set; }
        public DateTime RepirtTime { get; set; }

        public string ServiceStatus { get; set; }

        public string RepirtName { get; set; }

      public List<string> BreakDownPhotoList { get; set; }

        public string ICBCPhoto { get; set; }

    }




    public class RepirtForm_PM : Paging_Model
    {


        public Guid UserInfoGuid { get; set; }
        public DateTime? StartRepirtTime { get; set; }
        public DateTime? EndRepirTime { get; set; }




    }
    public class RepirtForm_OM
    {

        public string Address { get; set; }
        public string BikeNumber { get; set; }

        public string BreakDownType { get; set; }

        public string ServerStatus { get; set; }


        public DateTime? RepirtTime { get; set; }
    }

    public class PositionReport_PM
    {

        public string BikeNumber { get; set; }

        public string LockNumber { get; set; }

        public string SIMNumber { get; set; }

        public decimal? MobileLat { get; set; }
        public decimal? MobileLng { get; set; }

        public decimal? LockLat { get; set; }

        public decimal? LockLng { get; set; }

        public string Address { get; set; }


        public string Remark { get; set; }


        public Guid UserInfoGuid { get; set; }

    }





}
