// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.Extensions.Logging;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableMethods;
using Telegram.BotAPI.AvailableTypes;
using Telegram.BotAPI.Extensions;
using Telegram.BotAPI.GettingUpdates;

namespace SauceNAO.Application.Services;

/// <summary>
/// Implementation of the SauceNAO bot.
/// </summary>
partial class SauceNaoBot : SimpleTelegramBotBase, ISauceNaoBot
{
    private static User? Me;
    private readonly ILogger<SauceNaoBot> logger;
    private readonly ITelegramBotClient client;
    private readonly ISauceNaoContextFactory contextFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly IUserStateManager stateManager;
    private ISauceNaoContext? context;

    /// <summary>
    /// Initializes a new instance of the <see cref="SauceNaoBot"/> class.
    /// </summary>
    /// <param name="logger"></param>
    public SauceNaoBot(
        ILogger<SauceNaoBot> logger,
        ITelegramBotClient client,
        ISauceNaoContextFactory contextFactory,
        IServiceProvider serviceProvider,
        IUserStateManager stateManager
    )
    {
        this.logger = logger;
        this.client = client;
        this.contextFactory = contextFactory;
        this.serviceProvider = serviceProvider;
        this.stateManager = stateManager;

        // Get the bot information.
        Me ??= this.client.GetMe();
        // Configure the command extractor.
        this.SetCommandExtractor(this.Username);
    }

    private ISauceNaoContext Context =>
        this.context ?? throw new InvalidOperationException("The context is not set.");

    /// <inheritdoc />
    public long Id => Me!.Id;

    /// <inheritdoc />
    public string Name => Me!.FirstName;

    /// <inheritdoc />
    public string Username => Me!.Username!;

    /// <inheritdoc />
    public override Task OnUpdateAsync(Update update, CancellationToken cancellationToken = default)
    {
        // If the message was sent by Telegram (linked channels) or an anonymous user, ignore it.
        if (
            update.Message?.From?.Id == TelegramConstants.TelegramId
            || update.Message?.From?.Id == TelegramConstants.GroupAnonymousBotId
        )
        {
            return Task.CompletedTask;
        }

        this.context = this.contextFactory.Create(update);

        // Continue processing the update.
        return base.OnUpdateAsync(update, cancellationToken);
    }
}
