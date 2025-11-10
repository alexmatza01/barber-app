namespace BarberApp.API.DTOs;

public class UpdateBarberInfoDto
{
    public string? Name { get; set; }
    public string? Bio { get; set; }
    public string? ProfileImage { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? GoogleCalendarId { get; set; }
}

public class CreatePortfolioItemDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public int Order { get; set; }
}

public class UpdatePortfolioItemDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int? Order { get; set; }
}

public class CreateServiceTypeDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int DurationMinutes { get; set; }
}

public class UpdateServiceTypeDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? Price { get; set; }
    public int? DurationMinutes { get; set; }
    public bool? IsActive { get; set; }
}
