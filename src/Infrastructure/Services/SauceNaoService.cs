// Copyright (c) 2024 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SauceNAO.Domain;
using SauceNAO.Domain.Extensions;
using SauceNAO.Domain.Services;
using SauceNAO.Infrastructure.API;
using SauceNAO.Infrastructure.API.Types;

namespace SauceNAO.Infrastructure.Services;

/// <summary>
/// Provides functionality to retrieve media information from the SauceNAO API.
/// </summary>
class SauceNaoService(ILogger<SauceNaoService> logger, IDistributedCache cache) : ISauceNaoService
{
    private const string SAUCENAO_BANNER_URL = "https://saucenao.com/images/static/banner.gif";

    /// <inheritdoc/>
    public async Task<Pantry?> SearchByUrlAsync(
        string url,
        string apiKey,
        CancellationToken cancellationToken = default
    )
    {
        var client = new SauceNaoClient(OutputType.JsonApi, apiKey);
        try
        {
            var response = await client.SearchAsync(url, cancellationToken);
            if (response is null)
            {
                logger.LogSnaoEmptyResponse();
                return null;
            }

            if (response is SnaoSuccessfulResponse success)
            {
                var results = success.Results.Select(r =>
                {
                    var urls = new List<string>();
                    var title = r.Data.Title;
                    if (r.Data.ExtUrls is not null)
                    {
                        urls.AddRange(r.Data.ExtUrls);
                    }
                    if (r.Data.Source is not null)
                    {
                        if (r.Data.Source.StartsWith("http"))
                        {
                            urls.Add(r.Data.Source);
                        }
                        else
                        {
                            title ??= r.Data.Source;
                        }
                    }

                    return new Recipe(
                        r.Header.Similarity,
                        r.Header.IndexId,
                        r.Header.IndexName,
                        urls,
                        title,
                        r.Data.Author,
                        r.Data.Characters,
                        r.Data.Material,
                        r.Data.Part,
                        r.Data.Year,
                        r.Data.EstTime
                    );
                });
                return new Pantry(results, true, false);
            }
            else if (response is SnaoErrorResponse error)
            {
                var searchLimitReached = error.Header.Message.Contains("Search Rate Too High");
                // If the error message is not search limit reached, log it.
                if (!searchLimitReached)
                {
                    logger.LogSnaoError(error.Header.Message);
                }
                return new Pantry([], false, searchLimitReached);
            }
            else
            {
                logger.LogSnaoUnknownResponse();
                return null;
            }
        }
        catch (Exception e)
        {
            logger.LogSnaoRequestFailed(e, e.Message);
            return null;
        }
    }

    /// <inheritdoc/>
    public Task<bool?> IsPremiumUserAsync(
        string apikey,
        CancellationToken cancellationToken = default
    )
    {
        var cacheKey = $"snao:apikey-is-premium:{apikey}";

        return cache.GetOrCreateAsync(
            cacheKey,
            async (entry) =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);

                var client = new SauceNaoClient(OutputType.JsonApi, apikey);
                try
                {
                    var response = await client.SearchAsync(SAUCENAO_BANNER_URL, cancellationToken);
                    if (response is null)
                    {
                        logger.LogSnaoEmptyResponse();
                        return null;
                    }

                    if (response is SnaoSuccessfulResponse success)
                    {
                        var isPremium = success.Header.AccountType == 2;
                        if (isPremium)
                        {
                            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                        }
                        return isPremium;
                    }
                    else if (response is SnaoErrorResponse error)
                    {
                        var searchLimitReached = error.Header.Message.Contains(
                            "Search Rate Too High"
                        );
                        // If the error message is not search limit reached, log it.
                        if (!searchLimitReached)
                        {
                            logger.LogSnaoError(error.Header.Message);
                        }
                    }
                    else
                    {
                        logger.LogSnaoUnknownResponse();
                    }
                }
                catch (Exception e)
                {
                    logger.LogSnaoRequestFailed(e, e.Message);
                }

                return null;
            },
            cancellationToken
        );
    }
}
