namespace DfE.FindInformationAcademiesTrusts.Data.Repositories.PupilCensus;

public record Statistic<T>
{
    public static readonly Statistic<T> Suppressed = new(StatisticKind.Suppressed);
    public static readonly Statistic<T> NotPublished = new(StatisticKind.NotPublished);
    public static readonly Statistic<T> NotApplicable = new(StatisticKind.NotApplicable);
    public static readonly Statistic<T> NotAvailable = new(StatisticKind.NotAvailable);
    public static readonly Statistic<T> NotYetSubmitted = new(StatisticKind.NotYetSubmitted);

    public StatisticKind Kind { get; }

    private Statistic(StatisticKind Kind)
    {
        this.Kind = Kind;
    }

    public virtual bool TryGetValue(out T? value)
    {
        value = default;
        return false;
    }

    public static Statistic<T> FromKind(StatisticKind kind) => kind switch
    {
        StatisticKind.Suppressed => Suppressed,
        StatisticKind.NotPublished => NotPublished,
        StatisticKind.NotApplicable => NotApplicable,
        StatisticKind.NotAvailable => NotAvailable,
        StatisticKind.NotYetSubmitted => NotYetSubmitted,
        StatisticKind.WithValue => throw new ArgumentException(
            "WithValue can't be constructed without a value. Use the WithValue constructor instead of this method."
        ),
        _ => throw new ArgumentException($"Unknown statistic kind '{kind}'.")
    };

    public record WithValue(T Value) : Statistic<T>(StatisticKind.WithValue)
    {
        public override bool TryGetValue(out T? value)
        {
            value = Value;
            return true;
        }
    }
}

public enum StatisticKind
{
    WithValue,
    Suppressed,
    NotPublished,
    NotApplicable,
    NotAvailable,
    NotYetSubmitted,
}

public static class StatisticExtensions
{
    public static Statistic<TResult> Compute<T, TResult>(this Statistic<T> statistic, Func<T, TResult> computeFunc)
    {
        if (statistic is Statistic<T>.WithValue value)
        {
            return new Statistic<TResult>.WithValue(computeFunc(value.Value));
        }

        return Statistic<TResult>.FromKind(statistic.Kind);
    }

    public static Statistic<TResult> Compute<T1, T2, TResult>(this Statistic<T1> statistic, Statistic<T2> other,
        Func<T1, T2, TResult> computeFunc)
    {
        if (statistic is Statistic<T1>.WithValue value)
        {
            return other.Compute(s2 => computeFunc(value.Value, s2));
        }
        
        return Statistic<TResult>.FromKind(statistic.Kind);
    }
}