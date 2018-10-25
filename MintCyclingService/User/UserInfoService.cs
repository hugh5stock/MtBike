using HuRongClub.DBUtility;
using MintCyclingData;
using MintCyclingService.AdminLog;
using MintCyclingService.BaiDu;
using MintCyclingService.Common;
using MintCyclingService.Cycling;
using MintCyclingService.Handler;
using MintCyclingService.Login;
using MintCyclingService.MicroMsg;
using MintCyclingService.Utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Transactions;
using Utility;
using Utility.Common;
using Utility.File;
using Utility.LogHelper;

namespace MintCyclingService.User
{
    public class UserInfoService : IUserInfoService
    {
        private ICyclingService _cycling = new CyclingService();
        private IBaiduService _baiduService = new BaiduService();
        private MicroMsgVistor _mmVistor = new MicroMsgVistor();

        //日志服务
        private readonly ImanagerlogService _LogService = new ManagerlogService();

        #region 微信数据接口

        /// <summary>
        /// 新增或增加微信用户session
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public ResultModel AddOrUpdateMicroMsgUser(string code)
        {
            var result = new ResultModel();
            var wxreq = _mmVistor.CheckMicroMsg(code);
            if (wxreq.IsSuccess)
            {
                var rv = (JObject)wxreq.ResObject;
                var o = rv.Value<string>("openid");
                var k = rv.Value<string>("session_key");

                using (var db = new MintBicycleDataContext())
                {
                    var user = db.UserInfo.FirstOrDefault(u => u.MicroMsg == o && !u.IsDeleted);
                    if (user == null)
                    {
                        var nuser = new UserInfo()
                        {
                            UserInfoGuid = Guid.NewGuid(),
                            CreateTime = DateTime.Now,
                            IsDeleted = false,
                            MicroMsg = o,
                            sessionKey = k
                        };
                        db.UserInfo.InsertOnSubmit(nuser);
                        db.SubmitChanges();

                        result.ResObject = nuser;
                    }
                    else
                    {
                        user.UpdateTime = DateTime.Now;
                        user.MicroMsg = o;
                        user.sessionKey = k;
                        db.SubmitChanges();

                        result.ResObject = user;
                    }
                }
                return result;
            }
            else
            {
                return wxreq;
            }
        }

        /// <summary>
        /// 修改来自微信用户授权的信息
        /// </summary>
        /// <param name="userid">用户Guid</param>
        /// <param name="encryptedData">加密数据</param>
        /// <returns></returns>
        public ResultModel EditMicroMsgUserInfo(Guid info, string encryptedData, string iv)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var user = db.UserInfo.FirstOrDefault(u => u.UserInfoGuid == info && !u.IsDeleted);
                if (user == null || user.sessionKey == null)
                {
                    return new ResultModel() { IsSuccess = false, Message = "用户未Login" };
                }

                var data = MicroMsgVistor.Decrypt(encryptedData, user.sessionKey, iv);
                if (data==null)
                {
                    return new ResultModel() { IsSuccess = false, Message = "解密失败" };
                }

