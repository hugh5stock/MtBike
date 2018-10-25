using Autofac;
using MintCyclingService.Common;
using MintCyclingService.Photo;
using MintCyclingService.User;
using MintCyclingService.Utils;
using MintCyclingWebApi.Filter;
using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Utility.File;

namespace MintCyclingWebApi.AppControllers
{
    /// <summary>
    /// 用户个人中心控制器
    /// </summary>
    [RequestCheck]
    public class pcenterController : ApiController
    {
        private IUserInfoService _userInfoService;
        private IPhotoService _photoService;

        /// <summary>
        /// 初始化用户个人中心控制器
        /// </summary>
        public pcenterController()
        {
            _userInfoService = AutoFacConfig.Container.Resolve<IUserInfoService>();
            _photoService = AutoFacConfig.Container.Resolve<IPhotoService>();
        }

        /// <summary>
        /// 查询个人行程
        /// </summary>
        /// <returns></returns>
        public ResultModel qtraveldata([FromBody]UserTravel_PM model)
        {
            return _userInfoService.GetUserTravelByUserGuid(model);
        }

        /// <summary>
        /// 用户骑行完成结算费用
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResultModel qtravelend([FromBody]CyclingEnd_PM model)
        {
            return _userInfoService.GetUserTravelEndByUserGuid(model);
        }

        /// <summary>
        /// 实名认证用户的身份证号  complete TOM
        /// DATE:2017-02-18
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ResultModel AddUserAuthentication([FromBody]AddUserAuth_PM model)
        {
            return _userInfoService.AddUserAuthentication(model);
        }



        /// <summary>
        ///查询个人中心用户信息  complete TOM
        /// DATE:2017-02-17
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ResultModel GetUserInfoCenterByUserGuid([FromUri]Guid UserGuid)
        {
            return _userInfoService.GetUserInfoCenterByUserGuid(UserGuid);
        }

        /// <summary>
        ///查询个人详细信息 complete TOM
        /// DATE:2017-02-18
        /// </summary>
        /// <returns></returns>
       [HttpGet]
        public ResultModel GetUserInfoDetailByUserGuid([FromUri]Guid UserGuid)
        {
            return _userInfoService.GetUserinfoByUserGuid(UserGuid);
        }

        /// <summary>
        /// 通过用户UserGuid更新用户头像  complete TOM
        /// DATE:2017-02-19
        /// </summary>
        [HttpPost]
        public async Task<FineUploaderResultModel> UploadUserPhotoFile()
        {
            Guid guid = Guid.Empty;
            var st = Guid.TryParse(HttpContext.Current.Request["UserGuid"], out guid);
            if (st)
            {
                GetUserInfo_PM model = new GetUserInfo_PM { UserGuid = guid };
                var result = new FineUploaderResultModel();
                result = await EditUploadFile(Request, model.UserGuid, "UserLogo", "用户头像");
                return result;
            }
            return new FineUploaderResultModel { Message = ResPrompt.CustomerGuidErrorMessage, MsgCode = ResPrompt.CustomerGuidErrorMessage, IsSuccess = false };
        }


        /// <summary>
        /// 上传头像照片  complete TOM暂时没用
        /// DATE:2017-02-22
        /// </summary>
        /// <returns>上传照片结果</returns>
        [HttpPost]
        public async Task<FineUploaderResultModel> UploadAvatarPhotoFile()
        {
            var result = new FineUploaderResultModel();

            result = await UploadFile(Request, "UserLogo", "用户头像", PhotoTypeEnum.Avatar);

            return result;
        }


        /// <summary>
        ///修改用户的昵称或手机号 complete TOM
        /// DATE:2017-02-18
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ResultModel UpdateUserNickOrPhone([FromBody] EditUserPhoneOrNickName_PM model)
        {
            return _userInfoService.EditUserPhoneOrNickNameByUserGuid(model);
        }

        /// <summary>
        ///绑定充电宝
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ResultModel BindPowerBank([FromBody]BindPowerBank data)
        {


            return   _userInfoService.BindPowerBank(data);



        }

        /// <summary>
        /// 上传文件--用户修改或者添加图片
        /// </summary>
        /// <param name="Request">Http请求</param>
        /// <param name="uploadDir">上传目录</param>
        /// <param name="imgName">图片名</param>
        /// <returns>上传文件结果</returns>
        private async Task<FineUploaderResultModel> EditUploadFile(HttpRequestMessage Request, Guid UserGuid, string uploadDir, string imgName)
        {
            var result = new FineUploaderResultModel();
            //result.success = true;

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
                        // var guid = _photoRepo.AddPhoto(imgName, PhotoTypeEnum.Avatar, model.NewFileName, model.RelativePath).ResObject as Guid?;
                        var guid = _photoService.AddOrUpdatePhoto(UserGuid, imgName, PhotoTypeEnum.Avatar, model.NewFileName, model.RelativePath).ResObject as Guid?;
                        result.ResObject = new { Guid = guid, Path = model.RelativePath, absUrl = FileUtility.GetFullUrlByRelativePath(model.RelativePath), fileName = model.NewFileName };
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
                result.ResObject = new { MsgCode = "6020", Message = "上传图片信息出现错误：" + ex.Message +"\r\n"+ ex.StackTrace};
            }
            return result;
        }

        /// <summary>
        /// 上传文件--添加图片
        /// </summary>
        /// <param name="Request">Http请求</param>
        /// <param name="uploadDir">上传目录</param>
        /// <param name="imgName">图片名</param>
        /// <returns>上传文件结果</returns>
        private async Task<FineUploaderResultModel> UploadFile(HttpRequestMessage Request, string uploadDir, string imgName, PhotoTypeEnum typeEnum)
        {
            var result = new FineUploaderResultModel();
            //result.success = true;

            try
            {
                // Check whether the POST operation is MultiPart?
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

                    var guid = _photoService.AddPhoto(imgName, PhotoTypeEnum.Avatar, model.NewFileName, model.RelativePath).ResObject as Guid?;
                    result.ResObject = new { Guid = guid, Path = model.RelativePath, absUrl = FileUtility.GetFullUrlByRelativePath(model.RelativePath), fileName = model.NewFileName };

                    var thumbnailPath = Path.Combine(dir, Path.GetFileNameWithoutExtension(model.NewFileName) + "_1" + Path.GetExtension(model.NewFileName));

                    var _fileUtility = new FileUtility();
                    var imgFormat = _fileUtility.GetImageFormatByExtension(Path.GetExtension(thumbnailPath));
                    _fileUtility.GenerateThumbnail(200, 200, imgFile, thumbnailPath, imgFormat);
                }
            }
            catch (Exception ex)
            {
                //return new ResultModel { IsSuccess = false, MsgCode = "4042", Message = "上传图片信息出现错误" };
                result.ResObject = new { MsgCode = "6020", Message = "上传图片信息出现错误" };
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



    }
}