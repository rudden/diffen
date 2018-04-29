using System.Security.Claims;

namespace Diffen.Helpers.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
		public static string GetUserId(this ClaimsPrincipal user)
		{
			return !user.Identity.IsAuthenticated ? null : user.FindFirst(ClaimTypes.NameIdentifier).Value;
		}
	}
}
