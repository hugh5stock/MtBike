using Autofac;
using MintCyclingService.Electronicfence;
using MintCyclingService.Utils;
using MtBikeAdminWebApi.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MtBikeAdminWebApi.AdminControllers
{
    /// <summary>
    /// 电子围栏控制器
    /// </summary>
    //[CheckAdminAccessCodeFilter]
    public class ElectronicfenceController : ApiController
    {
        IElectronicfenceService _electronicfenceService;

        /// <summary>
        /// 初始化电子围栏模块控制器
        /// </summary>
        public ElectronicfenceController()
        {
            _electronicfenceService= AutoFacConfig.Container.Resolve<IElectronicfenceService>();
        }

        /// <summary>
        /// 查询后台省市区列表
        /// </summary>
        /// <returns></returns>
        public ResultModel QueryProvinceCityData()
        {
            return _electronicfenceService.GetProvinceCityData();
        }

        /// <summary>
        /// 根据查询条件搜索后台电子围栏列表 complete TOM
        /// DATE:2017-02-24
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel QueryElectronicfenceListByCondition([FromBody]AdminEnclosureList_PM model)
        {
            return _electronicfenceService.GetElectronicfenceListByCondition(model);
        }

        /// <summary>
        /// 查询电子围栏详情 complete TOM
        /// DATE:2017-02-24
        ///
        /// </summary>
        /// <param name="electronicfenceGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultModel QueryElectronicfenceDetail([FromUri]Guid electronicfenceGuid)
        {
            return _electronicfenceService.GetElectronicfenceDetail(electronicfenceGuid);
        }

        /// <summary>
        /// 新增或修改指定的电子围栏数据 complete TOM
        /// DATE:2017-02-23
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultModel AddOrEditElectronicfence([FromBody]AdminEnclosureAddAndUpdate_PM model)
        {
            return _electronicfenceService.AddAndUpdateElectronicfence(model);
        }

        /// <summary>
        /// 删除指定的电子围栏 complete TOM
        /// DATE:2017-02-27
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultModel CancelElectronicfence([FromBody]AdminEnclosureDelete_PM model)
        {
            return _electronicfenceService.RemoveElectronicfence(model);
        }

        /// <summary>
        /// 根据电子围栏的Guid查询电子围栏下的单车列表 complete TOM
        /// DATE:2017-02-22
        /// </summary>
        /// <param name="electronicfenceGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultModel GetElectronicfenceBicycle([FromUri]Guid electronicfenceGuid)
        {
            return _electronicfenceService.GetElectronicfenceBicycle(electronicfenceGuid);
        }


        /// <summary>
        /// 根据电子围栏编号查询电子围栏下的单车列表 complete TOM
        /// DATE:2017-05-15
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ResultModel GetElectronicfenceBicycleInfo([FromBody]EnclosureList_PM model)
        {
            return _electronicfenceService.GetElectronicfenceBicycleInfo(model);
        }


        /// <summary>
        /// 根据选择省市区三级条件搜索电子围栏数和车辆总数信息
        /// DATE:2017-03-20
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultModel SearchaMapElectronicListByCondition([FromBody]SearchElectronic_PM model)
        {
            return _electronicfenceService.GetMapElectronicListByCondition(model);
        }


        /// <summary>
        /// 根据查询条件搜索后台地图上显示是我电子围栏中的车辆总数信息和电子围栏外的车辆  complete TOM
        /// DATE:2017-03-17
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultModel SearchaMapElectronicListByConditionNew([FromBody]MapBicycleEnclosure_PM model)
        {
            return _electronicfenceService.GetBicycleEnclosureList(model);
        }

          /// <summary>
        /// 根据查询条件搜索后台地图上显示是我电子围栏中的车辆总数信息和电子围栏外的车辆[暂时废弃]  complete TOM
        /// DATE:2017-03-17
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultModel SearchaMapElectronicListByGEOHash([FromBody]MapBicycleEnclosure_PM model)
        {
            return _electronicfenceService.GetBicycleEnclosureListByGEOHash(model);
        }

        /// <summary>
        /// 生成电子围栏编号 complete TOM
        /// DATE:2017-03-20
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultModel  CreateElectronicFenCingNumber()
        {
            return _electronicfenceService.GetCreateElectronicFenCingNumber();
        }

        /// <summary>
        /// 统计车辆新和电子围栏信息   complete TOM
        /// DATE:2017-03-20
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ResultModel GetStatisticalListData()
        {
            return _electronicfenceService.GetStatisticalList();
        }


        /// <summary>
        /// 统计区域的电子围栏总数和电子围栏下的车辆总数   complete TOM
        /// DATE:2017-5-6
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ResultModel GetElectronicTjListByCondition([FromBody]SearchTj_PM model)
        {
            return _electronicfenceService.GetElectronicTjCountByCondition(model);
        }



    }


}