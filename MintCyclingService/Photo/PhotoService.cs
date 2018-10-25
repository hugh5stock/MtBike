using MintCyclingData;
using MintCyclingService.Common;
using MintCyclingService.Utils;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using Utility.File;

namespace MintCyclingService.Photo
{
    public class PhotoService : IPhotoService
    {
        private static string BicImageUrl = System.Web.Configuration.WebConfigurationManager.AppSettings["BicImageUrl"];                      //AK
        

        /// <summary>
        /// APP首页广告位查询
        /// </summary>
        /// <returns></returns>
        public ResultModel GetPhotoUrlInfo()
        {
            var result = new ResultModel();
            List<PhotoData_OM> list = new List<PhotoData_OM>();
            using (var db = new MintBicycleDataContext())
            {
                var query = (from s in db.AdvertisingInfo
                             where !s.IsDeleted && s.Status == 1 && s.TypeName == (int)PhotoTypeEnum.GuidePage
                             orderby s.CreateTime descending
                             select s);
                if (query.Any())
                {
                    //var admin = new Func<Guid?, string>(k =>
                    //{
                    //    var qk = db.Admin.FirstOrDefault(t => t.AdminGuid == k);
                    //    return qk == null ? string.Empty : qk.UserName;
                    //});
                    foreach (var q in query)
                    {
                        list.Add(new PhotoData_OM
                        {
                            PicGuid = q.AdverGuid,
                            Title = q.Title,
                            Url = BicImageUrl+q.Url
                        });
                    }
                }
            }

            return new ResultModel { ResObject = new { List = list } };
        }

        /// <summary>
        /// APP首页广告弹窗
        /// </summary>
        /// <returns></returns>
        public ResultModel GetAdvertPhotoUrlWindow()
        {
            var result = new ResultModel();
            string url = string.Empty;
            using (var db = new MintBicycleDataContext())
            {
                var query = (from s in db.AdvertisingInfo
                             where !s.IsDeleted && s.Status == 1 && s.TypeName ==(int) PhotoTypeEnum.Commercial
                             orderby s.CreateTime descending
                             select s).FirstOrDefault();
                if (query!=null)
                {
                    url = BicImageUrl + query.Url;
                }             
            }

            return new ResultModel { ResObject = new { Url = url } };
        }


        public ResultModel GetHttpGet()
        {
            var result = new ResultModel();
            
            string Address = string.Empty;
            string strUrl = @"http://www.mintcar.com:80/mintbikeservice/UpgradeFile.aspx?Mac=d43639a53DF0&Version=1.0&Url=http://www.mintcar.com:8082/1152.bin";

            string statusStr = string.Empty;
            string strResult;
            try
            {
                //创建远程服务请求
                WebRequest request = WebRequest.Create(strUrl);
                request.Timeout = 12000;
                request.Method = "GET";
                
                HttpWebResponse HttpWResp = (HttpWebResponse)request.GetResponse();
                Stream myStream = HttpWResp.GetResponseStream();
                StreamReader reader = new StreamReader(myStream, System.Text.Encoding.GetEncoding("utf-8"));
                strResult = reader.ReadToEnd();
            }
            catch (Exception exp)
            {
                statusStr = "错误：" + exp.Message;
            }
            return result;
        }

        /// <summary>
        /// 根据图片Guid获取图片的完整Url
        /// </summary>
        /// <param name="photoGuid">图片Guid</param>
        /// <returns>完整Url</returns>
        public ResultModel GetPhotoUrlByGuid(Guid photoGuid)
        {
            var result = new ResultModel();

            using (var db = new MintBicycleDataContext())
            {
                var queryPhoto = (from photo in db.Photo
                                  where photo.PhotoGuid == photoGuid
                                  select photo).FirstOrDefault();

                if (queryPhoto == null)
                {
                    result.IsSuccess = false;
                    result.MsgCode = ResPrompt.PhotoNotExisted;
                    result.Message = ResPrompt.PhotoNotExistedMessage;

                    return result;
                }

                result.ResObject = FileUtility.GetFullUrlByRelativePath(queryPhoto.Url);
            }

            return result;
        }

