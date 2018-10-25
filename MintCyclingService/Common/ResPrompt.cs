using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MintCyclingService.Common
{
    public static class ResPrompt
    {
        public static readonly string Success = "0";
        public static readonly string SystemError = "-1";

        #region  自定义错误信息
        //Upload File
        public static readonly string UnsupportedMediaType = "133";
        public static readonly string UnsupportedMediaTypeMessage = "不支持的媒体类型!";
        public static readonly string CanNotUploadEmptyFile = "110";
        public static readonly string CanNotUploadEmptyFileMessage = "不能上传空文件!";

        public static readonly string FileOperationFailed = "111";
        public static readonly string FileOperationFailedMessage = "文件操作失败!";

        public static readonly string SourceFileNotExisted = "112";
        public static readonly string SourceFileNotExistedMessage = "源文件不存在!";
        public static readonly string PhotoNotExisted = "134";
        public static readonly string PhotoNotExistedMessage = "照片不存在!";


        public static readonly string UserDepositError = "10100";
        public static readonly string UserDepositFormatErrorMessage = "此用户已充值押金!";



        //UserInfo message
        public static readonly string UserAuthFormatError = "8080";
        public static readonly string UserAuthFormatErrorMessage = "此用户已认证!";

        public static readonly string PasswordHashCodeError = "104";
        public static readonly string PasswordHashCodeErrorMessage = "密码输入错误!";

        public static readonly string UserNameNotExisted = "105";
        public static readonly string UserNameNotExistedMessage = "账号不存在或被冻结!";

        public static readonly string UserNameCanNotEmpty = "106";
        public static readonly string UserNameCanNotEmptyMessage = "用户名不能为空!";

        public static readonly string PasswordCanNotEmpty = "107";
        public static readonly string PasswordCanNotEmptyMessage = "密码不能为空!";

        public static readonly string ValiCodeNotMatch = "108";
        public static readonly string ValiCodeNotMatchMessage = "验证码不匹配!";


        public static readonly string ValiCodeInputError = "1200";
        public static readonly string ValiCodeInputErrorMessage = "验证码输入错误，请重新输入!";

        public static readonly string ValiCodeNotError = "1203";
        public static readonly string ValiCodeNotErrorMessage = "验证码已过期，请重新获取!";

        public static readonly string ValiCodeServiceError = "1202";
        public static readonly string ValiCodeServiceErrorMessage = "获取手机验证码异常!";


        public static readonly string UserinfoNotExist = "160";
        public static readonly string UserinfoNotExistMessage = "不存在用户信息!";

        public static readonly string TestAccount = "171";
        public static readonly string TestAccountMessage = "测试账号登录成功!";

        public static readonly string UserUserAuthNotExist = "172";
        public static readonly string UserUserAuthNotExistMessage = "您未认证，请先认证，才可以结束骑行!";

        public static readonly string wxUserinfoNotExist = "166";
        public static readonly string wxUserinfoNotExistMessage = "不存在维修人员信息!";

        public static readonly string UserPhoneNotExist = "0";
        public static readonly string UserPhoneNotExistMessage = "已存在相同的手机号码,您不能修改!";

        public static readonly string UsersNotExist = "8089";
        public static readonly string UsersNotExistMessage = "此用户已注册过，请重新注册!";


        public static readonly string UserinfoNotError = "4020";
        public static readonly string UserinfoNotRegisterMessage = "注册用户信息失败!";


        public static readonly string StopUserNotError = "85052";
        public static readonly string StartUserNotRegisterMessage = "此用户已被禁用，请联系客服！";

        public static readonly string CustomerAccessTokenNotError = "4025";
        public static readonly string CustomerAccessTokenMessage = "添加过期信息失败!";

        public static readonly string CustomerGuidError = "402001";
        public static readonly string CustomerGuidErrorMessage = "用户标识错误!";

        public static readonly string BreakDownGuidError = "80000";
        public static readonly string BreakDownGuidErrorMessage = "故障Guid不存在!";

        public static readonly string CustomerTravelNotExist = "403001";
        public static readonly string CustomerTravelNotExistMessage = "不存在行程记录!";

        public static readonly string CustomerTravelNotEndError = "403002";
        public static readonly string CustomerTravelNotEndErrorMessage = "当前行程没有结束时间!";

        public static readonly string CustomerTravelUpdateError = "403003";
        public static readonly string CustomerTravelUpdateErrorMessage = "结束当前行程出现异常!";

        public static readonly string UserAccountNotExist = "404001";
        public static readonly string UserAccountNotExistMessage = "不存在账户信息!";

        public static readonly string UserAccountBalanceError = "404002";
        public static readonly string UserAccountBalanceErrorMessage = "当前账户余额不足!";


        public static readonly string UserTokenNotEixist = "401001";
        public static readonly string UserTokenNotEixistMessage = "当前Token已失效!";

        public static readonly string UserTokenError = "401002";
        public static readonly string UserTokenErrorMessage = "当前Token不合法!";


        public static readonly string UserTokenCheckError = "401003";
        public static readonly string UserTokenCheckErrorMessage = "Token检查异常!";

        public static readonly string UserPermNotExist = "404003";
        public static readonly string UserPermNotExistMessage = "不存在此权限!";

        //bicycle message
        public static readonly string CloseLockExist = "1058";
        public static readonly string CloseLockExisttMessage = "不能上传相同的关锁交易记录!";

        public static readonly string OpenLockExist = "1059";
        public static readonly string OpenLockExisttMessage = "不能上传相同的开锁交易记录!";

        public static readonly string ParaModelNotExist = "1050";
        public static readonly string ParaModelNotExistMessage = "参数不能为空!";

        public static readonly string BicycleNotExist = "3001";
        public static readonly string BicycleNotExistMessage = "此单车不存在!";

        public static readonly string BicycleLockNotExist = "2020";
        public static readonly string BicycleLockNotExistMessage = "此单车和锁未做匹配，请重新选择预约用车!";

        public static readonly string BicycleYNotExist = "3006";
        public static readonly string BicycleYNotExistMessage = "此单车已存在，请重新添加入库!";

        public static readonly string BicycleVoltageNotExist = "3007";
        public static readonly string BicycleVoltageNotExistMessage = "此单车电量不足，暂时无法使用!";

        public static readonly string BicycleOpenNotExist = "3009";
        public static readonly string BicycleOpenNotExistMessage = "当前车锁在打开状态，暂时无法使用!";

        public static readonly string BicycleOrLockNotExist = "3010";
        public static readonly string BicycleOrLockNotExistMessage = "锁和车未做匹配，暂时无法使用!";

        public static readonly string BicycleReservationExist = "3002";
        public static readonly string BicycleReservationExistMessage = "此单车已被预约，您不能预约，请换一辆单车!";

        public static readonly string ReservationNotError = "45450";
        public static readonly string ReservationNotMessage = "当前单车锁已打开，您不能预约!";

        public static readonly string BicycleQiExist = "30801";
        public static readonly string BicycleQiExistMessage = "此单车正在骑行中，您不能开锁!";

        public static readonly string ReservaExist = "33888";
        public static readonly string ReservaExistMessage = "此单正在骑行中，您不能预约!";

        public static readonly string  ReservationExist = "30088";
        public static readonly string  ReservationExistMessage = "此单车已被预约,您不能开锁!";

        //repair
        public static readonly string NormalBikeCode = "3188";
        public static readonly string NormalBikeMessage = "此单车已经正常";


        public static readonly string BikeNoExistCode = "3189";
        public static readonly string BikeNoExistMessage = "单车不存在";

        public static readonly string NoRepairRecordCode = "3190";
        public static readonly string NoRepairRecordMessage = "暂无维修记录";

        public static readonly string NoUserTransationCode = "32020";
        public static readonly string NoUserTransationMessage = "暂无数据";

        public static readonly string ServicePersonCode = "60001";
        public static readonly string ServicePersonMessage = "已存在相同的用户名信息，请重新添加！";


        //Lock
        public static readonly string BicycleNumberError = "3001002";
        public static readonly string BicycleNumberErrorMessage = "已存在相同编号的锁!";

        public static readonly string BicycleUnlockError = "3001003";
        public static readonly string BicycleUnlockErrorMessage = "当前单车锁未打开!";

        public static readonly string UserUnlockError = "300900";
        public static readonly string UserUnlockErrorMessage = "此用户没有开过锁，不能关锁，请开锁!";

        public static readonly string OpenNotError = "3001020";
        public static readonly string OpenErrorMessage = "当前单车锁已打开，不能开锁!";

        public static readonly string CloseNotError = "3001021";
        public static readonly string CloseErrorMessage = "当前单车锁已关闭，不能关闭锁!";

        public static readonly string ReservationTimeOutNotError = "3001088";
        public static readonly string ReservationTimeOutMessage = "预约已超时，请重新预约!";

        public static readonly string UserReservatonNotError = "3001099";
        public static readonly string UserReservatonMessage = "您已预约过用车，不能直接开锁!";

        public static readonly string BicycleLockTypeError = "3001004";
        public static readonly string BicycleLockTypeErrorMessage = "未知的锁处理类型!";

        public static readonly string BicycleUnLockUserNotExist = "3001005";
        public static readonly string BicycleUnLockUserNotExistMessage = "锁操作的用户不存在!";



        //electronicfence message
        public static readonly string ElectronicfenceNotExist = "3002";
        public static readonly string ElectronicfenceNotExistMessage = "不存指定的电子围栏!";

        public static readonly string ElectronicfenceSeqExist = "3002001";
        public static readonly string ElectronicfenceSeqExistMessage = "已存在相同编号的电子围栏!";

        public static readonly string ElectronicfenceCoordsExist = "3002002";
        public static readonly string ElectronicfenceCoordsExistMessage = "已存在相同经纬度的电子围栏!";

        public static readonly string ElectronicfenceAddressExist = "3002003";
        public static readonly string ElectronicfenceAddressExistMessage = "已存在相同详细地址的电子围栏!";


        //admin
        public static readonly string AdminUserNameCanNotNull = "131";
        public static readonly string AdminUserNameCanNotNullMessage = "管理员名称不能为空!";

        public static readonly string AdminUserNameExsited = "132";
        public static readonly string AdminUserNameExsitedMessage = "管理员名称已存在!";

        public static readonly string AdminPasswordCanNotNull = "115";
        public static readonly string AdminPasswordCanNotNullMessage = "密码不能为空!";

        public static readonly string AdminOldPasswordError = "115001";
        public static readonly string AdminOldPasswordErrorMessage = "原密码不正确!";


        public static readonly string AccessCodeExisted = "118001";
        public static readonly string AccessCodeExistedMessage = "Access code已存在!";

        public static readonly string AccessCodeNotExisted = "118002";
        public static readonly string AccessCodeNotExistedMessage = "Access code不存在!";

        public static readonly string AccessCodeExpired = "118003";
        public static readonly string AccessCodeExpiredMessage = "Access code已过期!";

        public static readonly string AccessCodeError = "118004";
        public static readonly string AccessCodeErrorMessage = "Access code错误!";

        //role
        public static readonly string AdminRoleExisted = "116";
        public static readonly string AdminRoleExistedMessage = "管理员角色唯一标识或角色名已存在!";

        public static readonly string AdminRoleCanNotNull = "117";
        public static readonly string AdminRoleCanNotNullMessage = "角色名称不能为空!";



        //DB
        public static readonly string DBOperateFalied = "114";
        public static readonly string DBOperateFaliedMessage = "数据库操作失败!";


        //验证码
        public static readonly string ValidateCodeExpired = "146";
        public static readonly string ValidateCodeExpiredMessage = "验证码已过期!";

        public static readonly string UnixTimeExpired = "121";
        public static readonly string UnixTimeExpiredMessage = "时间戳已过期!";

        public static readonly string UnixTimeError = "122";
        public static readonly string UnixTimeErrorMessage = "时间戳错误!";

        //BreakdownLog-故障维护
        public static readonly string BreakDownNotExist = "40050";
        public static readonly string BreakDownNotExistMessage = "不存故障维护信息!";



        //SMS
        public static readonly string SMSHaveNotReturnMessage = "142";
        public static readonly string SMSHaveNotReturnMessageMessage = "发送短信后无回应!";

        public static readonly string PhoneNoFormatError = "151";
        public static readonly string PhoneNoFormatErrorMessage = "手机号码错误!";

        //供应商
        public static readonly string SupplierError = "5050";
        public static readonly string SupplierErrorMessage = "已存在相同的供应商信息，请重新添加!";

        //客户
        public static readonly string CustomerError = "5051";
        public static readonly string CustomerErrorMessage = "已存在相同的客户信息，请重新添加!";

        public static readonly string CustomerBicLockError = "5052";
        public static readonly string CustomerBicLockErrorMessage = "已存在分配相同的车锁信息，请重新分配!";


        public static readonly string ChoiceNoError = "5053";
        public static readonly string ChoiceNoErrorMessage = "您未选择要分配的车锁，请选择!";


        public static readonly string ChoiceExNoError = "5054";
        public static readonly string ChoiceExNoErrorMessage = "查询客户车锁分配信息异常！";

        #endregion

        #region 自定义调用支付宝和订单错误信息


        public static readonly string AlipayRSACheckV1ErrorCode = "1011";
        public static readonly string AlipayRSACheckV1ErrorMessageMessage = "支付宝验签失败，请检查对应的接口调用！";

        public static readonly string AlipayOut_trade_noErrorCode = "1012";
        public static readonly string AlipayOut_trade_noErrorMessageMessage = "订单号不存在！";

        public static readonly string AlipayAmountNotErrorCode = "1013";
        public static readonly string AlipayAmountNotErrorMessageMessage = "订单金额和支付金额不相符！";

        public static readonly string AlipayServerErrorCode = "1014";
        public static readonly string AlipayServerErrorMessageMessage = "验签过程出现异常！";

        public static readonly string AlipayCloseErrorCode = "1015";
        public static readonly string AlipayColseErrorMessageMessage = "交易关闭！";

        public static readonly string AlipayFailGErrorCode = "0";
        public static readonly string AlipayFailErrorMessageMessage = "充值失败！";


        public static readonly string AlipayExistErrorCode = "1017";
        public static readonly string AlipayExistErrorMessageMessage = "此用户已充值了押金！";


        public static readonly string AlipayDepositOrderErrorCode = "1018";
        public static readonly string AlipayDepositOrderErrorMessageMessage = "生成押金充值订单信息异常！";

        public static readonly string AlipayAccountOrderErrorCode = "1010";
        public static readonly string AlipayAccountOrdeErrorMessageMessage = "生成账户余额充值订单信息异常！";


        public static readonly string AlipayServiceErrorCode = "1019";
        public static readonly string AlipayServiceErrorMessageMessage = "服务器生成订单信息异常！";


        #endregion


    }
}