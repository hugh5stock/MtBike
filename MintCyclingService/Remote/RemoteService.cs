using MintCyclingData;
using MintCyclingService.BaiDu;
using MintCyclingService.BicycleHandle;
using MintCyclingService.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using Utility;
using Utility.LogHelper;
using static MintCyclingService.Cycling.BaiduModel;

namespace MintCyclingService.Remote
{
    public class RemoteService : IRemoteService
    {
        private IBaiduService baiduService = new BaiduService();

        /// <summary>
        /// 发送远程控制骑行模式
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public ResultModel SendHandleCyclingModeByLockNumber(HandleCyclingMode_PM Model)
        {
            LogHelper.Info("服务器发送骑行模式控制指令：时间：" + DateTime.Now + ", 锁编号：" + Model.LockNumber + "，骑行模式: " + Model.RunningMode);
            var result = new ResultModel();
            //调用远程骑行模式控制指令
            var sk = CommonHelper.HttpGet("http://124.15.130.103:1080/LockWcfService/HandleControllerRiding/" + Model.LockNumber + "/" + Model.RunningMode + "", "");

            var st = JObject.Parse(sk);
            result.Message = st["Message"].Value<string>();
            var isSuccess = st["SuccessSend"].Value<bool>();

            if (!isSuccess)
            {
                LogHelper.Error("调用发送远程骑行模式控制指令失败,返回的Message:" + result.Message);
                //Utility.Common.FileLog.Log("调用发送远程骑行模式控制指令失败,返回的Message:" + result.Message, "SendCyclingModeLog");
                result.IsSuccess = false;
                result.MsgCode = "0";
                result.Message = "调用发送远程骑行模式控制指令失败" + result.Message;
            }
            else
            {
                LogHelper.Info("调用发送远程骑行模式控制指令成功");
                //Utility.Common.FileLog.Log("调用发送远程骑行模式控制指令成功,返回的Message:" + result.Message, "SendCyclingModeLog");
                result.IsSuccess = true;
                result.MsgCode = "0";
                result.Message = "调用发送远程骑行模式控制指令成功";
            }
            return result;
        }

        /// <summary>
        /// 接受远程控制骑行模式
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public ResultModel ReceiveUpdateCylingModeByLockNumber(HandleCyclingMode_PM Model)
        {
            LogHelper.Info("服务器接受骑行模式控制指令：时间：" + DateTime.Now + ", 锁编号：" + Model.LockNumber + "，骑行模式: " + Model.RunningMode);
            var result = new ResultModel();
            string strLockNumber = string.Empty;

            if (string.IsNullOrEmpty(Model.LockNumber))
            {
                LogHelper.Error("锁编号为空,时间：" + DateTime.Now + ",锁编号：" + Model.LockNumber + "");
                return new ResultModel { IsSuccess = false, MsgCode = "0", Message = "锁编号为空" };
            }
            else
            {
                //strLockNumber = Model.LockNumber.Remove(0, 3);
                strLockNumber = Model.LockNumber.Trim();
            }

            using (var db = new MintBicycleDataContext())
            {
                var IsQuery = db.Reservation.FirstOrDefault(s => s.DeviceNo == strLockNumber && (s.Status == 1 || s.Status == 3));
                var lockQuery = db.BicycleLockInfo.FirstOrDefault(s => s.LockNumber == strLockNumber && (s.LockStatus == 0 || s.LockStatus == 2 || s.LockStatus == 3));

                if (IsQuery == null || lockQuery == null)
                {
                    var ConQuery = db.Bicycle_Controller_Battery_relevance.FirstOrDefault(p => p.LockNumber == strLockNumber && !p.IsDeleted);
                    if (ConQuery != null)
                    {
                        ConQuery.RunningCtrl = 2;     //受后台控制进行充放电切换运行模式
                        ConQuery.RunningStatus = int.Parse(Model.RunningMode);  //骑行模式
                        ConQuery.UpdateTime = DateTime.Now;
                        db.SubmitChanges();
                    }
                    LogHelper.Info("接受远程控制骑行模式成功：时间：" + DateTime.Now + ",锁编号：" + strLockNumber + ",骑行模式：" + Model.RunningMode);
                    //Utility.Common.FileLog.Log("接受远程控制骑行模式成功：时间：" + DateTime.Now + ",锁编号：" + Model.LockNumber + ",骑行模式：" + Model.RunningMode, "ReceiveCyclingModeLog");
                    result.ResObject = true;
                }
                else
                {
                    LogHelper.Error("接受远程控制骑行模式失败：时间：" + DateTime.Now + ",锁编号：" + strLockNumber + ",骑行模式：" + Model.RunningMode);
                    //Utility.Common.FileLog.Log("远程控制骑行模式失败：时间：" + DateTime.Now + ",锁编号：" + Model.LockNumber + ",骑行模式：" + Model.RunningMode, "ReceiveCyclingModeLog");
                    result.ResObject = false;
                }
            }
            return result;
        }

