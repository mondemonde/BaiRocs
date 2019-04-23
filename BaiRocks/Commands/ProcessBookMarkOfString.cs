using System;
using LogApplication.Common.Commands;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiRocs.COMMANDS
{
    public sealed class ProcessBookMarkOfString : NativeActivity<object>
    {
        [RequiredArgument]
        public InArgument<string> BookmarkName { get; set; }
        //public OutArgument<string> CommandName { get; set; }
        //public OutArgument<object> CommandParam { get; set; }

        protected override bool CanInduceIdle
        {
            get
            {
                return true;
            }
        }

        protected override void Execute(NativeActivityContext context)
        {
            //create waiting or listener here...
            context.CreateBookmark(this.BookmarkName.Get(context), new BookmarkCallback(OnReadComplete));
        }

        void OnReadComplete(NativeActivityContext context, Bookmark bookmark, object state)
        {
            //
            string bname = context.GetValue(this.BookmarkName);


            switch (bname)
            {
                case "String":
                    var input = state as string;
                    context.SetValue(this.Result, input);
                    Console.WriteLine("OnReadComplete: " + input);
                    break;


                default:
                    var input0 = state as CmdParam;
                    context.SetValue(this.Result, input0);
                    //context.SetValue(this.CommandName, input0.CommandName);
                    Console.WriteLine(input0.ToString());
                    break;
            }


        }
    }
}
