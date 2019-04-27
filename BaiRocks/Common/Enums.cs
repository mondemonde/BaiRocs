using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiRocs.Common
{
    public enum ReceiptParts
    {
        VendorName,
        Address,
        VendorTIN,
        VendorTINTitle,
        DateTitle,
        DateSold,
        PriceTitle,
        Price,
        AmountTenderTiTle,
        AmoundTender,
        ChangeTitle,
        Change,
        Detail

    }

    public enum ProcessStatus
    {
        Ready,
        Searching,
        Scanning,
        Weighing,
        Sigma,
        Election,
        Error
    }
   
    public enum EnumCmd
    {
        ScanImage,
        ConsoleTest


    }
    public static class MyReceiptMetaData
    {
        public static Dictionary<string, int> NatureOfTnx { get; set; }

        //public static List<string> ListOfNatureOfTnx
        //{
        //    get
        //    {
        //        var list = new List<string>
        //        {
        //       "Parking" ,
        //         "Gasoline",
        //         "Transportation",
        //        "Hotel",
        //         "Meal",
        //        "Grocery",
        //        "Repairs", 
        //        "Telephone",
        //        "Wellness", 
        //        "Appliances",
        //        "Internet"
        //        };

        //        return list.OrderBy(s => s).ToList();
        //    }
        //}

        public static List<string> ListOfDimensions
        {
            get
            {
                var list = new List<string>
                {
                    "VendorName",
                    "Address",
                    "VendorTIN",
                    "VendorTINTitle",
                    "DateTitle",
                    "DateSold",
                    "PriceTitle",
                    "Price",
                    "AmountTenderTiTle",
                    "AmoundTender",
                    "ChangeTitle",
                    "Change",
                    "Detail"

                };

                return list.OrderBy(s => s).ToList();
            }
        }

    }

}
