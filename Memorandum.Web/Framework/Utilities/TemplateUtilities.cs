using DotLiquid;

namespace Memorandum.Web.Framework.Utilities
{
    public static class TemplateUtilities
    {
        public static string LoadTemplate(string templatePath)
        {
            const string tmpKey = "EntryPointTemplate";
            var ctx = new DotLiquid.Context {[tmpKey] = templatePath};
            return DotLiquid.Template.FileSystem.ReadTemplateFile(ctx, tmpKey);
        }

        public static string RenderTemplate(string templatePath, object context)
        {
            var tpl = DotLiquid.Template.Parse(LoadTemplate(templatePath));
            return tpl.Render(Hash.FromAnonymousObject(context));
        }
    }
}
