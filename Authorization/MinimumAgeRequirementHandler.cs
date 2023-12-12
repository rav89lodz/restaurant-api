using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RestaurantAPI.Authorization;

public class MinimumAgeRequirementHandler : AuthorizationHandler<MinimumAgeRequirement>
{
    private readonly ILogger<MinimumAgeRequirementHandler> _logger;

    public MinimumAgeRequirementHandler(ILogger<MinimumAgeRequirementHandler> logger)
    {
        _logger = logger;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
    {
        var dateOfBirht = DateTime.Parse(context.User.FindFirst(c => c.Type == "DateOfBirth").Value);
        var userId = context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value;

        _logger.LogInformation($"User: {userId} with date of birth [{dateOfBirht}]");

        if(dateOfBirht.AddYears(requirement.MinimumAge) <= DateTime.Today)
        {
            _logger.LogInformation("Authorization succeedded");
            context.Succeed(requirement);
        }
        else
        {
            _logger.LogInformation("Authorization failed");
        }
        return Task.CompletedTask;
    }
}
