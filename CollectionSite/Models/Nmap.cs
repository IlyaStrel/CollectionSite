using Simple.DotNMap;
using Simple.DotNMap.Extensions;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace CollectionSite.Models
{
    /// <summary>
    /// Available flags
    /// </summary>
    public enum Flag
    {
        InputFilename,
        RandomTargets,
        ExcludeHosts,
        ExcludeFile,
        ListScan,
        PingScan,
        TreatHostsAsOnline,
        TcpSynDiscovery,
        AckDiscovery,
        UdpDiscovery,
        SctpDiscovery,
        IcmpEchoDiscovery,
        IcmpTimestampDiscovery,
        IcmpNetmaskDiscovery,
        ProtocolPing,
        NeverDnsResolve,
        DnsServers,
        SystemDns,
        Traceroute,
        HostScan,
        TcpSynScan,
        ConnectScan,
        AckScan,
        WindowScan,
        MaimonScan,
        UdpScan,
        TcpNullScan,
        FinScan,
        XmasScan,
        ScanFlags,
        IdleScan,
        SctpInitScan,
        CookieEchoScan,
        IpProtocolScan,
        FtpBounceScan,
        PortSpecification,
        FastScanMode,
        ScanPortsConsecutively,
        TopPorts,
        PortRatio,
        ServiceVersion,
        VersionIntensity,
        VersionLight,
        VersionAll,
        VersionTrace,
        DefaultScriptScan,
        Script,
        ScriptArgs,
        ScriptTrace,
        ScriptUpdateDb,
        OsDetection,
        OsScanLimit,
        OsScanGuess,
        ParanoidTiming,
        SneakyTiming,
        PoliteTiming,
        NormalTiming,
        AggressiveTiming,
        InsaneTiming,
        ParallelMinHostGroupSize,
        ParallelMaxHostGroupSize,
        MinProbeParallelization,
        MaxProbeParallelization,
        MinRttTimeout,
        MaxRttTimeout,
        InitialRttTimeout,
        MaxRetries,
        HostTimeout,
        ScanDelay,
        MaxScanDelay,
        MinRate,
        MaxRate,
        FragmentPackets,
        Mtu,
        Decoy,
        SpoofSourceAddress,
        Interface,
        SourcePortG,
        SourcePort,
        DataLength,
        IpOptions,
        TimeToLive,
        SpoofMacAddress,
        BadSum,
        Adler32,
        NormalOutput,
        XmlOutput,
        ScriptKiddieOutput,
        GreppableOutput,
        AllThreeOutput,
        Verbose,
        DebugLevel,
        Reason,
        Open,
        PacketTrace,
        PrintHostInterfaceList,
        LogErrors,
        AppendOutput,
        Resume,
        Stylesheet,
        WebXml,
        NoStylesheet,
        Ipv6,
        A,
        DataDir,
        SendEth,
        SendIp,
        Privileged,
        Unprivileged,
        Version,
        Help,
    }

    /// <summary>
    /// Service Nmap
    /// </summary>
    public class Nmap
    {
        readonly ILogger _logger;
        bool _isWindows = true;

        public Nmap(
            string target, ILogger logger, ProcessWindowStyle windowStyle = ProcessWindowStyle.Hidden)
        {
            _logger = logger;
            _isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            Path = _isWindows ? GetPathToNmap() : "/bin/bash";
            OutputPath = _isWindows ? System.IO.Path.GetTempFileName() : $"{Guid.NewGuid()}.xml";
            Options = new Options();
            WindowStyle = windowStyle;
            Target = target; //?? "45.33.49.119";
            WorkDir = _isWindows ? "" : "/tmp/";
        }

        /// <summary>
        /// Path to Nmap service
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Output xml file path
        /// </summary>
        public string OutputPath { get; set; }

        /// <summary>
        /// Options for Nmap
        /// </summary>
        public Options Options { get; set; }

        /// <summary>
        /// Target IP addresses
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// Output window display type
        /// </summary>
        public ProcessWindowStyle WindowStyle { get; set; }

        /// <summary>
        /// Working directory
        /// </summary>
        public string WorkDir { get; set; }

        static string LocateExecutable(
            string filename)
        {
            string path = Environment.GetEnvironmentVariable("path");
            string[] folders = path.Split(';');

            foreach (string folder in folders)
            {
                string combined = System.IO.Path.Combine(folder, filename);
                if (File.Exists(combined))
                    return combined;
            }

            return string.Empty;
        }

        /// <summary>
        /// Search nmap file
        /// </summary>
        static string GetPathToNmap()
            => LocateExecutable("nmap.exe");

        /// <summary>
        /// Starting the process
        /// </summary>
        public virtual nmaprun? Run()
        {
            if (string.IsNullOrEmpty(OutputPath))
                throw new ApplicationException("Output file path is null");

            if (_isWindows && (string.IsNullOrEmpty(Path) || !File.Exists(Path)))
                throw new ApplicationException("Nmap service path is not valid");

            if (string.IsNullOrEmpty(Target))
                throw new ApplicationException("Target addresses are not set");

            if (Options == null)
                throw new ApplicationException("Options are not set");

            // Add base options
            // todo: added get params from front
            Options.AddAll(new[]
            {
                //  Flag.ServiceVersion,
                //  Flag.OsDetection,
                //  Flag.AggressiveTiming,
                //  Flag.Verbose,
                Flag.A
            });

            // Add output file path
            Options[Flag.XmlOutput] = OutputPath;

            using (var process = new Process())
            {
                process.StartInfo.FileName = Path;
                process.StartInfo.Arguments = _isWindows
                    ? string.Format("{0} {1}", Options, Target)
                    : string.Format("-c \"nmap {0} {1}\"", Options, Target);
                process.StartInfo.WindowStyle = WindowStyle;
                process.StartInfo.WorkingDirectory = WorkDir;

                _logger.LogInformation("Process params: {@Path}", process.StartInfo);

                process.Start();
                process.WaitForExit();

                _logger.LogInformation($"Created report");

                if (!File.Exists(WorkDir + OutputPath))
                    throw new ArgumentNullException(process.StartInfo.Arguments);
            }

            return Serialization.DeserializeFromFile<nmaprun>(WorkDir + OutputPath);
        }
    }

    /// <summary>
    /// Options Nmap
    /// </summary>
    public class Options : IDictionary<Flag, string>
    {
        // Nmap flag by option
        readonly Dictionary<Flag, string> _flagByOption = new()
        {
            {Flag.InputFilename, "-iL"},
            {Flag.RandomTargets, "-iR"},
            {Flag.ExcludeHosts, "--exclude"},
            {Flag.ExcludeFile, "--excludefile"},
            {Flag.ListScan, "-sL"},
            {Flag.PingScan, "-sP"},
            {Flag.TreatHostsAsOnline, "-PN"},
            {Flag.TcpSynDiscovery, "-PS"},
            {Flag.AckDiscovery, "-PA"},
            {Flag.UdpDiscovery, "-PU"},
            {Flag.SctpDiscovery, "-PY"},
            {Flag.IcmpEchoDiscovery, "-PE"},
            {Flag.IcmpTimestampDiscovery, "-PP"},
            {Flag.IcmpNetmaskDiscovery, "-PM"},
            {Flag.ProtocolPing, "-PO"},
            {Flag.NeverDnsResolve, "-n"},
            {Flag.DnsServers, "--dns-servers"},
            {Flag.SystemDns, "--system-dns"},
            {Flag.Traceroute, "--traceroute"},
            {Flag.HostScan, "-sn"},
            {Flag.TcpSynScan, "-sS"},
            {Flag.ConnectScan, "-sT"},
            {Flag.AckScan, "-sA"},
            {Flag.WindowScan, "-sW"},
            {Flag.MaimonScan, "-sM"},
            {Flag.UdpScan, "-sU"},
            {Flag.TcpNullScan, "-sN"},
            {Flag.FinScan, "-sF"},
            {Flag.XmasScan, "-sX"},
            {Flag.ScanFlags, "--scanflags"},
            {Flag.IdleScan, "-sI"},
            {Flag.SctpInitScan, "-sY"},
            {Flag.CookieEchoScan, "-sZ"},
            {Flag.IpProtocolScan, "-sO"},
            {Flag.FtpBounceScan, "-b"},
            {Flag.PortSpecification, "-p"},
            {Flag.FastScanMode, "-F"},
            {Flag.ScanPortsConsecutively, "-r"},
            {Flag.TopPorts, "--top-ports"},
            {Flag.PortRatio, "--port-ratio"},
            {Flag.ServiceVersion, "-sV"},
            {Flag.VersionIntensity, "--version-intensity"},
            {Flag.VersionLight, "--version-light"},
            {Flag.VersionAll, "--version-all"},
            {Flag.VersionTrace, "--version-trace"},
            {Flag.DefaultScriptScan, "-sC"},
            {Flag.Script, "--script"},
            {Flag.ScriptArgs, "--script-args"},
            {Flag.ScriptTrace, "--script-trace"},
            {Flag.ScriptUpdateDb, "--script-updatedb"},
            {Flag.OsDetection, "-O"},
            {Flag.OsScanLimit, "--osscan-limit"},
            {Flag.OsScanGuess, "--osscan-guess"},
            {Flag.ParanoidTiming, "-T0"},
            {Flag.SneakyTiming, "-T1"},
            {Flag.PoliteTiming, "-T2"},
            {Flag.NormalTiming, "-T3"},
            {Flag.AggressiveTiming, "-T4"},
            {Flag.InsaneTiming, "-T5"},
            {Flag.ParallelMinHostGroupSize, "--min-hostgroup"},
            {Flag.ParallelMaxHostGroupSize, "--max-hostgroup"},
            {Flag.MinProbeParallelization, "--min-parallelism"},
            {Flag.MaxProbeParallelization, "--max-parallelism"},
            {Flag.MinRttTimeout, "--min-rtt-timeout"},
            {Flag.MaxRttTimeout, "--max-rtt-timeout"},
            {Flag.InitialRttTimeout, "--initial-rtt-timeout"},
            {Flag.MaxRetries, "--max-retries"},
            {Flag.HostTimeout, "--host-timeout"},
            {Flag.ScanDelay, "--scan-delay"},
            {Flag.MaxScanDelay, "--max-scan-delay"},
            {Flag.MinRate, "--min-rate"},
            {Flag.MaxRate, "--max-rate"},
            {Flag.FragmentPackets, "-f"},
            {Flag.Mtu, "--mtu"},
            {Flag.Decoy, "-D"},
            {Flag.SpoofSourceAddress, "-S"},
            {Flag.Interface, "-e"},
            {Flag.SourcePortG, "-g"},
            {Flag.SourcePort, "--source-port"},
            {Flag.DataLength, "--data-length"},
            {Flag.IpOptions, "--ip-options"},
            {Flag.TimeToLive, "--ttl"},
            {Flag.SpoofMacAddress, "--spoof-mac"},
            {Flag.BadSum, "--badsum"},
            {Flag.Adler32, "--adler32"},
            {Flag.NormalOutput, "-oN"},
            {Flag.XmlOutput, "-oX"},
            {Flag.ScriptKiddieOutput, "-oS"},
            {Flag.GreppableOutput, "-oG"},
            {Flag.AllThreeOutput, "-oA"},
            {Flag.Verbose, "-v"},
            {Flag.DebugLevel, "-d"},
            {Flag.Reason, "--reason"},
            {Flag.Open, "--open"},
            {Flag.PacketTrace, "--packet-trace"},
            {Flag.PrintHostInterfaceList, "--iflist"},
            {Flag.LogErrors, "--log-errors"},
            {Flag.AppendOutput, "--append-output"},
            {Flag.Resume, "--resume"},
            {Flag.Stylesheet, "--stylesheet"},
            {Flag.WebXml, "--webxml"},
            {Flag.NoStylesheet, "--no-stylesheet"},
            {Flag.Ipv6, "-6"},
            {Flag.A, "-A"},
            {Flag.DataDir, "--datadir"},
            {Flag.SendEth, "--send-eth"},
            {Flag.SendIp, "--send-ip"},
            {Flag.Privileged, "--privileged"},
            {Flag.Unprivileged, "--unprivileged"},
            {Flag.Version, "-V"},
            {Flag.Help, "-h"}
        };

        // Nmap option by flag
        readonly Dictionary<string, Flag> _optionByFlag = new()
        {
            {"-iL", Flag.InputFilename},
            {"-iR", Flag.RandomTargets},
            {"--exclude", Flag.ExcludeHosts},
            {"--excludefile", Flag.ExcludeFile},
            {"-sL", Flag.ListScan},
            {"-sP", Flag.PingScan},
            {"-PN", Flag.TreatHostsAsOnline},
            {"-PS", Flag.TcpSynDiscovery},
            {"-PA", Flag.AckDiscovery},
            {"-PU", Flag.UdpDiscovery},
            {"-PY", Flag.SctpDiscovery},
            {"-PE", Flag.IcmpEchoDiscovery},
            {"-PP", Flag.IcmpTimestampDiscovery},
            {"-PM", Flag.IcmpNetmaskDiscovery},
            {"-PO", Flag.ProtocolPing},
            {"-n", Flag.NeverDnsResolve},
            {"--dns-servers", Flag.DnsServers},
            {"--system-dns", Flag.SystemDns},
            {"--traceroute", Flag.Traceroute},
            {"-sn", Flag.HostScan},
            {"-sS", Flag.TcpSynScan},
            {"-sT", Flag.ConnectScan},
            {"-sA", Flag.AckScan},
            {"-sW", Flag.WindowScan},
            {"-sM", Flag.MaimonScan},
            {"-sU", Flag.UdpScan},
            {"-sN", Flag.TcpNullScan},
            {"-sF", Flag.FinScan},
            {"-sX", Flag.XmasScan},
            {"--scanflags", Flag.ScanFlags},
            {"-sI", Flag.IdleScan},
            {"-sY", Flag.SctpInitScan},
            {"-sZ", Flag.CookieEchoScan},
            {"-sO", Flag.IpProtocolScan},
            {"-b", Flag.FtpBounceScan},
            {"-p", Flag.PortSpecification},
            {"-F", Flag.FastScanMode},
            {"-r", Flag.ScanPortsConsecutively},
            {"--top-ports", Flag.TopPorts},
            {"--port-ratio", Flag.PortRatio},
            {"-sV", Flag.ServiceVersion},
            {"--version-intensity", Flag.VersionIntensity},
            {"--version-light", Flag.VersionLight},
            {"--version-all", Flag.VersionAll},
            {"--version-trace", Flag.VersionTrace},
            {"-sC", Flag.DefaultScriptScan},
            {"--script", Flag.Script},
            {"--script-args", Flag.ScriptArgs},
            {"--script-trace", Flag.ScriptTrace},
            {"--script-updatedb", Flag.ScriptUpdateDb},
            {"-O", Flag.OsDetection},
            {"--osscan-limit", Flag.OsScanLimit},
            {"--osscan-guess", Flag.OsScanGuess},
            {"-T0", Flag.ParanoidTiming},
            {"-T1", Flag.SneakyTiming},
            {"-T2", Flag.PoliteTiming},
            {"-T3", Flag.NormalTiming},
            {"-T4", Flag.AggressiveTiming},
            {"-T5", Flag.InsaneTiming},
            {"--min-hostgroup", Flag.ParallelMinHostGroupSize},
            {"--max-hostgroup", Flag.ParallelMaxHostGroupSize},
            {"--min-parallelism", Flag.MinProbeParallelization},
            {"--max-parallelism", Flag.MaxProbeParallelization},
            {"--min-rtt-timeout", Flag.MinRttTimeout},
            {"--max-rtt-timeout", Flag.MaxRttTimeout},
            {"--initial-rtt-timeout", Flag.InitialRttTimeout},
            {"--max-retries", Flag.MaxRetries},
            {"--host-timeout", Flag.HostTimeout},
            {"--scan-delay", Flag.ScanDelay},
            {"--max-scan-delay", Flag.MaxScanDelay},
            {"--min-rate", Flag.MinRate},
            {"--max-rate", Flag.MaxRate},
            {"-f", Flag.FragmentPackets},
            {"--mtu", Flag.Mtu},
            {"-D", Flag.Decoy},
            {"-S", Flag.SpoofSourceAddress},
            {"-e", Flag.Interface},
            {"-g", Flag.SourcePortG},
            {"--source-port", Flag.SourcePort},
            {"--data-length", Flag.DataLength},
            {"--ip-options", Flag.IpOptions},
            {"--ttl", Flag.TimeToLive},
            {"--spoof-mac", Flag.SpoofMacAddress},
            {"--badsum", Flag.BadSum},
            {"--adler32", Flag.Adler32},
            {"-oN", Flag.NormalOutput},
            {"-oX", Flag.XmlOutput},
            {"-oS", Flag.ScriptKiddieOutput},
            {"-oG", Flag.GreppableOutput},
            {"-oA", Flag.AllThreeOutput},
            {"-v", Flag.Verbose},
            {"-d", Flag.DebugLevel},
            {"--reason", Flag.Reason},
            {"--open", Flag.Open},
            {"--packet-trace", Flag.PacketTrace},
            {"--iflist", Flag.PrintHostInterfaceList},
            {"--log-errors", Flag.LogErrors},
            {"--append-output", Flag.AppendOutput},
            {"--resume", Flag.Resume},
            {"--stylesheet", Flag.Stylesheet},
            {"--webxml", Flag.WebXml},
            {"--no-stylesheet", Flag.NoStylesheet},
            {"-6", Flag.Ipv6},
            {"-A", Flag.A},
            {"--datadir", Flag.DataDir},
            {"--send-eth", Flag.SendEth},
            {"--send-ip", Flag.SendIp},
            {"--privileged", Flag.Privileged},
            {"--unprivileged", Flag.Unprivileged},
            {"-V", Flag.Version},
            {"-h", Flag.Help}
        };

        // Nmap options
        readonly Dictionary<string, string> _options;

        public Options()
        {
            _options = new Dictionary<string, string>();
        }

        public void Add(
            KeyValuePair<Flag, string> kvp)
            => Add(kvp.Key, kvp.Value);

        public void Clear()
            => _options.Clear();

        public bool Contains(
            KeyValuePair<Flag, string> item)
            => _options.Contains(new KeyValuePair<string, string>(_flagByOption[item.Key], item.Value));

        public void CopyTo(
            KeyValuePair<Flag, string>[] array, int arrayIndex)
            => _options.Select(d => new KeyValuePair<Flag, string>(_optionByFlag[d.Key], d.Value))
                .ToArray()
                .CopyTo(array, arrayIndex);

        public bool Remove(
            KeyValuePair<Flag, string> item)
            => _options.Remove(_flagByOption[item.Key]);

        public int Count => _options.Count;

        public bool IsReadOnly => false;

        public bool ContainsKey(
            Flag key)
            => _options.ContainsKey(_flagByOption[key]);

        public void Add(
            Flag flag, string argument)
        {
            string option = _flagByOption[flag];

            if (_options.ContainsKey(option))
                _options[option] = string.Format("{0},{1}", _options[option], argument);
            else
                _options.Add(option, argument);
        }

        public bool Remove(
            Flag key)
            => _options.Remove(_flagByOption[key]);

        public bool TryGetValue(
            Flag key, out string value)
            => _options.TryGetValue(_flagByOption[key], out value);

        public string this[Flag key]
        {
            get => _options[_flagByOption[key]];
            set => _options[_flagByOption[key]] = value;
        }

        public ICollection<Flag> Keys => _options.Select(x => _optionByFlag[x.Key]).ToArray();

        public ICollection<string> Values => _options.Values;

        public IEnumerator<KeyValuePair<Flag, string>> GetEnumerator()
            => _options.Select(x => new KeyValuePair<Flag, string>(_optionByFlag[x.Key], x.Value))
                .GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public void Add(
            Flag flag)
            => Add(flag, string.Empty);

        public void AddAll(
            IEnumerable<Flag> flags)
        {
            foreach (Flag flag in flags)
                Add(flag);
        }

        public void AddAll(
            IEnumerable<KeyValuePair<Flag, string>> kvps)
        {
            foreach (var kvp in kvps)
                Add(kvp.Key, kvp.Value);
        }

        public override string ToString()
            => _options.Aggregate(
                new StringBuilder(),
                (sb, kvp) => sb.AppendFormat("{0} {1} ", kvp.Key, kvp.Value),
                sb => sb.ToString())
            .Trim();
    }
}