        /// <summary>
        /// 新增或者修改照片
        /// </summary>
        /// <param name="title">照片标题</param>
        /// <param name="photoTypeEnum">照片类型</param>
        /// <param name="fileName">文件名</param>
        /// <param name="url">文件路径</param>
        /// <returns>新增或者修改照片</returns>
        public ResultModel AddOrUpdatePhoto(Guid UserGuid, string title, PhotoTypeEnum photoType, string fileName, string url)
        {
            var result = new ResultModel();
            var now = DateTime.Now;
            try
            {
                using (var db = new MintBicycleDataContext())
                {
                    var userinfo = db.UserInfo.FirstOrDefault(s => s.UserInfoGuid == UserGuid);
                    if (userinfo != null)
                    {
                        #region 修改或则新增

                        if (userinfo.PhotoGuid != null) //修改图片
                        {
                            var query = db.Photo.FirstOrDefault(s => s.PhotoGuid == userinfo.PhotoGuid);
                            if (query != null)
                            {
                                query.Title = title;
                                query.Type = (byte)photoType;
                                query.FileName = fileName;
                                query.Url = url;

                                db.SubmitChanges();
                                result.ResObject = userinfo.PhotoGuid;
                            }
                        }
                        else  //添加
                        {
                            var newPhoto = new MintCyclingData.Photo();
                            newPhoto.PhotoGuid = Guid.NewGuid();
                            newPhoto.Title = title;
                            newPhoto.Type = (byte)photoType;
                            newPhoto.FileName = fileName;
                            newPhoto.Url = url;
                            newPhoto.IsDeleted = false;
                            newPhoto.CreateTime = now;

                            db.Photo.InsertOnSubmit(newPhoto);
                            db.SubmitChanges();

                            result.ResObject = newPhoto.PhotoGuid;
                        }

                        #endregion 修改或则新增
                    }
                    else
                    {
                        return new ResultModel { IsSuccess = false, MsgCode = "9056", Message = "没有此用户信息" };
                    }
                }
            }
            catch (Exception ex)
            {
                return new ResultModel { IsSuccess = false, MsgCode = "4042", Message = "添加图片信息出现错误" };
            }
            return result;
        }

        /// <summary>
        /// 新增照片
        /// </summary>
        /// <param name="title">照片标题</param>
        /// <param name="photoTypeEnum">照片类型</param>
        /// <param name="fileName">文件名</param>
        /// <param name="url">文件路径</param>
        /// <returns>新增照片</returns>
        public ResultModel AddPhoto(string title, PhotoTypeEnum photoType, string fileName, string url)
        {
            var result = new ResultModel();
            var now = DateTime.Now;
            try
            {
                using (var db = new MintBicycleDataContext())
                {
                    var newPhoto = new MintCyclingData.Photo();
                    newPhoto.PhotoGuid = Guid.NewGuid();
                    newPhoto.Title = title;
                    newPhoto.Type = (byte)photoType;
                    newPhoto.FileName = fileName;
                    newPhoto.Url = url;
                    newPhoto.IsDeleted = false;
                    newPhoto.CreateTime = now;

                    db.Photo.InsertOnSubmit(newPhoto);
                    db.SubmitChanges();

                    result.ResObject = newPhoto.PhotoGuid;
                }
            }
            catch (Exception ex)
            {
                return new ResultModel { IsSuccess = false, MsgCode = "4042", Message = "添加图片信息出现错误" };
            }
            return result;
        }

        /// <summary>
        /// 修改图片
        /// </summary>
        /// <param name="title">照片标题</param>
        /// <param name="photoTypeEnum">照片类型</param>
        /// <param name="fileName">文件名</param>
        /// <param name="url">文件路径</param>
        /// <returns>修改图片</returns>
        public ResultModel EditPhoto(Guid PhotoGuid, string title, PhotoTypeEnum photoType, string fileName, string url)
        {
            var result = new ResultModel();
            var now = DateTime.Now;

            using (var db = new MintBicycleDataContext())
            {
                var query = db.Photo.FirstOrDefault(s => s.PhotoGuid == PhotoGuid);
                if (query != null)
                {
                    query.Title = title;
                    query.Type = (byte)photoType;
                    query.FileName = fileName;
                    query.Url = url;

                    db.SubmitChanges();
                    result.ResObject = true;
                }
            }

            return result;
        }

