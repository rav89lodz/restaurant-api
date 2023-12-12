using Microsoft.AspNetCore.Authorization;

namespace RestaurantAPI.Authorization;

public class CreatedMultipleRestaurantsRequirement : IAuthorizationRequirement
{
    public int MinimumRestaurantsCreated { get; set; }

    public CreatedMultipleRestaurantsRequirement(int minimumRestaurantsCreated)
    {
        MinimumRestaurantsCreated = minimumRestaurantsCreated;
    }
}
