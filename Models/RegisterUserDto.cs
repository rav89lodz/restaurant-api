﻿using System;
using System.ComponentModel.DataAnnotations;

namespace RestaurantAPI.Models;

public class RegisterUserDto
{
    // Walidacja tego modelu przez paczkę FluentValidation.AspNetCore w klasie /Models/Validators/RegisterUserDtoValidator
    public string Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Nationality { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }

    public int RoleId { get; set; } = 3;
}
