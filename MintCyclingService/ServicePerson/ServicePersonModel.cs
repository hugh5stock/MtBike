using MintCyclingService.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MintCyclingService.ServicePerson
{
    public class LoginModel_PM
    {
        /// <summary>
        /// 账号
        /// </summary>
        public String UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord { get; set; }

        /// <summary>
        /// pushid
        /// </summary>
        public string PushId { get; set; }

    }
    public class LoginModel_OM
    {
        /// <summary>
        /// 真实名字
        /// </summary>

        public string RealName { get; set; }
        /// <summary>
        /// 用户编号
        /// </summary>
        public Guid UserNumber { get; set; }
        /// <summary>
        /// 省份名字
        /// </summary>
        public string ProvinceName { get; set; }
        /// <summary>
        /// 省份id
        /// </summary>
        public int ProvinceId { get; set; }
        /// <summary>
        /// 城市id
        /// </summary>
        public int CityId { get; set; }
        /// <summary>
        /// 城市名字
        /// </summary>
        public string CityName { get; set; }
        /// <summary>
        /// 镇区id
        /// </summary>
        public int DistinctId { get; set; }
        /// <summary>
        /// 镇区名字
        /// </summary>
        public string DistinctName { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        public Guid UserInfoGuid { get; set; }
        public Guid AccessCode { get; set; }
    }

    /// <summary>
    /// 维护人员输入参数模型
    /// </summary>
    public class ServicePerson_PM
    {
        /// <summary>
        /// 维护人员Guid
        /// </summary>
        public Guid? ServicePersonID { get; set; }

        /// <summary>
        /// 操作者Guid
        /// </summary>
        public Guid AdminGuid { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }


        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; }


        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }


        /// <summary>
        /// 性别
        /// </summary>
        public string Sex { get; set; }

        /// <summary>
        /// 省id
        /// </summary>
        public int ProvinceID { get; set; }
        /// <summary>
        /// 市id
        /// </summary>
        public int CityID { get; set; }
        /// <summary>
        /// 区县id
        /// </summary>
        public int DistinctId { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Remark { get; set; }


    }

    /// <summary>
    ///  维护人员列表查询条件输入参数
    /// </summary>
    public class SearchServicePerson_PM : Paging_Model
    {
        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 车锁编号
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        public int ProvinceId { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        public int CityId { get; set; }

        /// <summary>
        /// 区
        /// </summary>
        public int DistinctId { get; set; }

    }


    /// <summary>
    /// 维护人员列表输出参数模型
    /// </summary>
    public class ServicePersonList_OM
    {
        public int ProvinceID { get; set; }
        public int CityID { get; set; }
        public int DistrictID { get; set; }
        public string Remark { get; set; }

        /// <summary>
        /// 维护人员Guid
        /// </summary>
        public Guid ServicePersonID { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        public string RealName { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }


        /// <summary>
        /// 省
        /// </summary>
        public string ProvinceName { get; set; }
        /// <summary>
        /// 市
        /// </summary>
        public string CityName { get; set; }
        /// <summary>
        /// 区县
        /// </summary>
        public string DistinctName { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }



        public string Sex { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

    }


    /// <summary>
    /// 删除维护人员输入参数模型
    /// </summary>
    public class DeleteServicePerson_PM
    {
        /// <summary>
        /// 维护人员Guid
        /// </summary>
        public Guid ServicePersonID { get; set; }

        /// <summary>
        /// 操作者Guid
        /// </summary>
        public Guid AdminGuid { get; set; }

    }


}
