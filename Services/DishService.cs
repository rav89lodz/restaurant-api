using AutoMapper;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models;
using RestaurantAPI.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantAPI.Services;

public class DishService : IDishService
{
    private readonly RestaurantDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IRestaurantRepository _repository;

    public DishService(RestaurantDbContext dbContext, IMapper mapper, IRestaurantRepository repository)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _repository = repository;
    }

    public int Create(CreateDishDto dto, int restaurantId)
    {
        var restaurant = _repository.GetRestaurantWithDishesById(restaurantId);
        var dishEntity = _mapper.Map<Dish>(dto);

        dishEntity.RestaurantId = restaurant.Id;

        _dbContext.Dishes.Add(dishEntity);
        _dbContext.SaveChanges();

        return dishEntity.Id;
    }

    public DishDto GetById(int restaurantId, int dishId)
    {
        var restaurant = _repository.GetRestaurantWithDishesById(restaurantId);
        var dish = _dbContext.Dishes.FirstOrDefault(d => d.Id == dishId);
        if(dish is null || dish.RestaurantId != restaurantId)
        {
            throw new NotFoundException("Dish not found");
        }
        var dishDto = _mapper.Map<DishDto>(dish);
        return dishDto;
    }

    public List<DishDto> GetAll(int restaurantId)
    {
        var restaurant = _repository.GetRestaurantWithDishesById(restaurantId);
        var dishDtos = _mapper.Map<List<DishDto>>(restaurant.Dishes);
        return dishDtos;
    }

    public void DeleteById(int restaurantId, int dishId)
    {
        var restaurant = _repository.GetRestaurantWithDishesById(restaurantId);
        var dish = _dbContext.Dishes.FirstOrDefault(d => d.Id == dishId);
        if (dish is null || dish.RestaurantId != restaurantId)
        {
            throw new NotFoundException("Dish not found");
        }
        _dbContext.Dishes.Remove(dish);
        _dbContext.SaveChanges();
    }

    public void DeleteAll(int restaurantId)
    {
        var restaurant = _repository.GetRestaurantWithDishesById(restaurantId);

        _dbContext.RemoveRange(restaurant.Dishes);
        _dbContext.SaveChanges();
    }
}
