using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaiRocs.Models
{
    [Table("TableStatus")]
   public class EngineStatus
    {
        //  [Id] int IDENTITY(1,1) NOT NULL
        public int Id { get; set; }
        //, [MaxValue] int DEFAULT((100)) NOT NULL
        public int MaxValue { get; set; }
        //, [Value] int DEFAULT((0)) NULL
        public int Value { get; set; }
        //, [LastStart]        datetime NULL
        public DateTime LastStart { get; set; }
        //, [TotalConvert] int DEFAULT((0)) NULL
        public int TotalConvert { get; set; }
        //, [Status] nvarchar(25) NULL
        public string Status { get; set; }
        //, [CurrentFolder] nvarchar(100) NULL
        public string CurrentFolder { get; set; }
        //, [LastBatchCount] int DEFAULT((0)) NULL
        public int LastBatchCount { get; set; }
    }
}
