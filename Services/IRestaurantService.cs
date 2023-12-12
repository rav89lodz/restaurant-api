using RestaurantAPI.Models;
using System.Collections.Generic;

namespace RestaurantAPI.Services;

public interface IRestaurantService
{
    public RestaurantDto? GetById(int id);
    public PagedResult<RestaurantDto> GetAll(RestaurantQuery query);
    public int Create(CreateRestaurantDto dto);
    public void Delete(int id);
    public void Update(UpdateRestaurantDto dto, int id);
}