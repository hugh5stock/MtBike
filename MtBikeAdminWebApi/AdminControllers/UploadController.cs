using Autofac;
using MintCyclingService.Common;
using MintCyclingService.Photo;
using MintCyclingService.Utils;
using MtBikeAdminWebApi.Filter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Utility.File;

namespace MtBikeAdminWebApi.AdminControllers
{
    /// <summary>
    /// APP首页引导页广告位控制器
    /// </summary>
    public class UploadController : ApiController
    {
        IPhotoService _photoService;
        private ResultModel _adminModel = null;

        /// <summary>
        /// APP首页引导页广告位控制器
        /// </summary>
        public UploadController()
        {
            _adminModel = WebApiApplication.GetAdminUserData();
            _photoService = AutoFacConfig.Container.Resolve<IPhotoService>();
        }

        /// <summary>
        /// 增加引导页的图片
        /// </summary>
        /// <returns></returns>
        public ResultModel NewGuidePagePhoto()
        {
            if (!_adminModel.IsSuccess)
                return _adminModel;
            var req = HttpContext.Current.Request;
            var pics = req.Files;
            var tag = req["picTag"];
            var sDt = req["sDt"];
            var eDt = req["eDt"];
            var seq = req["seq"];
            if (pics.Count == 0 || string.IsNullOrEmpty(tag))
                return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ParaModelNotExist, Message = ResPrompt.ParaModelNotExistMessage };
            var result = new ResultModel { };
            try
            {
                var pic = pics[0];
                var strPicName = pic.FileName;
                var extension = Path.GetExtension(strPicName);
                var picType = new List<string> { ".jpg", ".gif", ".png", ".bmp", ".jpeg" };
                if (!picType.Any(s => s == extension)) return new ResultModel { IsSuccess = false, Message = "不是有效的图片格式" };
                var tq = PhotoTypeEnum.GuidePage;
                var now = DateTime.Now;
                var relativePathWithoutFileName = "~/Upload/" + tq.ToString() + "/" + now.Year.ToString() + now.Month.ToString().PadLeft(2, '0');
                var dir = HttpContext.Current.Server.MapPath(relativePathWithoutFileName);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                var preTag = Guid.NewGuid().ToString("N");
                var strPicTag = preTag + extension;
                var strPath = Path.Combine(dir, strPicTag);
                pic.SaveAs(strPath);
                var qSeq = 0;
                int.TryParse(seq, out qSeq);
                var dt0 = DateTime.MinValue; var dt1 = DateTime.MaxValue;
                DateTime.TryParse(sDt, out dt0);
                DateTime.TryParse(eDt, out dt1);
                var st = (_adminModel.ResObject) as Guid?;
                var model = new NewGuidePagePhoto
                {
                    Title = tag,
                    Url = relativePathWithoutFileName.Replace("~", "") + "/" + strPicTag,
                    SortId = qSeq,
                    BeginDate = dt0 == DateTime.MinValue ? (DateTime?)null : dt0,
                    EndDate = dt1 == DateTime.MaxValue ? (DateTime?)null : dt1,
                    AdminGuid = st
                };
                var sk = _photoService.AddGuidePagePhoto(model);
                result = sk;
            }
            catch
            {
                return new ResultModel { IsSuccess = false, Message = "新增引导页图片出现异常" };
            }
            return result;
        }

        /// <summary>
        /// 查询引导页图片数据
        /// </summary>
        /// <returns></returns>
        [CheckAdminAccessCodeFilter]
        public ResultModel QueryGuidePagePhoto()
        {
            return _photoService.GetGuidePagePhotoList();
        }

        /// <summary>
        /// 删除引导页图片
        /// </summary>
        /// <returns></returns>
        [CheckAdminAccessCodeFilter]
        public ResultModel RemoveGuidePagePhoto(GuidePagePhotoDeletion model)
        {
            var st = (_adminModel.ResObject) as Guid?;
            model.AdminGuid = st;
            return _photoService.DeleteGuidePagePhoto(model);
        }

