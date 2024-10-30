using FluentAssertions;
using PawsAndHearts.SharedKernel.ValueObjects;
using PawsAndHearts.SharedKernel.ValueObjects.Ids;
using PawsAndHearts.VolunteerRequests.Domain.Entities;
using PawsAndHearts.VolunteerRequests.Domain.Enums;
using PawsAndHearts.VolunteerRequests.Domain.ValueObjects;

namespace PawsAndHearts.UnitTests;

public class VolunteerRequestTests
{
    [Fact]
    public void Create_Volunteer_Request_And_Approve_Successfully()
    {
        // arrange
        var adminId = Guid.NewGuid();
        var discussionId = Guid.NewGuid();

        var volunteerRequest = CreateVolunteerRequest();

        // act
        volunteerRequest.TakeRequestForSubmit(adminId, discussionId);
        volunteerRequest.Approve();

        // assert
        volunteerRequest.Status.Should().Be(VolunteerRequestStatus.Approved);
    }

    [Fact]
    public void Create_Volunteer_Request_And_Reject_Successfully()
    {
        // arrange
        var adminId = Guid.NewGuid();
        var discussionId = Guid.NewGuid();

        var volunteerRequest = CreateVolunteerRequest();

        // act
        volunteerRequest.TakeRequestForSubmit(adminId, discussionId);
        volunteerRequest.Reject("comment");

        // assert
        volunteerRequest.Status.Should().Be(VolunteerRequestStatus.Rejected);
    }

    [Fact]
    public void Create_Volunteer_Request_Send_On_Revision_Resend_Approve_Successfully()
    {
        // arrange
        var adminId = Guid.NewGuid();
        var discussionId = Guid.NewGuid();

        var volunteerRequest = CreateVolunteerRequest();

        // act
        volunteerRequest.TakeRequestForSubmit(adminId, discussionId);
        volunteerRequest.SendForRevision("comment");
        volunteerRequest.ResendVolunteerRequest();
        volunteerRequest.Approve();

        // assert
        volunteerRequest.Status.Should().Be(VolunteerRequestStatus.Approved);
    }
    
    [Fact]
    public void Create_Volunteer_Request_Send_On_Revision_Resend_Reject_Successfully()
    {
        // arrange
        var adminId = Guid.NewGuid();
        var discussionId = Guid.NewGuid();

        var volunteerRequest = CreateVolunteerRequest();

        // act
        volunteerRequest.TakeRequestForSubmit(adminId, discussionId);
        volunteerRequest.SendForRevision("comment");
        volunteerRequest.ResendVolunteerRequest();
        volunteerRequest.Reject("comment");

        // assert
        volunteerRequest.Status.Should().Be(VolunteerRequestStatus.Rejected);
    }
    
    private static VolunteerRequest CreateVolunteerRequest()
    {
        var createdAt = DateTime.UtcNow;

        var volunteerRequestId = VolunteerRequestId.NewId();
        
        var userId = Guid.NewGuid();

        var volunteerInfo = new VolunteerInfo(
            FullName.Create("abc", "abc", "anc").Value,
            PhoneNumber.Create("89206679987").Value,
            Experience.Create(10).Value,
            new List<SocialNetwork>());

        var volunteerRequest = new VolunteerRequest(
            volunteerRequestId,
            userId,
            createdAt,
            volunteerInfo,
            VolunteerRequestStatus.Waiting);

        return volunteerRequest;
    }
}