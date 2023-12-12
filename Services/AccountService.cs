using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace RestaurantAPI.Services;

public class AccountService : IAccountService
{
    private readonly RestaurantDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly AuthenticationSettings _authenticationSettings;

    public AccountService(RestaurantDbContext dbContext, IMapper mapper, IPasswordHasher<User> passwordHasher, AuthenticationSettings authenticationSettings)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _authenticationSettings = authenticationSettings;
    }

    public int Register(RegisterUserDto dto)
    {
        var user = _mapper.Map<User>(dto);
        user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

        _dbContext.Users.Add(user);
        _dbContext.SaveChanges();

        return user.Id;
    }

    public string GenerateJwt(LoginDto dto)
    {
        var user = _dbContext.Users.Include(u => u.Role).FirstOrDefault(u => u.Email == dto.Email);
        if(user is null)
        {
            throw new BadRequestException("Invalid email or password");
        }
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if(result == PasswordVerificationResult.Failed)
        {
            throw new BadRequestException("Invalid email or password");
        }

        string dateOfBirth = user.DateOfBirth.HasValue ? user.DateOfBirth.Value.ToString("yyyy-MM-dd") : DateTime.Now.ToString("yyyy-MM-dd");

        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new Claim(ClaimTypes.Role, $"{user.Role.Name}"),
            new Claim("DateOfBirth", dateOfBirth)
        };
        
        if(!string.IsNullOrEmpty(user.Nationality) )
        {
            claims.Add(new Claim("Nationality", user.Nationality));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
        int check = key.KeySize;
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddDays(_authenticationSettings.JwtExpireDays);

        var token = new JwtSecurityToken(_authenticationSettings.JwtIssuer, _authenticationSettings.JwtIssuer, claims, expires: expires, signingCredentials: cred);

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }
}
