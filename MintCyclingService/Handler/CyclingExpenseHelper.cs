using MintCyclingService.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MintCyclingService.Handler
{
    public class CyclingExpenseHelper
    {
        CyclingCostModel para = null;
        delegate decimal SpendModeDelegate();

        public CyclingExpenseHelper(CyclingCostModel cyclingCost)
        {
            para = cyclingCost;
        }

        public decimal GetSpendAmount()
        {
            var s0 = SpendAmountProcessFunc(() => CyclingLessMinMode());
            return s0;
        }

        private decimal SpendAmountProcessFunc(SpendModeDelegate process)
        {
            return process == null ? 0m : process();
        }

        /// <summary>
        /// 骑行低于两分钟不扣费
        /// </summary>
        /// <returns></returns>
        private decimal CyclingLessMinMode()
        {
            if (para == null) return 0m;
            if (para.Tick.TotalMinutes <= 2d) return 0m;

            var ts = para.Tick.TotalHours;
            var t0 = Math.Floor(ts);
            var tq = ts - t0;
            //计算每小时骑行费用
            var hourAmount = t0 * para.Price;
            var minuteAmount = 0d;
            if (tq > 0)
            {
                //转换成分钟
                var min = tq * 60d;
                minuteAmount = min > 30 ? para.Price : para.Price * 1.0d / 2d;
            }
            return Convert.ToDecimal(hourAmount + minuteAmount);
        }
    }
}