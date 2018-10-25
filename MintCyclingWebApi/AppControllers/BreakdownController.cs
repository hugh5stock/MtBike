using Autofac;
using MintCyclingService.Breakdown;
using MintCyclingService.Common;
using MintCyclingService.Photo;
using MintCyclingService.Utils;
using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Utility.File;
using Utility.LogHelper;

namespace MintCyclingWebApi.AppControllers
{
    /// <summary>
    /// 故障控制器类
    /// </summary>
    public class BreakdownController : ApiController
    {
        private IBreakdownService _reakdownService;

        /// <summary>
        /// 初始化车辆模块控制器
        /// </summary>
        public BreakdownController()
        {
            _reakdownService = AutoFacConfig.Container.Resolve<IBreakdownService>();
        }

        /// <summary>
        /// 单车app故障上报   complete TOM
        /// DATE:2017-05-18
        /// </summary>s
        /// <returns></returns>

        [HttpPost]
        public ResultModel AddBreakDownLog([FromBody]AddBreakdownLog_PM model)
        {
            return _reakdownService.AddBreakDownLog(model);
        }

        /// <summary>
        /// 通过故障的Guid添加图片  complete TOM
        /// DATE:2017-05-18
        /// </summary>
        [HttpPost]
        public async Task<FineUploaderResultModel> UploadBreankDownPhotoFile()
        {

            //GetBreakdown_PM model = new GetBreakdown_PM { BreakdownGuid = guid };
            var result = new FineUploaderResultModel();
            var query = HttpContext.Current.Request.Form["BreakDownPhotoGuid"];
            var photoGuid = Guid.Empty;
            if (!string.IsNullOrEmpty(query))
            {
               photoGuid=Guid.Parse(query);
            }
            result = await UploadBreakDownFile(Request, "BreakDownBicycleLogo", "故障车图片", photoGuid);
            return result;
        }

        /// <summary>
        /// 通过添加持工证照片图片
        /// DATE:2017-05-18
        /// </summary>
        [HttpPost]
        public async Task<FineUploaderResultModel> UploadICBCCreditPhotoFile()
        {

            //GetBreakdown_PM model = new GetBreakdown_PM { BreakdownGuid = guid };
            var result = new FineUploaderResultModel();
            result = await UploadICBCCreditPhotoFile(Request, "ICBCCreditLogo", "持工证图片");
            return result;
        }


        #region 上传图片

