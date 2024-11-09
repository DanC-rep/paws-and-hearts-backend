using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Dtos;

namespace PawsAndHearts.Accounts.Application.UseCases.CreateVolunteerAccount;

public record CreateVolunteerAccountCommand(
    Guid UserId, 
    int Experience, 
    IEnumerable<RequisiteDto> Requisites) : ICommand;