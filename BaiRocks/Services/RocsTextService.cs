using BaiRocs.Common;
using BaiRocs.Models;
using BaiRocs.WF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BaiRocs.Services
{
   public class RocsTextService
    {
        //public static string GetVendorName(List<string> rawList)
        //{
        //    HashSet<string> candidates = new HashSet<string>();
        //    foreach(string line in rawList)
        //    {
        //        if(!string.IsNullOrEmpty(line))
        //           candidates.Add(line);
        //    }

        //    return "";

        //}

        public static void ElectOcrLineBySigmaDeferTotalTitle(ReceiptParts part)
        {
            var ocr = Global.OcrLines.First();
            if (!string.IsNullOrEmpty(ocr.ElectedAs))
                return;

            switch (part)
            {
                case ReceiptParts.VendorName:
                    ocr = Global.OcrLines.OrderByDescending(w => w.WeightedAsVendorName).First();
                    ocr.ElectedAs = ReceiptParts.VendorName.ToString();
                    break;

                case ReceiptParts.Address:
                    ocr = Global.OcrLines.OrderByDescending(w => w.WeightedAsAddress).First();
                    ocr.ElectedAs = ReceiptParts.Address.ToString();
                    break;

                case ReceiptParts.VendorTINTitle:
                    ocr = Global.OcrLines.OrderByDescending(w => w.WeightedAsVendorTINTitle).First();
                    ocr.ElectedAs = ReceiptParts.VendorTINTitle.ToString();
                    break;

                case ReceiptParts.DateTitle:
                    ocr = Global.OcrLines.OrderByDescending(w => w.WeightedAsDateTitle).First();
                    ocr.ElectedAs = ReceiptParts.DateTitle.ToString();
                    break;
                case ReceiptParts.DateSold:
                    break;
                case ReceiptParts.PriceTitle:
                    //defer this
                    //ocr = Global.OcrLines.OrderByDescending(w => w.WeightedAsTotalTitle).First();
                   // ocr.ElectedAs = ReceiptParts.PriceTitle.ToString();

                    break;
                case ReceiptParts.Price:
                    break;
                case ReceiptParts.AmountTenderTiTle:
                    ocr = Global.OcrLines.OrderByDescending(w => w.WeightedAsTenderTitle).First();
                    ocr.ElectedAs = part.ToString();//ReceiptParts.PriceTitle.ToString();
                    break;
                case ReceiptParts.AmoundTender:
                    break;
                case ReceiptParts.ChangeTitle:
                    ocr = Global.OcrLines.OrderByDescending(w => w.WeightedAsChangeTitle).First();
                    ocr.ElectedAs = part.ToString();//ReceiptParts.PriceTitle.ToString();

                    break;
                case ReceiptParts.Change:
                    break;

              

                default:
                    break;
            }

            Global.LogWarn("Elected: " + ocr.Content.ToString() + " ---->" + part.ToString());
        }

        public static void ElectOcrLineBySigmaWithTotalTitle(ReceiptParts part)
        {
            var ocr = Global.OcrLines.First();
            if (!string.IsNullOrEmpty(ocr.ElectedAs))
                return;

            switch (part)
            {
                case ReceiptParts.VendorName:
                    ocr = Global.OcrLines.OrderByDescending(w => w.WeightedAsVendorName).First();
                    ocr.ElectedAs = ReceiptParts.VendorName.ToString();
                    break;

                case ReceiptParts.Address:
                    ocr = Global.OcrLines.OrderByDescending(w => w.WeightedAsAddress).First();
                    ocr.ElectedAs = ReceiptParts.Address.ToString();
                    break;

                case ReceiptParts.VendorTINTitle:
                    ocr = Global.OcrLines.OrderByDescending(w => w.WeightedAsVendorTINTitle).First();
                    ocr.ElectedAs = ReceiptParts.VendorTINTitle.ToString();
                    break;

                case ReceiptParts.DateTitle:
                    ocr = Global.OcrLines.OrderByDescending(w => w.WeightedAsDateTitle).First();
                    ocr.ElectedAs = ReceiptParts.DateTitle.ToString();
                    break;
                case ReceiptParts.DateSold:
                    break;
                case ReceiptParts.PriceTitle:
                    ocr = Global.OcrLines.OrderByDescending(w => w.WeightedAsTotalTitle).First();
                    ocr.ElectedAs = ReceiptParts.PriceTitle.ToString();

                    break;
                case ReceiptParts.Price:
                    break;
                case ReceiptParts.AmountTenderTiTle:
                    ocr = Global.OcrLines.OrderByDescending(w => w.WeightedAsTenderTitle).First();
                    ocr.ElectedAs = part.ToString();//ReceiptParts.PriceTitle.ToString();
                    break;
                case ReceiptParts.AmoundTender:
                    break;
                case ReceiptParts.ChangeTitle:
                    ocr = Global.OcrLines.OrderByDescending(w => w.WeightedAsChangeTitle).First();
                    ocr.ElectedAs = part.ToString();//ReceiptParts.PriceTitle.ToString();

                    break;
                case ReceiptParts.Change:
                    break;



                default:
                    break;
            }

            Global.LogWarn("Elected: " + ocr.Content.ToString() + " ---->" + part.ToString());
        }


        public static string Refine(string content)
        {
            var split = content.Split(':');
            if(split.Length>1)
            {
              content =  content.Replace(split[0] + ":",string.Empty).Trim();
            }

            return content;
        }

        public static string RefineToMoney(string content)
        {
            var split = content.Split(':');
            string newContent = content;
            if (split.Length > 1)
            {
                newContent = content.Replace(split[0] + ":", string.Empty).Trim();
            }

            var result = string.Empty;
            foreach (char c in newContent)
            {
                if (char.IsNumber(c) || char.IsDigit(c) || c == '.' || c == ',')
                {
                    result += c;
                    int len = result.Length;
                    int index = (len - 2) - 1;
                    if (index > 1)
                    {
                        if (result[index] == '.')
                        {
                            return result;
                        }
                    }

                }
            }

            return result;
        }

        public static string RefineToTIN(string content)
        {
            string newContent = string.Empty;
            foreach(char c in content)
            {
                if(char.IsNumber(c)|| c=='-')
                {
                    newContent += c;
                }
            }
            return newContent;
        }
        public static string RefineToDate(string content)
        {
           content = content.ToLower().Replace("date", string.Empty);
           content = content.ToLower().Replace(":", string.Empty);
           string newContent = string.Empty;

            if (content.Contains('/'))
            {
                foreach (char c in content)
                {
                    if (char.IsNumber(c) || c == '/')
                    {
                        newContent += c;
                    }

                    // 01/12/2019
                    var year = DateTime.Now.Date.Year;
                    string strYear = year.ToString();
                    strYear = strYear.Substring(2, 2);

                    var length = newContent.Length;
                    if(length>=8)
                    {
                        if (newContent.EndsWith(strYear))
                            return newContent;
                    }
                }
            }
            else if (content.Contains('-'))
            {
                foreach (char c in content)
                {
                    if (char.IsNumber(c) || c == '-')
                    {
                        newContent += c;
                    }

                    // 01/12/2019
                    var year = DateTime.Now.Date.Year;
                    string strYear = year.ToString();
                    strYear = strYear.Substring(2, 2);

                    var length = newContent.Length;
                    if (length >= 8)
                    {
                        if (newContent.EndsWith(strYear))
                            return newContent;
                    }
                }
            }


            return newContent;
        }
        public static double GetPercentNumber(string Content)
        {
            var text = Content.Trim();

            text = text.Replace(" ", string.Empty);
            text = text.Replace("P", string.Empty);
            text = text.Replace("PHP", string.Empty);
            text = text.Replace(".", string.Empty);
            text = text.Replace(",", string.Empty);
            text = text.Replace("peso", string.Empty);


            var length = text.Length;
            int cnt = 0;
            foreach (var c in text)
            {
                if (Char.IsDigit(c) || char.IsNumber(c))
                    cnt += 1;
            }
            double percent = Convert.ToDouble(cnt) / Convert.ToDouble(length);
            return percent * 100;
        }

        public static double GetPercentLocation(BaiOcrLine ocr)
        {

            var totalCnt =Convert.ToDouble(Global.OcrLines.Count());
            var i = Convert.ToDouble(ocr.LineNo);
            double percent = i / totalCnt;
            return percent * 100;
        }


        public static void GetReceiptVendorName()
        {
            string vendor = string.Empty;
            vendor = Global.OcrLines.OrderByDescending(o => o.WeightedAsVendorName).First().Content;

            foreach (BaiOcrLine ocr in Global.OcrLines)
            {
                if(ocr.ElectedAs== ReceiptParts.VendorName.ToString())
                {
                    vendor = ocr.Content;
                }
            }

            Global.CurrentReciept.Comapany_Name = vendor;

        }
        public static void GetReceiptVendorAddress()
        {
            string address = string.Empty;
            address = Global.OcrLines.OrderByDescending(o => o.WeightedAsAddress).First().Content;

            foreach (BaiOcrLine ocr in Global.OcrLines)
            {
                if (ocr.ElectedAs == ReceiptParts.Address.ToString())
                {
                    address = ocr.Content;
                }
            }

            Global.CurrentReciept.Address = address;

        }




        public static void GetReceiptDateValue(BaiOcrLine ocr)
        {
            var newContent = RefineToDate (ocr.Content);
            var year = DateTime.Now.Date.Year;
            string strYear = year.ToString();
            strYear = strYear.Substring(2, 2);

            var length = newContent.Length;
            if (length >= 8)
            {
                if (newContent.EndsWith(strYear))
                {
                    Global.CurrentReciept.Date = newContent;
                }
            }
            else
            {
                int i = ocr.LineNo + 1;
                var nextLine =Global.OcrLines.Where(o => o.LineNo == i).FirstOrDefault();
                if (nextLine == null || nextLine.LineNo == 0)
                {
                   
                    Global.CurrentReciept.Date =RefineToDate(Global.OcrLines.OrderByDescending(o => o.WeightedAsDateTitle).First().Content);
                }
                else
                    GetReceiptDateValue(nextLine);

            };


        }
        public static void GetReceiptTINValue(BaiOcrLine ocr)
        {
            var newContent = RefineToTIN(ocr.Content);
           
            var length = newContent.Length;
            if (length > 5)
            {
               
                Global.CurrentReciept.Tax_Identification = newContent;
                return;
             
            }
            else
            {
                int i = ocr.LineNo + 1;
                var nextLine = Global.OcrLines.Where(o => o.LineNo == i).FirstOrDefault();
                if (nextLine ==null || nextLine.LineNo == 0)
                {

                    Global.CurrentReciept.Tax_Identification =RefineToTIN( Global.OcrLines.OrderByDescending(o => o.WeightedAsVendorTINTitle).First().Content);
                }
                else
                    GetReceiptTINValue(nextLine);

            };


        }
        public static void GetReceiptTotalValue(BaiOcrLine ocr)
        {   //do 1st level
            if(!GetReceiptTotalTopLevelOnly(ocr))            
            {
                //check next
                int i = ocr.LineNo + 1;
                var nextLine = Global.OcrLines.Where(o => o.LineNo == i).FirstOrDefault();
                if (nextLine == null || nextLine.LineNo == 0)
                {
                    Global.CurrentReciept.Amount =RefineToMoney( Global.OcrLines.OrderByDescending(o => o.WeightedAsTotalTitle).First().Content);
                    return;
                }
                else
                {
                    //do 2nd level
                    if (!GetReceiptTotalTopLevelOnly(nextLine))
                    {
                        //do by tender -change
                        var change = GetChangeValue();
                        var tender = GetTenderValue();
                        if(change.HasValue && tender.HasValue)
                        {
                           var amount = (tender.Value - change.Value);
                            if (amount > 0)
                            {
                                Global.CurrentReciept.Amount = amount.ToString();
                                return;
                            }

                        }
                    }
                }
                //if not found dig more...
                GetReceiptTotalValue(nextLine);

            };


        }

        public static bool GetReceiptTotalTopLevelOnly(BaiOcrLine ocr)
        {
            var newContent = RefineToMoney(ocr.Content);
            var length = newContent.Length;

            Regex moneyPattern = new Regex(@"[.\d,]+");

            bool isMatched = (moneyPattern.Match(newContent).Value.Length == length);

            if (((length > 4 && GetPercentNumber(newContent) > 70)
                || (length > 3 && GetPercentNumber(newContent) == 100)) && isMatched)
            {
                string result = string.Empty;
                foreach (char c in newContent)
                {
                    if (char.IsNumber(c) || char.IsDigit(c) || c == '.' || c == ',')
                    {
                        result += c;

                        int len = result.Length;
                        int index = (len - 2) - 1;
                        if (index > 1)
                        {
                            if (result[index] == '.')
                            {
                                Global.CurrentReciept.Amount = result;
                                return true;
                            }


                        }

                    }
                }
            }
            return false;
        }

        public static double? GetChangeValue()
        {
            double? result=null;
            var change = Global.OcrLines.Where(l => l.ElectedAs == ReceiptParts.ChangeTitle.ToString()).FirstOrDefault();
            if(change!=null && change.LineNo>0)
            {
                DigValue(change,ref result);
            }
            else
            {
                change = Global.OcrLines.OrderBy(l => l.WeightedAsChangeTitle).Last();
                DigValue(change, ref result);

            }
            return result;           
        }

        public static double? GetTenderValue()
        {
            double? result = null;
            var tender = Global.OcrLines.Where(l => l.ElectedAs == ReceiptParts.AmountTenderTiTle.ToString()).FirstOrDefault();
            if ( tender!=null && tender.LineNo > 0)
            {
                DigValue(tender, ref result);
            }
            else
            {
                tender = Global.OcrLines.OrderBy(l => l.WeightedAsTenderTitle).Last();
                DigValue(tender, ref result);

            }
            return result;

        }

        public static void DigValue(BaiOcrLine ocr, ref double? value)
        {
            var newContent = RefineToMoney(ocr.Content);
            var length = newContent.Length;

            Regex moneyPattern = new Regex(@"[.\d,]+");

            bool isMatched = (moneyPattern.Match(newContent).Value.Length == length);

            if (((length > 4 && GetPercentNumber(newContent) > 70)
                || (length > 3 && GetPercentNumber(newContent) == 100)) && isMatched)
            {
                try
                {
                    value = Convert.ToDouble(newContent);
                    return;
                }
                catch (Exception)
                {

                    //throw;
                    //continue search
                }
              
            }
           
           int i = ocr.LineNo + 1;
           var nextLine = Global.OcrLines.Where(o => o.LineNo == i).FirstOrDefault();
           if (nextLine == null || nextLine.LineNo == 0)
                {
                    value = null;
                    return;
                }
                else
                    DigValue(nextLine,ref value);           

        }
    }
}
