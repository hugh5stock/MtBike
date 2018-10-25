using MintCyclingService.Utils;
using System;
using Utility.Common;

namespace MintCyclingService.Photo
{
    public interface IPhotoService
    {
        /// <summary>
        /// APP首页广告位查询
        /// </summary>
        /// <returns></returns>
        ResultModel GetPhotoUrlInfo();
 
        /// <summary>
        /// APP首页广告弹窗
        /// </summary>
        /// <returns></returns>
        ResultModel GetAdvertPhotoUrlWindow();

        /// <summary>
        /// 查询引导页图片数据
        /// </summary>
        /// <returns></returns>
        ResultModel GetGuidePagePhotoList();

        /// <summary>
        /// 删除引导页图片
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        ResultModel DeleteGuidePagePhoto(GuidePagePhotoDeletion para);

        /// <summary>
        /// 编辑引导页
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        ResultModel ModifyGuidePage(GuidePagePhotoModification para);

        /// <summary>
        /// 新增引导页的图片
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        ResultModel AddGuidePagePhoto(NewGuidePagePhoto para);

        /// <summary>
        /// 新增广告弹窗
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        ResultModel AddCommercialPhoto(NewGuidePagePhoto para);

        /// <summary>
        /// 查询广告弹窗数据
        /// </summary>
        /// <returns></returns>
        ResultModel GetCommercialList();

        /// <summary>
        /// 删除广告弹窗
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        ResultModel DeleteCommercial(GuidePagePhotoDeletion para);

        /// <summary>
        /// 编辑广告弹窗
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        ResultModel ModifyCommercial(GuidePagePhotoModification para);

        /// <summary>
        /// 根据图片Guid获取图片的完整Url
        /// </summary>
        /// <param name="photoGuid">图片Guid</param>
        /// <returns>完整Url</returns>
        ResultModel GetPhotoUrlByGuid(Guid photoGuid);

        /// <summary>
        /// 新增照片
        /// </summary>
        /// <param name="title">照片标题</param>
        /// <param name="photoTypeEnum">照片类型</param>
        /// <param name="fileName">文件名</param>
        /// <param name="url">文件路径</param>
        /// <returns>新增照片结果</returns>
        ResultModel AddOrUpdatePhoto(Guid PhotoGuid,string title, PhotoTypeEnum photoType, string fileName, string url);



        /// <summary>
        /// 新增照片
        /// </summary>
        /// <param name="title">照片标题</param>
        /// <param name="photoTypeEnum">照片类型</param>
        /// <param name="fileName">文件名</param>
        /// <param name="url">文件路径</param>
        /// <returns>新增照片结果</returns>
        ResultModel AddPhoto(string title, PhotoTypeEnum photoType, string fileName, string url);


        /// <summary>
        /// 修改图片
        /// </summary>
        /// <param name="title">照片标题</param>
        /// <param name="photoTypeEnum">照片类型</param>
        /// <param name="fileName">文件名</param>
        /// <param name="url">文件路径</param>
        /// <returns>修改图片</returns>
        ResultModel EditPhoto(Guid PhotoGuid, string title, PhotoTypeEnum photoType, string fileName, string url);


        ResultModel GetHttpGet();
       

    }
}
