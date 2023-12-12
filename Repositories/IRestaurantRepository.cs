using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using System.Collections.Generic;

namespace RestaurantAPI.Repositories;

public interface IRestaurantRepository
{
    public Restaurant GetRestaurantById(int id);
    public Restaurant GetRestaurantWithDishesById(int restaurantId);
    public Restaurant GetRestaurantWithDishesAndAddressById(int restaurantId);
    public List<Restaurant> GetAllRestaurantsWithData();
    public List<Restaurant> GetAllRestaurantsWithDataAndSearchPhrase(RestaurantQuery query);
    public int GetCount(List<Restaurant> list);
    public Restaurant CreateRestaurant(Restaurant restaurant);
    public void DeleteRestaurnat(Restaurant restaurant);
}
