namespace SauceNAO.App.Features.ApiKeys;

record AddApiKeyState(string? Name = null, string? Value = null)
{
    public string? Name { get; set; } = Name;
    public string? Value { get; set; } = Value;
    public bool? IsPublic { get; set; }
    public bool IsNameValidated { get; set; }
    public bool IsValueValidated { get; set; }
};
