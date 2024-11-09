using PawsAndHearts.SharedKernel.ValueObjects;

namespace PawsAndHearts.Accounts.Domain;

public class VolunteerAccount
{
    private VolunteerAccount()
    {
        
    }

    public VolunteerAccount(User user, Experience experience, IEnumerable<Requisite> requisites)
    {
        User = user;
        Experience = experience;
        Requisites = requisites.ToList();
    }
    
    public Guid Id { get; set; }
    
    public Experience Experience { get; set; }
    
    public IReadOnlyList<Requisite> Requisites { get; set; }

    private readonly List<string> certificates = [];

    public IReadOnlyList<string> Certificates => certificates;
    
    public User User { get; set; }
    
    public Guid UserId { get; set; }

    public void UpdateRequisites(List<Requisite> requisites)
    {
        Requisites = requisites;
    }
}