using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.JSInterop;
using RoomReservations.Models;

namespace RoomReservations.Components;

public enum GetCurrentUserError
{
    Success,
    UserNotLoggedIn,
    HttpContextIsNull
}

public class SharedMethodsService(
    UserManager<ApplicationUser> userManager,
    IHttpContextAccessor httpContextAccessor,
    IJSRuntime jsRuntime,
    AuthenticationStateProvider authenticationStateProvider)
{
    private ApplicationUser? _loggedInUser;
    private ClaimsPrincipal? _roleUser;

    public async void GoBackAsync()
    {
        // Navigate to previous page user was on using JS
        await jsRuntime.InvokeVoidAsync("history.back");
    }

    public async Task<(ApplicationUser?, GetCurrentUserError)> GetCurrentUserAsync()
    {
        // If we already have the user, return it
        if (_loggedInUser != null) return (_loggedInUser, GetCurrentUserError.Success);

        if (httpContextAccessor.HttpContext is null) return (null, GetCurrentUserError.HttpContextIsNull);

        var user = await userManager.GetUserAsync(httpContextAccessor.HttpContext.User);
        if (user is null) return (null, GetCurrentUserError.UserNotLoggedIn);

        _loggedInUser = user;
        return (_loggedInUser, GetCurrentUserError.Success);
    }

    public async Task<bool> IsCurrentUserInRoleAsync(string role)
    {
        if (_roleUser is null)
        {
            var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
            _roleUser = authState.User;
        }

        return _roleUser.IsInRole(role);
    }
}