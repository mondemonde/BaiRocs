using BaiRocs.WF;
using LogApplication.Common;
using System;
using System.Activities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiRocs.Commands
{
    public sealed class FilterForImage : NativeActivity<bool>
    {
        // Define an activity input argument of type string
        //[RequiredArgument]
         public InArgument<string> FilePath { get; set; }


        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(NativeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument

            try
            {
                #region --------------------TRY CONTENT----------------------
                //Global.IsScanBusy = true;

                while (Global.IsScanBusy)
                {
                    ConsoleSpinner.Instance.Update();
                    //AsciiArt.Draw();
                    Task.Delay(100);

                }

                List<string> restrictions = new List<string>
                {
                    ".bmp",
                    ".png",
                    ".jpg"
                };            

              
                var fname =context.GetValue(FilePath);
                
                var ext = Path.GetExtension(fname);
                if (!restrictions.Contains(ext.ToLower()))
                {
                    if (Path.GetFileName(fname) != "log.txt")
                    {
                        File.Delete(fname);
                        Global.LogError("File Deleted..." + fname);
                    }
                    context.SetValue(this.Result, false);

                }
                else
                {
                    Console.WriteLine("");
                    Console.WriteLine("------------Processing..." + fname);
                    //Do OCR here..
                    context.SetValue(this.Result, true);

                }
                #endregion
            }
            catch (Exception err)
            {

                Global.LogError("FilterImage---> " + err.Message);

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
