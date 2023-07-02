using CS.Model.DB;
using System.Collections.Generic;
using System.Linq;

namespace CS.Model.DTO
{
    public class ScanOut
    {
        public ScanOut(
            Host host)
        {
            Ip = host.Ip;
            TypeIp = host.TypeIp;
            Name = host.Name;
            State = host.State;
            Ports = host.Ports?.Select(d => new PortOut(d))?.ToList() ?? new List<PortOut>();
            Traces = host.Traces?.Select(d => new TraceOut(d))?.ToList() ?? new List<TraceOut>();
        }

        /// <summary>
        /// Host IP
        /// </summary>
        public string? Ip { get; set; }

        /// <summary>
        /// Host Type IP
        /// </summary>
        public string? TypeIp { get; set; }

        /// <summary>
        /// Host name
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Host status
        /// </summary>
        public string? State { get; set; }

        /// <summary>
        /// Port list
        /// </summary>
        public List<PortOut> Ports { get; set; }

        /// <summary>
        /// Trace list
        /// </summary>
        public List<TraceOut> Traces { get; set; }

        public class PortOut
        {
            public PortOut(
                Port port)
            {
                Number = port.Number;
                Type = port.Type;
                Name = port.Name;
                Product = port.Product;
                Version = port.Version;
                Extrainfo = port.Extrainfo;
            }

            /// <summary>
            /// Port number
            /// </summary>
            public ushort Number { get; set; }

            /// <summary>
            /// Port type
            /// </summary>
            public string? Type { get; set; }

            /// <summary>
            /// Service name
            /// </summary>
            public string? Name { get; set; }

            /// <summary>
            /// Service product
            /// </summary>
            public string? Product { get; set; }

            /// <summary>
            /// Servic product version
            /// </summary>
            public string? Version { get; set; }

            /// <summary>
            /// External information
            /// </summary>
            public string? Extrainfo { get; set; }

            /// <summary>
            /// Status
            /// </summary>
            public string? Status { get; set; }
        }

        public class TraceOut
        {
            public TraceOut(
                Trace trace)
            {
                TTL = trace.TTL;
                Ip = trace.Ip;
                Name = trace.Name;
            }

            /// <summary>
            /// Hop
            /// </summary>
            public int TTL { get; set; }

            /// <summary>
            /// Intermediate host ip
            /// </summary>
            public string? Ip { get; set; }

            /// <summary>
            /// Intermediate host name
            /// </summary>
            public string? Name { get; set; }
        }
    }
}