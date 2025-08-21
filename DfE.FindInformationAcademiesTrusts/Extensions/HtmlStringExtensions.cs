using Microsoft.AspNetCore.Html;
using System.Text.RegularExpressions;

namespace DfE.FindInformationAcademiesTrusts.Extensions
{
    public static class HtmlStringExtensions
    {
        private static readonly Regex NotAlphaNumeric = new("[^[a-z0-9-_]", RegexOptions.Compiled | RegexOptions.CultureInvariant, TimeSpan.FromSeconds(2));

        public static HtmlString Stub(this string input)
        {
            return new HtmlString(NotAlphaNumeric.Replace(input.ToLowerInvariant(), "-"));
        }
    }
}
