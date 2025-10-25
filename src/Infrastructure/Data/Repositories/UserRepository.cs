using Microsoft.EntityFrameworkCore;
using SauceNAO.Core.Entities.UserAggregate;
using SauceNAO.Core.Repositories;
using SauceNAO.Core.Specifications;
using Telegram.BotAPI.AvailableTypes;

namespace SauceNAO.Infrastructure.Data.Repositories;

class UserRepository(ApplicationDbContext context)
    : AsyncRepositoryBase<TelegramUser>(context),
        IUserRepository
{
    public Task<TelegramUser> UpsertAsync(User user, CancellationToken cancellationToken)
    {
        var spec = new UserSpecification(user.Id);
        var userEntity = spec.Evaluate(context.Users).SingleOrDefault();
        if (userEntity is null)
        {
            userEntity = new TelegramUser()
            {
                UserId = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                LanguageCode = user.LanguageCode,
            };
            return this.InsertAsync(userEntity, cancellationToken);
        }

        userEntity.Username = user.Username;
        userEntity.FirstName = user.FirstName;
        userEntity.LastName = user.LastName;
        userEntity.LanguageCode ??= user.LanguageCode;
        return this.UpdateAsync(userEntity, cancellationToken);
    }

    public Task<IReadOnlyDictionary<string, int>> GetLanguageCodesAsync(
        CancellationToken cancellationToken
    )
    {
        var languages = context
            .Users.GroupBy(u => u.LanguageCode)
            .Select(u => new { LanguageCode = u.Key, Count = u.Count() });

        return languages
            .ToDictionaryAsync(
                lc => lc.LanguageCode ?? "default",
                lc => lc.Count,
                cancellationToken
            )
            .ContinueWith(task => (IReadOnlyDictionary<string, int>)task.Result);
    }
}
