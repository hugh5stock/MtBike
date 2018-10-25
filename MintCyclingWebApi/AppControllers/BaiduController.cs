using Autofac;
using MintCyclingService.BaiDu;
using MintCyclingService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace MintCyclingWebApi.AppControllers
{
    public class BaiduController : ApiController
    {
        private IBaiduService _baiduService;

        /// <summary>
        /// 初始化车辆模块控制器
        /// </summary>
        public BaiduController()
        {
            _baiduService = AutoFacConfig.Container.Resolve<IBaiduService>();
        }

        /// <summary>
        /// 测试通过经纬度查询详细地址   complete TOM
        /// DATE:2017-03-30
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ResultModel GetAddressBaiduApi(string Longitude, string Latitude)
        {
            ResultModel result = new ResultModel();
            //百度API从经纬度坐标到地址的转换服务
            string address = _baiduService.GetAddress(Longitude, Latitude);
            if (address != null)
            {
                result.ResObject = address;
            }
            return result;
        }




    }
}