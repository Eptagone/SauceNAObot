// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License, See LICENCE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SauceNAO.Data;
using SauceNAO.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.BotAPI.GettingUpdates;

namespace SauceNAO.Controllers
{
    /// <summary>POST | https://example.com/SecurityApiToken </summary>
    [ApiController]
    [Route(Settings.SecurityToken)]
    public sealed class SauceBotController : ControllerBase
    {
        private readonly SauceNAOContext DB;
        private readonly SauceNAOBot Bot;
        public SauceBotController(SauceNAOContext context)
        {
            DB = context;
            Bot = new SauceNAOBot(context);
        }

        /// <summary>New Update</summary>
        /// <param name="update">Update</param>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Update update)
        {
            // If update == null return BadRequest
            if (update == null)
            {
                return BadRequest();
            }
            // Process Update
            await Bot.OnUpdateAsync(update).ConfigureAwait(false);
            // return Ok
            return Ok();
        }

        // Reports
        /// <summary>Get all reports.</summary>
        [HttpGet("reports")]
        public async Task<ActionResult<List<Report>>> GetReports()
            => await DB
            .Reports
            .ToListAsync()
            .ConfigureAwait(false);
        /// <summary>Get report by Id.</summary>
        /// <param name="id">Report Id</param>
        [HttpGet("reports/{id}", Name = "getReport")]
        public async Task<ActionResult<Report>> GetReports(int id)
            => await DB.Reports
            .FirstOrDefaultAsync(s => s.Id == id)
            .ConfigureAwait(false);
        /// <summary>Post New report.</summary>
        /// <param name="report">Report</param>
        [HttpPost("reports")]
        public async Task<ActionResult<Report>> PostReports([FromBody] Report report)
        {
            if (report == default)
            {
                return BadRequest();
            }

            await DB
                .AddAsync(report)
                .ConfigureAwait(false);
            await DB.SaveChangesAsync()
                .ConfigureAwait(false);
            return new CreatedAtRouteResult("getReport", new { id = report.Id }, report);
        }
        /// <summary>Delete Report by Id</summary>
        /// <param name="id">Report Id</param>
        [HttpDelete("reports/{id}")]
        public async Task<IActionResult> DeleteReports(int id)
        {
            Report report = await DB
                .Reports
                .FirstOrDefaultAsync(a => a.Id == id)
                .ConfigureAwait(false);
            if (report != default)
            {
                DB.Remove(report);
                await DB
                    .SaveChangesAsync()
                    .ConfigureAwait(false);
            }
            return Ok();
        }
    }
}
