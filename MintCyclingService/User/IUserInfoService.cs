using MintCyclingService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MintCyclingService.User
{
    /// <summary>
    /// 作者：TOM
    /// 2017-02-09
    /// 用户数据类
    /// </summary>
    public interface IUserInfoService
    {
        /// <summary>
        /// 新增或增加微信用户session
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        ResultModel AddOrUpdateMicroMsgUser(string code);

        /// <summary>
        /// 修改来自微信用户授权的信息
        /// </summary>
        /// <param name="userid">用户Guid</param>
        /// <param name="encryptedData">加密数据</param>
        /// <returns></returns>
        ResultModel EditMicroMsgUserInfo(Guid userid, string encryptedData, string iv);

        /// <summary>
        /// 查询个人行程
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        ResultModel GetUserTravelByUserGuid(UserTravel_PM para);

        /// <summary>
        /// 查询用户完成骑行的数据
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        ResultModel GetUserTravelEndByUserGuid(CyclingEnd_PM para);

        /// <summary>
        /// 查询个人中心用户信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        ResultModel GetUserInfoCenterByUserGuid(Guid UserGuid);

        /// <summary>
        /// 通过用户的Guid查看用户个人信息
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        ResultModel GetUserinfoByUserGuid(Guid UserGuid);

        /// <summary>
        /// 修改用户的手机号码
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        ResultModel EditUserPhoneOrNickNameByUserGuid(EditUserPhoneOrNickName_PM data);

        /// <summary>
        /// 用户认证
        /// </summary>
        /// <param name="UserGuid"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        ResultModel AddUserAuthentication(AddUserAuth_PM data);

        /// <summary>
        ///绑定充电宝
        /// </summary>
        /// <returns></returns>
        ResultModel BindPowerBank(BindPowerBank data);

        /// <summary>
        /// 查询用户Token是否存在
        /// </summary>
        /// <param name="utk"></param>
        /// <returns></returns>
        ResultModel GetUserTokenExist(Guid utk);
        
        /// <summary>
        /// 后台根据查询条件搜索用户列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResultModel GetUserInfoList(AdminUserInfo_PM model);
        
        /// <summary>
        /// 编辑用户信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResultModel EditUserInfoByUserGuid(EditUserInfo_PM model);
        
        /// <summary>
        /// 删除用户信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResultModel DeleteUserByUserGuid(DeleteUserInfo_PM model);
        
        /// <summary>
        /// 通过用户的Guid查询个人信息
        /// 作者：TOM
        /// 时间：2017-05-23
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        ResultModel GetUserByUserGuid(Guid UserGuid);
        
        /// <summary>
        /// 还车异常处理
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        ResultModel ReturnCarByPhone(ReturnCar_PM para);

        /// <summary>
        /// 锁定用户状态
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResultModel LockUserStatusByUserGuid(LockUserInfo_PM model);
        
        }
}
