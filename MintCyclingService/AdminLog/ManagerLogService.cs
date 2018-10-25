using DTcms.Common;
using HuRongClub.DBUtility;
using MintCyclingData;
using MintCyclingService.Common;
using MintCyclingService.Cycling;
using MintCyclingService.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace MintCyclingService.AdminLog
{
    public class ManagerlogService : ImanagerlogService
    {
        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int AddManagerLog(dt_manager_log model)
        {
            using (var db = new MintBicycleDataContext())
            {
                var AdminQuery = db.Admin.FirstOrDefault(s => s.AdminGuid == model.AdminGuid && !s.IsDeleted);
                if (AdminQuery != null)
                {
                    model.UserName = AdminQuery.UserName;
                }
            }

            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into dt_manager_log(");
            strSql.Append("AdminGuid,UserName,action_type,remark,IP,CreateTime,IsDeleted)");
            strSql.Append(" values (");
            strSql.Append("@AdminGuid,@UserName,@action_type,@remark,@IP,@CreateTime,@IsDeleted)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
                    new SqlParameter("@AdminGuid", SqlDbType.UniqueIdentifier,4),
                    new SqlParameter("@UserName", SqlDbType.NVarChar,100),
                    new SqlParameter("@action_type", SqlDbType.NVarChar,100),
                    new SqlParameter("@remark", SqlDbType.NVarChar,4000),
                    new SqlParameter("@IP", SqlDbType.NVarChar,30),
                    new SqlParameter("@CreateTime", SqlDbType.DateTime),
                    new SqlParameter("@IsDeleted", SqlDbType.Bit)
            };
            parameters[0].Value = model.AdminGuid;
            parameters[1].Value = model.UserName;
            parameters[2].Value = model.action_type;
            parameters[3].Value = model.remark;
            parameters[4].Value = DTRequest.GetIP();
            parameters[5].Value = DateTime.Now;
            parameters[6].Value = false;

            object obj = DbHelperSQL.GetSingle(strSql.ToString(), parameters);
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(obj);
            }
        }

        /// <summary>
        /// 删除7天前的日志数据
        /// </summary>
        public int Delete(int dayCount)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from  dt_manager_log ");
            strSql.Append(" where DATEDIFF(day, add_time, getdate()) > " + dayCount);

            return DbHelperSQL.ExecuteSql(strSql.ToString());
        }

    }
}