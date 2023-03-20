using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace SelfHost
{
    public class InitialServer
    {
        private readonly HttpSelfHostServer server;

        public InitialServer()
        {
            var config = new HttpSelfHostConfiguration("http://localhost:8080");

            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/octet-stream"));

            config.Routes.MapHttpRoute(
                "API Default", "api/{controller}/{id}",
                new { id = RouteParameter.Optional });

            server = new HttpSelfHostServer(config);
        }

        public void Start()
        {
            server.OpenAsync();
        }

        public void Stop()
        {
            server.CloseAsync();
            server.Dispose();
        }
    }
}
