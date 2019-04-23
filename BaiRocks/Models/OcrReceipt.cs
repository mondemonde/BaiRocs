using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiRocs.Models
{
   public class OcrReceipt
    {
        public BaiOcrLine DateSold { get; set; }
        public BaiOcrLine TotalAmount { get; set; }
        public BaiOcrLine VendorName { get; set; }
        public BaiOcrLine VendorTIN { get; set; }
        public BaiOcrLine Details { get; set; }


    }
}
