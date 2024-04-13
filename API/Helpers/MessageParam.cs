using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Helpers
{
    public class MessageParam : PaginationParam
    {
        public string UserName { get; set; }

        public string Container { get; set; }="Unread";
    }
}