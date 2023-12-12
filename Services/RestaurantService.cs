using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestaurantAPI.Authorization;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models;
using RestaurantAPI.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace RestaurantAPI.Services;

public class RestaurantService : IRestaurantService
{
    private readonly RestaurantDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ILogger<RestaurantService> _logger;
    private readonly IRestaurantRepository _repository;
    private readonly IAuthorizationService _authorizationService;
    private readonly IUserContextService _userContextService;

    public RestaurantService(RestaurantDbContext dbContext, IMapper mapper, ILogger<RestaurantService> logger, IRestaurantRepository repository,
        IAuthorizationService authorizationService, IUserContextService userContextService) 
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _logger = logger;
        _repository = repository;
        _authorizationService = authorizationService;
        _userContextService = userContextService;
    }

    public RestaurantDto? GetById(int id)
    {
        return _mapper.Map<RestaurantDto>(_repository.GetRestaurantWithDishesAndAddressById(id));
    }

    public PagedResult<RestaurantDto> GetAll(RestaurantQuery query)
    {
        var restaurants = _repository.GetAllRestaurantsWithDataAndSearchPhrase(query);
        var restaurantsDtos = _mapper.Map<List<RestaurantDto>>(restaurants);
        return new PagedResult<RestaurantDto>(restaurantsDtos, _repository.GetCount(restaurants), query.PageSize, query.PageNumber);
    }

    public int Create(CreateRestaurantDto dto)
    {
        var restaurant = _mapper.Map<Restaurant>(dto);
        restaurant.CreatedById = _userContextService.GetUserId;
        _repository.CreateRestaurant(restaurant);
        return restaurant.Id;
    }

    public void Delete(int id)
    {
        _logger.LogError($"Restaurant with id: {id} DELETE action invaked");

        var restaurant = _repository.GetRestaurantById(id);

        var authorizationResult = _authorizationService.AuthorizeAsync(_userContextService.User, restaurant, new ResourceOperationRequirement(ResourceOperation.Delete)).Result;

        if (!authorizationResult.Succeeded)
        {
            throw new ForbiddenException("Delete forbidden");
        }
        _repository.DeleteRestaurnat(restaurant);
    }

    public void Update(UpdateRestaurantDto dto, int id)
    {
        var restaurant = _repository.GetRestaurantById(id);

        var authorizationResult = _authorizationService.AuthorizeAsync(_userContextService.User, restaurant, new ResourceOperationRequirement(ResourceOperation.Update)).Result;

        if (!authorizationResult.Succeeded)
        {
            throw new ForbiddenException("Update forbidden");
        }

        restaurant.Name = dto.Name;
        restaurant.Description = dto.Description;
        restaurant.HasDelivery = dto.HasDelivery;

        _dbContext.SaveChanges();
    }
}
