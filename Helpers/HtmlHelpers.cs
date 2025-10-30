using HtmlAgilityPack;
using System.Net;

namespace NETForum.Helpers
{
    public static class HtmlHelpers
    {
        public static string TruncateHtml(string html, int maxLength)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var text = WebUtility.HtmlDecode(doc.DocumentNode.InnerText);
            if (text.Length <= maxLength) return WebUtility.HtmlEncode(text);
            text = text[..maxLength];
            text += "...";
            return WebUtility.HtmlEncode(text);
        }
    }
}
