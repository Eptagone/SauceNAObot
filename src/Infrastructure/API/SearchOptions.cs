namespace SauceNAO.Infrastructure.API;

/// <summary>
/// Defines the options for the SauceNaoClient.
/// </summary>
/// <param name="OutputType">0=normal html 1=xml api(not implemented) 2=json api</param>
/// <param name="Apikey">Allows using the API from anywhere regardless of whether the client is logged in, or supports cookies.</param>
record SearchOptions(OutputType OutputType, string Apikey)
{
    /// <summary>
    /// Causes each index which has a match to output at most 1 for testing. Works best with a numres greater than the number of indexes searched.
    /// </summary>
    public bool? TestMode { get; init; }

    /// <summary>
    /// Mask for selecting specific indexes to ENABLE. dbmask=8191 will search all of the first 14 indexes. If intending to search all databases, the db=999 option is more appropriate.
    /// </summary>
    public string? Dbmask { get; init; }

    /// <summary>
    /// Mask for selecting specific indexes to DISABLE. dbmaski=8191 would search only indexes higher than the first 14. This is ideal when attempting to disable only certain indexes, while allowing future indexes to be included by default.<para>Bitmask Note: Index numbers start with 0. Even though pixiv is labeled as index 5, it would be controlled with the 6th bit position, which has a decimal value of 32 when set.</para>
    /// </summary>
    public string? Dbmaski { get; init; }

    /// <summary>
    /// Search a specific index number or all without needing to generate a bitmask.
    /// </summary>
    public uint? Db { get; init; }

    /// <summary>
    /// Change the number of results requested.
    /// </summary>
    public uint? Numres { get; init; }

    /// <summary>
    /// 0=no result deduping 1=consolidate booru results and dedupe by item identifier 2=all implemented dedupe methods such as by series name. Default is 2, more levels may be added in future.
    /// </summary>
    public Dedupe? Dedupe { get; init; }

    /// <summary>
    /// This controls the hidden field of results based on result content info. All results still show up in the api, check the hidden field for whether the site would have shown the image file.
    /// </summary>
    public Hide? Hide { get; init; }
}
