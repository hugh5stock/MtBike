namespace Utility.Common
{

    /// <summary>
    /// 结果代码枚举和对应消息
    /// </summary>
    public static class ResultCodeEnum
    {
        public static readonly string Success = "0";

        public static readonly string SystemError = "-1";

        public static readonly string UserNameExisted = "101";
        public static readonly string UserNameExistedMessage = "账号已注册!";

        public static readonly string EmailExisted = "102";
        public static readonly string EmailExistedMessage = "邮件地址已被注册!";

        public static readonly string PhoneExisted = "103";
        public static readonly string PhoneExistedMessage = "手机号码已存在!";

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

        public static readonly string AdminRoleNotExisted = "109";
        public static readonly string AdminRoleNotExistedMessage = "管理员角色不存在!";

        public static readonly string CanNotUploadEmptyFile = "110";
        public static readonly string CanNotUploadEmptyFileMessage = "不能上传空文件!";

        public static readonly string FileOperationFailed = "111";
        public static readonly string FileOperationFailedMessage = "文件操作失败!";

        public static readonly string SourceFileNotExisted = "112";
        public static readonly string SourceFileNotExistedMessage = "源文件不存在!";

        public static readonly string RecorderNotExisted = "113";
        public static readonly string RecorderNotExistedMessage = "记录不存在!";

        public static readonly string DBOperateFalied = "114";
        public static readonly string DBOperateFaliedMessage = "数据库操作失败!";

        public static readonly string AdminPasswordCanNotNull = "115";
        public static readonly string AdminPasswordCanNotNullMessage = "密码不能为空!";

        public static readonly string AdminOldPasswordError = "115001";
        public static readonly string AdminOldPasswordErrorMessage = "原密码不正确!";

        public static readonly string AdminRoleExisted = "116";
        public static readonly string AdminRoleExistedMessage = "管理员角色唯一标识或角色名已存在!";

        public static readonly string CustRoleExisted = "117";
        public static readonly string CustRoleExistedMessage = "会员角色唯一标识或角色名已存在!";

        public static readonly string AccessCodeExisted = "118001";
        public static readonly string AccessCodeExistedMessage = "Access code已存在!";

        public static readonly string AccessCodeNotExisted = "118002";
        public static readonly string AccessCodeNotExistedMessage = "Access code不存在!";

        public static readonly string AccessCodeExpired = "118003";
        public static readonly string AccessCodeExpiredMessage = "Access code已过期!";

        public static readonly string AccessCodeError = "118004";
        public static readonly string AccessCodeErrorMessage = "Access code错误!";

        public static readonly string UnixTimeExpired = "121";
        public static readonly string UnixTimeExpiredMessage = "时间戳已过期!";

        public static readonly string UnixTimeError = "122";
        public static readonly string UnixTimeErrorMessage = "时间戳错误!";

        public static readonly string SignError = "123";
        public static readonly string SignErrorMessage = "签名错误!";

        public static readonly string SignNotExisted = "124";
        public static readonly string SignNotExistedMessage = "没有签名!";

        public static readonly string TimestampNotExisted = "125";
        public static readonly string TimestampNotExistedMessage = "没有时间戳!";

        public static readonly string GuidExisted = "126";
        public static readonly string GuidExistedMessage = "唯一标识已存在!";

        public static readonly string GetCustomerError = "127";
        public static readonly string GetCustomerErrorMessage = "获取会员信息失败!";

        public static readonly string CourseNameExsited = "128";
        public static readonly string CourseNameExsitedMessage = "Course名已被使用!";

        public static readonly string LiveScopeError = "129";
        public static readonly string LiveScopeErrorMessage = "直播类型参数错误!";

        public static readonly string ScoreTypeError = "130";
        public static readonly string ScoreTypeErrorMessage = "记分类型错误!";

        public static readonly string AdminUserNameCanNotNull = "131";
        public static readonly string AdminUserNameCanNotNullMessage = "管理员名称不能为空!";

        public static readonly string AdminUserNameExsited = "132";
        public static readonly string AdminUserNameExsitedMessage = "管理员名称已存在!";

        public static readonly string UnsupportedMediaType = "133";
        public static readonly string UnsupportedMediaTypeMessage = "不支持的媒体类型!";

        public static readonly string PhotoNotExisted = "134";
        public static readonly string PhotoNotExistedMessage = "照片不存在!";

        public static readonly string OldPasswordError = "135";
        public static readonly string OldPasswordErrorMessage = "原密码错误!";

        public static readonly string ParaFormatError = "136";
        public static readonly string ParaFormatErrorMessage = "不能识别的参数格式!";

        public static readonly string ScoreCardNameExsited = "137";
        public static readonly string ScoreCardNameExsitedMessage = "记分卡名称已存在!";

        public static readonly string UnKnowCustomerStatus = "138";
        public static readonly string UnKnowCustomerStatusMessage = "未知的会员状态!";

        public static readonly string CustomerUnAuthentication = "139";
        public static readonly string CustomerUnAuthenticationMessage = "会员未被认证!";

        public static readonly string NickNameExsited = "139001";
        public static readonly string NickNameExsitedMessage = "会员昵称已存在!";

        public static readonly string ScoreCardNameCanNotNull = "140";
        public static readonly string ScoreCardNameCanNotNullMessage = "记分卡名称不能为空!";

        public static readonly string FriendAliasCanNotEmpty = "141";
        public static readonly string FriendAliasCanNotEmptyMessage = "好友别名不能为空!";

        public static readonly string SMSHaveNotReturnMessage = "142";
        public static readonly string SMSHaveNotReturnMessageMessage = "发送短信后无回应!";

        public static readonly string ScoreCardCanNotModify = "143";
        public static readonly string ScoreCardCanNotModifyMessage = "已启用的记分卡不能修改为未启用!";

        public static readonly string CourseCanNotModify = "144";
        public static readonly string CourseCanNotModifyMessage = "已启用的Course不能修改为未启用!";

        public static readonly string PlayerGuidError = "145";
        public static readonly string PlayerGuidErrorMessage = "球员标识错误!";

        public static readonly string ValidateCodeExpired = "146";
        public static readonly string ValidateCodeExpiredMessage = "验证码已过期!";

        public static readonly string UserMemberGuidError = "147";
        public static readonly string UserMemberGuidErrorMessage = "会员标识错误!";

        public static readonly string HoleNoExisted = "148";
        public static readonly string HoleNoExistedErrorMessage = "球洞编号已存在!";

        public static readonly string ClubNameExistedError = "149";
        public static readonly string ClubNameExistedErrorMessage = "球场名称已存在!";

        public static readonly string ClubEnglishNameExistedError = "150";
        public static readonly string ClubEnglishNameExistedErrorMessage = "球场英文名称已存在!";

        public static readonly string PhoneNoFormatError = "151";
        public static readonly string PhoneNoFormatErrorMessage = "手机号码错误!";

        public static readonly string CourseHoleCountNotEnough = "152";
        public static readonly string CourseHoleCountNotEnoughErrorMessage = "Course所包含的球洞应该有9个";

        public static readonly string CourseOrderNoExisted = "153";
        public static readonly string CourseOrderNoExistedErrorMessage = "球洞的Course序号已存在!";

        public static readonly string ScoreCardCourseExsited = "154";
        public static readonly string ScoreCardCourseExsitedMessage = "记分卡Course组合已存在!";

        public static readonly string ShareHasBeDeleted = "155";
        public static readonly string ShareHasBeDeletedMessage = "分享已删除!";

        public static readonly string MatchCanNotDelete = "156";
        public static readonly string MatchCanNotDeleteMessage = "已记录详细记分,记分卡不能被删除!";

        public static readonly string TeamAwardsNameExsited = "157";
        public static readonly string TeamAwardsNameExsitedMessage = "活动奖项名称已存在!";

        public static readonly string TeamEventNotExsit = "157001";
        public static readonly string TeamEventNotExsitMessage = "活动不存在!";

        public static readonly string TeamEventSignUpExsit = "157002";
        public static readonly string TeamEventSignUpExsitMessage = "当前活动已报名!";

        public static readonly string TeamEventSignUpPlayerNotExsit = "157003";
        public static readonly string TeamEventSignUpPlayerNotExsitMessage = "当前球员不存在!";

        public static readonly string TeamGuestExsit = "157004";
        public static readonly string TeamGuestExsitMessage = "当前球队嘉宾已存在!";

        public static readonly string EventSignUpTypeNotExsit = "157005";
        public static readonly string EventSignUpTypeNotExsitMessage = "未知的报名会员类型!";

        public static readonly string TeamNameExsit = "160";
        public static readonly string TeamNameExsitMessage = "球队名已存在!";

        public static readonly string TeamNameAuditExsit = "161";
        public static readonly string TeamNameAuditMessage = "球队审核中!";

        public static readonly string TeamNameRejectExsit = "162";
        public static readonly string TeamNameRejectMessage = "创建球队未通过审核!";


        public static readonly string  UserNoFriendy = "163";
        public static readonly string  UserNoFriendyMessage = "此用户没有好友!";

        public static readonly string TeamNoExist = "164";
        public static readonly string TeamNoExistMessage = "球队已被删除!";

        public static readonly string Verify = "165";
        public static readonly string VerifyMessage = "正在审核中..!";

        public static readonly string SubmitVerify = "166";
        public static readonly string SubmitVerifyMessage = "正在审核中..!";

        public static readonly string  NoTeamMember = "167";
        public static readonly string NoTeamMemberMessage = " 你还不是球队成员！请先确认身份！";


        public static readonly string  RejectJoinTeam = "168";
        public static readonly string RejectJoinTeamMessage = " 管理员拒绝了你的请求！";


        public static readonly string AgainSubmit = "169";
        public static readonly string AgainSubmitMessage = "请重新提交验证！";

        public static readonly string  NoExistProvince = "170";
        public static readonly string NoExistProvinceMessage = "数据库没有省份信息！";

        public static readonly string TestAccount = "171";
        public static readonly string TestAccountMessage = "测试账号登录成功!";
        public static readonly string  TeamNoEvent = "172";
        public static readonly string TeamNoEventMessage = "此球队暂无任何活动!";

        public static readonly string FollowerTeam = "173";
        public static readonly string FollowerTeamMessage = "你已关注，不能重复关注!";

        public static readonly string CancelFollowerTeam = "174";
        public static readonly string CancelFollowerTeamMessage = "你还没有关注，不能进行取消操作!";

        public static readonly string FollowerTeamParamerError = "175";
        public static readonly string FollowerTeamParamerErrorMessage = "关注不关注球队参数有误!";

        public static readonly string TeamNoFollower = "176";
        public static readonly string TeamNoFollowerMessage = "球队暂无粉丝!";

        public static readonly string TeamNoCoach = "177";
        public static readonly string TeamNoCoachMessage = "球队暂无教练!";

        public static readonly string TeamNoFriendlyTeam = "178";
        public static readonly string TeamNoFriendlyTeamMessage = "球队暂无友好球队!";

        public static readonly string  ReplacePlayerGuid = "179";
        public static readonly string ReplacePlayerGuidTeamMessage = "替换playerGuid失败!";

        public static readonly string SubmitVerifyTeamMemberRepe = "180";
        public static readonly string SubmitVerifyTeamMemberRepeMessage = "你已申请我是队员，不能重复申请!请等待审核..";


        public static readonly string ApplyFriendlyTeamRepe = "181";
        public static readonly string ApplyFriendlyTeamMessage = "你已申请成为友好球队，等待球队管理员同意...";



        public static readonly string MatchPlayerFull = "182";
        public static readonly string MatchPlayerFullMessage = "此活动分组成员已达到4人";


        public static readonly string MatchPlayerExist = "183";
        public static readonly string MatchPlayerExistMessage = "此成员已分组";

        public static readonly string MatchPlayerNOExist = "184";
        public static readonly string MatchPlayerNOExisMessage = "此活动分组不存在此成员";



        public static readonly string  EventInfomationNoExist = "185";
        public static readonly string EventInfomationNoExistMessage = "活动资讯已不存在";


        public static readonly string TeamMemberNoExist = "186";
        public static readonly string TeamMemberNoExistMessage = "球队成员不存在！";

        public static readonly string SetTeamMemberRole = "187";
        public static readonly string SetTeamMemberRoleMessage = "请指派一个队长，至少保证球队有一位队长！";

        public static readonly string SetTeamMemberRemove = "188";
        public static readonly string SetTeamMemberRemoveMessage ="自己不能把自己设置为离队";


        public static readonly string ApplyFriendlyExistTeamRepe = "189";
        public static readonly string ApplyFriendlyExistTeamMessage = "已经是友好球队";


        public static readonly string TeamNoExistTeamRepe = "190";
        public static readonly string TeamNoExistTeamRepeMessage = "球队不存在!";

        public static readonly string TeamMemberExistRepe = "191";
        public static readonly string TeamMemberExistRepeMessage = "成员已存在！";

        public static readonly string TeamMemberLeaveRepe = "192";
        public static readonly string TeamMemberLeaveRepeMessage = "此成员已离队！";


        public static readonly string TeamPassRepe = "193";
        public static readonly string TeampassRepeMessage = "球队已通过审核！";

        public static readonly string NoFinishExistEmptyMatchRepe = "194";
        public static readonly string NoFinishExistEmptyMatchRepeMessage= "不能结束分组，存在空分组请删除空白分组！";

        public static readonly string StartTeamEventFailed = "195";
        public static readonly string StartTeamEventFailedMessage = "球队活动开始失败！因为没有参加人员";

        public static readonly string StartTeamEventStarted = "196";
        public static readonly string StartTeamEventStartedMessage = "球队活动开始失败！因为曾经开始过";

        public static readonly string StartTeamEventNotHaveMarker = "197";
        public static readonly string StartTeamEventNotHaveMarkerMessage = "球队活动开始失败！因为不是所有的分组都有记分员";

        public static readonly string NoAuthorDeleteTeam= "198";
        public static readonly string NoAuthorDeleteTeamMessage = "你没有权限删除球队！";

        public static readonly string AuditingDeleteTeam = "199";
        public static readonly string AuditingDeleteTeamMessage = "球队正在审核，不能进行删除操作！";


        public static readonly string SignUpFull = "200";
        public static readonly string SignUpFullMessage = "报名人数已满！";


        public static readonly string AuitTeamMemberNoExist = "201";
        public static readonly string AuitTeamMemberNoExistMessage = "申请队员已同意或拒绝！";


        public static readonly string MatchTeamMemberNoExist = "202";
        public static readonly string MatchTeamMemberNoExistMessage = "所匹配队员已离队！";

        public static readonly string SignUpNoExist = "203";
        public static readonly string SignUpNoExistMessage = "此报名人员已被删除！";

        public static readonly string TeamEventNoExist = "204";
        public static readonly string TeamEventNoExistMessage = "此活动已被删除！";

        public static readonly string GuestingContextNotFitCode = "205";
        public static readonly string GuestingContextNotFitMessage = "自定义规则标题不可为空！";

        public static readonly string CreateGuessingFailCode = "206";
        public static readonly string CreateGuessingFailMessage = "发起竞猜活动失败！";

        public static readonly string TeamEventNoDeletesignUp = "207";
        public static readonly string TeamEventNoDeletesignUpMessage ="球队活动状态已开始或结束或取消，不能删除报名！";

        public static readonly string TeamEventNoAddMatch = "208";
        public static readonly string TeamEventNoAddMatchMessage = "球队活动状态已开始或结束或取消，不能新增分组！";


        public static readonly string MatchPlayerInExist = "209";
        public static readonly string MatchPlayerInExistMessage = "成员列表中存在已分组,新增失败";

        public static readonly string GuestingNoExistCode = "210";
        public static readonly string GuestingNoExistCodeMessage = "找不到竞猜信息！";

        public static readonly string AddGuestingFailCode = "211";
        public static readonly string AddGuestingFailCodeMessage = "未找到用户信息，创建竞猜人员表失败！";

        public static readonly string SetTeamSideFailCode = "212";
        public static readonly string SetTeamSideFailCodeMessage = "设定竞猜分组失败！根据条件未找到符合条件的记录！";

        public static readonly string NotExistedAvailableScoreCard = "213";
        public static readonly string NotExistedAvailableScoreCardMessage = "没有可用的记分卡！";


        public static readonly string AdminSetSignUpPaidFailCode = "214";
        public static readonly string AdminSetSignUpPaidFailCodeMessage = "未找到符合条件的记录，设置报名记录为已缴费失败！";


        public static readonly string AdminRemoveTeamRoleFailCode = "215";
        public static readonly string AdminSRemoveTeamRoleCodeMessage = "未找到符合条件的记录，删除角色失败！";

        public static readonly string GetEventSignUpListFailCode = "216";
        public static readonly string GetEventSignUpListFailCodeMessage = "未找到符合条件的报名球员信息记录！";

        public static readonly string CanNotDeleteSysRole = "217";
        public static readonly string CanNotDeleteSysRoleMessage = "系统角色不能删除！";

        public static readonly string TeamNotFoundFailCode = "218";
        public static readonly string TeamNotFoundFailCodeMessage = "指定的球队不存在！";


        public static readonly string SetTeamEventPlayerScore = "219";
        public static readonly string SetTeamEventPlayerScoreMessage = "修改球洞积分时数据错误！";

        public static readonly string NoExistScoreDetail = "220";
        public static readonly string NoExistScoreDetailMessage = "未找到积分卡的详细信息！";



        public static readonly string NotFoundCode = "221";
        public static readonly string NotFoundCodeMessage = "未找到符合条件的记录！";

        public static readonly string SetTeamEventAwardsPlayerOverFlowCode = "222";
        public static readonly string SetTeamEventAwardsPlayerOverFlowMessage = "超过该奖项数量！";

        public static readonly string RecordExitsCode = "223";
        public static readonly string RecordExitsCodeMessage = "该记录已存在，不能重复添加！";

        public static readonly string InivalidInviationCode = "224";
        public static readonly string InivalidInviationCodeMessage = "不可用的邀请码！";

        public static readonly string UserMemberHasRegisted = "225";
        public static readonly string UserMemberHasRegistedMessage = "此邀请码用户已经是注册用户,请用验证码登录！";

        public static readonly string PlayerHasUserMember = "226";
        public static readonly string PlayerHasUserMemberMessage = "此用户已有注册用户！";

        public static readonly string PlayerNotHasPhone = "227";
        public static readonly string PlayerNotHasPhoneMessage = "此用户无手机号码！";

        public static readonly string NotExistedAvailableTee = "228";
        public static readonly string NotExistedAvailableTeeMessage = "没有可用的Tee台！";

        public static readonly string ExistedRoleCode = "229";
        public static readonly string ExistedRoleCodeMessage = "该球队角色已存在，请修改角色名后重试！";

        public static readonly string PermissionNotSetCode = "230";
        public static readonly string PermissionNotSetCodeMessage = "尚为该角色设置任何权限！" ;


        public static readonly string TeamRoleNoexistCode = "231";
        public static readonly string TeamRoleNoexistCodeMessage = "球队角色已不存在！";

        public static readonly string TeamRoleNameexistCode = "232";
        public static readonly string TeamRoleNameexistCodeMessage = "球队角色名已存在！";

        public static readonly string PlayerExistedInRound = "233";
        public static readonly string PlayerExistedInRoundMessage = "球员已被分配到其他分组！";

        public static readonly string UpdateSocreFailCode = "234";
        public static readonly string UpdateSocreFailMessage = "{0}修改失败！";

        public static readonly string MatchBelongEventCode = "235";
        public static readonly string MatchBelongEventMessage = "比赛属于球队活动，不能修改日期";

        public static readonly string ClubGuidNotChanged = "236";
        public static readonly string ClubGuidNotChangedMessage = "球会GUID未改变";
    }

}
