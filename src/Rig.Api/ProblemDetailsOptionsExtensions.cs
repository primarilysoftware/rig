using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Rig.Api;

public static class ProblemDetailsOptionsExtensions
{
    public static ProblemDetailsOptions MapExceptionToStatusCode<T>(this ProblemDetailsOptions options, int statusCode, string? title = null, string? type = null) where T : Exception
    {
        return options.MapException<T>(problemDetails =>
        {
            var typeTitlePair = ("https://datatracker.ietf.org/doc/html/rfc9110#name-status-codes", ReasonPhrases.GetReasonPhrase(statusCode));
            Defaults.TryGetValue(statusCode, out typeTitlePair);
            problemDetails.Status = statusCode;
            problemDetails.Type = type ?? typeTitlePair.Item1;
            problemDetails.Title = title ?? typeTitlePair.Item2;
        });
    }

    public static ProblemDetailsOptions MapException<T>(this ProblemDetailsOptions options, Action<ProblemDetails> action) where T : Exception
    {
        return options.MapException<T>((problemDetails, _) => action(problemDetails));
    }

    public static ProblemDetailsOptions MapException<T>(this ProblemDetailsOptions options, Action<ProblemDetails, T> action) where T : Exception
    {
        var customization = options.CustomizeProblemDetails;

        options.CustomizeProblemDetails = ctx =>
        {
            customization?.Invoke(ctx);

            if (ctx.Exception is T ex)
            {
                action.Invoke(ctx.ProblemDetails, ex);

                if (ctx.ProblemDetails.Status.HasValue && ctx.ProblemDetails.Status != ctx.HttpContext.Response.StatusCode)
                {
                    ctx.HttpContext.Response.StatusCode = ctx.ProblemDetails.Status.Value;
                }
            }
        };

        return options;
    }

    // https://github.com/dotnet/aspnetcore/blob/529e84427bb4e281762622f2984c4320b6d0a08f/src/Shared/ProblemDetails/ProblemDetailsDefaults.cs#L87
    public static readonly Dictionary<int, (string Type, string Title)> Defaults = new()
    {
        [400] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.1",
            "Bad Request"
        ),

        [401] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.2",
            "Unauthorized"
        ),

        [403] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.4",
            "Forbidden"
        ),

        [404] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.5",
            "Not Found"
        ),

        [405] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.6",
            "Method Not Allowed"
        ),

        [406] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.7",
            "Not Acceptable"
        ),

        [408] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.9",
            "Request Timeout"
        ),

        [409] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.10",
            "Conflict"
        ),

        [412] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.13",
            "Precondition Failed"
        ),

        [415] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.16",
            "Unsupported Media Type"
        ),

        [422] =
        (
            "https://tools.ietf.org/html/rfc4918#section-11.2",
            "Unprocessable Entity"
        ),

        [426] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.5.22",
            "Upgrade Required"
        ),

        [500] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.6.1",
            "An error occurred while processing your request."
        ),

        [502] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.6.3",
            "Bad Gateway"
        ),

        [503] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.6.4",
            "Service Unavailable"
        ),

        [504] =
        (
            "https://tools.ietf.org/html/rfc9110#section-15.6.5",
            "Gateway Timeout"
        ),
    };
}
