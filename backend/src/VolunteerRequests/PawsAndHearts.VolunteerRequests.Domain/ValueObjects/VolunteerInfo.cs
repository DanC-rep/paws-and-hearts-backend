using CSharpFunctionalExtensions;
using PawsAndHearts.SharedKernel.ValueObjects;

namespace PawsAndHearts.VolunteerRequests.Domain.ValueObjects;

public class VolunteerInfo : ValueObject
{
    public Experience Experience { get; }
    
    public IReadOnlyList<Requisite> Requisites { get; }

    private VolunteerInfo()
    {
        
    }

    public VolunteerInfo(
        Experience experience,
        IEnumerable<Requisite> requisites)
    {
        Experience = experience;
        Requisites = requisites.ToList();
    }
    
    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Experience;
        foreach (var requisite in Requisites) yield return requisite;
    }
}