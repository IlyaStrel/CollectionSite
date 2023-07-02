using CollectionSite.Services;
using CS.Model.DB;
using Microsoft.AspNetCore.Http;
using Moq;
using Simple.DotNMap;

namespace CollectionSite.Tests
{
    public class BaseDataProviderTests
    {
        [Fact]
        public void parse_nmap_report_result_not_null()
        {
            // Arrange
            var report = CreateNmaprun();
            var httpContextMock = new Mock<HttpContext>();
            var hosts = new List<Host>();

            // Act
            BaseDataProvider.ParseNmapReport(httpContextMock.Object, hosts, report);

            // Assert
            Assert.NotNull(hosts);
            Assert.True(hosts.Any());
        }

        [Fact]
        public void parse_nmap_report_result_hostIp_contains()
        {
            // Arrange
            var report = CreateNmaprun();
            var httpContextMock = new Mock<HttpContext>();
            var hosts = new List<Host>();

            // Act
            BaseDataProvider.ParseNmapReport(httpContextMock.Object, hosts, report);

            // Assert
            Assert.Contains("127.0.0.1", hosts.Select(d => d.Ip));
        }

        [Fact]
        public void parse_nmap_report_result_hop_not_null()
        {
            // Arrange
            var report = CreateNmaprun();
            var httpContextMock = new Mock<HttpContext>();
            var hosts = new List<Host>();

            // Act
            BaseDataProvider.ParseNmapReport(httpContextMock.Object, hosts, report);

            // Assert
            Assert.NotNull(hosts.Select(d => d.Traces));
            Assert.True(hosts.Select(d => d.Traces).Any());
        }

        private nmaprun CreateNmaprun()
        {
            return new nmaprun
            {
                runstats = new runstats
                {
                    hosts = new hosts
                    {
                        total = "0",
                        down = "0",
                        up = "0"
                    }
                },
                Items = new object[]
                {
                    new host
                    {
                        address = new address
                        {
                            addr = "127.0.0.1"
                        },
                        Items = new object[]
                        {
                            new ports
                            {
                                port = new[]
                                {
                                    new port
                                    {
                                        portid = "5643",
                                        protocol = portProtocol.tcp,
                                        state = new state
                                        {
                                            state1 = "parsed"
                                        }
                                    }
                                }
                            },
                            new trace
                            {
                                hop = new[]
                                {
                                    new hop
                                    {
                                        ttl = "0"
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}