#region

using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;

#endregion

namespace Web.Consumer._Infra
{
    public static class UrlHelperExtensions
    {
        private static readonly bool IsDebug;
        private static readonly string CdnPath;
        private static readonly string CacheBust = "1.7";

        static UrlHelperExtensions()
        {

            CdnPath = ConfigurationManager.AppSettings["cdnPath"];
            IsDebug = HttpContext.Current.IsDebuggingEnabled;
            CacheBust = Assembly.GetAssembly(typeof(UrlHelperExtensions)).GetName().Version.ToString();
        }

        //http://localhost:56747/
        public static string ScriptResource(this UrlHelper urlHelper, string developmentPath, string productionPath)
        {
          
            var scriptUrl = (IsDebug ? developmentPath : productionPath).Replace("~", string.Empty);
            var scriptUrlAbs = CdnPath + scriptUrl;

            return scriptUrlAbs + "?v=" + CacheBust;
        }
    }

    public static class StaticResourceScripts
    {
        public static StaticResourceScriptsBundle Bundle()
        {
            return new StaticResourceScriptsBundle();
        }
    }

    public class StaticResourceScriptsBundle
    {
        private readonly StringBuilder _sbScripts = new StringBuilder();

        public StaticResourceScriptsBundle Include(string developmentPath, string productionPath)
        {
            var scriptUrl = UrlHelperExtensions.ScriptResource(null, developmentPath, productionPath);
            _sbScripts.Append(CreateScriptTag(scriptUrl));
            return this;
        }

        public MvcHtmlString Render()
        {
            var scripts = _sbScripts.ToString();
            return MvcHtmlString.Create(scripts);
        }

        private static string CreateScriptTag(string scriptUrl)
        {
            return string.Format(@"<script type=""text/javascript"" src=""{0}""></script>" + "\n", scriptUrl);

        }
    }


}
