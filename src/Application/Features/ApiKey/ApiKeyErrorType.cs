namespace SauceNAO.Application.Features.ApiKey;

/// <summary>
/// Represents different types of API key errors.
/// </summary>
enum ApiKeyErrorType
{
    MissingApiKey,
    NameNotAvailable,
    ApiKeyAlreadyExists,
    NotPremium,
    CouldNotBeAdded,
}
