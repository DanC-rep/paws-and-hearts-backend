using CSharpFunctionalExtensions;

namespace PawsAndHearts.SharedKernel.ValueObjects;

public class SocialNetwork : ValueObject
{
    private SocialNetwork(string link, string name)
    {
        Link = link;
        Name = name;
    }

    private SocialNetwork()
    {
        
    }
    
    public string Link { get; } = default!;

    public string Name { get; } = default!;

    public static Result<SocialNetwork, Error> Create(string link, string name)
    {
        if (string.IsNullOrWhiteSpace(link))
            return Errors.General.ValueIsRequired("link");

        if (string.IsNullOrWhiteSpace(name))
            return Errors.General.ValueIsRequired("name");
                
        return new SocialNetwork(link, name);
    }

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Link;
        yield return Name;
    }
}