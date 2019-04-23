using BaiRocs.Models;
using BaiRocs.Policy;
using BaiRocs.WF;
using System;
using System.Activities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiRocs.Commands
{
    public sealed class AddSigma : NativeActivity<Sigma>
    {
        // Define an activity input argument of type string
        //[RequiredArgument]
        public InArgument<BaiOcrLine> OcrLine { get; set; }


        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(NativeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument

            try
            {
                #region --------------------TRY CONTENT----------------------  
                var ocr = context.GetValue(OcrLine);
                Global.CurrentSigma.SigmaAddress.Add(Convert.ToDouble(ocr.WeightedAsAddress));
                Global.CurrentSigma.SigmaDateTitle.Add(Convert.ToDouble(ocr.WeightedAsDateTitle));
                Global.CurrentSigma.SigmaTotalTitle.Add(Convert.ToDouble(ocr.WeightedAsTotalTitle));
                Global.CurrentSigma.SigmaVendorName.Add(Convert.ToDouble(ocr.WeightedAsVendorName));
                Global.CurrentSigma.SigmaVendorTINTitle.Add(Convert.ToDouble(ocr.WeightedAsVendorTINTitle));

                context.SetValue(Result, Global.CurrentSigma);
                #endregion
            }
            catch (Exception err)
            {
                Global.LogError("AddSigma---> " + err.Message);

            }
            finally
            {
                // Global.MainWindow.SafeInvoke(c => c.HideBusy());

            }


        }

        void OnReadComplete(NativeActivityContext context, Bookmark bookmark, object state)
        {
            //
            //string bname = context.GetValue(this.BookmarkName);
            //context.SetValue(this.FileCount, input0.CommandName);
            //var input = state as string;
            //context.SetValue(this.Result, input);

        }

    }
}
