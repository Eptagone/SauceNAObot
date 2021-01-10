// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License, See LICENCE in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Net.Http.Headers;
using System.IO;
using System.Threading.Tasks;

namespace SauceNAO.Controllers
{
    [ApiController]
    [Route("temp")]
    public class FileController : ControllerBase
    {
        ///<Summary>If file exists, return file.</Summary>
        [HttpGet]
        public async Task<IActionResult> Get(string file)
        {
            string tmp = Path.GetTempPath();
            string path = $"{tmp}{file}";
            // If file not exist return NotFound!
            if (!System.IO.File.Exists(path))
            {
                return NotFound();
            }

            byte[] filearray = await System.IO.File.ReadAllBytesAsync(path).ConfigureAwait(false);
            MemoryStream stream = new MemoryStream(filearray);
            FileStreamResult response = new FileStreamResult(stream, new MediaTypeHeaderValue("application/octet-stream"))
            {
                FileDownloadName = file
            };
            return response;
        }
    }
}
