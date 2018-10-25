using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Utility.Common
{
    public class ExportExcelHelper
    {
        // 生成Excel导出地址
        public static string PadExcelUrl<T>(List<T> list, string strTag, string url, string str = "", string strCols = "")
        {
            if (list == null || !list.Any()) return string.Empty;
            var content = GetData(list, !string.IsNullOrEmpty(str) ? str : strTag);
            var css = ".firstTR td{width:100%;text-align:center;}.secondTR td{width:100%;text-align:center;}";
            content = string.Format("<meta http-equiv='content-type' content='application/ms-excel; charset=gb2312'/><style type='text/css'>{0}</style>{1}", css, string.IsNullOrEmpty(strCols) ? content : content + strCols);
            var date = DateTime.Now;
            var node = "SysDownload/" + date.Year.ToString() + date.Month.ToString().PadLeft(2, '0'); ;
            var dic = Path.Combine(HttpContext.Current.Server.MapPath("/"), node);
            if (!Directory.Exists(dic))
            {
                Directory.CreateDirectory(dic);
            }
            var filename = strTag + ".xlsx";
            var fileContents = Encoding.Default.GetBytes(content);
            var customPath = Path.Combine(dic, filename);
            var fileStream = new MemoryStream(fileContents);
            using (var fs = new FileStream(customPath, FileMode.Create, FileAccess.Write))
            {
                var tag = fileStream.ToArray();
                fs.Write(tag, 0, tag.Length);
                fs.Flush();
                tag = null;
            }
            var strUrl = string.Format("http://{0}/{1}/{2}", url, node, filename);
            return strUrl;
        }

        private static string GetData<T>(List<T> list, string strTag)
        {
            var strContent = string.Empty;
            if (list == null || !list.Any())
                return strContent;
            Type t = typeof(T);
            var att = t.GetProperties();
            var dict = new Dictionary<string, string>();
            foreach (var col in att)
            {
                var obj = Attribute.GetCustomAttribute(col, typeof(DisplayAttribute)) as DisplayAttribute;
                if (obj != null)
                {
                    dict.Add(col.Name, obj.Name);
                }
                else
                {
                    var ts = Attribute.GetCustomAttribute(col, typeof(TagResCols)) as TagResCols;
                    if (ts != null)
                    {
                        dict.Add(col.Name, ts.DisplayName);
                    }
                }
            }
            if (dict.Any())
            {
                var cols = dict.Count;
                var st = new StringBuilder();
                st.Append("<table border='1' cellspacing='0' cellpadding='0' borderColor='black'>");
                st.AppendFormat("<thead><tr><th colSpan='{0}' bgColor='#ccfefe'>{1}</th></tr>", cols, strTag);
                st.AppendFormat("<tr>{0}</tr></thead>", string.Join("", dict.Values.Select(k => "<th>" + k + "</th>").ToArray()));
                st.Append("<tbody>");
                var cnt = 0;
                list.ForEach(p =>
                {
                    cnt++;
                    var s = cnt % 2 == 0 ? "#CCCCCC" : "#FAF9DD";
                    st.AppendFormat("<tr class='{0}'>", cnt % 2 == 0 ? "firstTR" : "secondTR");
                    dict.Keys.ToList().ForEach(q =>
                    {
                        var data = TypeDescriptor.GetProperties(p)[q].GetValue(p);
                        st.AppendFormat("<td {0}bgColor='{1}'>{2}</td>", TagResCols.SetDataFormat(q, data), s, data ?? string.Empty);
                    });
                    st.Append(Environment.NewLine);
                    st.Append("</tr>");
                });
                st.Append("</tbody></table>");
                strContent = st.ToString();
            }
            return strContent;
        }
    }

    public class TagResCols : DisplayNameAttribute
    {
        public TagResCols(string resourceId)
            : base(GetMessageFromResource(resourceId))
        { }

        private static string GetMessageFromResource(string resourceId)
        {
            return TipReader(resourceId);
        }

        public static string TipReader(string resourceName)
        {
            var tag = "IngPAL";
            var assembly = Assembly.Load(tag);
            var st = GetCurCultureName();
            switch (st)
            {
                case "cn":
                case "zh":
                    st = "zh-CN";
                    break;
                case "en":
                    st = "en-US";
                    break;
                default:
                    st = "en-US";
                    break;
            }
            ResourceManager res = new ResourceManager(string.Format("{0}.App_GlobalResources.TipMessage_{1}", tag, st), assembly);
            var strValue = string.Empty;
            if (res.GetObject(resourceName) != null)
            {
                strValue = (string)res.GetObject(resourceName);
            }
            return strValue;
        }

        private static string GetCurCultureName()
        {
            var cultureName = HttpContext.Current.Request.Headers["Lang"];
            var currentUICulture = Thread.CurrentThread.CurrentUICulture;
            return string.IsNullOrEmpty(cultureName) ? currentUICulture.Name : cultureName;
        }

        public static string SetDataFormat(string tag, object obj)
        {
            var AMOUNTSTYLE = "style= 'vnd.ms-excel.numberformat:@;text-align:right'";
            var RATESTYLE = "style= 'vnd.ms-excel.numberformat:#,##0.0000000;text-align:right;'";

            var str = string.Empty;
            var amountType = new List<string> { "amount", "fee", "revenue", "cost", "exchangegainsloss", "payprofitloss", "balance" };
            if (tag.IndexOf("count", StringComparison.CurrentCultureIgnoreCase) > -1)
            {
                str = "style= 'text-align:right'";
            }
            else if (amountType.Any(s => tag.IndexOf(s, StringComparison.CurrentCultureIgnoreCase) > -1))
            {
                str = AMOUNTSTYLE;
            }
            else if (tag.IndexOf("rate", StringComparison.CurrentCultureIgnoreCase) > -1)
            {
                str = RATESTYLE;
            }
            else
            {
                str = "style= 'vnd.ms-excel.numberformat:@'";
            }
            return str;
        }
    }
}
