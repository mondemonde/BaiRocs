using BaiRocs.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiRocs.Models
{
    public class BaiOcrLine
    {
        public int LineNo { get; set; }
        public string Content { get; set; }
        public string ElectedAs { get; set; }

        public int WeightedAsVendorName { get; set; }
        public int WeightedAsDateTitle { get; set; }
        public int WeightedAsVendorTINTitle { get; set; }
        public int WeightedAsTotalTitle { get; set; }
        public int WeightedAsProduct { get; set; }
        public int WeightedAsAddress { get; set; }
        public double PercentNumber
        {
            get
            {
                return RocsTextService.GetPercentNumber(Content);

            }
        }

       // todos...
        public int WeightedAsTenderTitle { get; set; }
        public int WeightedAsChangeTitle { get; set; }
        public double GetPercentLocation
        {
            get
            {
                return RocsTextService.GetPercentLocation(this);

            }
        }


    }

}
