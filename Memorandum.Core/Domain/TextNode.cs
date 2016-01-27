﻿using System;
using System.Globalization;
using Memorandum.Core.Repositories;

namespace Memorandum.Core.Domain
{
    public class TextNode : Node
    {
        public override NodeIdentifier NodeId
        {
            get { return new NodeIdentifier("text", Id.ToString(CultureInfo.InvariantCulture)); }
        }
        public virtual int Id { get; set; }
        public virtual string Text { get; set; }
        public override User User { get; set; }

        public virtual DateTime DateAdded { get; set; }
        
    }
}