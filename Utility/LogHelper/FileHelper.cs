using System;
using System.IO;
using System.Web;

namespace Utility.LogHelper
{
    /// <summary>
    /// 文件日志
    /// Author:LiJun Time:2016/7/1 16:00:27
    /// Versions: 1.0.0.1
    /// </summary>
    public static class FileHelper
    {
        private static object error_Lock = new object();
        private static object info_Lock = new object();
        private static object debug_Lock = new object();
        private static object warn_Lock = new object();
        private static string vpath = HttpRuntime.AppDomainAppPath;

        public static int Debug(string message, Exception exception = null, int userid = 0, string platName = "", string module = "", string operating = "", string flgValue = "")
        {
            string filepath = vpath + "LogFile\\Debug\\" + DateTime.Now.ToString("yyyyMMdd") + "\\";
            string filename = "debug.log";
            int iWriteAppLogFile = 0;
            lock (debug_Lock)
            {
                iWriteAppLogFile = TP_WriteAppLogFile(filepath, filename, message, exception, userid, platName, module, operating, flgValue);
            }
            return iWriteAppLogFile;
        }

        public static int Warn(string message, Exception exception = null, int userid = 0, string platName = "", string module = "", string operating = "", string flgValue = "")
        {
            string filepath = vpath + "LogFile\\Warn\\" + DateTime.Now.ToString("yyyyMMdd") + "\\";
            string filename = "warn.log";
            int iWriteAppLogFile = 0;
            lock (warn_Lock)
            {
                iWriteAppLogFile = TP_WriteAppLogFile(filepath, filename, message, exception, userid, platName, module, operating, flgValue);
            }
            return iWriteAppLogFile;
        }

        public static int Info(string message, Exception exception = null, int userid = 0, string platName = "", string module = "", string operating = "", string flgValue = "")
        {
            string filepath = vpath + "LogFile\\Info\\" + DateTime.Now.ToString("yyyyMMdd") + "\\";
            string filename = "info.log";
            int iWriteAppLogFile = 0;
            lock (info_Lock)
            {
                iWriteAppLogFile = TP_WriteAppLogFile(filepath, filename, message, exception, userid, platName, module, operating, flgValue);
            }
            return iWriteAppLogFile;
        }

        public static int Error(string message, Exception exception, int userid = 0, string platName = "", string module = "", string operating = "", string flgValue = "")
        {
            string filepath = vpath + "LogFile\\Error\\" + DateTime.Now.ToString("yyyyMMdd") + "\\";
            string filename = "error.log";
            int iWriteAppLogFile = 0;
            lock (error_Lock)
            {
                iWriteAppLogFile = TP_WriteAppLogFile(filepath, filename, message, exception, userid, platName, module, operating, flgValue);
            }
            return iWriteAppLogFile;
        }

        private static int TP_WriteAppLogFile(string filepath, string filename, string message, Exception exception, int userid = 0, string platName = "", string module = "", string operating = "", string flgValue = "")
        {
            try
            {
                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }
                if (!File.Exists(filepath + filename))
                {
                    FileStream fs = null;
                    try
                    {
                        fs = File.Create(filepath + filename);
                        fs.Close();
                        fs.Dispose();
                    }
                    catch
                    {
                        fs.Close();
                        fs.Dispose();
                    }
                    finally
                    {
                        fs.Close();
                        fs.Dispose();
                    }
                }
                FileInfo file = new FileInfo(filepath + filename);

                if (file.Length > 5000 * 1000)
                {
                    string[] str = System.IO.Directory.GetFiles(file.DirectoryName);

                    file.MoveTo(filepath + filename.Substring(0, filename.IndexOf(".")) + "_" + str.Length + file.Extension);
                }
                using (FileStream fs = file.OpenWrite())
                {
                    StreamWriter sw = new StreamWriter(fs);
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sw.Write(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff  "));
                    sw.WriteLine(" platName:" + platName + "     module:" + module + "     operating:" + operating + "     flgValue:" + flgValue + "     message:" + message);
                    if (exception != null)
                    {
                        sw.WriteLine(" exception:" + exception);
                    }
                    sw.Flush();
                    sw.Close();
                    sw.Dispose();
                }
            }
            catch
            {
                return 0;
            }
            return 1;
        }
    }
}