        /// <summary>
        /// 发送查询控制器与BMS序列号
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public ResultModel SendSearchControllerBMS(SearchControllerBMS_PM Model)
        {
            LogHelper.Info("发送查询控制器与BMS序列号：时间：" + DateTime.Now + ", 锁编号：" + Model.LockNumber);

            var result = new ResultModel();
            //调用远程查询控制器与BMS序列号
            var sk = CommonHelper.HttpGet("http://124.15.130.103:1080/LockWcfService/SearchControllerBMS/" + Model.LockNumber + "", "");

            var st = JObject.Parse(sk);
            result.Message = st["Message"].Value<string>();
            var isSuccess = st["SuccessSend"].Value<bool>();

            if (!isSuccess)
            {
                LogHelper.Error("调用发送查询控制器与BMS序列号失败,返回的Message:" + result.Message);
                //Utility.Common.FileLog.Log("调用发送查询控制器与BMS序列号失败,返回的Message:" + result.Message, "SendSearchControllerBMSLog");
                result.IsSuccess = false;
                result.MsgCode = "0";
                result.Message = "调用发送查询控制器与BMS序列号失败" + result.Message;
            }
            else
            {
                LogHelper.Info("调用发送查询控制器与BMS序列号成功");
                //Utility.Common.FileLog.Log("调用发送查询控制器与BMS序列号成功,返回的Message:" + result.Message, "SendSearchControllerBMSLog");
                result.IsSuccess = true;
                result.MsgCode = "0";
                result.Message = "调用发送查询控制器与BMS序列号成功";
            }
            return result;
        }

        /// <summary>
        /// 接受查询控制器与BMS序列号
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public ResultModel ReceiveSearchControllerBMS(ReceiveSearchControllerBMS_PM Model)
        {
            var result = new ResultModel();
            string strLockNumber = string.Empty;
            string BMSnumber = string.Empty;
            if (!string.IsNullOrEmpty(Model.BMSSN))
            {
                BMSnumber = Model.BMSSN.Substring(0, Model.BMSSN.Length - 3);
            }

            LogHelper.Info("接受查询控制器与BMS序列号：时间：" + DateTime.Now + ", 锁编号：" + Model.LockNumber + ",BMS序列号：" + BMSnumber + ",控制器序列号:" + Model.CtrlerSN);

            using (var db = new MintBicycleDataContext())
            {
                if (string.IsNullOrEmpty(Model.LockNumber))
                {
                    LogHelper.Error("锁编号为空，您不能查询，请重新查询：时间：" + DateTime.Now + ",锁编号：" + Model.LockNumber + "");
                    return new ResultModel { IsSuccess = false, MsgCode = "0", Message = "锁编号为空，您不能查询，请重新查询!" };
                }
                else
                {
                    //strLockNumber = Model.LockNumber.Remove(0, 3);
                    strLockNumber = Model.LockNumber.Trim();
                }

                if (Model.ResCode == 0) //接收成功
                {
                    var ConQuery = db.Bicycle_Controller_Battery_relevance.OrderByDescending(p => p.CreateTime).FirstOrDefault(p => p.LockNumber == strLockNumber && !p.IsDeleted);
                    if (ConQuery != null)
                    {
                        ConQuery.BatteryNumber = BMSnumber;             //BMS序列号
                        ConQuery.ControllerNumber = Model.CtrlerSN;      //控制器序列号
                        ConQuery.UpdateTime = DateTime.Now;
                        db.SubmitChanges();

                        LogHelper.Info("接受查询控制器与BMS序列号成功：时间：" + DateTime.Now + "Code:" + Model.ResCode + ",锁编号：" + strLockNumber + "");
                        //Utility.Common.FileLog.Log("接受查询控制器与BMS序列号成功：时间：" + DateTime.Now + ",锁编号：" + Model.LockNumber + "", "ReceiveCyclingModeLog");
                        result.IsSuccess = true;
                        result.MsgCode = "0";
                        result.Message = "接受查询控制器与BMS序列号成功";
                    }
                    else
                    {
                        //Utility.Common.FileLog.Log("没有查询到相关的信息：时间：" + DateTime.Now + ",锁编号：" + Model.LockNumber + "", "ReceiveCyclingModeLog");
                        result.IsSuccess = false;
                        result.MsgCode = "0";
                        result.Message = "没有查询到相关的信息";
                        LogHelper.Error("没有查询到相关的信息：时间：" + DateTime.Now + ",锁编号：" + strLockNumber + "");
                    }
                }
                else if (Model.ResCode == 255)
                {
                    result.IsSuccess = false;
                    result.MsgCode = Model.ResCode.ToString();
                    result.Message = "格式错误";
                    LogHelper.Error("格式错误：时间：" + DateTime.Now + ",锁编号：" + strLockNumber + "");
                }
                else if (Model.ResCode == 254)
                {
                    //return new ResultModel { IsSuccess = false, MsgCode = "0", Message = "锁编号为空，您不能查询，请重新查询!" };
                    result.IsSuccess = false;
                    result.MsgCode = Model.ResCode.ToString();
                    result.Message = "设备不存在";

                    LogHelper.Error("设备不存在：时间：" + DateTime.Now + ",锁编号：" + strLockNumber + "");
                }
                else if (Model.ResCode == 253)
                {
                    result.IsSuccess = false;
                    result.MsgCode = Model.ResCode.ToString();
                    result.Message = "加密数据验证失败";
                    LogHelper.Error("加密数据验证失败：时间：" + DateTime.Now + ",锁编号：" + strLockNumber + "");
                }
                else
                {
                    result.IsSuccess = false;
                    result.MsgCode = Model.ResCode.ToString();
                    result.Message = "数据空";
                    LogHelper.Error("数据空：时间：" + DateTime.Now + ",锁编号：" + strLockNumber + "");
                }
            }
            return result;
        }

