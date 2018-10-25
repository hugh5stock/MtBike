using MintCyclingService.Utils;
using Utility.Common;

namespace Utility.File
{
    /// <summary>
    /// 上传文件信息模型
    /// </summary>
    public class UploadFileInfoModel
    {
        /// <summary>
        /// 原文件名
        /// </summary>
        public string OriFileName { get; set; }

        /// <summary>
        /// 原文件扩展名
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// 上传文件所在目录
        /// </summary>
        public string UploadDirectory { get; set; }

        /// <summary>
        /// 上传服务器后新的文件名
        /// </summary>
        public string NewFileName { get; set; }

        /// <summary>
        /// 上传服务器后完成的图片文件路径名
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// 上传服务器后完成的图片文件相对路径名
        /// </summary>
        public string RelativePath { get; set; }
    }

    /// <summary>
    /// 上传图片文件信息模型
    /// </summary>
    public class UploadPicFileInfoModel : UploadFileInfoModel
    {
        /// <summary>
        /// 上传图片的宽度
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 上传图片的高度
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// 上传图片的大小
        /// </summary>
        public long Size { get; set; }
    }

    /// <summary>
    /// FineUploader的输出模型
    /// </summary>
    public class FineUploaderResultModel : ResultModel
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool success { get; set; }

        /// <summary>
        /// 出错信息
        /// </summary>
        public string error { get; set; }

    }
}
