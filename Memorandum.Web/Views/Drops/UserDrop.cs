using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotLiquid;
using Memorandum.Core.Domain;
using Memorandum.Core.Domain.Users;

namespace Memorandum.Web.Views.Drops
{
    class UserDrop : Drop
    {
        public string Name { get; private set; }
        public DirectoryItemDrop Base { get; }

        public UserDrop(User user)
        {
            Name = user.Name;
            Base = new DirectoryItemDrop(user.Base);
        }
    }
}
