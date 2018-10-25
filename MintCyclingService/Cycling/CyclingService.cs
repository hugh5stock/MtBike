using HuRongClub.DBUtility;
using MintCyclingData;
using MintCyclingService.BaiDu;
using MintCyclingService.Common;
using MintCyclingService.Handler;
using MintCyclingService.JPush;
using MintCyclingService.LogServer;
using MintCyclingService.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Utility;
using Utility.LogHelper;
using static MintCyclingService.Cycling.BaiduModel;

namespace MintCyclingService.Cycling
{
    public class CyclingService : ICyclingService
    {
        #region 电子围栏和车辆相关方法

        private ILogService LogServer = new LogServer.LogService();
        private IBaiduService baiduService = new BaiduService();

        /// <summary>
        /// 通过蓝牙关锁判断当前车辆是否在电子围栏内
        /// 如果不在，不用更新我的行程表中的EndTime时间，如果在则更新EndTime，同时更新车辆表中的ElectronicFenCingGuid和IsInElectronicFenCing两个字段
        /// 当车辆的设备编号为空时。表示此车在电子围栏内
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public BicycleOrEnclosureModel GetBicycleIsElectronicFenCing(decimal CurLongitude, decimal CurLatitude, decimal Radius, string DeviceNo)
        {
            //var result = new ResultModel();
            Guid ElectronicFenCingGuid = new Guid();
            string deviceNo = string.Empty;
            var data = new BicycleOrEnclosureModel { };

            //var radius = Radius == 0 || Radius < 1m ? 2000m : (1m);
            using (var db = new MintBicycleDataContext())
            {
                var stk = CoordinateHelper.GetDegreeCoordinates(CurLatitude, CurLongitude, Radius);
                var q0 = stk[0];
                var q1 = stk[1];
                var q2 = stk[2];
                var q3 = stk[3];

                #region 处理电子围栏数据

                //电子围栏信息
                var enclosureQuery = (from s in db.Electronic_FenCing
                                      where !s.IsDeleted && ((s.Longitude < q0.Longitude && s.Latitude < q0.Latitude) &&
                                         (s.Longitude > q1.Longitude && s.Latitude < q1.Latitude) && (s.Longitude < q2.Longitude && s.Latitude > q2.Latitude) && (s.Longitude > q3.Longitude && s.Latitude > q3.Latitude))
                                      select new BicycleOrEnclosureModel
                                      {
                                          EnclosureGuid = s.ElectronicFenCingGuid
                                      });
                if (enclosureQuery.Any())
                {
                    ElectronicFenCingGuid = enclosureQuery.FirstOrDefault().EnclosureGuid;
                    //foreach (var q in enclosureQuery)
                    //{
                    //    enclosureList.Add(new MapEnclosureModel
                    //    {
                    //        EnclosureGuid = q.ElectronicFenCingGuid,             //电子围栏编号
                    //    });
                    //}
                    data.EnclosureGuid = ElectronicFenCingGuid;
                }

                #endregion 处理电子围栏数据

                #region 判断车辆是否在电子围栏内

                var bicycleQuery = (from s in db.BicycleLockInfo
                                    where !s.IsDeleted && s.LockNumber == DeviceNo && ((s.Longitude < q0.Longitude && s.Latitude < q0.Latitude) &&
                                       (s.Longitude > q1.Longitude && s.Latitude < q1.Latitude) && (s.Longitude < q2.Longitude && s.Latitude > q2.Latitude) && (s.Longitude > q3.Longitude && s.Latitude > q3.Latitude))
                                    select new BicycleOrEnclosureModel
                                    {
                                        DeviceNo = s.LockNumber   //设备编号
                                    });
                if (bicycleQuery.Any())
                {
                    deviceNo = bicycleQuery.FirstOrDefault().DeviceNo;
                    //var bicycleList = new List<MapBicycleModel>();
                    //foreach (var q in bicycleQuery)
                    //{
                    //    bicycleList.Add(new MapBicycleModel
                    //    {
                    //        DeviceNo = q.DeviceNo,   //设备编号
                    //    });
                    //}
                    data.DeviceNo = deviceNo;
                }

                #endregion 判断车辆是否在电子围栏内
            }
            return data;
        }

        /// <summary>
        /// APP首页地图显示根据输入的地址搜索半径两公里范围内的车辆、电子围栏
        /// </summary>
        /// <returns></returns>
        public ResultModel GetNearBicycleEnclosure(NearBicycleEnclosure_PM para)
        {
            int FZLBicycleNum = 0;
            int helpBicycleNum = 0;
            string FZLCleTypeName = string.Empty;
            string helpCleTypeName = string.Empty;
            //int BicTypeName = para.BicyCleTypeName == null ? 2 : para.BicyCleTypeName; //全部

            var result = new ResultModel();
            //var radius = para.Radius == null || para.Radius < 1m ? 2000m : ((para.Radius ?? 1m) + 15m);
            var radius = para.Radius == null || para.Radius < 1m ? 2000m : ((para.Radius ?? 1m) + 20m);

            try
            {
                using (var db = new MintBicycleDataContext())
                {
                    //根据半径查找经纬度
                    var stk = CoordinateHelper.GetDegreeCoordinates(para.CurLatitude, para.CurLongitude, radius);
                    var q0 = stk[0];
                    var q1 = stk[1];
                    var q2 = stk[2];
                    var q3 = stk[3];
                    var data = new NearBicycleEnclosure_OM { };

                    #region 处理电子围栏数据

                    //电子围栏信息
                    var enclosureQuery = (from s in db.Electronic_FenCing
                                          where !s.IsDeleted && ((s.Longitude < q0.Longitude && s.Latitude < q0.Latitude) &&
                                             (s.Longitude > q1.Longitude && s.Latitude < q1.Latitude) && (s.Longitude < q2.Longitude && s.Latitude > q2.Latitude) && (s.Longitude > q3.Longitude && s.Latitude > q3.Latitude))
                                          select s);
                    if (enclosureQuery.Any())
                    {
                        var enclosureList = new List<MapEnclosureModel>();
                        foreach (var q in enclosureQuery)
                        {
                            #region 百度地图API

                            //调用百度API接口
                            //获取当前电子围栏的详细地址
                            string strJosn = baiduService.GetAddress(q.Longitude.ToString(), q.Latitude.ToString());

                            //获取距离和步行时间
                            double distance = 0; //单位：M
                            double time = 0;   //分钟
                            string strJosn1 = baiduService.GetDistanceTime(para.CurLatitude.ToString() + "," + para.CurLongitude.ToString(), q.Latitude.ToString() + "," + q.Longitude.ToString(), out distance, out time);

                            #endregion 百度地图API

                            #region 查询车辆数分为非助力车和助力车两种情况

                            //查询非助力车的数量
                            //string strSql = "select  count(*) as bicCount from BicycleLockInfo as b  where b.ElectronicFenCingGuid='" + q.ElectronicFenCingGuid + "' and b.LockStatus not in(0,2,3) and b.LockGuid not in(select BicycleBaseGuid from Reservation where    Status=1 )";
                            StringBuilder strSql = new StringBuilder();
                            strSql.Append("select  count(*) as bicCount from BicycleLockInfo as b  inner join  BicycleBaseInfo as c on b.LockNumber =c.LockNumber ");
                            strSql.Append("where  c.BicyCleTypeName=0 and b.ElectronicFenCingGuid='" + q.ElectronicFenCingGuid + "' ");
                            strSql.Append("and b.LockStatus =1 and b.LockGuid not in(select BicycleBaseGuid from Reservation where Status=1  or Status=3) ");
                            DataSet ds = DbHelperSQL.Query(strSql.ToString());
                            if (ds != null && ds.Tables[0].Rows.Count > 0)
                            {
                                FZLBicycleNum = int.Parse(ds.Tables[0].Rows[0]["bicCount"].ToString());
                                FZLCleTypeName = "非助力车";
                            }

                            //助力车的数量
                            StringBuilder str = new StringBuilder();
                            str.Append(" select  count(*) as helpBicycleNum from BicycleLockInfo as b inner join  BicycleBaseInfo as c on b.LockNumber =c.LockNumber ");
                            str.Append(" where c.BicyCleTypeName=1 and  b.ElectronicFenCingGuid='" + q.ElectronicFenCingGuid + "' ");
                            str.Append(" and b.LockStatus =1 and b.LockGuid not in(select BicycleBaseGuid from Reservation where Status=1 or Status=3) ");
                            DataSet fds = DbHelperSQL.Query(str.ToString());
                            if (fds != null && fds.Tables[0].Rows.Count > 0)
                            {
                                helpBicycleNum = int.Parse(fds.Tables[0].Rows[0]["helpBicycleNum"].ToString());
                                helpCleTypeName = "助力车";
                            }

                            #endregion 查询车辆数分为非助力车和助力车两种情况

                            //计算直线距离
                            var stq = CoordinateHelper.QDistance(new Coordinates(para.CurLongitude, para.CurLatitude), new Coordinates(q.Longitude, q.Latitude));

                            if (stq <= radius)
                            {
                                enclosureList.Add(new MapEnclosureModel
                                {
                                    EnclosureGuid = q.ElectronicFenCingGuid,                   //电子围栏编号
                                    EnclosureLongitude = q.Longitude,                         //经度
                                    EnclosureLatitude = q.Latitude,                          //纬度
                                    //BicycleNum = qBicycle(q.ElectronicFenCingGuid),       //车辆数量
                                    BicycleNum = FZLBicycleNum,
                                    HelpBicycleNum = helpBicycleNum,
                                    FZLCleTypeName = FZLCleTypeName,
                                    HelpCleTypeName = helpCleTypeName,
                                    Price = decimal.Parse("0.5"),
                                    Distance = distance,
                                    Mintues = time,
                                    Address = strJosn
                                });
                            }
                        }
                        data.EnclosureList = enclosureList;
                    }

                    #endregion 处理电子围栏数据

                    #region 处理车辆数据

                    //var qReservation = db.Reservation.OrderByDescending(k => k.CreateTime).ThenByDescending(k => k.DeviceNo).GroupBy(k => k.DeviceNo).Select(k => new { CreateTime = k.Max(p => p.CreateTime), DeviceNo = k.Min(y => y.DeviceNo) });
                    //List<Guid> qReservationList = new List<Guid>();
                    //if (qReservation.Any())
                    //{
                    //    var func = new Action<string, DateTime>((sk, st) =>
                    //    {
                    //        var s0 = db.Reservation.FirstOrDefault(qk => qk.DeviceNo == sk && qk.CreateTime == st && qk.Status != 1);
                    //        if (s0 != null)
                    //        {
                    //            qReservationList.Add(s0.ReservationGuid);
                    //        }
                    //    });
                    //    foreach (var s1 in qReservation)
                    //    {
                    //        func(s1.DeviceNo, s1.CreateTime);
                    //    }
                    //}

                    #region 非助力车数据

                    var bicycleQuery = (from b in db.BicycleBaseInfo
                                        join s in db.BicycleLockInfo on b.LockNumber equals s.LockNumber
                                        //into tep from bb in tep.DefaultIfEmpty()
                                        //join r in db.Reservation on s.LockGuid equals r.BicycleBaseGuid into temp
                                        //from rr in temp.DefaultIfEmpty()
                                        where s.LockStatus == 1 && s.IsInElectronicFenCing == null && !s.IsDeleted && b.BicyCleTypeName == 0
                                        //&& (rr == null || (rr != null && (rr.Status != 1) && (rr.Status == 0 || rr.Status == 2)))
                                        && ((s.Longitude < q0.Longitude && s.Latitude < q0.Latitude)
                                        && (s.Longitude > q1.Longitude && s.Latitude < q1.Latitude)
                                        && (s.Longitude < q2.Longitude && s.Latitude > q2.Latitude)
                                        && (s.Longitude > q3.Longitude && s.Latitude > q3.Latitude))
                                        //&& (!qReservationList.Any() || rr == null || (rr != null && qReservationList.Contains(rr.ReservationGuid)))
                                        select new MapBicycleModel
                                        {
                                            //DeviceNo = s.LockNumber,                      //锁的编号
                                            DeviceNo = b.BicycleNumber,                    //车辆编号
                                            BicycleLongitude = s.Longitude ?? 0,         //经度
                                            BicycleLatitude = s.Latitude ?? 0,          //纬度
                                            Price = decimal.Parse("0.5"),              //价格
                                            Distance = 0,                             //距离
                                            Mintues = 0,                             //步行时间
                                            Address = baiduService.GetAddress(s.Longitude.ToString(), s.Latitude.ToString())    //详细地址
                                        }).Distinct();
                    if (bicycleQuery.Any())
                    {
                        var bicycleList = new List<MapBicycleModel>();

                        foreach (var q in bicycleQuery)
                        {
                            //获取距离和步行时间
                            double distance = 0;
                            double time = 0;
                            //调用百度API接口
                            string strJosn = baiduService.GetDistanceTime(para.CurLatitude.ToString() + "," + para.CurLongitude.ToString(), q.BicycleLatitude.ToString() + "," + q.BicycleLongitude.ToString(), out distance, out time);

                            //计算直线距离
                            var st0 = CoordinateHelper.QDistance(new Coordinates(para.CurLongitude, para.CurLatitude), new Coordinates(q.BicycleLongitude ?? 0, q.BicycleLatitude ?? 0));

                            if (st0 <= radius)
                            {
                                var res = from x in db.Reservation
                                          where x.DeviceNo == q.DeviceNo && (x.Status == 1 || x.Status == 3)
                                          select x;
                                if (!res.Any())
                                {
                                    bicycleList.Add(new MapBicycleModel
                                    {
                                        DeviceNo = q.DeviceNo,                                //车辆编号
                                        BicycleLongitude = q.BicycleLongitude ?? 0,          //经度
                                        BicycleLatitude = q.BicycleLatitude ?? 0,           //纬度
                                        Price = decimal.Parse("0.5"),
                                        Distance = distance,
                                        Mintues = time,
                                        Address = baiduService.GetAddress(q.BicycleLongitude.ToString(), q.BicycleLatitude.ToString()),
                                    });
                                }
                            }
                        }

                        data.BicycleList = bicycleList;
                    }

                    #endregion 非助力车数据

                    #region 助力车的数据

                    var helpBicQuery = (from b in db.BicycleBaseInfo
                                        join s in db.BicycleLockInfo on b.LockNumber equals s.LockNumber
                                        //into tep from bb in tep.DefaultIfEmpty()
                                        //join r in db.Reservation on s.LockGuid equals r.BicycleBaseGuid into temp
                                        //from rr in temp.DefaultIfEmpty()
                                        where s.LockStatus == 1 && s.IsInElectronicFenCing == null && b.BicyCleTypeName == 1
                                        //&& (rr == null || (rr != null && (rr.Status != 1) && (rr.Status == 0 || rr.Status == 2)))
                                        && !s.IsDeleted
                                        && ((s.Longitude < q0.Longitude && s.Latitude < q0.Latitude) &&
                                           (s.Longitude > q1.Longitude && s.Latitude < q1.Latitude) &&
                                           (s.Longitude < q2.Longitude && s.Latitude > q2.Latitude) && (s.Longitude > q3.Longitude && s.Latitude > q3.Latitude))
                                        //&& (!qReservationList.Any() || rr == null || (rr != null && qReservationList.Contains(rr.ReservationGuid)))
                                        select new MapHelpBicycleModel
                                        {
                                            //DeviceNo = s.LockNumber,                      //设备编号
                                            DeviceNo = b.BicycleNumber,                    //车辆编号
                                            BicycleLongitude = s.Longitude ?? 0,         //经度
                                            BicycleLatitude = s.Latitude ?? 0,          //纬度
                                            Price = decimal.Parse("0.5"),              //价格
                                            Distance = 0,                             //距离
                                            Mintues = 0,                             //步行时间
                                            Address = baiduService.GetAddress(s.Longitude.ToString(), s.Latitude.ToString())    //详细地址
                                        }).Distinct();
                    if (helpBicQuery.Any())
                    {
                        var HelpBicycleList = new List<MapHelpBicycleModel>();

                        foreach (var q in helpBicQuery)
                        {
                            //获取距离和步行时间
                            double distance = 0;
                            double time = 0;
                            //调用百度API接口
                            string strJosn = baiduService.GetDistanceTime(para.CurLatitude.ToString() + "," + para.CurLongitude.ToString(), q.BicycleLatitude.ToString() + "," + q.BicycleLongitude.ToString(), out distance, out time);

                            //计算直线距离
                            var st0 = CoordinateHelper.QDistance(new Coordinates(para.CurLongitude, para.CurLatitude), new Coordinates(q.BicycleLongitude ?? 0, q.BicycleLatitude ?? 0));

                            if (st0 <= radius)
                            {
                                //去除预约中或者骑行中的车辆
                                var res1 = from x in db.Reservation
                                           where x.DeviceNo == q.DeviceNo && (x.Status == 1 || x.Status == 3)
                                           select x;
                                if (!res1.Any())
                                {
                                    HelpBicycleList.Add(new MapHelpBicycleModel
                                    {
                                        DeviceNo = q.DeviceNo,                                //车辆编号
                                        BicycleLongitude = q.BicycleLongitude ?? 0,          //经度
                                        BicycleLatitude = q.BicycleLatitude ?? 0,           //纬度
                                        Price = decimal.Parse("0.5"),
                                        Distance = distance,
                                        Mintues = time,
                                        Address = baiduService.GetAddress(q.BicycleLongitude.ToString(), q.BicycleLatitude.ToString()),
                                    });
                                }
                            }
                        }
                        data.HelpBicycleList = HelpBicycleList;
                    }

                    #endregion 助力车的数据

                    #endregion 处理车辆数据

                    //车辆信息
                    result.ResObject = data.BicycleList == null && data.EnclosureList == null && data.HelpBicycleList == null ? null : data;
                    //LogHelper.Info("App首页面地图查询结果集："+result.ResObject);
                }
            } catch (Exception ex)
            {
                LogHelper.Error("搜索半径两公里范围内的车辆、电子围栏异常:" + ex.Message + ",经度:" + para.CurLongitude + ",纬度：" + para.CurLatitude + "半径：" + para.Radius+"", ex);
                result.Message = "搜索半径两公里范围内的车辆、电子围栏异常,请稍后！";
                result.ResObject=null;
            }
            return result;
        }

