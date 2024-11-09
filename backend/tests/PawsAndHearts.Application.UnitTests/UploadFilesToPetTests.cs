using System.Data.Common;
using CSharpFunctionalExtensions;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using PawsAndHearts.Core.Abstractions;
using PawsAndHearts.Core.Dtos;
using PawsAndHearts.Core.Messaging;
using PawsAndHearts.PetManagement.Application.Interfaces;
using PawsAndHearts.PetManagement.Application.UseCases.AddPhotosToPet;
using PawsAndHearts.PetManagement.Domain.Entities;
using PawsAndHearts.PetManagement.Domain.Enums;
using PawsAndHearts.PetManagement.Domain.ValueObjects;
using PawsAndHearts.SharedKernel;
using PawsAndHearts.SharedKernel.Enums;
using PawsAndHearts.SharedKernel.FileProvider;
using PawsAndHearts.SharedKernel.Interfaces;
using PawsAndHearts.SharedKernel.ValueObjects;
using PawsAndHearts.SharedKernel.ValueObjects.Ids;
using FileInfo = PawsAndHearts.SharedKernel.FileProvider.FileInfo;

namespace PawsAndHearts.Application.UnitTests;

public class UploadFilesToPetTests
{
    private readonly Mock<IFileProvider> _fileProviderMock = new();
    private readonly Mock<IVolunteersRepository> _volunteersRepositoryMock = new();
    private readonly Mock<ILogger<AddPhotosToPetHandler>> _loggerMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IValidator<AddPhotosToPetCommand>> _validatorMock = new();
    private readonly Mock<IMessageQueue<IEnumerable<FileInfo>>> _messageQueueMock = new();
    
    [Fact]
    public async Task Handle_Should_Upload_Files_To_Pet()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;
        
        var volunteer = CreateVolunteerWithPets(1);
        var pet = volunteer.Pets[0];
        
        var command = new AddPhotosToPetCommand(volunteer.Id, pet.Id, CreateFileDtos());
        
        var fileName = "123.jpg";

        List<FilePath> filePaths =
        [
            FilePath.Create(fileName).Value,
            FilePath.Create(fileName).Value
        ];
        
        _fileProviderMock.Setup(f => 
            f.UploadFiles(It.IsAny<List<UploadFileData>>(), cancellationToken))
            .ReturnsAsync(Result.Success<FilePathList, Error>(filePaths));
        
        _volunteersRepositoryMock.Setup(v =>
             v.GetById(volunteer.Id, cancellationToken))
            .ReturnsAsync(volunteer);
        
        var dbTransactionMock = new Mock<DbTransaction>();
        
