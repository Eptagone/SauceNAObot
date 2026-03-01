namespace SauceNAO.Core.Entities;

public interface IWithCreationDate
{
    /// <summary>
    /// Date and time of creation.
    /// </summary>
    DateTimeOffset CreatedAt { get; set; }
}
