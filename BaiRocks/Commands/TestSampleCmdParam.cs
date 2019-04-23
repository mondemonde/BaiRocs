using LogApplication.Common.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BaiRocs.Commands
{
  public  class TestSampleCmdParam:ClickButtonCmdParam
    {
        public TestSampleCmdParam(Button btn):base(btn)
        {           

        }
        public string SampleMessage { get; set; }


        public void SavePayload()
        {
            RequestDate = DateTime.Now;
            Payload = this;
        }
    }
}
