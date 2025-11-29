namespace NETForum.IntegrationTests;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

public class FakeLinkGenerator : LinkGenerator
{
    public override string? GetUriByAddress<TAddress>(
        HttpContext httpContext, 
        TAddress address, 
        RouteValueDictionary values, 
        RouteValueDictionary? ambientValues = default, 
        string? scheme = default, 
        HostString? host = default, 
        PathString? pathBase = default, 
        FragmentString fragment = default, 
        LinkOptions? options = default)
    {
        return "https://localhost/Account/ConfirmEmail?userId=1&code=faketoken";
    }
    
    public override string? GetUriByAddress<TAddress>(
        TAddress address, 
        RouteValueDictionary values, 
        string? scheme, 
        HostString host, 
        PathString pathBase = default, 
        FragmentString fragment = default, 
        LinkOptions? options = default)
    {
        return "https://localhost/Account/ConfirmEmail?userId=1&code=faketoken";
    }
    
    public override string? GetPathByAddress<TAddress>(
        HttpContext httpContext, 
        TAddress address, 
        RouteValueDictionary values, 
        RouteValueDictionary? ambientValues = default, 
        PathString? pathBase = default, 
        FragmentString fragment = default, 
        LinkOptions? options = default)
    {
        return "/Account/ConfirmEmail";
    }
    
    public override string? GetPathByAddress<TAddress>(
        TAddress address, 
        RouteValueDictionary values, 
        PathString pathBase = default, 
        FragmentString fragment = default, 
        LinkOptions? options = default)
    {
        return "/Account/ConfirmEmail";
    }
}