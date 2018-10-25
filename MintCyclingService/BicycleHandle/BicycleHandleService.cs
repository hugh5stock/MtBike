using MintCyclingData;
using MintCyclingService.Common;
using MintCyclingService.Cycling;
using MintCyclingService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility.LogHelper;

namespace MintCyclingService.BicycleHandle
{
    public class BicycleHandleService : IBicycleHandleService
    {
        private ICyclingService _cycService = new CyclingService();

        /// <summary>
        /// 单车入库
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public ResultModel AddBicycleBaseInfoRK(AddBicycleBase_PM model)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                #region 添加

                var queryB = db.BicycleBaseInfo.FirstOrDefault(s => s.BicycleNumber == model.BicycleNumber && !s.IsDeleted);
                if (queryB != null && queryB.BicycleNumber == model.BicycleNumber)
                {
                    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.BicycleYNotExist, Message = ResPrompt.BicycleYNotExistMessage };
                }
                else
                {
                    var Bicycle = new BicycleBaseInfo
                    {
                        BicycleGuid = Guid.NewGuid(),
                        BicycleNumber = model.BicycleNumber,
                        Status = 1, //默认正常
                        CreateBy = model.ServicePersonID,
                        UpdateTime = DateTime.Now,
                        CreateTime = DateTime.Now,
                        IsDeleted = false
                    };
                    db.BicycleBaseInfo.InsertOnSubmit(Bicycle);
                    db.SubmitChanges();
                }

                result.Message = "单车入库成功";

