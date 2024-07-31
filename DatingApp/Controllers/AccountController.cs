using System.Security.Cryptography;
using System.Text;
using DatingApp.Data;
using DatingApp.Dtos;
using DatingApp.Entities;
using DatingApp.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Controllers;

public class AccountController(
    DataContext context,
    ITokenService tokenService
) : BaseApiController
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (await UserExists(dto.UserName)) return BadRequest("User is already taken.");

        using var hmac = new HMACSHA512();

        var user = new AppUser()
        {
            UserName = dto.UserName.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)),
            PasswordSalt = hmac.Key
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        return Ok(UserDto.FromAppUser(user, tokenService.CreateToken(user)));
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginDto login)
    {
        var user = await context
                        .Users
                        .SingleOrDefaultAsync(x => x.UserName == login.UserName.ToLower());

        if (user is null) return Unauthorized("Invalid user.");

        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(login.Password));

        for (int i = 0; i < computedHash.Length; i++)
        {
            if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Password is not valid.");
        }

        return Ok(UserDto.FromAppUser(user, tokenService.CreateToken(user)));
    }

    private async Task<bool> UserExists(string userName)
        => await context.Users.AnyAsync(x => x.UserName == userName.ToLower());
}