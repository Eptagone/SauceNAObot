using Microsoft.EntityFrameworkCore;
using SauceNAO.Core.Data;
using SauceNAO.Core.Entities.UserAggregate;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Infrastructure.Data.Repositories;

sealed class UserRepository(SnaoDbContext context)
    : RepositoryBase<UserEntity>(context),
        IUserRepository
{
    public Task<UserEntity> UpsertAsync(User user, CancellationToken cancellationToken)
    {
        var userEntity = context
            .Users.AsNoTrackingWithIdentityResolution()
            .SingleOrDefault(u => u.UserId == user.Id);
        if (userEntity is null)
        {
            userEntity = new UserEntity(user.Id, user.FirstName)
            {
                Username = user.Username,
                LastName = user.LastName,
                LanguageCode = user.LanguageCode,
            };
            return this.InsertAsync(userEntity, cancellationToken);
        }

        userEntity.Username = user.Username;
        userEntity.FirstName = user.FirstName;
        userEntity.LastName = user.LastName;
        if (!userEntity.UseFixedLanguage)
        {
            userEntity.LanguageCode = user.LanguageCode;
        }
        return this.UpdateAsync(userEntity, cancellationToken);
    }

    public async Task<IReadOnlyDictionary<string, int>> GetLanguageCodesAsync(
        CancellationToken cancellationToken
    )
    {
        var languages = context
            .Users.AsNoTracking()
            .GroupBy(u => u.LanguageCode)
            .Select(u => new { LanguageCode = u.Key, Count = u.Count() });

        return await languages.ToDictionaryAsync(
            lc => lc.LanguageCode ?? "default",
            lc => lc.Count,
            cancellationToken
        );
    }
}
