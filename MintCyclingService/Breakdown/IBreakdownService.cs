
using MintCyclingService.Photo;
using MintCyclingService.Utils;
using System;
using static MintCyclingService.Cycling.BaiduModel;

namespace MintCyclingService.Breakdown
{
    public interface IBreakdownService
    {
        #region 后台故障维护接口

        /// <summary>
        /// 根据查询条件搜索故障维护列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResultModel GetBreakDownList(GetBreakdownList_PM model);

        /// <summary>
        /// 查询故障维护详情
        /// </summary>
        /// <param name="BreakdownGuid"></param>
        /// <returns></returns>
        ResultModel GetBreakDownDetail(Guid BreakdownGuid);

        /// <summary>
        /// 新增或修改故障维护信息
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        ResultModel AddOrUpdateBreakDown(AddOrUpdateBreakdown_PM model);

        /// <summary>
        /// 导入Excel文件
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResultModel ImportExcelFileData(ImportExcelData_PM model);

        #endregion

        #region 客户端API故障维护接口

        /// <summary>
        /// 故障上报
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        ResultModel AddBreakDownLog(AddBreakdownLog_PM model);


        /// <summary>
        /// 新增照片
        /// </summary>
        /// <param name="title">照片标题</param>
        /// <param name="photoTypeEnum">照片类型</param>
        /// <param name="fileName">文件名</param>
        /// <param name="url">文件路径</param>
        /// <returns>新增照片</returns>
        ResultModel AddPhoto(string title, PhotoTypeEnum photoType, string fileName, string url,Guid breakdownPhotoGuid);

        /// <summary>
        /// 新增持工证照片照片
        /// </summary>
        /// <param name="title">照片标题</param>
        /// <param name="photoTypeEnum">照片类型</param>
        /// <param name="fileName">文件名</param>
        /// <param name="url">文件路径</param>
        /// <returns>新增照片</returns>
        ResultModel AddICBCPhoto(string title, PhotoTypeEnum photoType, string fileName, string url);

        ///// <summary>
        ///// 查询当前维修人员的维修已完成的记录信息
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //ResultModel GetBreakDownListByServicePersonID(GetBreakList_PM model);



        /// <summary>
        /// 查看自己管辖范围内故障车辆
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        ResultModel GetBreakdownBikeByUser(GetCarModel_PM data);

        /// <summary>
        /// 首次获取故障列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        ResultModel GetBreakDownList(BreakDownList_PM data);
        /// <summary>
        /// 根据条件获取故障车辆列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        //ResultModel GetBreakDownListBycondition(BreakDownCondition data);

        /// <summary>
        /// 维修单车编辑
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>

         ResultModel UpdateBreakDown(UpdateBreakDown_PM data);

        /// <summary>
        /// 维修记录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        ResultModel RepairRecord(RepairRecord_PM data);

        /// <summary>
        /// 故障上报
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        ResultModel BreakDownReport(UpdateBreakDown_PM data);
        /// <summary>
        /// 维修表单
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>

        ResultModel RepairForm(RepirtForm_PM data);

        /// <summary>
        /// 位置上报
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        ResultModel PositionReport(PositionReport_PM data);
        /// <summary>
        /// 根据经纬度获取地址
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <returns></returns>
        ResultModel GetAddressByLJ(LatLng data);

        /// <summary>
        /// 请求一分钟之内的通知
        /// </summary>
        /// <returns></returns>

        ResultModel Getnotification();

        /// <summary>
        /// 后台维修记录
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        ResultModel AdminRepairRecord(AdminRepairRecord_PM data);

        #endregion

    }
}