        /// <summary>
        /// 查询指定地点的车辆信息
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        //public ResultModel GetBicycleListByCoordinate(CoordinateBicycleList_PM para)
        //{
        //    var result = new ResultModel();
        //    using (var db = new MintBicycleDataContext())
        //    {
        //        var data = new CoordinateBicycleList_OM { };
        //        var bicyCleTypeFunc = new Func<Guid?, string>(p =>
        //        {
        //            var q = db.BicycleTypeInfo.FirstOrDefault(t => t.BicycleTypeGuid == p && !t.IsDeleted);
        //            return q == null ? "未知类型" : q.BicyCleTypeName;
        //        });
        //        if (para.EnclosureGuid != null)
        //        {
        //            var enclosure = db.Electronic_FenCing.FirstOrDefault(y => y.ElectronicFenCingGuid == para.EnclosureGuid);
        //            if (enclosure != null)
        //            {
        //                var query = db.BicycleLockInfo.Where(s => s.ElectronicFenCingGuid == para.EnclosureGuid && s.BicyCleTypeGuid != null);
        //                if (query.Any())
        //                {
        //                    var cnt = query.Count();
        //                    var bicyCleType = query.GroupBy(k => k.BicyCleTypeGuid).Select(group => new CurRegionBicycleModel
        //                    {
        //                        BicyCleTypeName = bicyCleTypeFunc(group.Key),
        //                        RegionBicycleNum = group.Count()
        //                    });
        //                    data.DetailAddress = enclosure.Address;
        //                    data.CurRegionBicycleList = bicyCleType.ToList();
        //                    data.BicycleNum = cnt;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (para.EnclosureLongitude != null && para.EnclosureLatitude != null)
        //            {
        //                var bicycle = db.BicycleLockInfo.FirstOrDefault(sk => sk.Longitude == para.EnclosureLongitude && sk.Latitude == para.EnclosureLatitude);
        //                if (bicycle != null)
        //                {
        //                    data.DetailAddress = bicycle.Address;
        //                    data.CurRegionBicycleList = new List<CurRegionBicycleModel> { new CurRegionBicycleModel { BicyCleTypeName = bicyCleTypeFunc(bicycle.BicyCleTypeGuid), RegionBicycleNum = 1 } };
        //                    data.BicycleNum = 1;
        //                }
        //            }
        //        }
        //        result.ResObject = data;
        //    }
        //    return result;
        //}

        /// <summary>
        /// 查询是否停放在电子围栏区域
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public ResultModel QueryBicycleIsinRange(BicycleIsinRange_PM para)
        {
            var result = new ResultModel();
            var radius = 20m;
            using (var db = new MintBicycleDataContext())
            {
                var stk = CoordinateHelper.GetDegreeCoordinates(para.CurLatitude, para.CurLongitude, radius);
                var q0 = stk[0];
                var q1 = stk[1];
                var q2 = stk[2];
                var q3 = stk[3];
                var data = new NearBicycleEnclosure_OM { };
                var enclosureQuery = (from s in db.Electronic_FenCing
                                      where !s.IsDeleted && ((s.Longitude < q0.Longitude && s.Latitude < q0.Latitude) &&
                                         (s.Longitude > q1.Longitude && s.Latitude < q1.Latitude) && (s.Longitude < q2.Longitude && s.Latitude > q2.Latitude) && (s.Longitude > q3.Longitude && s.Latitude > q3.Latitude))
                                      select s);
                //result.ResObject = enclosureQuery.Any();
                var enclosureList = new List<MapEnclosureModel>();
                if (enclosureQuery.Any())
                {
                    foreach (var q in enclosureQuery)
                    {
                        //计算直线距离-判断当前车辆的坐标和围栏的距离，如果小于20m，说明此车在电子围栏内。
                        var st0 = CoordinateHelper.QDistance(new Coordinates(para.CurLongitude, para.CurLatitude), new Coordinates(q.Longitude, q.Latitude));
                        if (st0 <= radius)
                        {
                            enclosureList.Add(new MapEnclosureModel
                            {
                                EnclosureGuid = q.ElectronicFenCingGuid,             //电子围栏编号
                            });
                        }
                    }
                    result.ResObject = enclosureList;
                }
                else
                {
                    string str = "{00000000-0000-0000-0000-000000000000}";
                    Guid EGuid = new Guid(str);
                    enclosureList.Add(new MapEnclosureModel
                    {
                        EnclosureGuid = EGuid,             //电子围栏编号
                    });
                    result.ResObject = enclosureList;
                }
            }
            return result;
        }

        /// <summary>
        /// 检查单车是否存在
        /// </summary>
        /// <param name="deviceNo"></param>
        /// <returns></returns>
        public ResultModel GetLockStatus(string deviceNo)
        {
            ResultModel model = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var query = db.BicycleLockInfo.FirstOrDefault(s => s.LockNumber == deviceNo && !s.IsDeleted);
                if (query == null)
                {
                    model.IsSuccess = false;
                    model.MsgCode = ResPrompt.BicycleNotExist;
                    model.Message = ResPrompt.BicycleNotExistMessage;
                    return model;
                }
                model.ResObject = query.LockStatus;
            }
            return model;
        }

