using Autofac;
using MintCyclingService.Common;
using MintCyclingService.Photo;
using MintCyclingService.Utils;
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

namespace MintCyclingWebApi.AppControllers
{
    /// <summary>
    /// APP首页引导页广告位控制器
    /// </summary>
    public class UploadController : ApiController
    {
        IPhotoService _photoService;

        /// <summary>
        /// APP首页引导页广告位控制器
        /// </summary>
        public UploadController()
        {
            _photoService = AutoFacConfig.Container.Resolve<IPhotoService>();
        }

        /// <summary>
        /// 查询APP首页引导页广告位 complete TOM
        /// DATE:2017-06-19
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ResultModel GetAdvertPhotoUrlInfo()
        {
            return _photoService.GetPhotoUrlInfo();
        }


        /// <summary>
        /// 查询APP首页弹出广告弹窗 complete TOM
        /// DATE:2017-06-20
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ResultModel GetAdvertPhotoUrlWindow()
        {
            return _photoService.GetAdvertPhotoUrlWindow();
        }



        /// <summary>
        /// 测试主动发送Http的get请求 complete TOM
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ResultModel RemoteUpgradeHttpGet()
        {
             return _photoService.GetHttpGet();
        }




    }
}
