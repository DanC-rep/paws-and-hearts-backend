docker-compose up -d

dotnet-ef database drop -f -c WriteDbContext -p .\src\PetManagement\PawsAndHearts.PetManagement.Infrastructure\ -s .\src\PawsAndHearts.Web\

dotnet-ef migrations remove -c AccountsWriteDbContext -p .\src\Accounts\PawsAndHearts.Accounts.Infrastructure\ -s .\src\PawsAndHearts.Web\
dotnet-ef migrations remove -c WriteDbContext -p .\src\PetManagement\PawsAndHearts.PetManagement.Infrastructure\ -s .\src\PawsAndHearts.Web\
dotnet-ef migrations remove -c WriteDbContext -p .\src\BreedManagement\PawsAndHearts.BreedManagement.Infrastructure\ -s .\src\PawsAndHearts.Web\

dotnet-ef migrations add PetManagement_Initial -c WriteDbContext -p .\src\PetManagement\PawsAndHearts.PetManagement.Infrastructure\ -s .\src\PawsAndHearts.Web\
dotnet-ef migrations add BreedManagement_Initial -c WriteDbContext -p .\src\BreedManagement\PawsAndHearts.BreedManagement.Infrastructure\ -s .\src\PawsAndHearts.Web\
dotnet-ef migrations add Accounts_Initial -c AccountsWriteDbContext -p .\src\Accounts\PawsAndHearts.Accounts.Infrastructure\ -s .\src\PawsAndHearts.Web\

dotnet-ef database update -c WriteDbContext -p .\src\PetManagement\PawsAndHearts.PetManagement.Infrastructure\ -s .\src\PawsAndHearts.Web\
dotnet-ef database update -c WriteDbContext -p .\src\BreedManagement\PawsAndHearts.BreedManagement.Infrastructure\ -s .\src\PawsAndHearts.Web\
dotnet-ef database update -c AccountsWriteDbContext -p .\src\Accounts\PawsAndHearts.Accounts.Infrastructure\ -s .\src\PawsAndHearts.Web\


pause