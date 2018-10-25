using Autofac;
using MintCyclingService.Breakdown;
using MintCyclingService.Cycling;
using MintCyclingService.Utils;
using System.Web.Http;
using static MintCyclingService.Cycling.BaiduModel;

namespace MintCyclingWebApi.RepairContorllers
{
    /// <summary>
    /// 维护app故障控制器
    /// </summary>
    public class WXBreakDownController : ApiController
    {
        private IBreakdownService BreakdownService;
        private ICyclingService _cyclingService;

        /// <summary>
        /// 初始化控制器
        /// </summary>
        public WXBreakDownController()
        {
            BreakdownService = AutoFacConfig.Container.Resolve<IBreakdownService>();

            _cyclingService = AutoFacConfig.Container.Resolve<ICyclingService>();
        }

        /// <summary>
        /// 查看自己管辖范围内故障车辆
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultModel GetBreakdownBikeByUser([FromUri]GetCarModel_PM data)
        {
            return BreakdownService.GetBreakdownBikeByUser(data);
        }

        /// <summary>
        /// 获取故障列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultModel GetBreakDownList([FromUri]BreakDownList_PM data)
        {
            return BreakdownService.GetBreakDownList(data);
        }

        ///// <summary>
        ///// 根据条件获取故障车辆列表
        ///// </summary>
        ///// <param name="data"></param>
        ///// <returns></returns>
        //[HttpGet]
        //public ResultModel GetBreakDownListBycondition([FromUri]BreakDownCondition data)
        //{
        //    return BreakdownService.GetBreakDownListBycondition(data);

        //}
        /// <summary>
        /// 维修单车编辑
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultModel UpdateBreakDown([FromBody]UpdateBreakDown_PM data)
        {
            return BreakdownService.UpdateBreakDown(data);
        }

        /// <summary>
        /// 维修记录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultModel RepairRecord([FromUri]RepairRecord_PM data)
        {
            return BreakdownService.RepairRecord(data);
        }

        /// <summary>
        /// 故障上报
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultModel BreakDownReport([FromBody]UpdateBreakDown_PM data)
        {
            return BreakdownService.BreakDownReport(data);
        }

        /// <summary>
        /// 维修表单
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultModel RepairForm([FromUri]RepirtForm_PM data)
        {
            return BreakdownService.RepairForm(data);
        }

        /// <summary>
        /// 维修人员开关锁
        /// DATE:2017-05-20
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ResultModel WxOpenOrClosekBicycleLock([FromBody]OpenOrCloseLockBicycle_PM model)
        {
            return _cyclingService.WxOpenOrClosekBicycleLock(model);
        }

        /// <summary>
        /// 位置上报
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultModel PositionReport([FromBody]PositionReport_PM data)
        {
            return BreakdownService.PositionReport(data);
        }

        /// <summary>
        /// 根据经纬度获取地址
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultModel GetAddressByLJ([FromUri]LatLng data)
        {
            return BreakdownService.GetAddressByLJ(data);
        }
    }
}