        /// <summary>
        /// 上传文件--故障添加图片
        /// </summary>
        /// <param name="Request">Http请求</param>
        /// <param name="uploadDir">上传目录</param>
        /// <param name="imgName">图片名</param>
        ///  /// <param name="BreakPhotoGuid">故障图片Guid</param>
        /// <returns>上传文件结果</returns>
        private async Task<FineUploaderResultModel> UploadBreakDownFile(HttpRequestMessage Request, string uploadDir, string imgName,Guid BreakPhotoGuid)
        {
            var result = new FineUploaderResultModel();
            try
            {
                // Check whether the POST operation is MultiPart?
                //Request.Content

                if (!Request.Content.IsMimeMultipartContent())
                {
                    result.IsSuccess = false;
                    result.success = false;
                    result.MsgCode = ResPrompt.UnsupportedMediaType;
                    result.error = ResPrompt.UnsupportedMediaTypeMessage;
                    return result;
                }

                // 根据当前时间获取上传路径
                var now = DateTime.Now;

                var relativePathWithoutFileName = "~/Upload/" + uploadDir + "/" + now.Year.ToString() + now.Month.ToString().PadLeft(2, '0');
                var dir = HttpContext.Current.Server.MapPath(relativePathWithoutFileName);

                // 如不存在目录则创建目录
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                // Prepare CustomMultipartFormDataStreamProvider in which our multipart form
                // data will be loaded.
                CustomMultipartFormDataStreamProvider provider = new CustomMultipartFormDataStreamProvider(dir);

                var model = new UploadPicFileInfoModel();

                // Read all contents of multipart message into CustomMultipartFormDataStreamProvider.
                await Request.Content.ReadAsMultipartAsync(provider);

                foreach (MultipartFileData file in provider.FileData)
                {
                    var oriFileName = file.Headers.ContentDisposition.FileName.Replace("\"", string.Empty);

                    model.Extension = Path.GetExtension(oriFileName);
                    model.FullPath = file.LocalFileName;

                    var imgFile = Image.FromFile(model.FullPath);

                    model.Height = imgFile.Height;
                    model.Width = imgFile.Width;
                    model.NewFileName = Path.GetFileName(file.LocalFileName);
                    model.OriFileName = oriFileName;
                    model.RelativePath = relativePathWithoutFileName.Substring(1) + "/" + model.NewFileName;
                    model.Size = imgFile.Width * imgFile.Height;
                    try
                    {

                        var res = _reakdownService.AddPhoto(imgName, PhotoTypeEnum.BreakDownPhoto, model.NewFileName, model.RelativePath,BreakPhotoGuid);
                        if (res.IsSuccess)
                        {
                           var reslut=   res.ResObject as PhotoBreak_OM;
                            result.ResObject = new { PhotoGuid = reslut.PhotoGuid, BreakDownPhotoGuid = reslut.BreakDownPhotoGuid, Path = model.RelativePath, absUrl = FileUtility.GetFullUrlByRelativePath(model.RelativePath), fileName = model.NewFileName };
                        }
                        //if (guid!=null)
                        //{
                        //    result.IsSuccess = true;
                        //    result.MsgCode = "0";
                        //    result.Message = "添加故障图片成功";
                        //} else
                        //{
                        //    result.IsSuccess = true;
                        //    result.MsgCode = "0";
                        //    result.Message = "添加故障图片失败";
                        //}
                    }
                    catch (Exception ex)
                    {
                        result.ResObject = new { MsgCode = "7020", Message = "上传图片信息异常！" };
                        LogHelper.Error("上传图片信息异常:" + ex.Message);
                    }
                    var thumbnailPath = Path.Combine(dir, Path.GetFileNameWithoutExtension(model.NewFileName) + "_1" + Path.GetExtension(model.NewFileName));

                    var _fileUtility = new FileUtility();
                    var imgFormat = _fileUtility.GetImageFormatByExtension(Path.GetExtension(thumbnailPath));
                    _fileUtility.GenerateThumbnail(200, 200, imgFile, thumbnailPath, imgFormat);
                }
            }
            catch (Exception ex)
            {
                //return new ResultModel { IsSuccess = false, MsgCode = "4042", Message = "上传图片信息出现错误" };
                result.ResObject = new { MsgCode = "6020", Message = "上传图片信息出现错误：" + ex.Message + "\r\n" + ex.StackTrace };
                LogHelper.Error("上传图片信息出现错误:"+ ex.Message + "\r\n" + ex.StackTrace);
            }
            return result;
        }


      

        /// <summary>
        /// 上传文件--ICBC添加图片
        /// </summary>
        /// <param name="Request">Http请求</param>
        /// <param name="uploadDir">上传目录</param>
        /// <param name="imgName">图片名</param>
    
