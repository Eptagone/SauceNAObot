// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SauceNAO.Application;
using SauceNAO.Domain;
using Telegram.BotAPI;
using Telegram.BotAPI.GettingUpdates;

namespace SauceNAO.WebApp.Controllers;

/// <summary>
/// Represents a controller that receives updates from the Telegram bot.
/// </summary>
/// <param name="botOptions">Options for the Telegram bot.</param>
/// <param name="updateReceiver">The update receiver.</param>
[ApiController]
public class WebhookController(
    IOptions<TelegramBotOptions> botOptions,
    IUpdateReceiver updateReceiver
) : ControllerBase
{
    private readonly string? secretToken = botOptions.Value.SecretToken;
    private readonly IUpdateReceiver updateReceiver = updateReceiver;

    [HttpPost("~/")]
    public IActionResult Index(
        [FromHeader(Name = TelegramConstants.XTelegramBotApiSecretToken)] string secretToken,
        [FromBody] Update update
    )
    {
        if (string.IsNullOrEmpty(this.secretToken) || this.secretToken != secretToken)
        {
            return this.Unauthorized();
        }

        // Pass the update to the update receiver.
        this.updateReceiver.ReceiveUpdate(update);

        return this.Accepted();
    }
}
