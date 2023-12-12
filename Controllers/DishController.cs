using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Models;
using RestaurantAPI.Services;
using System.Collections.Generic;

namespace RestaurantAPI.Controllers;

[ApiController]
[Route("api/restaurant/{restaurantId}/dish")]
public class DishController : ControllerBase
{
    private readonly IDishService _service;
    public DishController(IDishService service)
    {
        _service = service;
    }

    [HttpGet]
    public ActionResult<List<DishDto>> GetAll([FromRoute] int restaurantId)
    {
        var dishes = _service.GetAll(restaurantId);
        return Ok(dishes);
    }

    [HttpGet("{dishId}")]
    public ActionResult<DishDto> GetById([FromRoute] int restaurantId, [FromRoute] int dishId)
    {
        var dish = _service.GetById(restaurantId, dishId);
        return Ok(dish);
    }

    [HttpPost]
    public ActionResult CreateDish([FromRoute] int restaurantId, [FromBody] CreateDishDto dto)
    {
        var newDishId = _service.Create(dto, restaurantId);
        return Created($"api/restaurant/{restaurantId}/dish/{newDishId}", null);
    }

    [HttpDelete]
    public ActionResult DeleteAll([FromRoute] int restaurantId)
    {
        _service.DeleteAll(restaurantId);
        return NoContent();
    }

    [HttpDelete("{dishId}")]
    public ActionResult DeleteById([FromRoute] int restaurantId, [FromRoute] int dishId)
    {
        _service.DeleteById(restaurantId, dishId);
        return NoContent();
    }
}
