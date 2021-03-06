﻿using System;

namespace Memorandum.Core.Domain
{
    public class TextNode : Node
    {
        public override NodeIdentifier NodeId => new NodeIdentifier("text", Id);
        public virtual int Id { get; set; }
        public virtual string Text { get; set; }
        public override User User { get; set; }
        public virtual DateTime DateAdded { get; set; }
    }
}