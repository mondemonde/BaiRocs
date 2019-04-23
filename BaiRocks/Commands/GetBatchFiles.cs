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
    public sealed class GetBatchFiles : NativeActivity<string[]>
    {
        // Define an activity input argument of type string
        //[RequiredArgument]
         //public OutArgument<DirectoryInfo> result { get; set; }


        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(NativeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument

            try
            {
                #region --------------------TRY CONTENT----------------------            

                var rootDir = Global.Config.GetValue("RawImageFolder");
                //var dir = new DirectoryInfo(rootDir);
                //FileInfo[] files = dir.GetFiles().Take(10);
                var motherDir = new DirectoryInfo(rootDir);
                DirectoryInfo[] Folders = motherDir.GetDirectories("*",SearchOption.TopDirectoryOnly); //( Directory.getd(rootDir, "*.*", SearchOption.AllDirectories).Take(5);
                Global.CurrentFolder = Folders.OrderBy(f => f.LastAccessTime).FirstOrDefault();           
                Global.LogWarn("Global.CurrentFolder -->" +Global.CurrentFolder.Name);
                //var files = Global.CurrentFolder.GetFiles( "*.*", SearchOption.AllDirectories);
                var files = Directory.GetFiles(Global.CurrentFolder.FullName);

                context.SetValue(this.Result, files);
                //Global.MainWindow.InvokeOnUiThread(
                //    () => Global.MainWindow.DoTestSample(payload.SampleMessage));

                #endregion
            }
            catch (Exception err)
            {

                Global.LogError("GetBatchFile---> " + err.Message);

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
