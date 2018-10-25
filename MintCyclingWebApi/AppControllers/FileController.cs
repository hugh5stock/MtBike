//using Autofac;
//using ErpWebApi;
//using GreenShowRepo.Photo;
//using System;
//using System.Drawing;
//using System.IO;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Threading.Tasks;
//using System.Web;
//using System.Web.Http;
//using Utility.Common;
//using Utility.File;


//namespace GreenShowWebApi.UiControllers
//{
//    /// <summary>
//    /// 文件控制器
//    /// </summary>
//    public class FileController : ApiController
//    {
//        private IPhotoRepo _photoRepo;

//        /// <summary>
//        /// 文件控制器初始化
//        /// </summary>
//        public FileController()
//        {
//            _photoRepo = AutoFacConfig.Container.Resolve<IPhotoRepo>();
//        }

//        /// <summary>
//        /// 上传球球道图
//        /// </summary>
//        /// <returns>上传照片结果</returns>
//        [HttpPost]
//        public async Task<FineUploaderResultModel> UploadHolePhotoFile()
//        {
//            var result = new FineUploaderResultModel();

//            result = await UploadFile(Request, "Hole", "球道图", PhotoTypeEnum.Hole);

//            return result;
//        }

//        /// <summary>
//        /// 上传球场Logo
//        /// </summary>
//        /// <returns>上传照片结果</returns>
//        [HttpPost]
//        public async Task<FineUploaderResultModel> UploadClubLogoPhotoFile()
//        {
//            var result = new FineUploaderResultModel();

//            result = await UploadFile(Request, "ClubLogo", "球场Logo", PhotoTypeEnum.ClubLogo);

//            return result;
//        }

//        /// <summary>
//        /// 上传头像照片
//        /// </summary>
//        /// <returns>上传照片结果</returns>
//        [HttpPost]
//        public async Task<FineUploaderResultModel> UploadAvatarPhotoFile()
//        {
//            var result = new FineUploaderResultModel();

//            result = await UploadFile(Request, "Avatar", "用户头像", PhotoTypeEnum.Avatar);

//            return result;
//        }

//        /// <summary>
//        /// 上传分享用户照片
//        /// </summary>
//        /// <returns>上传分享用户照片结果</returns>
//        [HttpPost]
//        public async Task<FineUploaderResultModel> UploadUserSharePhotoFile()
//        {
//            var result = new FineUploaderResultModel();

//            result = await UploadFile(Request, "UserShare", "用户分享", PhotoTypeEnum.SharePhoto);

//            return result;
//        }
//        /// <summary>
//        /// 上传球队队徽 candy
//        /// </summary>
//        /// <returns></returns>
//         [HttpPost]
//        public async Task<FineUploaderResultModel> UploadTeamLogoPhotoFile()
//        {
//            var result = new FineUploaderResultModel();

//            result = await UploadFile(Request, "TeamLogo", "球队队徽", PhotoTypeEnum.TeamLogo);

//            return result;
//        }


//        /// <summary>
//        /// 上传活动资讯图片 candy
//        /// </summary>
//        /// <returns></returns>
//        [HttpPost]
//        public async Task<FineUploaderResultModel> UploadInfomationPhotoFile()
//        {
//            var result = new FineUploaderResultModel();

//            result = await UploadFile(Request, "EventInformation", "活动资讯", PhotoTypeEnum.TeamEventInfoPhoto);

//            return result;
//        }


//        // We implement MultipartFormDataStreamProvider to override the filename of File which  
//        // will be stored on server, or else the default name will be of the format like Body-  
//        // Part_{GUID}. In the following implementation we simply get the FileName from   
//        // ContentDisposition Header of the Request Body.  

//        /// <summary>
//        /// CustomMultipartFormDataStreamProvider
//        /// </summary>
//        private class CustomMultipartFormDataStreamProvider : MultipartFormDataStreamProvider
//        {
//            /// <summary>
//            /// CustomMultipartFormDataStreamProvider
//            /// </summary>
//            /// <param name="path">路径</param>
//            public CustomMultipartFormDataStreamProvider(string path) : base(path) { }

