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
    public sealed class WeightDateSold : NativeActivity<BaiOcrLine>
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
                OcrPolicy.AssertAsDateTitle( ref ocr);
                context.SetValue(Result, ocr);

                #endregion
            }
            catch (Exception err)
            {

                Global.LogError("Weight DateSold---> " + err.Message);

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
