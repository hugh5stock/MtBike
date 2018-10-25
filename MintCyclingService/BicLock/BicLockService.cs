using MintCyclingData;
using MintCyclingService.AdminLog;
using MintCyclingService.Common;
using MintCyclingService.Cycling;
using MintCyclingService.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Utility;
using Utility.LogHelper;

namespace MintCyclingService.BicLock
{
    /// <summary>
    /// 车辆锁服务类
    /// </summary>
    public class BicLockService : IBicLockService
    {
        private readonly ImanagerlogService _LogService = new ManagerlogService();

        #region 车辆列表管理

        /// <summary>
        /// 根据查询条件搜索后台车辆列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel GetBicycleBaseList(GetBicycleBaseList_PM model)
        {
            var result = new ResultModel();
            var region = Config.UserRegion;
            if (region == null)
                return result;
            using (var db = new MintBicycleDataContext())
            {
                var query = (from s in db.BicycleLockInfo
                             join b in db.BicycleBaseInfo on s.LockNumber equals b.LockNumber
                             join p in db.Province on s.ProvinceID equals p.Id into temp1
                             from pp in temp1.DefaultIfEmpty()
                             join c in db.City on s.CityID equals c.Id into temp2
                             from cc in temp2.DefaultIfEmpty()
                             join d in db.District on s.DistrictID equals d.Id into temp
                             from tt in temp.DefaultIfEmpty()
                             where ((string.IsNullOrEmpty(model.BicyCleNumber)) || s.LockNumber.Contains(model.BicyCleNumber))
                             && (model.LockStatus == 0 || s.LockStatus == model.LockStatus)
                             && (model.ProvinceID == 0 || s.ProvinceID == model.ProvinceID)
                             && (model.CityID == 0 || s.CityID == model.CityID)
                             && (model.DistrictID == 0 || s.DistrictID == model.DistrictID)
                             && !s.IsDeleted
                             && (region.ExceptUserRegion || (s.ProvinceID == region.UserProvince && (region.UserCity == null || s.CityID == region.UserCity)
                                   && (region.UserDistrict == null || s.DistrictID == region.UserDistrict)))
                             orderby b.CreateTime descending
                             select new GetBicycleBaseList_OM
                             {
                                 BicycleGuid = b.BicycleGuid,
                                 LockGuid = s.LockGuid,
                                 BicyCleNumber = b.BicycleNumber,   //车辆编号
                                 BicycleTypeName = BicycleBaseInfoShow.GetBicycleTypeName(b.BicyCleTypeName), //车辆类型
                                 LockNumber = s.LockNumber,
                                 LockStatus = EnumRemarksHandler.GetBicycleState(int.Parse(s.LockStatus.ToString())),
                                 Voltage = s.Voltage,
                                 DistricName = string.Format("{0}{1}{2}", new object[] { pp.Name, cc.Name, tt.Name }),  //所在的区
                                 CreateTime = s.CreateTime,
                                 Remark = b.Remark
                             });
                if (query.Any())
                {
                    var tsk = new PagedList<GetBicycleBaseList_OM>(query, model.PageIndex, model.PageSize, query.Count());
                    result.ResObject = new { Total = query.Count(), List = tsk };
                }
            }
            return result;
        }

        /// <summary>
        /// 查询单车锁的状态
        /// </summary>
        /// <param name="st"></param>
        /// <returns></returns>
        public static string GetBicycleState(int st)
        {
            var str = string.Empty;
            switch (st)
            {
                case (int)BicycleStatusEnum.OpenLockStatus:
                    str = "使用中";
                    break;

                case (int)BicycleStatusEnum.CloseLockStatus:
                    str = "未使用";
                    break;

                case (int)BicycleStatusEnum.fault:
                    str = "已故障";
                    break;

                case (int)BicycleStatusEnum.LowBattery:
                    str = "电量不足";
                    break;

                default:
                    str = "未知状态";
                    break;
            }
            return str;
        }

