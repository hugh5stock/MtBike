using MintCyclingData;
using MintCyclingService.BaiDu;
using MintCyclingService.Common;
using MintCyclingService.Cycling;
using MintCyclingService.Photo;
using MintCyclingService.ServicePerson;
using MintCyclingService.Utils;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using Utility;
using Utility.LogHelper;
using static MintCyclingService.Cycling.BaiduModel;

namespace MintCyclingService.Breakdown
{
    public class BreakdownService : IBreakdownService
    {
        private ICyclingService cycService = new CyclingService();
        private IBaiduService baiduService = new BaiduService();

        #region 后台故障维护接口

        /// <summary>
        /// 根据查询条件搜索故障维护列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel GetBreakDownList(GetBreakdownList_PM model)
        {
            var result = new ResultModel();
            var region = Config.UserRegion;
            if (region == null)
                return result;
            using (var db = new MintBicycleDataContext())
            {
                var query = (from s in db.BreakdownLog
                             join t in db.District on s.DistrictID equals t.Id into temp
                             from tt in temp.DefaultIfEmpty()
                             where ((string.IsNullOrEmpty(model.BicyCleNumber)) || s.BicyCleNumber.Contains(model.BicyCleNumber))
                             && ((model.ProvinceID == 0 || s.ProvinceID == model.ProvinceID)
                             && (model.CityID == 0 || s.CityID == model.CityID)
                             && (model.DistrictID == 0 || s.DistrictID == model.DistrictID))
                             && !s.IsDeleted && s.Status != 1
                             && (region.ExceptUserRegion || (s.ProvinceID == region.UserProvince && (region.UserCity == null || s.CityID == region.UserCity)
                                   && (region.UserDistrict == null || s.DistrictID == region.UserDistrict)))
                             orderby s.CreateTime descending
                             select new GetBreakdownList_OM
                             {
                                 BreakdownGuid = s.BreakdownGuid,
                                 BicyCleNumber = s.BicyCleNumber,
                                 BreakTypeName = s.BreakTypeName == null ? string.Empty : s.BreakTypeName.ToString(),
                                 GradeName = s.GradeTypeName.ToString(),
                                 DistricName = tt.Name,  //所在的区
                                 Longitude = s.Longitude,
                                 Latitude = s.Latitude,
                                 Status = s.Status,
                                 Address = s.Address,
                                 Remark = s.Remark,
                                 PhotoGuid = s.BreakDownPhotoGuid,
                                 BreakDownTime = s.CreateTime
                             });
                if (query.Any())
                {
                    var list = query.OrderByDescending(x => x.BreakDownTime).ToList();
                    var cnt = list.Count();
                    var tsk = new PagedList<GetBreakdownList_OM>(list, model.PageIndex, model.PageSize, cnt);
                    var qk = tsk.ToList();

                    result.ResObject = new listModel_OM
                    {
                        Total = tsk.TotalCount,
                        List = qk
                    };

                    // result.ResObject = new { Total = cnt, List = tsk };
                    //return new ResultModel { ResObject = new BreakdownListreturn { Total = tsk.TotalCount, List = list } };
                }
            }
            return result;
        }

        /// <summary>
        /// 查询故障维护详情
        /// </summary>
        /// <param name="BreakdownGuid"></param>
        /// <returns></returns>
        public ResultModel GetBreakDownDetail(System.Guid BreakdownGuid)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var query = db.BreakdownLog.FirstOrDefault(s => s.BreakdownGuid == BreakdownGuid);
                if (query == null)
                    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.BreakDownNotExist, Message = ResPrompt.BreakDownNotExistMessage };
                var func = new Func<Guid?, List<string>>(t =>
                {
                    var list = new List<string>();
                    if (t == null) return list;
                    var pic = db.BreakdownPhotoInfo.Where(p => p.BreakDownPhotoGuid == t);
                    var tk = ConfigurationManager.AppSettings["BreakdownUploadLoc"] ?? "";
                    foreach (var item in pic)
                    {
                        list.Add(tk + item.Url);
                    }
                    return list;
                });

