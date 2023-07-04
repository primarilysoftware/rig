using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Mime;
using Yarp.ReverseProxy.Forwarder;
using Microsoft.AspNetCore.Http;

namespace Rig.Api;

public static class YarpExtensions
{
    public static void RigSpaProxy(this WebApplication app, string spaServer, bool requireAuthentication = false)
    {
        var forwarder = app.Services.GetRequiredService<IHttpForwarder>();

        var httpClient = new HttpMessageInvoker(new SocketsHttpHandler()
        {
            UseProxy = false,
            AllowAutoRedirect = false,
            AutomaticDecompression = DecompressionMethods.None,
            UseCookies = false
        });

        app.Map("{**catch-all}", async httpContext =>
        {
            if (requireAuthentication && !(httpContext.User.Identity?.IsAuthenticated ?? false))
            {
                await httpContext.ChallengeAsync();
                return;
            }

            await forwarder.SendAsync(httpContext, spaServer, httpClient);

            if (httpContext.Response.ContentType == MediaTypeNames.Text.Html)
            {
                // do not cache the HTML page coming back from the SPA
                httpContext.Response.Headers.Append("Cache-Control", "no-cache");
            }
        });
    }
}
