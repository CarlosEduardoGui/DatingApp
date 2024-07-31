using DatingApp.Entities;

namespace DatingApp.Dtos;

public class UserDto(string username, string token)
{
    public string Username { get; set; } = username;
    public string Token { get; set; } = token;

    public static UserDto FromAppUser(AppUser user, string token)
        => new(user.UserName, token);
}
