// Copyright (c) 2022 Quetzal Rivera.
// Licensed under the GNU General Public License v3.0, See LICENCE in the project root for license information.

using Xunit;
using SauceNAO.Core;
using SauceNAO.Core.API;
using System.Threading.Tasks;

namespace SauceNAO.Tests
{
    public class SauceNaoApiTest
    {
        private const string sampleUrl = "http://saucenao.com/images/static/banner.gif";

        [Fact]
        public void AnonymousRequest()
        {
            var snao = new SauceNaoApiService(OutputType.JsonApi, db: 999);
            var result = snao.Search(sampleUrl);
        }
    }
}