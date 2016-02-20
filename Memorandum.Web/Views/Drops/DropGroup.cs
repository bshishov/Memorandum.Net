using System.Collections.Generic;
using DotLiquid;

namespace Memorandum.Web.Views.Drops
{
    internal class DropGroup<T> : Drop
    {
        public string Name { get; protected set; }
        public List<T> Items { get; protected set; }
    }
}