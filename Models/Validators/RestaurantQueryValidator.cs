﻿using FluentValidation;
using RestaurantAPI.Entities;
using System.Linq;

namespace RestaurantAPI.Models.Validators;

public class RestaurantQueryValidator : AbstractValidator<RestaurantQuery>
{
    private int[] allowedPageSize = { 5, 10, 15 };
    private string[] allowedSortByColumnNames = { nameof(Restaurant.Name), nameof(Restaurant.Category), nameof(Restaurant.HasDelivery) };
    public RestaurantQueryValidator()
    {
        RuleFor(r => r.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(r => r.PageSize).Custom((value, context) => 
        { 
            if(!allowedPageSize.Contains(value))
            {
                context.AddFailure("PageSize", $"PageSize must in [{string.Join(",", allowedPageSize)}]"); // łączenie tablicy jako string po przecinku
            }
        });
        RuleFor(r => r.SortBy).Must(value => string.IsNullOrEmpty(value) || allowedSortByColumnNames.Contains(value))
                                .WithMessage($"Sort by is optional, or must be in [{string.Join(",", allowedSortByColumnNames)}]");
    }
}
