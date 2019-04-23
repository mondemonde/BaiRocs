using BaiRocs.Common;
using BaiRocs.Models;
using BaiRocs.Policy;
using BaiRocs.Services;
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
    public sealed class DoFirstElect : NativeActivity
    {
        // Define an activity input argument of type string
        //[RequiredArgument]


        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(NativeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument

            try
            {
                #region --------------------TRY CONTENT----------------------  

                var sigmas = Global.CurrentSigma.GetSigmasDesc();

                foreach(var s in sigmas)
                {
                    if (s.Value >= 3)//3 sigma accuracy
                    {
                        if (Enum.TryParse(s.Key, out ReceiptParts part))
                           RocsTextService.ElectOcrLineBySigma(part);

                       
                    }
                }

                //context.SetValue(Result, Global.CurrentSigma);
                #endregion
            }
            catch (Exception err)
            {

                Global.LogError("Elect---> " + err.Message);

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
