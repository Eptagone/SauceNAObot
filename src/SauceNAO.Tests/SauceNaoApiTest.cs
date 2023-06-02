// Copyright (c) 2023 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using SauceNAO.Core.API;
using Xunit;

namespace SauceNAO.Tests;

public class SauceNaoApiTest
{
	private const string sampleUrl = "http://saucenao.com/images/static/banner.gif";

	[Fact]
	public void AnonymousRequest()
	{
		var snao = new SauceNaoApiService(OutputType.JsonApi, db: 999);
		var result = snao.Search(sampleUrl);
		Assert.NotNull(result);
	}
}