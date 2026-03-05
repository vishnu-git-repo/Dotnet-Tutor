using App.Common;
using App.Data;
using App.Models.Dtos;
using App.Models.Entities;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly AppDBContext _context;

    public UserController(AppDBContext context)
    {
        _context = context;
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAllClients()
    {
        var users = await _context.Users
    .Where(u => u.Role != UserRole.Admin)
    .OrderByDescending(u => u.Name)
    .Select(u => new UserResponseDto
    {
        Id = u.Id,
        Name = u.Name,
        Email = u.Email,
        Gender = u.Gender,
        Address = u.Address,
        Phone = u.Phone,
        Role = (int)u.Role,
        Status = (int)u.Status,
        CreatedAt = u.CreatedAt,
        UpdatedAt = u.UpdatedAt
    })
    .ToListAsync();


        return Ok(new ApiResponse<object>
        {
            Status = true,
            Message = "User list",
            Data = users
        });
    }

    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        try
        {
            var user = await _context.Users
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Gender = u.Gender,
                    Address = u.Address,
                    Phone = u.Phone,
                    Role = (int)u.Role,
                    Status = (int)u.Status,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt
                })
                .FirstOrDefaultAsync(u => u.Id == id);


            if (user == null)
                return NotFound(new ApiResponse<object>
                {
                    Status = false,
                    Message = "User not found",
                    Data = null
                });

            return Ok(new ApiResponse<object>
            {
                Status = true,
                Message = "User details",
                Data = user
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                Status = false,
                Message = "Internal Server Error : " + ex.Message,
                Data = null
            });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("filterclient")]
    public async Task<IActionResult> GetFilteredClient(GetFilteredClientDto dto)
    {
        try
        {
            var baseQuery = _context.Users
            .Where(u => u.Role != UserRole.Admin);
            var totalClientCount = await baseQuery.CountAsync();
            var filteredQuery = baseQuery.AsQueryable();

            if (dto.Status != 0)
            {
                filteredQuery = filteredQuery
                    .Where(u => u.Status == (UserStatus)dto.Status);
            }

            if (!string.IsNullOrWhiteSpace(dto.SearchString))
            {
                var search = dto.SearchString.Trim().ToLower();

                filteredQuery = filteredQuery.Where(u =>
                    (u.Name != null && u.Name.ToLower().Contains(search)) ||
                    (u.Email != null && u.Email.ToLower().Contains(search)) ||
                    (u.Phone != null && u.Phone.ToLower().Contains(search)) ||
                    // (u.Gender != null && u.Gender.Contains(search)) ||
                    (u.Address != null && u.Address.ToLower().Contains(search))
                );
            }

            var filteredCount = await filteredQuery.CountAsync();
            var filteredUsers = await filteredQuery
                .OrderByDescending(u => u.CreatedAt)
                .ThenByDescending(u => u.Id)
                .Skip((dto.PageNo - 1) * dto.RowCount)
                .Take(dto.RowCount)
                .Select(u => new
                {
                    id = u.Id,
                    name = u.Name,
                    email = u.Email,
                    gender = u.Gender,
                    address = u.Address,
                    phone = u.Phone,
                    role = u.Role,
                    status = u.Status,
                    createdAt = u.CreatedAt
                })
                .ToListAsync();

            var activeClient = await baseQuery
                .Where(u => u.Status == UserStatus.Active)
                .CountAsync();

            var blockedClient = await baseQuery
                .Where(u => u.Status == UserStatus.Inactive)
                .CountAsync();

            return Ok(new ApiResponse<Object>()
            {
                Status = true,
                Message = "Fetching Users Count : " + filteredCount,
                Data = new
                {
                    status = dto.Status,
                    rowCount = dto.RowCount,
                    pageNo = dto.PageNo,
                    totalCount = totalClientCount,
                    blockedCount = blockedClient,
                    activeCount = activeClient,
                    searchString = dto.SearchString,
                    users = filteredUsers
                }
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                Status = false,
                Message = "Internal Server Error : " + ex.Message,
                Data = null
            });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("create/client")]
    public async Task<IActionResult> CreateClient(RegisterDto dto)
    {
        try
        {
            var exists = await _context.Users.AnyAsync(u => u.Email == dto.Email);
            if (exists)
                return BadRequest(new ApiResponse<object>
                {
                    Status = false,
                    Message = "User already found",
                    Data = null
                });

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Gender = dto.Gender,
                Address = dto.Address,
                Phone = dto.Phone,
                Role = UserRole.Client,
                Status = UserStatus.Active
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Registration successful");
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                Status = false,
                Message = "Internal Server Error : " + ex.Message,
                Data = null
            });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("create/admin")]
    public async Task<IActionResult> CreateAdmin(RegisterDto dto)
    {
        try
        {
            var exists = await _context.Users.AnyAsync(u => u.Email == dto.Email);
            if (exists)
                return BadRequest("Already registered");

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Gender = dto.Gender,
                Address = dto.Address,
                Phone = dto.Phone,
                Role = UserRole.Client,
                Status = UserStatus.Active
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Registration successful");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);

            return StatusCode(500, "Something went wrong");
        }
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, UpdateUserDto dto)
    {
        try
        {
            var user = await _context.Users
            .FindAsync(int.Parse(id));

            if (user == null)
                return BadRequest(new ApiResponse<object>
                {
                    Status = false,
                    Message = "User not found",
                    Data = null
                });

            user.Name = dto.Name;
            user.Email = user.Email;
            user.PasswordHash = user.PasswordHash;
            user.Gender = dto.Gender;
            user.Phone = dto.Phone;
            user.Address = dto.Address;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Status = true,
                Message = $"{user.Name} details updated",
                Data = user
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                Status = false,
                Message = "Internal Server Error : " + ex.Message,
                Data = null
            });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("password/changeByAdmin/{id:int}")]
    public async Task<IActionResult> UpdatePasswordByAdmin(int id, UpdateUserPasswordDto dto)
    {
        try
        {
            var user = await _context.Users
            .FindAsync(id);

            if (user == null)
                return NotFound(new ApiResponse<object>
                {
                    Status = false,
                    Message = "User not found",
                    Data = null
                });
            user.Name = user.Name;
            user.Email = dto.Email;
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            user.Gender = user.Gender;
            user.Phone = user.Phone;
            user.Address = user.Address;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Status = true,
                Message = $"{user.Name} updated successfully",
                Data = null
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                Status = false,
                Message = "Internal Server Error : " + ex.Message,
                Data = null
            });
        }
    }


    [Authorize(Roles = "Client")]
    [HttpPut("password/changeByClient/{id:int}")]
    public async Task<IActionResult> UpdatePasswordByClient(int id, UpdateUserPasswordDto dto)
    {
        try
        {
            var user = await _context.Users
           .FindAsync(id);

            if (user == null)
                return NotFound(new ApiResponse<object>
                {
                    Status = false,
                    Message = "User not found",
                    Data = null
                });
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Status = false,
                    Message = "Password Mismatch",
                    Data = null
                });
            }
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Status = true,
                Message = "Password updated successfully",
                Data = null
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                Status = false,
                Message = "Internal Server Error : " + ex.Message,
                Data = null
            });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("disable/{id:int}")]
    public async Task<IActionResult> DisableUser(int id)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound(new ApiResponse<object>
                {
                    Status = false,
                    Message = "User not found",
                    Data = null
                });
            user.Status = UserStatus.Inactive;
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Status = true,
                Message = $"{user.Name} blocked successfully",
                Data = null
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                Status = false,
                Message = "Internal Server Error : " + ex.Message,
                Data = null
            });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("enable/{id:int}")]
    public async Task<IActionResult> EnableUser(int id)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound(new ApiResponse<object>
                {
                    Status = false,
                    Message = "User not found",
                    Data = null
                });
            user.Status = UserStatus.Active;
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<object>
            {
                Status = true,
                Message = $"{user.Name} unblocked successfully",
                Data = null
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                Status = false,
                Message = "Internal Server Error : " + ex.Message,
                Data = null
            });
        }
    }
}