        /// <returns>上传文件结果</returns>
        private async Task<FineUploaderResultModel> UploadICBCCreditPhotoFile(HttpRequestMessage Request, string uploadDir, string imgName)
        {
            var result = new FineUploaderResultModel();
            try
            {
                // Check whether the POST operation is MultiPart?
                //Request.Content

                if (!Request.Content.IsMimeMultipartContent())
                {
                    result.IsSuccess = false;
                    result.success = false;
                    result.MsgCode = ResPrompt.UnsupportedMediaType;
                    result.error = ResPrompt.UnsupportedMediaTypeMessage;
                    return result;
                }

                // 根据当前时间获取上传路径
                var now = DateTime.Now;

                var relativePathWithoutFileName = "~/Upload/" + uploadDir + "/" + now.Year.ToString() + now.Month.ToString().PadLeft(2, '0');
                var dir = HttpContext.Current.Server.MapPath(relativePathWithoutFileName);

                // 如不存在目录则创建目录
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                // Prepare CustomMultipartFormDataStreamProvider in which our multipart form
                // data will be loaded.
                CustomMultipartFormDataStreamProvider provider = new CustomMultipartFormDataStreamProvider(dir);

                var model = new UploadPicFileInfoModel();

                // Read all contents of multipart message into CustomMultipartFormDataStreamProvider.
                await Request.Content.ReadAsMultipartAsync(provider);

                foreach (MultipartFileData file in provider.FileData)
                {
                    var oriFileName = file.Headers.ContentDisposition.FileName.Replace("\"", string.Empty);

                    model.Extension = Path.GetExtension(oriFileName);
                    model.FullPath = file.LocalFileName;

                    var imgFile = Image.FromFile(model.FullPath);

                    model.Height = imgFile.Height;
                    model.Width = imgFile.Width;
                    model.NewFileName = Path.GetFileName(file.LocalFileName);
                    model.OriFileName = oriFileName;
                    model.RelativePath = relativePathWithoutFileName.Substring(1) + "/" + model.NewFileName;
                    model.Size = imgFile.Width * imgFile.Height;
                    try
                    {

                        var res = _reakdownService.AddICBCPhoto(imgName, PhotoTypeEnum.ICBCCreditPhoto, model.NewFileName, model.RelativePath);
                        if (res.IsSuccess)
                        {
                            var reslut = res.ResObject as PhotoBreak_OM;
                            result.ResObject = new { ICBCPhotoGuid = reslut.PhotoGuid,Path = model.RelativePath, absUrl = FileUtility.GetFullUrlByRelativePath(model.RelativePath), fileName = model.NewFileName };
                        }
                        //if (guid!=null)
                        //{
                        //    result.IsSuccess = true;
                        //    result.MsgCode = "0";
                        //    result.Message = "添加故障图片成功";
                        //} else
                        //{
                        //    result.IsSuccess = true;
                        //    result.MsgCode = "0";
                        //    result.Message = "添加故障图片失败";
                        //}
                    }
                    catch (Exception ex)
                    {
                        result.ResObject = new { MsgCode = "7020", Message = "添加图片信息异常！" };
                    }
                    var thumbnailPath = Path.Combine(dir, Path.GetFileNameWithoutExtension(model.NewFileName) + "_1" + Path.GetExtension(model.NewFileName));

                    var _fileUtility = new FileUtility();
                    var imgFormat = _fileUtility.GetImageFormatByExtension(Path.GetExtension(thumbnailPath));
                    _fileUtility.GenerateThumbnail(200, 200, imgFile, thumbnailPath, imgFormat);
                }
            }
            catch (Exception ex)
            {
                //return new ResultModel { IsSuccess = false, MsgCode = "4042", Message = "上传图片信息出现错误" };
                result.ResObject = new { MsgCode = "6020", Message = "上传图片信息出现错误：" + ex.Message + "\r\n" + ex.StackTrace };
            }
            return result;
        }

        /// <summary>
        /// CustomMultipartFormDataStreamProvider
        /// </summary>
        private class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
        {
            /// <summary>
            /// CustomMultipartFormDataStreamProvider
            /// </summary>
            /// <param name="path">路径</param>
            public CustomMultipartFormDataStreamProvider(string path) : base(path) { }

            /// <summary>
            /// 获取本地文件名
            /// </summary>
            /// <param name="headers">头部信息</param>
            /// <returns>字符串</returns>
            public override string GetLocalFileName(HttpContentHeaders headers)
            {
                // return headers.ContentDisposition.FileName.Replace("\"", string.Empty);

                var oriFileName = headers.ContentDisposition.FileName.Replace("\"", string.Empty);
                var ext = Path.GetExtension(oriFileName);
                var guidFileName = Guid.NewGuid().ToString();
                var newFileName = guidFileName.Replace("-", string.Empty);

                return newFileName + ext;
            }
        }

        #endregion 上传图片


    }
}