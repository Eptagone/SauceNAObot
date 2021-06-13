// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SauceNao.Data;
using SauceNao.Data.Models;
using SauceNao.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.BotAPI.GettingUpdates;

namespace SauceNao.Controllers
{
    /// <summary>POST | https://example.com/SecurityApiToken </summary>
    [ApiController]
    [Route("{SauceNaoWebhook}")]
    public sealed class SauceBotController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>Initialize a new instance of SauceBotController</summary>
        /// <param name="logger">The logger object</param>
        /// <param name="serviceProvider">Service Provider</param>
        public SauceBotController(ILogger<SauceBotController> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        /// <summary>New Update</summary>
        /// <param name="update">Update</param>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Update update)
        {
            // If update == null return BadRequest
            if (update == null)
            {
                _logger.LogWarning("Bad Request: Null update!");
                return BadRequest();
            }
            // Get Scopre
            using (var scope = _serviceProvider.CreateScope())
            {
                // Process Update
                var bot = scope.ServiceProvider.GetRequiredService<SauceNaoBot>();
                await bot.OnUpdateAsync(update, default).ConfigureAwait(false);
            }
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(() =>
            {
                var scope = _serviceProvider.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<SauceNaoService>();
                service.Start();
            });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            // return Ok
            return Ok();
        }

        // Chats
        /// <summary>Get all chats data.</summary>
        [HttpGet("chats")]
        public async Task<ActionResult<List<AppChat>>> GetChats()
        {
            using var scope = _serviceProvider.CreateScope();
            using var context = scope.ServiceProvider.GetService<SauceNaoContext>();
            return await context.Chats.AsNoTracking()
                       .ToListAsync()
                       .ConfigureAwait(false);
        }

        /// <summary>Get chat by CHat Id.</summary>
        /// <param name="id">Chat Id</param>
        [HttpGet("chats/{id}", Name = "getChat")]
        public async Task<ActionResult<AppChat>> GetChat(long id)
        {
            using var scope = _serviceProvider.CreateScope();
            using var context = scope.ServiceProvider.GetService<SauceNaoContext>();
            return await context.Chats.AsNoTracking()
                       .FirstOrDefaultAsync(c => c.ChatId == id)
                       .ConfigureAwait(false);
        }

        // Users
        /// <summary>Get all user dat.a</summary>
        [HttpGet("users")]
        public async Task<ActionResult<List<AppUser>>> GetUsers()
        {
            using var scope = _serviceProvider.CreateScope();
            using var context = scope.ServiceProvider.GetService<SauceNaoContext>();
            return await context.Users.AsNoTracking()
                       .ToListAsync()
                       .ConfigureAwait(false);
        }

        /// <summary>Get user by user Id</summary>
        /// <param name="id">User Id</param>
        /// <returns></returns>
        [HttpGet("users/{id}", Name = "getUser")]
        public async Task<ActionResult<AppUser>> GetUser(int id)
        {
            using var scope = _serviceProvider.CreateScope();
            using var context = scope.ServiceProvider.GetService<SauceNaoContext>();
            return await context.Users.AsNoTracking()
                       .FirstOrDefaultAsync(u => u.Id == id)
                       .ConfigureAwait(false);
        }

        // Searchs
        /// <summary>Get all searchs</summary>
        [HttpGet("search")]
        public async Task<ActionResult<List<SuccessfulSauce>>> GetSearches()
        {
            using var scope = _serviceProvider.CreateScope();
            using var context = scope.ServiceProvider.GetService<SauceNaoContext>();
            return await context.SuccessfulSauces.AsNoTracking()
                       .OrderByDescending(s => s.Date)
                       .ToListAsync()
                       .ConfigureAwait(false);
        }

        /// <summary>Get search by File Unique Id.</summary>
        /// <param name="id">File Unique Id.</param>
        [HttpGet("search/{id}", Name = "getSearch")]
        public async Task<ActionResult<SuccessfulSauce>> GetSearch(string id)
        {
            using var scope = _serviceProvider.CreateScope();
            using var context = scope.ServiceProvider.GetService<SauceNaoContext>();
            return await context.SuccessfulSauces.AsNoTracking()
                       .FirstOrDefaultAsync(s => s.FileUniqueId == id)
                       .ConfigureAwait(false);
        }

        /// <summary>Delete search by File Unique Id.</summary>
        /// <param name="id">File Unique Id.</param>
        [HttpDelete("search/{id}")]
        public async Task<IActionResult> DeleteSearch(string id)
        {
            using var scope = _serviceProvider.CreateScope();
            using var context = scope.ServiceProvider.GetService<SauceNaoContext>();
            var sauce = await context.SuccessfulSauces
                .FirstOrDefaultAsync(a => a.FileUniqueId == id)
                .ConfigureAwait(false);
            if (sauce != default)
            {
                context.Remove(sauce);
                await context.SaveChangesAsync().ConfigureAwait(false);
            }
            return Ok();
        }
    }
}