        /// <summary>
        /// 编辑引导页
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [CheckAdminAccessCodeFilter]
        public ResultModel UpdateGuidePage(GuidePagePhotoModification model)
        {
            var st = (_adminModel.ResObject) as Guid?;
            model.AdminGuid = st;
            return _photoService.ModifyGuidePage(model);
        }

        /// <summary>
        /// 增加广告弹窗
        /// </summary>
        /// <returns></returns>
        public ResultModel NewCommercialPhoto()
        {
            if (!_adminModel.IsSuccess)
                return _adminModel;
            var req = HttpContext.Current.Request;
            var pics = req.Files;
            var tag = req["picTag"];
            var sDt = req["sDt"];
            var eDt = req["eDt"];
            var seq = req["seq"];
            if (pics.Count == 0 || string.IsNullOrEmpty(tag))
                return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.ParaModelNotExist, Message = ResPrompt.ParaModelNotExistMessage };
            var result = new ResultModel { };
            try
            {
                var pic = pics[0];
                var strPicName = pic.FileName;
                var extension = Path.GetExtension(strPicName);
                var picType = new List<string> { ".jpg", ".gif", ".png", ".bmp", ".jpeg" };
                if (!picType.Any(s => s == extension)) return new ResultModel { IsSuccess = false, Message = "不是有效的图片格式" };
                var tq = PhotoTypeEnum.Commercial;
                var now = DateTime.Now;
                var relativePathWithoutFileName = "~/Upload/" + tq.ToString() + "/" + now.Year.ToString() + now.Month.ToString().PadLeft(2, '0');
                var dir = HttpContext.Current.Server.MapPath(relativePathWithoutFileName);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                var preTag = Guid.NewGuid().ToString("N");
                var strPicTag = preTag + extension;
                var strPath = Path.Combine(dir, strPicTag);
                pic.SaveAs(strPath);
                var qSeq = 0;
                int.TryParse(seq, out qSeq);
                var dt0 = DateTime.MinValue; var dt1 = DateTime.MaxValue;
                DateTime.TryParse(sDt, out dt0);
                DateTime.TryParse(eDt, out dt1);
                var st = (_adminModel.ResObject) as Guid?;
                var model = new NewGuidePagePhoto
                {
                    Title = tag,
                    Url = relativePathWithoutFileName.Replace("~", "") + "/" + strPicTag,
                    SortId = qSeq,
                    BeginDate = dt0 == DateTime.MinValue ? (DateTime?)null : dt0,
                    EndDate = dt1 == DateTime.MaxValue ? (DateTime?)null : dt1,
                    AdminGuid = st
                };
                var sk = _photoService.AddCommercialPhoto(model);
                result = sk;
            }
            catch
            {
                return new ResultModel { IsSuccess = false, Message = "新增广告弹窗出现异常" };
            }
            return result;
        }

        /// <summary>
        /// 查询广告弹窗数据
        /// </summary>
        /// <returns></returns>
        [CheckAdminAccessCodeFilter]
        public ResultModel QueryCommercialPhoto()
        {
            return _photoService.GetCommercialList();
        }

        /// <summary>
        /// 删除广告弹窗
        /// </summary>
        /// <returns></returns>
        [CheckAdminAccessCodeFilter]
        public ResultModel RemoveCommercialPhoto(GuidePagePhotoDeletion model)
        {
            var st = (_adminModel.ResObject) as Guid?;
            model.AdminGuid = st;
            return _photoService.DeleteCommercial(model);
        }

        /// <summary>
        /// 编辑广告弹窗
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [CheckAdminAccessCodeFilter]
        public ResultModel UpdateCommercial(GuidePagePhotoModification model)
        {
            var st = (_adminModel.ResObject) as Guid?;
            model.AdminGuid = st;
            return _photoService.ModifyCommercial(model);
        }
    }
}
