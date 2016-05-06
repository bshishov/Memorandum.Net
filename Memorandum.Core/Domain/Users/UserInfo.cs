using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memorandum.Core.Domain.Users
{
    public class UserInfo
    {
        public string Name { get; set; }
        public bool UsePasswordAuth { get; set; }
        public string Password { get; set; }
        public string BaseDirectory { get; set; }
        public bool UseIpAuth { get; set; }
        public string IpAddress { get; set; }
    }
}
