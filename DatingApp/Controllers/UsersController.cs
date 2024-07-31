using DatingApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Controllers;

[Authorize]
public class UsersController(DataContext context) : BaseApiController
{

    [HttpGet]
    public async Task<IActionResult> ListAllUsers(CancellationToken token)
    {
        var users = await context.Users.ToListAsync(token);

        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id, CancellationToken token)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken: token);

        return Ok(user);
    }
}
