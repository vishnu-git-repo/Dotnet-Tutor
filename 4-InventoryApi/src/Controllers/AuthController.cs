using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using App.Entities;
using App.Dtos;
using App.Data;
using App.Services;
using System.Runtime.CompilerServices;

namespace App.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDBContext _context;
    private readonly JwtService _jwt;

    public AuthController (AppDBContext context, JwtService jwt)
    {
        _context = context;
        _jwt = jwt;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register (RegisterDto dto)
    {
        
    }

}