        /// <summary>
        /// 发送远程升级锁程序包
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public ResultModel SendUpgradeBicLockProgram(RemoteUpgradeBicLockProgram_PM Model)
        {
            //获取服务器升级锁程序包的路径
            string ServerUrl = System.Web.Configuration.WebConfigurationManager.AppSettings["RemoteServerUrl"];

            var result = new ResultModel();
            string lockNumber = string.Empty;
            string version = string.Empty;
            int customerID = 0;
            string Host = "www.mintcar.com";
            string port ="8082";
            string path = string.Empty;
            string apn = "CMNET";

            using (var db = new MintBicycleDataContext())
            {
                try
                {
                    var query = db.BicycleBaseInfo.FirstOrDefault(s => s.LockGuid == Model.LockGuid && s.BicycleGuid == Model.BicycleGuid && !s.IsDeleted);
                    if (query != null)
                    {
                        lockNumber = query.LockNumber;
                    }
                    var customer = db.CustomerInfo.FirstOrDefault(s => s.CustomerID == Model.CustomerID && !s.IsDeleted);
                    if (customer != null)
                    {
                        customerID = customer.CustomerID;
                    }
                    //查询最新的升级程序包，并且未升级的
                    var BicyleLockUpgradeFile = db.BicyleLockUpgradeFile.OrderByDescending(s => s.CreateTime).FirstOrDefault(s => s.IsUpgrade == 0 && !s.IsDeleted);
                    if (BicyleLockUpgradeFile != null)
                    {
                        version = BicyleLockUpgradeFile.Version;     //版本号
                        path = BicyleLockUpgradeFile.Url;            //要升级文件存储位置
                    }

                    LogHelper.Info("发送远程升级锁程序包：时间：" + DateTime.Now + ", 锁编号：" + lockNumber + ",版本号：" + version + ",升级的文件路径：" + Host+port+path + ",客户编号：" + customerID);

                    string jsonData = "{";
                    jsonData += "\"LockNumber\":" + "\"" + lockNumber + "\",";
                    jsonData += "\"CustomerID\":" + "\"" + customerID + "\",";
                    jsonData += "\"Version\":" + "\"" + version + "\",";
                    jsonData += "\"Host\":" + "\"" + Host + "\",";
                    jsonData += "\"Port\":" + "\"" + port + "\",";
                    jsonData += "\"Path\":" + "\"" + path + "\",";
                    jsonData += "\"Apn\":" + "\"" + apn + "\" ";
                    //jsonData += "\"Url\":" + "\"" + ServerUrl + url + "\" ";
                    jsonData += "}";

                    WebClient client = new WebClient();
                    client.Headers["Content-Type"] = "application/json";
                    //string data = "{\"ID\":1,\"Name\":\"ss\"}";
                    string str = client.UploadString("http://124.15.130.103:1080/LockWcfService/UpgradeBicLockProgram", "POST", jsonData);
                    if (str.Contains("The device has not been connected."))
                    {
                        LogHelper.Error("调用发送远程升级锁程序包失败,返回的Message:The device has not been connected.");
                        result.IsSuccess = false;
                        result.MsgCode = "0";
                        result.Message = "调用发送远程升级锁程序包失败,当前锁未连接";
                    }
                    else
                    {
                        LogHelper.Info("调用发送远程升级锁程序包成功");
                        result.IsSuccess = true;
                        result.MsgCode = "0";
                        result.Message = "调用发送远程升级锁程序包成功";
                    }
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.MsgCode = "0";
                    result.Message = "查询锁相关信息异常，请稍后！";
                    LogHelper.Error("查询锁相关信息异常:" + ex.Message + ",客户编号：" + Model.CustomerID + ",车辆Guid:" + Model.BicycleGuid + ",锁的Guid：" + Model.LockGuid + "", ex);
                }
            }

            return result;
        }

        /// <summary>
        /// 接受远程升级锁程序包完成后锁的应答
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel ReceiveUpgradeFinish(ReceiveUpgradeFinish_PM Model)
        {
            Guid? LockGuid = Guid.NewGuid();
            Guid? BicycleGuid = Guid.NewGuid();

            LogHelper.Info("接受远程升级锁程序包完成后锁的应答：时间：" + DateTime.Now + ", 锁编号：" + Model.LockNumber + "，升级状态: " + Model.Status);
            var result = new ResultModel();
            string strLockNumber = string.Empty;

            if (string.IsNullOrEmpty(Model.LockNumber))
            {
                LogHelper.Error("锁编号为空,时间：" + DateTime.Now + ",锁编号：" + Model.LockNumber + "");
                return new ResultModel { IsSuccess = false, MsgCode = "0", Message = "锁编号为空" };
            }
            else
            {
                strLockNumber = Model.LockNumber.Trim();
            }

            using (var db = new MintBicycleDataContext())
            {
                try
                {
                    var queryBicBase = db.BicycleBaseInfo.FirstOrDefault(p => p.LockNumber == Model.LockNumber && !p.IsDeleted);
                    if (queryBicBase != null && queryBicBase.BicycleNumber != null)
                    {
                        LockGuid = queryBicBase.LockGuid;
                        BicycleGuid = queryBicBase.BicycleGuid;
                    }
                    else
                    {
                        LogHelper.Info("没有此单车信息，请重新选择升级：时间：" + DateTime.Now + ",锁编号：" + strLockNumber + ",升级状态：" + Model.Status);
                        return new ResultModel { IsSuccess = false, MsgCode = "0", Message = "没有此单车信息，请重新选择升级" };
                    }

                    var Upgrade = new BicycleLockRelevanceUpgrade
                    {
                        UpgradeGuid = Guid.NewGuid(),
                        LockGuid = LockGuid,
                        BicycleGuid = BicycleGuid,
                        CustomerID = Model.CustomerID,
                        Status = Model.Status,    //表示升级成功
                        CreateTime = DateTime.Now,
                        IsDeleted = false
                    };
                    db.BicycleLockRelevanceUpgrade.InsertOnSubmit(Upgrade);
                    db.SubmitChanges();

                    LogHelper.Info("接受远程升级锁程序包完成后锁的应答成功：时间：" + DateTime.Now + ",锁编号：" + strLockNumber + ",升级状态：" + Model.Status);
                    result.ResObject = true;
                }
                catch (Exception ex)
                {
                    LogHelper.Error("接受远程升级锁程序包完成后锁的应答异常:" + ex.Message + ",时间：" + DateTime.Now + ",锁编号：" + strLockNumber + ",升级状态：" + Model.Status);
                    result.ResObject = false;
                }
            }
            return result;
        }

