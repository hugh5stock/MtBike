using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MintCyclingService.Cycling
{
  
    public enum BicycleTypeNameEnum
    {
        /// <summary>   
        /// <summary>
        /// 0非助力车
        /// </summary>
        F_BicyCleTypeName,

        /// <summary>   
        /// 1助力车
        /// </summary>
        Z_BicyCleTypeName,
 

    }

    /// <summary>
    /// 车辆相关方法
    /// </summary>
    public class BicycleBaseInfoShow
    {
        private static string ZCKM = System.Web.Configuration.WebConfigurationManager.AppSettings["ZCKM"];      //助力车电量正常预计骑行公里数
        private static string CZKM = System.Web.Configuration.WebConfigurationManager.AppSettings["CZKM"];     //助力车电量充足预计骑行公里数

        /// <summary>
        /// 根据电量获取电量描述
        /// </summary>
        /// <param name="ElectricQuantity"></param>
        /// <returns></returns>
        public static BicElectricModel GetElectricQuantity(decimal ElectricQuantity)
        {
            //1-低电量：20 % 以下
            //2-电量正常：20 % -80 % 之间
            //3-电量充足：80 % 以上
            BicElectricModel model = new BicElectricModel();
            var ElectricQuantityDesc = string.Empty;

            if (ElectricQuantity < decimal.Parse(0.2.ToString()))
            {
                model.ElectricQuantityStatus = 1;
                model.ElectricQuantityDesc = "低电量:当前电量" + Convert.ToInt32(ElectricQuantity * 100) + "%";

            } else if (ElectricQuantity >=decimal.Parse(0.2.ToString()) && ElectricQuantity <=decimal.Parse(0.8.ToString()))
            {
                model.ElectricQuantityStatus = 2;
                model.ElectricQuantityDesc = "电量正常:当前电量" + Convert.ToInt32(ElectricQuantity * 100) + "% " +",预计助力骑行"+ ZCKM + "公里";
            }
            else if (ElectricQuantity>decimal.Parse(0.8.ToString()))
            {
                model.ElectricQuantityStatus = 3;
                model.ElectricQuantityDesc = "电量充足:当前电量" + Convert.ToInt32(ElectricQuantity * 100) + "% " + ",预计助力骑行" + CZKM + "公里";
            }
            return  model;
        }

        /// <summary>
        /// 车辆类型
        /// </summary>
        /// <param name="typeID"></param>
        /// <returns></returns>
        public static string GetBicycleTypeName(int? typeID)
        {
            var str = string.Empty;
            switch (typeID)
            {
                case (int)BicycleTypeNameEnum.F_BicyCleTypeName:
                    str = "非助力车";
                    break;

              
                case (int)BicycleTypeNameEnum.Z_BicyCleTypeName:
                    str = "助力车";
                    break;

                default:
                    str = "无法获取类型";
                    break;
            }
            return str;
        }

    }


 
    

 

}
