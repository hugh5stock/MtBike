using Utility.Common;
using System;
using System.IO;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;
using MintCyclingService.Utils;
using MintCyclingService.Common;

namespace Utility.File
{
    public class FileUtility
    {


        /// <summary>
        /// 上传图片文件
        /// </summary>
        /// <param name="Filedata">上传图片文件数据</param>
        /// <param name="uploadDir">上传图片文件目录,相对路径</param>
        /// <returns>返回上传图片文件结果</returns>
        public static ResultModel<UploadPicFileInfoModel> UploadPicFile(HttpPostedFileBase Filedata, string uploadDir)
        {
            var result = new ResultModel<UploadPicFileInfoModel>();

            // 如果没有上传文件
            if (Filedata == null || string.IsNullOrEmpty(Filedata.FileName) || Filedata.ContentLength == 0)
            {
                result.IsSuccess = false;
                result.MsgCode = ResPrompt.CanNotUploadEmptyFile;
                result.Message = ResPrompt.CanNotUploadEmptyFileMessage;
                return result;
            }

            // 获取原文件名
            var oriFileName = Path.GetFileName(Filedata.FileName);

            // 获取原文件扩展名
            var ext = Path.GetExtension(oriFileName);

            // 根据当前时间获取上传路径
            var now = DateTime.Now;

            var relativePathWithoutFileName = "\\" + uploadDir + "\\" + now.Year.ToString() + now.Month.ToString().PadLeft(2, '0');
            var dir = HttpContext.Current.Server.MapPath("~" + relativePathWithoutFileName);

            // 如不存在目录则创建目录
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            // 生成上传服务器后新的文件名
            var newfileName = Guid.NewGuid().ToString().Replace("-", string.Empty) + ext;

            // 获取上传服务器后完成的文件路径名
            var path = Path.Combine(dir, newfileName);
            var relativePath = Path.Combine(relativePathWithoutFileName, newfileName);

            // 保存文件到指定目录
            Filedata.SaveAs(path);

            var pic = Image.FromFile(path);

            var stream = Filedata.InputStream;
            var image = Image.FromStream(stream);

            result.IsSuccess = true;
            result.MsgCode = ResPrompt.Success;
            result.ResObject = new UploadPicFileInfoModel();
            result.ResObject.Extension = ext;
            result.ResObject.FullPath = path.Replace('\\','/');
            result.ResObject.RelativePath = relativePath.Replace('\\', '/');
            result.ResObject.Height = image.Height;
            result.ResObject.Width = image.Width;
            result.ResObject.NewFileName = newfileName;
            result.ResObject.OriFileName = oriFileName;
            result.ResObject.Size = stream.Length;
            result.ResObject.UploadDirectory = dir.Replace('\\', '/');

            image.Dispose();
            stream.Close();


            return result;
        }

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="maxWidth">生成的缩略图最大宽度</param>
        /// <param name="maxHeight">生成的缩略图最大高度</param>
        /// <param name="imageFile">图片文件</param>
        /// <param name="savePath">文件保存绝对路径</param>
        /// <param name="imgFormat">生成图片的格式</param>
        public void GenerateThumbnail(int maxWidth, int maxHeight, Image imageFile, string savePath, ImageFormat imgFormat)
        {
            var sourceWidth = imageFile.Width;
            var sourceHeight = imageFile.Height;

            var thumbWidth = sourceWidth; //要生成的缩略图的宽度
            var thumbHeight = sourceHeight; //要生成的缩略图的高度

            //计算生成图片的高度和宽度
            if (sourceWidth > maxWidth || sourceHeight > maxHeight)
            {
                var rateWidth = (double)sourceWidth / maxWidth;
                var rateHeight = (double)sourceHeight / maxHeight;

                if (rateWidth > rateHeight)
                {
                    thumbWidth = maxWidth;
                    thumbHeight = (int)Math.Round(sourceHeight / rateWidth);
                }
                else
                {
                    thumbWidth = (int)Math.Round(sourceWidth / rateHeight);
                    thumbHeight = maxHeight;
                }
            }

            using (var bmp = new Bitmap(thumbWidth, thumbHeight))
            {
                //从Bitmap创建一个System.Drawing.Graphics对象，用来绘制高质量的缩小图。
                using (var gr = Graphics.FromImage(bmp))
                {
                    //设置 System.Drawing.Graphics对象的SmoothingMode属性为HighQuality
                    gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    //下面这个也设成高质量
                    gr.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                    //下面这个设成High
                    gr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

                    //把原始图像绘制成上面所设置宽高的缩小图
                    var rectDestination = new Rectangle(0, 0, thumbWidth, thumbHeight);
                    gr.DrawImage(imageFile, rectDestination, 0, 0, sourceWidth, sourceHeight,
                                 GraphicsUnit.Pixel);
                    //保存图像
                    bmp.Save(savePath, imgFormat);
                }
            }
        }