        _unitOfWorkMock.Setup(u => 
             u.SaveChanges(cancellationToken))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u =>
             u.BeginTransaction(cancellationToken))
            .ReturnsAsync(dbTransactionMock.Object);
        
        _validatorMock.Setup(v =>
             v.ValidateAsync(command, cancellationToken))
            .ReturnsAsync(new ValidationResult());

        var handler = new AddPhotosToPetHandler(
            _volunteersRepositoryMock.Object,
            _loggerMock.Object,
            _fileProviderMock.Object,
            _validatorMock.Object,
            _unitOfWorkMock.Object,
            _messageQueueMock.Object);
        
        // act
        var result = await handler.Handle(command, cancellationToken);

        // assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Equal((FilePathList)filePaths);
    }

    [Fact]
    public async Task Handle_Should_Return_Error_When_Volunteer_Does_Not_Exists()
    {
        // arrange
        var cancellationToken = new CancellationTokenSource().Token;
        
        var volunteerId = VolunteerId.NewId();
        var petId = PetId.NewId();
        
        var command = new AddPhotosToPetCommand(volunteerId, petId, CreateFileDtos());
        
        _volunteersRepositoryMock.Setup(v =>
                v.GetById(volunteerId, cancellationToken))
            .ReturnsAsync(Errors.General.NotFound(volunteerId));
        
        _validatorMock.Setup(v =>
                v.ValidateAsync(command, cancellationToken))
            .ReturnsAsync(new ValidationResult());

        var handler = new AddPhotosToPetHandler(
            _volunteersRepositoryMock.Object,
            _loggerMock.Object,
            _fileProviderMock.Object,
            _validatorMock.Object,
            _unitOfWorkMock.Object,
            _messageQueueMock.Object);
        
        // act
        var result = await handler.Handle(command, cancellationToken);

        // assert
        result.IsFailure.Should().BeTrue();
        
        var error = result.Error.First();
        error.Type.Should().Be(ErrorType.NotFound);
        error.Code.Should().Be("record.not.found");
    }

    [Fact]
    public async Task Handle_Should_Return_Validation_Errors()
    {
        // act
        var cancellationToken = new CancellationTokenSource().Token;

        var volunteerId = VolunteerId.NewId();
        var petId = PetId.NewId();

        var command = new AddPhotosToPetCommand(volunteerId, petId, CreateFileDtos());

        var errorValidation = Errors.General.ValueIsInvalid(nameof(command.Files)).Serialize();

        var validationFailures = new List<ValidationFailure>
        {
            new(nameof(command.Files), errorValidation)
        };

        var validationResult = new ValidationResult(validationFailures);
        
        _validatorMock.Setup(v =>
                v.ValidateAsync(command, cancellationToken))
            .ReturnsAsync(validationResult);

        var handler = new AddPhotosToPetHandler(
            _volunteersRepositoryMock.Object,
            _loggerMock.Object,
            _fileProviderMock.Object,
            _validatorMock.Object,
            _unitOfWorkMock.Object,
            _messageQueueMock.Object);
        
        // arrange
        var result = await handler.Handle(command, cancellationToken);
        
        // assert
        result.IsFailure.Should().BeTrue();

        var error = result.Error.First();
        error.Type.Should().Be(ErrorType.Validation);
        error.Code.Should().Be("value.is.invalid");
        error.InvalidField.Should().Be(nameof(command.Files));
    }

    [Fact]
    public async Task Handle_Should_Return_File_Upload_Error()
    {
        // act
        var cancellationToken = new CancellationTokenSource().Token;

        var volunteer = CreateVolunteerWithPets(1);
        var pet = volunteer.Pets[0];
        
        var command = new AddPhotosToPetCommand(volunteer.Id, pet.Id, CreateFileDtos());
        
        var fileName = "123.jpg";

        List<FilePath> filePaths =
        [
            FilePath.Create(fileName).Value,
            FilePath.Create(fileName).Value
        ];
        
        _fileProviderMock.Setup(f => 
                f.UploadFiles(It.IsAny<List<UploadFileData>>(), cancellationToken))
            .ReturnsAsync(Error.Failure("file.upload", "Fail to upload file in minio"));

        _messageQueueMock.Setup(m =>
            m.WriteAsync(It.IsAny<List<FileInfo>>(), cancellationToken));
        
        _volunteersRepositoryMock.Setup(v =>
                v.GetById(volunteer.Id, cancellationToken))
            .ReturnsAsync(volunteer);
        
        var dbTransactionMock = new Mock<DbTransaction>();
        
        _unitOfWorkMock.Setup(u => 
                u.SaveChanges(cancellationToken))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u =>
                u.BeginTransaction(cancellationToken))
            .ReturnsAsync(dbTransactionMock.Object);
        
        _validatorMock.Setup(v =>
                v.ValidateAsync(command, cancellationToken))
            .ReturnsAsync(new ValidationResult());

        var handler = new AddPhotosToPetHandler(
            _volunteersRepositoryMock.Object,
            _loggerMock.Object,
            _fileProviderMock.Object,
            _validatorMock.Object,
            _unitOfWorkMock.Object,
            _messageQueueMock.Object);
        
        // act
        var result = await handler.Handle(command, cancellationToken);

        // assert
        result.IsFailure.Should().BeTrue();

        var error = result.Error.First();
        error.Type.Should().Be(ErrorType.Failure);
        error.Code.Should().Be("file.upload");
    }

    private IEnumerable<UploadFileDto> CreateFileDtos()
    {
        var stream = new MemoryStream();
        var fileName = "123.jpg";

        var uploadFileDto = new UploadFileDto(stream, fileName);

        List<UploadFileDto> files = [uploadFileDto, uploadFileDto];

        return files;
    }
    
    private Volunteer CreateVolunteerWithPets(int petsCount)
    {
        var petId = PetId.NewId();
        var fullName = FullName.Create("123", "123", "123").Value;
        var experience = Experience.Create(1).Value;
        var phoneNumber = PhoneNumber.Create("89207741189").Value;
        var petIdentity = new PetIdentity(SpeciesId.Empty(), Guid.Empty);
        var color = Color.Create("123").Value;
        var address = Address.Create("123", "123", "123", "213").Value;
        var petMetrics = PetMetrics.Create(123, 213).Value;
        var birthDate = BirthDate.Create(DateTime.Now).Value;
        var creationDate = CreationDate.Create(DateTime.Now).Value;
        
        var requisites = new ValueObjectList<Requisite>(
            [Requisite.Create("123", "123").Value]);
        
        var volunteer = new Volunteer(
            VolunteerId.NewId(),
            fullName,
            experience,
            phoneNumber);

        for (int i = 0; i < petsCount; i++)
        {
            var pet = new Pet(
                petId,
                "123",
                "123",
                petIdentity,
                color,
                "norm",
                address,
                petMetrics,
                phoneNumber,
                true,
                birthDate,
                true,
                HelpStatus.FoundHome,
                creationDate,
                requisites);

            volunteer.AddPet(pet);
        }

        return volunteer;
    }
}