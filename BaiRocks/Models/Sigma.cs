using BaiRocs.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiRocs.Models
{
    public class Sigma
    {
        public Sigma()
        {
            SigmaAddress = new List<double>();
            SigmaTotalTitle = new List<double>();
            SigmaDateTitle = new List<double>();
            SigmaVendorTINTitle = new List<double>();
            SigmaVendorName = new List<double>();
            SigmaTenderTitle = new List<double>();
            SigmaChangeTitle = new List<double>();
        }

        public List<double> SigmaVendorName { get; set; }
        public List<double> SigmaVendorTINTitle { get; set; }
        public List<double> SigmaTotalTitle { get; set; }
        public List<double> SigmaDateTitle { get; set; }
        public List<double> SigmaAddress { get; set; }


        public List<double> SigmaChangeTitle { get; set; }
        public List<double> SigmaTenderTitle { get; set; }


        public List<KeyValuePair<string, double>> GetSigmasDesc()
        {
            List<KeyValuePair<string, double>> sigmas = new List<KeyValuePair<string, double>>();

            sigmas.Add(new KeyValuePair<string, double>
     (
         ReceiptParts.Address.ToString(),
         SigmaAddress.StandardDeviation(SigmaAddress.Max())
     ));

            sigmas.Add(new KeyValuePair<string, double>
         (
             ReceiptParts.DateTitle.ToString(),
             SigmaDateTitle.StandardDeviation(SigmaDateTitle.Max())
         ));

            sigmas.Add(new KeyValuePair<string, double>
        (
            ReceiptParts.VendorTINTitle.ToString(),
            SigmaVendorTINTitle.StandardDeviation(SigmaVendorTINTitle.Max())
        ));


            sigmas.Add(new KeyValuePair<string, double>
            (
                ReceiptParts.VendorName.ToString(),
                SigmaVendorName.StandardDeviation(SigmaVendorName.Max())
            ));



            sigmas.Add(new KeyValuePair<string, double>
          (
              ReceiptParts.PriceTitle.ToString(),
              SigmaTotalTitle.StandardDeviation(SigmaTotalTitle.Max())
          ));


            sigmas.Add(new KeyValuePair<string, double>
      (
          ReceiptParts.ChangeTitle.ToString(),
          SigmaChangeTitle.StandardDeviation(SigmaChangeTitle.Max())
      ));

            sigmas.Add(new KeyValuePair<string, double>
      (
          ReceiptParts.AmountTenderTiTle.ToString(),
          SigmaTenderTitle.StandardDeviation(SigmaTenderTitle.Max())
      ));


            var sorted = sigmas.OrderByDescending(s => s.Value);
            return sorted.ToList();
        }

    }
}
