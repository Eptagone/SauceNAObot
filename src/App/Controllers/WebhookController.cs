// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SauceNAO.Core.Configuration;
using SauceNAO.Core.Services;
using Telegram.BotAPI;
using Telegram.BotAPI.GettingUpdates;

namespace SauceNAO.App.Controllers;

/// <summary>
/// Represents a controller that receives updates from the Telegram bot.
/// </summary>
[ApiController]
public sealed class WebhookController(
    IUpdateHandlerPool updatePool,
    IOptions<BotConfiguration> optionsAccessor
) : ControllerBase
{
    [HttpPost("~/webhook")]
    public IActionResult Index(
        [FromHeader(Name = TelegramConstants.XTelegramBotApiSecretToken)] string secretToken,
        [FromBody] Update update
    )
    {
        if (optionsAccessor.Value.SecretToken != secretToken)
        {
            return this.Unauthorized();
        }

        // Queue the update to be processed.
        updatePool.QueueUpdate(update);
        return this.Accepted();
    }
}