                #endregion 添加
            }
            return result;
        }

        /// <summary>
        /// 根据车牌号获取锁编号
        /// </summary>
        /// <param name="BikeNumber"></param>
        /// <returns></returns>
        public ResultModel GetLockNumber(string BikeNumber)
        {
            using (var db = new MintBicycleDataContext())
            {
                try
                {
                    var query = from x in db.BicycleBaseInfo
                                where x.BicycleNumber == BikeNumber && !x.IsDeleted
                                select x;

                    if (query.Any())
                    {
                        var fir = query.FirstOrDefault();
                        return new ResultModel { ResObject = fir.LockNumber };  //锁的编号
                    }
                    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.BicycleNotExist, Message = ResPrompt.BicycleNotExistMessage };
                }
                catch (Exception ex)
                {
                    LogHelper.Error("根据车牌号获取锁编号:" + ex.Message + "车辆编号：" + BikeNumber, ex);
                    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.BicycleNotExist, Message = ResPrompt.BicycleNotExistMessage };
                }
            }
        }

        /// <summary>
        /// 车锁入库
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ResultModel HelpComponentsBinding(ComponentsBinding data)
        {
            using (var db = new MintBicycleDataContext())
            {
                try
                {
                    var date = DateTime.Now;

                    var mat = from x in db.BicycleBaseInfo
                              where x.BicycleNumber == data.BikeNumber && x.LockNumber == data.LockNumber && x.BicyCleTypeName == (int)data.BicycleType
                              select x;
                    if (mat.Any())
                    {
                        return new ResultModel { IsSuccess = true, Message = "已进行匹配，不需要重复操作" };
                    }

                    #region  车辆表
                    var query = from x in db.BicycleBaseInfo
                                where x.BicycleNumber == data.BikeNumber && !x.IsDeleted
                                select x;

                    if (!query.Any())
                    {
                        var bike = new BicycleBaseInfo();
                        bike.BicycleGuid = Guid.NewGuid();
                        bike.BicycleNumber = data.BikeNumber;
                        bike.BicyCleTypeName = (int)data.BicycleType;
                        bike.CreateBy = data.ServicePersonGuid;
                        bike.CreateTime = date;
                        bike.IsDeleted = false;
                        db.BicycleBaseInfo.InsertOnSubmit(bike);
                        db.SubmitChanges();
                    }
                    else
                    {
                        var queryBic = query.FirstOrDefault();
                        queryBic.BicycleNumber = data.BikeNumber;
                        queryBic.BicyCleTypeName = (int)data.BicycleType;
                        queryBic.UpdateBy = data.ServicePersonGuid;
                        queryBic.UpdateTime = date;
                        db.SubmitChanges();
                    }
                    #endregion

                    var lockQ = from x in db.BicycleLockInfo
                                where x.LockNumber == data.LockNumber && !x.IsDeleted
                                select x;
                    if (!lockQ.Any())
                    {
                        return new ResultModel { IsSuccess = false, Message = "车锁还没有入库，请联系硬件厂家入库" };
                    }

                    if (data.BicycleType == (int)BicycleType.Powermode) //助力车
                    {
                        //添加或者修改控制器或电池组
                        var res = HardWareHelpUpadate(data);

                        if (res.IsSuccess == false)
                        {
                            var Recordbreak = new BicycleLockMatchRecord
                            {
                                BatteryNumber = data.BatteryNumber,
                                BicycleNumber = data.BikeNumber,
                                ControllerNumber = data.ControllerNumber,
                                LockNumber = data.LockNumber,
                                CreateBy = data.ServicePersonGuid,
                                CreateTime = date,
                                Guid = Guid.NewGuid(),
                                IsDeleted = false,
                                Status = 0,
                                Remark = "控制器或电池组上传信息失败"
                            };
                            db.BicycleLockMatchRecord.InsertOnSubmit(Recordbreak);
                            return new ResultModel { IsSuccess = false, Message = "控制器或电池组上传信息失败！" };
                        }

                        //车辆和锁匹配记录表
                        var recQ = from x in db.BicycleLockMatchRecord
                                   where x.LockNumber == data.LockNumber && x.BicycleNumber == data.BikeNumber
                                   select x;
                        if (recQ.Any())
                        {
                            //修改
                            var firRe = recQ.FirstOrDefault();
                            firRe.UpdateTime = date;
                            firRe.UpdateBy = data.ServicePersonGuid;
                            firRe.Status = 1;   //匹配成功
                        }
                        else //添加
                        {
                            var record = new BicycleLockMatchRecord();
                            record.Guid = Guid.NewGuid();
                            record.IsDeleted = false;
                            record.LockNumber = data.LockNumber;
                            record.BicycleNumber = data.BikeNumber;
                            record.BatteryNumber = data.BatteryNumber;
                            record.ControllerNumber = data.ControllerNumber;
                            record.CreateTime = date;
                            record.CreateBy = data.ServicePersonGuid;
                            record.Status = 1;
                            db.BicycleLockMatchRecord.InsertOnSubmit(record);
                        }
                    }
                    else
                    {
                        #region 车锁匹配记录表
                        var recQ = from x in db.BicycleLockMatchRecord
                                   where x.LockNumber == data.LockNumber && x.BicycleNumber == data.BikeNumber
                                   select x;
                        if (recQ.Any())
                        {
                            //修改
                            var firRe = recQ.FirstOrDefault();
                            firRe.UpdateTime = date;
                            firRe.UpdateBy = data.ServicePersonGuid;
                            firRe.Status = 1;   //匹配成功
                        }
                        else //添加
                        {
                            var record = new BicycleLockMatchRecord();
                            record.Guid = Guid.NewGuid();
                            record.IsDeleted = false;
                            record.LockNumber = data.LockNumber;
                            record.BicycleNumber = data.BikeNumber;
                            record.CreateTime = date;
                            record.CreateBy = data.ServicePersonGuid;
                            record.Status = 1;
                            db.BicycleLockMatchRecord.InsertOnSubmit(record);
                        }

                        #endregion
                    }

                    //修改车辆表和锁表
                    var firB = query.FirstOrDefault();
                    var firQ = lockQ.FirstOrDefault();
                    firB.BicyCleTypeName = data.BicycleType;
                    firB.LockGuid = firQ.LockGuid;
                    firB.LockNumber = firQ.LockNumber;
                    firB.UpdateTime = date;

                    firQ.UpdateTime = date;
                    db.SubmitChanges();


                    //如果换了一辆新车，把原来的匹配的锁赋值空值
                    var bicqueyr = from x in db.BicycleBaseInfo
                                   where x.LockNumber == data.LockNumber && x.BicycleNumber != data.BikeNumber && !x.IsDeleted
                                   select x;
                    if (bicqueyr.Any())
                    {
                        foreach (var item in bicqueyr)
                        {
                            item.LockNumber = null;
                            item.LockGuid = null;
                            item.UpdateTime = date;
                            item.UpdateBy = data.ServicePersonGuid;
                        }
                        db.SubmitChanges();
                    }
                    return new ResultModel { IsSuccess = true, MsgCode = "0", Message = "车锁入库和匹配信息成功" };
                }
                catch (Exception ex)
                {
                    //LogHelper.Error(this.GetType().ToString(), ex.ToString());
                    LogHelper.Error(ex.Message, ex);
                    return new Utils.ResultModel { IsSuccess = false, Message = "车锁入库异常，稍后重试" };
                }
            }
        }

        /// <summary>
        /// 控制器和电池组上报信息
        /// </summary>
        /// <param name="Model"></param>
        /// <returns></returns>
        public ResultModel HardWareHelpUpadate(ComponentsBinding Model)
        {
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
                                    //Utility.Common.FileLog.Log("修改控控制器和电池组上报信息异常：设备编号：" + Model.LockNumber + ex.Message, "控制器和电池组上报日志");
                                    LogHelper.Error("修改控控制器和电池组上报信息异常:" + ex.Message + "设备编号：" + Model.LockNumber, ex);
                                    //LogHelper.Error(ex.Message, ex);
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
                                        #endregion

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

                                        #endregion

                                        CreateTime = DateTime.Now,
                                        IsDeleted = false
                                    };
                                    db.Bicycle_Controller_Battery_relevance.InsertOnSubmit(contr);
                                    db.SubmitChanges();
                                }
                                catch (Exception ex)
                                {
                                    //Utility.Common.FileLog.Log("添加控制器和电池组上报信息异常：设备编号：" + Model.LockNumber, "控制器和电池组上报日志");
                                    LogHelper.Error("添加控制器和电池组上报信息异常:" + ex.Message + "设备编号：" + Model.LockNumber, ex);
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
                            LogHelper.Error("控制器和电池组上报信息失败:设备编号：" + Model.LockNumber + ",不存在此单车");
                            //Utility.Common.FileLog.Log("控制器和电池组上报信息失败：锁编号：" + Model.LockNumber + ",不存在此单车", "控制器和电池组上报日志");
                        }
                    }
                }
                else
                {
                    LogHelper.Error("控制器和电池组上报信息失败:上传的锁编号为空,不存在此单车");
                    //Utility.Common.FileLog.Log("控制器和电池组上报信息失败：上传的锁编号为空", "控制器和电池组上报日志");
                }

                return result;
            }
            catch (Exception ex)
            {
                //Utility.Common.FileLog.Log(ex.Message, "BicycleHandleService");
                LogHelper.Error("控制器和电池组上报信息异常:" + ex.Message + "设备编号：" + Model.LockNumber, ex);
                return new Utils.ResultModel { IsSuccess = false, Message = "控制器和电池组上报信息异常" };
            }
        }

        /// <summary>
        /// 车辆检测报告
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ResultModel BicycleDetecting(Detecting data)
        {
            try
            {
                using (var db = new MintBicycleDataContext())
                {
                    var date = DateTime.Now;

                    var insert = new BiycleDetection();
                    insert.BellStatus = data.BellStatus;
                    insert.BicycleDetectionGuid = Guid.NewGuid();
                    insert.BicycleNumber = data.BicycleNumber;
                    insert.BluetoothStatus = data.BluetoothStatus;
                    insert.Brake = data.Brake;
                    insert.ContorollerNumber = data.ContorollerNumber;
                    insert.CreateBy = data.UserGuid;
                    insert.CreateTime = date;
                    insert.ContorollerBreakReason = data.ContorollerExReason;
                    insert.GyroscopeStatus = data.GyroscopeStatus;
                    insert.IsDeleted = false;
                    insert.Basket = data.Basket;
                    insert.BicycleFrame = data.BicycleFrame;
                    insert.ControlBox = data.ControlBox;
                    insert.Cushion = data.Cushion;
                    insert.FrontFork = data.FrontFork;
                    insert.HandleBar = data.HandleBar;
                    insert.HandlebarGrip = data.HandlebarGrip;
                    insert.KickStand = data.KickStand;
                    insert.Lock = data.Lock;
                    insert.Mud = data.Mud;
                    insert.LockNumber = data.LockNumber;
                    insert.QRCodeStatus = data.QRCodeStatus;
                    insert.ContorollerStatus = data.ContorollerStatus;
                    insert.UpdateTime = DateTime.Now;
                    insert.UpdateBy = data.UserGuid;
                    insert.QRCode = data.QRCode;
                    insert.TailLight = data.TailLight;
                    insert.Tires = data.Tires;
                    insert.Wheels = data.Wheels;
                    insert.HeadLight = data.HeadLight;

                    db.BiycleDetection.InsertOnSubmit(insert);
                    db.SubmitChanges();

                    return new ResultModel { ResObject = true };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message, ex);
                return new ResultModel { IsSuccess = false, Message = "记录车辆检测报告异常" };
            }
        }

        /// <summary>
        /// 检测报告列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ResultModel GetBicycleDetecting(Detec_PM data)
        {
            try
            {
                using (var db = new MintBicycleDataContext())
                {
                    var date = DateTime.Now;
                    var query = from d in db.BiycleDetection
                                join x in db.BicycleLockInfo on d.LockNumber equals x.LockNumber into L
                                from s in L.DefaultIfEmpty()
                                join b in db.BicycleBaseInfo on d.BicycleNumber equals b.BicycleNumber into B
                                from i in B.DefaultIfEmpty()
                                where !d.IsDeleted
                                select new Detec_OM
                                {
                                    BikeType = i.BicyCleTypeName ?? 0,
                                    LockNumber = d.LockNumber,

                                    BikeNumber = d.BicycleNumber,
                                    BluetoothStatus = d.BluetoothStatus,
                                    ContorollerNumber = d.ContorollerNumber,
                                    ContorollerExReason = d.ContorollerBreakReason,
                                    GyroscopeStatus = d.GyroscopeStatus,
                                    QRCodeStatus = d.QRCodeStatus,
                                    DelecTime = d.CreateTime,
                                    ContorollerStatus = d.ContorollerStatus,
                                    Basket = d.Basket,
                                    Wheels = d.Wheels,
                                    Tires = d.Tires,
                                    TailLight = d.TailLight,
                                    QRCode = d.QRCode,
                                    Mud = d.Mud,
                                    BellStatus = d.BellStatus,
                                    BicycleFrame = d.BicycleFrame,
                                    Brake = d.Brake,
                                    ControlBox = d.ControlBox,
                                    Cushion = d.Cushion,
                                    FrontFork = d.FrontFork,
                                    HandleBar = d.HandleBar,
                                    HandlebarGrip = d.HandlebarGrip,
                                    KickStand = d.KickStand,
                                    Lock = d.Lock,
                                     HeadLight=d.HeadLight




                                };

                    if (!string.IsNullOrEmpty(data.BicycleNumber))
                    {
                        query = query.Where(x => x.BikeNumber == data.BicycleNumber);
                    }

                    if (query.Any())
                    {
                        var list = new List<Detec_OM>();

                        var qlist = query.ToList();
                        foreach (var item in qlist)
                        {
                            var sig = new Detec_OM();

                            sig = item;


                            if (item.BellStatus == 1 || item.Basket == 1 || item.Brake == 1 || item.BicycleFrame == 1 || item.ControlBox == 1 || item.Cushion == 1 || item.FrontFork == 1 ||
                                item.HandleBar == 1 || item.HandlebarGrip == 1 || item.KickStand == 1 || item.Lock == 1 || item.Mud == 1 || item.QRCode == 1 || item.TailLight == 1 || item.Tires == 1 || item.Wheels == 1)
                            {
                                sig.HumanCheck = false;
                            }
                            else
                            {
                                sig.HumanCheck = true;

                            }

                            if (item.BikeType == 1)
                            {

                                if (sig.HumanCheck == true)
                                {
                                    if (sig.HeadLight == 1)
                                    {
                                        sig.HumanCheck =false;
                                    }

                                }

                            }


                            if (item.BluetoothStatus == 1)
                            {
                                sig.LockStatus = false;
                            }
                            else
                            {
                                sig.LockStatus = true;
                            }
                            if (item.ContorollerStatus == 1)
                            {

                                sig.ContorollerExReason = item.ContorollerExReason;
                            }


                            list.Add(sig);
                        }

                        var order = list.OrderByDescending(x => x.DelecTime).ToList();
                        return new ResultModel { ResObject = new PagedList<Detec_OM>(order, data.PageIndex, data.PageSize) };
                    }
                    return new ResultModel { IsSuccess = false, Message = "暂无检测报告" };
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message, ex);
                return new ResultModel { IsSuccess = false, Message = "获取检测报告异常" };
            }
        }



    }
}