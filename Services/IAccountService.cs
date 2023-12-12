using RestaurantAPI.Models;

namespace RestaurantAPI.Services;

public interface IAccountService
{
    public int Register(RegisterUserDto dto);
    public string GenerateJwt(LoginDto dto);
}
