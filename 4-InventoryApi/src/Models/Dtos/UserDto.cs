namespace App.Models.Dtos;

public class UpdateUserDto
{
    public required string Gender { get; set; }
    public required string Address { get; set; }
    public required string Phone { get; set; }
}

public class UpdateUserPasswordDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
}