        /// <summary>
        /// 控制器上报信息
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public ResultModel HardWareControllerReport(ControllerReport_PM Model)
        {
            LogHelper.Info("控制器上报信息：时间：" + DateTime.Now + ", 锁编号：" + Model.LockNumber + "，Type: " + Model.Type);
            var result = new ResultModel();
            string strDeviceNo = string.Empty;

            if (!string.IsNullOrEmpty(Model.LockNumber))
            {
                //Utility.Common.FileLog.Log("1111111：" + Model.LockNumber + "", "ControllerReportLog");

                //strDeviceNo = Model.LockNumber.Remove(0, 3);
                strDeviceNo = Model.LockNumber.Trim();

                using (var db = new MintBicycleDataContext())
                {
                    //锁基本信息
                    var query = db.BicycleLockInfo.FirstOrDefault(s => s.LockNumber == strDeviceNo && !s.IsDeleted);
                    if (query != null)
                    {
                        var contrQuery = db.Bicycle_Controller_Battery_relevance.FirstOrDefault(s => s.LockNumber == strDeviceNo && !s.IsDeleted);
                        if (contrQuery != null)
                        {
                            #region 修改

                            //控制器上报信息：时间：2017 / 6 / 12 13:38:04,锁编号：D43639A5EBC4,Type01111111：D43639A5EBC42222222：639A5EBC43333333：D43639A5EBC4 639A5EBC4
                            try
                            {
                                contrQuery.LockNumber = strDeviceNo;
                                contrQuery.Type = Model.Type;
                                contrQuery.Date = Model.Date;
                                contrQuery.Vol1 = Model.Vol1;
                                contrQuery.Cur1 = Model.Cur1;
                                contrQuery.Speed = Model.Speed;   //当前整车速度, 单位: 0.1km/h
                                contrQuery.RPM = Model.RPM;
                                //故障代码 0x00:无故障(0); 0x21:电流异常(33); 0x24:电机霍尔异常(36); 0x25:刹车故障(37); 0x26: 电池欠压(38)；0x27: I2C通讯故障(39)
                                contrQuery.ErrCode = Model.ErrCode;
                                contrQuery.CalibrateStatus = Model.CalibrateStatus;  //0: 无校准 1: 校准成功
                                contrQuery.RunningCtrl = Model.RunningCtrl;        //1: 不受后台控制，即自行充放电运行模式; 2: 受后台控制进行充放电切换运行模式
                                contrQuery.RunningStatus = Model.RunningStatus;   //0: 普通模式，既不助力也不充电1: 助力模式2: 欠压充电模式3: 超速充电模式
                                contrQuery.Kms = Model.Kms;   //本次骑行公里数，单位公里
                                //contrQuery.Status = Model.Status;
                                //contrQuery.Temp = Model.Temp;
                                //contrQuery.Electricity = Model.Electricity;
                                //contrQuery.ChargeDischargeCounts = Model.ChargeDischargeCounts;
                                //contrQuery.MaxChargeInterval = Model.MaxChargeInterval;
                                //contrQuery.CurChargeInterval = Model.CurChargeInterval;
                                //contrQuery.Capacity = Model.Capacity;
                                //contrQuery.Elec = Model.Elec;
                                //contrQuery.Vol2 = Model.Vol2;
                                //contrQuery.Cur2 = Model.Cur2;
                                contrQuery.UpdateTime = DateTime.Now;
                                db.SubmitChanges();
                            }
                            catch (Exception ex)
                            {
                                LogHelper.Error("修改控制器上报信息异常：锁编号：" + Model.LockNumber + ex.Message);
                                //Utility.Common.FileLog.Log("修改控制器上报信息异常：锁编号：" + Model.LockNumber + ex.Message, "修改控制器上报信息Log");
                                return new ResultModel { IsSuccess = false, MsgCode = "0", Message = "修改控制器上报信息异常" };
                            }
                            result.IsSuccess = true;
                            result.MsgCode = "0";
                            result.Message = "修改控制器上报信息成功";
                            result.ResObject = null;
                            //Utility.Common.FileLog.Log("6666666：" + result.Message + "", "ControllerReportLog");

                            #endregion 修改
                        }
                        else
                        {
                            #region 添加

                            try
                            {
                                var contr = new Bicycle_Controller_Battery_relevance
                                {
                                    ControllerBatteryGuid = Guid.NewGuid(),
                                    LockNumber = strDeviceNo,
                                    Type = Model.Type,
                                    Date = Model.Date,
                                    Vol1 = Model.Vol1,
                                    Cur1 = Model.Cur1,
                                    Speed = Model.Speed,
                                    RPM = Model.RPM,
                                    ErrCode = Model.ErrCode,
                                    CalibrateStatus = Model.CalibrateStatus,  //0: 无校准 1: 校准成功
                                    RunningCtrl = Model.RunningCtrl,        //1: 不受后台控制，即自行充放电运行模式; 2: 受后台控制进行充放电切换运行模式
                                    RunningStatus = Model.RunningStatus,   //0: 普通模式，既不助力也不充电1: 助力模式2: 欠压充电模式3: 超速充电模式
                                    Kms = Model.Kms,   //本次骑行公里数，单位公里
                                                       //Status = Model.Status,
                                                       //Temp = Model.Temp,
                                                       //Electricity = Model.Electricity,
                                                       //ChargeDischargeCounts = Model.ChargeDischargeCounts,
                                                       //MaxChargeInterval = Model.MaxChargeInterval,
                                                       //CurChargeInterval = Model.CurChargeInterval,
                                                       //Capacity = Model.Capacity,
                                                       //Elec = Model.Elec,
                                                       //Vol2 = Model.Vol2,
                                                       //Cur2 = Model.Cur2,
                                    CreateTime = DateTime.Now,
                                    IsDeleted = false
                                };
                                db.Bicycle_Controller_Battery_relevance.InsertOnSubmit(contr);
                                db.SubmitChanges();
                            }
                            catch (Exception ex)
                            {
                                LogHelper.Error("添加控制器上报信息异常：锁编号：" + Model.LockNumber + ex.Message);
                                //Utility.Common.FileLog.Log("添加控制器上报信息异常：设备编号：" + Model.LockNumber + ex.Message, "添加控制器上报信息Log");
                                return new ResultModel { IsSuccess = false, MsgCode = "0", Message = "添加控制器上报信息异常" };
                            }
                            result.IsSuccess = true;
                            result.MsgCode = "0";
                            result.Message = "添加控制器上报信息成功";
                            result.ResObject = null;

                            #endregion 添加
                        }
                        //query.BicyCleTypeName = Model.Status; //  控制器运行模式 0x01:助力模式(1); 0x02:欠压充电(2); 0x03:超速充电(3)
                        db.SubmitChanges();
                    }
                    else
                    {
                        LogHelper.Error("添加控制器上报信息失败：锁编号：" + Model.LockNumber + ",不存在此锁");
                        //Utility.Common.FileLog.Log("添加控制器上报信息失败：锁编号：" + strDeviceNo + ",不存在此锁", "ControllerReportLog");
                        result.IsSuccess = false;
                        result.MsgCode = "0";
                        result.Message = "不存在此锁";
                        result.ResObject = null;
                    }
                }
            }
            else
            {
                LogHelper.Error("添加控制器上报信息失败：上报的锁编号为空");
                //Utility.Common.FileLog.Log("添加控制器上报信息失败：上报的锁编号为空", "ControllerReportLog");
                result.IsSuccess = false;
                result.MsgCode = "0";
                result.Message = "添加控制器上报信息失败,上报的锁编号为空";
                result.ResObject = null;
            }
            return result;
        }

