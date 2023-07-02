using CollectionSite.Models;
using CS.Data;
using CS.Model.DB;
using CS.Model.DTO;
using Simple.DotNMap;

namespace CollectionSite.Services
{
    public class BaseDataProvider : IBaseDataProvider
    {
        readonly ILogger _logger;
        readonly IHostRepository _hostRepository;
        readonly IConsumerRepository _consumerRepository;

        public BaseDataProvider(
            ILogger<BaseDataProvider> logger,
            IHostRepository hostRepository,
            IConsumerRepository consumerRepository
            )
        {
            _logger = logger;
            _hostRepository = hostRepository;
            _consumerRepository = consumerRepository;
        }

        /// <summary>
        /// Get consumer data and add to DB
        /// </summary>
        /// <param name="httpContext">Context http request</param>
        /// <returns></returns>
        public async Task<Consumer> GetConsumerData(
            HttpContext httpContext)
        {
            await Task.CompletedTask;

            var consumerData = new Consumer
            {
                Received = DateTimeOffset.UtcNow,
                UserAgent = httpContext.Request.Headers["User-Agent"].ToString(),
                Platform = httpContext.Request.Headers["Sec-Ch-Ua-Platform"].ToString(),
                RealIp = httpContext.Request.Headers["X-Real-Ip"].ToString()
            };

            _consumerRepository.Add(consumerData);

            return consumerData;
        }

        /// <summary>
        /// Get scan ports data and add to DB
        /// </summary>
        /// <param name="httpContext">Context http request</param>
        /// <param name="target">Target IP-addresses</param>
        /// <returns></returns>
        public async Task<List<ScanOut>> GetScanPortsData(
            HttpContext httpContext, string target)
        {
            var hosts = new List<CS.Model.DB.Host>();
            var result = new List<ScanOut>();

            try
            {
                var nmap = new Nmap(target, _logger);
                var report = nmap.Run() 
                    ?? throw new ApplicationException("Scanning report is empty.");

                ParseNmapReport(httpContext, hosts, report);

                if (!hosts.Any())
                    return result;

                try
                {
                    await _hostRepository.Add(hosts);
                }
                catch (Exception e)
                {
                    _logger.LogError("Saving database error. {0}", e.Message);
                    return result;
                }

                return result = hosts.Select(d => new ScanOut(d)).ToList();
            }
            catch (Exception e)
            {
                _logger.LogError("Scanning error. {0}", e.Message);
                return result;
            }
        }

        /// <summary>
        /// Parse nmap scanning report
        /// </summary>
        /// <param name="httpContext">Context http request</param>
        /// <param name="hosts">Host list</param>
        /// <param name="report">Nmap report</param>
        public static void ParseNmapReport(
            HttpContext httpContext, List<CS.Model.DB.Host> hosts, nmaprun report)
        {
            foreach (var host in report.Items.Select(d => d).Cast<host>())
            {
                // Adding target host data
                var targetHost = new CS.Model.DB.Host
                {
                    ConsumerIp = httpContext.Request?.Headers["X-Real-Ip"].ToString(),
                    Ip = host.address?.addr,
                    TypeIp = host.address?.addrtype.ToString(),
                    Name = host.Items?.Where(d => d as hostnames != null)?.Cast<hostnames>()?.FirstOrDefault()?.hostname?.FirstOrDefault()?.name,
                    State = host.status?.state.ToString()
                };

                if (host != null && host.Items != null && host.Items.Any())
                {
                    // Adding ports data for target host
                    var portsReport = host.Items.Where(d => d as ports != null)?.Cast<ports>()?.Select(d => d.port);
                    if (portsReport != null && portsReport.Any())
                        foreach (var ports in portsReport)
                        {
                            foreach (var port in ports)
                                targetHost.Ports.Add(
                                    new Port()
                                    {
                                        Host = targetHost,
                                        Number = ushort.Parse(port.portid),
                                        Type = port.protocol.ToString(),
                                        Name = port.service?.name,
                                        Product = port.service?.product,
                                        Version = port.service?.version,
                                        Extrainfo = port.service?.extrainfo,
                                        Status = port.state?.state1
                                    });
                        }

                    // Adding traces data for target host
                    var traceReport = host.Items.Where(d => d as trace != null)?.Cast<trace>()?.Select(d => d.hop);
                    if (traceReport != null && traceReport.Any())
                    {
                        foreach (var hops in host.Items.Where(d => d as trace != null).Cast<trace>().Select(d => d.hop))
                        {
                            foreach (var hop in hops)
                                targetHost.Traces.Add(
                                    new Trace()
                                    {
                                        Host = targetHost,
                                        TTL = int.Parse(hop.ttl),
                                        Ip = hop.ipaddr,
                                        Name = hop.host
                                    });
                        }
                    }
                }

                hosts.Add(targetHost);
            }
        }
    }
}