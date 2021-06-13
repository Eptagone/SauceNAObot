// Copyright (c) 2021 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SauceNao.Data;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SauceNao.Services
{
    /// <summary>This class allows you to clean up old data.</summary>
    public sealed class SauceNaoService
    {
        internal static bool IsBusy;
        private static readonly object @lock = new();

        private readonly ILogger<SauceNaoContext> _logger;
        private readonly SauceNaoContext _context;

        /// <summary>Initialize a nes instance of SauceNaoService.</summary>
        /// <param name="logger">Logger.</param>
        /// <param name="context">Database access class.</param>
        public SauceNaoService(ILogger<SauceNaoContext> logger, SauceNaoContext context)
        {
            _logger = logger;
            _context = context;
        }
        /// <summary>Start the service.</summary>
        public void Start()
        {
            if (!IsBusy)
            {
                IsBusy = true;
                lock (@lock)
                {
                    ExecuteAsync().Wait();
                }
                IsBusy = false;
            }
        }
        private async Task ExecuteAsync()
        {
            await Task.Run(() =>
            {
                var oldDate = DateTime.Now.AddDays(-7);
                var oldSauces = _context.SuccessfulSauces.AsNoTracking().Where(s => s.Date <= oldDate);
                var oldSaucesCount = oldSauces.Count();
                if (oldSaucesCount > 2000)
                {
                    _logger.LogInformation("[{0}] CleanUp Task: Started!", DateTime.Now);
                    var userSauces = _context.UserSauces.AsNoTracking().Where(s => s.Sauce.Date < oldDate);
                    var oldFiles = _context.TemporalFiles.AsNoTracking().Where(f => f.Date < oldDate);
                    var oldFilesCount = oldFiles.Count();
                    var filePaths = oldFiles.Select(f => f.FilePath).ToArray();
                    _context.RemoveRange(userSauces);
                    _context.RemoveRange(oldSauces);
                    _context.RemoveRange(oldFiles);

                    _context.SaveChanges();
                    string tmpPath = Path.GetTempPath();
                    foreach (var path in filePaths)
                    {
                        string fullFilePath = $"{tmpPath}{path}";
                        if (File.Exists(fullFilePath))
                        {
                            File.Delete(fullFilePath);
                        }
                    }

                    _logger.LogInformation("[{0}] CleanUp Task: Finished!", DateTime.Now);
                }
            });
        }
    }
}
