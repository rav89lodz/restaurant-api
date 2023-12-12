using Microsoft.AspNetCore.Authorization;

namespace RestaurantAPI.Authorization;

public class MinimumAgeRequirement : IAuthorizationRequirement
{
    public int MinimumAge {  get; }
    public MinimumAgeRequirement(int age)
    {
        MinimumAge = age;
    }
}
