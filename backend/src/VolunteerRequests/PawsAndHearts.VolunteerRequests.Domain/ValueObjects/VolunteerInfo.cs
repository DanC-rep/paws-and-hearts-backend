using CSharpFunctionalExtensions;
using PawsAndHearts.SharedKernel.ValueObjects;

namespace PawsAndHearts.VolunteerRequests.Domain.ValueObjects;

public class VolunteerInfo : ValueObject
{
    public FullName FullName { get; }
    
    public PhoneNumber PhoneNumber { get; }
    
    public Experience Experience { get; }
    
    public IReadOnlyList<SocialNetwork> SocialNetworks { get; }

    private VolunteerInfo()
    {
        
    }

    public VolunteerInfo(
        FullName fullName,
        PhoneNumber phoneNumber,
        Experience experience,
        IEnumerable<SocialNetwork> socialNetworks)
    {
        FullName = fullName;
        PhoneNumber = phoneNumber;
        Experience = experience;
        SocialNetworks = socialNetworks.ToList();
    }
    
    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return FullName;
        yield return PhoneNumber;
        yield return Experience;
        foreach (var socialNetwork in SocialNetworks) yield return socialNetwork;
    }
}