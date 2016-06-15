using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotLiquid;
using Memorandum.Core.Domain.Files;
using Memorandum.Core.Domain.Permissions;
using Memorandum.Core.Domain.Users;
using Memorandum.Web.ViewModels;

namespace Memorandum.Web.Utitlities
{
    static class TemplateFilters
    {
        public static bool CanRead(dynamic user, dynamic item)
        {
            var u = user.ConvertToValueType();
            var i = item.ConvertToValueType();
            return false;
            //return user.GetModel().CanRead(item.GetModel());
        }


        public static string CanWrite(dynamic user, dynamic item)
        {
            return user.ToString() + item.ToString();
            //return user.GetModel().CanRead(item.GetModel());
        }
    }
}
