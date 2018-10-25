using MintCyclingService.Breakdown;
using MintCyclingService.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MintCyclingService.Cycling.BaiduModel;

namespace MintCyclingService.BaiDu
{
    public interface IBaiduService
    {
        /// <summary>
        /// 百度API从经纬度坐标到地址的转换服务
        /// </summary>
        /// <param name="lng"></param>
        /// <param name="lat"></param>
        /// <returns></returns>
        string GetAddress(string lng, string lat);

        /// <summary>
        /// 百度API从经纬度坐标到地址的转换服务
        /// 返回省、市、区
        /// http://lbsyun.baidu.com/index.php?title=webapi/guide/webservice-geocoding#
        /// </summary>
        /// <param name="lng"></param>
        /// <param name="lat"></param>
        /// <returns></returns>
        List<AddressModel> GetAddressInfo(string lng, string lat);

        /// <summary>
        /// 计算骑行路线的距离和耗时
        /// </summary>
        /// <param name="origins"></param>
        /// <param name="destinations"></param>
        /// <param name="distance"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        string GetRidingDistance(string origins, string destinations, out double distance, out double time);

        /// <summary>
        /// 调用百度API--查询省市区ID编号
        /// </summary>
        /// <returns></returns>
        List<AddressIDList> GetBaiDuProvinceID(BaiduApiModel model);


        /// <summary>
        /// 计算步行路线的距离和耗时
        /// Route Matrix API v2.0 Beta是一套以HTTP/HTTPS形式提供的批量算路接口，返回路线规划距离和行驶时间。
        /// http://lbsyun.baidu.com/index.php?title=webapi/route-matrix-api-v2
        ///  --用户步行的距离和所用的时间
        /// http://api.map.baidu.com/routematrix/v2/walking?output=json&origins=40.45,116.34&destinations=40.34,116.45&ak=ELYtYXEH1WmDrK1wDsEWRNkGviRL0ZEo
        /// </summary>
        /// <param name="lng"></param>
        /// <param name="lat"></param>
        /// <returns></returns>
        string GetDistanceTime(string origins, string destinations, out double distance, out double time);


        /// <summary>
        /// 根据地址解析经纬度
        /// </summary>
        /// <param name="Address"></param>
        /// <returns></returns>
         Latlng GetLJByAdress(string Address);





        }
}