                var data = new GetBreakdownDetail_OM
                {
                    BreakdownGuid = query.BreakdownGuid,
                    BicyCleNumber = query.BicyCleNumber,
                    BreakTypeNameID = query.BreakTypeName == null ? -1 : query.BreakTypeName,
                    GradeNameID = query.GradeTypeName == null ? -1 : int.Parse(query.GradeTypeName),
                    //DistricName = tt.Name,  //所在的区
                    Longitude = query.Longitude,
                    Latitude = query.Latitude,
                    Status = query.Status,
                    Address = query.Address,
                    Remark = query.Remark,
                    PicLoc = func(query.BreakDownPhotoGuid)
                };
                result.ResObject = data;
            }
            return result;
        }

        /// <summary>
        /// 新增或修改故障维护信息
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public ResultModel AddOrUpdateBreakDown(AddOrUpdateBreakdown_PM model)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                if (model.BreakdownGuid == null)
                {
                    var breakdown = new BreakdownLog
                    {
                        BreakdownGuid = Guid.NewGuid(),
                        BicyCleNumber = model.BicyCleNumber,
                        //BreakTypeNameID = model.BreakTypeNameID,
                        //GradeNameID = model.GradeNameID,
                        Longitude = model.Longitude,
                        Latitude = model.Latitude,
                        Status = model.Status,
                        ProvinceID = 0,
                        CityID = 0,
                        DistrictID = 0,
                        //ReportPerson=,
                        Address = model.Address,
                        Remark = model.Remark,
                        CreateBy = model.OperatorGuid,
                        UpdateTime = DateTime.Now,
                        CreateTime = DateTime.Now,
                        IsDeleted = false
                    };
                    db.BreakdownLog.InsertOnSubmit(breakdown);
                    db.SubmitChanges();
                }
                else  //修改
                {
                    var query = db.BreakdownLog.FirstOrDefault(s => s.BreakdownGuid == model.BreakdownGuid && !s.IsDeleted);
                    if (query == null)
                    {
                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.BreakDownNotExist, Message = ResPrompt.BreakDownNotExistMessage };
                    }
                    query.BicyCleNumber = model.BicyCleNumber;
                    //query.BreakTypeNameID = model.BreakTypeNameID;
                    //query.GradeNameID = model.GradeNameID;
                    query.Longitude = model.Longitude;
                    query.Latitude = model.Latitude;
                    query.Status = model.Status;
                    query.ProvinceID = 0;
                    query.CityID = 0;
                    query.DistrictID = 0;
                    query.Address = model.Address;
                    query.Remark = model.Remark;
                    query.UpdateBy = model.OperatorGuid;
                    query.UpdateTime = DateTime.Now;
                    db.SubmitChanges();
                }
                result.ResObject = true;
            }
            return result;
        }

        /// <summary>
        /// 导入Excel文件
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel ImportExcelFileData(ImportExcelData_PM model)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                if (string.IsNullOrEmpty(model.exportFile))
                {
                    result.IsSuccess = false;
                    result.Message = "请上传文件后导入!";
                }
                try
                {
                    IWorkbook wk = null;
                    FileStream fs = System.IO.File.OpenRead(HttpContext.Current.Server.MapPath(model.exportFile));
                    //把xls文件中的数据写入wk中
                    wk = new HSSFWorkbook(fs);
                    fs.Close();
                    //读取当前表数据
                    ISheet sheet = wk.GetSheetAt(0);

                    IRow row = sheet.GetRow(0);  //读取当前行数据
                                                 //LastRowNum 是当前表的总行数-1（注意）
                    int offset = 0;
                    for (int i = 1; i <= sheet.LastRowNum; i++)
                    {
                        row = sheet.GetRow(i);  //读取当前行数据
                        if (row != null)
                        {
                            for (int j = 0; j < row.LastCellNum; j++)
                            {
                                //Model.HUR_TelCallRecord model = new BLL.HUR_TelCallRecordBLL().GetModelBy(row.GetCell(1).ToString(), Convert.ToDateTime(row.GetCell(3).DateCellValue));
                                //if (model == null)
                                //{
                                PayTypeInfo payModel = new PayTypeInfo();
                                payModel.PayTypeGuid = Guid.NewGuid();
                                payModel.PayTypeName = row.GetCell(0).ToString();
                                db.PayTypeInfo.InsertOnSubmit(payModel);
                                db.SubmitChanges();
                                //    new BLL.HUR_TelCallRecordBLL().Add(model);
                                //}
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return new ResultModel { IsSuccess = false, MsgCode = "0", Message = "导入格式不正确" };
                }
                result.IsSuccess = true;
                result.Message = "导入成功!";
            }
            return result;
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <param name="BicyCleNumber"></param>
        /// <param name="ProvinceID"></param>
        /// <param name="CityID"></param>
        /// <param name="DistrictID"></param>
        public void ExportBreakDownData(string BicyCleNumber, int ProvinceID, int CityID, int DistrictID)
        {
            //  DataSet list = BLL.Account_MoneyRecord.GetList(state, moneytype, beginDate, endDate, username);
            HSSFWorkbook book = new HSSFWorkbook();
            using (var db = new MintBicycleDataContext())
            {
                var breakDowns = db.BreakdownLog.Where(q => !q.IsDeleted);
                if (breakDowns.Any())
                {
                    ISheet sheet = book.CreateSheet("故障维护信息");

                    IRow rowa = sheet.CreateRow(0);
                    rowa.CreateCell(0).SetCellValue("车辆编号");
                    rowa.CreateCell(1).SetCellValue("维修等级");
                    rowa.CreateCell(2).SetCellValue("故障类型");
                    rowa.CreateCell(3).SetCellValue("所属区域");
                    rowa.CreateCell(4).SetCellValue("详细地址");
                    rowa.CreateCell(5).SetCellValue("经度");
                    rowa.CreateCell(6).SetCellValue("纬度");
                    rowa.CreateCell(7).SetCellValue("备注");

                    int i = 0;
                    foreach (var log in breakDowns)
                    {
                        i++;
                        NPOI.SS.UserModel.IRow row = sheet.CreateRow(i + 1);
                        row.CreateCell(1).SetCellValue(log.BicyCleNumber);
                        row.CreateCell(1).SetCellValue(log.Remark);
                        row.CreateCell(2).SetCellValue(log.BicyCleNumber);
                        row.CreateCell(3).SetCellValue(log.Remark);
                        row.CreateCell(4).SetCellValue(log.Remark);
                        row.CreateCell(5).SetCellValue((double)(log.Longitude ?? 0));
                        row.CreateCell(6).SetCellValue((double)(log.Latitude ?? 0));
                        row.CreateCell(7).SetCellValue(log.Remark);
                    }
                }
            }

            // 写入到客户端
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            //Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xls", DateTime.Now.ToString("yyyyMMddHHmmssfff")));
            //Response.BinaryWrite(ms.ToArray());
            //book = null;
            //ms.Close();
            //ms.Dispose();
        }

        #endregion 后台故障维护接口

        #region 客户端API故障维护接口

        /// <summary>
        /// 故障上报
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public ResultModel AddBreakDownLog(AddBreakdownLog_PM model)
        {
            DateTime dtNow = DateTime.Now;
            string provinceName = string.Empty;
            string cityName = string.Empty;
            string districtName = string.Empty;
            string Address = string.Empty;
            string BreakTypeName = string.Empty;

            int ProvinceID = 0;
            int cityID = 0;
            int districtID = 0;

            var result = new ResultModel();

            using (var db = new MintBicycleDataContext())
            {
                //车辆基本信息
                var query = from x in db.BicycleBaseInfo
                            join L in db.BicycleLockInfo on x.LockNumber equals L.LockNumber
                            where x.BicycleNumber == model.BicyCleNumber && !x.IsDeleted && !L.IsDeleted
                            select L;
                if (!query.Any())
                {
                    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.BicycleNotExist, Message = ResPrompt.BicycleNotExistMessage };
                }
                var fir = query.FirstOrDefault();

                var BreakLog = db.BreakdownLog.FirstOrDefault(s => s.BicyCleNumber == model.BicyCleNumber && !s.IsDeleted);
                if (BreakLog != null)
                {
                    if (BreakLog.BicyCleNumber == model.BicyCleNumber && BreakLog.ServicePersonID == model.UserInfoGuid && BreakLog.Longitude == model.Longitude && BreakLog.Latitude == model.Latitude)
                    {
                        return new ResultModel { IsSuccess = false, MsgCode = "0", Message = "您已提交故障上报信息，不需要重复提交！" };
                    }
                }

                #region 调用百度API--更新车辆基本信息表

                try
                {
                    //详细地址
                    Address = baiduService.GetAddress(model.Longitude.ToString(), model.Latitude.ToString());
                    List<AddressModel> list = baiduService.GetAddressInfo(model.Longitude.ToString(), model.Latitude.ToString());
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
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error("单车客户端故障上报调用百度API--更新车辆基本信息异常：" + ex.Message);
                }

                #endregion 调用百度API--更新车辆基本信息表

                try  //故障类型：0无法开关锁、1无法骑行、2无法结算、3二维码受损、4其它问题
                {
                    foreach (var item in model.BreakTypeName)
                    {
                        var breakdown = new BreakdownLog
                        {
                            BreakdownGuid = Guid.NewGuid(),
                            BicyCleNumber = model.BicyCleNumber,
                            //0无法开关锁、1无法骑行、2无法结算、3二维码受损、4其它问题
                            BreakTypeName = item,
                            BreakDownPhotoGuid = model.BreakdownPhotoGuid,
                            Longitude = model.Longitude,
                            Latitude = model.Latitude,
                            Status = 0,                            //默认故障
                            ReportSource = 0,                      //0：绿帝出行app故障上报，1维护app上报
                            ProvinceID = ProvinceID,
                            CityID = cityID,
                            DistrictID = districtID,
                            Address = Address,
                            Remark = model.Remark,
                            CreateBy = model.UserInfoGuid,        //上报人Guid
                            CreateTime = dtNow,
                            IsDeleted = false
                        };
                        db.BreakdownLog.InsertOnSubmit(breakdown);
                    }

                    db.SubmitChanges();

                    #region 修改车辆信息

                    //车辆基本信息

                    fir.ProvinceID = ProvinceID;
                    fir.CityID = cityID;
                    fir.DistrictID = districtID;
                    fir.Longitude = model.Longitude != null ? model.Longitude : fir.Longitude;
                    fir.Latitude = model.Latitude != null ? model.Latitude : fir.Latitude;
                    fir.Address = Address;
                    //query.LockStatus = 3;
                    //query.ReservationStatus = 2;
                    fir.UpdateTime = dtNow;
                    db.SubmitChanges();

                    result.IsSuccess = true;
                    result.MsgCode = "0";
                    result.Message = "故障上报成功";
                }
                catch (Exception ex)
                {
                    LogHelper.Error("添加故障维护信息异常：" + ex.Message);
                    return new ResultModel { IsSuccess = false, MsgCode = "0", Message = "添加故障维护信息异常" };
                }

                #endregion 修改车辆信息
            }
            return result;
        }

        /// <summary>
        /// 新增故障照片照片
        /// </summary>
        /// <param name="title">照片标题</param>
        /// <param name="photoTypeEnum">照片类型</param>
        /// <param name="fileName">文件名</param>
        /// <param name="url">文件路径</param>
        /// <returns>新增照片</returns>
        public ResultModel AddPhoto(string title, PhotoTypeEnum photoType, string fileName, string url, Guid breakdownPhotoGuid)
        {
            var result = new ResultModel();
            var now = DateTime.Now;
            try
            {
                using (var db = new MintBicycleDataContext())
                {
                    var newPhoto = new BreakdownPhotoInfo();
                    newPhoto.PhotoGuid = Guid.NewGuid();
                    newPhoto.Title = title;
                    newPhoto.Type = (byte)photoType;
                    newPhoto.FileName = fileName;
                    newPhoto.Url = url;
                    newPhoto.IsDeleted = false;
                    newPhoto.CreateTime = now;
                    if (breakdownPhotoGuid == Guid.Empty)
                    {
                        newPhoto.BreakDownPhotoGuid = Guid.NewGuid();
                    }
                    else
                    {
                        newPhoto.BreakDownPhotoGuid = breakdownPhotoGuid;
                    }

                    db.BreakdownPhotoInfo.InsertOnSubmit(newPhoto);
                    db.SubmitChanges();

                    result.ResObject = new PhotoBreak_OM { PhotoGuid = newPhoto.PhotoGuid, BreakDownPhotoGuid = newPhoto.BreakDownPhotoGuid };
                }
            }
            catch (Exception ex)
            {
                return new ResultModel { IsSuccess = false, MsgCode = "4042", Message = "添加图片信息出现错误" };
            }
            return result;
        }

        /// <summary>
        /// 新增持工证照片照片
        /// </summary>
        /// <param name="title">照片标题</param>
        /// <param name="photoTypeEnum">照片类型</param>
        /// <param name="fileName">文件名</param>
        /// <param name="url">文件路径</param>
        /// <returns>新增照片</returns>
        public ResultModel AddICBCPhoto(string title, PhotoTypeEnum photoType, string fileName, string url)
        {
            var result = new ResultModel();
            var now = DateTime.Now;
            try
            {
                using (var db = new MintBicycleDataContext())
                {
                    var newPhoto = new MintCyclingData.ServicePersonPhoto();
                    newPhoto.PhotoGuid = Guid.NewGuid();
                    newPhoto.Title = title;
                    newPhoto.Type = (byte)photoType;
                    newPhoto.FileName = fileName;
                    newPhoto.Url = url;
                    newPhoto.IsDeleted = false;
                    newPhoto.CreateTime = now;

                    db.ServicePersonPhoto.InsertOnSubmit(newPhoto);
                    db.SubmitChanges();

                    result.ResObject = new PhotoBreak_OM { PhotoGuid = newPhoto.PhotoGuid };
                }
            }
            catch (Exception ex)
            {
                return new ResultModel { IsSuccess = false, MsgCode = "4042", Message = "添加图片信息出现错误" };
            }
            return result;
        }

        ///// <summary>
        ///// 查询当前维修人员的维修已完成的记录信息
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //public ResultModel GetBreakDownListByServicePersonID(GetBreakList_PM model)
        //{
        //    var result = new ResultModel();
        //    using (var db = new MintBicycleDataContext())
        //    {
        //        var query = (from s in db.BreakdownLog
        //                     join t in db.District on s.DistrictID equals t.Id into temp
        //                     from tt in temp.DefaultIfEmpty()
        //                     where  && !s.IsDeleted
        //                     orderby s.CreateTime descending
        //                     select new GetBreakdownList_OM
        //                     {
        //                         BreakdownGuid = s.BreakdownGuid,
        //                         BicyCleNumber = s.BicyCleNumber,
        //                         BreakTypeName = BreakDownState.GetBreakDownState(int.Parse(s.BreakTypeNameID.ToString())),
        //                         //GradeName = GreaGradeNameState.GetGradeNameState(int.Parse(s.GradeNameID.ToString())),
        //                         DistricName = tt.Name,  //所在的区
        //                         Longitude = s.Longitude,
        //                         Latitude = s.Latitude,
        //                         Status = s.Status,
        //                         Address = s.Address,
        //                         Remark = s.Remark
        //                     });
        //        if (query.Any())
        //        {
        //            var list = query;
        //            var cnt = list.Count();
        //            var tsk = new PagedList<GetBreakdownList_OM>(query, model.PageIndex, model.PageSize, cnt);
        //            result.ResObject = new listModel_OM
        //            {
        //                Total = tsk.TotalCount,
        //                List = tsk.ToList()
        //            };

        //            // result.ResObject = new { Total = cnt, List = tsk };
        //            //return new ResultModel { ResObject = new BreakdownListreturn { Total = tsk.TotalCount, List = list } };
        //        }
        //    }
        //    return result;

        //}

        #endregion 客户端API故障维护接口

        #region Candy

        /// <summary>
        /// 查看自己管辖范围内所有车，故障车辆
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ResultModel GetBreakdownBikeByUser(GetCarModel_PM data)
        {
            using (var db = new MintBicycleDataContext())
            {
                try
                {
                    var queryA = from x in db.BicycleBaseInfo
                                 join z in db.BicycleLockInfo on x.LockNumber equals z.LockNumber
                                 where !x.IsDeleted
                                 select z;
                    var count = 0;

                    if (queryA.Any())
                    {
                        if (data.ProvinceID > 0)
                        {
                            queryA = queryA.Where(z => z.ProvinceID == data.ProvinceID);
                        }

                        if (data.CityID > 0)
                        {
                            queryA = queryA.Where(z => z.CityID == data.CityID);
                        }

                        if (data.DistinctId > 0)
                        {
                            queryA = queryA.Where(z => z.DistrictID == data.DistinctId);
                        }
                        count = queryA.Count();
                    }

                    var query = from x in db.BreakdownLog
                                where !x.IsDeleted && x.Status != (int)BikeStatusEnum.Good
                                select x;

                    if (data.ProvinceID > 0)
                    {
                        query = query.Where(x => x.ProvinceID == data.ProvinceID);
                    }

                    if (data.CityID > 0)
                    {
                        query = query.Where(x => x.CityID == data.CityID);
                    }

                    if (data.DistinctId > 0)
                    {
                        query = query.Where(x => x.DistrictID == data.DistinctId);
                    }

                    var DownCount = query.Select(x => x.BicyCleNumber).Distinct().Count();

                    var todaycount = query.Where(x => x.CreateTime.Value.Date == DateTime.Now.Date).Select(x => x.BicyCleNumber).Distinct().Count();

                    return new ResultModel { ResObject = new { AllCount = count, AllBreakCount = DownCount, TodayCount = todaycount } };
                }
                catch (Exception ex)
                {
                    return new Utils.ResultModel { IsSuccess = false, Message = ex.Message };
                }
            }
        }

        /// <summary>
        /// 获取故障列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ResultModel GetBreakDownList(BreakDownList_PM data)
        {
            using (var db = new MintBicycleDataContext())
            {
                try
                {
                    var query = from x in db.BreakdownLog
                                where x.Status != (int)ServiceStatus.Normal && !x.IsDeleted && x.CityID == data.CityId && x.DistrictID == data.DistinctId && (x.Address.Contains(data.KeyWord) || string.IsNullOrEmpty(data.KeyWord) || x.BicyCleNumber.Contains(data.KeyWord))

                                select x;

                    if (data.TodayBreakDown)
                    {
                        query = query.Where(x => x.CreateTime.Value.Date == DateTime.Now.Date);
                    }
                    if (data.BreakDownStartTime.HasValue)
                    {
                        query = query.Where(x => x.CreateTime.Value.Date >= data.BreakDownStartTime.Value.Date);
                    }
                    if (data.BreakDownEndTime.HasValue)
                    {
                        query = query.Where(x => x.CreateTime.Value.Date <= data.BreakDownEndTime.Value.Date);
                    }

                    var func = new Func<string, List<int?>>(a =>
                    {
                        var type = query.Where(x => x.BicyCleNumber == a).Select(x => x.BreakTypeName).Distinct();

                        return type.ToList();
                    });

                    if (query.Any())
                    {
                        var list = query.GroupBy(x => new { x.BicyCleNumber, x.BreakTypeName }).Select(x => new { value = x.OrderByDescending(z => z.CreateTime).FirstOrDefault() });

                        var Rlist = new List<BreakDownList_OM>();
                        var returnlist = new List<BreakDownList>();
                        if (data.Latitude != null && data.Longitude != null)
                        {
                            foreach (var item in list)
                            {
                                var Ritem = new BreakDownList_OM();

                                Ritem.BreakDownGuid = item.value.BreakdownGuid;
                                Ritem.BikeNumber = item.value.BicyCleNumber;

                                Ritem.Distance = Math.Round(CoordinateHelper.QDistance(new Coordinates(data.Longitude ?? 0.00M, data.Latitude ?? 0.00M), new Coordinates(item.value.Longitude ?? decimal.MinValue, item.value.Latitude ?? decimal.MinValue)) / 1000);
                                Ritem.Latitude = item.value.Latitude;
                                Ritem.Longitude = item.value.Longitude;
                                Ritem.DistictName = item.value.Address;
                                Ritem.ReportTime = item.value.CreateTime;
                                Rlist.Add(Ritem);
                            }

                            var reslist = Rlist.GroupBy(x => x.BikeNumber).Select(x => x.OrderByDescending(z => z.ReportTime).FirstOrDefault()).ToList();

                            var pagelist = new PagedList<BreakDownList_OM>(reslist, data.PageIndex, data.PageSize);

                            foreach (var item in pagelist)
                            {
                                var sig = new BreakDownList();
                                sig.BikeNumber = item.BikeNumber;
                                sig.BreakDownGuid = item.BreakDownGuid;
                                sig.BreakDownType = func(item.BikeNumber);
                                sig.Distance = item.Distance;
                                sig.DistictName = item.DistictName;
                                sig.Latitude = item.Latitude;
                                sig.Longitude = item.Longitude;
                                sig.ReportTime = item.ReportTime;
                                returnlist.Add(sig);
                            }

                            return new ResultModel { ResObject = returnlist };
                        }
                        else
                        {
                            foreach (var item in list)
                            {
                                var Ritem = new BreakDownList_OM();

                                Ritem.BikeNumber = item.value.BicyCleNumber;
                                Ritem.BreakDownGuid = item.value.BreakdownGuid;
                                Ritem.ReportTime = item.value.CreateTime;
                                Ritem.DistictName = item.value.Address;
                                Ritem.Latitude = item.value.Latitude;
                                Ritem.Longitude = item.value.Longitude;
                                Rlist.Add(Ritem);
                            }

                            var reslist = Rlist.GroupBy(x => x.BikeNumber).Select(x =>
                          {
                              return x.OrderByDescending(z => z.ReportTime).FirstOrDefault();
                          }).ToList();

                            var pagelist = new PagedList<BreakDownList_OM>(reslist, data.PageIndex, data.PageSize);
                            foreach (var item in pagelist)
                            {
                                var sig = new BreakDownList();
                                sig.BikeNumber = item.BikeNumber;
                                sig.BreakDownGuid = item.BreakDownGuid;
                                sig.BreakDownType = func(item.BikeNumber);
                                sig.DistictName = item.DistictName;
                                sig.Latitude = item.Latitude;
                                sig.Longitude = item.Longitude;
                                sig.ReportTime = item.ReportTime;
                                returnlist.Add(sig);
                            }

                            return new ResultModel { ResObject = returnlist };
                        }
                    }
                    return new ResultModel { IsSuccess = false, Message = "无故障车辆" };
                }
                catch (Exception ex)
                {
                    LogHelper.Error("获取故障列表信息异常：" + ex.Message);
                    return new ResultModel { IsSuccess = false, Message = ex.Message };
                }
            }
        }

        #region 放弃
        ///// <summary>
        ///// 根据条件获取故障车辆列表
        ///// </summary>
        ///// <param name="data"></param>
        ///// <returns></returns>
        //public ResultModel GetBreakDownListBycondition(BreakDownCondition data)
        //{
        //    using (var db = new MintBicycleDataContext())
        //    {
        //        try
        //        {
        //            var query = from x in db.BreakdownLog
        //                        where !x.IsDeleted && x.Status != (int)ServiceStatus.Normal && (x.Address.Contains(data.KeyWord) || string.IsNullOrEmpty(data.KeyWord) || x.BicyCleNumber.Contains(data.KeyWord))
        //                        select x;

        //            if (data.ProvinceId > 0)
        //            {
        //                query = query.Where(x => x.ProvinceID == data.ProvinceId);
        //            }
        //            if (data.CityId > 0)
        //            {
        //                query = query.Where(x => x.CityID == data.CityId);

        //            }
        //            if (data.DistinctId > 0)
        //            {
        //                query = query.Where(x => x.DistrictID == data.DistinctId);
        //            }

        //            if (data.BreakdownType != BreakdownEnum.BreakDown_ALL)
        //            {
        //                query = query.Where(x => x.BreakTypeName == (int)data.BreakdownType);
        //            }

        //            if (data.StartReportTime.HasValue)
        //            {
        //                query = query.Where(x => x.CreateTime.Value.Date >= data.StartReportTime);
        //            }
        //            if (data.EndReportTime.HasValue)
        //            {
        //                query = query.Where(x => x.CreateTime.Value.Date <= data.EndReportTime);
        //            }

        //            if (!query.Any())
        //            {
        //                return new ResultModel { IsSuccess = false, Message = "暂无此查询故障车辆" };
        //            }

        //            var list = query.ToList();
        //            var Rlist = new List<BreakDownList_OM>();

        //            if (data.Latitude != null && data.Longitude != null)
        //            {
        //                foreach (var item in list)
        //                {
        //                    var Ritem = new BreakDownList_OM();

        //                    Ritem.BreakDownGuid = item.BreakdownGuid;
        //                    Ritem.BikeNumber = item.BicyCleNumber;
        //                    Ritem.BreakDownType = BreakDownState.GetBreakDownState(item.BreakTypeName ?? 4);
        //                    Ritem.Distance = Math.Round(CoordinateHelper.QDistance(new Coordinates(data.Longitude ?? Decimal.MinValue, data.Latitude ?? decimal.MinValue), new Coordinates(item.Longitude ?? decimal.MinValue, item.Latitude ?? decimal.MinValue)) / 1000);
        //                    Ritem.Latitude = item.Latitude;
        //                    Ritem.Longitude = item.Longitude;
        //                    Ritem.DistictName = item.Address;
        //                    Ritem.ReportTime = item.CreateTime;
        //                    Rlist.Add(Ritem);
        //                }

        //                return new ResultModel { ResObject = new PagedList<BreakDownList_OM>(Rlist.OrderBy(x => x.Distance).ToList(), data.PageIndex, data.PageSize) };
        //            }
        //            else
        //            {
        //                foreach (var item in list)
        //                {
        //                    var Ritem = new BreakDownList_OM();
        //                    Ritem.BreakDownGuid = item.BreakdownGuid;
        //                    Ritem.BikeNumber = item.BicyCleNumber;
        //                    Ritem.BreakDownType = BreakDownState.GetBreakDownState(item.BreakTypeName ?? 4);
        //                    Ritem.ReportTime = item.CreateTime;
        //                    Ritem.DistictName = item.Address;
        //                    Ritem.Latitude = item.Latitude;
        //                    Ritem.Longitude = item.Longitude;
        //                    Rlist.Add(Ritem);
        //                }

        //                return new ResultModel { ResObject = Rlist };
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            return new Utils.ResultModel { IsSuccess = false, Message = ex.Message };
        //        }
        //    }
        //}
        #endregion 放弃

        /// <summary>
        /// 位置上报
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ResultModel PositionReport(PositionReport_PM data)
        {
            using (var db = new MintBicycleDataContext())
            {
                //try
                //{
                var date = DateTime.Now;

                var BLMatchQ = from x in db.BicycleBaseInfo
                               where x.BicycleNumber == data.BikeNumber && x.LockNumber == data.LockNumber
                               select x;
                if (!BLMatchQ.Any())
                {
                    return new ResultModel { IsSuccess = false, Message = "车辆编号和锁还没有匹配入库" };
                }

                if (data.LockLat != null || data.LockLng != null || data.LockLat != 0 || data.LockLng != 0)
                {
                    //var sQuery = from x in db.SIMCard
                    //             where x.LockNumber == data.LockNumber && x.SIMNumber == data.SIMNumber
                    //             select x;
                    //if (sQuery.Any())
                    //{
                    //    var fir = sQuery.FirstOrDefault();

                    //    fir.Status = 1;
                    //    fir.UpdateTime = date;
                    //    fir.UpdateBy = data.UserInfoGuid;
                    //    fir.ActivationTime = date;
                    //}
                    //else
                    //{
                    //    var ins = new MintCyclingData.SIMCard();
                    //    ins.ActivationTime = date;
                    //    ins.CreateBy = data.UserInfoGuid;
                    //    ins.CreateTime = date;
                    //    ins.Guid = Guid.NewGuid();
                    //    ins.IsDeleted = false;
                    //    ins.LockNumber = data.LockNumber;
                    //    ins.SIMNumber = data.SIMNumber;
                    //    ins.Status = 1;
                    //    db.SIMCard.InsertOnSubmit(ins);
                    //}

                    //db.SubmitChanges();

                    //return new ResultModel { IsSuccess = false, Message = "锁定位异常" };

                    var queryL = from x in db.BicycleLockInfo
                                 where x.LockNumber == data.LockNumber
                                 select x;
                    if (!queryL.Any())
                    {
                        return new ResultModel { IsSuccess = false, Message = "此单车不存在" };
                    }

                    //CyclingService cyc = new CyclingService();

                    var listL = baiduService.GetBaiDuProvinceID(new BaiduApiModel { Latitude = data.LockLat.ToString(), Longitude = data.LockLng.ToString() });
                    string ADDRESSL = baiduService.GetAddress(data.LockLng.ToString(), data.LockLat.ToString());
                    var fir = queryL.FirstOrDefault();
                    fir.ProvinceID = listL[0].ProvinceID;
                    fir.CityID = listL[0].cityID;
                    fir.DistrictID = listL[0].districtID;
                    fir.Address = ADDRESSL;
                    fir.Latitude = data.LockLat;
                    fir.Longitude = data.LockLng;
                    fir.UpdateTime = DateTime.Now;
                    db.SubmitChanges();
                    return new ResultModel { ResObject = true };
                }
                if (data.MobileLat == null || data.MobileLng == null)
                {
                    return new ResultModel { IsSuccess = false, Message = "手机定位异常" };
                }

                var queryM = from x in db.BicycleLockInfo
                             where x.LockNumber == data.LockNumber
                             select x;
                if (!queryM.Any())
                {
                    return new ResultModel { IsSuccess = false, Message = "此单车不存在" };
                }

                // CyclingService cyc1 = new CyclingService();

                var list = baiduService.GetBaiDuProvinceID(new BaiduApiModel { Latitude = data.MobileLat.ToString(), Longitude = data.MobileLng.ToString() });
                string AddressM = baiduService.GetAddress(data.MobileLng.ToString(), data.MobileLat.ToString());
                var firM = queryM.FirstOrDefault();
                firM.ProvinceID = list[0].ProvinceID;
                firM.CityID = list[0].cityID;
                firM.DistrictID = list[0].districtID;
                firM.Address = AddressM;
                firM.Latitude = data.LockLat;
                firM.Longitude = data.LockLng;
                firM.UpdateTime = DateTime.Now;
                db.SubmitChanges();
                return new ResultModel { ResObject = true };

                #region
                //        var stq = CoordinateHelper.QDistance(new Coordinates(data.LockLng ?? decimal.MinValue, data.LockLat ?? decimal.MinValue), new Coordinates(data.MobileLng ?? decimal.MinValue, data.MobileLat ?? decimal.MinValue));
                //        if (stq > 1000)
                //        {
                //            var sQuery = from x in db.SIMCard
                //                         where x.LockNumber == data.LockNumber && x.SIMNumber == data.SIMNumber
                //                         select x;
                //            if (sQuery.Any())
                //            {
                //                var fir = sQuery.FirstOrDefault();

                //                fir.Status = 1;
                //                fir.UpdateTime = date;
                //                fir.UpdateBy = data.UserInfoGuid;
                //                fir.ActivationTime = date;
                //            }
                //            else
                //            {
                //                var ins = new MintCyclingData.SIMCard();
                //                ins.ActivationTime = date;
                //                ins.CreateBy = data.UserInfoGuid;
                //                ins.CreateTime = date;
                //                ins.Guid = Guid.NewGuid();
                //                ins.IsDeleted = false;
                //                ins.LockNumber = data.LockNumber;
                //                ins.SIMNumber = data.SIMNumber;
                //                ins.Status = 1;
                //                db.SIMCard.InsertOnSubmit(ins);
                //            }

                //            db.SubmitChanges();

                //            return new ResultModel { IsSuccess = false, Message = "锁的定位跟手机定位差异过大" };

                //        }
                //        else
                //        {
                //            var sQuery = from x in db.SIMCard
                //                         where x.LockNumber == data.LockNumber && x.SIMNumber == data.SIMNumber
                //                         select x;
                //            if (sQuery.Any())
                //            {
                //                var fir = sQuery.FirstOrDefault();

                //                fir.Status = 0;
                //                fir.UpdateTime = date;
                //                fir.UpdateBy = data.UserInfoGuid;
                //                fir.ActivationTime = date;
                //            }
                //            else
                //            {
                //                var ins = new MintCyclingData.SIMCard();
                //                ins.ActivationTime = date;
                //                ins.CreateBy = data.UserInfoGuid;
                //                ins.CreateTime = date;
                //                ins.Guid = Guid.NewGuid();
                //                ins.IsDeleted = false;
                //                ins.LockNumber = data.LockNumber;
                //                ins.SIMNumber = data.SIMNumber;
                //                ins.Status = 0;
                //                db.SIMCard.InsertOnSubmit(ins);
                //            }

                //        }

                //        var result = new ResultModel();
                //        var query = from x in db.BicycleLockInfo
                //                    where x.LockNumber == data.LockNumber && !x.IsDeleted
                //                    select x;
                //        if (query.Any())
                //        {
                //            var fir = query.FirstOrDefault();

                //            fir.Address = data.Address;
                //            fir.Latitude = data.LockLat;
                //            fir.Longitude = data.LockLng;
                //            fir.UpdateBy = data.UserInfoGuid;
                //            fir.UpdateTime = date;

                //            CyclingService cyc = new CyclingService();

                //            var list = cyc.GetBaiDuProvinceID(new BaiduApiModel { Latitude = data.LockLat.ToString(), Longitude = data.LockLng.ToString() });

                //            fir.ProvinceID = list[0].ProvinceID;
                //            fir.CityID = list[0].cityID;
                //            fir.DistrictID = list[0].districtID;

                //            db.SubmitChanges();

                //            return new ResultModel { ResObject = true };
                //        }

                //        return new ResultModel { IsSuccess = false, Message = "此单车不存在" };
                //    }
                //    catch (Exception ex)
                //    {
                //        Utility.Common.FileLog.Log(ex.Message, "BreakdownService");
                //        return new Utils.ResultModel { IsSuccess = false, Message = ex.Message };
                //    }
                //}
                #endregion Candy
            }
        }

        /// <summary>
        /// 维修单车编辑
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ResultModel UpdateBreakDown(UpdateBreakDown_PM data)

        {
            DateTime dtNow = DateTime.Now;
            string provinceName = string.Empty;
            string cityName = string.Empty;
            string districtName = string.Empty;
            string Address = string.Empty;
            string BreakTypeName = string.Empty;

            int ProvinceID = 0;
            int cityID = 0;
            int districtID = 0;

            var result = new ResultModel();

            using (var db = new MintBicycleDataContext())
            {
                //车辆基本信息
                var query = from x in db.BicycleBaseInfo
                            join L in db.BicycleLockInfo on x.LockNumber equals L.LockNumber
                            where x.BicycleNumber == data.BikeNumber && !x.IsDeleted && !L.IsDeleted
                            select L;
                if (!query.Any())
                {
                    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.BicycleNotExist, Message = ResPrompt.BicycleNotExistMessage };
                }
                var fir = query.FirstOrDefault();

                #region 调用百度API--更新车辆基本信息表

                try
                {
                    //详细地址
                    Address = baiduService.GetAddress(data.Longitude.ToString(), data.Latitude.ToString());
                    List<AddressModel> list = baiduService.GetAddressInfo(data.Longitude.ToString(), data.Latitude.ToString());
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
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error("维修单车编辑：调用百度API--更新车辆基本信息异常：" + ex.Message);
                }

                #endregion 调用百度API--更新车辆基本信息表

                try  //故障类型：0无法开关锁、1无法骑行、2无法结算、3二维码受损、4其它问题
                {
                    //车辆基本信息

                    if (data.ServiceStatus == ServiceStatus.Normal)
                    {
                        var breakquery = from x in db.BreakdownLog
                                         where x.BicyCleNumber == data.BikeNumber && !x.IsDeleted && x.Status != (int)ServiceStatus.Normal
                                         select x;
                        if (breakquery.Any())
                        {
                            var list = breakquery.ToList();
                            foreach (var item in list)
                            {
                                item.Status = (int)ServiceStatus.Normal;
                                item.ServicePersonID = data.UserInfoGuid;
                                item.UpdateTime = dtNow;
                                item.UpdateBy = data.UserInfoGuid;
                                item.ServiceTypeName = ServiceStatus.Normal.ToString();
                            }
                            db.SubmitChanges();
                        }

                        var rep = new MintCyclingData.RepairRecord();

                        rep.BicyCleNumber = data.BikeNumber;
                        rep.Address = data.Adress;
                        rep.BreakTypeName = string.Join(",", data.BreakDownType);
                        rep.CreateTime = dtNow;
                        rep.GradeTypeName = (int)data.GreaGradeType;
                        rep.Guid = Guid.NewGuid();
                        rep.IsDeleted = false;
                        rep.Remark = data.Remark;
                        rep.ServicePersonID = data.UserInfoGuid;
                        rep.ServicePersonPhotoGuid = data.ICBCPhotoGuid;
                        rep.BreakDownPhotoGuid = data.BreakPhotoGuid;
                        rep.ServiceTypeName = (int)data.ServiceStatus;

                        fir.ProvinceID = ProvinceID;
                        fir.CityID = cityID;
                        fir.DistrictID = districtID;
                        fir.Longitude = data.Longitude != null ? data.Longitude : fir.Longitude;
                        fir.Latitude = data.Latitude != null ? data.Latitude : fir.Latitude;
                        fir.Address = Address;
                        fir.LockStatus = 1;
                        fir.UpdateTime = dtNow;
                        db.RepairRecord.InsertOnSubmit(rep);
                        db.SubmitChanges();
                        return new ResultModel { ResObject = true };
                    }
                    else
                    {
                        var rep = new MintCyclingData.RepairRecord();

                        rep.BicyCleNumber = data.BikeNumber;
                        rep.Address = data.Adress;
                        rep.BreakTypeName = string.Join(",", data.BreakDownType);
                        rep.CreateTime = dtNow;
                        rep.GradeTypeName = (int)data.GreaGradeType;
                        rep.Guid = Guid.NewGuid();
                        rep.IsDeleted = false;
                        rep.Remark = data.Remark;
                        rep.ServicePersonID = data.UserInfoGuid;
                        rep.ServicePersonPhotoGuid = data.ICBCPhotoGuid;
                        rep.BreakDownPhotoGuid = data.BreakPhotoGuid;
                        rep.ServiceTypeName = (int)data.ServiceStatus;

                        fir.ProvinceID = ProvinceID;
                        fir.CityID = cityID;
                        fir.DistrictID = districtID;
                        fir.Longitude = data.Longitude != null ? data.Longitude : fir.Longitude;
                        fir.Latitude = data.Latitude != null ? data.Latitude : fir.Latitude;
                        fir.Address = Address;
                        fir.LockStatus = 2;
                        fir.UpdateTime = dtNow;
                        db.RepairRecord.InsertOnSubmit(rep);
                        db.SubmitChanges();
                        return new ResultModel { ResObject = true };
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error("维修单车编辑异常：" + ex.Message);
                    return new Utils.ResultModel { IsSuccess = false, Message = ex.Message };
                }
            }
        }

        /// <summary>
        /// 维修记录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ResultModel RepairRecord(RepairRecord_PM data)
        {
            using (var db = new MintBicycleDataContext())
            {
                try
                {
                    var query = from x in db.RepairRecord
                                join u in db.ServicePersonInfo on x.ServicePersonID equals u.ServicePersonID
                                where !x.IsDeleted && x.BicyCleNumber == data.BikeNumber
                                select new RepairRecord_OM
                                {
                                    BikeNumber = x.BicyCleNumber,
                                    BreakDownType = x.BreakTypeName,
                                    RepirtTime = x.CreateTime,
                                    ServiceStatus = ServiceState.GetServiceState(x.ServiceTypeName),
                                    RepirtName = u.RealName
                                };

                    var list = new List<RepairRecord_OM>();

                    if (query.Any())
                    {
                        query = query.OrderByDescending(x => x.RepirtTime);

                        if (data.LastRepair)
                        {
                            list.Add(query.FirstOrDefault());
                            return new ResultModel { ResObject = new { Count = query.Count(), List = list } };
                        }

                        list = new PagedList<RepairRecord_OM>(query, data.PageIndex, data.PageSize);
                        return new ResultModel { ResObject = new { Count = query.Count(), List = list } };
                    }
                    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.NoRepairRecordCode, Message = ResPrompt.NoRepairRecordMessage };
                }
                catch (Exception ex)
                {
                    LogHelper.Error("维修记录查询信息异常：" + ex.Message);
                    return new ResultModel { IsSuccess = false, Message = ex.Message };
                }
            }
        }

        /// <summary>
        /// 维修记录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ResultModel AdminRepairRecord(AdminRepairRecord_PM data)
        {
            using (var db = new MintBicycleDataContext())
            {
                try
                {
                    var query = from x in db.RepairRecord
                                join u in db.ServicePersonInfo on x.ServicePersonID equals u.ServicePersonID
                                where !x.IsDeleted && (string.IsNullOrEmpty(data.BikeNumber) || x.BicyCleNumber.Contains(data.BikeNumber)) && (data.UserName == u.RealName || string.IsNullOrEmpty(data.UserName))
                                select new RepairRecord
                                {
                                    BikeNumber = x.BicyCleNumber,
                                    BreakDownType = x.BreakTypeName,
                                    RepirtTime = x.CreateTime,
                                    ServiceStatus = ServiceState.GetServiceState(x.ServiceTypeName),
                                    RepirtName = u.RealName,
                                    ICBCPhoto = x.ServicePersonPhotoGuid,
                                    BreakDownPhoto = x.BreakDownPhotoGuid
                                };

                    if (data.RepirtStartTime.HasValue)
                    {
                        query = query.Where(x => x.RepirtTime >= data.RepirtStartTime.Value.Date);
                    }
                    if (data.RepirtEndTime.HasValue)
                    {
                        query = query.Where(x => x.RepirtTime.Date <= data.RepirtEndTime.Value.Date);
                    }
                    var list = new List<AdminRepairRecord_OM>();
                    var tk = ConfigurationManager.AppSettings["BreakdownUploadLoc"] ?? "";

                    var GetICBCPhotoFunc = new Func<Guid, string>(c =>
                     {
                         var Iquery = from x in db.ServicePersonPhoto
                                      where x.PhotoGuid == c
                                      select x.Url;
                         var Qurl = Iquery.FirstOrDefault() == null ? "" : tk + Iquery.FirstOrDefault();

                         return Qurl;
                     });

                    var getBreakphotoFunc = new Func<Guid?, List<string>>(c =>
                   {
                       var Blist = new List<string>();
                       if (c == null) return Blist;

                       var bquery = from x in db.BreakdownPhotoInfo
                                    where x.BreakDownPhotoGuid == c
                                    select x;

                       foreach (var item in bquery)
                       {
                           Blist.Add(tk + item.Url);
                       }
                       return Blist;
                   });

                    if (query.Any())
                    {
                        query = query.OrderByDescending(x => x.RepirtTime);

                        var pagelist = new PagedList<RepairRecord>(query, data.PageIndex, data.PageSize);
                        var reslist = new List<AdminRepairRecord_OM>();
                        foreach (var item in pagelist)
                        {
                            var sig = new AdminRepairRecord_OM();
                            sig.BikeNumber = item.BikeNumber;
                            var breakType = new List<string>();
                            foreach (var C in item.BreakDownType.Split(','))
                            {
                                breakType.Add(BreakDownState.GetBreakDownState(Convert.ToInt32(C)));
                            }
                            sig.BreakDownType = string.Join(",", breakType);
                            sig.ICBCPhoto = GetICBCPhotoFunc(item.ICBCPhoto);
                            sig.RepirtName = item.RepirtName;
                            sig.RepirtTime = item.RepirtTime;
                            sig.ServiceStatus = item.ServiceStatus;
                            sig.BreakDownPhotoList = getBreakphotoFunc(item.BreakDownPhoto);
                            reslist.Add(sig);
                        }
                        return new ResultModel { ResObject = new { Total = query.Count(), List = reslist } };
                    }
                    return new ResultModel { MsgCode = ResPrompt.NoRepairRecordCode, Message = ResPrompt.NoRepairRecordMessage };
                }
                catch (Exception ex)
                {
                    LogHelper.Error("维修记录信息异常：" + ex.Message);
                    return new ResultModel { IsSuccess = false, Message = ex.Message };
                }
            }
        }

        /// <summary>
        /// 故障上报
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public ResultModel BreakDownReport(UpdateBreakDown_PM data)
        {
            DateTime dtNow = DateTime.Now;
            string provinceName = string.Empty;
            string cityName = string.Empty;
            string districtName = string.Empty;
            string Address = string.Empty;
            string BreakTypeName = string.Empty;

            int ProvinceID = 0;
            int cityID = 0;
            int districtID = 0;

            var result = new ResultModel();

            using (var db = new MintBicycleDataContext())
            {
                //车辆基本信息
                var query = from x in db.BicycleBaseInfo
                            join L in db.BicycleLockInfo on x.LockNumber equals L.LockNumber
                            where x.BicycleNumber == data.BikeNumber && !x.IsDeleted && !L.IsDeleted
                            select L;
                if (!query.Any())
                {
                    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.BicycleNotExist, Message = ResPrompt.BicycleNotExistMessage };
                }
                var fir = query.FirstOrDefault();

                #region 调用百度API--更新车辆基本信息表

                try
                {
                    //详细地址
                    Address = baiduService.GetAddress(data.Longitude.ToString(), data.Latitude.ToString());
                    List<AddressModel> list = baiduService.GetAddressInfo(data.Longitude.ToString(), data.Latitude.ToString());
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
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error("调用百度API--更新车辆基本信息异常：" + ex.Message);
                }

                #endregion 调用百度API--更新车辆基本信息表

                try  //故障类型：0无法开关锁、1无法骑行、2无法结算、3二维码受损、4其它问题
                {
                    foreach (var item in data.BreakDownType)
                    {
                        var breakdown = new BreakdownLog
                        {
                            BreakdownGuid = Guid.NewGuid(),
                            BicyCleNumber = data.BikeNumber,
                            //0无法开关锁、1无法骑行、2无法结算、3二维码受损、4其它问题
                            BreakTypeName = item,
                            BreakDownPhotoGuid = data.BreakPhotoGuid,
                            Longitude = data.Longitude,
                            Latitude = data.Latitude,
                            Status = 0,                            //默认故障
                            ReportSource = 0,                      //0：绿帝出行app故障上报，1维护app上报
                            ProvinceID = ProvinceID,
                            CityID = cityID,
                            DistrictID = districtID,
                            Address = Address,
                            Remark = data.Remark,
                            CreateBy = data.UserInfoGuid,        //上报人Guid
                            CreateTime = dtNow,
                            IsDeleted = false
                        };
                        db.BreakdownLog.InsertOnSubmit(breakdown);
                    }

                    db.SubmitChanges();
                    if (data.ServiceStatus == ServiceStatus.Normal)
                    {
                        var breakquery = from x in db.BreakdownLog
                                         where x.BicyCleNumber == data.BikeNumber && !x.IsDeleted && x.Status != (int)ServiceStatus.Normal
                                         select x;
                        if (breakquery.Any())
                        {
                            var list = breakquery.ToList();
                            foreach (var item in list)
                            {
                                item.Status = (int)ServiceStatus.Normal;
                                item.ServicePersonID = data.UserInfoGuid;
                                item.UpdateTime = dtNow;
                                item.UpdateBy = data.UserInfoGuid;
                                item.ServiceTypeName = ServiceStatus.Normal.ToString();
                            }
                            db.SubmitChanges();
                        }

                        var rep = new MintCyclingData.RepairRecord();

                        rep.BicyCleNumber = data.BikeNumber;
                        rep.Address = data.Adress;
                        rep.BreakTypeName = string.Join(",", data.BreakDownType);
                        rep.CreateTime = dtNow;
                        rep.GradeTypeName = (int)data.GreaGradeType;
                        rep.Guid = Guid.NewGuid();
                        rep.IsDeleted = false;
                        rep.Remark = data.Remark;
                        rep.ServicePersonID = data.UserInfoGuid;
                        rep.ServicePersonPhotoGuid = data.ICBCPhotoGuid;
                        rep.BreakDownPhotoGuid = data.BreakPhotoGuid;
                        rep.ServiceTypeName = (int)data.ServiceStatus;

                        fir.ProvinceID = ProvinceID;
                        fir.CityID = cityID;
                        fir.DistrictID = districtID;
                        fir.Longitude = data.Longitude != null ? data.Longitude : fir.Longitude;
                        fir.Latitude = data.Latitude != null ? data.Latitude : fir.Latitude;
                        fir.Address = Address;
                        fir.LockStatus = 1;
                        fir.UpdateTime = dtNow;
                        db.SubmitChanges();
                        return new ResultModel { ResObject = true };
                    }
                    else
                    {
                        var rep = new MintCyclingData.RepairRecord();

                        rep.BicyCleNumber = data.BikeNumber;
                        rep.Address = data.Adress;
                        rep.BreakTypeName = string.Join(",", data.BreakDownType);
                        rep.CreateTime = dtNow;
                        rep.GradeTypeName = (int)data.GreaGradeType;
                        rep.Guid = Guid.NewGuid();
                        rep.IsDeleted = false;
                        rep.Remark = data.Remark;
                        rep.ServicePersonID = data.UserInfoGuid;
                        rep.ServicePersonPhotoGuid = data.ICBCPhotoGuid;
                        rep.ServiceTypeName = (int)data.ServiceStatus;
                        rep.BreakDownPhotoGuid = data.BreakPhotoGuid;

                        fir.ProvinceID = ProvinceID;
                        fir.CityID = cityID;
                        fir.DistrictID = districtID;
                        fir.Longitude = data.Longitude != null ? data.Longitude : fir.Longitude;
                        fir.Latitude = data.Latitude != null ? data.Latitude : fir.Latitude;
                        fir.Address = Address;
                        fir.LockStatus = 2;
                        fir.UpdateTime = dtNow;
                        db.SubmitChanges();
                        return new ResultModel { ResObject = true };
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Error("故障上报信息异常：" + ex.Message);

                    return new Utils.ResultModel { IsSuccess = false, Message = ex.Message };
                }
            }
        }

        /// <summary>
        ///我的维修
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>

        public ResultModel RepairForm(RepirtForm_PM data)
        {
            using (var db = new MintBicycleDataContext())
            {
                try
                {
                    var date = DateTime.Now;

                    var query = from x in db.RepairRecord
                                where !x.IsDeleted && x.ServicePersonID == data.UserInfoGuid
                                select x;
                    if (data.StartRepirtTime.HasValue)
                    {
                        query = query.Where(x => data.StartRepirtTime.Value.Date <= x.CreateTime);
                    }
                    if (data.EndRepirTime.HasValue)
                    {
                        query = query.Where(x => x.CreateTime <= data.EndRepirTime.Value.AddDays(1).Date);
                    }

                    if (query.Any())
                    {
                        var list = query.OrderByDescending(x => x.CreateTime).ToList();

                        var resList = new List<RepirtForm_OM>();

                        foreach (var item in list)
                        {
                            var fir = new RepirtForm_OM();
                            fir.Address = item.Address;
                            fir.BikeNumber = item.BicyCleNumber;
                            fir.BreakDownType = item.BreakTypeName;
                            fir.ServerStatus = ServiceState.GetServiceState(item.ServiceTypeName);
                            fir.RepirtTime = item.CreateTime;
                            resList.Add(fir);
                        }

                        return new ResultModel { ResObject = new PagedList<RepirtForm_OM>(resList, data.PageIndex, data.PageSize) };
                    }

                    return new ResultModel { IsSuccess = false, Message = ResPrompt.NoRepairRecordMessage, MsgCode = ResPrompt.NoRepairRecordCode };
                }
                catch (Exception ex)
                {
                    Utility.Common.FileLog.Log(ex.Message, "BreakdownService");
                    return new Utils.ResultModel { IsSuccess = false, Message = ex.Message };
                }
            }
        }

        /// <summary>
        /// 请求一分钟之内的通知
        /// </summary>
        /// <returns></returns>

        public ResultModel Getnotification()
        {
            using (var db = new MintBicycleDataContext())
            {
                var date = DateTime.Now;
                var query = from x in db.BreakdownLog
                            where date.AddMinutes(-1) <= x.CreateTime && x.CreateTime <= date
                            select x;
                if (query.Any())
                {
                    return new ResultModel { ResObject = new { Total = query.Count(), List = query.Select(x => x.BreakdownGuid).ToList() } };
                }
                return new ResultModel { ResObject = new { Total = 0 } };
            }
        }

        /// <summary>
        /// 根据经纬度获取地址
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <returns></returns>

        public ResultModel GetAddressByLJ(LatLng data)
        {
            var addres = baiduService.GetAddress(data.Lng, data.Lat);

            var res = new ResultModel { };
            if (addres != null)
            {
                res.ResObject = addres;
                return res;
            }
            return res;
        }
    }
}

#endregion