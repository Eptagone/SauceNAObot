// Copyright (c) 2023 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using SauceNAO.Core;
using Telegram.BotAPI.GettingUpdates;

namespace SauceNAO.WebApp.Controllers;

/// <summary>
/// The bot controller. It is used to receive updates from Telegram.
/// </summary>
[ApiController]
[Route("[controller]")]
public sealed class BotController : ControllerBase
{
	private readonly string _secretToken;
	private readonly ILogger<BotController> _logger;
	private readonly SauceNaoBot _bot;

	/// <summary>
	/// Initializes a new instance of the <see cref="BotController"/> class.
	/// </summary>
	/// <param name="logger">The logger service.</param>
	/// <param name="configuration">The configuration.</param>
	/// <param name="bot">The bot service.</param>
	public BotController(ILogger<BotController> logger, IConfiguration configuration, SauceNaoBot bot)
	{
		this._logger = logger;
		this._bot = bot;

		var options = configuration.GetRequiredSection("SNAO").Get<SnaoOptions>();
		this._secretToken = options.SecretToken ?? string.Empty;
	}

	[HttpGet]
	public IActionResult Get([FromHeader(Name = "X-Telegram-Bot-Api-Secret-Token")] string secretToken)
	{
		if (this._secretToken != secretToken)
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
		if (this._secretToken != secretToken)
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
