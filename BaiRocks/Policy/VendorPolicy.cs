using BaiRocs.Common;
using BaiRocs.DAL;
using BaiRocs.Models;
using BaiRocs.WF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiRocs.Policy
{
    public static class OcrPolicy
    {
        public static void AssertAsVendorName(ref BaiOcrLine ocrLine)
        {
            var dim = ReceiptParts.VendorName.ToString();
            using (MyDBContext db = new MyDBContext())
            {
                var wfactors = db.WeightFactors.ToList();
                var factors = wfactors.Where(f => f.Dimension == dim);

                foreach (var f in factors)
                {
                    if (ocrLine.Content.ToLower().Contains(f.keyWord.ToLower()))
                    {
                        ocrLine.WeightedAsVendorName += f.Weight;
                    }

                }

            }

            if (ocrLine.LineNo < 3)
                ocrLine.WeightedAsVendorName += 3;


            if (ocrLine.PercentNumber < 10)
            {
                ocrLine.WeightedAsVendorName += 3;
            }

            //add weight by line
            if (ocrLine.WeightedAsVendorName > 2)
            {
                var maxLine = Global.OcrLines.Count;
                var weightLine = maxLine - ocrLine.LineNo;
                ocrLine.WeightedAsVendorName += weightLine;
            }

        }

        public static void AssertAsVendorTINtitle(ref BaiOcrLine ocrLine)
        {
            var dim = ReceiptParts.VendorTINTitle.ToString();
            using (MyDBContext db = new MyDBContext())
            {
                var wfactors = db.WeightFactors.ToList();
                var factors = wfactors.Where(f => f.Dimension == dim);

                foreach (var f in factors)
                {
                    if (ocrLine.Content.ToLower().Contains(f.keyWord.ToLower()))
                    {
                        ocrLine.WeightedAsVendorTINTitle += f.Weight;
                    }

                }

            }


            if (ocrLine.LineNo >= 3 && ocrLine.LineNo <= 5)
                ocrLine.WeightedAsVendorTINTitle += 2;

        }

        public static void AssertAsDateTitle(ref BaiOcrLine ocrLine)
        {
            var dim = ReceiptParts.DateTitle.ToString();
            using (MyDBContext db = new MyDBContext())
            {
                var wfactors = db.WeightFactors.ToList();
                var factors = wfactors.Where(f => f.Dimension == dim);

                foreach (var f in factors)
                {
                    if (ocrLine.Content.ToLower().Contains(f.keyWord.ToLower()))
                    {
                        ocrLine.WeightedAsDateTitle += f.Weight;
                    }                 

                }

                //as observed of SI  entry
                if((!string.IsNullOrEmpty(ocrLine.Content) 
                    && ocrLine.Content.Trim() == "SI"))
                    ocrLine.WeightedAsDateTitle += 3;

            }          

            //do not for 11/11/11
            if (ocrLine.PercentNumber == 0)
            {
                ocrLine.WeightedAsDateTitle += 1;
            }
            if (ocrLine.Content.Contains("/"))
            {
                ocrLine.WeightedAsDateTitle -= 1;
            }

            var y = DateTime.Now.Year.ToString();
            if (ocrLine.Content.Contains(y))
            {
                ocrLine.WeightedAsDateTitle -= 2;
            }

            var y2 = y.Substring(2, 2);
            if (ocrLine.Content.Contains(y2))
            {
                ocrLine.WeightedAsDateTitle -= 1;
            }

            //add weight by line
            if (ocrLine.WeightedAsDateTitle > 2)
            {
                var maxLine = Global.OcrLines.Count;
                var weightLine = maxLine - ocrLine.LineNo;
                ocrLine.WeightedAsDateTitle += weightLine;
            }

        }

        public static void AssertAsAddress(ref BaiOcrLine ocrLine)
        {
            var dim = ReceiptParts.Address.ToString();
            using (MyDBContext db = new MyDBContext())
            {
                var wfactors = db.WeightFactors.ToList();
                var factors = wfactors.Where(f => f.Dimension == dim);

                foreach (var f in factors)
                {
                    if (ocrLine.Content.ToLower().Contains(f.keyWord.ToLower()))
                    {
                        ocrLine.WeightedAsAddress += f.Weight;
                    }

                }

            }

            //add weight by line
            if (ocrLine.WeightedAsAddress > 2)
            {
                var maxLine = Global.OcrLines.Count;
                var weightLine = maxLine - ocrLine.LineNo;
                ocrLine.WeightedAsAddress += weightLine;
            }

            if (ocrLine.LineNo < 3)
                ocrLine.WeightedAsAddress += 3;

            if (ocrLine.LineNo == 1)
                ocrLine.WeightedAsAddress += -3;



            if (ocrLine.PercentNumber < 25)
            {
                ocrLine.WeightedAsAddress += 3;
            }

            var split = ocrLine.Content.Split();
            if (split.Length == 1)
                ocrLine.WeightedAsAddress += -5;

            if (split.Length > 3)
                ocrLine.WeightedAsAddress += 2;

          


        }

        public static void AssertAsTotalTitle(ref BaiOcrLine ocrLine)
        {
            var dim = ReceiptParts.PriceTitle.ToString();
            using (MyDBContext db = new MyDBContext())
            {
                var wfactors = db.WeightFactors.ToList();
                var factors = wfactors.Where(f => f.Dimension == dim);

                foreach (var f in factors)
                {
                    if (ocrLine.Content.ToLower().Contains(f.keyWord.ToLower()))
                    {
                        ocrLine.WeightedAsTotalTitle += f.Weight;
                    }

                }

            }



        }




        #region Xtra Policy

        public static void AssertAsTenderTitle(ref BaiOcrLine ocrLine)
        {
            var dim = ReceiptParts.AmountTenderTiTle.ToString();
            using (MyDBContext db = new MyDBContext())
            {
                var wfactors = db.WeightFactors.ToList();
                var factors = wfactors.Where(f => f.Dimension == dim);

                foreach (var f in factors)
                {
                    if (ocrLine.Content.ToLower().Contains(f.keyWord.ToLower()))
                    {
                        ocrLine.WeightedAsTenderTitle += f.Weight;
                    }

                }
            }

            if (ocrLine.GetPercentLocation > 40 && ocrLine.GetPercentLocation < 70)
                ocrLine.WeightedAsTenderTitle += 2;



        }

        public static void AssertAsChangeTitle(ref BaiOcrLine ocrLine)
        {
            var dim = ReceiptParts.ChangeTitle.ToString();
            using (MyDBContext db = new MyDBContext())
            {
                var wfactors = db.WeightFactors.ToList();
                var factors = wfactors.Where(f => f.Dimension == dim);

                foreach (var f in factors)
                {
                    if (ocrLine.Content.ToLower().Contains(f.keyWord.ToLower()))
                    {
                        ocrLine.WeightedAsChangeTitle += f.Weight;
                    }

                }
            }

            if (ocrLine.GetPercentLocation > 40 && ocrLine.GetPercentLocation < 70)
                ocrLine.WeightedAsTenderTitle += 2;



        }


        //remove all elected first then do this policy
        public static void AssertAsDetail(ref BaiOcrLine ocrLine)
        {
            var dim = ReceiptParts.Detail.ToString();
            using (MyDBContext db = new MyDBContext())
            {
                var wfactors = db.WeightFactors.ToList();
                var factors = wfactors.Where(f => f.Dimension == dim);

                foreach (var f in factors)
                {
                    if (ocrLine.Content.ToLower().Contains(f.keyWord.ToLower()))
                    {
                        ocrLine.WeightedAsProduct += f.Weight;
                    }

                }

            }

            if (ocrLine.PercentNumber < 25)
            {
                ocrLine.WeightedAsAddress += 2;
            }

            if (ocrLine.Content.Contains(":"))
                ocrLine.WeightedAsAddress -= 1;
        }
        //no date word
        //no total word
        #endregion


    }
}
