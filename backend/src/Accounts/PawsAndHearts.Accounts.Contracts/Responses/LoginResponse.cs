﻿namespace PawsAndHearts.Accounts.Contracts.Responses;

public record LoginResponse(string AccessToken, Guid RefreshToken, UserResponse User);