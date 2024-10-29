using PawsAndHearts.Core.Abstractions;

namespace PawsAndHearts.Accounts.Application.Queries.GetUserById;

public record GetUserByIdQuery(Guid UserId) : IQuery;