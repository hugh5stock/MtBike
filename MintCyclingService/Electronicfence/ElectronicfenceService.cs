using HuRongClub.DBUtility;
using MintCyclingData;
using MintCyclingService.BaiDu;
using MintCyclingService.Common;
using MintCyclingService.Cycling;
using MintCyclingService.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Utility;
using static MintCyclingService.Cycling.BaiduModel;

namespace MintCyclingService.Electronicfence
{
    public class ElectronicfenceService : IElectronicfenceService
    {
        ICyclingService _cycService = new CyclingService();
        IBaiduService _baiduService = new BaiduService();


        /// <summary>
        /// 查询后台省市区列表
        /// </summary>
        /// <returns></returns>
        public ResultModel GetProvinceCityData()
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var query = db.Province.OrderBy(s => s.Id).Select(s => new { ProvinceId = s.Id, ProvinceName = s.Name });
                if (query.Any())
                {
                    var provinceData = query.ToList();

                    var qCity = (from t in db.City
                                 join y in db.Province on t.ProvinceId equals y.Id
                                 orderby t.OrderWord
                                 select new
                                 {
                                     CityId = t.Id,
                                     CityName = t.Name,
                                     ProvinceId = y.Id
                                 });
                    var qDistrict = (from p in db.District
                                     join q in db.City on p.CityId equals q.Id
                                     join n in db.Province on q.ProvinceId equals n.Id
                                     orderby p.PostCode
                                     select new
                                     {
                                         DistrictId = p.Id,
                                         DistrictName = p.Name,
                                         CityId = q.Id
                                     });
                    result.ResObject = new { ProvinceData = provinceData, CityData = qCity.Any() ? qCity.ToList() : null, District = qDistrict.Any() ? qDistrict.ToList() : null };
                }
            }
            return result;
        }

        /// <summary>
        /// 根据查询条件搜索后台电子围栏列表
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public ResultModel GetElectronicfenceListByCondition(AdminEnclosureList_PM para)
        {
            var result = new ResultModel();
            //获取总账号省市区
            var region = Config.UserRegion;
            if (region == null)
                return result;

            using (var db = new MintBicycleDataContext())
            {
                var query = (from s in db.Electronic_FenCing
                             join province in db.Province on s.ProvinceID equals province.Id
                             join city in db.City on s.CityID equals city.Id
                             join t in db.District on s.DistrictID equals t.Id
                             where !s.IsDeleted && (string.IsNullOrEmpty(para.EnclosureSeq) || s.ElectronicFenCingNumber == para.EnclosureSeq)
                             && (para.EnclosureProvince == 0 || para.EnclosureProvince == s.ProvinceID)
                              && (para.EnclosureCity == 0 || para.EnclosureCity == s.CityID)
                               && (para.EnclosureDistrict == 0 || para.EnclosureDistrict == s.DistrictID)
                               && (region.ExceptUserRegion || (province.Id == region.UserProvince && (region.UserCity == null || city.Id == region.UserCity)
                                   && (region.UserDistrict == null || t.Id == region.UserDistrict)))
                             orderby s.CreateTime descending
                             select new AdminEnclosureList_OM
                             {
                                 EnclosureGuid = s.ElectronicFenCingGuid,
                                 EnclosureNumber = s.ElectronicFenCingNumber,
                                 EnclosureDistrict = string.Format("{0}{1}{2}", province.Name, city.Name, t.Name),
                                 EnclosureState = s.Status == 0 ? "报警" : "正常"
                             });
                if (query.Any())
                {
                    var tsk = new PagedList<AdminEnclosureList_OM>(query, para.PageIndex, para.PageSize, query.Count());
                    result.ResObject = new { Total = query.Count(), List = tsk };
                }
            }
            return result;
        }

        /// <summary>
        /// 查询电子围栏详情
        /// </summary>
        /// <param name="electronicfenceGuid"></param>
        /// <returns></returns>
        public ResultModel GetElectronicfenceDetail(Guid electronicfenceGuid)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var query = db.Electronic_FenCing.FirstOrDefault(s => s.ElectronicFenCingGuid == electronicfenceGuid);
                if (query == null)
                    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ElectronicfenceNotExist, Message = ResPrompt.ElectronicfenceNotExistMessage };
                var data = new AdminEnclosureDetail_OM
                {
                    EnclosureGuid = query.ElectronicFenCingGuid,
                    EnclosureSeq = query.ElectronicFenCingNumber,
                    EnclosureLongitude = query.Longitude,
                    EnclosureLatitude = query.Latitude,
                    EnclosureProvinceId = query.ProvinceID ?? 0,
                    EnclosureCityId = query.CityID ?? 0,
                    EnclosureDistrictId = query.DistrictID ?? 0,
                    DetailAddress = query.Address,
                    EnclosureState = query.Status,
                    EnclosureRemark = query.Remark
                };
                result.ResObject = data;
            }
            return result;
        }

        /// <summary>
        /// 新增或修改指定的电子围栏数据
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public ResultModel AddAndUpdateElectronicfence(AdminEnclosureAddAndUpdate_PM para)
        {
            var result = new ResultModel();
            BaiduApiModel model = new BaiduApiModel();
            model.Longitude = para.EnclosureLongitude.ToString();
            model.Latitude = para.EnclosureLatitude.ToString();
            List<AddressIDList> list = _baiduService.GetBaiDuProvinceID(model);
            if (list.Count == 0)
            {
                result.Message = "解析经纬度失败,请重试！";
                result.ResObject = false;
                return result;
            }

            using (var db = new MintBicycleDataContext())
            {
                if (para.EnclosureGuid == null)
                {
                    if (db.Electronic_FenCing.Any(s => s.ElectronicFenCingNumber == para.EnclosureSeq))
                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ElectronicfenceSeqExist, Message = ResPrompt.ElectronicfenceSeqExistMessage };
                    //已存在相同经纬度的电子围栏
                    if (db.Electronic_FenCing.Any(s => s.Longitude == para.EnclosureLongitude && s.Latitude == para.EnclosureLatitude))
                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ElectronicfenceCoordsExist, Message = ResPrompt.ElectronicfenceCoordsExistMessage };

                    if (db.Electronic_FenCing.Any(s => s.Address == para.DetailAddress))
                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ElectronicfenceAddressExist, Message = ResPrompt.ElectronicfenceAddressExistMessage };
                    var enclosure = new Electronic_FenCing
                    {
                        ElectronicFenCingGuid = Guid.NewGuid(),
                        ElectronicFenCingNumber = para.EnclosureSeq,
                        Longitude = para.EnclosureLongitude,
                        Latitude = para.EnclosureLatitude,
                        Status = para.EnclosureState,
                        //省ID
                        //ProvinceID = para.EnclosureProvinceId,
                        //CityID = para.EnclosureCityId,
                        //DistrictID = para.EnclosureDistrictId,
                        ProvinceID = list[0].ProvinceID,
                        CityID = list[0].cityID,
                        DistrictID = list[0].districtID,
                        Address = para.DetailAddress,
                        Remark = para.EnclosureRemark,
                        CreateBy = para.OperatorGuid,
                        UpdateTime = DateTime.Now,
                        CreateTime = DateTime.Now,
                        IsDeleted = false
                    };
                    db.Electronic_FenCing.InsertOnSubmit(enclosure);
                    db.SubmitChanges();
                }
                else
                {
                    var enclosure = db.Electronic_FenCing.FirstOrDefault(p => p.ElectronicFenCingGuid == para.EnclosureGuid && !p.IsDeleted);
                    if (enclosure == null)
                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ElectronicfenceNotExist, Message = ResPrompt.ElectronicfenceNotExistMessage };
                    if (db.Electronic_FenCing.Any(s => s.ElectronicFenCingNumber == para.EnclosureSeq && s.ElectronicFenCingGuid != para.EnclosureGuid))
                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ElectronicfenceSeqExist, Message = ResPrompt.ElectronicfenceSeqExistMessage };
                    //if (db.Electronic_FenCing.Any(s => s.ElectronicFenCingGuid != para.EnclosureGuid && s.Longitude == para.EnclosureLongitude && s.Latitude == para.EnclosureLatitude))
                    //    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ElectronicfenceCoordsExist, Message = ResPrompt.ElectronicfenceCoordsExistMessage };
                    if (db.Electronic_FenCing.Any(s => s.ElectronicFenCingGuid != para.EnclosureGuid && s.Address == para.DetailAddress))
                        return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ElectronicfenceAddressExist, Message = ResPrompt.ElectronicfenceAddressExistMessage };
                    enclosure.ElectronicFenCingNumber = para.EnclosureSeq;
                    enclosure.Longitude = para.EnclosureLongitude;
                    enclosure.Latitude = para.EnclosureLatitude;
                    enclosure.Status = para.EnclosureState;

                    //enclosure.ProvinceID = para.EnclosureProvinceId;
                    //enclosure.CityID = para.EnclosureCityId;
                    //enclosure.DistrictID = para.EnclosureDistrictId;
                    enclosure.ProvinceID = list[0].ProvinceID;
                    enclosure.CityID = list[0].cityID;
                    enclosure.DistrictID = list[0].districtID;
                    enclosure.Address = para.DetailAddress;

                    enclosure.Remark = para.EnclosureRemark;
                    enclosure.UpdateBy = para.OperatorGuid;
                    enclosure.UpdateTime = DateTime.Now;
                    db.SubmitChanges();
                }
                result.ResObject = true;
            }
            return result;
        }

        /// <summary>
        /// 删除指定的电子围栏
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public ResultModel RemoveElectronicfence(AdminEnclosureDelete_PM para)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var query = db.Electronic_FenCing.FirstOrDefault(s => s.ElectronicFenCingGuid == para.EnclosureGuid && !s.IsDeleted);
                if (query == null)
                    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ElectronicfenceNotExist, Message = ResPrompt.ElectronicfenceNotExistMessage };
                query.IsDeleted = true;
                query.UpdateBy = para.OperatorGuid;
                query.UpdateTime = DateTime.Now;
                db.SubmitChanges();
                result.ResObject = true;
            }
            return result;
        }

        /// <summary>
        /// 查询电子围栏下的单车列表
        /// </summary>
        /// <param name="electronicfenceGuid"></param>
        /// <returns></returns>
        public ResultModel GetElectronicfenceBicycle(Guid electronicfenceGuid)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var query = (from b in db.BicycleBaseInfo
                             join s in db.BicycleLockInfo on b.LockGuid equals s.LockGuid
                             //join t in db.BicycleTypeInfo on s.BicyCleTypeGuid equals t.BicycleTypeGuid
                             where !s.IsDeleted && s.ElectronicFenCingGuid == electronicfenceGuid
                             select new AdminEnclosureBicycleModel
                             {
                                 BicycleNumber = b.BicycleNumber,
                                 BicyCleType = BicycleBaseInfoShow.GetBicycleTypeName(b.BicyCleTypeName),
                                 BicyCleState = EnumRemarksHandler.GetBicycleState(s.LockStatus ?? 0),
                                 Voltage = s.Voltage.ToString()
                             });
                if (query.Any())
                {
                    var cnt = query.Count();
                    result.ResObject = new AdminEnclosureBicycle_OM { BicyCleCount = cnt, BicyCleList = query.ToList() };
                }
            }
            return result;
        }


        /// <summary>
        /// 查询电子围栏下的单车列表
        /// </summary>
        /// <param name="ElectronicFenCingNumber"></param>
        /// <returns></returns>
        public ResultModel GetElectronicfenceBicycleInfo(EnclosureList_PM model)
        {
            var result = new ResultModel();
            Guid electronicGuid = new Guid();

            using (var db = new MintBicycleDataContext())
            {
                var eleQuery = db.Electronic_FenCing.FirstOrDefault(s => s.ElectronicFenCingNumber == model.ElectronicFenCingNumber);
                if (eleQuery != null)
                {
                    electronicGuid = eleQuery.ElectronicFenCingGuid;
                }
                else
                {
                    return new ResultModel { IsSuccess = false, MsgCode = "0", Message = "没有要查询的电子围栏信息!" };
                }
                var query = (from b in db.BicycleBaseInfo
                                  join s in db.BicycleLockInfo on b.LockGuid  equals s.LockGuid
                                 //join t in db.BicycleTypeInfo on s.BicyCleTypeGuid equals t.BicycleTypeGuid
                             where !s.IsDeleted && s.ElectronicFenCingGuid == electronicGuid
                             select new AdminEnclosureBicycleModel
                             {
                                 BicycleNumber = b.BicycleNumber,
                                 BicyCleType = BicycleBaseInfoShow.GetBicycleTypeName(b.BicyCleTypeName),
                                 BicyCleState = EnumRemarksHandler.GetBicycleState(s.LockStatus ?? 0),
                                 Voltage = s.Voltage.ToString()
                             });
                if (query.Any())
                {
                    var cnt = query.Count();
                    result.ResObject = new AdminEnclosureBicycle_OM { BicyCleCount = cnt, BicyCleList = query.ToList() };
                }
            }
            return result;
        }



        /// <summary>
        /// 根据输入的关键字查询电子围栏的列表信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel GetElectronicfenceList(GetElectrlnic_PM model)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {

                var query = (from s in db.Electronic_FenCing
                             where !s.IsDeleted && (string.IsNullOrEmpty(model.KeyWordName) || s.Address.Contains(model.KeyWordName))
                             select new GetElectrlnic_OM
                             {
                                 ElectronicFenCingGuid = s.ElectronicFenCingGuid,
                                 Longitude = s.Longitude,
                                 Latitude = s.Latitude,
                                 Status = s.Status == 0 ? "报警" : "正常",
                                 Address = s.Address
                             });

                if (query.Any())
                {
                    var list = new List<GetElectrlnic_OM>();
                    foreach (var q in query)
                    {
                        list.Add(new GetElectrlnic_OM
                        {
                            ElectronicFenCingGuid = q.ElectronicFenCingGuid,
                            Longitude = q.Longitude,
                            Latitude = q.Latitude,
                            Status = q.Status,
                            Address = q.Address
                        });
                    }
                    var cnt = list.Count;
                    var tsk = new PagedList<GetElectrlnic_OM>(list, model.PageIndex, model.PageSize);
                    result.ResObject = new { Total = cnt, List = tsk };
                }
            }
            return result;
        }

        /// <summary>
        /// 根据查询条件搜索后台地图上显示是我电子围栏中的车辆总数信息
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public ResultModel GetMapElectronicListByCondition(SearchElectronic_PM para)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {

                var query = (from s in db.Electronic_FenCing
                             join province in db.Province on s.ProvinceID equals province.Id
                             join city in db.City on s.CityID equals city.Id
                             join t in db.District on s.DistrictID equals t.Id
                             join b in db.BicycleLockInfo on s.ElectronicFenCingGuid equals b.ElectronicFenCingGuid
                             where !s.IsDeleted
                             && (para.EnclosureProvince == 0 || para.EnclosureProvince == s.ProvinceID)
                             && (para.EnclosureCity == 0 || para.EnclosureCity == s.CityID)
                             && (para.EnclosureDistrict == 0 || para.EnclosureDistrict == s.DistrictID)
                             orderby s.CreateTime descending
                             group s by new
                             {
                                 s.ElectronicFenCingGuid,
                                 t.Name

                             } into g
                             select new AdminEnclosureListNew_OM
                             {
                                 BicycleTotalCount = g.Count(),
                                 ElectronicFenCingGuid = g.Key.ElectronicFenCingGuid,
                                 Name = g.Key.Name

                             });

                if (query.Any())
                {
                    var list = query.ToList();
                    var cnt = list.Count;
                    result.ResObject = new { Total = cnt, List = list };
                }
            }

            return result;
        }

        /// <summary>
        /// 根据地理位置信息获取车辆以及电子围栏
        /// </summary>
        /// <param name="para">地理位置信息</param>
        /// <returns></returns>
        public ResultModel GetBicycleEnclosureListByGEOHash(MapBicycleEnclosure_PM para)
        {
            var result = new ResultModel();
            //var db = new MintBicycleDataContext();
            //var pos = para.lngLatOrEnclosureNumber.Split(',');
            //var geo = Utility.Common.GEOHashHelper.Encode(double.Parse(pos[1]), double.Parse(pos[0]));
            //var geo_06 = geo.Substring(0, 6);

            //var data = new NearBicycleEnclosureMap_OM();

            //var els = db.Electronic_FenCing.Where(q => !q.IsDeleted && q.GEOHash.StartsWith(geo_06)).Select(s => new MapEnclosureModel
            //{
            //    EnclosureGuid = s.ElectronicFenCingGuid,     //电子围栏编号
            //    EnclosureLongitude = s.Longitude,             //经度
            //    EnclosureLatitude = s.Latitude,               //纬度
            //    BicycleNum = 0
            //});
            //if (els.Any())
            //{
            //    data.EnclosureList = els.ToList();
            //    data.Totalcount = els.Count();
            //}

            //var bics = db.BicycleLockInfo.Where(q => !q.IsDeleted && q.LockStatus == 1 && q.GEOHash.StartsWith(geo_06)).Select(s => new MapBicycleModel
            //{

            //    DeviceNo = s.LockNumber,                   //设备编号
            //    BicycleLongitude = s.Longitude ?? 0,          //经度
            //    BicycleLatitude = s.Latitude ?? 0            //纬度

            //});
            //if (bics.Any())
            //{
            //    data.B_Totalcount = bics.Count();
            //    data.BicycleList = bics.ToList();
            //}
            //result.ResObject = data.BicycleList == null && data.EnclosureList == null ? null : data;


            return result;
        }

        /// <summary>
        /// 根据输入的地址或者电子围栏编号搜索一定范围内电子围栏信息中的车辆数和没有在电子围栏内的车辆信息
        /// DATE:2017-03-17
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public ResultModel GetBicycleEnclosureList(MapBicycleEnclosure_PM para)
        {
            int FZLBicycleNum = 0;
            int helpBicycleNum = 0;
            string FZLCleTypeName = string.Empty;
            string helpCleTypeName = string.Empty;

            var result = new ResultModel();
            decimal CurLongitude = 0;
            decimal CurLatitude = 0;
            int BicycleNums = 0;
            //经纬度变量
            string lngLat = para.lngLatOrEnclosureNumber;

            #region  判断输入的是地址还是电子围栏编号查询

            if (para.Type == "address")
            {
                string[] strarr = lngLat.Split(',');
                CurLongitude = decimal.Parse(strarr[0]);
                CurLatitude = decimal.Parse(strarr[1]);
            }
            else
            {
                using (var db = new MintBicycleDataContext())
                {
                    //根据电子围栏的编号查询电子围栏所在的经纬度信息
                    var query = db.Electronic_FenCing.FirstOrDefault(s => s.ElectronicFenCingNumber == para.lngLatOrEnclosureNumber);
                    if (query != null)
                    {
                        CurLongitude = query.Longitude;
                        CurLatitude = query.Latitude;
                    }
                }
            }
            #endregion


            //根据半径查找经纬度
            var radius = para.Radius == null || para.Radius < 1m ? 2000m : ((para.Radius ?? 1m) + 20m);
            using (var db = new MintBicycleDataContext())
            {
                var stk = CoordinateHelper.GetDegreeCoordinates(CurLatitude, CurLongitude, radius);
                var q0 = stk[0];
                var q1 = stk[1];
                var q2 = stk[2];
                var q3 = stk[3];
                var data = new AdminBicycleEnclosureMap_OM { };

                #region 电子围栏信息

                var enclosureQuery = (from s in db.Electronic_FenCing
                                      where !s.IsDeleted && ((s.Longitude < q0.Longitude && s.Latitude < q0.Latitude) &&
                                         (s.Longitude > q1.Longitude && s.Latitude < q1.Latitude) && (s.Longitude < q2.Longitude && s.Latitude > q2.Latitude) && (s.Longitude > q3.Longitude && s.Latitude > q3.Latitude))
                                      select s);
                if (enclosureQuery.Any())
                {
                    //围栏信息
                    var enclosureList = new List<MapEnclosureModel>();
                    foreach (var q in enclosureQuery)
                    {
                        #region 旧版本
                        //string strSql = "select  count(*) as bicCount from BicycleLockInfo as b where b.ElectronicFenCingGuid='" + q.ElectronicFenCingGuid + "' and b.LockStatus not in(0,2,3) and b.LockGuid not in(select BicycleBaseGuid from Reservation where    Status=1 )";
                        //DataSet ds = DbHelperSQL.Query(strSql);
                        //if (ds != null && ds.Tables[0].Rows.Count > 0)
                        //{
                        //    BicycleNums = int.Parse(ds.Tables[0].Rows[0]["bicCount"].ToString());
                        //}
                        //else
                        //{
                        //    BicycleNums = 0;
                        //}

                        #endregion
                        #region 查询车辆数分为非助力车和助力车两种情况

                        //查询非助力车的数量
                        //string strSql = "select  count(*) as bicCount from BicycleLockInfo as b  where b.ElectronicFenCingGuid='" + q.ElectronicFenCingGuid + "' and b.LockStatus not in(0,2,3) and b.LockGuid not in(select BicycleBaseGuid from Reservation where    Status=1 )";
                        StringBuilder strSql = new StringBuilder();
                        strSql.Append("select  count(*) as bicCount from BicycleLockInfo as b  inner join  BicycleBaseInfo as c on b.LockNumber =c.LockNumber ");
                        strSql.Append("where  b.ElectronicFenCingGuid='" + q.ElectronicFenCingGuid + "' ");
                        strSql.Append("and b.LockStatus =1 and b.LockGuid not in(select BicycleBaseGuid from Reservation where Status=1  or Status=3) ");
                        DataSet ds = DbHelperSQL.Query(strSql.ToString());
                        if (ds != null && ds.Tables[0].Rows.Count > 0)
                        {
                            FZLBicycleNum = int.Parse(ds.Tables[0].Rows[0]["bicCount"].ToString());
                            //FZLCleTypeName = "非助力车";
                        }

                        ////助力车的数量
                        //StringBuilder str = new StringBuilder();
                        //str.Append(" select  count(*) as helpBicycleNum from BicycleLockInfo as b inner join  BicycleBaseInfo as c on b.LockNumber =c.LockNumber ");
                        //str.Append(" where c.BicyCleTypeName=1 and  b.ElectronicFenCingGuid='" + q.ElectronicFenCingGuid + "' ");
                        //str.Append(" and b.LockStatus =1 and b.LockGuid not in(select BicycleBaseGuid from Reservation where Status=1 or Status=3) ");
                        //DataSet fds = DbHelperSQL.Query(str.ToString());
                        //if (fds != null && fds.Tables[0].Rows.Count > 0)
                        //{
                        //    helpBicycleNum = int.Parse(fds.Tables[0].Rows[0]["helpBicycleNum"].ToString());
                        //    helpCleTypeName = "助力车";
                        //}

                        #endregion 查询车辆数分为非助力车和助力车两种情况

                        //计算直线距离
                        var stq = CoordinateHelper.QDistance(new Coordinates(CurLongitude, CurLatitude), new Coordinates(q.Longitude, q.Latitude));
                        if (stq <= radius)
                        {
                            enclosureList.Add(new MapEnclosureModel
                            {
                                EnclosureGuid = q.ElectronicFenCingGuid,                //电子围栏Guid
                                ElectronicFenCingNumber = q.ElectronicFenCingNumber,   //电子围栏编号
                                EnclosureLongitude = q.Longitude,             //经度
                                EnclosureLatitude = q.Latitude,               //纬度
                                BicycleNum = FZLBicycleNum,                   //非助力车辆数量
                                //HelpBicycleNum = helpBicycleNum,              //助力车辆数量
                                //FZLCleTypeName = FZLCleTypeName,
                                //HelpCleTypeName = helpCleTypeName
                            });
                        }
                    }
                    data.EnclosureList = enclosureList;
                    data.Totalcount = enclosureList.Count;

                }


                #endregion

                #region 处理车辆数据

                ////去除预约过的车辆不显示在地图上
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

                #region 车辆数据
                var bicycleQuery = (from b in db.BicycleBaseInfo
                                    join s in db.BicycleLockInfo on b.LockNumber equals s.LockNumber
                                    join r in db.Reservation on s.LockGuid equals r.BicycleBaseGuid into temp
                                    from rr in temp.DefaultIfEmpty()
                                    where s.LockStatus == 1 && s.IsInElectronicFenCing == null && !s.IsDeleted  
                                    && (rr == null || (rr != null && (rr.Status != 1) && (rr.Status == 0 || rr.Status == 2)))
                                    && !s.IsDeleted
                                    && ((s.Longitude < q0.Longitude && s.Latitude < q0.Latitude) &&
                                       (s.Longitude > q1.Longitude && s.Latitude < q1.Latitude) &&
                                       (s.Longitude < q2.Longitude && s.Latitude > q2.Latitude) && (s.Longitude > q3.Longitude && s.Latitude > q3.Latitude))
                                    //&& (!qReservationList.Any() || rr == null || (rr != null && qReservationList.Contains(rr.ReservationGuid))) && s.IsInElectronicFenCing == null
                                    select new AdminMapHelpBicycleModel_OM
                                    {
                                        //DeviceNo = s.LockNumber,                      //设备编号
                                        DeviceNo = b.BicycleNumber,                    //车辆编号
                                        BicycleLongitude = s.Longitude ?? 0,          //经度
                                        BicycleLatitude = s.Latitude ?? 0,           //纬度
                                        BicyCleTypeName = b.BicyCleTypeName ?? 0    //车辆类型：0非助力车；1助力车

                                    }).Distinct();
                if (bicycleQuery.Any())
                {
                    //int count = bicycleQuery.ToList().Count();
                    var bicycleList = new List<AdminMapHelpBicycleModel_OM>();

                    foreach (var q in bicycleQuery)
                    {
                        //计算直线距离
                        var st0 = CoordinateHelper.QDistance(new Coordinates(CurLongitude, CurLatitude), new Coordinates(q.BicycleLongitude ?? 0, q.BicycleLatitude ?? 0));
                        if (st0 <= radius)
                        {
                            var res = from x in db.Reservation
                                      where x.DeviceNo == q.DeviceNo && (x.Status == 1 || x.Status == 3)
                                      select x;
                            if (!res.Any())
                            {
                                bicycleList.Add(new AdminMapHelpBicycleModel_OM
                                {
                                    DeviceNo = q.DeviceNo,                                //设备编号
                                    BicycleLongitude = q.BicycleLongitude ?? 0,          //经度
                                    BicycleLatitude = q.BicycleLatitude ?? 0,           //纬度
                                    BicyCleTypeName = q.BicyCleTypeName                 //车辆类型：0非助力车；1助力车

                                });
                            }
                        }
                    }
                    data.NoHelpTotalcount = bicycleList.Count();
                    data.NoHelpBicycleList = bicycleList.ToList();
                }

                #endregion

                #region 助力车数据
                //var helpBicQuery = (from b in db.BicycleBaseInfo
                //                    join s in db.BicycleLockInfo on b.LockNumber equals s.LockNumber
                //                    join r in db.Reservation on s.LockGuid equals r.BicycleBaseGuid into temp
                //                    from rr in temp.DefaultIfEmpty()
                //                    where s.LockStatus == 1 && s.IsInElectronicFenCing == null && !s.IsDeleted && b.BicyCleTypeName == 1
                //                    && (rr == null || (rr != null && (rr.Status != 1) && (rr.Status == 0 || rr.Status == 2)))
                //                    && !s.IsDeleted
                //                    && ((s.Longitude < q0.Longitude && s.Latitude < q0.Latitude) &&
                //                       (s.Longitude > q1.Longitude && s.Latitude < q1.Latitude) &&
                //                       (s.Longitude < q2.Longitude && s.Latitude > q2.Latitude) && (s.Longitude > q3.Longitude && s.Latitude > q3.Latitude))
                //                    //&& (!qReservationList.Any() || rr == null || (rr != null && qReservationList.Contains(rr.ReservationGuid))) && s.IsInElectronicFenCing == null
                //                    select new AdminMapHelpBicycleModel_OM
                //                    {
                //                        //DeviceNo = s.LockNumber,                      //设备编号
                //                        DeviceNo = b.BicycleNumber,                    //车辆编号
                //                        BicycleLongitude = s.Longitude ?? 0,          //经度
                //                        BicycleLatitude = s.Latitude ?? 0,           //纬度
                //                        BicyCleTypeName = b.BicyCleTypeName ?? 0    //车辆类型：0非助力车；1助力车

                //                    }).Distinct();
                //if (helpBicQuery.Any())
                //{
                //    //int count = bicycleQuery.ToList().Count();
                //    var helpBicycleList = new List<AdminMapHelpBicycleModel_OM>();

                //    foreach (var q in helpBicQuery)
                //    {
                //        //计算直线距离
                //        var st0 = CoordinateHelper.QDistance(new Coordinates(CurLongitude, CurLatitude), new Coordinates(q.BicycleLongitude ?? 0, q.BicycleLatitude ?? 0));
                //        if (st0 <= radius)
                //        {
                //            var res = from x in db.Reservation
                //                      where x.DeviceNo == q.DeviceNo && (x.Status == 1 || x.Status == 3)
                //                      select x;
                //            if (!res.Any())
                //            {
                //                helpBicycleList.Add(new AdminMapHelpBicycleModel_OM
                //                {
                //                    DeviceNo = q.DeviceNo,                                //设备编号
                //                    BicycleLongitude = q.BicycleLongitude ?? 0,          //经度
                //                    BicycleLatitude = q.BicycleLatitude ?? 0,           //纬度
                //                    BicyCleTypeName = q.BicyCleTypeName                 //车辆类型：0非助力车；1助力车

                //                });
                //            }
                //        }
                //    }
                //    data.HelpTotalcount = helpBicycleList.Count();
                //    data.HelpBicycleList = helpBicycleList.ToList();
                //}

                #endregion

                #endregion 处理车辆数据

                //车辆信息
                  result.ResObject = data.NoHelpBicycleList == null && data.EnclosureList == null ? null : data;

                //result.ResObject = data.NoHelpBicycleList == null && data.HelpBicycleList == null && data.EnclosureList == null ? null : data;


            }
            return result;
        }

        /// <summary>
        /// 生成电子围栏编号规则
        /// </summary>
        /// <returns></returns>
        public ResultModel GetCreateElectronicFenCingNumber()
        {
            var result = new ResultModel();
            string EnclosureSeq = "E" + OrderHelper.GenerateOrderNumber();
            result.IsSuccess = true;
            result.ResObject = EnclosureSeq;
            return result;

        }
        /// <summary>
        /// 统计后台车辆信息和电子围栏信息
        /// </summary>
        /// <returns></returns>
        public ResultModel GetStatisticalList()
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                //电子围栏总数
                var ETotal = db.Electronic_FenCing.Count(q => !q.IsDeleted);

                //车辆总数
                //var TotalCount = db.BicycleLockInfo.Count(s => !s.IsDeleted);
                var TotalCount = (from b in db.BicycleBaseInfo
                                  join s in db.BicycleLockInfo on b.LockNumber equals s.LockNumber select b).Count();

                //使用数
                string sql = " select count(*) as UserTotalCount from BicycleLockInfo as b  where (b.LockStatus=0 and   b.IsDeleted<>1)  and b.LockGuid not in( select BicycleBaseGuid from Reservation where Status=1 or Status=3) ";
                object UserTotalCount = DbHelperSQL.GetSingle(sql);
                if (UserTotalCount != null)
                {
                    UserTotalCount = UserTotalCount;
                }
                else
                {
                    UserTotalCount = 0;
                }


                //空闲数
                string sqlK = " select   count(*) as UserTotalCount from BicycleLockInfo as b  where b.LockStatus=1 and b.LockStatus not in(2,3)   and   b.IsDeleted<>1 ";
                object FreeTotalCount = DbHelperSQL.GetSingle(sqlK);
                if (FreeTotalCount != null)
                {
                    FreeTotalCount = FreeTotalCount;
                }
                else
                {
                    FreeTotalCount = 0;
                }

                //var FreeTotalCount = db.BicycleLockInfo.Count(s => !s.IsDeleted &&
                //                                                    s.LockStatus == 1 &&
                //                                                    (!db.Reservation.Where(q => q.BicycleBaseGuid == s.BicycleBaseGuid
                //                                               && !q.IsDeleted && new int?[] { 1, 3 }.Contains(q.Status)).Select(q => q.BicycleBaseGuid)
                //                                              .Contains(s.BicycleBaseGuid)));
                //故障数
                var breakdownTotalCount = db.BicycleLockInfo.Count(s => s.LockStatus == 3 || s.LockStatus == 2 && !s.IsDeleted);

                result.ResObject = new { ETotal = ETotal, TotalCount = TotalCount, UserTotalCount = UserTotalCount, FreeTotalCount = FreeTotalCount, breakdownTotalCount = breakdownTotalCount };
            }
            return result;

        }

        /// <summary>
        /// 统计区域的电子围栏总数和电子围栏下的车辆总数
        /// </summary>
        /// <returns></returns>
        public ResultModel GetElectronicTjCountByCondition(SearchTj_PM model)
        {
            var result = new ResultModel();
            int EleCount = 0;
            int BicycleCount = 0;
            string Name = string.Empty;
            string sql = string.Empty;
            var data = new SearchTjList_OM { };
            var TjModel = new List<SearchTj_OM>();

            #region 地图选择省市区搜索信息
            sql = "select sum(temp.EleCount) EleCount,min(temp.DistrictID) as DistrictID,min(temp.Name) as Name,sum(temp.bicycleCount) as bicycleCount from(select min(DistrictID) as DistrictID,(select Name from District where id=e.DistrictID) as Name,count(*) as EleCount,(select count(*) from BicycleLockInfo as b where b.ElectronicFenCingGuid=e.ElectronicFenCingGuid) bicycleCount from Electronic_FenCing as e  ";
            if ((model.ProvinceID != 0) && (model.CityID != 0) && (model.DistrictID != 0))
            {
                sql += "  where (ProvinceID='" + model.ProvinceID + "'and CityID='" + model.CityID + "' and   DistrictID='" + model.DistrictID + "')  ";

            }
            else if ((model.ProvinceID != 0) && (model.CityID != 0) && (model.DistrictID == 0)) //表示选择所有的区
            {
                sql += "  where (ProvinceID='" + model.ProvinceID + "'or CityID='" + model.CityID + "' )  ";

            }
            else if (model.ProvinceID == 0 || model.CityID == 0)
            {
                //return new ResultModel { IsSuccess = false, MsgCode = "0", Message = "请选择省和市" };
                result.IsSuccess = false;
                result.Message = "请选择省和市";
            }
            sql += "    group by DistrictID,e.ElectronicFenCingGuid)temp group by temp.DistrictID,temp.Name  ";

            #endregion

            DataTable dt = DbHelperSQL.dt_table(sql);
            if (dt.Rows.Count > 0 && dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    TjModel.Add(new SearchTj_OM
                    {
                        Name = dt.Rows[i]["Name"].ToString(),
                        EleCount = int.Parse(dt.Rows[i]["EleCount"].ToString()),
                        bicycleCount = int.Parse(dt.Rows[i]["bicycleCount"].ToString()),
                        //TotalCount =int.Parse(dt.Rows.Count.ToString()),
                    });
                }
            }
            else
            {
                result.IsSuccess = false;
                result.Message = "暂无数据";
            }
            data.EnclosureList = TjModel;
            data.Totalcount = TjModel.Count;
            //result.ResObject = new { EleCount = EleCount, BicycleCount = BicycleCount, Name = Name };
            result.ResObject = data.EnclosureList == null ? null : data;

            return result;

        }


    } 
}