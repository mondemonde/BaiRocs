using BaiRocs.Common;
using BaiRocs.Models;
using BaiRocs.Services;
using BaiRocs.WF;
using LogApplication.Common;
using LogApplication.Common.Commands;
using System;
using System.Activities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiRocs.Commands
{
    public sealed class ScanImagesActivity : NativeActivity
    {
        // Define an activity input argument of type string
        [RequiredArgument]
        public InArgument<CmdParam> Param { get; set; }


        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(NativeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument

            try
            {
                #region --------------------TRY CONTENT----------------------
                //Global.IsScanBusy = true;
                Global.ProcessStatus = ProcessStatus.Scanning.ToString();
                BaiRocService azureSvc = new BaiRocService();
                azureSvc.OnReadDone += AzureSvc_OnReadDone;

                // Obtain the runtime value of the Text input argument
                CmdParam p = context.GetValue(this.Param);
                ScanImageCmdParam payload = (ScanImageCmdParam)p.Payload;

                var fname = payload.FileFullPath;
                azureSvc.ReadImage(fname);
                //context.SetValue(this.Result, azureSvc.RawList);             

                #endregion
            }
            catch (Exception err)
            {

                Global.LogError("ScanImage---> " + err.Message);
                Global.ProcessStatus = ProcessStatus.Ready.ToString();
            }
           


        }

        private void AzureSvc_OnReadDone(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            BaiRocService azureSvc = (BaiRocService)sender;
            Global.OcrLines = azureSvc.RawList;
            //bindingSourceOCR.DataSource = Global.OcrLines;
            //dgOCR.DataSource = bindingSourceOCR;
            Global.ProcessStatus = ProcessStatus.Ready.ToString();
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