        /// <summary>
        /// 删除车锁信息
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public ResultModel DeleteBicycleByBicycleGuid(BicycleDelete_PM Model)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var query = db.BicycleLockInfo.FirstOrDefault(s => s.LockGuid == Model.BicycleBaseGuid && !s.IsDeleted);
                if (query == null)
                    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.NoUserTransationCode, Message = ResPrompt.NoUserTransationMessage };
                query.IsDeleted = true;
                query.UpdateBy = Model.OperatorGuid;
                query.UpdateTime = DateTime.Now;
                db.SubmitChanges();
                result.ResObject = true;
            }
            return result;
        }

        /// <summary>
        /// 新增或修改锁信息
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public ResultModel AddAndUpdateBicycleBase(AddBicycleOrUpdate_PM Model)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                #region 添加或者修改

                if (Model.LockGuid == null)
                {
                    //判断是否存在相同车锁编号的锁信息
                    if (db.BicycleLockInfo.Any(x => x.LockNumber == Model.LockNumber))
                    {
                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.BicycleNumberError, Message = ResPrompt.BicycleNumberErrorMessage };
                    }
                    var Bicycle = new BicycleLockInfo
                    {
                        LockGuid = Guid.NewGuid(),
                        LockNumber = Model.DeviceNo,   //暂时车辆编号和设备编号都是一样的值
                        DeviceNo = Model.DeviceNo == null ? string.Empty : Model.DeviceNo,
                        //SecretKey = Model.SecretKey == null ? string.Empty : Model.SecretKey,
                        //LockMac = Model.LockMac,
                        //LockNumber = Model.LockNumber,
                        LockStatus = Model.LockStatus,
                        //Longitude = Model.Longitude,
                        //Latitude = Model.Latitude,
                        Remark = Model.Remark,
                        CreateBy = Model.OperatorGuid,
                        CreateTime = DateTime.Now,
                        IsDeleted = false
                    };
                    db.BicycleLockInfo.InsertOnSubmit(Bicycle);
                    db.SubmitChanges();
                    result.ResObject = true;
                }
                else
                {
                    var Bicycle = db.BicycleLockInfo.FirstOrDefault(p => p.LockGuid == Model.LockGuid && !p.IsDeleted);
                    if (Bicycle == null)
                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.BicycleNotExist, Message = ResPrompt.BicycleNotExistMessage };

                    //Bicycle.BicyCleNumber = Model.DeviceNo;
                    //Bicycle.DeviceNo = Model.DeviceNo;
                    //Bicycle.SecretKey = Model.SecretKey;
                    //Bicycle.LockNumber = Model.LockNumber;
                    // Bicycle.LockStatus = Model.LockStatus;
                    //Bicycle.ElectricQuantity = Model.ElectricQuantity;
                    //Bicycle.Address = Model.Address;
                    Bicycle.Remark = Model.Remark;
                    Bicycle.UpdateBy = Model.OperatorGuid;
                    Bicycle.UpdateTime = DateTime.Now;
                    db.SubmitChanges();
                    result.ResObject = true;
                }

                #endregion 添加或者修改
            }
            return result;
        }

        #endregion 车辆列表管理

        #region 批量生成车辆编号管理
        /// <summary>
        /// 根据查询条件搜索批量生成车牌号列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel GetBicycleNumberListByCondition(GetBicycleNumberList_PM model)
        {
            var result = new ResultModel();
            string dt1 = model.StartTime.ToString("yyyy-MM-dd");
            DateTime startTime = DateTime.Parse(dt1);
            string dt2 = model.EndTime.ToString("yyyy-MM-dd");
            DateTime endTime = DateTime.Parse(dt2);
            using (var db = new MintBicycleDataContext())
            {
                var query = (from s in db.ProduceBicycleNumberInfo
                             join c in db.CustomerInfo on s.CustomerID equals c.CustomerID
                             where ((string.IsNullOrEmpty(model.CustomerName)) || c.CustomerName.Contains(model.CustomerName))
                              && (s.CreateTime.Date >= startTime.Date && s.CreateTime.Date <= endTime.Date)
                              && !s.IsDeleted && !c.IsDeleted
                             orderby s.BicycleNumber ascending
                             select new GetBicycleOutList_PM
                             {
                                 ProduceID = s.ProduceID,
                                 BicycleNumber = s.BicycleNumber,   //车辆编号
                                 CustomerName = c.CustomerName,
                                 CreateTime = s.CreateTime
                             });
                if (query.Any())
                {
                    var tsk = new PagedList<GetBicycleOutList_PM>(query, model.PageIndex, model.PageSize, query.Count());
                    result.ResObject = new { Total = query.Count(), List = tsk };
                }
            }
            return result;
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel GetBicycleDetailByBicyleGuid(GetBicDeatilsModel_PM model)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var query = (from s in db.BicycleLockInfo
                             join b in db.BicycleBaseInfo on s.LockNumber equals b.LockNumber
                             join p in db.Province on s.ProvinceID equals p.Id into temp1
                             from pp in temp1.DefaultIfEmpty()
                             join c in db.City on s.CityID equals c.Id into temp2
                             from cc in temp2.DefaultIfEmpty()
                             join d in db.District on s.DistrictID equals d.Id into temp
                             from tt in temp.DefaultIfEmpty()

                             where s.LockGuid == model.BicycleBaseGuid && !s.IsDeleted

                             orderby s.CreateTime descending
                             select new GetBicDeatilsModel_OM
                             {
                                 BicycleNumber = b.BicycleNumber,  //车辆编号
                                 BicyCleTypeName = BicycleBaseInfoShow.GetBicycleTypeName(b.BicyCleTypeName), //车辆类型
                                 LockNumber = s.LockNumber,
                                 LockStatus = EnumRemarksHandler.GetBicycleState(int.Parse(s.LockStatus.ToString())),
                                 Voltage = s.Voltage,
                                 DistricName = string.Format("{0}{1}{2}", new object[] { pp.Name, cc.Name, tt.Name }),  //所在的区
                                 CreateTime = s.CreateTime
                             });

                var modelQuery = query.FirstOrDefault();
                var data = new GetBicDeatilsModel_OM
                {
                    BicycleNumber = modelQuery.BicycleNumber,  //车辆编号
                    BicyCleTypeName = modelQuery.BicyCleTypeName, //车辆类型
                    LockNumber = modelQuery.LockNumber,
                    LockStatus = modelQuery.LockStatus,
                    Voltage = modelQuery.Voltage,
                    DistricName = modelQuery.DistricName,  //所在的区
                    CreateTime = modelQuery.CreateTime
                };

                result.ResObject = data;
            }
            return result;
        }

        /// <summary>
        /// 批量生成车牌号-废弃
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ResultModel GetBicycleProduceBicNumber1(GetRandomNumModel_PM1 model)
        {
            var result = new ResultModel();
            int bicycleNumber = 1000;
            try
            {
                for (int i = 0; i < model.num; i++)
                {
                    using (var db = new MintBicycleDataContext())
                    {
                        var bicycleQuey = db.ProduceBicycleNumberInfo.FirstOrDefault(s => s.BicycleNumber == bicycleNumber && !s.IsDeleted);
                        if (bicycleQuey == null)
                        {
                            var Bicycle = new ProduceBicycleNumberInfo
                            {
                                BicycleNumber = bicycleNumber,
                                CustomerID = model.CustomerID,
                                CreateTime = DateTime.Now,
                                IsDeleted = false
                            };
                            db.ProduceBicycleNumberInfo.InsertOnSubmit(Bicycle);
                            db.SubmitChanges();
                            result.Message = "生成自行车编号成功！";
                            result.ResObject = true;
                        }
                        else
                        {
                            //var q = db.ProduceBicycleNumberInfo.Select(e => e).Max();
                            //var bicycleQuey = db.ProduceBicycleNumberInfo.FirstOrDefault(s => s.BicycleNumber == bicycleNumber && !s.IsDeleted);
                            //var Bicycle = new ProduceBicycleNumberInfo
                            //{
                            //    BicycleNumber = bicycleNumber,
                            //    CustomerID = model.CustomerID,
                            //    CreateTime = DateTime.Now,
                            //    IsDeleted = false
                            //};
                            //db.ProduceBicycleNumberInfo.InsertOnSubmit(Bicycle);
                            //db.SubmitChanges();
                            //result.Message = "生成自行车编号成功！";
                            //result.ResObject = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("批量生成自行车编号异常:" + ex.Message, ex);
                return new ResultModel { IsSuccess = false, Message = "批量生成自行车编号异常！" };
            }
            return result;
        }

        /// <summary>
        /// 批量生成车牌号
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ResultModel GetBicycleProduceBicNumber(GetRandomNumModel_PM model)
        {
            var result = new ResultModel();
            try
            {
                int[] gg = getRandomNum(model.num, model.minValue, model.maxValue);
                //int[] gg = getRandomNum(2, 1000000, 1000000000);
                for (int i = 0; i < gg.Length; i++)
                {
                    using (var db = new MintBicycleDataContext())
                    {
                        var bicycleQuey = db.ProduceBicycleNumberInfo.FirstOrDefault(s => s.BicycleNumber == gg[i] && !s.IsDeleted);
                        if (bicycleQuey == null)
                        {
                            var Bicycle = new ProduceBicycleNumberInfo
                            {
                                BicycleNumber = gg[i],
                                CustomerID = model.CustomerID,
                                CreateTime = DateTime.Now,
                                IsDeleted = false
                            };
                            db.ProduceBicycleNumberInfo.InsertOnSubmit(Bicycle);
                            db.SubmitChanges();
                            result.Message = "生成自行车编号成功！";
                            result.ResObject = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Error("批量生成自行车编号异常:" + ex.Message, ex);
                return new ResultModel { IsSuccess = false, Message = "批量生成自行车编号异常！" };
            }
            return result;
        }

      

        /// <summary>
        /// Random ra=new Random();  系统自动选取当前时前作随机种子：
        /// Random ra=new Random(6) 指定一个int型的参数作为随机种子;
        /// ra.Next(); 返回一个大于或等于零而小于2,147,483,647的随机数
        /// ra.Next(20);返回一个大于或等于零而小于20的随机数
        /// ra.Next(1,20); 返回一个大于或等于1而小于20之间的随机数
        /// 以下函数返回几个大于或等于某正整数(含0)而小于等于某正整数无重复的正整数.
        /// 示例 int[] a= getRandomNum(12,1,100); //在1-100间随机取12个不同的数并存于数组a
        /// 来自 俱会一处
        /// </summary>
        public int[] getRandomNum(int num, int minValue, int maxValue)
        {
            if ((maxValue + 1 - minValue - num < 0))
                maxValue += num - (maxValue + 1 - minValue);
            Random ra = new Random(unchecked((int)DateTime.Now.Ticks));
            int[] arrNum = new int[num];
            int tmp = 0;
            StringBuilder sb = new StringBuilder(num * maxValue.ToString().Trim().Length);

            for (int i = 0; i <= num - 1; i++)
            {
                tmp = ra.Next(minValue, maxValue);
                while (sb.ToString().Contains("#" + tmp.ToString().Trim() + "#"))
                    tmp = ra.Next(minValue, maxValue + 1);
                arrNum[i] = tmp;
                sb.Append("#" + tmp.ToString().Trim() + "#");
            }
            return arrNum;
        }

        /// <summary>
        ///生成制定位数的随机码（数字）
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GenerateRandomCode(int length)
        {
            var result = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                var r = new Random(Guid.NewGuid().GetHashCode());
                result.Append(r.Next(0, 10));
            }
            return result.ToString();
        }

        #region 生成随机数方法[暂时废弃]

        public int getNum(int[] arrNum, int tmp, int minValue, int maxValue, Random ra)
        {
            int n = 0;
            while (n <= arrNum.Length - 1)
            {
                if (arrNum[n] == tmp) //利用循环判断是否有重复
                {
                    tmp = ra.Next(minValue, maxValue); //重新随机获取。
                    getNum(arrNum, tmp, minValue, maxValue, ra);//递归:如果取出来的数字和已取得的数字有重复就重新随机获取。
                }
            }
            return tmp;
        }

        public static int[] GetRandom2(int minValue, int maxValue, int count)
        {
            int[] intList = new int[maxValue];
            for (int i = 0; i < maxValue; i++)
            {
                intList[i] = i + minValue;
            }
            int[] intRet = new int[count];
            int n = maxValue;
            Random rand = new Random();
            for (int i = 0; i < count; i++)
            {
                int index = rand.Next(0, n);
                intRet[i] = intList[index];
                intList[index] = intList[--n];
            }

            return intRet;
        }

        /// <summary>
        /// 方法2
        /// </summary>
        /// <param name="minValue"></param>
        /// <param name="maxValue"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static int[] GetRandom1(int minValue, int maxValue, int count)
        {
            Random rnd = new Random();
            int length = maxValue - minValue + 1;
            byte[] keys = new byte[length];
            rnd.NextBytes(keys);
            int[] items = new int[length];
            for (int i = 0; i < length; i++)
            {
                items[i] = i + minValue;
            }
            Array.Sort(keys, items);
            int[] result = new int[count];
            Array.Copy(items, result, count);
            return result;
        }

        private long GenerateIntID()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();

            return BitConverter.ToInt64(buffer, 0);
        }

        #endregion 生成随机数方法[暂时废弃]

        #endregion 批量生成车辆编号管理

        #region 分配客户车锁管理

        /// <summary>
        /// 客户车辆绑定列表
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public ResultModel CustomerBicycleLockList(CustomerBicycleLocklist_PM model)
        {
            var result = new ResultModel();
            string dt1 = model.StartTime.ToString("yyyy-MM-dd");
            DateTime startTime = DateTime.Parse(dt1);
            string dt2 = model.EndTime.ToString("yyyy-MM-dd");
            DateTime endTime = DateTime.Parse(dt2);

            using (var db = new MintBicycleDataContext())
            {
                try
                {
                    var query = (from c in db.CustomerRelevanceBicycleLockInfo
                                 join b in db.BicycleBaseInfo on c.BicycleGuid equals b.BicycleGuid
                                // join l in db.BicycleBaseInfo on c.LockGuid equals l.LockGuid
                                 join t in db.CustomerInfo on c.CustomerID equals t.CustomerID
                                 where ((string.IsNullOrEmpty(model.BicycleNumber)) || b.BicycleNumber == model.BicycleNumber)
                                 && ((string.IsNullOrEmpty(model.LockNumber)) || b.LockNumber.Contains(model.LockNumber))
                                 && ((string.IsNullOrEmpty(model.CustomerName)) || t.CustomerName.Contains(model.CustomerName))
                                  && (c.CreateTime.Date >= startTime.Date && c.CreateTime.Date <= endTime.Date)
                                 && !c.IsDeleted
                                 orderby c.CreateTime descending
                                 select new CustomerBicycleLocklist_OM
                                 {
                                     DistributionGuid = c.DistributionGuid,
                                     BicycleGuid = c.BicycleGuid,
                                     BicycleNumber = b.BicycleNumber,
                                     LockGuid = b.LockGuid,
                                     LockNumber = b.LockNumber,
                                     CustomerID = t.CustomerID,
                                     CustomerName = t.CustomerName,
                                     UpdateTime = c.UpdateTime,
                                     CreateTime = DateTime.Now
                                 });
                    if (query.Any())
                    {
                        var list = query;
                        var cnt = list.Count();
                        var tsk = new PagedList<CustomerBicycleLocklist_OM>(query, model.PageIndex, model.PageSize, query.Count());
                        result.ResObject = new { Total = query.Count(), List = tsk };
                    }
                }
                catch (Exception ex)
                {
                    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ChoiceExNoError, Message = ResPrompt.ChoiceExNoErrorMessage+ex.Message };
                }
            }
            return result;
        }

        /// <summary>
        /// 分配客户车锁信息
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public ResultModel AddOrUpdateCustomerBicycleLock(AddCustomerBicycleLockJson_PM Model)
        {
            var result = new ResultModel();
            string EditText = string.Empty;
            dt_manager_log LogModel = new dt_manager_log();
            string parameters = string.Empty;
            List<AddCustomerBicycleLock_PM> List = new List<AddCustomerBicycleLock_PM>();

            using (var db = new MintBicycleDataContext())
            {
                try
                {
                    //object str = JsonConvert.DeserializeObject(Model.StrJson);
                    // 反序列化为Person列表对象
                    List = JsonConvert.DeserializeObject<List<AddCustomerBicycleLock_PM>>(Model.StrJson);

                    if (List.Count > 0)
                    {
                        #region 循环分配客户车锁信息

                        foreach (var item in List)
                        {
                            if (!string.IsNullOrEmpty(item.BicycleGuid) && !string.IsNullOrEmpty(item.LockGuid) && item.CustomerID != 0)
                            {
                                if (item.DistributionGuid != null && item.Status==0)
                                {
                                    var DisQuery = db.CustomerRelevanceBicycleLockInfo.FirstOrDefault(p => p.DistributionGuid == item.DistributionGuid && !p.IsDeleted);
                                    if (DisQuery == null)
                                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.NoUserTransationCode, Message = ResPrompt.NoUserTransationMessage };

                                    DisQuery.BicycleGuid = Guid.Parse(item.BicycleGuid);
                                    DisQuery.LockGuid = Guid.Parse(item.LockGuid);
                                    DisQuery.CustomerID = item.CustomerID;
                                    //DisQuery.Status = item.Status;
                                    //DisQuery.Remark = item.Remark;
                                    DisQuery.UpdateBy = item.OperatorGuid;
                                    DisQuery.UpdateTime = DateTime.Now;
                                    DisQuery.IsDeleted = true;
                                    db.SubmitChanges();

                                    parameters = "BicycleGuid:" + item.BicycleGuid + ",LockGuid:" + item.LockGuid + ",CreateTime:" + DateTime.Now + ",CustomerID:" + item.CustomerID + ",OperatorGuid:" + item.OperatorGuid + "";
                                    EditText = "修改客户车锁分配信息成功";
                                    //操作日志记录
                                    LogModel.action_type = ActionEnum.Add.ToString();
                                    LogModel.remark = EditText + ",参数:" + parameters;
                                    LogModel.AdminGuid = item.OperatorGuid;
                                    _LogService.AddManagerLog(LogModel);

                                    result.ResObject = true;
                                }
                                else
                                {
                                    var DisQuery = db.CustomerRelevanceBicycleLockInfo.FirstOrDefault(p => p.BicycleGuid == Guid.Parse(item.BicycleGuid) && p.LockGuid == Guid.Parse(item.LockGuid) && p.CustomerID == item.CustomerID && !p.IsDeleted);
                                    if (DisQuery == null)
                                    {
                                        var CustomerBicLock = new CustomerRelevanceBicycleLockInfo
                                        {
                                            DistributionGuid = Guid.NewGuid(),
                                            BicycleGuid = Guid.Parse(item.BicycleGuid),
                                            LockGuid = Guid.Parse(item.LockGuid),
                                            CustomerID = item.CustomerID,
                                            Status = 1,
                                            //Remark = Model.Remark,
                                            CreateBy = item.OperatorGuid,
                                            CreateTime = DateTime.Now,
                                            IsDeleted = false
                                        };
                                        db.CustomerRelevanceBicycleLockInfo.InsertOnSubmit(CustomerBicLock);
                                        db.SubmitChanges();
                                    }
                                    else
                                    {
                                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.CustomerBicLockError, Message = ResPrompt.CustomerBicLockErrorMessage };
                                    }

                                    parameters = "BicycleGuid:" + item.BicycleGuid + ",LockGuid:" + item.LockGuid + ",CreateTime:" + DateTime.Now + ",CustomerID:" + item.CustomerID + ",OperatorGuid:" + item.OperatorGuid + "";
                                    EditText = "添加客户车锁分配信息成功";

                                    //操作日志记录
                                    LogModel.action_type = ActionEnum.Add.ToString();
                                    LogModel.remark = EditText + ",参数:" + parameters;
                                    LogModel.AdminGuid = item.OperatorGuid;
                                    _LogService.AddManagerLog(LogModel);

                                    result.ResObject = true;
                                }
                                //修改车辆表中的是否已分配状态
                                var BicQuery = db.BicycleBaseInfo.FirstOrDefault(x=>x.BicycleGuid== Guid.Parse(item.BicycleGuid) && x.LockGuid== Guid.Parse(item.LockGuid) && !x.IsDeleted);
                                if (BicQuery!=null)
                                {
                                    BicQuery.IsDistribution = 1;
                                    db.SubmitChanges();
                                }

                            }
                            else
                            {
                                return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ChoiceNoError, Message = ResPrompt.ChoiceNoErrorMessage };
                            }
                        }

                        #endregion 循环分配客户车锁信息
                    }
                }
                catch (Exception ex)
                {
                    EditText = "客户车锁分配出现异常";
                    //操作日志记录
                    LogModel.action_type = ActionEnum.Add.ToString();
                    LogModel.remark = EditText + ",参数:" + parameters + "," + ex.Message;
                    _LogService.AddManagerLog(LogModel);
                    result.ResObject = false;
                }

                #region 废弃

                //if (!string.IsNullOrEmpty(Model.BicycleGuid) && !string.IsNullOrEmpty(Model.LockGuid) && Model.CustomerID != 0)
                //{
                //    #region 循环分配客户车锁信息
                //    foreach (string itemBicGuid in BicycleGuid)
                //    {
                //        foreach (string itemLockGuid in LockGuid)
                //        {
                //            if (itemBicGuid != null && itemLockGuid != null)
                //            {
                //                var bicQuery = db.BicycleBaseInfo.FirstOrDefault(p => p.BicycleGuid == Guid.Parse(itemBicGuid) && p.LockGuid == Guid.Parse(itemLockGuid) && !p.IsDeleted);
                //                if(bicQuery!=null)
                //                {
                //                    #region 添加或修改
                //                    if (Model.DistributionGuid != null) //修改
                //                    {
                //                        var DisQuery = db.CustomerRelevanceBicycleLockInfo.FirstOrDefault(p => p.DistributionGuid == Model.DistributionGuid && !p.IsDeleted);
                //                        if (DisQuery == null)
                //                            return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.NoUserTransationCode, Message = ResPrompt.NoUserTransationMessage };

                //                        DisQuery.BicycleGuid = Guid.Parse(itemBicGuid);
                //                        DisQuery.LockGuid = Guid.Parse(itemLockGuid);
                //                        DisQuery.CustomerID = Model.CustomerID;
                //                        DisQuery.Status = Model.Status;
                //                        DisQuery.Remark = Model.Remark;
                //                        DisQuery.UpdateBy = Model.OperatorGuid;
                //                        DisQuery.UpdateTime = DateTime.Now;
                //                        db.SubmitChanges();

                //                        EditText = "修改客户车锁分配信息成功";
                //                        //操作日志记录
                //                        LogModel.action_type = ActionEnum.Add.ToString();
                //                        LogModel.remark = EditText + ",参数:" + parameters;
                //                        LogModel.AdminGuid = Model.OperatorGuid;
                //                        _LogService.AddManagerLog(LogModel);

                //                        result.ResObject = true;
                //                    }
                //                    else
                //                    {
                //                        var CustomerBicLock = new CustomerRelevanceBicycleLockInfo
                //                        {
                //                            DistributionGuid = Guid.NewGuid(),
                //                            BicycleGuid = Guid.Parse(itemBicGuid),
                //                            LockGuid = Guid.Parse(itemLockGuid),
                //                            CustomerID = Model.CustomerID,
                //                            Status = 1,
                //                            Remark = Model.Remark,
                //                            CreateBy = Model.OperatorGuid,
                //                            CreateTime = DateTime.Now,
                //                            IsDeleted = false
                //                        };
                //                        db.CustomerRelevanceBicycleLockInfo.InsertOnSubmit(CustomerBicLock);
                //                        db.SubmitChanges();

                //                        EditText = "添加客户车锁分配信息成功";

                //                        //操作日志记录
                //                        LogModel.action_type = ActionEnum.Add.ToString();
                //                        LogModel.remark = EditText + ",参数:" + parameters;
                //                        //LogModel.AdminGuid = Model.OperatorGuid;
                //                        _LogService.AddManagerLog(LogModel);

                //                        result.ResObject = true;
                //                    }
                //                    #endregion
                //                }
                //            }
                //        }
                //    }
                //    #endregion
                //}
                //else
                //{
                //    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ChoiceNoError, Message = ResPrompt.ChoiceNoErrorMessage };
                //}

                #endregion 废弃
            }
            return result;
        }

        #endregion 分配客户车锁管理

        #region 获取生产锁管理列表

        /// <summary>
        /// 获取生产锁管理列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ResultModel GetLockSupplierList(BicLock_PM data)
        {
            using (var db = new MintBicycleDataContext())
            {
                var query = from x in db.BicycleLockInfo
                            join b in db.BicycleBaseInfo on x.LockNumber equals b.LockNumber
                            join s in db.SupplierInfo on x.SupplierID equals s.SupplierID
                            join c in db.CustomerInfo on x.CustomerID equals c.CustomerID
                            where (x.LockNumber == data.LockNumber || string.IsNullOrEmpty(data.LockNumber)) || (c.CustomerName.Contains(data.CustormName) || string.IsNullOrEmpty(data.CustormName)) || (string.IsNullOrEmpty(data.SupplierName) || s.SupplierName.Contains(data.SupplierName))
                            select new BicLock_OM
                            {
                                BikeNumber = b.BicycleNumber,
                                CustomName = c.CustomerName,
                                LockNumber = x.LockNumber,
                                LockType = GetLockTypeName(x.LockType),
                                SupplierID = s.SupplierID,
                                SupplierName = s.SupplierName,
                                ProductTime = x.CreateTime,
                                Remark = x.Remark
                            };

                if (query.Any())
                {
                    var res = new PagedList<BicLock_OM>(query, data.PageIndex, data.PageSize).OrderByDescending(x => x.ProductTime);

                    return new ResultModel { ResObject = new { Total = query.Count(), List = res } };
                }

                return new Utils.ResultModel { Message = "暂无锁的信息" };
            }
        }

        /// <summary>
        /// 锁的类型
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public string GetLockTypeName(int? a)
        {
            var name = string.Empty;
            switch (a)
            {
                case 0:
                    name = "蓝牙锁";
                    break;

                default:
                    name = "其他";
                    break;
            }

            return name;
        }

        #endregion 获取生产锁管理列表
    }
}