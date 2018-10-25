using Autofac;
using MintCyclingService.AdminAccessCode;
using MintCyclingService.Breakdown;
using MintCyclingService.Common;
using MintCyclingService.Utils;
using MtBikeAdminWebApi.Filter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Http;
using System.Web.UI.HtmlControls;
using Utility.Common;

namespace MtBikeAdminWebApi.AdminControllers
{
    /// <summary>
    /// 后台故障管理控制器
    /// </summary>
    [CheckAdminAccessCodeFilter]
    public class AdminBreakdownController : ApiController
    {
        IBreakdownService _BreakdownService;
        IAdminAccessCodeService _AdminAccessCodeService;

        /// <summary>
        /// 初始化单车控制器
        /// </summary>
        public AdminBreakdownController()
        {
            _BreakdownService = AutoFacConfig.Container.Resolve<IBreakdownService>();
            _AdminAccessCodeService= AutoFacConfig.Container.Resolve<MintCyclingService.AdminAccessCode.IAdminAccessCodeService>();
        }


        /// <summary>
        /// 根据查询条件搜索故障维护列表  complete TOM
        ///DATE：2017-02-23
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultModel GetBreakdownListByCondition([FromUri]GetBreakdownList_PM model)
        {
            var result = new ResultModel();
           
            result = _BreakdownService.GetBreakDownList(model);
            //if (model.T == "Search") //查询
            //{
            //    result= _BreakdownService.GetBreakDownList(model);
            //}
            //else
            //{
            //   result = ExportBreakDownData(model);

            //}
            return result;
        }

        //导出Excel[暂时废弃]
        public ResultModel ExportBreakDownData(GetBreakdownList_PM model)
        {
            
           var  res = _BreakdownService.GetBreakDownList(model);
 
            var url = HttpContext.Current.Request.Url.Host + ":" + HttpContext.Current.Request.Url.Port;

            var starg = Guid.NewGuid().ToString().Replace("-", string.Empty);

            var list = res.ResObject as BreakdownListreturn;


            var list1 = ExportExcelHelper.PadExcelUrl<GetBreakdownList_OM>(list.List, starg, url, "", "");


            return new ResultModel
            {
                ResObject = list1
            };

            #region    导出暂时废弃
            //NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //NPOI.SS.UserModel.ISheet sheet = book.CreateSheet("test_01");

            //NPOI.SS.UserModel.IRow rowa = sheet.CreateRow(0);
            //rowa.CreateCell(0).SetCellValue("车辆编号");
            //rowa.CreateCell(1).SetCellValue("所属区域");
            //rowa.CreateCell(2).SetCellValue("维修等级");
            //rowa.CreateCell(3).SetCellValue("故障类型");
            //rowa.CreateCell(4).SetCellValue("详细地址");
            //rowa.CreateCell(5).SetCellValue("备注");

            //var BreakList = result.ResObject as listModel_OM;
            //if (BreakList != null)
            //{
            //    var list = BreakList.List;
            //    for (int i = 0; i < list.Count; i++)
            //    {
            //        NPOI.SS.UserModel.IRow row = sheet.CreateRow(i + 1);
            //        row.CreateCell(0).SetCellValue(list[i].BicyCleNumber);
            //        row.CreateCell(1).SetCellValue(list[i].DistricName);
            //        row.CreateCell(2).SetCellValue(list[i].GradeName);
            //        row.CreateCell(3).SetCellValue(list[i].BreakTypeName);
            //        row.CreateCell(4).SetCellValue(list[i].Address);
            //        row.CreateCell(5).SetCellValue(list[i].Remark);
            //    }

            //    // 写入到客户端
            //    System.IO.MemoryStream ms = new System.IO.MemoryStream();
            //    book.Write(ms);
            //    System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xls", DateTime.Now.ToString("yyyyMMddHHmmssfff")));
            //    System.Web.HttpContext.Current.Response.BinaryWrite(ms.ToArray());
            //    book = null;
            //    ms.Close();
            //    ms.Dispose();
            //}
            //else
            //{
            //    //不存在故障数据
            //    return new ResultModel { IsSuccess = false, MsgCode = ResPrompt.BreakDownNotExist, Message = ResPrompt.BreakDownNotExistMessage };
            //}
            //result.ResObject = true;
            //return result;

            #endregion

        }

        /// <summary>
        /// 查询故障维护详情  complete TOM
        ///DATE：2017-02-27
        /// </summary>
        /// <param name="BreakdownGuid"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultModel GetBreakDownByDetail([FromUri]Guid BreakdownGuid)
        {
            return _BreakdownService.GetBreakDownDetail(BreakdownGuid);
        }

        /// <summary>
        /// 新增或修改故障维护信息 complete TOM
        ///DATE：2017-02-27
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultModel AddOrUpdateBreakDown([FromBody] AddOrUpdateBreakdown_PM model)
        {
            var accesscode = Guid.Parse(HttpContext.Current.Request.Headers.GetValues(WebApiApplication.AccessCodeName)[0]);
            var operationId = _AdminAccessCodeService.GetAdminGuidByAccessCode(accesscode);
            model.OperatorGuid = operationId;
            return _BreakdownService.AddOrUpdateBreakDown(model);
        }

        /// <summary>
        /// 导入Excel功能 complete TOM
        ///DATE：2017-04-07
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ResultModel ImportExcelData([FromBody] ImportExcelData_PM model)
        {
          
            return _BreakdownService.ImportExcelFileData(model);
        }


        /// <summary>
        /// 维修记录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpGet]
        public ResultModel RepairRecord([FromUri]AdminRepairRecord_PM data)
        {

            return _BreakdownService.AdminRepairRecord(data);



        }


        /// <summary>
        /// 请求一分钟之内的通知
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ResultModel Getnotification()
        {

           return   _BreakdownService.Getnotification();

        }


        /// <summary>
        /// 上传excel
        /// </summary>
        /// <returns></returns>
        //public ResultModel UploadFile()
        //{
        //    HttpPostedFileBase file =  HttpContext.Current.Request["FileExcel"];

        //    HttpPostedFileBase file = Request.Files[0];
        //    string mappath = "/Upload/CallRecord/";
        //    string newFilename = DateTime.Now.ToString("yyMMddhhmmssffff") + file.FileName.Substring(file.FileName.LastIndexOf('.'));
        //    if (Directory.Exists(HttpContext.Current.Server.MapPath(mappath + DateTime.Now.ToString("yyyyMMdd"))) == false)
        //    {
        //        Directory.CreateDirectory(HttpContext.Current.Server.MapPath(mappath + DateTime.Now.ToString("yyyyMMdd")));
        //    }
        //    file.SaveAs(HttpContext.Current.Server.MapPath(mappath + DateTime.Now.ToString("yyyyMMdd") + "/") + newFilename);
        //    return mappath + DateTime.Now.ToString("yyyyMMdd") + "/" + newFilename;
        //}

    }
}
