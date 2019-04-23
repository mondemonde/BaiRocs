using MyCommonLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiRocs.Models
{
   public class WeightFactor:BaseModel
    {
        //see listOfDimension of MyReceipt
        public string Dimension { get; set; }

        //like date, tin, change
        public string keyWord { get; set; }
        public int Weight { get; set; }

    }
}