        /// <summary>
        /// 根据锁编号或者车辆编号查询车辆类型
        /// </summary>
        /// <param name="LockNumber"></param>
        /// <returns></returns>
        public ResultModel GetBicycleTypeNameByBicycleNumber(string LockNumber)
        {
            var result = new ResultModel();
            var BicycleModel = new GetBicycleTypeName_OM();
            int Ischoice = 1;   //如果是助力车并且电量低于20%Ischoice的值为0，app不能选择助力模式骑行-是否能选择助力车模式
            decimal Electricity = 0;
            decimal Capacity = 0;
            decimal electricQuantity = 0;

            using (var db = new MintBicycleDataContext())
            {
                var query = db.BicycleBaseInfo.FirstOrDefault(s => s.LockNumber == LockNumber || s.BicycleNumber == LockNumber && !s.IsDeleted);
                if (query == null)
                {
                    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.BicycleNotExist, Message = ResPrompt.BicycleNotExistMessage };
                }
                else
                {
                    if (query.BicycleNumber == null && LockNumber == null) //车和锁未做匹配
                    {
                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.BicycleLockNotExist, Message = ResPrompt.BicycleLockNotExistMessage };
                    }
                    //根据锁编号获取电量值
                    electricQuantity = GetElectricQuantityByLockNumber(LockNumber);
                    if (query.BicyCleTypeName == 1 && electricQuantity > decimal.Parse(0.2.ToString()))
                    {
                        Ischoice = 1; //app用来做选择用
                    }
                    else
                    {
                        Ischoice = 0;
                    }
                    BicycleModel.electricQuantity = electricQuantity;
                    BicycleModel.Ischoice = Ischoice;
                    BicycleModel.BicycleTypeName = BicycleBaseInfoShow.GetBicycleTypeName(query.BicyCleTypeName);
                    result.IsSuccess = true;
                    result.Message = "获取车辆类型成功";
                    result.ResObject = BicycleModel;
                }
            }
            return result;
        }

        /// <summary>
        /// 根据车辆编号或者锁编号查询匹配信息
        /// </summary>
        /// <param name="Number"></param>
        /// <returns></returns>
        public bool GetBicycleLockByNumber(string Number)
        {
            using (var db = new MintBicycleDataContext())
            {
                var query = db.BicycleBaseInfo.FirstOrDefault(s => s.LockNumber == Number || s.BicycleNumber == Number && !s.IsDeleted);
                if (query == null && query.BicycleNumber == null && query.LockNumber == null)//车和锁未做匹配
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        #endregion 电子围栏和车辆相关方法

        #region 关锁相关方法

        /// <summary>
        /// 新增锁信息-提供给硬件锁调用
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public ResultModel AddBicycleLockInfo(AddBicycleHardWare_PM Model)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                #region 添加

                try
                {
                    //判断是否存在相同车锁编号的车辆信息
                    if (db.BicycleLockInfo.Any(x => x.LockNumber == Model.DeviceNo))
                    {
                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.BicycleNumberError, Message = ResPrompt.BicycleNumberErrorMessage };
                    }
                    var Bicycle = new BicycleLockInfo
                    {
                        LockGuid = Guid.NewGuid(),
                        LockNumber = Model.DeviceNo,    
                        DeviceNo = "0",
                        SecretKey = Model.SecretKey == null ? string.Empty : Model.SecretKey,
                        LockMac = Model.LockMac,
                        LockStatus = 1,
                        Voltage = decimal.Parse(Model.Voltage == null ? string.Empty : Model.Voltage) / 100,
                        CreateTime = DateTime.Now,
                        IsDeleted = false
                    };
                    db.BicycleLockInfo.InsertOnSubmit(Bicycle);
                    db.SubmitChanges();
                }
                catch (Exception ex)
                {
                    LogHelper.Error("上传车锁信息异常:" + ex.Message + "锁的编号：" + Model.DeviceNo, ex);
                    return new ResultModel { IsSuccess = false, MsgCode = "0", Message = "上传车锁信息异常" };
                }
                result.IsSuccess = true;
                result.MsgCode = "0";
                result.Message = "上传车锁信息成功";
                result.ResObject = null;

                #endregion 添加
            }
            return result;
        }

        /// <summary>
        /// 上传交易记录-手机蓝牙或者硬件锁上传
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public ResultModel OpenOrCloseBicycleLockData(OpenOrCloseLockBicycle_PM para)
        {
            var result = new ResultModel();

            Guid UserGuid = new Guid("{00000000-0000-0000-0000-000000000000}");
            string Msg = string.Empty;
            DateTime dtNow = DateTime.Now;
            string MsgCode = string.Empty;
            string StrDeviceNo = string.Empty;
            string typename = para.TypeName.Trim().ToString();
            decimal? Voltage = 0;

            if (typename == "mobile")
            {
                Voltage = para.Voltage == null ? 0 : para.Voltage;
            }
            else
            {
                Voltage = para.Voltage == null ? 0 : para.Voltage / 100;
            }
      
            //参数不能为空
            if (string.IsNullOrEmpty(para.LockAction.ToString()) || string.IsNullOrEmpty(para.Timestamp) || string.IsNullOrEmpty(para.DeviceNo))
            {
                LogServer.InsertDBLockOC(StrDeviceNo, para.LockAction, UserGuid, para.TypeName, "参数出现问题，为空");
                return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ParaModelNotExist, Message = ResPrompt.ParaModelNotExistMessage };
            }
            //锁编号
            StrDeviceNo = para.DeviceNo;

            var dt = DateTime.Now;
            //把时间戳转换成日期格式
            if ((para.LockAction == 0 || para.LockAction == 1) && (para.TypeName == "mobile"))
            {
                dt = GetTime(para.Timestamp);
            }
            else
            {
                DateTime dtEnd1 = DateTime.Parse(para.Timestamp);
                //转换时间--把UTC格式转换datetime
                double dtTamp = ConvertDateTimeInt(dtEnd1);
                dt = ConvertIntDatetime(dtTamp);
                para.Timestamp = dtTamp.ToString();
                //Utility.Common.FileLog.Log("时间戳：" + para.Timestamp, "OpenOrCloseLog");
            }

            #region 处理设备编号

            //if (para.DeviceNo != null && para.LockAction == 0 && para.TypeName == "mobile") //手机端开锁
            //{
            //    StrDeviceNo = para.DeviceNo;
            //    //Utility.Common.FileLog.Log("设备编号：" + StrDeviceNo + "\r\n", "开锁加密串");
            //}
            //else if (para.DeviceNo != null && para.LockAction == 1 && para.TypeName == "hardware") //表示硬件上传过来的关锁交易记录
            //{
            //    StrDeviceNo = para.DeviceNo.Remove(0, 3);
            //}
            //else if (para.DeviceNo != null && para.LockAction == 1 && para.TypeName == "mobile")//手机端关锁
            //{
            //    StrDeviceNo = para.DeviceNo;
            //}

            #endregion 处理设备编号

            try
            {
                using (var db = new MintBicycleDataContext())
                {
                    #region 判断处理

                    if (para.UserInfoGuid == UserGuid) //表示硬件上传，没有用户Guid
                    {
                        var queryUserGuid = db.TransactionLog.OrderByDescending(s => s.CreateTime).FirstOrDefault(s => s.DeviceNo == StrDeviceNo && s.OpenLockAction == 0 && !s.IsDeleted);
                        if (queryUserGuid != null)
                        {
                            UserGuid = queryUserGuid.UserInfoGuid;
                        }
                    }
                    else
                    {
                        UserGuid = para.UserInfoGuid;
                    }

                    //车辆基本信息
                    var query = db.BicycleLockInfo.FirstOrDefault(s => s.LockNumber == StrDeviceNo && !s.IsDeleted);
                    if (query == null)
                    {
                        LogServer.InsertDBLockOC(StrDeviceNo, para.LockAction, UserGuid, para.TypeName, "未查找到锁信息");
                        //Utility.Common.FileLog.Log("设备编号：" + StrDeviceNo + ",车辆基本信息：" + query + "\r\n", "车辆不存在");
                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.BicycleNotExist, Message = ResPrompt.BicycleNotExistMessage };
                    }

                    //查询用户表
                    var usersQuery = db.UserInfo.FirstOrDefault(s => s.UserInfoGuid == UserGuid && !s.IsDeleted);
                    if (usersQuery == null)
                    {
                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.UserinfoNotExist, Message = ResPrompt.UserinfoNotExistMessage };
                    }

                    ////已开锁状态
                    //if (usersQuery.BicycleBaseGuid != null && query.LockStatus == 0 && para.LockAction == 0)
                    //{
                    //    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.OpenNotError, Message = ResPrompt.OpenErrorMessage };
                    //}
                    ////已关锁状态
                    //if (usersQuery.BicycleBaseGuid == null && query.LockStatus == 1 && para.LockAction == 1)
                    //{
                    //    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.CloseNotError, Message = ResPrompt.CloseErrorMessage };
                    //}

                    #endregion 判断处理

                    #region 预约处理

                    //在开锁时判断此车是否被别的用户预约过，预约过就不能开锁
                    var IsQuery = db.Reservation.FirstOrDefault(s => s.DeviceNo == StrDeviceNo && s.Status == 1);
                    if (IsQuery != null && IsQuery.UserInfoGuid != UserGuid)
                    {
                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ReservationExist, Message = ResPrompt.ReservationExistMessage };
                    }

                    //处理预约是否超时
                    var ReservationQuery = db.Reservation.OrderByDescending(s => s.CreateTime).FirstOrDefault(s => s.DeviceNo == StrDeviceNo && s.UserInfoGuid == UserGuid && s.Status == 1);

                    //修改预约的状态为已结束
                    if (ReservationQuery != null) //说明此用户用手机预约了单车，否则直接扫码骑行单车
                    {
                        // 计算两个时间的差值,取分钟--20分钟倒计时处理业务逻辑
                        int Mintues = GetTotalMinutes(ReservationQuery.StartTme);
                        if (Mintues > 20)
                        {
                            ReservationQuery.Status = 2; //预约结束
                        }
                        if (para.LockAction == 0)
                        {
                            ReservationQuery.Status = 3; //骑行中
                        }

                        ReservationQuery.EndTime = dtNow;
                        ReservationQuery.UpdateBy = UserGuid;
                        ReservationQuery.UpdateTime = dtNow;
                        db.SubmitChanges();
                    }
                    if (para.LockAction == 1) //关锁时
                    {
                        var Rquery = db.Reservation.OrderByDescending(s => s.CreateTime).FirstOrDefault(s => s.DeviceNo == StrDeviceNo && s.UserInfoGuid == UserGuid && s.Status == 3);
                        if (Rquery != null)
                        {
                            Rquery.Status = 2; //已结束
                            Rquery.EndTime = dtNow;
                            Rquery.UpdateBy = UserGuid;
                            Rquery.UpdateTime = dtNow;


                            db.SubmitChanges();
                        }
                    }

                    #endregion 预约处理

                    //添加交易记录信息
                    if (!AddTransactionLog(para, dt, UserGuid, StrDeviceNo, dtNow, Voltage, out Msg, out MsgCode))
                    {
                        LogServer.InsertDBLockOC(StrDeviceNo, para.LockAction, UserGuid, para.TypeName, "添加交易记录信息异常");
                        result.IsSuccess = false;
                        result.MsgCode = MsgCode;
                        result.Message = Msg;
                        return result;
                    }

                    #region 处理开关锁相关数据

                    switch (para.LockAction)
                    {
                        case 0:
                            //添加我的行程
                            var newTravel = new MyTravel
                            {
                                MyTravelGuid = Guid.NewGuid(),
                                LockNumber = StrDeviceNo,
                                LockGuid = query.LockGuid,
                                //新增骑行模式
                                CyclingMode = para.CyclingMode,
                                UserInfoGuid = UserGuid,
                                StartLongitude = para.Longitude != null ? para.Longitude : query.Longitude,
                                StartLatitude = para.Latitude != null ? para.Latitude : query.Latitude,
                                StartTime = dt,
                                CreateTime = dtNow,
                                IsDeleted = false
                            };
                            db.MyTravel.InsertOnSubmit(newTravel);
                            //修改车辆的状态为0表示开锁或者使用
                            query.LockStatus = (int)BicycleStatusEnum.OpenLockStatus;
                            query.DeviceNo = StrDeviceNo;
                            query.LockNumber = StrDeviceNo;
                            query.Voltage = Voltage;
                            usersQuery.LockGuid = query.LockGuid;  //修改用户表中的锁的Guid表示那个用户开锁

                            result.IsSuccess = true;
                            result.MsgCode = "0";
                            result.Message = "开锁成功！";
                            break;

                        case 1:
                            //修改行程信息
                            var myTravel = db.MyTravel.OrderByDescending(s => s.CreateTime).FirstOrDefault(s => !s.IsDeleted && s.LockGuid == query.LockGuid && s.UserInfoGuid == UserGuid);
                            if (myTravel != null)  //没有上传开锁交易记录信息
                            {
                                myTravel.EndLongitude = para.Longitude != null ? para.Longitude : query.Longitude;
                                myTravel.EndLatitude = para.Latitude != null ? para.Latitude : query.Latitude;
                                if (Convert.ToInt64(para.Timestamp) < 0) //处理传过来的时间戳为负数
                                {
                                    myTravel.EndTime = dtNow;
                                }
                                else
                                {
                                    myTravel.EndTime = dt;
                                }
                                myTravel.UpdateTime = dtNow;
                                myTravel.Status = 1; // 交易状态(0手动还车、1自动还车)
                            }

                            //修改车辆的状态为1表示关锁或者空闲
                            //query.LockStatus = (int)BicycleStatusEnum.CloseLockStatus;
                            //query.DeviceNo = CloseDeviceNo; //清空为0
                            query.Voltage = Voltage;
                            //usersQuery.BicycleBaseGuid = null;  //修改用户表中的车辆Guid表示那个用户关锁

                            result.IsSuccess = true;
                            result.MsgCode = "0";
                            result.Message = "关锁成功！";
                            break;

                        default:
                            return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.BicycleLockTypeError, Message = ResPrompt.BicycleLockTypeErrorMessage };
                    }

                    #endregion 处理开关锁相关数据

                    #region 调用百度API--更新车辆基本信息表

                    string provinceName = string.Empty;
                    string cityName = string.Empty;
                    string districtName = string.Empty;
                    string Address = string.Empty;

                    int ProvinceID = 0;
                    int cityID = 0;
                    int districtID = 0;
                    decimal? pLongitude = 0;
                    decimal? pLatitude = 0;

                    pLongitude = para.Longitude != null ? para.Longitude : query.Longitude;
                    pLatitude = para.Latitude != null ? para.Latitude : query.Latitude;

                    Address = baiduService.GetAddress(pLongitude.ToString(), pLatitude.ToString()); //详细地址
                    List<AddressModel> list = baiduService.GetAddressInfo(pLongitude.ToString(), pLatitude.ToString());
                    if (list != null && list.Count > 0)
                    {
                        provinceName = list[0].provinceName;
                        cityName = list[0].city;
                        districtName = list[0].district;

                        string sqlStr = @"select p.Id as ProvinceID,c.Id as cityID,d.Id as districtID from Province as p
                                                inner join City as c on p.Id =c.ProvinceId inner join   District as d on c.Id = d.CityId
                                                where p.Name in('" + provinceName + "') and c.Name in('" + cityName + "') and d.Name   in('" + districtName + "') ";

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
                        query.ProvinceID = ProvinceID;
                        query.CityID = cityID;
                        query.DistrictID = districtID;
                        //修改用户所在的省市区
                        usersQuery.ProvinceID = ProvinceID;
                        usersQuery.CityID = cityID;
                        usersQuery.DistrictID = districtID;
                    }

                    #region 开关锁时修改车辆信息

                    if (query != null && para.Longitude == null && para.Latitude == null)
                    {
                        pLongitude = para.Longitude != null ? para.Longitude : query.Longitude;
                        pLatitude = para.Latitude != null ? para.Latitude : query.Latitude;
                    }
                    else if (query != null && para.Longitude == 0 && para.Latitude == 0)
                    {
                        pLongitude = para.Longitude != 0 ? para.Longitude : query.Longitude;
                        pLatitude = para.Latitude != 0 ? para.Latitude : query.Latitude;
                    }
                    else
                    {
                        pLongitude = para.Longitude;
                        pLatitude = para.Latitude;
                    }
                    query.Longitude = pLongitude;
                    query.Latitude = pLatitude;
                    query.Address = Address;
                    query.Voltage = Voltage != null ? Voltage : query.Voltage;    //电压
                    query.UpdateTime = dt;
                    db.SubmitChanges();

                    #endregion 开关锁时修改车辆信息

                    //LogServer.InsertDBLockOC(StrDeviceNo, para.LockAction, usersQuery.UserInfoGuid, para.TypeName, "调用百度API--更新车辆基本信息异常");
                    //Utility.Common.FileLog.Log("调用百度API--更新车辆基本信息异常：" + para.LockAction + ex.Message, "OpenOrCloseLog");

                    #endregion 调用百度API--更新车辆基本信息表

                    #region 推送给App关锁是否成功

                    if (para.LockAction == 1 && para.TypeName == "hardware") //关锁时推送
                    {
                        //Utility.Common.FileLog.Log("关锁时推送：" + usersQuery.PushId + usersQuery.UserInfoGuid, "JPushLog");
                        IJPushService _JPushService = new JPushService();
                        string Content = "关锁成功";
                        List<string> registrationList = new List<string>();
                        registrationList.Add(usersQuery.PushId);
                        //调用极光自定义消息接口
                        string PushMessage = string.Empty;
                        string ResultStr = _JPushService.SendPostRequest(Content, registrationList);
                        JObject jo = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(ResultStr);
                        string Code = jo["error"]["code"].ToString();
                        if (Code == "200")
                        {
                            usersQuery.IsPush = "已推送";
                            PushMessage = "自定义消息推送成功";
                            //Utility.Common.FileLog.Log("锁的编号：" + StrDeviceNo + ", 调用极光推送自定义消息：" + ResultStr + "，状态：已推送，PushID=" + usersQuery.PushId + "，用户Guid" + usersQuery.UserInfoGuid, "JPushLog");
                            LogHelper.Info("锁的编号：" + StrDeviceNo + "调用极光推送自定义消息：" + ResultStr + "状态：已推送PushID=" + usersQuery.PushId + "，用户Guid" + usersQuery.UserInfoGuid);
                        }
                        else
                        {
                            LogServer.InsertDBLockOC(StrDeviceNo, para.LockAction, usersQuery.UserInfoGuid, para.TypeName, "极光推送未推送，出现异常");
                            usersQuery.IsPush = "未推送";
                            PushMessage = "自定义消息推送失败";
                            //Utility.Common.FileLog.Log("设备编号：" + StrDeviceNo + ", 调用极光推送自定义消息：" + ResultStr + "，状态：未推送，PushID=" + usersQuery.PushId + "，用户Guid" + usersQuery.UserInfoGuid, "JPushLog");
                            LogHelper.Error("锁的编号：" + StrDeviceNo+"调用极光推送自定义消息：" + ResultStr+ "状态：未推送,PushID=" + usersQuery.PushId + "，用户Guid" + usersQuery.UserInfoGuid);
                        }
                        db.SubmitChanges();
                    }

                    #endregion 推送给App关锁是否成功
                }
            }
            catch (Exception ex)
            {
                LogServer.InsertDBLockOC(StrDeviceNo, para.LockAction, UserGuid, para.TypeName, "开关锁异常" + ex);
                result.IsSuccess = false;
                result.MsgCode = "0";
                result.Message = "开关锁异常,请重新开关锁！";
                LogHelper.Error("平台用户开关锁异常:" + ex.Message + ",用户Guid:" + UserGuid + ",锁的编号：" + para.DeviceNo + "", ex);
            }
            return result;
        }

        /// <summary>
        /// 维修人员开关锁
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public ResultModel WxOpenOrClosekBicycleLock(OpenOrCloseLockBicycle_PM para)
        {
            Guid UserGuid = new Guid("{00000000-0000-0000-0000-000000000000}");
            string Msg = string.Empty;
            DateTime dtNow = DateTime.Now;
            string MsgCode = string.Empty;
            string StrDeviceNo = string.Empty;
            var result = new ResultModel();
            string typename = para.TypeName.Trim().ToString();
            decimal? Voltage = 0;

            if (typename == "mobile")
            {
                Voltage = para.Voltage == null ? 0 : para.Voltage;
            }
            else
            {
                Voltage = para.Voltage == null ? 0 : para.Voltage / 100;
            }
            //测试日志记录
            //Utility.Common.FileLog.Log("区分硬件类型：" + typename + "用户Guid:" + para.UserInfoGuid + ",车锁状态值：" + para.LockAction + "设备编号" + para.DeviceNo + "当前时间" + DateTime.Now + "经度值：" + para.Longitude + "纬度值：" + para.Latitude + "时间戳：" + para.Timestamp, logType);

            //参数不能为空
            if (string.IsNullOrEmpty(para.LockAction.ToString()) || string.IsNullOrEmpty(para.Timestamp) || string.IsNullOrEmpty(para.DeviceNo))
            {
                return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ParaModelNotExist, Message = ResPrompt.ParaModelNotExistMessage };
            }
            //锁编号
            StrDeviceNo = para.DeviceNo;

            var dt = DateTime.Now;
            //把时间戳转换成日期格式
            if ((para.LockAction == 0 || para.LockAction == 1) && (para.TypeName == "mobile"))
            {
                dt = GetTime(para.Timestamp);
            }
            else
            {
                DateTime dtEnd1 = DateTime.Parse(para.Timestamp);
                //转换时间--把UTC格式转换datetime
                double dtTamp = ConvertDateTimeInt(dtEnd1);
                dt = ConvertIntDatetime(dtTamp);
                para.Timestamp = dtTamp.ToString();
                //Utility.Common.FileLog.Log("时间戳：" + para.Timestamp, "OpenOrCloseLog");
            }

            #region 处理设备编号

            //if (para.DeviceNo != null && para.LockAction == 0 && para.TypeName == "mobile") //手机端开锁
            //{
            //    StrDeviceNo = para.DeviceNo;
            //    //Utility.Common.FileLog.Log("设备编号：" + StrDeviceNo + "\r\n", "开锁加密串");
            //}
            //else if (para.DeviceNo != null && para.LockAction == 1 && para.TypeName == "hardware") //表示硬件上传过来的关锁交易记录
            //{
            //    StrDeviceNo = para.DeviceNo.Remove(0, 3);
            //}
            //else if (para.DeviceNo != null && para.LockAction == 1 && para.TypeName == "mobile")//手机端关锁
            //{
            //    StrDeviceNo = para.DeviceNo;
            //}

            #endregion 处理设备编号

            try
            {
                using (var db = new MintBicycleDataContext())
                {
                    #region 判断处理

                    if (para.UserInfoGuid == UserGuid) //表示硬件上传，没有用户Guid
                    {
                        //var querys = (from l in db.TransactionLog select l.UserInfoGuid).FirstOrDefault();
                        var queryUserGuid = db.PersonTransactionLog.OrderByDescending(s => s.CreateTime).FirstOrDefault(s => s.DeviceNo == StrDeviceNo && s.OpenLockAction == 0 && !s.IsDeleted);
                        if (queryUserGuid != null)
                        {
                            UserGuid = queryUserGuid.UserInfoGuid;
                        }
                    }
                    else
                    {
                        UserGuid = para.UserInfoGuid;
                    }

                    //车辆基本信息
                    var query = db.BicycleLockInfo.FirstOrDefault(s => s.LockNumber == StrDeviceNo && !s.IsDeleted);
                    if (query == null)
                    {
                        LogServer.InsertDBLockOC(StrDeviceNo, para.LockAction, UserGuid, para.TypeName, "未查找到锁信息");
                        //Utility.Common.FileLog.Log("设备编号：" + StrDeviceNo + ",车辆基本信息：" + query + "\r\n", "车辆日志");
                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.BicycleNotExist, Message = ResPrompt.BicycleNotExistMessage };
                    }

                    var PersonInfo = db.ServicePersonInfo.FirstOrDefault(s => s.ServicePersonID == UserGuid);
                    if (PersonInfo == null)
                    {
                        LogServer.InsertDBLockOC(StrDeviceNo, para.LockAction, UserGuid, para.TypeName, "未查找到维修人员信息");
                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.wxUserinfoNotExist, Message = ResPrompt.wxUserinfoNotExistMessage };
                    }

                    #endregion 判断处理

                    #region 处理开关锁相关数据

                    switch (para.LockAction)
                    {
                        case 0:
                            query.LockStatus = (int)BicycleStatusEnum.OpenLockStatus;
                            query.DeviceNo = StrDeviceNo;
                            query.LockNumber = StrDeviceNo;
                            query.Voltage = Voltage;
                            PersonInfo.LockGuid = query.LockGuid;  //修改用户表中的车辆Guid表示那个用户开锁

                            result.IsSuccess = true;
                            result.MsgCode = "0";
                            result.Message = "维修人员开锁成功！";
                            break;

                        case 1:
                            query.LockStatus = (int)BicycleStatusEnum.CloseLockStatus;
                            query.Voltage = Voltage;
                            PersonInfo.LockGuid = null;
                            result.IsSuccess = true;
                            result.MsgCode = "0";
                            result.Message = "维修人员关锁成功！";
                            break;

                        default:
                            return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.BicycleLockTypeError, Message = ResPrompt.BicycleLockTypeErrorMessage };
                    }

                    #endregion 处理开关锁相关数据

                    //添加维修人员开关锁交易记录信息
                    if (!AddPersonTransactionLog(para, dt, UserGuid, StrDeviceNo, dtNow, Voltage, out Msg, out MsgCode))
                    {
                        result.IsSuccess = false;
                        result.MsgCode = MsgCode;
                        result.Message = Msg;
                        return result;
                    }

                    #region 调用百度API--更新车辆基本信息表

                    string provinceName = string.Empty;
                    string cityName = string.Empty;
                    string districtName = string.Empty;
                    string Address = string.Empty;

                    int ProvinceID = 0;
                    int cityID = 0;
                    int districtID = 0;

                    Address = baiduService.GetAddress(para.Longitude.ToString(), para.Latitude.ToString()); //详细地址
                    List<AddressModel> list = baiduService.GetAddressInfo(para.Longitude.ToString(), para.Latitude.ToString());
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
                        query.ProvinceID = ProvinceID;
                        query.CityID = cityID;
                        query.DistrictID = districtID;
                    }

                    #region 开关锁时修改车辆信息

                    query.Longitude = para.Longitude != null ? para.Longitude : query.Longitude;
                    query.Latitude = para.Latitude != null ? para.Latitude : query.Latitude;
                    query.Address = Address;
                    query.Voltage = Voltage != null ? Voltage : query.Voltage;    //电压
                    query.UpdateTime = dt;
                    db.SubmitChanges();

                    #endregion 开关锁时修改车辆信息

                    #endregion 调用百度API--更新车辆基本信息表

                    #region 推送给App关锁是否成功

                    if (para.LockAction == 1 && para.TypeName == "hardware") //关锁时推送
                    {
                        //Utility.Common.FileLog.Log("关锁时推送：" + usersQuery.PushId + usersQuery.UserInfoGuid, "JPushLog");
                        IJPushService _JPushService = new JPushService();
                        string Content = "关锁成功";
                        List<string> registrationList = new List<string>();
                        registrationList.Add(PersonInfo.PushId);
                        //调用极光自定义消息接口
                        string PushMessage = string.Empty;
                        string ResultStr = _JPushService.SendPostRequest(Content, registrationList);
                        JObject jo = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(ResultStr);
                        string Code = jo["error"]["code"].ToString();
                        if (Code == "200")
                        {
                            PushMessage = "自定义消息推送成功";
                            Utility.Common.FileLog.Log("设备编号：" + StrDeviceNo + ", 调用极光推送自定义消息：" + ResultStr + "，状态：已推送，PushID=" + PersonInfo.PushId + "，用户Guid" + PersonInfo.ServicePersonID, "JPushLog");
                        }
                        else
                        {
                            LogServer.InsertDBLockOC(StrDeviceNo, para.LockAction, UserGuid, para.TypeName, "极光推送出现异常");
                            PushMessage = "自定义消息推送失败";
                            Utility.Common.FileLog.Log("设备编号：" + StrDeviceNo + ", 调用极光推送自定义消息：" + ResultStr + "，状态：未推送，PushID=" + PersonInfo.PushId + "，用户Guid" + PersonInfo.ServicePersonID, "JPushLog");
                        }
                        db.SubmitChanges();
                    }

                    #endregion 推送给App关锁是否成功
                }
            }
            catch (Exception ex)
            {
                LogServer.InsertDBLockOC(StrDeviceNo, para.LockAction, UserGuid, para.TypeName, "开关锁出现异常" + ex.Message);
                result.IsSuccess = false;
                result.MsgCode = "0";
                result.Message = "开关锁异常,请重新开关锁！";
                LogHelper.Error("维修人员开关锁:" + ex.Message + ",用户Guid:" + UserGuid + ",锁的编号：" + para.DeviceNo + "", ex);
            }
            return result;
        }

        /// <summary>
        /// 添加用户开关锁交易记录信息
        /// </summary>
        /// <param name="model"></param>
        public bool AddTransactionLog(OpenOrCloseLockBicycle_PM model, DateTime dt, Guid UserInfoGuid, string StrDeviceNo, DateTime dtNow, decimal? Voltage, out string Msg, out string MsgCode)
        {
            Msg = string.Empty;
            MsgCode = string.Empty;
            string CloseTypeName = string.Empty;
            decimal? pLongitude = 0;
            decimal? pLatitude = 0;
            //车辆基本信息

            try
            {
                using (var db = new MintBicycleDataContext())
                {
                    var query = db.BicycleLockInfo.FirstOrDefault(s => s.LockNumber == StrDeviceNo && !s.IsDeleted);
                    if (query != null && model.Longitude == null && model.Latitude == null)
                    {
                        pLongitude = model.Longitude != null ? model.Longitude : query.Longitude;
                        pLatitude = model.Latitude != null ? model.Latitude : query.Latitude;
                    }
                    else if (query != null && model.Longitude == 0 && model.Latitude == 0)
                    {
                        pLongitude = model.Longitude != 0 ? model.Longitude : query.Longitude;
                        pLatitude = model.Latitude != 0 ? model.Latitude : query.Latitude;
                    }
                    else
                    {
                        pLongitude = model.Longitude;
                        pLatitude = model.Latitude;
                    }

                    #region 调用百度API

                    string Address = baiduService.GetAddress(pLongitude.ToString(), pLatitude.ToString());

                    #endregion 调用百度API

                    if (model.LockAction == 0)
                    {
                        var tranLog = new TransactionLog
                        {
                            TransactionGuid = Guid.NewGuid(),
                            UserInfoGuid = UserInfoGuid,
                            DeviceNo = StrDeviceNo,
                            OpenTypeName = "mobile",
                            OpenTimestamp = model.Timestamp,
                            OpenConvertTimestamp = dt,
                            OpenLockAction = 0,
                            OpenLatitude = pLatitude,
                            OpenLongitude = pLongitude,
                            OpenAddress = Address,
                            OpenVoltage = model.Voltage,
                            CreateTime = dtNow,
                            IsDeleted = false,
                            Status = 0 //表示开锁状态
                        };
                        db.TransactionLog.InsertOnSubmit(tranLog);
                        db.SubmitChanges();
                    }
                    else
                    {
                        if (model.TypeName == "mobile")
                        {
                            CloseTypeName = "mobile";
                        }
                        else
                        {
                            CloseTypeName = "hardware";
                        }

                        //查询记录表
                        var tranLogQuery = db.TransactionLog.OrderByDescending(s => s.CreateTime).FirstOrDefault(s => s.DeviceNo == StrDeviceNo && s.OpenLockAction == 0 && !s.IsDeleted);
                        if (tranLogQuery != null)
                        {
                            tranLogQuery.CloseTypeName = CloseTypeName;
                            tranLogQuery.CloseTimestamp = model.Timestamp;
                            tranLogQuery.CloseConvertTimestamp = dt;
                            tranLogQuery.CloseLockAction = 1;
                            tranLogQuery.CloseLatitude = pLatitude;
                            tranLogQuery.CloseLongitude = pLongitude;
                            tranLogQuery.CloseAddress = Address;
                            tranLogQuery.CloseVoltage = Voltage;
                            tranLogQuery.UpdateTime = dtNow;
                            tranLogQuery.Status = 1;  //1表示两者都有(正常)
                            db.SubmitChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("添加平台用户开关锁交易记录异常:" + ex.Message + ",时间：" + DateTime.Now + ",用户Guid:" + UserInfoGuid + ",锁的编号：" + StrDeviceNo + "", ex);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 添加维修人员开关锁交易记录信息
        /// </summary>
        /// <param name="model"></param>
        public bool AddPersonTransactionLog(OpenOrCloseLockBicycle_PM model, DateTime dt, Guid UserInfoGuid, string StrDeviceNo, DateTime dtNow, decimal? Voltage, out string Msg, out string MsgCode)
        {
            Msg = string.Empty;
            MsgCode = string.Empty;
            string CloseTypeName = string.Empty;

            using (var db = new MintBicycleDataContext())
            {
                try
                {
                    #region 调用百度API

                    string Address = baiduService.GetAddress(model.Longitude.ToString(), model.Latitude.ToString());

                    #endregion 调用百度API

                    if (model.LockAction == 0)
                    {
                        var tranLog = new PersonTransactionLog
                        {
                            TransactionGuid = Guid.NewGuid(),
                            UserInfoGuid = UserInfoGuid,
                            DeviceNo = StrDeviceNo,
                            OpenTypeName = "mobile",
                            OpenTimestamp = model.Timestamp,
                            OpenConvertTimestamp = dt,
                            OpenLockAction = 0,
                            OpenLatitude = model.Latitude,
                            OpenLongitude = model.Longitude,
                            OpenAddress = Address,
                            OpenVoltage = model.Voltage,
                            CreateTime = dtNow,
                            IsDeleted = false,
                            Status = 0 //表示开锁状态
                        };
                        db.PersonTransactionLog.InsertOnSubmit(tranLog);
                        db.SubmitChanges();
                    }
                    else
                    {
                        if (model.TypeName == "mobile")
                        {
                            CloseTypeName = "mobile";
                        }
                        else
                        {
                            CloseTypeName = "hardware";
                        }
                        //查询维修人员记录表
                        var tranLogQuery = db.PersonTransactionLog.OrderByDescending(s => s.CreateTime).FirstOrDefault(s => s.DeviceNo == model.DeviceNo && s.UserInfoGuid == model.UserInfoGuid && s.OpenLockAction == 0 && !s.IsDeleted);
                        if (tranLogQuery != null)
                        {
                            tranLogQuery.CloseTypeName = CloseTypeName;
                            tranLogQuery.CloseTimestamp = model.Timestamp;
                            tranLogQuery.CloseConvertTimestamp = dt;
                            tranLogQuery.CloseLockAction = 1;
                            tranLogQuery.CloseLatitude = model.Latitude;
                            tranLogQuery.CloseLongitude = model.Longitude;
                            tranLogQuery.CloseAddress = Address;
                            tranLogQuery.CloseVoltage = model.Voltage == null ? 0 : model.Voltage;
                            tranLogQuery.UpdateTime = dtNow;
                            tranLogQuery.Status = 1;  //1表示两者都有(正常)
                            db.SubmitChanges();
                        }
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    LogHelper.Error("添加维修人员开关锁交易记录异常:" + ex.Message + ",时间：" + DateTime.Now + ",用户Guid:" + UserInfoGuid + ",锁的编号：" + StrDeviceNo + "", ex);
                    return false;
                }
            }
        }

        /// <summary>
        /// 根据编码开锁
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public ResultModel UnLockBicycle(UnLockBicycle_PM para)
        {
            LogHelper.Info("服务器发送远程开锁：时间：" + DateTime.Now + ", 锁编号：" + para.LockNumber+"开锁用户：" + para.UserInfoGuid + "经度：" + para.Longitude + "纬度：" + para.Latitude);
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var query = db.BicycleLockInfo.FirstOrDefault(s => s.LockNumber == para.LockNumber);
                //if (query == null)
                //    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.BicycleNotExist, Message = ResPrompt.BicycleNotExistMessage };

                //调用远程开锁
                //var sk = CommonHelper.HttpGet("http://124.15.130.103:1080/MintLock/api/Lock/Open/C8FD199A6250", "");
                //var sk = CommonHelper.HttpGet("http://124.15.130.103:1080/LockWcfService/open/" + para.LockNumber + "/" + para.SecretKey +"/" + para.UserInfoGuid + "", "");
                var sk = CommonHelper.HttpGet("http://124.15.130.103:1080/LockWcfService/open/" + para.LockNumber + "/" + para.SecretKey + "", "");

                var st = JObject.Parse(sk);
                result.Message = st["Message"].Value<string>();
                var isSuccess = st["SuccessSend"].Value<bool>();
                if (!isSuccess)
                {
                    LogHelper.Error("调用服务器发送远程请求失败,返回的Message:" + result.Message);
                    result.Message = "调用服务器发送远程请求失败";
                }
                else
                {
                    LogHelper.Error("调用服务器发送远程请求成功");
                    //query.LockStatus = (int)BicycleStatusEnum.OpenLockStatus; //已开锁
                    //query.UpdateTime = DateTime.Now;
                    //if (para.UserInfoGuid != null && para.UserInfoGuid != Guid.Empty)
                    //{
                    //    var userTravel = new MyTravel
                    //    {
                    //        MyTravelGuid = Guid.NewGuid(),
                    //        LockGuid = query.LockGuid,
                    //        UserInfoGuid = para.UserInfoGuid,
                    //        StartLongitude = para.Longitude,
                    //        StartLatitude = para.Latitude,
                    //        StartTime = DateTime.Now,
                    //        CreateTime = DateTime.Now,
                    //        IsDeleted = false,
                    //    };
                    //    db.MyTravel.InsertOnSubmit(userTravel);
                    //}
                    result.Message = "调用服务器发送远程请求成功";
                }
                result.ResObject = isSuccess;
            }
            return result;
        }

        /// <summary>
        /// 服务器查询开锁状态
        /// </summary>
        /// <param name="deviceNo"></param>
        /// <returns></returns>
        public ResultModel QueryUnLockBicycleStatus(string deviceNo)
        {
            if (string.IsNullOrWhiteSpace(deviceNo))
                return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.BicycleNotExist, Message = ResPrompt.BicycleNotExistMessage };
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var query = db.BicycleLockInfo.FirstOrDefault(s => s.LockNumber == deviceNo && !s.IsDeleted);
                if (query == null)
                    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.BicycleNotExist, Message = ResPrompt.BicycleNotExistMessage };
                if (query.LockStatus != (int)BicycleStatusEnum.OpenLockStatus) //表示已开锁
                    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.BicycleUnlockError, Message = ResPrompt.BicycleUnlockErrorMessage };
                var qk = db.MyTravel.OrderByDescending(s => s.CreateTime).FirstOrDefault(s => !s.IsDeleted && s.LockGuid == query.LockGuid);
                if (qk == null)
                    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.BicycleUnlockError, Message = ResPrompt.BicycleUnlockErrorMessage };
                qk.StartTime = DateTime.Now;
                qk.UpdateTime = DateTime.Now;
                db.SubmitChanges();
                result.ResObject = true;
            }
            return result;
        }

        /// <summary>
        /// 把时间戳转换成Datetime格式
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        private DateTime GetTime(string timeStamp)
        {
            try
            {
                DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                long lTime = long.Parse(timeStamp + "0000000");
                TimeSpan toNow = new TimeSpan(lTime);
                return dtStart.Add(toNow);
            }
            catch (Exception ex)
            {
            }
            return DateTime.Now;
        }

        #endregion 关锁相关方法

        #region 预约用车相关方法

        /// <summary>
        /// 根据锁编号或者车辆编号查询车辆类型
        /// </summary>
        /// <param name="LockNumber"></param>
        /// <returns></returns>
        public int GetBicycleTypeNameBy(string LockNumber)
        {
            int TypeID = 0;

            using (var db = new MintBicycleDataContext())
            {
                //查询单车Guid
                var bicycle = db.BicycleBaseInfo.FirstOrDefault(b => b.LockNumber == LockNumber || b.BicycleNumber == LockNumber && !b.IsDeleted);
                if (bicycle != null)
                {
                    TypeID = int.Parse(bicycle.BicyCleTypeName.ToString());
                }
                else
                {
                    TypeID = -1;
                }
            }
            return TypeID;
        }

        /// <summary>
        /// 添加预约用车
        /// </summary>
        /// <param name="model"></param>
        public ResultModel AddReservation(AddRegionInfo_PM model)
        {
            var result = new ResultModel();
            DateTime dt = DateTime.Now;
            Guid BicycleBaseGuid = new Guid("{00000000-0000-0000-0000-000000000000}");
            string LockNumber = string.Empty;
            string errorMsg = string.Empty;
            StringBuilder strSql = new StringBuilder();
            string bicycleTypeName = string.Empty;
            decimal electricQuantity = 0;   //按小数
            String ElectricQuantityDesc = String.Empty;
            string DistanceDesc = string.Empty;
            int ElectricQuantityStatus = 0;

            try
            {
                using (var db = new MintBicycleDataContext())
                {
                    #region 处理业务

                    if (model.TypeName==1)
                    {
                        var bicQuery = db.BicycleBaseInfo.FirstOrDefault(s => s.BicycleNumber == model.DeviceNo && !s.IsDeleted);
                        if (bicQuery != null && bicQuery.BicycleNumber != null && bicQuery.LockNumber != null)
                        {
                            LockNumber = bicQuery.LockNumber;
                        }
                        else
                        {
                            return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.BicycleLockNotExist, Message = ResPrompt.BicycleLockNotExistMessage };
                        }
                    }

                    if (model.TypeName == 0 && model.DeviceNo == null && model.ElectronicFenCingGuid != null)
                    {
                        #region  0表示电子围栏内,在添加预约用车时系统给自动分配一辆单车

                        strSql.Append("select top 1 b.LockGuid,b.LockNumber,b.LockStatus from BicycleLockInfo as b  ");
                        strSql.Append(" inner join  BicycleBaseInfo as c on b.LockNumber =c.LockNumber ");
                        if (model.BicyCleTypeName == 0) //非助力车
                        {
                            strSql.Append(" where  c.BicyCleTypeName=0 ");
                        }
                        else
                        {
                            strSql.Append(" where  c.BicyCleTypeName=1 ");
                        }

                        strSql.Append(" and b.LockStatus=1   and b.ElectronicFenCingGuid='" + model.ElectronicFenCingGuid + "' ");
                        strSql.Append(" and b.LockGuid not in(select r.BicycleBaseGuid from Reservation  as r where r.Status=1)  order by b.UpdateTime asc  ");
                        //方法：如果电子围栏内有车的情况下，会优先分配电子围栏范围内的车辆
                        //如果电子围栏内没有车辆,在App端会提示用户：电子围栏内车辆为0时提示：“当前区域暂无车辆，无法预约”，并且不调用接口
                        //strSql = "select top 1 b.LockGuid,b.LockNumber,b.LockStatus from BicycleLockInfo as b where b.LockStatus=1 and b.ElectronicFenCingGuid='" + model.ElectronicFenCingGuid + "' and b.LockGuid not in(select r.BicycleBaseGuid from Reservation  as r where r.Status=1)  order by b.CreateTime asc  ";

                        DataSet ds = DbHelperSQL.Query(strSql.ToString());
                        if (ds != null && ds.Tables[0].Rows.Count > 0) //如果查询返回false时，表示没有查询到数据，说明在预约表中已存在了预约的车辆
                        {
                            //判断车辆是否存在
                            result = CheckBicycle(ds.Tables[0].Rows[0]["LockNumber"].ToString());
                            if (!result.IsSuccess)
                            {
                                LogServer.InsertDBReservation(model.UserGuid, model.DeviceNo, 0, result.Message);
                                return result;
                            }
                            BicycleBaseGuid = new Guid(ds.Tables[0].Rows[0]["LockGuid"].ToString());
                            LockNumber = ds.Tables[0].Rows[0]["LockNumber"].ToString();
                            //判断车和锁是否匹配
                            bool flg = GetBicycleLockByNumber(LockNumber);
                            if (!flg)
                            {
                                return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.BicycleLockNotExist, Message = ResPrompt.BicycleLockNotExistMessage };
                            }
                        }
                        else
                        {
                            if (model.BicyCleTypeName == 0)
                            {
                                LogServer.InsertDBReservation(model.UserGuid, model.DeviceNo, 0, "暂无非助力车，请选择其它车型");
                                return new ResultModel { IsSuccess = false, MsgCode = "0", Message = "暂无非助力车，请选择其它车型！" };
                            }
                            else
                            {
                                LogServer.InsertDBReservation(model.UserGuid, model.DeviceNo, 0, "暂无助力车，请选择其它车型");
                                return new ResultModel { IsSuccess = false, MsgCode = "0", Message = "暂无助力车，请选择其它车型！" };
                            }
                        }
                        ////修改车辆信息表的时间
                        //var bicQuery = db.BicycleLockInfo.FirstOrDefault(s => s.LockNumber == LockNumber && s.LockGuid == BicycleBaseGuid);
                        //if (bicQuery != null)
                        //{
                        //    bicQuery.UpdateTime = DateTime.Now;
                        //    bicQuery.ReservationStatus = 1;  //已预约
                        //    db.SubmitChanges();
                        //}

                        #endregion 
                    }
                    else //电子围栏外车辆预约
                    {
                        //查看单车是否可用
                        result = CheckBicycle(LockNumber);
                        if (!result.IsSuccess)
                        {
                            LogServer.InsertDBReservation(model.UserGuid, LockNumber, 0, result.Message);
                            return result;
                        }
                        //查询单车Guid
                        var bicycle = db.BicycleLockInfo.FirstOrDefault(b => b.LockNumber == LockNumber);
                        if (bicycle != null)
                        {
                            BicycleBaseGuid = bicycle.LockGuid; //锁编号
                        }
                        //LockNumber = model.DeviceNo;
                    }

                    #region 调用百度API

                    string Address = baiduService.GetAddress(model.Longitude.ToString(), model.Latitude.ToString());

                    #endregion 调用百度API

                    //添加预约信息
                    var reservation = new Reservation
                    {
                        ReservationGuid = Guid.NewGuid(),
                        BicycleBaseGuid = BicycleBaseGuid,
                        UserInfoGuid = model.UserGuid,
                        DeviceNo = LockNumber,
                        BicyCleTypeName = BicycleBaseInfoShow.GetBicycleTypeName(model.BicyCleTypeName),
                        Address = Address,
                        Longitude = model.Longitude,
                        Latitude = model.Latitude,
                        Status = 1,      //表示已预约中
                        StartTme = dt,
                        CreateBy = model.UserGuid,
                        CreateTime = dt,
                        IsDeleted = false
                    };
                    db.Reservation.InsertOnSubmit(reservation);
                    db.SubmitChanges();

                    #region 预约成功返回预约信息

                    if (reservation.ReservationGuid != null) //表示预约成功
                    {
                        //根据锁编号或者车辆编号查询车辆类型
                        int bicTypeID = GetBicycleTypeNameBy(LockNumber);
                        if (bicTypeID == 0) //非助力车
                        {
                            bicycleTypeName = BicycleBaseInfoShow.GetBicycleTypeName(bicTypeID);
                            ElectricQuantityStatus = 0;
                            ElectricQuantityDesc = null;
                        }
                        else if (bicTypeID == 1) //助力车
                        {
                            bicycleTypeName = BicycleBaseInfoShow.GetBicycleTypeName(bicTypeID);

                            var bicNumber = db.BicycleBaseInfo.FirstOrDefault(s=>s.LockNumber== LockNumber && !s.IsDeleted);

                            //根据锁编号获取电量值
                            electricQuantity = GetElectricQuantityByLockNumber(LockNumber);
                            //根据电量获取电量描述
                            //ElectricQuantityDesc = BicycleBaseInfoShow.GetElectricQuantity(electricQuantity);
                            BicElectricModel emodel = BicycleBaseInfoShow.GetElectricQuantity(electricQuantity);
                            ElectricQuantityStatus = emodel.ElectricQuantityStatus;
                            ElectricQuantityDesc = emodel.ElectricQuantityDesc;
                        }

                        DateTime StartTme = DateTime.Parse(reservation.StartTme.ToString());
                        result.ResObject = new ReservationBicycleSucess_OM
                        {
                            ReservationGuid = reservation.ReservationGuid,               //预约的Guid编号
                            DeviceNo = reservation.DeviceNo,                            //锁的编号
                            //StartTme = StartTme.ToString("yyyy-MM-dd HH:mm:ss"),     //预约的时间
                            CountDownTime = 1200,                                      //预约倒计时默认20分钟，1200秒
                            Status = GetReservationState(reservation.Status),         //预约状态
                            Address = Address == null ? "" : Address,                 //详细地址
                            BicycleLongitude = reservation.Longitude,                  //经度
                            BicycleLatitude = reservation.Latitude,                   //纬度

                            BicycleTypeName = bicycleTypeName,                        //车辆类型
                            ElectricQuantityStatus = ElectricQuantityStatus,          //电量状态
                            ElectricQuantityDesc = ElectricQuantityDesc,              //电量描述
                        };
                        result.IsSuccess = true;
                        result.MsgCode = "0";
                        result.Message = "预约用车成功！";
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.MsgCode = "0";
                        result.Message = "预约用车失败！";
                    }

                    #endregion 预约成功返回预约信息

                    #endregion 处理业务
                }
            }
            catch (Exception ex)
            {
                LogServer.InsertDBReservation(model.UserGuid, model.DeviceNo, 0, "添加预约用车出现异常");
                result.IsSuccess = false;
                result.MsgCode = "0";
                result.Message = "添加预约用车出现异常，请重新换一辆车骑行!";
                LogHelper.Error("添加预约用车出现异常:" + ex.Message + "，用户的Guid:" + model.UserGuid + ",锁的编号：" + model.DeviceNo, ex);
            }
            return result;
        }

        /// <summary>
        /// 处理某用户预约超时，如果预约超时20分钟后，修改预约状态为2已结束
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel UserReservationOvertime(Guid userInfoGuid)
        {
            DateTime dtNow = DateTime.Now;
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                //查询预约是否超时
                var reservation = db.Reservation.OrderByDescending(r => r.CreateTime).FirstOrDefault(r => r.UserInfoGuid == userInfoGuid && r.Status == 1);
                if (reservation != null)
                {
                    // 计算两个时间的差值,取分钟--20分钟倒计时处理业务逻辑
                    int Mintues = GetTotalMinutes(reservation.StartTme);
                    if (Mintues > 20)
                    {
                        reservation.Status = 2; //预约结束
                        reservation.EndTime = dtNow;
                        reservation.UpdateBy = userInfoGuid;
                        reservation.UpdateTime = dtNow;
                        db.SubmitChanges();
                        result.IsSuccess = true;
                        result.Message = "修改预约状态成功";
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 根据锁编号获取电量值
        /// </summary>
        /// <param name="LockNumber"></param>
        /// <returns></returns>
        public decimal GetElectricQuantityByLockNumber(string LockNumber)
        {
            decimal Electricity = 0;
            decimal Capacity = 0;
            decimal electricQuantity = 0;
            Random ra = new Random();

            try
            {
                using (var db = new MintBicycleDataContext())
                {
                    var BicRelevance = db.Bicycle_Controller_Battery_relevance.FirstOrDefault(s => s.LockNumber == LockNumber && !s.IsDeleted);
                    if (BicRelevance != null)
                    {
                        Electricity = BicRelevance.Electricity;      //电池剩余容量，单位: mAh
                        Capacity = BicRelevance.Capacity;           //电池满充容量, 单位: mAh

                        if (Electricity != 0 && Capacity != 0)
                        {
                            electricQuantity = Math.Round(Electricity / Capacity, 2); //计算电量
                            //electricQuantity = Math.Round(decimal.Parse(ra.Next(10, 100).ToString()), 2) / 100;
                        }
                        else
                        {
                            electricQuantity = 0;
                        }
                    }
                    else
                    {
                        electricQuantity = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("根据锁编号获取电量值:" + ex.Message + ",锁的编号：" + LockNumber, ex);
            }
            return electricQuantity;
        }

        /// <summary>
        /// 获取用户的预约倒计时信息
        /// </summary>
        /// <param name="model"></param>
        public ResultModel GetCountDownTimeByUserGuid(Guid? userInfoGuid)
        {
            var result = new ResultModel();
            int SecondsTime = 0;
            string bicycleTypeName = string.Empty;
            decimal electricQuantity = 0;   //电量按小数
            String ElectricQuantityDesc = String.Empty;
            int ElectricQuantityStatus = 0;

            using (var db = new MintBicycleDataContext())
            {
                //1.当返回值Status =”已结束”时，客户端要隐藏掉预约弹出层，否则显示。
                //2.新增了返回字段OpenOrCloseDeviceNo，表示获取当前用户的开关锁情况。当OpenOrCloseDeviceNo = 0表示关锁，OpenOrCloseDeviceNo不等于0时表示开锁

                #region 1.处理用户预约倒计时信息

                try
                {
                    var query = db.Reservation.OrderByDescending(r => r.CreateTime).FirstOrDefault(r => r.UserInfoGuid == userInfoGuid);
                    if (query != null)
                    {
                        #region 查询车辆类型

                        int bicTypeID = GetBicycleTypeNameBy(query.DeviceNo);
                        if (bicTypeID == 0)
                        {
                            bicycleTypeName = BicycleBaseInfoShow.GetBicycleTypeName(bicTypeID);
                            ElectricQuantityDesc = null;
                        }
                        else if (bicTypeID == 1) //助力车
                        {
                            bicycleTypeName = BicycleBaseInfoShow.GetBicycleTypeName(bicTypeID);
                            //根据锁编号获取电量值
                            electricQuantity = GetElectricQuantityByLockNumber(query.DeviceNo);
                            //根据电量获取电量描述
                            //ElectricQuantityDesc = BicycleBaseInfoShow.GetElectricQuantity(electricQuantity);
                            BicElectricModel model = BicycleBaseInfoShow.GetElectricQuantity(electricQuantity);
                            ElectricQuantityStatus = model.ElectricQuantityStatus;
                            ElectricQuantityDesc = model.ElectricQuantityDesc;
                        }

                        #endregion 查询车辆类型

                        TimeSpan ts = Convert.ToDateTime(DateTime.Now) - Convert.ToDateTime(query.StartTme);
                        SecondsTime = 20 * 60 - Convert.ToInt32(ts.TotalSeconds);  //秒数为负数时，表示预约超时对应的状态是已结束
                        if (SecondsTime < 0 && query.Status == 2)
                        {
                            //处理某用户预约超时，如果预约超时20分钟后，修改预约状态为2已结束
                            result = UserReservationOvertime(userInfoGuid.Value);
                            result.IsSuccess = true;
                            result.Message = "您预约用车已超时，请重新预约用车";
                            result.ResObject = new ReservationBicycleSucess_OM
                            {
                                ReservationGuid = query.ReservationGuid,                     //预约的Guid编号
                                DeviceNo = query.DeviceNo,                                  //设备编号
                                                                                            //StartTme = StartTme.ToString("yyyy-MM-dd HH:mm:ss"),      //预约的时间
                                CountDownTime = SecondsTime,                                //剩余的秒数
                                Status = "已结束",                                           //预约状态
                                Address = query.Address == null ? "" : query.Address,         //详细地址
                                BicycleLongitude = query.Longitude,                           //经度
                                BicycleLatitude = query.Latitude,                            //纬度
                            };
                        }
                        else if (query.Status == 0)
                        {
                            result.IsSuccess = true;
                            result.Message = "您已取消预约用车";
                            result.ResObject = new ReservationBicycleSucess_OM
                            {
                                ReservationGuid = query.ReservationGuid,                     //预约的Guid编号
                                DeviceNo = query.DeviceNo,                                  //设备编号
                                                                                            //StartTme = StartTme.ToString("yyyy-MM-dd HH:mm:ss"),      //预约的时间
                                CountDownTime = SecondsTime,                                //秒数
                                Status = GetReservationState(query.Status),                //预约状态
                                Address = query.Address == null ? "" : query.Address,      //详细地址
                                BicycleLongitude = query.Longitude,                       //经度
                                BicycleLatitude = query.Latitude,                        //纬度
                            };
                        }
                        else if (query.Status == 1)
                        {
                            result.IsSuccess = true;
                            result.Message = "您正在预约用车中";
                            result.ResObject = new ReservationBicycleSucess_OM
                            {
                                ReservationGuid = query.ReservationGuid,                     //预约的Guid编号
                                DeviceNo = query.DeviceNo,                                  //设备编号
                                                                                            //StartTme = StartTme.ToString("yyyy-MM-dd HH:mm:ss"),      //预约的时间
                                CountDownTime = SecondsTime,                                //秒数
                                Status = GetReservationState(query.Status),                //预约状态
                                Address = query.Address == null ? "" : query.Address,      //详细地址
                                BicycleLongitude = query.Longitude,                       //经度
                                BicycleLatitude = query.Latitude,                        //纬度

                                ElectricQuantityStatus = ElectricQuantityStatus,          //电量状态
                                BicycleTypeName = bicycleTypeName,                        //车辆类型
                                ElectricQuantityDesc = ElectricQuantityDesc,              //电量描述
                                                                                          //DistanceDesc = "预计助力10公里",                         //电量充足时预约返回助力的距离
                            };
                        }
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = "无此用户的预约用车数据！";
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error("处理用户预约倒计时信息异常:" + ex.Message + ",用户Guid：" + userInfoGuid, ex);
                    result.IsSuccess = false;
                    result.Message = "处理用户预约倒计时信息异常！";
                }

                #endregion 1.处理用户预约倒计时信息
            }
            return result;
        }

        /// <summary>
        /// 是否骑行中
        /// </summary>
        /// <param name="userInfoGuid"></param>
        public ResultModel GetIsRidingByUserGuid(Guid? userInfoGuid)
        {
            var result = new ResultModel();
            int? CyclingMode = 3;
            if (userInfoGuid == null)
            {
                return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ParaModelNotExist, Message = ResPrompt.ParaModelNotExistMessage };
            }

            try
            {
                using (var db = new MintBicycleDataContext())
                {
                    //查询某用户开关锁的锁的编号信息
                    var BicQuery = (from u in db.UserInfo
                                        //join b in db.BicycleLockInfo on u.LockGuid equals b.LockGuid
                                    join b in db.BicycleBaseInfo on u.LockGuid equals b.LockGuid
                                    where u.UserInfoGuid == userInfoGuid
                                    orderby u.CreateTime descending
                                    select new
                                    {
                                        LockNumber = b.LockNumber,
                                        BicyCleNumber = b.BicycleNumber
                                    }).FirstOrDefault();

                    if (BicQuery != null)
                    {
                        var MyTravel = db.MyTravel.OrderByDescending(s => s.CreateTime).FirstOrDefault(s => s.UserInfoGuid == userInfoGuid && !s.IsDeleted);
                        if (MyTravel != null)
                        {
                            CyclingMode = MyTravel.CyclingMode;
                        }
                        result.ResObject = new IsRidingModel_OM
                        {
                            OpenOrCloseDeviceNo = BicQuery.LockNumber,      //开关锁的设编号
                            ElectricQuantity = GetElectricQuantityByLockNumber(BicQuery.LockNumber),  //根据锁编号获取电量值
                            CyclingMode = CyclingMode          //骑行模式
                        };
                        result.IsSuccess = true;
                        result.Message = "获取设备编号成功";
                    }
                    else
                    {
                        result.ResObject = new IsRidingModel_OM
                        {
                            OpenOrCloseDeviceNo = "0"    //开关锁的设备编号
                        };
                        result.IsSuccess = false;
                        result.Message = "暂无设备编号";
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("查询是否骑行中出现异常:" + ex.Message + ",用户Guid：" + userInfoGuid, ex);
            }

            return result;
        }

        /// <summary>
        /// 取消预约
        /// </summary>
        /// <param name="reservationGuid"></param>
        public ResultModel CancelReservation(UpdateReservation_PM model)
        {
            DateTime dtNow = DateTime.Now;
            var result = new ResultModel();

            try
            {
                //检查预约是否存在
                bool flg = CheckReservationExist(model.ReservationGuid);
                if (!flg)
                {
                    result.IsSuccess = false;
                    result.Message = "预约不存在";
                }
                using (var db = new MintBicycleDataContext())
                {
                    var reservation = db.Reservation.FirstOrDefault(r => r.ReservationGuid == model.ReservationGuid && r.UserInfoGuid == model.UserGuid);
                    if (reservation != null && reservation.BicycleBaseGuid != null)
                    {
                        reservation.Status = 0; //
                        reservation.EndTime = dtNow;
                        reservation.UpdateBy = model.UserGuid;
                        reservation.UpdateTime = dtNow;
                        db.SubmitChanges();

                        result.IsSuccess = true;
                        result.Message = "取消预约成功";
                        //修改预约状态为未预约
                        var bicycle = db.BicycleLockInfo.FirstOrDefault(b => b.LockNumber == reservation.DeviceNo);
                        if (bicycle != null)
                        {
                            db.SubmitChanges();
                        }
                    }
                    else
                    {
                        LogServer.InsertDBReservation(model.UserGuid, reservation.DeviceNo, 1, "取消预约失败，用户预约不存在");
                        result.IsSuccess = false;
                        result.Message = "取消预约失败";
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("取消预约出现异常:" + ex.Message + ",用户Guid：" + model.UserGuid + "预约Guid:" + model.ReservationGuid, ex);
                result.IsSuccess = false;
                result.Message = "取消预约出现异常,请联系客服！";
            }
            return result;
        }

        /// <summary>
        /// 如果倒计时20分钟后，APP调用此接口修改预约状态为已结束[暂时废弃]
        /// </summary>
        /// <param name="model"></param>
        public ResultModel ReservationOvertimeUpdateStatus(UpdateOvertime_PM model)
        {
            var result = new ResultModel();
            //检查预约是否存在
            bool flg = CheckReservationExist(model.ReservationGuid);
            if (!flg)
            {
                result.IsSuccess = false;
                result.Message = "预约不存在";
            }
            using (var db = new MintBicycleDataContext())
            {
                var reservation = db.Reservation.FirstOrDefault(r => r.ReservationGuid == model.ReservationGuid && r.UserInfoGuid == model.UserGuid);
                if (reservation != null && reservation.BicycleBaseGuid != null)
                {
                    reservation.Status = 2; //已结束
                    reservation.UpdateTime = DateTime.Now;
                    reservation.UpdateBy = model.UserGuid;
                    db.SubmitChanges();
                    result.IsSuccess = true;
                    result.Message = "修改预约状态成功";

                    var bicycle = db.BicycleLockInfo.FirstOrDefault(b => b.LockNumber == reservation.DeviceNo);
                    if (bicycle != null)
                    {
                        db.SubmitChanges();
                    }
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = "修改预约状态失败";
                }
            }
            return result;
        }

        /// <summary>
        /// 查看单车是否可用--用于用户输入编码或者扫码开锁
        /// </summary>
        /// <param name="deviceNo"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public ResultModel CheckBicycleAvailable(string deviceNo, Guid userInfoGuid)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                #region 查询单车是否存在

                var queryB = db.BicycleLockInfo.FirstOrDefault(s => s.LockNumber == deviceNo && !s.IsDeleted);
                if (queryB == null)
                {
                    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.BicycleNotExist, Message = ResPrompt.BicycleNotExistMessage };
                }
                else
                {
                    if (queryB.LockStatus == 0)
                    {
                        return new ResultModel { IsSuccess = false, MsgCode = "0", Message = "此单车已开锁,您不能开锁" };
                    }
                    if (queryB.LockStatus == 2)
                    {
                        return new ResultModel { IsSuccess = false, MsgCode = "0", Message = "单车目前发生故障，请换一辆单车骑行" };
                    }
                    if (queryB.LockStatus == 3)
                    {
                        return new ResultModel { IsSuccess = false, MsgCode = "0", Message = "单车目前电量不足，请换一辆单车骑行" };
                    }
                }

                #endregion 查询单车是否存在

                #region //查询预约情况

                //在开锁时判断此车是否被别的用户预约过，预约过就不能开锁
                var IsQuery = db.Reservation.FirstOrDefault(s => s.DeviceNo == deviceNo && s.Status == 1);
                if (IsQuery != null && IsQuery.UserInfoGuid != userInfoGuid)
                {
                    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ReservationExist, Message = ResPrompt.ReservationExistMessage };
                }
                var IsQuery1 = db.Reservation.FirstOrDefault(s => s.DeviceNo == deviceNo && s.Status == 3);
                if (IsQuery1 != null)
                {
                    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.BicycleQiExist, Message = ResPrompt.BicycleQiExistMessage };
                }

                #endregion //查询预约情况
            }

            return result;
        }

        /// <summary>
        /// 查看单车是否可用--用于添加预约用车
        /// </summary>
        /// <param name="deviceNo"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public ResultModel CheckBicycle(string deviceNo)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                #region 查询单车是否存在

                var queryB = db.BicycleLockInfo.FirstOrDefault(s => s.LockNumber == deviceNo && !s.IsDeleted);
                if (queryB == null)
                {
                    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.BicycleNotExist, Message = ResPrompt.BicycleNotExistMessage };
                }
                else
                {
                    if (queryB.LockStatus == 0)
                    {
                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ReservationNotError, Message = ResPrompt.ReservationNotMessage };
                    }
                    if (queryB.LockStatus == 2)
                    {
                        return new ResultModel { IsSuccess = false, MsgCode = "0", Message = "单车目前发生故障，请换一辆单车骑行" };
                    }
                    if (queryB.LockStatus == 3)
                    {
                        return new ResultModel { IsSuccess = false, MsgCode = "0", Message = "单车目前电量不足，请换一辆单车骑行" };
                    }
                }

                #endregion 查询单车是否存在

                #region //查询预约情况

                //在开锁时判断此车是否被别的用户预约过，预约过就不能预约用车
                var IsQuery = db.Reservation.FirstOrDefault(s => s.DeviceNo == deviceNo && s.Status == 1);
                if (IsQuery != null)
                {
                    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.BicycleReservationExist, Message = ResPrompt.BicycleReservationExistMessage };
                }
                var IsQuery1 = db.Reservation.FirstOrDefault(s => s.DeviceNo == deviceNo && s.Status == 3);
                if (IsQuery1 != null)
                {
                    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ReservaExist, Message = ResPrompt.ReservaExistMessage };
                }

                #endregion //查询预约情况
            }

            return result;
        }

        /// <summary>
        /// 预约状态处理
        /// </summary>
        /// <param name="st"></param>
        /// <returns></returns>
        public static string GetReservationState(int? st)
        {
            //状态:0取消预约；1预约中；2已结束；3骑行中
            var str = string.Empty;
            switch (st)
            {
                case 0:
                    str = "取消预约";
                    break;

                case 1:
                    str = "预约中";
                    break;

                case 2:
                    str = "已结束";
                    break;

                case 3:
                    str = "骑行中";
                    break;

                default:
                    str = "未知状态";
                    break;
            }
            return str;
        }

        /// <summary>
        /// 检查预约用户是否已经有其他预约
        /// </summary>
        /// <param name="userGuid"></param>
        /// <returns></returns>
        public bool CheckUserReservationExist(Guid userGuid)
        {
            using (var db = new MintBicycleDataContext())
            {
                var query = db.Reservation.OrderByDescending(r => r.CreateTime).FirstOrDefault(r => r.UserInfoGuid == userGuid);
                return query != null && query.Status == 1; //预约中
            }
        }

        /// <summary>
        /// 查询车辆是否已被预约
        /// </summary>
        /// <param name="DeviceNo"></param>
        /// <returns></returns>
        public ResultModel GetIsUserReservation(string DeviceNo, Guid UserGuid)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                //在开锁时判断此车是否被别的用户预约过，预约过就不能开锁
                var IsQuery = db.Reservation.FirstOrDefault(s => s.DeviceNo == DeviceNo && s.Status == 1);
                if (IsQuery != null && IsQuery.UserInfoGuid != UserGuid)
                {
                    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ReservationExist, Message = ResPrompt.ReservationExistMessage };
                }
                if (IsQuery != null && IsQuery.UserInfoGuid == UserGuid && IsQuery.DeviceNo == DeviceNo) //表示自己预约的车辆
                {
                    return new ResultModel { IsSuccess = true, MsgCode = "0", Message = "您本人预约的车辆可以获取密钥串开锁" };
                }
            }
            return result;
        }

        /// <summary>
        /// 检查预约是否存在
        /// </summary>
        /// <param name="reservationGuid"></param>
        /// <returns></returns>
        public bool CheckReservationExist(Guid reservationGuid)
        {
            using (var db = new MintBicycleDataContext())
            {
                var reservation = db.Reservation.FirstOrDefault(r => r.ReservationGuid == reservationGuid);
                return reservation != null && reservation.Status == 1;
            }
        }

        /// <summary>
        /// 用户预约的次数
        /// </summary>
        /// <param name="userGuid"></param>
        /// <returns></returns>
        public int GetCountByUserInfoGuid(Guid userInfoGuid)
        {
            int count = 0;
            using (var db = new MintBicycleDataContext())
            {
                //string sql = "select count(*) as counts from Reservation where UserInfoGuid='"+ userInfoGuid + "'";
                count = db.Reservation.Where(s => s.UserInfoGuid == userInfoGuid).ToList().Count();
                //var count = db.Reservation.FirstOrDefault(s => s.UserInfoGuid == userInfoGuid).Count();
                //预约中
            }
            return count;
        }

        /// <summary>
        /// 电子围栏中的车辆数
        /// </summary>
        /// <param name="ElectronicFenCingGuid"></param>
        /// <returns></returns>
        public int GetBicCountByElectronicGuid(Guid ElectronicFenCingGuid)
        {
            int count = 0;
            using (var db = new MintBicycleDataContext())
            {
                count = db.BicycleLockInfo.Where(s => s.ElectronicFenCingGuid == ElectronicFenCingGuid).ToList().Count();
            }
            return count;
        }

        /// <summary>
        /// 判断输入车辆编号或者扫码开锁
        /// </summary>
        /// <param name="model"></param>
        public ResultModel CheckInputOrScan(InputOrScan_PM model)
        {
            var result = new ResultModel();

            try
            {
                using (var db = new MintBicycleDataContext())
                {
                    //查看单车是否可用
                    result = CheckBicycleAvailable(model.DeviceNo, model.UserGuid);
                    if (!result.IsSuccess)
                    {
                        return result;
                    }

                    #region 判断输入车辆编号或者扫码开锁

                    //查询预约
                    var IsQuery = db.Reservation.OrderByDescending(s => s.CreateTime).FirstOrDefault(s => s.UserInfoGuid == model.UserGuid && !s.IsDeleted);
                    //表示输入或者扫码开锁--表示您已预约了车辆。先取消，在扫码。
                    if (IsQuery != null)
                    {
                        if (IsQuery.Status == 1 && IsQuery.DeviceNo != model.DeviceNo)
                        {
                            return new ResultModel { IsSuccess = false, MsgCode = "0", Message = "非预约车辆，请先取消预约,再扫码开锁" };
                        }
                        else if ((IsQuery.Status == 0 || IsQuery.Status == 2) && IsQuery.DeviceNo != model.DeviceNo)
                        {
                            //没有预约车辆直接输入编码或者扫码开锁
                            return new ResultModel { IsSuccess = true, MsgCode = "0", Message = "输入编码或者扫码开锁成功" };
                        }
                        else if (IsQuery.DeviceNo == model.DeviceNo && IsQuery.Status == 1 && IsQuery.UserInfoGuid == model.UserGuid)
                        {
                            //预约车辆A，扫码开车辆A，开锁成功。
                            return new ResultModel { IsSuccess = true, MsgCode = "0", Message = "输入编码或者扫码开锁成功" };
                        }
                    }
                    else
                    {
                        return new ResultModel { IsSuccess = true, MsgCode = "0", Message = "直接输入编码或者扫码开锁成功" };
                    }

                    //return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ReservationExist, Message = ResPrompt.ReservationExistMessage };

                    #endregion 判断输入车辆编号或者扫码开锁
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("输入车辆编号或者扫码开锁异常:" + ex.Message + ",锁的编号：" + model.DeviceNo + "用户Guid:" + model.UserGuid, ex);
                result.IsSuccess = false;
                result.Message = "输入车辆编号或者扫码开锁异常,请换一辆车开锁！";
            }
            return result;
        }

        /// <summary>
        /// 服务器端定时处理预约超过20分钟的数据
        /// 把status修改为2已结束
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public ResultModel ReservationTimedTask()
        {
            var result = new ResultModel();

            using (var db = new MintBicycleDataContext())
            {
                //处理预约是否超时
                var ReservationQuery = db.Reservation.OrderByDescending(s => s.CreateTime).Where(s => s.Status == 1).Select(s => s).ToList();

                #region 修改预约的状态为已结束

                if (ReservationQuery != null) //说明此用户用手机预约了单车，否则直接扫码骑行单车
                {
                    foreach (var item in ReservationQuery)
                    {
                        try
                        {
                            // 计算两个时间的差值,取分钟--20分钟倒计时处理业务逻辑
                            int Mintues = GetTotalMinutes(item.StartTme);
                            if (Mintues > 20)
                            {
                                //处理预约是否超时
                                var Query = db.Reservation.OrderByDescending(s => s.CreateTime).FirstOrDefault(s => s.ReservationGuid == item.ReservationGuid);

                                //修改预约的状态为已结束
                                if (Query != null) //说明此用户用手机预约了单车，否则直接扫码骑行单车
                                {
                                    Query.Status = 2; //预约结束
                                    Query.EndTime = DateTime.Now;
                                    Query.UpdateBy = null;
                                    Query.UpdateTime = DateTime.Now;
                                    db.SubmitChanges();
                                }
                                //修改
                                //var bicycle = db.BicycleLockInfo.FirstOrDefault(b => b.LockNumber == Query.DeviceNo);
                                //if (bicycle != null)
                                //{
                                //    db.SubmitChanges();
                                //}
                                //日志记录
                                // Utility.Common.FileLog.Log("批量处理预约状态成功,当前创建时间：" + DateTime.Now + ",预约Guid:" + item.ReservationGuid + ",设备编号：" + item.DeviceNo + ",用户Guid:" + item.UserInfoGuid + "", "ReservationTimedTaskLog");
                            }
                        }
                        catch (Exception ex)
                        {
                            LogHelper.Error("服务器自动处理预约状态异常:" + ex.Message + ",当前创建时间：" + DateTime.Now + ",预约Guid:" + item.ReservationGuid + ",设备编号：" + item.DeviceNo + ",用户Guid:" + item.UserInfoGuid + "", ex);
                            result.IsSuccess = false;
                            result.Message = "服务器自动处理预约状态异常";
                        }
                    }
                    result.IsSuccess = true;
                    result.Message = "批量处理预约状态成功";
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = "暂时没有预约超时要处理的数据";
                    //日志记录
                    Utility.Common.FileLog.Log("批量处理预约状态，暂时没有预约超时要处理的数据", "ReservationTimedTaskLog");
                }

                #endregion 修改预约的状态为已结束
            }

            return result;
        }

        #endregion 预约用车相关方法

        #region 测试极光推送

        /// <summary>
        /// 测试某用户关锁流程
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public ResultModel HardwareOpenOrClosekLock_Test(Test_OpenOrCloseLockBicycle_PM para)
        {
            var result = new ResultModel();
            string PushId = string.Empty;

            using (var db = new MintBicycleDataContext())
            {
                //查询用户表
                var usersQuery = db.UserInfo.FirstOrDefault(s => s.UserInfoGuid == para.UserGuid && !s.IsDeleted);
                if (usersQuery != null)
                {
                    PushId = usersQuery.PushId; //推送编号

                    //清空用户的设备编号
                    usersQuery.LockGuid = null;
                    db.SubmitChanges();
                }
                //车辆基本信息
                var query = db.BicycleLockInfo.FirstOrDefault(s => s.LockGuid == usersQuery.LockGuid && !s.IsDeleted);
                if (query != null)
                {
                    //修改车辆的状态为1表示关锁或者空闲
                    query.LockStatus = (int)BicycleStatusEnum.CloseLockStatus;
                    query.DeviceNo = "0"; //清空为0
                    query.LockNumber = "0";
                    db.SubmitChanges();
                }
            }

            //关锁时推送消息
            var msg = NotificationPush.PushMessageToReg("锁已关闭", PushId);

            result.ResObject = true;
            return result;
        }

        #endregion 测试极光推送

        #region 开锁加密串

        /// <summary>
        /// 开锁加密串
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public ResultModel GetEncryptionKey(GetEncyptionKey_PM para)
        {
            var resultModel = new ResultModel();
            var encryptionModel = new GetEncryptionKey_OM();
            double Vol = 3.3;

            #region 判断处理

            try
            {
                if (string.IsNullOrEmpty(para.KeySerial) || string.IsNullOrEmpty(para.DeviceNo))
                {
                    resultModel.IsSuccess = false;
                    resultModel.MsgCode = ResPrompt.ParaModelNotExist;
                    resultModel.Message = ResPrompt.ParaModelNotExistMessage;
                    return resultModel;
                }
                //查询锁的信息
                BicycleLockInfo LockInfo = GetBicycleLockInfo(para.DeviceNo);
                if (LockInfo == null)
                {
                    resultModel.IsSuccess = false;
                    resultModel.MsgCode = ResPrompt.BicycleNotExist;
                    resultModel.Message = ResPrompt.BicycleNotExistMessage;
                    //Utility.Common.FileLog.Log("设备编号：" + para.DeviceNo + ",开锁加密串：" + baseInfo + "\r\n", "开锁加密串");
                    return resultModel;
                }
                else if (LockInfo.LockStatus == 0)
                {
                    resultModel.IsSuccess = false;
                    resultModel.MsgCode = ResPrompt.BicycleOpenNotExist;
                    resultModel.Message = ResPrompt.BicycleOpenNotExistMessage;
                    return resultModel;
                }

                //判断是否已预约车辆
                resultModel = GetIsUserReservation(para.DeviceNo, para.UserInfoGuid);
                if (!resultModel.IsSuccess)
                {
                    return resultModel;
                }

                #endregion 判断处理

                encryptionModel.EncryptionKey = 0;
                string pwd = "k846eudn4jshaw7e";
                string keySource = para.KeySerial.PadRight(16, '0').ToUpper();

                if (para.Version > 80 && LockInfo.SecretKey.Length >= 80)
                {
                    Random ran = new Random();
                    //encryptionModel.EncryptionKey = ran.Next(1, 63);
                    //encryptionModel.EncryptionKey = 20;
                    encryptionModel.EncryptionKey = 46;
                    pwd = LockInfo.SecretKey.Substring(encryptionModel.EncryptionKey, 16);
                }
                encryptionModel.EncryptionInfo = AESEncrypt(keySource, pwd);
                encryptionModel.EncryptionKey += 128;
                resultModel.ResObject = encryptionModel;
                //Utility.Common.FileLog.Log("返回结果集encryptionModel：" + encryptionModel +"\r\n", "开锁加密串");
            }
            catch (Exception ex)
            {
                LogHelper.Error("获取开锁密钥异常:" + ex.Message + ",当前创建时间：" + DateTime.Now + ",KeySerial:" + para.KeySerial + ",锁的编号：" + para.DeviceNo + "", ex);
                resultModel.IsSuccess = false;
                resultModel.Message = "获取开锁密钥异常！";
            }
            return resultModel;
        }

        /// <summary>
        /// 加密方法
        /// </summary>
        /// <param name="encryptStr"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private string AESEncrypt(string encryptStr, string key)
        {
            byte[] keyArray = Encoding.ASCII.GetBytes(key);
            byte[] toEncryptArray = Encoding.ASCII.GetBytes(encryptStr);
            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.None;
            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return GetHexString(resultArray);
        }

        private string GetHexString(byte[] content)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in content)
            {
                int value = item & 0xFF;
                string str = value.ToString("x");
                if (str.Length < 2)
                {
                    sb.Append(0);
                }
                sb.Append(str);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 根据设备编号获取车锁信息
        /// </summary>
        /// <param name="DeviceNo"></param>
        /// <returns></returns>
        private BicycleLockInfo GetBicycleLockInfo(string DeviceNo)
        {
            using (var db = new MintBicycleDataContext())
            {
                var LockInfo = db.BicycleLockInfo.FirstOrDefault(s => s.LockNumber == DeviceNo);
                return LockInfo;
            }
        }

        /// <summary>
        /// 根据锁编号查询车辆信息
        /// </summary>
        /// <param name="DeviceNo"></param>
        /// <returns></returns>
        private BicycleBaseInfo GetBicycleBaseInfoByLockNumber(string LockNumber)
        {
            using (var db = new MintBicycleDataContext())
            {
                var bicycle = db.BicycleBaseInfo.FirstOrDefault(s => s.LockNumber == LockNumber);
                return bicycle;
            }
        }

        #endregion 开锁加密串

        #region 日期函数

        /// <summary>
        /// 计算两个时间的差值,取秒
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public int GetTotalSecond(DateTime? dt)
        {
            TimeSpan ts = Convert.ToDateTime(DateTime.Now).AddMinutes(20) - Convert.ToDateTime(dt);
            int SecondsTime = Convert.ToInt32(ts.TotalSeconds);
            return SecondsTime;
        }

        /// <summary>
        /// 计算两个时间的差值,取分钟
        /// </summary>
        /// <returns></returns>
        public int GetTotalMinutes(DateTime? dt)
        {
            TimeSpan ts = Convert.ToDateTime(DateTime.Now) - Convert.ToDateTime(dt);
            int MinutesTime = Convert.ToInt32(ts.TotalMinutes);
            return MinutesTime;
        }

        /// <summary>
        /// 把UTC时间格式转换成datetime
        /// </summary>
        /// <param name="utc"></param>
        /// <returns></returns>
        public static DateTime ConvertIntDatetime(double utc)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            startTime = startTime.AddSeconds(utc);
            startTime = startTime.AddHours(8);//转化为北京时间(北京时间=UTC时间+8小时 )
            return startTime;
        }

        /// <summary>
        /// 把datetime转换成int
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int ConvertDateTimeInt(System.DateTime time)
        {
            double intResult = 0;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            intResult = (time - startTime).TotalSeconds;
            return (int)intResult;
        }

        /// <summary>
        /// 时间戳Timestamp转换成日期
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public DateTime GetDateTime(int timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = ((long)timeStamp * 10000000);
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime targetDt = dtStart.Add(toNow);
            return targetDt;
        }

        #endregion 日期函数
    }
}