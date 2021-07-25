using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuditSvc
{
    public class AuditInfo
    {
        public string Who { get; set; }
        public DateTimeOffset When { get; set; }
        public string What { get; set; }
    }
}
