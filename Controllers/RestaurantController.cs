using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Models;
using RestaurantAPI.Services;
using System.Collections.Generic;

namespace RestaurantAPI.Controllers;

[ApiController]
[Route("api/restaurant")]
[Authorize] // sprawdzenie nagłówka autoryzacji, można tutaj dla całego kontrolera, lub dla pojedynczej akcji
public class RestaurantController : ControllerBase
{
    private readonly IRestaurantService _service;

    public RestaurantController(IRestaurantService service)
    {
        _service = service;
    }

    [HttpGet]
    [AllowAnonymous] // jesli mamy autoryzację na poziomie kontrolera, to tutaj wykluczamy tą akcję, nie będzie ona podlegała sprawdzeniu
    public ActionResult<IEnumerable<RestaurantDto>> GetAll([FromQuery] RestaurantQuery query)
    {
        return Ok(_service.GetAll(query));
    }

    [HttpGet("{id}")]
    // atrybut z poziomu kontrolera jest tutaj nadpisany z nadaniem konkretnej polityki autoryzacji zdefiniowanej w Program.cs
    [Authorize(Policy = "HasNationality")]
    public ActionResult<RestaurantDto> Get([FromRoute] int id)
    {
        return Ok(_service.GetById(id));
    }

    [HttpPost]
    // atrybut z poziomu kontrolera jest tutaj nadpisany z nadaniem konkretnej roli systemowej (musi być ona podana w tokenie jwt)
    [Authorize(Roles = "Admin,Manager")]
    public ActionResult CreateRestaurant([FromBody] CreateRestaurantDto dto)
    {
        /*
            dzięki zastosowaniu zapisu [ApiController] ten warunek wykona się automatycznie

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
        */
        var id = _service.Create(dto);
        return Created($"/api/restaurant/{id}", null);
    }

    [HttpPut("{id}")]
    // atrybut z poziomu kontrolera jest tutaj nadpisany z nadaniem konkretnej polityki autoryzacji zdefiniowanej w Program.cs
    [Authorize(Policy = "Atleast20")]
    public ActionResult Update([FromBody] UpdateRestaurantDto dto, [FromRoute] int id)
    {
        _service.Update(dto, id);
        return Ok();
    }

    [HttpDelete("{id}")]
    public ActionResult Delete([FromRoute] int id)
    {
        _service.Delete(id);
        return NoContent();
    }
}
