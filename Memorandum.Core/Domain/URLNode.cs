using System;

namespace Memorandum.Core.Domain
{
    public class URLNode : Node
    {
        public virtual int Id { get; set; }
        public virtual string URL { get; set; }
        public virtual string Hash { get; set; }
        public virtual string Name { get; set; }
        public virtual string Image { get; set; }
        public virtual DateTime DateAdded { get; set; }
        public override User User { get; set; }

        public override string Provider
        {
            get { return "url"; }
        }

        public override string NodeId
        {
            get { return Hash; }
        }
    }
}