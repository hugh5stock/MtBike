using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MintCyclingData
{
    public partial class MintBicycleDataContext
    {
        public MintBicycleDataContext() :
                base(ConfigurationManager.ConnectionStrings["MintBicycleConnectionString"].ConnectionString)
        {
            OnCreated();
        }
    }
}
