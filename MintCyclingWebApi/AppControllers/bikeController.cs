using Autofac;
using MintCyclingService.BicycleHandle;
using MintCyclingService.Cycling;
using MintCyclingService.Electronicfence;
using MintCyclingService.JPush;
using MintCyclingService.LogServer;
using MintCyclingService.Remote;
using MintCyclingService.UserAccount;
using MintCyclingService.Utils;
using MintCyclingWebApi.Filter;
using System;
using System.Collections.Generic;
using System.Web.Http;
using static MintCyclingService.Cycling.AddBicycleHardWare_PM;

namespace MintCyclingWebApi.AppControllers
{
    /// <summary>
    /// 车辆模块控制器
    /// </summary>
    public class bikeController : ApiController
    {
        private ICyclingService _cyclingService;
        private IUserAccountService _accountService;
        private IElectronicfenceService _electronicService;
        private IJPushService _JPushService;
        private ILogService _LogService;
        private IRemoteService _RemoteService;

        /// <summary>
        /// 初始化车辆模块控制器
        /// </summary>
        public bikeController()
        {
            _cyclingService = AutoFacConfig.Container.Resolve<ICyclingService>();

            _accountService = AutoFacConfig.Container.Resolve<IUserAccountService>();

            _electronicService = AutoFacConfig.Container.Resolve<IElectronicfenceService>();

            _JPushService = AutoFacConfig.Container.Resolve<IJPushService>();
            _LogService = AutoFacConfig.Container.Resolve<ILogService>();
            _RemoteService = AutoFacConfig.Container.Resolve<IRemoteService>();

        }


        [HttpPost]
        public ResultModel searchbyradius([FromBody]NearBicycleEnclosure_PM model)
        {
            return _cyclingService.GetNearBicycleEnclosure(model);
        }


        /// <summary>
        /// 查询是否停放在电子围栏区域
        /// </summary>
        /// <returns></returns>
        [RequestCheck]
        [HttpPost]
        public ResultModel isinfence([FromBody]BicycleIsinRange_PM model)
        {
            return _cyclingService.QueryBicycleIsinRange(model);
        }



        /// <summary>
        /// 服务器查询开锁状态
        /// </summary>
        /// <returns></returns>
        [RequestCheck]
        [HttpGet]
        public ResultModel qunlockstatus([FromUri]string deviceNo)
        {
            return _cyclingService.QueryUnLockBicycleStatus(deviceNo);
        }



        #region  


        /// <summary>
        /// 远程开锁 complete Tom
        /// DATE:2017-06-22
        /// </summary>
        /// <returns></returns>
        //[RequestCheck]
        [HttpPost]
        public ResultModel SendRemoteOPenLock([FromBody]UnLockBicycle_PM model)
        {
            return _cyclingService.UnLockBicycle(model);
        }

        /// <summary>
        /// 出厂时硬件上传锁信息 complete TOM
        /// DATE:2017-04-27
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ResultModel HardWareAddBicycleInfo([FromBody]AddBicycleHardWare_PM model)
        {
            //Utility.Common.FileLog.Log("硬件上传锁信息：时间："+DateTime.Now+",实体信息：" + model, "AddBicycleLog");
            return _cyclingService.AddBicycleLockInfo(model);
        }


        /// <summary>
        /// 12小时锁自动上传锁信息  complete TOM
        /// DATE:2017-05-16
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ResultModel HardWareAutoUpadate([FromBody]HardWareAuto_PM model)
        {
            //Utility.Common.FileLog.Log("自动上传锁信息：时间：" + DateTime.Now + ",设备编号：" + model.DeviceNo, "自动上传日志");
           
            return _RemoteService.HardWareAutoUpadate(model);
        }


 


        /// <summary>
        /// 上传开关锁交易记录-手机蓝牙或者硬件锁上传 complete TOM
        /// DATE:2017-03-03
        /// </summary>
        /// <returns></returns>
        //[RequestCheck]
        [HttpPost]
        public ResultModel OpenOrClosekBicycleLock([FromBody]OpenOrCloseLockBicycle_PM model)
        {
            return _cyclingService.OpenOrCloseBicycleLockData(model);
        }


        /// <summary>
        /// 输入车辆编号或者扫码开锁获取该车锁的状态 complete Tom
        /// DATE:2017-04-13
        /// </summary>
        /// <returns></returns>
        [RequestCheck]
        [HttpPost]
        public ResultModel CheckInputDeviceNoOrScanCode([FromBody]InputOrScan_PM model)
        {
            return _cyclingService.CheckInputOrScan(model);
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
        [RequestCheck]
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


        #endregion

        #region 预约用车接口
        /// <summary>
        /// 根据车辆编号查询车辆类型  complete TOM
        /// DATE:2017-05-25
        /// </summary>
        /// <param name="BicycleNumber"></param>
        /// <returns></returns>
        [RequestCheck]
        [HttpGet]
        public ResultModel GetBicycleTypeNameByBicycleNumber(string BicycleNumber)
        {
            return _cyclingService.GetBicycleTypeNameByBicycleNumber(BicycleNumber);
        }




        [RequestCheck]
        [HttpPost]
        public ResultModel AddReservationBicycle(AddRegionInfo_PM model)
        {
            ResultModel result = new ResultModel();
            string errorMsg = string.Empty;

            #region 逻辑处理


            //检查预约用户是否已经有其他预约
            if (_cyclingService.CheckUserReservationExist(model.UserGuid))
            {
                return new ResultModel { IsSuccess = false, MsgCode = "0", Message = "您已经预约了其它车辆" };
            }

            //检查用户的余额
            if (!_accountService.CheckUserAvailableBalance(model.UserGuid, out errorMsg))
            {
                result.IsSuccess = false;
                result.Message = errorMsg;
                return result;
            }

            #endregion

            //添加预约用车信息
            return _cyclingService.AddReservation(model);
        }

        /// <summary>
        /// 获取用户的预约倒计时信息  complete TOM
        /// DATE:2017-04-17
        /// </summary>
        /// <returns></returns>
        [RequestCheck]
        [HttpGet]
        public ResultModel GetCountDownTimeByUserGuid([FromUri]Guid? userInfoGuid)
        {
            return _cyclingService.GetCountDownTimeByUserGuid(userInfoGuid);
        }

        /// <summary>
        /// 是否骑行中  complete TOM
        /// DATE:2017-04-21
        /// </summary>
        /// <returns></returns>
        [RequestCheck]
        [HttpGet]
        public ResultModel GetIsRidingByUserGuid([FromUri]Guid? userInfoGuid)
        {
            return _cyclingService.GetIsRidingByUserGuid(userInfoGuid);
        }



        /// <summary>
        /// 取消预约用车  complete TOM
        /// DATE:2017-03-21
        /// </summary>
        /// <returns></returns>
        [RequestCheck]
        [HttpPost]
        public ResultModel CancelReservationBicycle(UpdateReservation_PM model)
        {
            return _cyclingService.CancelReservation(model);
        }





        #endregion


    }
}