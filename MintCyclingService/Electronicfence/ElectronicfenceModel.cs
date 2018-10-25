using MintCyclingService.Common;
using MintCyclingService.Cycling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MintCyclingService.Electronicfence
{
    /// <summary>
    /// 后台电子围栏列表 参数模型
    /// </summary>
    public class AdminEnclosureList_PM : Paging_Model
    {
        /// <summary>
        /// 电子围栏编号
        /// </summary>
        public string EnclosureSeq { get; set; }
        /// <summary>
        /// 电子围栏所在省
        /// </summary>
        public int EnclosureProvince { get; set; }
        /// <summary>
        /// 电子围栏所在市
        /// </summary>
        public int EnclosureCity { get; set; }
        /// <summary>
        /// 电子围栏所在区
        /// </summary>
        public int EnclosureDistrict { get; set; }
 
    }

    /// <summary>
    /// 后台电子围栏列表 输出模型
    /// </summary>
    public class AdminEnclosureList_OM
    {
        /// <summary>
        /// 电子围栏Guid
        /// </summary>
        public Guid EnclosureGuid { get; set; }
        /// <summary>
        /// 电子围栏编号
        /// </summary>
        public string EnclosureNumber { get; set; }
        /// <summary>
        /// 电子围栏所在区
        /// </summary>
        public string EnclosureDistrict { get; set; }
        /// <summary>
        /// 电子围栏状态
        /// </summary>
        public string EnclosureState { get; set; }

        /// <summary>
        /// 电子围栏的经度
        /// </summary>
        public decimal EnclosureLongitude { get; set; }
        /// <summary>
        /// 电子围栏的纬度
        /// </summary>
        public decimal EnclosureLatitude { get; set; }

        /// <summary>
        /// 详细地址
        /// </summary>
        public string Address { get; set; }

 

    }

    /// <summary>
    /// 后台电子围栏列表 输出模型
    /// </summary>
    public class AdminEnclosureListNew_OM
    {

        /// <summary>
        /// 车辆总数
        /// </summary>
        public int BicycleTotalCount { get; set; }

        /// <summary>
        /// 电子围栏Guid
        /// </summary>
        public Guid ElectronicFenCingGuid { get; set; }

        ///// <summary>
        ///// 电子围栏的经度
        ///// </summary>
        //public decimal EnclosureLongitude { get; set; }
        ///// <summary>
        ///// 电子围栏的纬度
        ///// </summary>
        //public decimal EnclosureLatitude { get; set; }


        /// <summary>
        /// 区id
        /// </summary>
        public int? DistrictID { get; set; }

        /// <summary>
        /// 区名称
        /// </summary>
        public string Name { get; set; }
        

    }


    /// <summary>
    /// 后台电子围栏详情 输出模型
    /// </summary>
    public class AdminEnclosureDetail_OM
    {
        /// <summary>
        /// 电子围栏Guid
        /// </summary>
        public Guid? EnclosureGuid { get; set; }

        /// <summary>
        /// 电子围栏编号
        /// </summary>
        public string EnclosureSeq { get; set; }
        /// <summary>
        /// 电子围栏的经度
        /// </summary>
        public decimal? EnclosureLongitude { get; set; }
        /// <summary>
        /// 电子围栏的纬度
        /// </summary>
        public decimal? EnclosureLatitude { get; set; }
        /// <summary>
        /// 电子围栏所在省
        /// </summary>
        public int EnclosureProvinceId { get; set; }
        /// <summary>
        /// 电子围栏所在市
        /// </summary>
        public int EnclosureCityId { get; set; }
        /// <summary>
        /// 电子围栏所在区
        /// </summary>
        public int EnclosureDistrictId { get; set; }
        /// <summary>
        /// 电子围栏详细地址
        /// </summary>
        public string DetailAddress { get; set; }
        /// <summary>
        /// 电子围栏状态
        /// </summary>
        public int? EnclosureState { get; set; }
        /// <summary>
        /// 电子围栏备注
        /// </summary>
        public string EnclosureRemark { get; set; }
    }

    /// <summary>
    /// 后台电子围栏列表 参数模型
    /// </summary>
    public class AdminEnclosureAddAndUpdate_PM
    {
        /// <summary>
        /// 电子围栏Guid
        /// </summary>
        public Guid? EnclosureGuid { get; set; }
        /// <summary>
        /// 电子围栏编号
        /// </summary>
        public string EnclosureSeq { get; set; }
        /// <summary>
        /// 电子围栏的经度
        /// </summary>
        public decimal EnclosureLongitude { get; set; }
        /// <summary>
        /// 电子围栏的纬度
        /// </summary>
        public decimal EnclosureLatitude { get; set; }
        /// <summary>
        /// 电子围栏所在省
        /// </summary>
        public int EnclosureProvinceId { get; set; }
        /// <summary>
        /// 电子围栏所在市
        /// </summary>
        public int EnclosureCityId { get; set; }
        /// <summary>
        /// 电子围栏所在区
        /// </summary>
        public int EnclosureDistrictId { get; set; }
        /// <summary>
        /// 电子围栏详细地址
        /// </summary>
        public string DetailAddress { get; set; }
        /// <summary>
        /// 电子围栏状态
        /// </summary>
        public int EnclosureState { get; set; }
        /// <summary>
        /// 电子围栏备注
        /// </summary>
        public string EnclosureRemark { get; set; }
        /// <summary>
        /// 操作者Guid
        /// </summary>
        public Guid OperatorGuid { get; set; }
    }

    /// <summary>
    /// 后台电子围栏删除 参数模型
    /// </summary>
    public class AdminEnclosureDelete_PM
    {
        /// <summary>
        /// 电子围栏Guid
        /// </summary>
        public Guid EnclosureGuid { get; set; }
        /// <summary>
        /// 操作者Guid
        /// </summary>
        public Guid OperatorGuid { get; set; }
    }

    /// <summary>
    /// 后台电子围栏下的单车列表 输出模型
    /// </summary>
    public class AdminEnclosureBicycle_OM
    {
        /// <summary>
        /// 电子围栏下的单车数量
        /// </summary>
        public int BicyCleCount { get; set; }
        /// <summary>
        /// 单车列表
        /// </summary>
        public List<AdminEnclosureBicycleModel> BicyCleList { get; set; }
    }

    /// <summary>
    /// 后台电子围栏下的单车列表模型
    /// </summary>
    public class AdminEnclosureBicycleModel
    {
        /// <summary>
        /// 单车编号
        /// </summary>
        public string BicycleNumber { get; set; }
        /// <summary>
        /// 单车类型
        /// </summary>
        public string BicyCleType { get; set; }
        /// <summary>
        /// 单车状态
        /// </summary>
        public string BicyCleState { get; set; }
 

        /// <summary>
        /// 单车电压
        /// </summary>
        public string Voltage { get; set; }

    }




    /// <summary>
    /// 查询电子围栏 输入参数模型
    /// </summary>
    public class GetElectrlnic_PM:Paging_Model
    {
        /// <summary>
        /// 关键字搜索
        /// </summary>
        public string KeyWordName { get; set; }


    }
        /// <summary>
        /// 查询电子围栏 输出参数模型
        /// </summary>
        public class GetElectrlnic_OM
    {
        /// <summary>
        /// 电子围栏Guid
        /// </summary>
        public Guid? ElectronicFenCingGuid { get; set; }
       
        /// <summary>
        /// 电子围栏的经度
        /// </summary>
        public decimal Longitude { get; set; }
        /// <summary>
        /// 电子围栏的纬度
        /// </summary>
        public decimal Latitude { get; set; }

     
        /// <summary>
        /// 电子围栏状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 电子围栏详细地址
        /// </summary>
        public string Address { get; set; }
    }

    //电子围栏列表
    public class ElectronicList
    {
        /// <summary>
        /// 电子围栏列表
        /// </summary>
        public List<GetElectrlnic_OM> Electrlniclist { get; set; }
    }



    /// <summary>
    /// 后台地图电子围栏显示 参数模型
    /// </summary>
    public class SearchElectronic_PM
    {
        ///// <summary>
        ///// 电子围栏编号
        ///// </summary>
        //public string EnclosureNumber { get; set; }
 
        /// <summary>
        /// 电子围栏所在省
        /// </summary>
        public int EnclosureProvince { get; set; }
        /// <summary>
        /// 电子围栏所在市
        /// </summary>
        public int EnclosureCity { get; set; }
        /// <summary>
        /// 电子围栏所在区
        /// </summary>
        public int EnclosureDistrict { get; set; }

    }


    /// <summary>
    /// 后台范围内的车辆电子围栏 参数模型
    /// </summary>
    public class MapBicycleEnclosure_PM
    {
        ///// <summary>
        ///// 经度
        ///// </summary>
        //public decimal CurLongitude { get; set; }

        ///// <summary>
        ///// 纬度
        ///// </summary>
        //public decimal CurLatitude { get; set; }

            /// <summary>
            /// 经纬度或者电子围栏编号
            /// </summary>
         public string lngLatOrEnclosureNumber { get; set; }



        /// <summary>
        /// 区分是地址还是编号
        /// </summary>
        public string Type { get; set; }


        /// <summary>
        /// 查询的半径
        /// </summary>
        public decimal? Radius { get; set; }


    }


    /// <summary>
    /// 后台统计输出参数
    /// </summary>
    public class GetTotalData_OM
    {
 

        /// <summary>
        /// 电子围栏总数
        /// </summary>
        public int ElecTotalCount { get; set; }

        /// <summary>
        /// 车辆总数
        /// </summary>
        public int BicyleTotalCount { get; set; }

        /// <summary>
        /// 使用数
        /// </summary>
        public int UseTotalCount { get; set; }

        /// <summary>
        /// 空闲数
        /// </summary>
        public int FreeTotalCount { get; set; }

        /// <summary>
        /// 故障数
        /// </summary>
        public int breakdownTotalCount { get; set; }

    }

    /// <summary>
    /// 地图电子围栏统计
    /// </summary>
    public class SearchTj_PM
    {
 
        /// <summary>
        /// 电子围栏所在省
        /// </summary>
        public int ProvinceID { get; set; }
        /// <summary>
        /// 电子围栏所在市
        /// </summary>
        public int CityID { get; set; }
        /// <summary>
        /// 电子围栏所在区
        /// </summary>
        public int DistrictID { get; set; }

    }

    /// <summary>
    ///输出参数
    /// </summary>
    public class SearchTj_OM
    {

        /// <summary>
        /// 区名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 电子围栏数
        /// </summary>
        public int EleCount { get; set; }
        /// <summary>
        /// 车辆数
        /// </summary>
        public int bicycleCount { get; set; }

        /// <summary>
        /// 总条数
        /// </summary>
      //  public int TotalCount { get; set; }

    }


    public class SearchTjList_OM
    {
        /// <summary>
        /// 地图电子围栏列表
        /// </summary>
        public List<SearchTj_OM> EnclosureList { get; set; }

       

        /// <summary>
        /// 电子围栏的集合数量
        /// </summary>
        public int Totalcount { get; set; }
 


    }


    /// <summary>
    /// 地图电子围栏 参数模型
    /// </summary>
    public class EnclosureList_PM
    {
 
        /// <summary>
        /// 电子围栏编号
        /// </summary>
        public string ElectronicFenCingNumber { get; set; }
    }



    /// <summary>
    /// 后台地图显示助力车车辆输出模型
    /// </summary>
    public class AdminMapHelpBicycleModel_OM
    {
        /// <summary>
        /// 电子围栏外单车经度
        /// </summary>
        public decimal? BicycleLongitude { get; set; }

        /// <summary>
        /// 电子围栏外单车纬度
        /// </summary>
        public decimal? BicycleLatitude { get; set; }

        /// <summary>
        /// 车辆编号
        /// </summary>
        public string DeviceNo { get; set; }

        /// <summary>
        /// 车辆类型：0非助力车；1助力车
        /// </summary>
        public int BicyCleTypeName { get; set; }


    }



    /// <summary>
    /// 后台地图输出模型
    /// </summary>
    public class AdminBicycleEnclosureMap_OM
    {

        /// <summary>
        /// 后台地图电子围栏列表
        /// </summary>
        public List<MapEnclosureModel> EnclosureList { get; set; }

        /// <summary>
        /// 后台地图非助力车辆列表
        /// </summary>
        public List<AdminMapHelpBicycleModel_OM> NoHelpBicycleList { get; set; }



        /// <summary>
        /// 地图中的助力车辆列表
        /// </summary>
        public List<AdminMapHelpBicycleModel_OM> HelpBicycleList { get; set; }



        /// <summary>
        /// 电子围栏的集合数量
        /// </summary>
        public int Totalcount { get; set; }

        /// <summary>
        /// 非助力车辆的集合数量
        /// </summary>
        public int NoHelpTotalcount { get; set; }


        /// <summary>
        /// 助力车辆的集合数量
        /// </summary>
        public int HelpTotalcount { get; set; }


    }



}