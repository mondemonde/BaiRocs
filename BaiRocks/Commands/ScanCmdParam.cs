using BaiRocs.Common;
using LogApplication.Common.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiRocs.Commands
{
  public  class ScanImageCmdParam:CmdParam
    {
        public ScanImageCmdParam()
        {
            CommandName = EnumCmd.ScanImage.ToString();
            Payload = this;
        }
        public string FileFullPath { get; set; }
        public void SavePayload()
        {
            Payload = this;
        }
    }
}
