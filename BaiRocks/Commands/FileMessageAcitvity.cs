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
    public sealed class FileMesssageActivity : NativeActivity
    {
        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(NativeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument

            try
            {
                #region --------------------TRY CONTENT----------------------
                string dumpDir = FileService.Config.GetValue("RawImageFolder");//dump folder
                string fMesage = dumpDir + "\\running.txt";
                if (!File.Exists(fMesage))
                {
                    File.Create(fMesage).Close();
                }

                #endregion
            }
            catch (Exception err)
            {

                Global.LogError("FileMesssageActivity---> " + err.Message);

            }



        }



    }

}
