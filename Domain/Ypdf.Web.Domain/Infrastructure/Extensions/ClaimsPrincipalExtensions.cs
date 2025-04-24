using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Ypdf.Web.Domain.Models.Api.Exceptions;

namespace Ypdf.Web.Domain.Infrastructure.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string? Get(this ClaimsPrincipal userClaims, string claimType)
    {
        ArgumentNullException.ThrowIfNull(userClaims, nameof(userClaims));
        ArgumentNullException.ThrowIfNull(claimType, nameof(claimType));

        return userClaims.FindFirstValue(claimType);
    }

    public static bool Get<T>(this ClaimsPrincipal userClaims, string claimType, out T? value)
        where T : IParsable<T>
    {
        ArgumentNullException.ThrowIfNull(userClaims, nameof(userClaims));
        ArgumentNullException.ThrowIfNull(claimType, nameof(claimType));

        string? claimValue = userClaims.Get(claimType);

        return T.TryParse(claimValue, null, out value);
    }

    public static bool VerifyAccess<T>(
        this ClaimsPrincipal userClaims,
        string claimType,
        params T[] allowedValues)
        where T : IParsable<T>
    {
        ArgumentNullException.ThrowIfNull(userClaims, nameof(userClaims));
        ArgumentNullException.ThrowIfNull(claimType, nameof(claimType));
        ArgumentNullException.ThrowIfNull(allowedValues, nameof(allowedValues));

        return userClaims.VerifyAccess<T>(claimType, t => allowedValues.Contains(t));
    }

    public static bool VerifyAccess<T>(
        this ClaimsPrincipal userClaims,
        string claimType,
        Predicate<T> allowPredicate)
        where T : IParsable<T>
    {
        ArgumentNullException.ThrowIfNull(userClaims, nameof(userClaims));
        ArgumentNullException.ThrowIfNull(claimType, nameof(claimType));
        ArgumentNullException.ThrowIfNull(allowPredicate, nameof(allowPredicate));

        bool valueParsed = userClaims.Get(claimType, out T? claimValue);

        return valueParsed
            && claimValue is not null
            && !allowPredicate.Invoke(claimValue);
    }

    public static void VerifyAccessAndThrow<T>(
        this ClaimsPrincipal userClaims,
        string claimType,
        params T[] allowedValues)
        where T : IParsable<T>
    {
        ArgumentNullException.ThrowIfNull(userClaims, nameof(userClaims));
        ArgumentNullException.ThrowIfNull(claimType, nameof(claimType));
        ArgumentNullException.ThrowIfNull(allowedValues, nameof(allowedValues));

        userClaims.VerifyAccessAndThrow<T>(claimType, t => allowedValues.Contains(t));
    }

    public static void VerifyAccessAndThrow<T>(
        this ClaimsPrincipal userClaims,
        string claimType,
        Predicate<T> allowPredicate)
        where T : IParsable<T>
    {
        ArgumentNullException.ThrowIfNull(userClaims, nameof(userClaims));
        ArgumentNullException.ThrowIfNull(claimType, nameof(claimType));
        ArgumentNullException.ThrowIfNull(allowPredicate, nameof(allowPredicate));

        bool allowed = userClaims.VerifyAccess(claimType, allowPredicate);

        if (!allowed)
            throw new ForbiddenException();
    }

    public static IEnumerable<(string Type, string Value)> ToTypeValuePairs(
        this ClaimsPrincipal userClaims)
    {
        ArgumentNullException.ThrowIfNull(userClaims, nameof(userClaims));

        return userClaims.Claims
            .Select(t => (t.Type, t.Value));
    }
}
