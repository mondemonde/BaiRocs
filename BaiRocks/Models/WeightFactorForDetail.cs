using MyCommonLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiRocs.Models
{
   public class WeightFactorForDetail:BaseModel
    {
        //see listOfDimensionDetail of MyReceipt
        public string NatureOfTnx { get; set; }

        //like date, tin, change
        public string keyWord { get; set; }
        public int Weight { get; set; }
        public string Display { get; set; }



    }
}
