// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Net;
using System.Text.Json;
using System.Web;
using Microsoft.Extensions.Caching.Distributed;
using SauceNAO.Core.Exceptions.Sauce;
using SauceNAO.Core.Models;
using SauceNAO.Core.Services;
using SauceNAO.Infrastructure.API;
using SauceNAO.Infrastructure.API.Types;

namespace SauceNAO.Infrastructure.Services;

/// <summary>
/// Provides functionality to retrieve media information from the SauceNAO API.
/// </summary>
class SauceNAOClient(HttpClient httpClient, IDistributedCache cache) : ISauceNAOClient
{
    internal const string SAUCENAO_ENDPOINT = "https://saucenao.com/";
    private const string SAUCENAO_BANNER_URL = "https://saucenao.com/images/static/banner.gif";
    private static readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };

    /// <inheritdoc/>
    public async Task<IEnumerable<Sauce>> SearchByUrlAsync(
        string url,
        string apiKey,
        CancellationToken cancellationToken = default
    )
    {
        var stream = await this.SendSearchRequestAsync(
            url,
            new(OutputType.JsonApi, apiKey),
            cancellationToken
        );
        var resultPayload = await DeserializeAsync(stream, cancellationToken);
        if (resultPayload.Results is null)
        {
            throw new SauceNAOException("No results found");
        }

        return resultPayload.Results.Select(result =>
        {
            var urls = new List<string>();
            var title = result.Data.Title;
            if (result.Data.ExtUrls is not null)
            {
                urls.AddRange(result.Data.ExtUrls);
            }
            if (result.Data.Source is not null)
            {
                if (result.Data.Source.StartsWith("http"))
                {
                    urls.Add(result.Data.Source);
                }
                else
                {
                    title ??= result.Data.Source;
                }
            }
            return new Sauce()
            {
                Similarity = result.Header.Similarity,
                Title = title,
                Author = result.Data.Author,
                Characters = result.Data.Characters,
                Material = result.Data.Material,
                Part = result.Data.Part,
                Year = result.Data.Year,
                EstimationTime = result.Data.EstTime,
                Links = urls,
            };
        });
    }

    /// <inheritdoc/>
    public async Task<bool> IsPremiumAsync(
        string apikey,
        CancellationToken cancellationToken = default
    )
    {
        var cacheKey = $"snao-service/apikey-is-premium/{apikey}";
        try
        {
            var item = await cache.GetAsync(cacheKey, cancellationToken);
            SnaoApiResponse response;
            if (item is null)
            {
                using var stream = await this.SendSearchRequestAsync(
                    SAUCENAO_BANNER_URL,
                    new SearchOptions(OutputType.JsonApi, apikey),
                    cancellationToken
                );
                response = await DeserializeAsync(stream, cancellationToken);
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms, cancellationToken);
                item = ms.ToArray();
                await cache.SetAsync(
                    cacheKey,
                    item,
                    new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30) },
                    cancellationToken
                );
            }
            else
            {
                var stream = new MemoryStream(item);
                response = await DeserializeAsync(stream, cancellationToken);
            }

            return response.Header.AccountType == 2;
        }
        catch (SauceNAOException)
        {
            throw;
        }
        catch (Exception exp)
        {
            throw new SauceNAOException(innerException: exp);
        }
    }

    public async Task<Stream> SendSearchRequestAsync(
        string url,
        SearchOptions options,
        CancellationToken cancellationToken
    )
    {
        var parameters = HttpUtility.ParseQueryString(string.Empty);
        parameters["url"] = url;
        parameters["output_type"] = options.OutputType.ToString("d");
        parameters["api_key"] = options.Apikey;

        if (options.TestMode is not null)
        {
            parameters["testmode"] = options.TestMode.Value ? "1" : "0";
        }
        if (!string.IsNullOrEmpty(options.Dbmask))
        {
            parameters["dbmask"] = options.Dbmask;
        }
        if (!string.IsNullOrEmpty(options.Dbmaski))
        {
            parameters["dbmaski"] = options.Dbmaski;
        }
        if (options.Db is not null)
        {
            parameters["db"] = options.Db.ToString();
        }
        if (options.Numres is not null)
        {
            parameters["numres"] = options.Numres.ToString();
        }
        if (options.Dedupe is not null)
        {
            parameters["dedupe"] = options.Dedupe.Value.ToString("d");
        }
        if (options.Hide is not null)
        {
            parameters["hide"] = options.Hide.Value.ToString("d");
        }

        var request = new HttpRequestMessage(HttpMethod.Get, $"search.php?{parameters}");

        try
        {
            var response = await httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStreamAsync(cancellationToken);
        }
        catch (HttpRequestException exp)
        {
            if (exp.StatusCode == HttpStatusCode.TooManyRequests)
            {
                throw new SearchLimitReachedException(exp);
            }
            if (
                exp.StatusCode == HttpStatusCode.Unauthorized
                || exp.StatusCode == HttpStatusCode.Forbidden
            )
            {
                throw new InvalidApiKeyException(exp);
            }

            throw new SauceNAOException(
                $"The SauceNAO API returned an error (${exp.StatusCode}): {exp.Message}",
                innerException: exp
            );
        }
        catch (SauceNAOException)
        {
            throw;
        }
        catch (Exception exp)
        {
            throw new SauceNAOException(innerException: exp);
        }
    }

    private static async Task<SnaoApiResponse> DeserializeAsync(
        Stream stream,
        CancellationToken cancellationToken
    )
    {
        var payload =
            await JsonSerializer.DeserializeAsync<SnaoApiResponse>(
                stream,
                jsonSerializerOptions,
                cancellationToken
            ) ?? throw new SauceNAOException("Could not deserialize SauceNAO response.");
        return payload;
    }
}
