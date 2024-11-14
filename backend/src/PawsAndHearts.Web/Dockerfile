FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/PawsAndHearts.Web/PawsAndHearts.Web.csproj", "src/PawsAndHearts.Web/"]
COPY ["src/BreedManagement/PawsAndHearts.BreedManagement.Application/PawsAndHearts.BreedManagement.Application.csproj", "src/BreedManagement/PawsAndHearts.BreedManagement.Application/"]
COPY ["src/PetManagement/PawsAndHearts.PetManagement.Contracts/PawsAndHearts.PetManagement.Contracts.csproj", "src/PetManagement/PawsAndHearts.PetManagement.Contracts/"]
COPY ["src/Shared/PawsAndHearts.Core/PawsAndHearts.Core.csproj", "src/Shared/PawsAndHearts.Core/"]
COPY ["src/Shared/PawsAndHearts.SharedKernel/PawsAndHearts.SharedKernel.csproj", "src/Shared/PawsAndHearts.SharedKernel/"]
COPY ["src/Shared/PawsAndHearts.Framework/PawsAndHearts.Framework.csproj", "src/Shared/PawsAndHearts.Framework/"]
COPY ["src/Accounts/PawsAndHearts.Accounts.Contracts/PawsAndHearts.Accounts.Contracts.csproj", "src/Accounts/PawsAndHearts.Accounts.Contracts/"]
COPY ["src/BreedManagement/PawsAndHearts.BreedManagement.Domain/PawsAndHearts.BreedManagement.Domain.csproj", "src/BreedManagement/PawsAndHearts.BreedManagement.Domain/"]
COPY ["src/BreedManagement/PawsAndHearts.BreedManagement.Contracts/PawsAndHearts.BreedManagement.Contracts.csproj", "src/BreedManagement/PawsAndHearts.BreedManagement.Contracts/"]
COPY ["src/BreedManagement/PawsAndHearts.BreedManagement.Infrastructure/PawsAndHearts.BreedManagement.Infrastructure.csproj", "src/BreedManagement/PawsAndHearts.BreedManagement.Infrastructure/"]
COPY ["src/BreedManagement/PawsAndHearts.BreedManagement.Presentation/PawsAndHearts.BreedManagement.Presentation.csproj", "src/BreedManagement/PawsAndHearts.BreedManagement.Presentation/"]
COPY ["src/PetManagement/PawsAndHearts.PetManagement.Application/PawsAndHearts.PetManagement.Application.csproj", "src/PetManagement/PawsAndHearts.PetManagement.Application/"]
COPY ["src/PetManagement/PawsAndHearts.PetManagement.Domain/PawsAndHearts.PetManagement.Domain.csproj", "src/PetManagement/PawsAndHearts.PetManagement.Domain/"]
COPY ["src/PetManagement/PawsAndHearts.PetManagement.Infrastructure/PawsAndHearts.PetManagement.Infrastructure.csproj", "src/PetManagement/PawsAndHearts.PetManagement.Infrastructure/"]
COPY ["src/PetManagement/PawsAndHearts.PetManagement.Presentation/PawsAndHearts.PetManagement.Presentation.csproj", "src/PetManagement/PawsAndHearts.PetManagement.Presentation/"]
COPY ["src/Accounts/PawsAndHearts.Accounts.Infrastructure/PawsAndHearts.Accounts.Infrastructure.csproj", "src/Accounts/PawsAndHearts.Accounts.Infrastructure/"]
COPY ["src/Accounts/PawsAndHearts.Accounts.Application/PawsAndHearts.Accounts.Application.csproj", "src/Accounts/PawsAndHearts.Accounts.Application/"]
COPY ["src/Accounts/PawsAndHearts.Accounts.Domain/PawsAndHearts.Accounts.Domain.csproj", "src/Accounts/PawsAndHearts.Accounts.Domain/"]
COPY ["src/Accounts/PawsAndHearts.Accounts.Presentation/PawsAndHearts.Accounts.Presentation.csproj", "src/Accounts/PawsAndHearts.Accounts.Presentation/"]
COPY ["src/Discussions/PawsAndHearts.Discussions.Infrastructure/PawsAndHearts.Discussions.Infrastructure.csproj", "src/Discussions/PawsAndHearts.Discussions.Infrastructure/"]
COPY ["src/Discussions/PawsAndHearts.Discussions.Application/PawsAndHearts.Discussions.Application.csproj", "src/Discussions/PawsAndHearts.Discussions.Application/"]
COPY ["src/Discussions/PawsAndHearts.Discussions.Domain/PawsAndHearts.Discussions.Domain.csproj", "src/Discussions/PawsAndHearts.Discussions.Domain/"]
COPY ["src/Discussions/PawsAndHearts.Discussions.Contracts/PawsAndHearts.Discussions.Contracts.csproj", "src/Discussions/PawsAndHearts.Discussions.Contracts/"]
COPY ["src/Discussions/PawsAndHearts.Discussions.Presentation/PawsAndHearts.Discussions.Presentation.csproj", "src/Discussions/PawsAndHearts.Discussions.Presentation/"]
COPY ["src/VolunteerRequests/PawsAndHearts.VolunteerRequests.Infrastructure/PawsAndHearts.VolunteerRequests.Infrastructure.csproj", "src/VolunteerRequests/PawsAndHearts.VolunteerRequests.Infrastructure/"]
COPY ["src/VolunteerRequests/PawsAndHearts.VolunteerRequests.Application/PawsAndHearts.VolunteerRequests.Application.csproj", "src/VolunteerRequests/PawsAndHearts.VolunteerRequests.Application/"]
COPY ["src/VolunteerRequests/PawsAndHearts.VolunteerRequests.Contracts/PawsAndHearts.VolunteerRequests.Contracts.csproj", "src/VolunteerRequests/PawsAndHearts.VolunteerRequests.Contracts/"]
COPY ["src/VolunteerRequests/PawsAndHearts.VolunteerRequests.Domain/PawsAndHearts.VolunteerRequests.Domain.csproj", "src/VolunteerRequests/PawsAndHearts.VolunteerRequests.Domain/"]
COPY ["src/VolunteerRequests/PawsAndHearts.VolunteerRequests.Presentation/PawsAndHearts.VolunteerRequests.Presentation.csproj", "src/VolunteerRequests/PawsAndHearts.VolunteerRequests.Presentation/"]
RUN dotnet restore "src/PawsAndHearts.Web/PawsAndHearts.Web.csproj"
COPY . .
WORKDIR "/src/src/PawsAndHearts.Web"
RUN dotnet build "PawsAndHearts.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PawsAndHearts.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PawsAndHearts.Web.dll"]