                var djson = JObject.Parse(data);
                var p = db.Photo.FirstOrDefault(p => p.Url == djson.Value<string>("avatarUrl"));
                Guid pguid;
                if (p==null)
                {
                    var nphoto = new Photo()
                    {

                    }
                }
                db.Photo.InsertOnSubmit(new MintCyclingData.Photo() {
                });
                user.ProvinceID = db.Province.FirstOrDefault(p => p.Name == djson.Value<string>("province")).Id;
                user.CityID = db.Province.FirstOrDefault(c => c.Name == djson.Value<string>("city")).Id;
                user.UserNickName = djson.Value<string>("nickName");
                user.PhotoGuid = db.Photo
            }
            return result;
        }

        #endregion

        #region 绿帝出行App端接口

        /// <summary>
        /// 查询个人行程
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public ResultModel GetUserTravelByUserGuid(UserTravel_PM para)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                //判断用户是否认证
                var user = GenericQuery.GetUserData(db, para.UserGuid);
                if (!user.IsSuccess) return user;

                //查询用户的行程记录信息
                var query = (from s in db.MyTravel
                             join t in db.TransactionInfo on s.MyTravelGuid equals t.MyTravelGuid
                             where !s.IsDeleted && !t.IsDeleted && s.UserInfoGuid == para.UserGuid && s.EndTime != null && s.StartTime != null
                             orderby s.CreateTime descending
                             select new { StartTime = s.StartTime, OutTradeNo = t.OutTradeNo, LockGuid = s.LockGuid, EndTime = s.EndTime, TotalMinSpan = t.TotalMinSpan, TotalAmount = t.TotalAmount });
                if (query.Any())
                {
                    var cnt = query.Count();
                    var seq = para.PageSize * (para.PageIndex - 1);
                    query = query.Skip(seq < 1 ? 0 : seq).Take(para.PageSize);
                    var list = new List<UserTravel_OM>();
                    foreach (var q in query)
                    {
                        var sk = GenericQuery.GetBicycleData(db, q.LockGuid);
                        list.Add(new UserTravel_OM
                        {
                            UserTravelStartTime = Utility.CommonHelper.DtToString(q.StartTime, false),
                            OrderNo = q.OutTradeNo,
                            BicycleNo = sk == null ? string.Empty : sk.BicycleNo,
                            //BicycleUsingTime = CommonHelper.CalcTimeSpan(q.StartTime, q.EndTime),
                            BicycleUsingTime = Utility.CommonHelper.CalcTimeSpanMyTravel(q.TotalMinSpan),
                            BicycleSpend = Math.Round(double.Parse(q.TotalAmount.ToString()), 2) + "元"
                        });
                    }
                    result.ResObject = new { Total = cnt, List = list };
                }
                else //不存在行程记录
                {
                    result.IsSuccess = false;
                    result.MsgCode = ResPrompt.CustomerTravelNotExist;
                    result.Message = ResPrompt.CustomerTravelNotExistMessage;
                }
            }
            return result;
        }

        /// <summary>
        /// 查询用户完成骑行的数据
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public ResultModel GetUserTravelEndByUserGuid(CyclingEnd_PM para)
        {
            Guid userGuid = new Guid();
            userGuid = para.UserInfoGuid;
            if (userGuid == null)
                return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ParaModelNotExist, Message = ResPrompt.ParaModelNotExistMessage };

            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                try
                {
                    #region 判断处理

                    //判断用户是否认证
                    var user = GenericQuery.GetUserData(db, para.UserInfoGuid);
                    if (!user.IsSuccess) return user;

                    var myTravel = db.MyTravel.OrderByDescending(s => s.CreateTime).FirstOrDefault(s => s.UserInfoGuid == para.UserInfoGuid && !s.IsDeleted);
                    if (myTravel == null)
                    {
                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.CustomerTravelNotExist, Message = ResPrompt.CustomerTravelNotExistMessage };
                    }

                    var sk = db.BicycleLockInfo.FirstOrDefault(q => q.LockNumber == myTravel.LockNumber && !q.IsDeleted);
                    if (sk == null)
                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.BicycleNotExist, Message = ResPrompt.BicycleNotExistMessage };

                    var userTravel = db.MyTravel.OrderByDescending(s => s.CreateTime).FirstOrDefault(s => s.UserInfoGuid == para.UserInfoGuid && s.LockGuid == sk.LockGuid && !s.IsDeleted);
                    if (userTravel == null)
                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.CustomerTravelNotExist, Message = ResPrompt.CustomerTravelNotExistMessage };

                    var accountInfo = db.AccountInfo.FirstOrDefault(p => p.UserInfoGuid == para.UserInfoGuid && !p.IsDeleted);
                    if (accountInfo == null)
                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.UserAccountNotExist, Message = ResPrompt.UserAccountNotExistMessage };

                    #endregion 判断处理

                    #region 计算骑行数据

                    //获取时分秒
                    var tick = userTravel.EndTime.Value.Subtract(userTravel.StartTime.Value);
                    var CalorieExpend = ConfigurationManager.AppSettings["CalorieExpend"];              //骑单车每小时卡路里消耗
                    var CycleAmount = ConfigurationManager.AppSettings["CycleAmount"];                 //骑单车每小时金额
                    var Carbon = ConfigurationManager.AppSettings["Carbon"];                          //骑单车每公里减少碳排放
                    var calorieSq = 240d; //表示double类型
                    var spendTotal = 1d;
                    var carbonSq = 0.15d;
                    double.TryParse(CycleAmount, out spendTotal);
                    double.TryParse(CalorieExpend, out calorieSq);
                    double.TryParse(Carbon, out carbonSq);
                    var ts = tick.TotalHours;
                    var spendMin = tick.TotalMinutes < 1d; //小于1分钟

                    decimal spendAmount = 0;
                    if (userTravel.CyclingMode == 4) //充电骑行模式免费
                    {
                        //计算骑行所用的费用
                        spendAmount = 0;
                        //FileLog.Log("1\r\n" + userTravel.CyclingMode, "骑行模式不计费"+ spendAmount+"元");
                    }
                    else
                    {
                        //计算骑行所用的费用
                        spendAmount = new CyclingExpenseHelper(new CyclingCostModel { Price = spendTotal, Tick = tick }).GetSpendAmount();
                        //FileLog.Log("2\r\n" + userTravel.CyclingMode, "骑行模式计费"+ spendAmount + "元"+ tick.Minutes);
                    }

                    double distance = 0; //单位：KM
                    double time = 0;   //分钟
                    //通过行程表中的经纬度查询距离-调用百度API
                    string strJosn1 = _baiduService.GetRidingDistance(userTravel.StartLatitude.ToString() + "," + userTravel.StartLongitude.ToString(), userTravel.EndLatitude.ToString() + "," + userTravel.EndLongitude.ToString(), out distance, out time);

                    Random ra = new Random();
                    //计算骑行结束返回数据
                    var data = new CyclingEnd_OM
                    {
                        OrderNo = OrderHelper.GenerateOrderNumber(),
                        DeviceNo = sk.LockNumber,
                        TotalTimeStr = Utility.CommonHelper.CalcTimeSpan(userTravel.EndTime, userTravel.StartTime),
                        CalorieExpend = Math.Round(calorieSq * ts, 2),                      //运动消耗卡路里
                        TotalCarbon = Math.Round(distance * 1.0d / 1000 * carbonSq, 2),    //节约的碳排放量
                        TotalDistance = Math.Round(decimal.Parse(distance.ToString()) / 1000, 2),
                        TotalAmount = Math.Round(spendAmount, 2),

                        TotalPowerValue = Math.Round(decimal.Parse(ra.Next(200, 1000).ToString()), 2),  //骑行结束接口返回消耗的电量值-暂时失败
                    };

                    #endregion 计算骑行数据

                    #region 骑行结束计费

                    if (!accountInfo.UsableAmount.HasValue && accountInfo.UsableAmount < data.TotalAmount)
                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.UserAccountBalanceError, Message = ResPrompt.UserAccountBalanceErrorMessage };
                    //如果在结算费用表中已存在计费信息不添加
                    if (!db.TransactionInfo.Any(q => q.MyTravelGuid == userTravel.MyTravelGuid && q.UserInfoGuid == para.UserInfoGuid && !q.IsDeleted))
                    {
                        using (var scope = new TransactionScope())
                        {
                            //骑行结束计费
                            var transactionInfo = new TransactionInfo
                            {
                                OrderGuid = Guid.NewGuid(),
                                UserInfoGuid = para.UserInfoGuid,
                                MyTravelGuid = userTravel.MyTravelGuid,
                                OutTradeNo = data.OrderNo,
                                DeviceNo = data.DeviceNo,
                                TotalMinSpan = Convert.ToDecimal(tick.TotalMinutes),
                                //TotalCalorieExpend = spendMin ? 0 : data.CalorieExpend,   //消耗的总的卡路里（kg）
                                //TotalCarbon = spendMin ? 0 : data.TotalCarbon,    //节约的碳排放量
                                TotalCalorieExpend = data.CalorieExpend,   //消耗的总的卡路里（kg）
                                TotalCarbon = data.TotalCarbon,    //节约的碳排放量
                                TotalDistance = data.TotalDistance,               //距离
                                TotalAmount = data.TotalAmount,
                                TotalPowerValue = data.TotalPowerValue,
                                CreateBy = para.UserInfoGuid,
                                CreateTime = DateTime.Now,
                                IsDeleted = false
                            };
                            db.TransactionInfo.InsertOnSubmit(transactionInfo);

                            accountInfo.UsableAmount = Math.Round(accountInfo.UsableAmount.Value - spendAmount, 2);
                            accountInfo.UpdateBy = para.UserInfoGuid;
                            accountInfo.UpdateTime = DateTime.Now;
                            db.SubmitChanges();
                            scope.Complete();
                        }
                    }
                    data.UsableAmount = accountInfo.UsableAmount;

                    #endregion 骑行结束计费

                    #region 查询是否停放在电子围栏区域

                    ICyclingService _cyclingService = new CyclingService();
                    //查询是否停放在电子围栏区域
                    var tq = _cyclingService.QueryBicycleIsinRange(new BicycleIsinRange_PM
                    {
                        //CurLongitude = para.CurLongitude,
                        //CurLatitude = para.CurLatitude
                        CurLongitude = decimal.Parse(userTravel.EndLatitude.ToString()),
                        CurLatitude = decimal.Parse(userTravel.EndLongitude.ToString())
                    });

                    //判断关锁时车辆是否在电子围栏内
                    string str = "{00000000-0000-0000-0000-000000000000}";
                    Guid EGuid = new Guid(str);
                    List<MapEnclosureModel> isModel = tq.ResObject as List<MapEnclosureModel>;

                    if ((isModel.Any()) && isModel[0].EnclosureGuid != EGuid && isModel[0].EnclosureGuid != null)
                    {
                        //表示此车在电子围栏范围内
                        sk.IsInElectronicFenCing = true;
                        sk.ElectronicFenCingGuid = isModel[0].EnclosureGuid;
                        db.SubmitChanges();

                        data.IsinRange = true;
                    }
                    else
                    {
                        sk.IsInElectronicFenCing = null;
                        sk.ElectronicFenCingGuid = null;
                        db.SubmitChanges();

                        data.IsinRange = false;
                    }

                    #endregion 查询是否停放在电子围栏区域

                    //2017-05-12新增代码
                    //修改车辆的状态为1表示关锁
                    //查询用户表
                    var usersQuery = db.UserInfo.FirstOrDefault(s => s.UserInfoGuid == para.UserInfoGuid && !s.IsDeleted);
                    if (usersQuery != null)
                    {
                        usersQuery.LockGuid = null;      //修改用户表中的车辆Guid表示那个用户关锁
                    }
                    sk.LockStatus = (int)BicycleStatusEnum.CloseLockStatus;
                    sk.DeviceNo = "0";                       //清空为0
                    db.SubmitChanges();

                    LogHelper.Info("骑行结束计算费用完成，锁的状态：" + sk.LockStatus + "锁的编号：" + sk.DeviceNo + "");

                    result.Message = "当前骑行已完成";
                    result.ResObject = data;
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.MsgCode = ResPrompt.CustomerTravelUpdateError;
                    result.Message = ResPrompt.CustomerTravelUpdateErrorMessage;
                    LogHelper.Error("骑行结束计算费用异常:" + ex.Message + "时间：" + DateTime.Now+"", ex);
                    //Utility.Common.FileLog.Log("骑行结束计算费用异常：" + ex.Message + ",时间：" + DateTime.Now + "\r\n", "UserTravelJieSuanLog");
                }
            }
            return result;
        }

        /// <summary>
        /// 查询个人中心用户信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ResultModel GetUserInfoCenterByUserGuid(Guid UserGuid)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var query = (from users in db.UserInfo
                             join r in db.UserAuthentication on users.UserInfoGuid equals r.UserInfoGuid
                             into temp1 from rr in temp1.DefaultIfEmpty()
                             join t in db.TransactionInfo on users.UserInfoGuid equals t.UserInfoGuid
                             into temp2 from tt in temp2.DefaultIfEmpty()
                             join p in db.Photo on users.PhotoGuid equals p.PhotoGuid
                             into temp3 from pp in temp3.DefaultIfEmpty()
                             where users.UserInfoGuid == UserGuid
                             orderby users.CreateTime descending
                             select new
                             {
                                 PowerBank = users.PowerBank,
                                 UserGuid = users.UserInfoGuid,
                                 UserName = rr.UserName,
                                 Phone = users.Phone,
                                 //PhotoUrl = users.Photo == null ? string.Empty : users.Photo.Url
                                 PhotoUrl = pp.Url ?? string.Empty
                             });
                if (query.Any())
                {
                    double? TotalCalorieExpend = 0;
                    double? TotalCarbon = 0;
                    decimal? TotalDistance = 0;
                    decimal? TotalPowerValue = 0;

                    var sk = query.FirstOrDefault();

                    #region 统计

                    string sqlStr = " SELECT SUM(TotalCalorieExpend) as TotalCalorieExpend, SUM(TotalCarbon) as TotalCarbon,sum(TotalDistance) as TotalDistance,sum(TotalPowerValue) as TotalPowerValue  FROM TransactionInfo where UserInfoGuid='" + UserGuid + "'";
                    DataSet ds = DbHelperSQL.Query(sqlStr);

                    if (ds != null && ds.Tables[0].Rows.Count > 0) //如果查询返回false时，表示没有查询到数据，说明在预约表中已存在了预约的车辆
                    {
                        if (String.IsNullOrEmpty(ds.Tables[0].Rows[0]["TotalCalorieExpend"].ToString()))
                        {
                            TotalCalorieExpend = 0;
                        }
                        else
                        {
                            TotalCalorieExpend = double.Parse(ds.Tables[0].Rows[0]["TotalCalorieExpend"].ToString());
                        }
                        if (String.IsNullOrEmpty(ds.Tables[0].Rows[0]["TotalCarbon"].ToString()))
                        {
                            TotalCarbon = 0;
                        }
                        else
                        {
                            TotalCarbon = double.Parse(ds.Tables[0].Rows[0]["TotalCarbon"].ToString());
                        }
                        if (String.IsNullOrEmpty(ds.Tables[0].Rows[0]["TotalDistance"].ToString()))
                        {
                            TotalDistance = 0;
                        }
                        else
                        {
                            TotalDistance = decimal.Parse(ds.Tables[0].Rows[0]["TotalDistance"].ToString());
                        }
                        if (String.IsNullOrEmpty(ds.Tables[0].Rows[0]["TotalPowerValue"].ToString()))
                        {
                            TotalPowerValue = 0;
                        }
                        else
                        {
                            TotalPowerValue = decimal.Parse(ds.Tables[0].Rows[0]["TotalPowerValue"].ToString());
                        }
                    }
                    else
                    {
                        TotalCalorieExpend = 0;
                        TotalCarbon = 0;
                        TotalDistance = 0;
                        TotalPowerValue = 0;
                    }

                    #endregion 统计

                    //返回输入对象
                    var model = new GetUsersInfoCenter_OM
                    {
                        PowerBank = sk.PowerBank,
                        UserGuid = sk.UserGuid,
                        Phone = sk.Phone,
                        UserName = sk.UserName,
                        PhotoUrl = FileUtility.GetFullUrlByRelativePath(sk.PhotoUrl),
                        CalorieExpend = TotalCalorieExpend,
                        Carbon = TotalCarbon,
                        Distance = TotalDistance,
                        TotalPowerValue = TotalPowerValue

                    };
                    result.ResObject = model;
                }
                else
                {
                    result.ResObject = false;
                }
            }
            return result;
        }

        /// <summary>
        /// 通过用户的Guid查询个人详细信息
        /// 作者：TOM
        /// 时间：2017-02-09
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ResultModel GetUserinfoByUserGuid(Guid UserGuid)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var query = (from users in db.UserInfo
                             join r in db.UserAuthentication on users.UserInfoGuid equals r.UserInfoGuid into temp
                             from rr in temp.DefaultIfEmpty()
                             join p in db.Photo on users.PhotoGuid equals p.PhotoGuid into temp2
                             from pp in temp2.DefaultIfEmpty()
                             where users.UserInfoGuid == UserGuid
                             orderby users.CreateTime descending
                             select new
                             {
                                 UserGuid = users.UserInfoGuid,
                                 Phone = users.Phone,
                                 UserName = rr.UserName,
                                 UserNickName = users.UserNickName,
                                 PhotoUrl = pp.Url ?? string.Empty,
                                 StatusStr = LogService.GetAuthState(int.Parse(rr.status.ToString() ?? "0")),
                             });
                if (query.Any())
                {
                    var sk = query.FirstOrDefault();
                    var model = new GetUsersInfo_OM
                    {
                        UserGuid = sk.UserGuid,
                        Phone = sk.Phone,
                        UserName = sk.UserName,
                        UserNickName = sk.UserNickName,
                        PhotoUrl = FileUtility.GetFullUrlByRelativePath(sk.PhotoUrl),
                        StatusStr = sk.StatusStr
                    };
                    result.Message = "查询个人详细信息成功";
                    result.ResObject = model;
                }
                else
                {
                    result.Message = "查询个人详细信息失败";
                    result.ResObject = false;
                }
            }
            return result;
        }

        /// <summary>
        /// 修改用户的手机号码和昵称
        /// 作者：TOM
        /// 时间：2017-02-09
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ResultModel EditUserPhoneOrNickNameByUserGuid(EditUserPhoneOrNickName_PM data)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var query = (from s in db.UserInfo
                             where s.UserInfoGuid == data.UserGuid
                             select s);
                if (query.Any())
                {
                    var UserInfo = query.FirstOrDefault();
                    var qPhone = db.UserInfo.FirstOrDefault(s => s.Phone == data.Phone);
                    if (qPhone != null && qPhone.UserInfoGuid != data.UserGuid && qPhone.Phone == data.Phone) //表示数据库中已存在相同的手机号码不能修改
                    {
                        return new ResultModel() { IsSuccess = false, MsgCode = ResPrompt.UserPhoneNotExist, Message = ResPrompt.UserPhoneNotExistMessage };
                    }
                    if (data.PhotoGuid.HasValue)
                    {
                        UserInfo.PhotoGuid = data.PhotoGuid; //修改图像--图像的Guid
                    }

                    UserInfo.Phone = string.IsNullOrEmpty(data.Phone) ? UserInfo.Phone : data.Phone;
                    UserInfo.UserNickName = string.IsNullOrEmpty(data.NickName) ? UserInfo.UserNickName : data.NickName;
                    UserInfo.UpdateBy = data.UserGuid;
                    UserInfo.UpdateTime = DateTime.Now;

                    db.SubmitChanges();
                    result.Message = "修改用户信息成功";
                    result.ResObject = true;
                }
                else
                {
                    result.IsSuccess = false;
                    result.Message = "修改用户信息失败";
                    //result.ResObject = false;
                }
            }
            return result;
        }

        /// <summary>
        ///绑定充电宝
        /// </summary>
        /// <returns></returns>
        public ResultModel BindPowerBank(BindPowerBank data)
        {

            using (var db = new MintBicycleDataContext())
            {
                var power = from x in db.UserInfo
                            where x.PowerBank == data.PowerBank
                            select x;
                if (power.Any())
                {

                    return new Utils.ResultModel { IsSuccess = false, Message = "充电宝已被绑定！" };

                }

                var query = from x in db.UserInfo
                            where x.UserInfoGuid == data.UserGuid && !x.IsDeleted
                            select x;
                if (query.Any())
                {

                    var fir = query.FirstOrDefault();
                    if (fir.Status == 0)
                    {

                        return new ResultModel { IsSuccess = false, Message = "此用户已被禁用" };


                    }
                    fir.PowerBank = data.PowerBank;
                    fir.UpdateTime = DateTime.Now;
                    db.SubmitChanges();
                    return new ResultModel { ResObject = true };


                }


                return new ResultModel { IsSuccess = false, Message = "用户不存在！" };



            }



        }

        /// <summary>
        /// 用户认证
        /// </summary>
        /// <param name="UserGuid"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public ResultModel AddUserAuthentication(AddUserAuth_PM data)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                //判断用户是否已认证
                var userAuth = db.UserAuthentication.FirstOrDefault(s => s.UserInfoGuid == data.UserInfoGuid);

                if (userAuth != null && userAuth.status == 1)
                {
                    return new ResultModel() { IsSuccess = false, MsgCode = ResPrompt.UserAuthFormatError, Message = ResPrompt.UserAuthFormatErrorMessage };
                }
                if (userAuth != null && userAuth.status == 0)  //如果已存在记录只修改
                {
                    userAuth.CardNumber = data.CardNumber;
                    userAuth.UserName = data.UserName;
                    userAuth.status = 1;
                    userAuth.UpdateBy = data.UserInfoGuid;
                    db.SubmitChanges();
                    result.Message = "修改认证成功";
                    result.ResObject = true;
                }
                else
                {
                    var UserAuth = new UserAuthentication
                    {
                        UserAuthGuid = Guid.NewGuid(),
                        UserInfoGuid = data.UserInfoGuid,
                        UserName = data.UserName,
                        AuthTypeName = "身份证",
                        CardNumber = data.CardNumber,
                        status = 1,
                        CreateBy = data.UserInfoGuid,
                        createtime = DateTime.Now,
                        IsDeleted = false
                    };

                    db.UserAuthentication.InsertOnSubmit(UserAuth);
                    db.SubmitChanges();

                    result.Message = "添加认证成功";
                    result.ResObject = true;
                }
            }
            return result;
        }

        /// <summary>
        /// 查询用户Token是否存在
        /// </summary>
        /// <param name="utk"></param>
        /// <returns></returns>
        public ResultModel GetUserTokenExist(Guid utk)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var query = (from s in db.UserInfo
                             join t in db.CustomerAccessToken on s.UserInfoGuid equals t.UserInfoGuid
                             where s.Status == 1 && !s.IsDeleted && t.ProvisionalToken == utk
                             select t);
                result.IsSuccess = query.Any();
            }
            return result;
        }

        #endregion

        #region 后台用户管理接口

        /// <summary>
        /// 根据查询条件搜索用户列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel GetUserInfoList(AdminUserInfo_PM model)
        {
            var result = new ResultModel();
            var region = Config.UserRegion;
            if (region == null)
                return result;
            using (var db = new MintBicycleDataContext())
            {
                var query = (from u in db.UserInfo
                             join t in db.UserAuthentication on u.UserInfoGuid equals t.UserInfoGuid into temp
                             from tt in temp.DefaultIfEmpty()
                             join b in db.BicycleLockInfo on u.LockGuid equals b.LockGuid into tep
                             from bb in tep.DefaultIfEmpty()
                             join d in db.Deposit on u.UserInfoGuid equals d.UserInfoGuid into dtep
                             from dd in dtep.DefaultIfEmpty()
                             join a in db.AccountInfo on u.UserInfoGuid equals a.UserInfoGuid into atep
                             from aa in atep.DefaultIfEmpty()
                             where ((string.IsNullOrEmpty(model.Phone)) || u.Phone.Contains(model.Phone))
                             && ((string.IsNullOrEmpty(model.UserName)) || u.UserName.Contains(model.UserName))
                             && ((string.IsNullOrEmpty(model.UserNickName)) || u.UserNickName.Contains(model.UserNickName))
                             && ((model.StartTime == null || (model.StartTime != null && u.CreateTime.Date >= model.StartTime.Value.Date)) && (model.EndTime == null || (model.EndTime != null && u.CreateTime.Date <= model.EndTime.Value.Date)))
                             && !u.IsDeleted
                             && (region.ExceptUserRegion || (u.ProvinceID == region.UserProvince && (region.UserCity == null || u.CityID == region.UserCity)
                                   && (region.UserDistrict == null || u.DistrictID == region.UserDistrict)))
                             orderby u.CreateTime descending
                             select new AdminUserInfo_OM
                             {
                                 UserGuid = u.UserInfoGuid,
                                 Phone = u.Phone,
                                 UserName = u.UserName,
                                 UserNickName = u.UserNickName,
                                 UserStatus = u.Status ?? 0,
                                 StatusStr = LogService.GetAuthState(int.Parse(tt.status.ToString() == null ? "0" : tt.status.ToString())),
                                 Amount = dd.Amount,
                                 UsableAmount = aa.UsableAmount,
                                 DeviceNo = bb.DeviceNo,
                                 PushId = u.PushId,
                                 IsPush = u.IsPush,
                                 CreateTime = u.CreateTime
                             });
 
                //query = query.Where(x => x.CreateTime.Value.Date == DateTime.Now.Date);
                if (query.Any())
                {
                    var list = query;
                    var cnt = list.Count();
                    var tsk = new Common.PagedList<AdminUserInfo_OM>(query, model.PageIndex, model.PageSize, query.Count());
                    result.ResObject = new { Total = query.Count(), List = tsk };
                }
            }
            return result;
        }
        
        /// <summary>
        /// 通过用户的Guid查询个人信息
        /// 作者：TOM
        /// 时间：2017-05-23
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ResultModel GetUserByUserGuid(Guid UserGuid)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var query = (from users in db.UserInfo
                             join r in db.UserAuthentication on users.UserInfoGuid equals r.UserInfoGuid into temp
                             from rr in temp.DefaultIfEmpty()
                             join p in db.Photo on users.PhotoGuid equals p.PhotoGuid into temp2
                             from pp in temp2.DefaultIfEmpty()
                             where users.UserInfoGuid == UserGuid
                             orderby users.CreateTime descending
                             select new
                             {
                                 UserGuid = users.UserInfoGuid,
                                 Phone = users.Phone,
                                 UserName = users.UserName,
                                 UserNickName = users.UserNickName,
                                 PhotoUrl = pp.Url ?? string.Empty,
                                 StatusStr = LogService.GetAuthState(int.Parse(rr.status.ToString() ?? "0")),
                                 UserStatus = users.Status == 0 ? "禁用" : "启用",
                                 CardNumber = rr.CardNumber
                             });
                if (query.Any())
                {
                    var sk = query.FirstOrDefault();
                    var model = new UserInfoDetails_PM
                    {
                        UserGuid = sk.UserGuid,
                        Phone = sk.Phone,
                        UserName = sk.UserName,
                        UserNickName = sk.UserNickName,
                        PhotoUrl = FileUtility.GetFullUrlByRelativePath(sk.PhotoUrl),
                        StatusStr = sk.StatusStr,
                        UserStatus = sk.UserStatus,
                        CardNumber = sk.CardNumber
                    };
                    result.Message = "查询用户信息成功";
                    result.ResObject = model;
                }
                else
                {
                    result.Message = "查询用户信息失败";
                    result.ResObject = false;
                }
            }
            return result;
        }
        
        /// <summary>
        /// 编辑用户信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel EditUserInfoByUserGuid(EditUserInfo_PM model)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var userQuery = db.UserInfo.FirstOrDefault(s => s.UserInfoGuid == model.UserGuid && !s.IsDeleted);
                if (userQuery != null)
                {
                    userQuery.Phone = model.Phone;
                    userQuery.UserName = model.UserName;
                    userQuery.UserNickName = model.UserNickName;
                    if (!string.IsNullOrEmpty(model.UserStatus))
                    {
                        userQuery.Status = int.Parse(model.UserStatus);
                    }
                    userQuery.UpdateBy = model.UserGuid;
                    userQuery.UpdateTime = DateTime.Now;
                    db.SubmitChanges();
                    result.IsSuccess = true;
                }
            }
            return result;
        }
        
        /// <summary>
        /// 删除用户信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel DeleteUserByUserGuid(DeleteUserInfo_PM model)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var userQuery = db.UserInfo.FirstOrDefault(s => s.UserInfoGuid == model.UserGuid && !s.IsDeleted);
                if (userQuery != null)
                {
                    db.UserInfo.DeleteOnSubmit(userQuery);
                    db.SubmitChanges();
                }
                else
                {
                    return new ResultModel() { IsSuccess = false, MsgCode = ResPrompt.UserinfoNotExist, Message = ResPrompt.UserinfoNotExistMessage };
                }
            }
            return result;
        }

        /// <summary>
        /// 锁定用户状态
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel LockUserStatusByUserGuid(LockUserInfo_PM model)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var userQuery = db.UserInfo.FirstOrDefault(s => s.UserInfoGuid == model.UserGuid && !s.IsDeleted);
                if (userQuery != null)
                {
                    if (userQuery.Status == 0)
                    {
                        userQuery.Status = 1;
                    }
                    else
                    {
                        userQuery.Status = 0;
                    }
                    userQuery.UpdateBy = model.UserGuid;
                    userQuery.UpdateTime = DateTime.Now;
                    db.SubmitChanges();
                }
                else
                {
                    return new ResultModel() { IsSuccess = false, MsgCode = ResPrompt.UserinfoNotExist, Message = ResPrompt.UserinfoNotExistMessage };
                }
            }
            return result;
        }
        
        #endregion 后台用户管理接口

        #region 用户还车异常处理

        /// <summary>
        /// 用户还车异常处理
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public ResultModel ReturnCarByPhone(ReturnCar_PM model)
        {
            var resultModel = new ResultModel();
            DateTime dtNow = DateTime.Now;
            Guid? bicycleGuid = new Guid();
 
            using (var db = new MintBicycleDataContext())
            {
                #region 处理逻辑

                var userQuery = db.UserInfo.FirstOrDefault(s => s.Phone == model.Phone);
                if (userQuery != null)
                {
                    bicycleGuid = userQuery.LockGuid; //车辆Guid
                    if (bicycleGuid != null)
                    {
                        //修改车锁信息
                        var bicyQuery = db.BicycleLockInfo.FirstOrDefault(s => s.LockGuid == bicycleGuid && s.LockStatus == 0);
                        if (bicyQuery != null)
                        {
                            bicyQuery.LockStatus = 1;
                            bicyQuery.DeviceNo = "0";
                        }

                        //修改用户信息
                        var updateUser = db.UserInfo.FirstOrDefault(s => s.UserInfoGuid == userQuery.UserInfoGuid && s.LockGuid != null);
                        if (updateUser != null)
                        {
                            updateUser.LockGuid = null;
                        }

                        #region 预约处理

                        //处理预约是否超时
                        var ReservationQuery = db.Reservation.OrderByDescending(s => s.CreateTime).FirstOrDefault(s => s.DeviceNo == bicyQuery.LockNumber && s.UserInfoGuid == userQuery.UserInfoGuid && s.Status == 1);

                        //修改预约的状态为已结束
                        if (ReservationQuery != null) //说明此用户用手机预约了单车，否则直接扫码骑行单车
                        {
                            // 计算两个时间的差值,取分钟--20分钟倒计时处理业务逻辑
                            int Mintues = _cycling.GetTotalMinutes(ReservationQuery.StartTme);
                            if (Mintues > 20)
                            {
                                ReservationQuery.Status = 2; //预约结束
                            }

                            ReservationQuery.EndTime = dtNow;
                            ReservationQuery.UpdateBy = userQuery.UserInfoGuid;
                            ReservationQuery.UpdateTime = dtNow;
                            db.SubmitChanges();
                        }

                        var Rquery = db.Reservation.OrderByDescending(s => s.CreateTime).FirstOrDefault(s => s.DeviceNo == bicyQuery.LockNumber && s.UserInfoGuid == userQuery.UserInfoGuid && s.Status == 3);
                        if (Rquery != null)
                        {
                            Rquery.Status = 2; //已结束
                            Rquery.EndTime = dtNow;
                            Rquery.UpdateBy = userQuery.UserInfoGuid;
                            Rquery.UpdateTime = dtNow;
                            db.SubmitChanges();
                        }

                        #endregion 预约处理

                        //修改行程信息
                        var myTravel = db.MyTravel.OrderByDescending(s => s.CreateTime).FirstOrDefault(s => !s.IsDeleted && s.LockGuid == bicycleGuid && s.UserInfoGuid == userQuery.UserInfoGuid);
                        if (myTravel != null)  //没有上传开锁交易记录信息
                        {
                            myTravel.EndLongitude = bicyQuery.Longitude;
                            myTravel.EndLatitude = bicyQuery.Latitude;
                            myTravel.EndTime = dtNow;
                            myTravel.UpdateTime = dtNow;
                            myTravel.Status = 0;  // 交易状态   （0手动还车、1自动还车）
                        }

                        #region 添加结算信息
                        try
                        {
                            double distance = 0; //单位：KM
                            double time = 0;   //分钟
                                               //通过行程表中的经纬度查询距离-调用百度API
                            string strJosn1 = _baiduService.GetRidingDistance(myTravel.StartLatitude.ToString() + "," + myTravel.StartLongitude.ToString(), myTravel.EndLatitude.ToString() + "," + myTravel.EndLongitude.ToString(), out distance, out time);

                            //获取时分秒
                            var tick = myTravel.EndTime.Value.Subtract(myTravel.StartTime.Value);
                            var qt = ConfigurationManager.AppSettings["EnergyConsumption"];
                            var spendConfig = ConfigurationManager.AppSettings["CycleSpend"];
                            var carbonConfig = ConfigurationManager.AppSettings["CarbonConsumption"];
                            var calorieSq = 240d;
                            var spendTotal = 1d;
                            var carbonSq = 0.15d;
                            double.TryParse(spendConfig, out spendTotal);
                            double.TryParse(qt, out calorieSq);
                            double.TryParse(carbonConfig, out carbonSq);
                            var ts = tick.TotalHours;
                            var spendMin = tick.TotalMinutes < 1d; //小于1分钟

                            //骑行结束计费
                            var transactionInfo = new TransactionInfo
                            {
                                OrderGuid = Guid.NewGuid(),
                                UserInfoGuid = userQuery.UserInfoGuid,
                                MyTravelGuid = myTravel.MyTravelGuid,
                                OutTradeNo = OrderHelper.GenerateOrderNumber(),
                                DeviceNo = bicyQuery.LockNumber,
                                TotalMinSpan = Convert.ToDecimal(tick.TotalMinutes),
                                //TotalCalorieExpend = spendMin ? 0 : data.CalorieExpend,   //消耗的总的卡路里（kg）
                                //TotalCarbon = spendMin ? 0 : data.TotalCarbon,           //节约的碳排放量
                                TotalCalorieExpend = 1.12,                                 //运动消耗卡路里,
                                TotalCarbon = 1.05,                                       //节约的碳排放量
                                TotalDistance = Convert.ToDecimal(1.23),    //距离
                                TotalAmount = 0,
                                CreateBy = userQuery.UserInfoGuid,
                                CreateTime = dtNow,
                                IsDeleted = false
                            };
                            db.TransactionInfo.InsertOnSubmit(transactionInfo);
                            db.SubmitChanges();

                            resultModel.IsSuccess = true;
                            resultModel.Message = "用户还车处理成功！";

                            //操作日志记录
                            string parameters = "Phone:" + model.Phone;
                            dt_manager_log LogModel = new dt_manager_log
                            {
                                action_type = ActionEnum.Edit.ToString(),
                                remark = "用户还车处理成功,参数:" + parameters,
                                AdminGuid = model.OperatorGuid
                            };

                            _LogService.AddManagerLog(LogModel);
                        }
                        catch (Exception ex)
                        {
                            resultModel.IsSuccess = false;
                            resultModel.Message = "用户还车异常处理出现错误！";

                            //操作日志记录
                            string parameters2 = "Phone:" + model.Phone;
                            dt_manager_log LogModel2 = new dt_manager_log
                            {
                                action_type = ActionEnum.Edit.ToString(),
                                remark = "用户还车异常处理" + ex.Message + ",参数:" + parameters2,
                                AdminGuid = model.OperatorGuid
                            };

                            _LogService.AddManagerLog(LogModel2);
                        }

                        #endregion 添加结算信息
                    }
                    else
                    {
                        resultModel.IsSuccess = false;
                        resultModel.Message = "此用户暂无用车信息！";

                        //操作日志记录
                        string parameters1 = "Phone:" + model.Phone;
                        dt_manager_log LogModel1 = new dt_manager_log
                        {
                            action_type = ActionEnum.Edit.ToString(),
                            remark = "此用户暂无用车信息,参数:" + parameters1,
                            AdminGuid = model.OperatorGuid
                        };

                        _LogService.AddManagerLog(LogModel1);
                    }
                }
                else
                {
                    resultModel.IsSuccess = false;
                    resultModel.Message = "没有此用户！";
                    //操作日志记录
                    string parameters = "Phone:" + model.Phone;
                    dt_manager_log LogModel = new dt_manager_log
                    {
                        action_type = ActionEnum.Edit.ToString(),
                        remark = "没有此用户,参数:" + parameters,
                        AdminGuid = model.OperatorGuid
                    };

                    _LogService.AddManagerLog(LogModel);
                }

                #endregion 处理逻辑
            }
            return resultModel;
        }

        #endregion 用户还车异常处理


    }
}