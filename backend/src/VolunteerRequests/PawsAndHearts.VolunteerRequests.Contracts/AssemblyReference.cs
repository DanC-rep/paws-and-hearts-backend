using System.Reflection;

namespace PawsAndHearts.VolunteerRequests.Contracts;

public static class AssemblyReference
{
    public static Assembly Assembly => typeof(AssemblyReference).Assembly;
}