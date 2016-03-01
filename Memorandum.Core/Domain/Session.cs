using System;

namespace Memorandum.Core.Domain
{
    public class Session
    {
        public virtual string Key { get; set; }
        public virtual DateTime Expires { get; set; }
        public virtual string Data { get; set; }
    }
}
