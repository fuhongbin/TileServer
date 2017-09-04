using System;
using System.Configuration;
using System.IO;
using System.Web.Http;
using Beginor.Owin.Security.Aes;
using Beginor.Owin.Security.Gdep;
using Beginor.Owin.StaticFile;
using Microsoft.Owin.Security.DataProtection;
using Owin;

namespace OAuthTest {

    public class Startup {

        public void Configuration(IAppBuilder app) {
            ConfigStaticFile(app);
          //  ConfigOauth(app);
            ConfigWebApi(app);
        }

        private static void ConfigWebApi(IAppBuilder app) {
            var config = new HttpConfiguration();
            // remove xml formatter
            var xml = config.Formatters.XmlFormatter;
            config.Formatters.Remove(xml);
            // config json formatter
            var json = config.Formatters.JsonFormatter;
            json.Indent = true;
            json.UseDataContractJsonSerializer = false;
            // web api routes
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "rest/{controller}/{id}"
            );
            app.UseWebApi(config);
        }

        private static void ConfigOauth(IAppBuilder app) {
            // config auth
            var provider = new AesDataProtectionProvider("/OAuthTest");
            app.SetDataProtectionProvider(provider);
            //app.CreatePerOwinContext<IAuthenticationManager>((IdentityFactoryOptions<IAuthenticationManager> options, IOwinContext context) => {
            //    return null;
            //};);
            app.UseExternalSignInCookie();
            var appSettings = ConfigurationManager.AppSettings;
            // oauth
            var oauthOptions = new GdepAuthenticationOptions() {
                Caption = appSettings["oauth-caption"],
                AppId = appSettings["oauth-id"],
                AppSecret = appSettings["oauth-secret"],
                Scope = appSettings["oauth-scope"].Split(',')
            };
            app.UseGdepAuthentication(oauthOptions);
        }

        private static void ConfigStaticFile(IAppBuilder app) {
            // config static file
            var staticFileOptions = new StaticFileMiddlewareOptions() {
                DefaultFile = "index.html",
                EnableETag = false,
                EnableHtml5LocationMode = false,
                RootDirectory = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "../"
                )
            };
            app.UseStaticFile(staticFileOptions);
        }
    }

}
