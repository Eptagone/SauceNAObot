// Copyright (c) 2026 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

namespace SauceNAO.App.Features.ApiKeys;

record DeleteApiKeyState(string? Name)
{
    public string? Name { get; set; } = Name;
    public bool ConfirmDelete { get; set; }
}
