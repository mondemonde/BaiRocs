using LogApplication.Common.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiWcfServiceLibrary
{
  public  class DeleteCmdParam: CmdParam
    {
        public DeleteCmdParam()
        {
            CommandName = "DeleteCmd";
        }
    }
}
