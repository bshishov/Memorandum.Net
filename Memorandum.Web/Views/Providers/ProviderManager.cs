using System.Collections.Generic;
using System.Linq;

namespace Memorandum.Web.Views.Providers
{
    static class ProviderManager
    {
        private static readonly IEnumerable<IProviderView> Providers = new List<IProviderView>()
        {
            new FileProvider(),
            new UrlProvider(),
            new TextNodeProvider()
        };

        public static IProviderView Get(string key)
        {
            return Providers.FirstOrDefault(p => p.Id.Equals(key));
        }
    }
}