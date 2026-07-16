namespace HomeFinance.Core.Time;

public readonly struct MonthRange
{
    public DateOnly Start { get; }
    public DateOnly EndExclusive { get; }

    public MonthRange(int year, int month)
    {
        Start = new DateOnly(year, month, 1);
        EndExclusive = Start.AddMonths(1);
    }

    public bool Contains(DateOnly date) => date >= Start && date < EndExclusive;
}
