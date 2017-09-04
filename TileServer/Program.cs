using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Owin.Builder;
using Nowin;
using System.Threading;

namespace OAuthTest {

    public class Program {

        public static void Main(string[] args) {
            // Define listening ip and port;
            var ip = IPAddress.Loopback;
            const int port = 8088;
            // start the server;
            using (var server = BuildNowinServer(ip, port)) {
                ManualResetEvent exitEvent = new ManualResetEvent(false);
                Console.CancelKeyPress += (sender, e) => {
                    e.Cancel = true;
                    exitEvent.Set();
                };
                var serverRef = new WeakReference<INowinServer>(server);
                Task.Run(() => {
                    INowinServer nowinServer;
                    if (serverRef.TryGetTarget(out nowinServer)) {
                        nowinServer.Start();
                    }
                });
                var baseAddress = "http://" + ip + ":" + port + "/";
                var msg = $"Nowin server listening {baseAddress}, press [Ctrl + C] to exit.";
                Console.WriteLine(msg);
                exitEvent.WaitOne();
                Console.WriteLine("Stoping server ... ");
            }
        }

        private static INowinServer BuildNowinServer(IPAddress ip, int port) {
            // create a new AppBuilder
            var appBuilder = new AppBuilder();
            // init nowin's owin server factory.
            OwinServerFactory.Initialize(appBuilder.Properties);
            var startup = new Startup();
            startup.Configuration(appBuilder);
            // build server
            var serverBuilder = new ServerBuilder();
            var capabilities = appBuilder.Properties[OwinKeys.ServerCapabilitiesKey];
            serverBuilder
                .SetAddress(ip)
                .SetPort(port)
                .SetOwinApp(appBuilder.Build())
                .SetOwinCapabilities((IDictionary<string, object>)capabilities);
            return serverBuilder.Build();
        }

    }

}
