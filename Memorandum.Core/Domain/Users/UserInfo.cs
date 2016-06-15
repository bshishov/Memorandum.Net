using System.Collections.Generic;

namespace Memorandum.Core.Domain.Users
{
    /// <summary>
    /// Serailizable class for storing user settings
    /// </summary>
    public class UserInfo
    {
        public string Name { get; set; }
        public bool UsePasswordAuth { get; set; }
        public string Password { get; set; }
        public string BaseDirectory { get; set; }
        public bool UseIpAuth { get; set; }
        public string IpAddress { get; set; }
        public List<SharingInfo> Sharings { get; set; }
    }
}
