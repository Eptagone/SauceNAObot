// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using SauceNAO.Core;
using Telegram.BotAPI.GettingUpdates;

namespace SauceNAO.Webhook.Controllers;

[Route("[controller]")]
[ApiController]
public sealed class BotController : ControllerBase
{
	private readonly IConfiguration _configuration;
	private readonly ILogger<BotController> _logger;
	private readonly SauceNaoBot _bot;

	public BotController(ILogger<BotController> logger, IConfiguration configuration, SauceNaoBot bot)
	{
		this._configuration = configuration;
		this._logger = logger;
		this._bot = bot;
	}

	[HttpGet]
	public IActionResult Get([FromHeader(Name = "X-Telegram-Bot-Api-Secret-Token")] string secretToken)
	{
		if (this._configuration["AccessToken"] != secretToken)
		{
#if DEBUG
			this._logger.LogWarning("Failed access!");
#endif
			this.Unauthorized();
		}
		return this.Ok();
	}

	[HttpPost]
	public async Task<IActionResult> PostAsync(
		[FromHeader(Name = "X-Telegram-Bot-Api-Secret-Token")] string secretToken,
		[FromBody] Update update, CancellationToken cancellationToken)
	{
		if (this._configuration["AccessToken"] != secretToken)
		{
#if DEBUG
			this._logger.LogWarning("Failed access");
#endif
			this.Unauthorized();
		}
		if (update == default)
		{
#if DEBUG
			this._logger.LogWarning("Invalid update detected");
#endif
			return this.BadRequest();
		}
		await this._bot.OnUpdateAsync(update, cancellationToken).ConfigureAwait(false);
		return this.Ok();
	}
}
