using CSharpFunctionalExtensions;
using PawsAndHearts.SharedKernel;
using PawsAndHearts.SharedKernel.ValueObjects.Ids;
using PawsAndHearts.VolunteerRequests.Domain.Enums;
using PawsAndHearts.VolunteerRequests.Domain.Events;
using PawsAndHearts.VolunteerRequests.Domain.ValueObjects;

namespace PawsAndHearts.VolunteerRequests.Domain.Entities;

public class VolunteerRequest : DomainEntity<VolunteerRequestId>
{
    private VolunteerRequest(VolunteerRequestId id) : base(id)
    {
        
    }
    
    public VolunteerRequest(
        VolunteerRequestId id,
        Guid userId,
        DateTime createdAt,
        VolunteerInfo volunteerInfo,
        VolunteerRequestStatus status) : base(id)
    {
        UserId = userId;
        CreatedAt = createdAt;
        VolunteerInfo = volunteerInfo;
        Status = status;
        RejectionComment = RejectionComment.Create("No comment").Value;
    }

    public static VolunteerRequest CreateRequest(
        VolunteerRequestId volunteerRequestId,
        Guid userId,
        VolunteerInfo volunteerInfo) =>
        new(
            volunteerRequestId, 
            userId, 
            DateTime.UtcNow, 
            volunteerInfo,
            VolunteerRequestStatus.Waiting);

    public UnitResult<Error> TakeRequestForSubmit(Guid adminId, Guid discussionId)
    {
        if (Status != VolunteerRequestStatus.Waiting)
            Errors.General.ValueIsInvalid("volunteer request status");
        
        AdminId = adminId;
        DiscussionId = discussionId;
        Status = VolunteerRequestStatus.Submitted;

        return Result.Success<Error>();
    }

    public UnitResult<Error> SendForRevision(RejectionComment rejectionComment)
    {
        if (Status != VolunteerRequestStatus.Submitted)
            return Errors.General.ValueIsInvalid("volunteer request status");

        RejectionComment = rejectionComment;
        Status = VolunteerRequestStatus.Revision;

        return Result.Success<Error>();
    }

    public UnitResult<Error> Reject(RejectionComment rejectionComment)
    {
        if (Status != VolunteerRequestStatus.Submitted)
            return Errors.General.ValueIsInvalid("volunteer request status");

        RejectionComment = rejectionComment;
        Status = VolunteerRequestStatus.Rejected;

        var @event = new VolunteerRequestRejectedEvent(UserId);
        
        AddDomainEvent(@event);

        return Result.Success<Error>();
    }

    public UnitResult<Error> Approve()
    {
        if (Status != VolunteerRequestStatus.Submitted)
            return Errors.General.ValueIsInvalid("volunteer request status");
        
        Status = VolunteerRequestStatus.Approved;

        return Result.Success<Error>();
    }


    public UnitResult<Error> ResendVolunteerRequest()
    {
        if (Status != VolunteerRequestStatus.Revision)
            return Errors.General.ValueIsInvalid("volunteer request status");
        
        Status = VolunteerRequestStatus.Submitted;

        return Result.Success<Error>();
    }

    public UnitResult<Error> Update(VolunteerInfo volunteerInfo)
    {
        if (Status != VolunteerRequestStatus.Revision)
            return Errors.General.ValueIsInvalid("volunteer request status");
        
        VolunteerInfo = volunteerInfo;

        return Result.Success<Error>();
    }
    
    public Guid UserId { get; private set; }
    
    public Guid AdminId { get; private set; }
    
    public Guid DiscussionId {get; private set; }
    
    public VolunteerInfo VolunteerInfo { get; private set; }
    
    public VolunteerRequestStatus Status { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    
    public RejectionComment RejectionComment { get; private set; }
}