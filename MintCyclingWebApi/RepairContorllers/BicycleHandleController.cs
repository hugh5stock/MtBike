using Autofac;
using MintCyclingService.BicycleHandle;
using MintCyclingService.Cycling;
using MintCyclingService.Utils;
using System;
using System.Web.Http;

namespace MintCyclingWebApi.RepairContorllers
{
    /// <summary>
    /// 维护APP车锁入库匹配控制器
    /// </summary>
    public class BicycleHandleController : ApiController
    {
        private IBicycleHandleService _bicycleService;
        private ICyclingService _cyclingService;

        /// <summary>
        /// 初始化控制器
        /// </summary>
        public BicycleHandleController()
        {
            _bicycleService = AutoFacConfig.Container.Resolve<IBicycleHandleService>();
            _cyclingService = AutoFacConfig.Container.Resolve<ICyclingService>();
        }

        /// <summary>
        /// 获取锁的秘钥串加密信息   complete TOM
        /// 更新时间DATE:2017-05-29
        /// </summary>
        /// <param name="version"></param>
        /// <param name="keySerial"></param>
        /// <param name="deviceNo"></param>
        /// <param name="UserInfoGuid"></param>
        /// <returns></returns>

        [HttpGet]
        public ResultModel GetEncryptionKey(int version, string keySerial, string deviceNo, Guid UserInfoGuid)
        {
            ResultModel model = new ResultModel();
            GetEncyptionKey_PM para = new GetEncyptionKey_PM
            {
                Version = version,
                KeySerial = keySerial,
                DeviceNo = deviceNo,
                UserInfoGuid = UserInfoGuid
            };
            return _cyclingService.GetEncryptionKey(para);
        }

        /// <summary>
        /// 根据车牌号获取锁编号
        /// </summary>
        /// <param name="BikeNumber"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultModel GetLockNumber([FromUri]string BikeNumber)
        {
            return _bicycleService.GetLockNumber(BikeNumber);
        }

        /// <summary>
        /// 输入车辆编号或者扫码单车入库[暂时废弃] complete Tom
        /// DATE:2017-05-25
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ResultModel AddBicycleBaseInfoRK([FromBody]AddBicycleBase_PM model)
        {
            return _bicycleService.AddBicycleBaseInfoRK(model);
        }

        /// <summary>
        /// 车锁入库或检测绑定
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ResultModel ComponentsBinding([FromBody]ComponentsBinding data)
        {
            return _bicycleService.HelpComponentsBinding(data);
        }

        /// <summary>
        /// 车辆检测报告
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultModel BicycleDetecting([FromBody]Detecting data)
        {
            return _bicycleService.BicycleDetecting(data);
        }

        /// <summary>
        /// 检测报告列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultModel GetBicycleDetecting([FromUri]Detec_PM data)
        {
            return _bicycleService.GetBicycleDetecting(data);
        }
    }
}