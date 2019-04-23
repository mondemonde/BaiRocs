using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using LogApplication.Common.Commands;
using LogApplication.INFRA;
using BaiRocs.WF;
using BaiRocs.Common;

namespace BaiRocs.Commands
{
    public sealed class TestSample : CodeActivity
    {
        // Define an activity input argument of type string
        [RequiredArgument]
        public InArgument<CmdParam> Param { get; set; }

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {
            // Obtain the runtime value of the Text input argument

            try
            {
                #region --------------------TRY CONTENT----------------------
                CmdParam p = context.GetValue(this.Param);
               TestSampleCmdParam payload = (TestSampleCmdParam)p.Payload;


                //DoTest
                //Global.MainWindow.SafeInvoke(f => f.DoTestSample(payload.SampleMessage));
                Global.MainWindow.InvokeOnUiThread(
                    () => Global.MainWindow.DoTestSample(payload.SampleMessage));

                #endregion
            }
            catch (Exception err)
            {

                Global.LogError(err.Message);

            }
            finally
            {
               // Global.MainWindow.SafeInvoke(c => c.HideBusy());

            }







        }
    }
}
