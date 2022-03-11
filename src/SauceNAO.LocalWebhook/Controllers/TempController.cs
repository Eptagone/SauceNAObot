// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using SauceNAO.Core;
using SauceNAO.Infrastructure.Data;

namespace SauceNAO.Webhook.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public sealed class TempController : ControllerBase
    {
        private readonly ILogger<TempController> _logger;
        private readonly TemporalFileRepository _fileRepository;

        public TempController(ILogger<TempController> logger, TemporalFileRepository fileRepository)
        {
            _logger = logger;
            _fileRepository = fileRepository;
        }

        // GET: api/temp/5
        ///<Summary>If file exists, return file.</Summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTemporalFile(string id, CancellationToken cancellationToken)
        {
            var fileUniqueId = id;
            var tempFile = await _fileRepository.GetFileAsync(fileUniqueId, cancellationToken).ConfigureAwait(false);

            if (tempFile == default)
            {
                return NotFound();
            }
            try
            {
                return File(tempFile.RawData, tempFile.ContentType ?? "application/octet-stream", tempFile.Filename);
            }
            catch (Exception e)
            {
                _logger.LogError("Can't return temporal file [{fileUniqueId}]. Error message: {message}", fileUniqueId, e.InnerException?.Message ?? e.Message);
                return BadRequest();
            }
        }
    }
}