        /// <summary>
        /// 新增引导页的图片
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public ResultModel AddGuidePagePhoto(NewGuidePagePhoto para)
        {
            var result = new ResultModel();
            var now = DateTime.Now;
            try
            {
                var displayOrder = 1;
                using (var db = new MintBicycleDataContext())
                {
                    var st = ConfigurationManager.AppSettings["MaxGuidePagePhotoNum"];
                    var num = 0;
                    var qData = db.AdvertisingInfo.Where(s => !s.IsDeleted && s.TypeName==(int)AdvertisingType.GuidePage);
                    if (int.TryParse(st, out num))
                    {
                        var cnt = qData.Count();
                        if (cnt >= num)
                            return new ResultModel { IsSuccess = false, MsgCode = "4042", Message = "最多可上传" + num + "张引导页图片" };
                    }
                    if (para.SortId < 1)
                    {
                        displayOrder = qData.Any() ? qData.Max(t => t.SortId ?? 0) + 1 : 1;
                    }
                    else
                    {
                        if (qData.Any(y => y.SortId == para.SortId))
                        {
                            displayOrder = qData.Max(t => t.SortId ?? 0) + 1;
                        }
                    }
                    var newPic = new AdvertisingInfo
                    {
                        AdverGuid = Guid.NewGuid(),
                        Title = para.Title,
                        AdverContent = para.AdverContent,
                        TypeName = (int)AdvertisingType.GuidePage,
                        Url = para.Url,
                        Status = 1,
                        SortId = displayOrder,
                        BeginDate = para.BeginDate,
                        EndDate = para.EndDate,
                        CreateBy = para.AdminGuid,
                        CreateTime = DateTime.Now,
                        IsDeleted = false
                    };
                    db.AdvertisingInfo.InsertOnSubmit(newPic);
                    db.SubmitChanges();
                }
            }
            catch
            {
                return new ResultModel { IsSuccess = false, MsgCode = "4042", Message = "添加引导页图片出现错误" };
            }
            return result;
        }

        /// <summary>
        /// 查询引导页图片数据
        /// </summary>
        /// <returns></returns>
        public ResultModel GetGuidePagePhotoList()
        {
            var sk = GetGuidePagePhotos(AdvertisingType.GuidePage);
            var cnt = sk.Any() ? sk.Count : 0;
            var st = ConfigurationManager.AppSettings["MaxGuidePagePhotoNum"];
            var num = 5;
            int.TryParse(st, out num);
            return new ResultModel { ResObject = new { Total = cnt, List = sk, QNum = num } };
        }

