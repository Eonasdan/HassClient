using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HassClient.WS.Extensions;
using NUnit.Framework;

namespace HassClient.WS.Tests
{
    public partial class RenderTemplateApiTests : BaseHassWsApiTest
    {
        [Test]
        public async Task RenderTemplate()
        {
            var result = await HassWsApi.RenderTemplateAsync("The sun is {{ states('sun.sun') }}");

            Assert.That(result, Is.Not.Null);
            Assert.That(MyRegex().IsMatch(result), Is.True);
        }

        [GeneratedRegex("^The sun is (?:above_horizon|below_horizon)$")]
        private static partial Regex MyRegex();
    }
}
