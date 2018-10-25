using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Utility.Common;
using System.Security.Cryptography;
using System.IO;

namespace Utility.Hash
{
    public class HashUtility
    {

        /// <summary>
        /// 对文本进行SHA1加密
        /// </summary>
        /// <param name="text">被加密的文本</param>
        /// <returns>SHA1加密后的文本</returns>
        public static string EncodeSHA1Data(string text)
        {
            byte[] data = Encoding.Default.GetBytes(text);//以字节方式存储
            SHA1 sha1 = new System.Security.Cryptography.SHA1CryptoServiceProvider();
            byte[] result = sha1.ComputeHash(data);//得到哈希值
            return BitConverter.ToString(result).Replace("-", ""); //转换成为字符串的显示
        }

        /// <summary>
        /// 对文本进行MD5加密
        /// </summary>
        /// <param name="text">被加密的文本</param>
        /// <returns>MD5加密后的文本</returns>
        public static string EncodeMd5Data(string text)
        {
            var encoding = Encoding.UTF8;

            var csp = new MD5CryptoServiceProvider();
            var result = csp.ComputeHash(encoding.GetBytes(text));
            var enText = new StringBuilder();

            foreach (var Byte in result)
            {
                enText.AppendFormat("{0:X2}", Byte);
            }

            return enText.ToString();
        }



        /// <summary>
        /// 产生键值对MD5编码的签名
        /// </summary>
        /// <param name="dict">键值对</param>
        /// <param name="secretKey">秘钥</param>
        /// <returns>返回键值对MD5编码的签名</returns>
        public static string GenerateDictMD5Sign(Dictionary<string, string> dict, string secretKey)
        {
            var strData = string.Empty;

            // 添加秘钥键值对
            dict.Add(Config.SecretKeyName, secretKey);

            // 键值对按Key的首字母排序
            dict = dict.OrderBy(p => p.Key).ToDictionary(p => p.Key, p => p.Value);

            var i = 0;
            foreach (var item in dict)
            {
                if (i != 0)
                {
                    strData += "&";
                }

                strData += item.Key + "=" + item.Value;
                i++;
            }

            var strSign = EncodeMd5Data(strData.ToLower());
            strSign = strSign.ToLower();

            return strSign;
        }

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="data">被加密的明文</param>
        /// <param name="key">16位密钥</param>
        /// <param name="vector">16位向量</param>
        /// <returns>密文</returns>
        public static string AESEncryptToBase64(string text, string key, string vector)
        {
            var data = Encoding.UTF8.GetBytes(text);

            Byte[] bKey = new Byte[16];
            Array.Copy(Encoding.UTF8.GetBytes(key.PadRight(bKey.Length)), bKey, bKey.Length);
            Byte[] bVector = new Byte[16];
            Array.Copy(Encoding.UTF8.GetBytes(vector.PadRight(bVector.Length)), bVector, bVector.Length);
            Byte[] Cryptograph = null; // 加密后的密文
            AesCryptoServiceProvider Aes = new AesCryptoServiceProvider();
            try
            {
                // 开辟一块内存流
                using (MemoryStream Memory = new MemoryStream())
                {
                    // 把内存流对象包装成加密流对象
                    using (CryptoStream Encryptor = new CryptoStream(Memory,
                     Aes.CreateEncryptor(bKey, bVector),
                     CryptoStreamMode.Write))
                    {
                        // 明文数据写入加密流
                        Encryptor.Write(data, 0, data.Length);
                        Encryptor.FlushFinalBlock();

                        Cryptograph = Memory.ToArray();
                    }
                }
            }
            catch
            {
                Cryptograph = null;
            }

            return Convert.ToBase64String(Cryptograph);
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="Data">被解密的密文</param>
        /// <param name="Key">16位密钥</param>
        /// <param name="Vector">16位向量</param>
        /// <returns>明文</returns>
        public static string AESDecryptFromBase64(string text, string Key, string Vector)
        {
            var data = Convert.FromBase64String(text);

            Byte[] bKey = new Byte[16];
            Array.Copy(Encoding.UTF8.GetBytes(Key.PadRight(bKey.Length)), bKey, bKey.Length);
            Byte[] bVector = new Byte[16];
            Array.Copy(Encoding.UTF8.GetBytes(Vector.PadRight(bVector.Length)), bVector, bVector.Length);

            Byte[] original = null; // 解密后的明文

            Rijndael Aes = Rijndael.Create();
            try
            {
                // 开辟一块内存流，存储密文
                using (MemoryStream Memory = new MemoryStream(data))
                {
                    // 把内存流对象包装成加密流对象
                    using (CryptoStream Decryptor = new CryptoStream(Memory,
                    Aes.CreateDecryptor(bKey, bVector),
                    CryptoStreamMode.Read))
                    {
                        // 明文存储区
                        using (MemoryStream originalMemory = new MemoryStream())
                        {
                            Byte[] Buffer = new Byte[1024];
                            Int32 readBytes = 0;
                            while ((readBytes = Decryptor.Read(Buffer, 0, Buffer.Length)) > 0)
                            {
                                originalMemory.Write(Buffer, 0, readBytes);
                            }

                            original = originalMemory.ToArray();
                        }
                    }
                }
            }
            catch
            {
                original = null;
            }

            return Encoding.UTF8.GetString(original);
        }


    }
}