        /// <summary>
        /// 删除引导页图片
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public ResultModel DeleteGuidePagePhoto(GuidePagePhotoDeletion para)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var admin = db.Admin.FirstOrDefault(q => q.AdminGuid == para.AdminGuid && !q.IsDeleted);
                if (admin == null)
                    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ParaModelNotExist, Message = ResPrompt.ParaModelNotExistMessage };
                var query = db.AdvertisingInfo.FirstOrDefault(s => s.AdverGuid == para.PicGuid);
                if (query != null)
                {
                    query.IsDeleted = true;
                    query.UpdateBy = para.AdminGuid;
                    query.UpdateTime = DateTime.Now;
                    try
                    {
                        db.SubmitChanges();
                    }
                    catch
                    {
                        return new ResultModel { IsSuccess = false, MsgCode = "4042", Message = "删除引导页图片出现错误" };
                    }
                    result.ResObject = true;
                }
                else
                {
                    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ParaModelNotExist, Message = ResPrompt.ParaModelNotExistMessage };
                }
            }
            return result;
        }

        /// <summary>
        /// 编辑引导页
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public ResultModel ModifyGuidePage(GuidePagePhotoModification para)
        {
            if (!para.BeginDate.HasValue || !para.EndDate.HasValue)
                return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ParaModelNotExist, Message = ResPrompt.ParaModelNotExistMessage };
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var admin = db.Admin.FirstOrDefault(q => q.AdminGuid == para.AdminGuid && !q.IsDeleted);
                if (admin == null)
                    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ParaModelNotExist, Message = ResPrompt.ParaModelNotExistMessage };
                var query = db.AdvertisingInfo.FirstOrDefault(s => s.AdverGuid == para.PicGuid);
                if (query != null)
                {
                    var displayOrder = 1;
                    if (para.Seq < displayOrder)
                    {
                        var qData = db.AdvertisingInfo.Where(s => !s.IsDeleted && s.TypeName == (int)AdvertisingType.GuidePage);
                        displayOrder = qData.Any() ? qData.Max(t => t.SortId ?? 0) + 1 : 1;
                    }
                    else
                    {
                        displayOrder = para.Seq;
                    }
                    query.SortId = displayOrder;
                    query.BeginDate = para.BeginDate;
                    query.EndDate = para.EndDate;
                    query.UpdateBy = para.AdminGuid;
                    query.UpdateTime = DateTime.Now;
                    try
                    {
                        db.SubmitChanges();
                    }
                    catch
                    {
                        return new ResultModel { IsSuccess = false, MsgCode = "4042", Message = "编辑引导页图片出现错误" };
                    }
                    result.ResObject = true;
                }
                else
                {
                    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ParaModelNotExist, Message = ResPrompt.ParaModelNotExistMessage };
                }
            }
            return result;
        }

        /// <summary>
        /// 新增广告弹窗
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public ResultModel AddCommercialPhoto(NewGuidePagePhoto para)
        {
            var result = new ResultModel();
            var now = DateTime.Now;
            try
            {
                var displayOrder = 1;
                using (var db = new MintBicycleDataContext())
                {
                    var st = ConfigurationManager.AppSettings["MaxCommercialPhotoNum"];
                    var num = 0;
                    var qData = db.AdvertisingInfo.Where(s => !s.IsDeleted && s.TypeName == (int)AdvertisingType.Commercial);
                    if (int.TryParse(st, out num))
                    {
                        var cnt = qData.Count();
                        if (cnt >= num)
                            return new ResultModel { IsSuccess = false, MsgCode = "4042", Message = "最多可上传" + num + "张广告弹窗图片" };
                    }
                    if (para.SortId < 1)
                    {
                        displayOrder = qData.Any() ? qData.Max(t => t.SortId ?? 0) + 1 : 1;
                    }
                    else
                    {
                        if (qData.Any(y => y.SortId == para.SortId))
                        {
                            displayOrder = qData.Max(t => t.SortId ?? 0) + 1;
                        }
                    }
                    var newPic = new AdvertisingInfo
                    {
                        AdverGuid = Guid.NewGuid(),
                        Title = para.Title,
                        AdverContent = para.AdverContent,
                        TypeName = (int)AdvertisingType.Commercial,
                        Url = para.Url,
                        Status = 1,
                        SortId = displayOrder,
                        BeginDate = para.BeginDate,
                        EndDate = para.EndDate,
                        CreateBy = para.AdminGuid,
                        CreateTime = DateTime.Now,
                        IsDeleted = false
                    };
                    db.AdvertisingInfo.InsertOnSubmit(newPic);
                    db.SubmitChanges();
                }
            }
            catch
            {
                return new ResultModel { IsSuccess = false, MsgCode = "4042", Message = "添加广告弹窗出现错误" };
            }
            return result;
        }

        /// <summary>
        /// 查询广告弹窗数据
        /// </summary>
        /// <returns></returns>
        public ResultModel GetCommercialList()
        {
            var sk = GetGuidePagePhotos(AdvertisingType.Commercial);
            var cnt = sk.Any() ? sk.Count : 0;
            var st = ConfigurationManager.AppSettings["MaxCommercialPhotoNum"];
            var num = 5;
            int.TryParse(st, out num);
            return new ResultModel { ResObject = new { Total = cnt, List = sk, QNum = num } };
        }

        /// <summary>
        /// 删除广告弹窗
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public ResultModel DeleteCommercial(GuidePagePhotoDeletion para)
        {
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var admin = db.Admin.FirstOrDefault(q => q.AdminGuid == para.AdminGuid && !q.IsDeleted);
                if (admin == null)
                    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ParaModelNotExist, Message = ResPrompt.ParaModelNotExistMessage };
                var query = db.AdvertisingInfo.FirstOrDefault(s => s.AdverGuid == para.PicGuid);
                if (query != null)
                {
                    query.IsDeleted = true;
                    query.UpdateBy = para.AdminGuid;
                    query.UpdateTime = DateTime.Now;
                    try
                    {
                        db.SubmitChanges();
                    }
                    catch
                    {
                        return new ResultModel { IsSuccess = false, MsgCode = "4042", Message = "删除广告弹窗出现错误" };
                    }
                    result.ResObject = true;
                }
                else
                {
                    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ParaModelNotExist, Message = ResPrompt.ParaModelNotExistMessage };
                }
            }
            return result;
        }

        /// <summary>
        /// 编辑广告弹窗
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public ResultModel ModifyCommercial(GuidePagePhotoModification para)
        {
            if (!para.BeginDate.HasValue || !para.EndDate.HasValue)
                return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ParaModelNotExist, Message = ResPrompt.ParaModelNotExistMessage };
            var result = new ResultModel();
            using (var db = new MintBicycleDataContext())
            {
                var admin = db.Admin.FirstOrDefault(q => q.AdminGuid == para.AdminGuid && !q.IsDeleted);
                if (admin == null)
                    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ParaModelNotExist, Message = ResPrompt.ParaModelNotExistMessage };
                var query = db.AdvertisingInfo.FirstOrDefault(s => s.AdverGuid == para.PicGuid);
                if (query != null)
                {
                    var displayOrder = 1;
                    if (para.Seq < displayOrder)
                    {
                        var qData = db.AdvertisingInfo.Where(s => !s.IsDeleted && s.TypeName == (int)AdvertisingType.Commercial);
                        displayOrder = qData.Any() ? qData.Max(t => t.SortId ?? 0) + 1 : 1;
                    }
                    else
                    {
                        displayOrder = para.Seq;
                    }
                    query.SortId = displayOrder;
                    query.BeginDate = para.BeginDate;
                    query.EndDate = para.EndDate;
                    query.UpdateBy = para.AdminGuid;
                    query.UpdateTime = DateTime.Now;
                    try
                    {
                        db.SubmitChanges();
                    }
                    catch
                    {
                        return new ResultModel { IsSuccess = false, MsgCode = "4042", Message = "编辑广告弹窗出现错误" };
                    }
                    result.ResObject = true;
                }
                else
                {
                    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ParaModelNotExist, Message = ResPrompt.ParaModelNotExistMessage };
                }
            }
            return result;
        }

        /// <summary>
        /// 查询广告位
        /// </summary>
        /// <returns></returns>
        private List<GuidePagePhotoData> GetGuidePagePhotos(AdvertisingType tq)
        {
            List<GuidePagePhotoData> list = new List<GuidePagePhotoData>();
            using (var db = new MintBicycleDataContext())
            {
                var dt = DateTime.Now.Date;
                var tDt = dt.AddDays(1);
                var query = (from s in db.AdvertisingInfo
                             where !s.IsDeleted && s.Status == 1 && s.TypeName==(int)tq
                             orderby s.CreateTime descending
                             select s);
                if (query.Any())
                {
                    var admin = new Func<Guid?, string>(k =>
                    {
                        var qk = db.Admin.FirstOrDefault(t => t.AdminGuid == k);
                        return qk == null ? string.Empty : qk.UserName;
                    });
                    foreach (var q in query)
                    {
                        list.Add(new GuidePagePhotoData
                        {
                            PicGuid = q.AdverGuid,
                            Title = q.Title,
                            Pic = q.Url,
                            Seq = q.SortId ?? 1,
                            BeginDate = q.BeginDate,
                            EndDate = q.EndDate,
                            AdminName = admin(q.CreateBy),
                            CreateTime = q.CreateTime
                        });
                    }
                }
            }
            return list;
        }
    }
}