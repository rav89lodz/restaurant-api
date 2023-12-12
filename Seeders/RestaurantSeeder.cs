using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantAPI.Seeders;

public class RestaurantSeeder
{
    private readonly RestaurantDbContext _dbContext;

    public RestaurantSeeder(RestaurantDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Seed()
    {
        if (_dbContext.Database.CanConnect() && !_dbContext.Restaurants.Any())
        {
            // dodanie automatycznych migracji przy seedowaniu, jeśli są jakieś niewykonane migracje
            // dodaję tylko tutaj, bo to jest pierwszy w kolejce seeder

            var pendingMigrations = _dbContext.Database.GetPendingMigrations();
            if(pendingMigrations != null && pendingMigrations.Any())
            {
                _dbContext.Database.Migrate();
            }

            var restaurants = GetRestaurants();
            _dbContext.Restaurants.AddRange(restaurants);
            _dbContext.SaveChanges();
        }
    }

    private IEnumerable<Restaurant> GetRestaurants()
    {
        return new List<Restaurant>()
        {
            new Restaurant()
            {
                Name = "KFC",
                Category = "Fast Food",
                Description = "Kentucky Fried Chicken",
                ContactEmail = "contact@kfc.cm",
                HasDelivery = true,
                Dishes = new List<Dish>()
                {
                    new Dish()
                    {
                        Name = "Nashville Hot Chicken",
                        Price = 10.30M
                    },
                    new Dish()
                    {
                        Name = "Chicken Nuggets",
                        Price = 5.30M
                    }
                },
                Address = new Address()
                {
                    City = "Łódź",
                    Street = "Karskiego 5",
                    PostalCode = "91-200"
                }
            },
            new Restaurant()
            {
                Name = "McDonald's",
                Category = "Fast Food",
                Description = "McD",
                ContactEmail = "contact@mcd.cm",
                HasDelivery = true,
                Dishes = new List<Dish>()
                {
                    new Dish()
                    {
                        Name = "Big Mac",
                        Price = 14.99M
                    },
                    new Dish()
                    {
                        Name = "Chicken Burger",
                        Price = 4.80M
                    }
                },
                Address = new Address()
                {
                    City = "Łódź",
                    Street = "Piłsudskiego 24",
                    PostalCode = "91-340"
                }
            }
        };
    }
}
