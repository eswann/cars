using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using Xunit;

namespace EnjoyCQRS.IntegrationTests
{
    public class HandlersTests
    {
        public const string CategoryName = "Integration";
        public const string CategoryValue = "WebApi";
        
        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public async Task Should_resolve_by_instance_message_handler()
        {
            var server = TestServerFactory();

            var response = await (await server.CreateRequest("/command/bar/transient").GetAsync()).Content.ReadAsStringAsync();

            var obj = JsonConvert.DeserializeObject<Dictionary<string, int>>(response);

            obj["counter1"].Should().Be(1);
            obj["counter2"].Should().Be(1);
        }

        [Trait(CategoryName, CategoryValue)]
        [Fact]
        public async Task Should_resolve_by_scope_message_handler()
        {
            var server = TestServerFactory();

            var response = await (await server.CreateRequest("/command/bar/scoped").GetAsync()).Content.ReadAsStringAsync();

            var obj = JsonConvert.DeserializeObject<Dictionary<string, int>>(response);

            obj["counter1"].Should().Be(1);
            obj["counter2"].Should().Be(2);
        }

        private TestServer TestServerFactory()
        {
            var builder = new WebHostBuilder()
                .UseStartup<Startup2>();

            var testServer = new TestServer(builder);

            return testServer;
        }
    }
}