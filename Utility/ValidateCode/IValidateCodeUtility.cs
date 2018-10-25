namespace Utility.ValidateCode
{
    public interface IValidateCodeUtility
    {
        /// <summary>
        /// 生成指定长度的随机验证码
        /// </summary>
        /// <param name="codeLen">验证码长度</param>
        /// <returns>验证码</returns>
        string CreateVerifyCode(int codeLen);

        /// <summary>
        /// 已字节的方式输出图片
        /// </summary>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        byte[] CreateImageToByte(string code);
    }
}
