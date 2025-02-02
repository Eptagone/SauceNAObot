// Copyright (c) 2025 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using System.Text.Json;
using System.Web;
using SauceNAO.Infrastructure.API.Types;

namespace SauceNAO.Infrastructure.API;

/// <summary>
/// This class provides access to the saucenao.com API.
/// </summary>
internal sealed class SauceNaoClient
{
    private const string SAUCENAO_ENDPOINT = "https://saucenao.com/";
    private static readonly HttpClient httpClient =
        new() { BaseAddress = new Uri(SAUCENAO_ENDPOINT) };
    private static readonly JsonSerializerOptions jsonSerializerOptions =
        new() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

    private readonly string searchPathTemplate;

    /// <summary>
    /// Initializes a new instance of SauceNaoApiService class.
    /// </summary>
    /// <param name="outputType">0=normal html 1=xml api(not implemented) 2=json api</param>
    /// <param name="apikey">Allows using the API from anywhere regardless of whether the client is logged in, or supports cookies.</param>
    /// <param name="testMode">Causes each index which has a match to output at most 1 for testing. Works best with a numres greater than the number of indexes searched.</param>
    /// <param name="dbmask">Mask for selecting specific indexes to ENABLE. dbmask=8191 will search all of the first 14 indexes. If intending to search all databases, the db=999 option is more appropriate.</param>
    /// <param name="dbmaski">Mask for selecting specific indexes to DISABLE. dbmaski=8191 would search only indexes higher than the first 14. This is ideal when attempting to disable only certain indexes, while allowing future indexes to be included by default.<para>Bitmask Note: Index numbers start with 0. Even though pixiv is labeled as index 5, it would be controlled with the 6th bit position, which has a decimal value of 32 when set.</para></param>
    /// <param name="db">search a specific index number or all without needing to generate a bitmask.</param>
    /// <param name="numres">Change the number of results requested.</param>
    /// <param name="dedupe">0=no result deduping 1=consolidate booru results and dedupe by item identifier 2=all implemented dedupe methods such as by series name. Default is 2, more levels may be added in future.</param>
    /// <param name="hide">This controls the hidden field of results based on result content info. All results still show up in the api, check the hidden field for whether the site would have shown the image file.</param>
    public SauceNaoClient(
        OutputType outputType,
        string apikey,
        bool? testMode = null,
        string? dbmask = null,
        string? dbmaski = null,
        uint? db = null,
        uint? numres = null,
        Dedupe? dedupe = null,
        Hide? hide = null
    )
    {
        var parameters = HttpUtility.ParseQueryString(string.Empty);
        parameters["output_type"] = outputType.ToString("d");
        parameters["api_key"] = apikey;

        if (testMode is not null)
        {
            parameters["testmode"] = testMode.Value ? "1" : "0";
        }
        if (!string.IsNullOrEmpty(dbmask))
        {
            parameters["dbmask"] = dbmask;
        }
        if (!string.IsNullOrEmpty(dbmaski))
        {
            parameters["dbmaski"] = dbmaski;
        }
        if (db is not null)
        {
            parameters["db"] = db.ToString();
        }
        if (numres is not null)
        {
            parameters["numres"] = numres.ToString();
        }
        if (dedupe is not null)
        {
            parameters["dedupe"] = dedupe.Value.ToString("d");
        }
        if (hide is not null)
        {
            parameters["hide"] = hide.Value.ToString("d");
        }

        this.searchPathTemplate = $"search.php?{parameters}&url={{0}}";
    }

    /// <summary>
    /// Start a new search using the url provided.
    /// </summary>
    /// <param name="url">Image url.</param>
    /// <param name="cancellationToken">Cancellation Token.</param>
    /// <returns><see cref="ISnaoResponse"/></returns>
    public async Task<ISnaoResponse?> SearchAsync(
        string url,
        CancellationToken cancellationToken = default
    )
    {
        var fullPath = string.Format(this.searchPathTemplate, url);
        var httpResponse = await httpClient.GetAsync(fullPath, cancellationToken);
        var stream = await httpResponse.Content.ReadAsStreamAsync(cancellationToken);

        using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

        if (doc.RootElement.TryGetProperty("results", out var resultsDoc))
        {
            var header = doc
                .RootElement.GetProperty("header")
                .Deserialize<SnaoSuccessfulHeader>(jsonSerializerOptions);
            var results = resultsDoc.Deserialize<IEnumerable<SnaoResult>>(jsonSerializerOptions);
            return new SnaoSuccessfulResponse(
                header ?? throw new InvalidOperationException("Header is null."),
                results ?? throw new InvalidOperationException("Results is null.")
            );
        }
        else
        {
            return doc.Deserialize<SnaoErrorResponse>(jsonSerializerOptions);
        }
    }

    /// <summary>
    /// Start a new search using the url provided.
    /// </summary>
    /// <param name="url">Image url.</param>
    /// <returns><see cref="ISnaoResponse"/></returns>
    /// <exception cref="SearchResponseException"/>
    public ISnaoResponse? Search(string url)
    {
        return this.SearchAsync(url).GetAwaiter().GetResult();
    }
}
