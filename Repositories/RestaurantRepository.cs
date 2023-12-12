using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace RestaurantAPI.Repositories;

public class RestaurantRepository : IRestaurantRepository
{
    private readonly RestaurantDbContext _dbContext;
    public RestaurantRepository(RestaurantDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Restaurant GetRestaurantById(int id)
    {
        var restaurant = _dbContext.Restaurants.FirstOrDefault(r => r.Id == id);
        if (restaurant is null)
        {
            throw new NotFoundException("Restaurant not found");
        }
        return restaurant;
    }
    public Restaurant GetRestaurantWithDishesById(int restaurantId)
    {
        var restaurant = _dbContext.Restaurants.Include(r => r.Dishes).FirstOrDefault(r => r.Id == restaurantId);
        if (restaurant is null)
        {
            throw new NotFoundException("Restaurant not found");
        }
        return restaurant;
    }

    public Restaurant GetRestaurantWithDishesAndAddressById(int restaurantId)
    {
        var restaurant = _dbContext.Restaurants.Include(r => r.Address).Include(r => r.Dishes).FirstOrDefault(r => r.Id == restaurantId);
        if (restaurant is null)
        {
            throw new NotFoundException("Restaurant not found");
        }
        return restaurant;
    }

    public List<Restaurant> GetAllRestaurantsWithData()
    {
        var restaurants = _dbContext.Restaurants.Include(r => r.Address).Include(r => r.Dishes).ToList();
        return restaurants;
    }

    public List<Restaurant> GetAllRestaurantsWithDataAndSearchPhrase(RestaurantQuery query)
    {
        var baseQuery = _dbContext.Restaurants.Include(r => r.Address).Include(r => r.Dishes)
                            .Where(r => query.SearchPhrase == null ||
                                (r.Name.ToLower().Contains(query.SearchPhrase.ToLower())));

        if(!string.IsNullOrEmpty(query.SortBy))
        {
            var columnsSelectors = new Dictionary<string, Expression<Func<Restaurant, object>>>
            {
                { nameof(Restaurant.Name), r => r.Name },
                { nameof(Restaurant.Category), r => r.Category },
                { nameof(Restaurant.HasDelivery), r => r.HasDelivery },
            };
            var selectedColun = columnsSelectors[query.SortBy];

            baseQuery = query.SortOrder == SortOrder.ASC ? baseQuery.OrderBy(selectedColun) : baseQuery.OrderByDescending(selectedColun);
        }


        var restaurants = baseQuery.Skip(query.PageSize * (query.PageNumber - 1)).Take(query.PageSize).ToList();
        return restaurants;
    }

    public int GetCount(List<Restaurant> list)
    {
        return list.Count();
    }

    public Restaurant CreateRestaurant(Restaurant restaurant)
    {
        _dbContext.Restaurants.Add(restaurant);
        _dbContext.SaveChanges();
        return restaurant;
    }

    public void DeleteRestaurnat(Restaurant restaurant)
    {
        _dbContext.Restaurants.Remove(restaurant);
        _dbContext.SaveChanges();
    }
}
