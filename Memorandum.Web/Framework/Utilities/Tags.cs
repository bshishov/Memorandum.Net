using System.Collections.Generic;
using System.IO;
using DotLiquid;

namespace Memorandum.Web.Framework.Utilities
{
    class Tags
    {
        public class StaticTag : Tag
        {
            private string _path;

            public override void Initialize(string tagName, string markup, List<string> tokens)
            {
                base.Initialize(tagName, markup, tokens);
                _path = markup.Trim().Trim('"', '\'');
            }

            public override void Render(Context context, TextWriter result)
            {
                result.Write("/static/" + _path);
            }
        }

        public class DynamicTemplate : Tag
        {
             
        }
    }
}
