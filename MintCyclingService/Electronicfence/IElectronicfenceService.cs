using MintCyclingService.Cycling;
using MintCyclingService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MintCyclingService.Electronicfence
{
    public interface IElectronicfenceService
    {
        /// <summary>
        /// 查询后台省市区列表
        /// </summary>
        /// <returns></returns>
        ResultModel GetProvinceCityData();

        /// <summary>
        /// 根据查询条件搜索后台电子围栏列表
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        ResultModel GetElectronicfenceListByCondition(AdminEnclosureList_PM para);

        /// <summary>
        /// 查询电子围栏详情
        /// </summary>
        /// <param name="electronicfenceGuid"></param>
        /// <returns></returns>
        ResultModel GetElectronicfenceDetail(Guid electronicfenceGuid);

        /// <summary>
        /// 新增或修改指定的电子围栏数据
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        ResultModel AddAndUpdateElectronicfence(AdminEnclosureAddAndUpdate_PM para);

        /// <summary>
        /// 删除指定的电子围栏
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        ResultModel RemoveElectronicfence(AdminEnclosureDelete_PM para);

        /// <summary>
        /// 查询电子围栏下的单车列表
        /// </summary>
        /// <param name="electronicfenceGuid"></param>
        /// <returns></returns>
        ResultModel GetElectronicfenceBicycle(Guid electronicfenceGuid);


        /// <summary>
        /// 根据选择省市区三级条件搜索电子围栏数和车辆总数信息
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        ResultModel GetMapElectronicListByCondition(SearchElectronic_PM para);

        /// <summary>
        /// 根据输入的地址或者电子围栏编号搜索一定范围内电子围栏信息中的车辆数和没有在电子围栏内的车辆信息
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        ResultModel GetBicycleEnclosureList(MapBicycleEnclosure_PM para);

        /// <summary>
        /// 生成电子围栏编号规则
        /// </summary>
        /// <returns></returns>
        ResultModel GetCreateElectronicFenCingNumber();

        /// <summary>
        /// 统计后台车辆信息和电子围栏信息
        /// </summary>
        /// <returns></returns>
        ResultModel GetStatisticalList();

        /// <summary>
        /// 根据地理位置信息获取车辆以及电子围栏
        /// </summary>
        /// <param name="para">地理位置信息</param>
        /// <returns></returns>
        ResultModel GetBicycleEnclosureListByGEOHash(MapBicycleEnclosure_PM para);

        /// <summary>
        /// 统计区域的电子围栏总数和电子围栏下的车辆总数
        /// </summary>
        /// <returns></returns>
        ResultModel GetElectronicTjCountByCondition(SearchTj_PM para);



        /// <summary>
        /// 查询电子围栏下的单车列表
        /// </summary>
        /// <param name="ElectronicFenCingNumber"></param>
        /// <returns></returns>
        ResultModel GetElectronicfenceBicycleInfo(EnclosureList_PM para);
 


        }
}
