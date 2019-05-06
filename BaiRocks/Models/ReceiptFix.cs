using MyCommonLib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiRocs.Models
{
    public class ReceiptFix : BaseModel
    {
         public int ReceiptId { get; set; }
        //public string UserName { get; set; }
        public string Date { get; set; }
        public string Comapany_Name { get; set; }
        public string Tax_Identification { get; set; }
        public string Address { get; set; }
        public string Description { get; set; }
        public string Receipt_Attached { get; set; }
        public string Receipt_Name { get; set; }
        public string BusinessReason { get; set; }
        public string NameOfIndividual { get; set; }
        public string Project { get; set; }
        public string Amount { get; set; }



    }
}
