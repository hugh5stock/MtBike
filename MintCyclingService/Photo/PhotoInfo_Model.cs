using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MintCyclingService.Photo
{
    #region 后台实体模型


    /// <summary>
    /// 照片信息模型
    /// </summary>
    public class PhotoInfo_Model
    {
        /// <summary>
        /// 照片Guid
        /// </summary>
        public Guid PhotoGuid { get; set; }

        /// <summary>
        /// 照片路径
        /// </summary>
        public string Url { get; set; }
    }

    /// <summary>
    /// 新增的引导页图片
    /// </summary>
    public class NewGuidePagePhoto
    {
        public string Title { get; set; }
        public string AdverContent { get; set; }
        public string Url { get; set; }
        public int SortId { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid? AdminGuid { get; set; }
    }

    /// <summary>
    /// 引导页图片
    /// </summary>
    public class GuidePagePhotoData
    {
        public Guid PicGuid { get; set; }
        public string Title { get; set; }
        public string Pic { get; set; }
        public int Seq { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string AdminName { get; set; }
        public DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// 引导页图片删除参数
    /// </summary>
    public class GuidePagePhotoDeletion
    {
        public Guid PicGuid { get; set; }
        public Guid? AdminGuid { get; set; }
    }

    /// <summary>
    /// 引导页图片修改参数
    /// </summary>
    public class GuidePagePhotoModification
    {
        public Guid PicGuid { get; set; }
        public int Seq { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid? AdminGuid { get; set; }
    }


    #endregion

    /// <summary>
    /// APP引导页图片输出模型
    /// </summary>
    public class PhotoData_OM
    {
        public Guid PicGuid { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
   
    }


}