        /// <summary>
        /// BMS上报信息
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public ResultModel HardWareBMSReport(BMSReport_PM Model)
        {
            LogHelper.Info("BMS上报信息：时间：" + DateTime.Now + ", 锁编号：" + Model.LockNumber + "，Type: " + Model.Type);
            var result = new ResultModel();
            string strLockNumber = string.Empty;

            if (!string.IsNullOrEmpty(Model.LockNumber))
            {
                //strLockNumber = Model.LockNumber.Remove(0, 3);
                strLockNumber = Model.LockNumber.Trim();

                using (var db = new MintBicycleDataContext())
                {
                    //锁基本信息
                    var query = db.BicycleLockInfo.FirstOrDefault(s => s.LockNumber == strLockNumber && !s.IsDeleted);
                    if (query != null)
                    {
                        var contrQuery = db.Bicycle_Controller_Battery_relevance.FirstOrDefault(s => s.LockNumber == strLockNumber && !s.IsDeleted);
                        if (contrQuery != null)
                        {
                            #region 修改

                            try
                            {
                                contrQuery.LockNumber = strLockNumber;
                                contrQuery.Type = Model.Type;
                                contrQuery.Date = Model.Date;
                                contrQuery.Temp = Model.Temp;
                                contrQuery.Electricity = Model.Electricity;
                                contrQuery.ChargeDischargeCounts = Model.ChargeDischargeCounts;
                                contrQuery.MaxChargeInterval = Model.MaxChargeInterval;
                                contrQuery.CurChargeInterval = Model.CurChargeInterval;
                                contrQuery.Capacity = Model.Capacity;
                                contrQuery.Elec = Model.Elec;
                                contrQuery.Vol2 = Model.Vol2;
                                contrQuery.Cur2 = Model.Cur2;
                                contrQuery.UpdateTime = DateTime.Now;
                                db.SubmitChanges();
                            }
                            catch (Exception ex)
                            {
                                LogHelper.Error("修改BMS上报信息异常：锁编号：" + Model.LockNumber + ex.Message, ex);
                                //Utility.Common.FileLog.Log("修改BMS上报信息异常：锁编号：" + Model.LockNumber + ex.Message, "修改BMS上报信息Log");
                                return new ResultModel { IsSuccess = false, MsgCode = "0", Message = "修改BMS上报信息异常" };
                            }
                            result.IsSuccess = true;
                            result.MsgCode = "0";
                            result.Message = "修改BMS上报信息成功";
                            result.ResObject = null;

                            #endregion 修改
                        }
                        else
                        {
                            #region 添加

                            try
                            {
                                var contr = new Bicycle_Controller_Battery_relevance
                                {
                                    ControllerBatteryGuid = Guid.NewGuid(),
                                    LockNumber = strLockNumber,
                                    Type = Model.Type,
                                    Date = Model.Date,
                                    Temp = Model.Temp,
                                    Electricity = Model.Electricity,
                                    ChargeDischargeCounts = Model.ChargeDischargeCounts,
                                    MaxChargeInterval = Model.MaxChargeInterval,
                                    CurChargeInterval = Model.CurChargeInterval,
                                    Capacity = Model.Capacity,
                                    Elec = Model.Elec,
                                    Vol2 = Model.Vol2,
                                    Cur2 = Model.Cur2,
                                    CreateTime = DateTime.Now,
                                    IsDeleted = false
                                };
                                db.Bicycle_Controller_Battery_relevance.InsertOnSubmit(contr);
                                db.SubmitChanges();
                            }
                            catch (Exception ex)
                            {
                                LogHelper.Error("添加BMS上报信息异常：设备编号：" + Model.LockNumber + ex.Message, ex);
                                Utility.Common.FileLog.Log("添加BMS上报信息异常：设备编号：" + Model.LockNumber + ex.Message, "添加BMS上报信息Log");
                                return new ResultModel { IsSuccess = false, MsgCode = "0", Message = "添加BMS上报信息异常" };
                            }
                            result.IsSuccess = true;
                            result.MsgCode = "0";
                            result.Message = "添加BMS上报信息成功";
                            result.ResObject = null;

                            #endregion 添加
                        }
                        db.SubmitChanges();
                    }
                    else
                    {
                        LogHelper.Error("BMS上报信息失败：锁编号：" + strLockNumber + ",不存在此锁");
                        //Utility.Common.FileLog.Log("BMS上报信息失败：锁编号：" + strDeviceNo + ",不存在此锁", "控BMS上报日志");
                    }
                }
            }
            else
            {
                LogHelper.Error("BMS上报信息失败,上报的锁编号为空");
                //Utility.Common.FileLog.Log("BMS上报信息失败：上报的锁编号为空", "BMS上报日志");
            }

            return result;
        }

