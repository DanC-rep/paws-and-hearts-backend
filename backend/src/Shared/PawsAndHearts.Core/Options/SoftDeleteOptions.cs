namespace PawsAndHearts.Core.Options;

public class SoftDeleteOptions
{
    public const string SOFT_DELETE = "SoftDelete";

    public int ExpiredDaysTime { get; init; } = default!;
}