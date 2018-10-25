using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Utility
{
    public class SignUtility
    {
        public static string GetMd5Data(string strData)
        {
            var encoding = Encoding.UTF8;

            var csp = new MD5CryptoServiceProvider();
            var result = csp.ComputeHash(encoding.GetBytes(strData));
            var enText = new StringBuilder();

            foreach (var Byte in result)
            {
                enText.AppendFormat("{0:X2}", Byte);
            }

            return enText.ToString();
        }

        public static string EncodeSHA1Data(string text)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] result = sha1.ComputeHash(data);
            var str = new StringBuilder();
            foreach (var kt in result)
            {
                str.AppendFormat("{0:X2}", kt);
            }
            return str.ToString();
        }

        public static long DateTimeToUNIXTime(DateTime dt)
        {
            TimeSpan ts = dt.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }
    }
}