        /// <summary>
        /// 锁自动上报信息
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public ResultModel HardWareAutoUpadate(HardWareAuto_PM Model)
        {
            LogHelper.Info("12小时锁自动上传数据：时间：" + DateTime.Now + ", 锁编号：" + Model.DeviceNo);
            var result = new ResultModel();
            string strDeviceNo = string.Empty;
            if (!string.IsNullOrEmpty(Model.DeviceNo))
            {
                strDeviceNo = Model.DeviceNo.Remove(0, 3);

                using (var db = new MintBicycleDataContext())
                {
                    #region 修改

                    try
                    {
                        var bicycleQuery = db.BicycleLockInfo.FirstOrDefault(x => x.LockNumber == strDeviceNo && !x.IsDeleted);
                        if (bicycleQuery != null)
                        {
                            //if (Model.Voltage / 100 < Convert.ToDecimal(3.3))
                            //{
                            //    bicycleQuery.ElectricQuantityStatus = 0;   //电量不足
                            //}
                            //else
                            //{
                            //    bicycleQuery.ElectricQuantityStatus = 1;   //电量充足
                            //}
                            bicycleQuery.Voltage = Model.Voltage / 100;
                            bicycleQuery.Longitude = Model.Longitude != null ? Model.Longitude : bicycleQuery.Longitude;
                            bicycleQuery.Latitude = Model.Latitude != null ? Model.Latitude : bicycleQuery.Latitude;
                            bicycleQuery.UpdateTime = DateTime.Parse(Model.Timestamp);

                            #region 调用百度API--更新车辆基本信息表

                            try
                            {
                                string provinceName = string.Empty;
                                string cityName = string.Empty;
                                string districtName = string.Empty;
                                string Address = string.Empty;

                                int ProvinceID = 0;
                                int cityID = 0;
                                int districtID = 0;

                                Address = baiduService.GetAddress(Model.Longitude.ToString(), Model.Latitude.ToString()); //详细地址
                                List<AddressModel> list = baiduService.GetAddressInfo(Model.Longitude.ToString(), Model.Latitude.ToString());
                                if (list.Count > 0)
                                {
                                    provinceName = list[0].provinceName;
                                    cityName = list[0].city;
                                    districtName = list[0].district;

                                    string sqlStr = "select p.Id as ProvinceID,c.Id as cityID,d.Id as districtID from Province as p inner join City as c on p.Id =c.ProvinceId inner join   District as d on c.Id = d.CityId where p.Name like'%" + provinceName + "%' and c.Name like '%" + cityName + "%' and d.Name  like '%" + districtName + "%'";
                                    var queryArea = db.ExecuteQuery<AddressIDList>(sqlStr);
                                    if (queryArea != null)
                                    {
                                        foreach (var item in queryArea.ToList())
                                        {
                                            ProvinceID = item.ProvinceID;
                                            cityID = item.cityID;
                                            districtID = item.districtID;
                                        }
                                    }
                                    //通过开锁的时候，修改车辆基本信息表中的省市区字段值
                                    bicycleQuery.ProvinceID = ProvinceID;
                                    bicycleQuery.CityID = cityID;
                                    bicycleQuery.DistrictID = districtID;
                                }
                            }
                            catch (Exception ex)
                            {
                                //Utility.Common.FileLog.Log("调用百度API--更新车辆基本信息异常：" + ex.Message, "自动上传日志");
                                LogHelper.Error("自动上传日志:调用百度API--更新车辆基本信息异常" + ex.Message + ",锁的编号：" + Model.DeviceNo, ex);
                            }
                            //Utility.Common.FileLog.Log("PushID：" + usersQuery.PushId + "用户Guid"+para.TypeName+ ","+para.LockAction + usersQuery.UserInfoGuid, "JPushLog");

                            #endregion 调用百度API--更新车辆基本信息表

                            db.SubmitChanges();

                            result.IsSuccess = true;
                            result.MsgCode = "0";
                            result.Message = "自动上传车锁信息成功";
                            result.ResObject = null;
                        }
                        else
                        {
                            LogHelper.Error("自动上传日志:自动上传车锁信息失败：暂无车锁信息");
                            //Utility.Common.FileLog.Log("自动上传车锁信息失败：暂无车锁信息", "自动上传日志");
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Error("自动上传日志:自动上传车锁信息异常,锁的编号：" + Model.DeviceNo);
                        //Utility.Common.FileLog.Log("自动上传车锁信息异常：设备编号：" + Model.DeviceNo, "自动上传日志");
                        return new ResultModel { IsSuccess = false, MsgCode = "0", Message = "自动上传车锁信息异常" };
                    }

                    #endregion 修改
                }
            }
            else
            {
                //Utility.Common.FileLog.Log("自动上传车锁信息失败：上传的设备编号为空", "自动上传日志");
                LogHelper.Error("自动上传日志:自动上传车锁信息失败,上传的设备编号为空");
            }

            return result;
        }

        /// <summary>
        /// 控制器和电池组上报信息
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public ResultModel HardWareHelpUpadate(ComponentsBinding Model)
        {
            LogHelper.Info("控制器和电池组硬件锁上传信息：时间：" + DateTime.Now + ", 锁编号：" + Model.LockNumber);
            var result = new ResultModel();

            try
            {
                if (!string.IsNullOrEmpty(Model.LockNumber))
                {
                    using (var db = new MintBicycleDataContext())
                    {
                        //车辆基本信息
                        var query = db.BicycleLockInfo.FirstOrDefault(s => s.LockNumber == Model.LockNumber && !s.IsDeleted);
                        if (query != null)
                        {
                            var contrQuery = db.Bicycle_Controller_Battery_relevance.FirstOrDefault(s => s.BicyCleNumber == Model.BikeNumber && s.LockNumber == Model.LockNumber && !s.IsDeleted);
                            if (contrQuery != null)
                            {
                                #region 修改

                                try
                                {
                                    contrQuery.ControllerNumber = Model.ControllerNumber;
                                    contrQuery.BicyCleNumber = Model.BikeNumber;
                                    contrQuery.BatteryNumber = Model.BatteryNumber;
                                    contrQuery.LockNumber = Model.LockNumber;  //锁编号
                                    contrQuery.Type = Model.Type;
                                    contrQuery.Date = Model.Date;
                                    contrQuery.Vol1 = Model.Vol1;
                                    contrQuery.Cur1 = Model.Cur1;
                                    contrQuery.Speed = Model.Speed;
                                    contrQuery.RPM = Model.RPM;
                                    contrQuery.ErrCode = Model.ErrCode;
                                    contrQuery.CalibrateStatus = Model.CalibrateStatus;   //  0: 无校准 1: 校准成功
                                    contrQuery.RunningCtrl = Model.RunningCtrl;         //1: 不受后台控制，即自行充放电运行模式2: 受后台控制进行充放电切换运行模式
                                    contrQuery.RunningStatus = Model.RunningStatus;      //0: 普通模式，既不助力也不充电1: 助力模式2: 欠压充电模式3: 超速充电模式
                                    contrQuery.Kms = Model.Kms;                         // 本次骑行公里数，单位公里

                                    contrQuery.Temp = Model.Temp;
                                    contrQuery.Electricity = Model.Electricity;
                                    contrQuery.ChargeDischargeCounts = Model.ChargeDischargeCounts;
                                    contrQuery.MaxChargeInterval = Model.MaxChargeInterval;
                                    contrQuery.CurChargeInterval = Model.CurChargeInterval;
                                    contrQuery.Capacity = Model.Capacity;
                                    contrQuery.Elec = Model.Elec;
                                    contrQuery.Vol2 = Model.Vol2;
                                    contrQuery.Cur2 = Model.Cur2;
                                    contrQuery.UpdateTime = DateTime.Now;
                                    contrQuery.IsDeleted = false;

                                    db.SubmitChanges();
                                }
                                catch (Exception ex)
                                {
                                    LogHelper.Error("控制器和电池组上报日志:修改控控制器和电池组上报信息异常" + ex.Message + ",锁编号：" + Model.LockNumber, ex);
                                    //Utility.Common.FileLog.Log("修改控控制器和电池组上报信息异常：设备编号：" + Model.LockNumber + ex.Message, "控制器和电池组上报日志");
                                    return new ResultModel { IsSuccess = false, MsgCode = "0", Message = "修改控控制器和电池组上报信息异常" };
                                }
                                result.IsSuccess = true;
                                result.MsgCode = "0";
                                result.Message = "修改控制器和电池组上报信息成功";
                                result.ResObject = null;

                                #endregion 修改
                            }
                            else
                            {
                                #region 添加

                                try
                                {
                                    var contr = new Bicycle_Controller_Battery_relevance
                                    {
                                        BatteryNumber = Model.BatteryNumber,
                                        BicyCleNumber = Model.BikeNumber,
                                        ControllerNumber = Model.ControllerNumber,
                                        ControllerBatteryGuid = Guid.NewGuid(),
                                        LockNumber = Model.LockNumber,

                                        #region 控制器参数

                                        Type = Model.Type,
                                        Date = Model.Date,
                                        Bms_Type = Model.Bms_Type,
                                        Bms_Date = DateTime.Now,
                                        Vol1 = Model.Vol1,
                                        Cur1 = Model.Cur1,
                                        Speed = Model.Speed,
                                        RPM = Model.RPM,
                                        ErrCode = Model.ErrCode,
                                        CalibrateStatus = Model.CalibrateStatus,   //  0: 无校准 1: 校准成功
                                        RunningCtrl = Model.RunningCtrl,          //1: 不受后台控制，即自行充放电运行模式2: 受后台控制进行充放电切换运行模式
                                        RunningStatus = Model.RunningStatus,      //0: 普通模式，既不助力也不充电1: 助力模式2: 欠压充电模式3: 超速充电模式
                                        Kms = Model.Kms,                         // 本次骑行公里数，单位公里

                                        #endregion 控制器参数

                                        #region BMS电池组信息

                                        Temp = Model.Temp,
                                        Electricity = Model.Electricity,
                                        ChargeDischargeCounts = Model.ChargeDischargeCounts,
                                        MaxChargeInterval = Model.MaxChargeInterval,
                                        CurChargeInterval = Model.CurChargeInterval,
                                        Capacity = Model.Capacity,
                                        Elec = Model.Elec,
                                        Vol2 = Model.Vol2,
                                        Cur2 = Model.Cur2,

                                        #endregion BMS电池组信息

                                        CreateTime = DateTime.Now,
                                        IsDeleted = false
                                    };
                                    db.Bicycle_Controller_Battery_relevance.InsertOnSubmit(contr);
                                    db.SubmitChanges();
                                }
                                catch (Exception ex)
                                {
                                    LogHelper.Error("控制器和电池组上报日志:添加控制器和电池组上报信息异常" + ex.Message + ",锁编号：" + Model.LockNumber, ex);
                                    // Utility.Common.FileLog.Log("添加控制器和电池组上报信息异常：设备编号：" + Model.LockNumber, "控制器和电池组上报日志");
                                    return new ResultModel { IsSuccess = false, MsgCode = "0", Message = "添加控制器和电池组上报信息异常" };
                                }
                                result.IsSuccess = true;
                                result.MsgCode = "0";
                                result.Message = "添加控制器和电池组上报信息成功";
                                result.ResObject = null;

                                #endregion 添加
                            }
                            db.SubmitChanges();
                        }
                        else
                        {
                            LogHelper.Error("控制器和电池组上报日志:控制器和电池组上报信息失败,锁编号：" + Model.LockNumber + ",不存在此单车");
                            //Utility.Common.FileLog.Log("控制器和电池组上报信息失败：锁编号：" + Model.LockNumber + ",不存在此单车", "控制器和电池组上报日志");
                        }
                    }
                }
                else
                {
                    LogHelper.Error("控制器和电池组上报日志:控制器和电池组上报信息失败,上传的锁编号为空");
                    //Utility.Common.FileLog.Log("控制器和电池组上报信息失败：上传的锁编号为空", "控制器和电池组上报日志");
                }
            }
            catch (Exception ex)
            {
                //Utility.Common.FileLog.Log(ex.Message, "BicycleHandleService");
                LogHelper.Error("控制器和电池组上报日志:控制器和电池组上报信息异常" + ex.Message, ex);
                //return new Utils.ResultModel { IsSuccess = false, Message = "控制器和电池组上报信息异常" };
            }
            return result;
        }

        //public  void OnPost(HttpRequest request)
        //{
        //    //获取客户端传递的参数
        //    int num1 = int.Parse(request.Params["num1"]);
        //    int num2 = int.Parse(request.Params["num2"]);

        //    //设置返回信息
        //    string content = string.Format("这是通过Post方式返回的数据:num1={0},num2={1}", num1, num2);

        //    //构造响应报文
        //    HttpResponse response = new HttpResponse(content, Encoding.UTF8);
        //    response.StatusCode = 200;
        //    response.ContentType = "text/html; charset=UTF-8";

        //    //发送响应
        //    ProcessResponse(request.Handler, response);
        //}

        //public  void OnGet(HttpRequest request)
        //{
        //    //获取客户端传递的参数
        //    int num1 = int.Parse(request.Params["num1"]);
        //    int num2 = int.Parse(request.Params["num2"]);

        //    //设置返回信息
        //    string content = string.Format("这是通过Get方式返回的数据:num1={0},num2={1}", num1, num2);

        //    //构造响应报文
        //    HttpResponse response = new HttpResponse(content, Encoding.UTF8);
        //    response.StatusCode = 200;
        //    response.ContentType = "text/html; charset=UTF-8";

        //    //发送响应
        //    ProcessResponse(request.Handler, response);
        //}
    }
}