        /// <summary>
        /// 文件移动
        /// </summary>
        /// <param name="sourceFullFileName">相对路径源文件名</param>
        /// <param name="destFullFileName">目标文件名</param>
        /// <returns>文件移动结果</returns>
        public ResultModel MoveFile(string sourceFullFileName, string destFullFileName)
        {
            var result = new ResultModel();

            var absPathSourceFileName = HttpContext.Current.Server.MapPath(sourceFullFileName);


            if (!System.IO.File.Exists(absPathSourceFileName))
            {
                result.IsSuccess = false;
                result.MsgCode = ResPrompt.SourceFileNotExisted;
                result.Message = ResPrompt.SourceFileNotExistedMessage;

                return result;
            }

            try
            {
                var absPathDestFullFileName = HttpContext.Current.Server.MapPath(destFullFileName);
                var destDir = Path.GetDirectoryName(absPathDestFullFileName);

                // 如不存在目标目录则创建目录
                if (!Directory.Exists(destDir))
                {
                    Directory.CreateDirectory(destDir);
                }

                System.IO.File.Copy(absPathSourceFileName, absPathDestFullFileName);
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.MsgCode = ResPrompt.FileOperationFailed;
                result.Message = ex.Message;

                return result;
            }

            return result;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fullPathFileName">删除文件的绝对路径</param>
        /// <returns>删除文件结果</returns>
        public ResultModel DeleteFile(string fullPathFileName)
        {
            var result = new ResultModel();

            if (System.IO.File.Exists(fullPathFileName))
            {
                try
                {
                    System.IO.File.Delete(fullPathFileName);
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.MsgCode = ResPrompt.FileOperationFailed;
                    result.Message = ex.Message;

                    return result;
                }

            }

            return result;
        }


        /// <summary>
        /// 根据扩展名获取对应的图片格式
        /// </summary>
        /// <param name="extension">扩展名,格式为(.bmp 或 .gif等)</param>
        /// <returns>返回图片格式</returns>
        public ImageFormat GetImageFormatByExtension(string extension)
        {
            ImageFormat result = null;
            extension = extension.ToLower();

            switch (extension)
            {
                case ".bmp":
                    result = ImageFormat.Bmp;
                    break;

                case ".gif":
                    result = ImageFormat.Gif;
                    break;

                case ".jpg":
                    result = ImageFormat.Jpeg;
                    break;

                case ".jpeg":
                    result = ImageFormat.Jpeg;
                    break;

                case ".png":
                    result = ImageFormat.Png;
                    break;
            }

            return result;
        }

        /// <summary>
        /// 根据相对路径获取完整Url
        /// </summary>
        /// <param name="relativePath">相对路径</param>
        /// <returns>完整Url</returns>
        public static string GetFullUrlByRelativePath(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
            {
                return string.Empty;
            }


            if (relativePath.StartsWith("/"))
            {
                relativePath = relativePath.Substring(1);
            }

            var uri3 = string.Empty;

            var uri1 = new Uri("http://" + HttpContext.Current.Request.Url.Host + ":" + HttpContext.Current.Request.Url.Port);
            var uri2 = new Uri(uri1, HttpContext.Current.Request.ApplicationPath);
            if (uri1.ToString() != uri2.ToString())
            {
                uri3 = uri2.ToString() + "/" + relativePath;
            }
            else
            {
                uri3 = uri2.ToString() + relativePath;
            }

            return  uri3;
        }
    }
}
