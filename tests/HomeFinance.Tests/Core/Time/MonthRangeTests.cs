using HomeFinance.Core.Time;

namespace HomeFinance.Tests.Core.Time;

public sealed class MonthRangeTests
{
    // -------------------------------------------------------------------------
    // Construction
    // -------------------------------------------------------------------------

    [Fact]
    public void Constructor_January_SetsStartToFirstAndEndExclusiveToFebruary()
    {
        var range = new MonthRange(2024, 1);

        Assert.Equal(new DateOnly(2024, 1, 1), range.Start);
        Assert.Equal(new DateOnly(2024, 2, 1), range.EndExclusive);
    }

    [Fact]
    public void Constructor_December_RollsEndExclusiveIntoNextYear()
    {
        var range = new MonthRange(2024, 12);

        Assert.Equal(new DateOnly(2024, 12, 1), range.Start);
        Assert.Equal(new DateOnly(2025, 1, 1), range.EndExclusive);
    }

    [Fact]
    public void Constructor_FebruaryLeapYear_SetsCorrectBounds()
    {
        // 2024 is a leap year — February has 29 days
        var range = new MonthRange(2024, 2);

        Assert.Equal(new DateOnly(2024, 2, 1), range.Start);
        Assert.Equal(new DateOnly(2024, 3, 1), range.EndExclusive);
    }

    [Fact]
    public void Constructor_FebruaryNonLeapYear_SetsCorrectBounds()
    {
        // 2023 is not a leap year — February has 28 days
        var range = new MonthRange(2023, 2);

        Assert.Equal(new DateOnly(2023, 2, 1), range.Start);
        Assert.Equal(new DateOnly(2023, 3, 1), range.EndExclusive);
    }

    // -------------------------------------------------------------------------
    // Contains
    // -------------------------------------------------------------------------

    [Fact]
    public void Contains_DateInsideRange_ReturnsTrue()
    {
        var range = new MonthRange(2024, 3);

        var result = range.Contains(new DateOnly(2024, 3, 15));

        Assert.True(result);
    }

    [Fact]
    public void Contains_DateAtStart_ReturnsTrue()
    {
        var range = new MonthRange(2024, 3);

        var result = range.Contains(new DateOnly(2024, 3, 1));

        Assert.True(result);
    }

    [Fact]
    public void Contains_DateAtLastDayOfMonth_ReturnsTrue()
    {
        var range = new MonthRange(2024, 3);

        var result = range.Contains(new DateOnly(2024, 3, 31));

        Assert.True(result);
    }

    [Fact]
    public void Contains_DateAtEndExclusive_ReturnsFalse()
    {
        var range = new MonthRange(2024, 3);

        var result = range.Contains(range.EndExclusive);

        Assert.False(result);
    }

    [Fact]
    public void Contains_DateBeforeRange_ReturnsFalse()
    {
        var range = new MonthRange(2024, 3);

        var result = range.Contains(new DateOnly(2024, 2, 28));

        Assert.False(result);
    }

    [Fact]
    public void Contains_DateAfterRange_ReturnsFalse()
    {
        var range = new MonthRange(2024, 3);

        var result = range.Contains(new DateOnly(2024, 4, 1));

        Assert.False(result);
    }
}
