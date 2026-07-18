using HomeFinance.Web.Authentication;

namespace HomeFinance.Tests.Web.Authentication;

public sealed class SeededUsersOptionsTests
{
    [Fact]
    public void SectionName_HasExpectedValue()
    {
        Assert.Equal("Authentication:SeededUsers", SeededUsersOptions.SectionName);
    }
}
