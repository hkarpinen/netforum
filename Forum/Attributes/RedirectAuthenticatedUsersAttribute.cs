using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NETForum.Attributes;

public class RedirectAuthenticatedUsersAttribute(string redirectPage = "/Index") : Attribute, IPageFilter
{
    public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
    {
        if (context.HttpContext.User.Identity is { IsAuthenticated: true })
        {
            context.Result = new RedirectToPageResult(redirectPage);
        }
    }

    public void OnPageHandlerExecuted(PageHandlerExecutedContext context) { }
    public void OnPageHandlerSelected(PageHandlerSelectedContext context) { }
}