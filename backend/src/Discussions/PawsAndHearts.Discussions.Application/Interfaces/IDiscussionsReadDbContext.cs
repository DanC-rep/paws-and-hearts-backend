using PawsAndHearts.Discussions.Contracts.Dtos;

namespace PawsAndHearts.Discussions.Application.Interfaces;

public interface IDiscussionsReadDbContext
{
    IQueryable<DiscussionDto> Discussions { get; }
    
    IQueryable<MessageDto> Messages { get; }
}