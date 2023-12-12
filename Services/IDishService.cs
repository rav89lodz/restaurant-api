using RestaurantAPI.Models;
using System.Collections.Generic;

namespace RestaurantAPI.Services;

public interface IDishService
{
    public int Create(CreateDishDto dto, int restaurantId);
    public DishDto GetById(int restaurantId, int dishId);
    public List<DishDto> GetAll(int restaurantId);
    public void DeleteById(int restaurantId, int dishId);
    public void DeleteAll(int restaurantId);
}