//            /// <summary>
//            /// 获取本地文件名
//            /// </summary>
//            /// <param name="headers">头部信息</param>
//            /// <returns>字符串</returns>
//            public override string GetLocalFileName(HttpContentHeaders headers)
//            {
//                // return headers.ContentDisposition.FileName.Replace("\"", string.Empty);

//                var oriFileName = headers.ContentDisposition.FileName.Replace("\"", string.Empty);
//                var ext = Path.GetExtension(oriFileName);
//                var guidFileName = Guid.NewGuid().ToString();
//                var newFileName = guidFileName.Replace("-", string.Empty);

//                return newFileName + ext;
//            }



//        }

//        /// <summary>
//        /// 上传文件
//        /// </summary>
//        /// <param name="Request">Http请求</param>
//        /// <param name="uploadDir">上传目录</param>
//        /// <param name="imgName">图片名</param>
//        /// <param name="typeEnum">图片类型枚举</param>
//        /// <returns>上传文件结果</returns>
//        private async Task<FineUploaderResultModel> UploadFile(HttpRequestMessage Request, string uploadDir, string imgName, PhotoTypeEnum typeEnum)
//        {
//            var result = new FineUploaderResultModel();
//            result.success = true;

//            // Check whether the POST operation is MultiPart?  
//            if (!Request.Content.IsMimeMultipartContent())
//            {
//                result.IsSuccess = false;
//                result.success = false;
//                result.Code = ResultCodeEnum.UnsupportedMediaType;
//                result.error = ResultCodeEnum.UnsupportedMediaTypeMessage;
//                return result;
//            }

//            // 根据当前时间获取上传路径
//            var now = DateTime.Now;

//            var relativePathWithoutFileName = "~/Upload/" + uploadDir + "/" + now.Year.ToString() + now.Month.ToString().PadLeft(2, '0');
//            var dir = HttpContext.Current.Server.MapPath(relativePathWithoutFileName);

//            // 如不存在目录则创建目录
//            if (!Directory.Exists(dir))
//            {
//                Directory.CreateDirectory(dir);
//            }

//            // Prepare CustomMultipartFormDataStreamProvider in which our multipart form  
//            // data will be loaded.  
//            CustomMultipartFormDataStreamProvider provider = new CustomMultipartFormDataStreamProvider(dir);

//            var model = new UploadPicFileInfoModel();


//            // Read all contents of multipart message into CustomMultipartFormDataStreamProvider.  
//            await Request.Content.ReadAsMultipartAsync(provider);

//            foreach (MultipartFileData file in provider.FileData)
//            {
//                var oriFileName = file.Headers.ContentDisposition.FileName.Replace("\"", string.Empty);

//                model.Extension = Path.GetExtension(oriFileName);
//                model.FullPath = file.LocalFileName;

//                var imgFile = Image.FromFile(model.FullPath);

//                model.Height = imgFile.Height;
//                model.Width = imgFile.Width;
//                model.NewFileName = Path.GetFileName(file.LocalFileName);
//                model.OriFileName = oriFileName;
//                model.RelativePath = relativePathWithoutFileName.Substring(1) + "/" + model.NewFileName;
//                model.Size = imgFile.Width * imgFile.Height;

//                var guid = _photoRepo.AddPhoto(imgName, typeEnum, model.NewFileName, model.RelativePath).ReturnObject as Guid?;
//                result.ReturnObject = new { Guid = guid, Path = model.RelativePath, absUrl = FileUtility.GetFullUrlByRelativePath(model.RelativePath), fileName = model.NewFileName };

//                var thumbnailPath = Path.Combine(dir, Path.GetFileNameWithoutExtension(model.NewFileName) + "_1" + Path.GetExtension(model.NewFileName));

//                var _fileUtility = new FileUtility();
//                var imgFormat = _fileUtility.GetImageFormatByExtension(Path.GetExtension(thumbnailPath));
//                _fileUtility.GenerateThumbnail(200, 200, imgFile, thumbnailPath, imgFormat);
//            }

//            //var dic = new Dictionary<string, string>();

//            //foreach (var key in provider.FormData.AllKeys)
//            //{//接收FormData  
//            //    dic.Add(key, provider.FormData[key]);
//            //}

//            return result;
//        }
//    }
//}
