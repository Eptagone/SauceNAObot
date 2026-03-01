// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

namespace SauceNAO.App.Features.ApiKeys;

record AddApiKeyState(string? Name = null, string? Value = null)
{
    public string? Name { get; set; } = Name;
    public string? Value { get; set; } = Value;
    public bool? IsPublic { get; set; }
    public bool IsNameValidated { get; set; }
    public bool IsValueValidated { get; set; }
};
