﻿using Microsoft.AspNetCore.Authorization;
using RestaurantAPI.Entities;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RestaurantAPI.Authorization;

public class CreatedMultipleRestaurantsRequirementHandler : AuthorizationHandler<CreatedMultipleRestaurantsRequirement>
{
    private readonly RestaurantDbContext _dbContext;

    public CreatedMultipleRestaurantsRequirementHandler(RestaurantDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CreatedMultipleRestaurantsRequirement requirement)
    {
        var userId = int.Parse(context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);
        var createdRestaurantsCount = _dbContext.Restaurants.Count(r => r.CreatedById == userId);

        if(createdRestaurantsCount >= requirement.MinimumRestaurantsCreated)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
