using Microsoft.AspNetCore.Http;

namespace NETForum.IntegrationTests;

public class FakeHttpContextAccessor : IHttpContextAccessor
{
    public HttpContext? HttpContext { get; set; } = new DefaultHttpContext();
}