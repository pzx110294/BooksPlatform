using System.Security.Claims;

namespace Serwis_Książkowy.Helpers;

public static class UserHelper
{
    public static string GetUserId(this ClaimsPrincipal user)
    {
        var userId=  user.FindFirstValue(ClaimTypes.NameIdentifier);
        userId = userId ?? string.Empty;
        return userId;
    }
}