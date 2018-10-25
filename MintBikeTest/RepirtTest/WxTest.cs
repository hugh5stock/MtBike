using Microsoft.VisualStudio.TestTools.UnitTesting;
using MintCyclingService.Breakdown;
using MintCyclingService.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MintBikeTest.RepirtTest
{
    [TestClass]
    public   class WxTest
    {
        /// <summary>
        /// 获取故障列表
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [TestMethod]
        public void GetBreakDownList()
        {
            var data = new BreakDownList_PM { TodayBreakDown = false, CityId = 73, DistinctId = 759, PageIndex = 0, PageSize = 10 };

             var res=  new  BreakdownService().GetBreakDownList(data);


            Assert.IsNotNull(res.ResObject);

        }











    }
}
