using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LTuri.Abp.Application.Services.Response
{
    public class PaginationResponse
    {
        public long Total { get; set; } = 0;

        public long UnfilteredTotal { get; set; } = 0;
        public long Page { get; set; } = 0;
        public long Size { get; set; } = 100;
    }
}
