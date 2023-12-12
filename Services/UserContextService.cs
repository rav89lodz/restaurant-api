﻿using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace RestaurantAPI.Services;

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor httpContextAccessor) // dodać zależność IHttpContextAccessor w Program.cs
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User;
    public int? GetUserId => User is null ? null : (int?)int.Parse(User.FindFirst(u => u.Type == ClaimTypes.NameIdentifier).Value);
}
