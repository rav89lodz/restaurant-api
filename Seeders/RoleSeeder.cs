﻿using RestaurantAPI.Entities;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantAPI.Seeders;

public class RoleSeeder
{
    private readonly RestaurantDbContext _dbContext;

    public RoleSeeder(RestaurantDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void Seed()
    {
        if (_dbContext.Database.CanConnect() && !_dbContext.Roles.Any())
        {
            var roles = GetRoles();
            _dbContext.Roles.AddRange(roles);
            _dbContext.SaveChanges();
        }
    }

    private IEnumerable<Role> GetRoles()
    {
        return new List<Role>()
        {
            new Role()
            {
                Name = "Admin"
            },
            new Role()
            {
                Name = "Manager"
            },
            new Role()
            {
                Name = "User"
            },
        };
    }
}
