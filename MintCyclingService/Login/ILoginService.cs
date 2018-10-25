using MintCyclingService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MintCyclingService.Login
{
    public interface ILoginService
    {
        /// <summary>
        /// 新增短信验证记录
        /// </summary>
        /// <param name="phoneNo">手机号码</param>
        /// <param name="validateCode">验证码</param>
        /// <returns>新增短信验证记录</returns>
        ResultModel AddSmsValidateCode(string phoneNo, string validateCode);

        /// <summary>
        /// 搜索手机号与验证码匹配记录
        /// </summary>
        /// <param name="data">参数集合</param>
        /// <returns>手机号与验证码匹配记录是否存在</returns>
        ResultModel CheckPhoneNoAndValidateCode(ValidateLogin_PM data);

        /// <summary>
        /// 添加或者修改用户
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        ResultModel AddUserInfoOrUpdate(ValidateLogin_PM data);

        /// <summary>
        /// 是否已注册
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        ResultModel GetUserInfoByPhone(ValidateLogin_PM data);


        /// <summary>
        /// 添加用户过期信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        ResultModel AddOrUpdateCustomerAccessToken(Guid userInfoGuid);

        /// <summary>
        /// 登录成功后获取用户相关信息
        /// </summary>
        /// <param name="userInfoGuid"></param>
        /// <returns></returns>
        ResultModel GetLoginSuccessUserInfoByUserGuid(Guid userInfoGuid);



        /// <summary>
        /// 添加账户余额信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResultModel AddAccountUserInfo(UserAccount_PM model);

    }
}
