namespace SauceNAO.App.Features.ApiKeys;

record DeleteApiKeyState(string? Name)
{
    public string? Name { get; set; } = Name;
    public bool ConfirmDelete { get; set; }
}
