using System;
using System.IO;
using System.Web.Http;
using System.Web.Http.Cors;
using Beginor.Owin.StaticFile;
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
            // enable cors
            ConfigWebApiCors(config);
            // web api routes
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "rest/{controller}/{id}"
            );
            app.UseWebApi(config);
        }

        private static void ConfigWebApiCors(HttpConfiguration config) {
            var policy = new EnableCorsAttribute(
                origins: "*",
                headers: "*",
                methods: "*"
            ) {
                SupportsCredentials = true
            };
            config.EnableCors(policy);
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
