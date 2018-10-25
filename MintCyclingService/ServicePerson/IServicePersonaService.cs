using MintCyclingService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MintCyclingService.ServicePerson
{
 public   interface IServicePersonaService
    {

        /// <summary>
        /// 维护人员登录(维护APP端)
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        ResultModel Login(LoginModel_PM data);

        /// <summary>
        /// 维修人员列表(后台接口)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResultModel GetServicePersonInfList(SearchServicePerson_PM model);

        /// <summary>
        /// 维修人员添加和修改(后台接口)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResultModel AddServicePersonInfo(ServicePerson_PM model);


        /// <summary>
        ///删除维护人员信息 
        /// <returns></returns
        ResultModel DeleteServicePersonByServiceGuid(DeleteServicePerson_PM model);


        /// <summary>
        /// 修改pushid
        /// </summary>
        /// <param name="userGuid"></param>
        /// <param name="pushid"></param>
        /// <returns></returns>
        bool SetpushIdByUserGuid(Guid userGuid, string pushid);

        }
}
