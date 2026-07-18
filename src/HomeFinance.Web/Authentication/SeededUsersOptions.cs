namespace HomeFinance.Web.Authentication;

public sealed record SeededUsersOptions
{
    public const string SectionName = "Authentication:SeededUsers";

    public IReadOnlyList<SeededUser> Users { get; init; } = [];
}

public sealed record SeededUser
{
    public required string UserName { get; init; }
    public required string DisplayName { get; init; }
    public required string Password { get; init; }
}
