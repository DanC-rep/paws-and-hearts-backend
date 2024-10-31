using CSharpFunctionalExtensions;

namespace PawsAndHearts.Discussions.Domain.ValueObjects;

public class Users : ValueObject
{
    public Guid FirstMember { get; }
    
    public Guid SecondMemeber { get; }

    private Users()
    {
    }

    public Users(Guid firstMember, Guid secondMember)
    {
        FirstMember = firstMember;
        SecondMemeber = secondMember;
    }


    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return FirstMember;
        yield return SecondMemeber;
    }
}