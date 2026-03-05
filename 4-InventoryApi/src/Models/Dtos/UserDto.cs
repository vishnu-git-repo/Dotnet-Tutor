namespace App.Models.Dtos;

public class UpdateUserDto
{
    public required string Name { get; set; }
    public required string Gender { get; set; }
    public required string Address { get; set; }
    public required string Phone { get; set; }
}

public class UpdateUserPasswordDto
{   
    public required string Email { get; set; }
    public string? Password { get; set; }
    public required string NewPassword { get; set; }
}

public class GetFilteredClientDto
{
    public required int Status {get; set;}
    public required int RowCount {get; set;}
    public required int PageNo {get; set;}
    public int? TotalCount {get; set;}
    public int? BlockedCount {get; set;}
    public int? ActiveCount {get; set;}
    public string? SearchString {get; set;}
}



public class UserResponseDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public string? Gender { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public int Role { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ForgotPasswordDto
{
    public string Email { get; set; } = "";
}

public class VerifyOtpDto
{
    public string Email { get; set; } = "";
    public string Otp { get; set; } = "";
}

public class ResetPasswordDto
{
    public string Email { get; set; } = "";
    public string Otp { get; set; } = "";
    public string NewPassword { get; set; } = "";
}
