// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using SauceNAO.Core;
using Telegram.BotAPI.GettingUpdates;

namespace SauceNAO.Webhook.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public sealed class BotController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<BotController> _logger;
        private readonly SauceNaoBot _bot;

        public BotController(ILogger<BotController> logger, IConfiguration configuration, SauceNaoBot bot)
        {
            _configuration = configuration;
            _logger = logger;
            _bot = bot;
        }

        [HttpGet("{accessToken}")]
        public IActionResult Get(string accessToken)
        {
            if (_configuration["AccessToken"] != accessToken)
            {
                _logger.LogWarning("Failed access!");
                return Unauthorized();
            }
            return Ok();
        }

        [HttpPost("{accessToken}")]
        public async Task<IActionResult> PostAsync(string accessToken, [FromBody] Update update, CancellationToken cancellationToken)
        {
            if (_configuration["AccessToken"] != accessToken)
            {
                _logger.LogWarning("Failed access");
                return Unauthorized();
            }
            if (update == default)
            {
                _logger.LogWarning("Invalid update detected");
                return BadRequest();
            }
            await _bot.OnUpdateAsync(update, cancellationToken).ConfigureAwait(false);
            return Ok();
        }
    }
}