using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Rig.Api;

public static class MediatorExtensions
{
    public static RouteHandlerBuilder Mediate<T>(this RouteGroupBuilder group, HttpMethod method, string pattern = "") where T : notnull
    {
        return group.MapMethods(pattern, new[] { method.Method }, ([FromServices] IMediator mediator, [AsParameters] T request, CancellationToken cancellationToken) =>
        {
            var response = mediator.Send(request, cancellationToken);
            return response;
        });
    }
}
