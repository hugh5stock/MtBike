using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Utility.Common
{
    /// <summary>
    /// 记录文件日志
    /// 2017-03-13
    /// </summary>
   public class FileLog
    {

        /// <summary>
        /// 写日志，方便测试（看网站需求，也可以改成把记录存入数据库）
        /// </summary>
        /// <param name="sWord">要写入日志里的文本内容</param>
        public static void LogResult(string sWord)
        {
            string strPath = HttpContext.Current.Server.MapPath("log");
            strPath = strPath + "\\" + DateTime.Now.ToString().Replace(":", "") + ".txt";
            StreamWriter fs = new StreamWriter(strPath, false, System.Text.Encoding.Default);
            fs.Write(sWord);
            fs.Close();
        }

        /// <summary>
        /// 日志记录
        /// </summary>
        /// <param name="msg"></param>
        public static void Log(string msg,string typeName)
        {
            //string filename = DateTime.Now.ToString("yyyyMMdd") + ".txt";

            //System.IO.File.AppendAllText(HttpContext.Current.Server.MapPath("~/FileLog/"+ filename), msg);
            // 根据当前时间获取上传路径
            var now = DateTime.Now;
            string filename = DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";

            var UrlFile = "~/FileLog/" + typeName + "/";
            var Urls = HttpContext.Current.Server.MapPath(UrlFile);

            // 如不存在目录则创建目录
            if (!Directory.Exists(Urls))
            {
                Directory.CreateDirectory(Urls);
            }
            System.IO.File.AppendAllText(Urls+filename, msg);
        }


        public void Logs(string ex)
        {
            try
            {
                string filename = DateTime.Now.ToString("yyyyMMdd") + ".txt";
                FileInfo file = new FileInfo(System.AppDomain.CurrentDomain.BaseDirectory + filename); //如果是web程序，这个的变成Http什么的
                StreamWriter sw = null;
                if (!file.Exists)
                {
                    sw = file.CreateText();
                    sw.WriteLine(ex.ToString());
                }
                else
                {
                    sw = file.AppendText();
                    sw.WriteLine(ex.ToString());
                }
                sw.Close();
                sw.Flush();
                sw.Dispose();
            }
            catch (Exception e)
            {
                
            }
        }


    }
}
